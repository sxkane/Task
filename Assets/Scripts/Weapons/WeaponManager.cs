using System;
using System.Collections.Generic;
using Core;
using Enemy;
using Player;
using Stats;
using UnityEngine;
using Waves;

namespace Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        #region Inspector

        [Header("Settings")]
        [SerializeField] private float radius;
        [SerializeField] private List<WeaponSetBonusData> setBonuses = new();

        #endregion

        #region Runtime

        private GameSession _session;
        private PlayerManager _playerManager;
        private WaveManager _waveManager;
        private PlayerController _player;
        private EnemyManager _enemyManager;
        private Transform _weaponParent;
        private Transform _projectileRoot;
        private int _maxWeaponCount;

        private readonly List<Weapon> _weapons = new();
        private readonly Dictionary<WeaponTag, int> _tagCounts = new();
        private readonly HashSet<WeaponSetBonusData> _activeSetBonuses = new();

        public event Action OnLoadoutChanged;

        public IReadOnlyList<Weapon> Weapons => _weapons;

        #endregion

        public void LateUpdate()
        {
            if (_player == null)
                return;

            var playerPos = _player.transform.position;

            for (int i = 0; i < _weapons.Count; i++)
            {
                var weapon = _weapons[i];
                if (weapon == null)
                    continue;

                weapon.transform.position = playerPos + (Vector3)weapon.Offset;
            }
        }

        public void Configure(GameSession session, PlayerManager playerManager, WaveManager waveManager)
        {
            _session = session;
            _playerManager = playerManager;
            _waveManager = waveManager;
        }

        public void InitializeRun(List<WeaponEntry> initialWeapons)
        {
            ResetRun();

            _player = _playerManager != null ? _playerManager.Player : null;
            _enemyManager = _waveManager != null ? _waveManager.EnemyManager : null;
            _weaponParent = _player != null ? _player.transform : _session?.WeaponRoot;
            _projectileRoot = _session?.GetOrCreateGroupRoot(GameSessionRootType.Weapon, "Projectiles");
            _maxWeaponCount = _player != null && _player.Stats != null ? _player.Stats.MaxWeapons : 0;

            if (initialWeapons == null)
                return;

            foreach (var starterWeapon in initialWeapons)
            {
                if (starterWeapon == null || !starterWeapon.IsValid())
                    continue;

                TryAddWeapon(starterWeapon);
            }
        }

        public void ResetRun()
        {
            for (var i = 0; i < _weapons.Count; i++)
            {
                if (_weapons[i] != null)
                    Destroy(_weapons[i].gameObject);
            }

            _weapons.Clear();
            _player = null;
            _enemyManager = null;
            _weaponParent = null;
            _projectileRoot = null;
            _maxWeaponCount = 0;
            RebuildTagCounts();
            OnLoadoutChanged?.Invoke();
        }

        public void BeginPhase()
        {
            foreach (var weapon in _weapons)
                weapon.BeginPhase();
        }

        public void EndPhase()
        {
            foreach (var weapon in _weapons)
                weapon.EndPhase();
        }

        public bool TryAddWeapon(WeaponEntry weaponEntry)
        {
            if (weaponEntry == null || !weaponEntry.IsValid() || _player == null)
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

        public bool ContainsWeapon(int weaponId)
        {
            return _weapons.Exists(w => w != null && w.WeaponID == weaponId);
        }

        public Transform GetProjectileRoot()
        {
            return _projectileRoot;
        }

        private Weapon FindWeapon(WeaponEntry weaponEntry)
        {
            return _weapons.Find(w =>
                w.WeaponID == weaponEntry.GetDataId() && w.Entry != null && w.Entry.rarity == weaponEntry.rarity);
        }

        private void SpawnWeapon(WeaponEntry weaponEntry)
        {
            var weaponObject = Instantiate(weaponEntry.GetPrefab(), _weaponParent);
            var weapon = weaponObject.GetComponent<Weapon>();

            weapon.Configure(_player, weaponEntry, _enemyManager, _projectileRoot);
            weapon.InitializeRun(weaponEntry);
            _weapons.Add(weapon);

            ArrangeWeapons();
            RebuildTagCounts();
            OnLoadoutChanged?.Invoke();
        }

        private void UpgradeWeapon(Weapon weapon)
        {
            weapon.Upgrade();
            ArrangeWeapons();
            RebuildTagCounts();
            OnLoadoutChanged?.Invoke();
        }

        private void ArrangeWeapons()
        {
            var count = _weapons.Count;
            for (var i = 0; i < count; i++)
            {
                var angle = i * Mathf.PI * 2f / count;
                var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                _weapons[i].SetOffset(offset);
            }
        }

        #region Weapon Tag

        public IReadOnlyDictionary<WeaponTag, int> GetTagCounts()
        {
            return _tagCounts;
        }

        public int GetSetTier(WeaponTag weaponTag)
        {
            return _tagCounts.TryGetValue(weaponTag, out var count)
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

                for (var i = 0; i < tags.Count; i++)
                {
                    var weaponTag = tags[i];
                    if (weaponTag == WeaponTag.None)
                        continue;

                    _tagCounts.TryGetValue(weaponTag, out var value);
                    _tagCounts[weaponTag] = value + 1;
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

            for (var i = 0; i < setBonuses.Count; i++)
            {
                var setBonus = setBonuses[i];
                if (setBonus == null || !setBonus.IsValid())
                    continue;

                var count = GetSetTier(setBonus.WeaponTag);
                var tier = setBonus.ResolveActiveTier(count);
                if (tier == null || tier.modifiers == null)
                    continue;

                for (var j = 0; j < tier.modifiers.Count; j++)
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

                for (var statIndex = 0; statIndex <= (int)StatType.Harvesting; statIndex++)
                {
                    var statType = (StatType)statIndex;
                    _player.Stats.GetStat(statType).RemoveModifiersFromSource(setBonus);
                }
            }

            _activeSetBonuses.Clear();
        }

        #endregion
    }
}
