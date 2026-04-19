using System;
using System.Collections.Generic;
using Core;
using Enemy;
using Events;
using Events.WeaponEvents;
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

        private List<Weapon> _weapons = new();
        private readonly Dictionary<WeaponSetBonusData, int> _setBonusCounts = new();
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

                weapon.transform.position = playerPos + (Vector3)(weapon.Offset + weapon.RuntimeOffset);
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
            _weaponParent = _session.WeaponRoot == null ? _player?.transform : _session.WeaponRoot;
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
            EventBus.Publish(new OnWeaponChanged(_setBonusCounts, _weapons));
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

        private bool TryUpgradeWeapon(WeaponEntry weaponEntry, int currentSlot)
        {
            if (weaponEntry == null || !weaponEntry.IsValid() || _player == null)
                return false;

            for (int i = _weapons.Count - 1; i >= 0; i--)
            {
                if (i == currentSlot || _weapons[i] == null || _weapons[i].Entry == null)
                    continue;

                var candidate = _weapons[i].Entry;
                if (candidate.GetDataId() == weaponEntry.GetDataId() && candidate.GetRarity() == weaponEntry.GetRarity())
                {
                    UpgradeWeapon(_weapons[currentSlot]);
                    var duplicate = _weapons[i];
                    _weapons.RemoveAt(i);
                    if (duplicate != null)
                        Destroy(duplicate.gameObject);
                    return true;
                }
            }

            return false;
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
            EventBus.Publish(new OnWeaponChanged(_setBonusCounts, _weapons));
            OnLoadoutChanged?.Invoke();
        }

        private void UpgradeWeapon(Weapon weapon)
        {
            weapon.Upgrade();
            ArrangeWeapons();
            RebuildTagCounts();
            EventBus.Publish(new OnWeaponChanged(_setBonusCounts, _weapons));
            OnLoadoutChanged?.Invoke();
        }

        public bool TryUpgradeWeapon(Weapon weapon)
        {
            if (weapon == null)
                return false;

            var currentSlot = _weapons.IndexOf(weapon);
            if (currentSlot < 0 || !weapon.CanUpgrade())
                return false;

            var upgraded = TryUpgradeWeapon(weapon.Entry, currentSlot);
            if (!upgraded)
                return false;

            ArrangeWeapons();
            RebuildTagCounts();
            EventBus.Publish(new OnWeaponChanged(_setBonusCounts, _weapons));
            OnLoadoutChanged?.Invoke();
            return true;
        }

        public bool CanUpgradeWeapon(Weapon weapon)
        {
            if (weapon == null)
                return false;

            var currentSlot = _weapons.IndexOf(weapon);
            if (currentSlot < 0 || !weapon.CanUpgrade() || weapon.Entry == null)
                return false;

            for (var i = 0; i < _weapons.Count; i++)
            {
                if (i == currentSlot || _weapons[i] == null || _weapons[i].Entry == null)
                    continue;

                var candidate = _weapons[i].Entry;
                if (candidate.GetDataId() == weapon.Entry.GetDataId() && candidate.GetRarity() == weapon.Entry.GetRarity())
                    return true;
            }

            return false;
        }

        public bool TrySellWeapon(Weapon weapon, out int refund)
        {
            refund = 0;
            if (weapon == null || !_weapons.Contains(weapon))
                return false;

            refund = weapon.Entry != null ? weapon.Entry.GetRecyclePrice() : 0;

            _weapons.Remove(weapon);
            if (weapon != null)
                Destroy(weapon.gameObject);

            ArrangeWeapons();
            RebuildTagCounts();
            EventBus.Publish(new OnWeaponChanged(_setBonusCounts, _weapons));
            OnLoadoutChanged?.Invoke();
            return true;
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

        public int GetSetTier(WeaponSetBonusData setBonusData)
        {
            return _setBonusCounts.TryGetValue(setBonusData, out var count)
                ? Mathf.Clamp(count, 0, 6)
                : 0;
        }

        private void RebuildTagCounts()
        {
            _setBonusCounts.Clear();

            foreach (var weapon in _weapons)
            {
                var tags = weapon?.Entry?.weaponData?.bonusData;
                if (tags == null)
                    continue;

                for (var i = 0; i < tags.Count; i++)
                {
                    var weaponTag = tags[i];
                    if (weaponTag == null)
                        continue;

                    _setBonusCounts.TryGetValue(weaponTag, out var value);
                    _setBonusCounts[weaponTag] = value + 1;
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

                var count = GetSetTier(setBonus);
                var tier = setBonus.ResolveActiveTier(count);
                if (tier == null)
                    continue;

                if (tier.playerModifiers != null)
                {
                    for (var j = 0; j < tier.playerModifiers.Count; j++)
                    {
                        var mod = tier.playerModifiers[j];
                        var stat = _player.Stats.GetStat(mod.statType);
                        stat.AddModifier(StatValueUtility.CreatePlayerModifier(
                            mod.statType,
                            mod.value,
                            mod.modType,
                            setBonus));
                    }
                }

                if (tier.weaponModifiers != null)
                {
                    for (var j = 0; j < _weapons.Count; j++)
                    {
                        var weapon = _weapons[j];
                        if (weapon == null || weapon.Entry?.weaponData == null || weapon.RuntimeStats == null)
                            continue;

                        var allBonusData = weapon.Entry.weaponData.GetSetBonusData();
                        if (allBonusData == null || !HasTag(allBonusData, setBonus))
                            continue;

                        for (var modifierIndex = 0; modifierIndex < tier.weaponModifiers.Count; modifierIndex++)
                        {
                            var modifier = tier.weaponModifiers[modifierIndex];
                            weapon.RuntimeStats.GetStat(modifier.statType)
                                .AddModifier(StatValueUtility.CreateWeaponModifier(
                                    modifier.statType,
                                    modifier.value,
                                    modifier.modType,
                                    setBonus));
                        }
                    }
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

                foreach (StatType statType in Enum.GetValues(typeof(StatType)))
                {
                    _player.Stats.GetStat(statType).RemoveModifiersFromSource(setBonus);
                }

                for (var weaponIndex = 0; weaponIndex < _weapons.Count; weaponIndex++)
                {
                    var weapon = _weapons[weaponIndex];
                    if (weapon?.RuntimeStats == null)
                        continue;

                    weapon.RuntimeStats.RemoveModifiersFromSource(setBonus);
                }
            }

            _activeSetBonuses.Clear();
        }

        private static bool HasTag(IReadOnlyList<WeaponSetBonusData> weaponSetBonusData, WeaponSetBonusData bonusData)
        {
            if (weaponSetBonusData == null)
                return false;

            for (var i = 0; i < weaponSetBonusData.Count; i++)
            {
                if (weaponSetBonusData[i] == bonusData)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
