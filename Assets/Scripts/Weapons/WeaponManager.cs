using System.Collections.Generic;
using System;
using Enemy;
using Player;
using Stats;
using UnityEngine;
using Waves;

namespace Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        public static WeaponManager Instance;
        
        [SerializeField] private float radius;
        [SerializeField] private List<WeaponSetBonusData> setBonuses = new();
        private Transform _weaponParent;

        private PlayerController _player;
        private EnemyManager _enemyManager;

        private int _maxWeaponCount;
        private readonly List<Weapon> _weapons = new();
        private readonly Dictionary<WeaponTag, int> _tagCounts = new();
        private readonly HashSet<WeaponSetBonusData> _activeSetBonuses = new();
        
        public event Action OnLoadoutChanged;

        public IReadOnlyList<Weapon> Weapons => _weapons;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool TryAddWeapon(WeaponLoadoutEntry weaponEntry)
        {
            if (weaponEntry == null || !weaponEntry.IsValid())
                return false;

            var sameWeapon = FindWeapon(weaponEntry);

            if (_weapons.Count >= _maxWeaponCount)
            {
                if (sameWeapon != null && sameWeapon.CanUpgrade())
                {
                    UpgradeWeapon(sameWeapon);
                    return true;
                }

                return false;
            }

            SpawnWeapon(weaponEntry);
            return true;
        }

        public bool TryAddWeapon(WeaponData weaponData, Rarity rarity)
        {
            if (weaponData == null)
                return false;

            return TryAddWeapon(weaponData.CreateEntry(rarity));
        }
        
        private Weapon FindWeapon(WeaponLoadoutEntry weaponEntry)
        {
            return _weapons.Find(w => w.WeaponID == weaponEntry.GetDataId() && w.Entry != null && w.Entry.rarity == weaponEntry.rarity);
        }
        
        private void SpawnWeapon(WeaponLoadoutEntry weaponEntry)
        {
            var obj = Instantiate(weaponEntry.GetPrefab(), _weaponParent);
            var weapon = obj.GetComponent<Weapon>();
            
            weapon.Configure(_player, weaponEntry, _enemyManager);
            weapon.InitializeRun(weaponEntry);
            _weapons.Add(weapon);

            ArrangeWeapons();
            RebuildTagCounts();
            OnLoadoutChanged?.Invoke();
        }

        public void UpgradeWeapon(Weapon weapon)
        {
            weapon.Upgrade();
            ArrangeWeapons();
            RebuildTagCounts();
            OnLoadoutChanged?.Invoke();
        }

        private void ArrangeWeapons()
        {
            int count = _weapons.Count;

            for (int i = 0; i < count; i++)
            {
                float angle = i * Mathf.PI * 2f / count;

                Vector2 offset = new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)) * radius;

                _weapons[i].SetOffset(offset);
            }
        }

        public void Configure(PlayerManager playerManager, WaveManager waveManager)
        {
            _player = playerManager.Player;
            _enemyManager = waveManager.EnemyManager;
            _maxWeaponCount = _player.Stats.MaxWeapons;
            _weaponParent = _player.transform;
        }

        public void InitializeRun(List<WeaponLoadoutEntry> initialWeapons)
        {
            ResetRun();

            if (initialWeapons == null)
                return;
            
            foreach (var starterWeapon in initialWeapons)
            {
                if (starterWeapon == null || !starterWeapon.IsValid())
                    continue;

                TryAddWeapon(starterWeapon);
            }
        }

        public void InitializeRun(List<WeaponSelectionEntry> initialWeapons)
        {
            ResetRun();
            if (initialWeapons == null)
                return;

            for (int i = 0; i < initialWeapons.Count; i++)
            {
                var starterWeapon = initialWeapons[i];
                if (starterWeapon == null || !starterWeapon.IsValid())
                    continue;

                TryAddWeapon(starterWeapon);
            }
        }

        public void ResetRun()
        {
            for (int i = 0; i < _weapons.Count; i++)
            {
                if (_weapons[i] != null)
                    Destroy(_weapons[i].gameObject);
            }

            _weapons.Clear();
            RebuildTagCounts();
            OnLoadoutChanged?.Invoke();
        }

        public void BeginPhase()
        {
            foreach (var w in _weapons)
                w.BeginPhase();
        }

        public void EndPhase()
        {
            foreach (var w in _weapons)
                w.EndPhase();
        }

        public IReadOnlyDictionary<WeaponTag, int> GetTagCounts()
        {
            return _tagCounts;
        }

        public bool ContainsWeapon(int weaponId)
        {
            return _weapons.Exists(w => w != null && w.WeaponID == weaponId);
        }

        public int GetSetTier(WeaponTag tag)
        {
            return _tagCounts.TryGetValue(tag, out var count)
                ? Mathf.Clamp(count, 0, 6)
                : 0;
        }

        private void RebuildTagCounts()
        {
            _tagCounts.Clear();

            foreach (var weapon in _weapons)
            {
                var tags = weapon?.Entry?.weaponData?.GetTags();
                if (tags == null)
                    continue;

                for (int i = 0; i < tags.Count; i++)
                {
                    var tag = tags[i];
                    if (tag == WeaponTag.None)
                        continue;

                    _tagCounts.TryGetValue(tag, out var value);
                    _tagCounts[tag] = value + 1;
                }
            }

            ApplySetBonuses();
        }

        private void ApplySetBonuses()
        {
            if (_player == null || _player.Stats == null)
                return;

            ClearActiveSetBonuses();

            if (setBonuses == null)
                return;

            for (int i = 0; i < setBonuses.Count; i++)
            {
                var setBonus = setBonuses[i];
                if (setBonus == null || !setBonus.IsValid())
                    continue;

                int count = GetSetTier(setBonus.WeaponTag);
                var tier = setBonus.ResolveActiveTier(count);
                if (tier == null || tier.modifiers == null)
                    continue;

                for (int j = 0; j < tier.modifiers.Count; j++)
                {
                    var mod = tier.modifiers[j];
                    var stat = _player.Stats.GetStat(mod.statType);
                    stat.AddModifier(new Modifier(mod.value, mod.modType, setBonus));
                }

                _activeSetBonuses.Add(setBonus);
            }
        }

        private void ClearActiveSetBonuses()
        {
            if (_activeSetBonuses.Count == 0 || _player == null || _player.Stats == null)
                return;

            foreach (var setBonus in _activeSetBonuses)
            {
                if (setBonus == null)
                    continue;
                
                for (int statIndex = 0; statIndex <= (int)StatType.Harvesting; statIndex++)
                {
                    var statType = (StatType)statIndex;
                    _player.Stats.GetStat(statType).RemoveModifiersFromSource(setBonus);
                }
            }

            _activeSetBonuses.Clear();
        }

        // Legacy wrappers.
        public void Initialize(PlayerManager playerManager, WaveManager waveManager, List<WeaponLoadoutEntry> initialWeapons)
        {
            Configure(playerManager, waveManager);
            InitializeRun(initialWeapons);
        }

        public void Activate() => BeginPhase();
        public void Deactivate() => EndPhase();
    }
}
