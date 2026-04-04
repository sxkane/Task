using System.Collections.Generic;
using Enemy;
using Events;
using Player;
using Rewards.Shops;
using UnityEngine;
using Waves;

namespace Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        public static WeaponManager Instance;
        
        [SerializeField] private float radius;
        private Transform _weaponParent;

        private PlayerController _player;
        private EnemyManager _enemyManager;

        private int _maxWeaponCount;
        private readonly List<Weapon> _weapons = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool TryAddWeapon(WeaponData weaponData, Rarity rarity)
        {
            var sameWeapon = FindWeapon(weaponData.weaponID, rarity);

            if (_weapons.Count >= _maxWeaponCount)
            {
                if (sameWeapon != null && sameWeapon.CanUpgrade())
                {
                    UpgradeWeapon(sameWeapon, weaponData);
                    return true;
                }

                return false;
            }

            SpawnWeapon(weaponData, rarity);
            return true;
        }
        
        private Weapon FindWeapon(int id, Rarity rarity)
        {
            return _weapons.Find(w => w.WeaponID == id && w.Stats.rarity == rarity);
        }
        
        private void SpawnWeapon(WeaponData data, Rarity rarity)
        {
            var obj = Instantiate(data.weaponPrefab, _weaponParent);
            var weapon = obj.GetComponent<Weapon>();
            var stats = data.GetStats(rarity);
            
            weapon.Init(_player, data.weaponID, stats, _enemyManager);
            _weapons.Add(weapon);

            ArrangeWeapons();
        }

        public void UpgradeWeapon(Weapon weapon, WeaponData data)
        {
            weapon.Upgrade(data);
            ArrangeWeapons();
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

        public void Initialize(PlayerManager playerManager, WaveManager waveManager, List<WeaponData> initialWeapons)
        {
            _player = playerManager.Player;
            _enemyManager = waveManager.EnemyManager;
            _maxWeaponCount = _player.Stats.MaxWeapons;
            _weaponParent = _player.transform;
            
            foreach (var w in initialWeapons)
                TryAddWeapon(w, Rarity.Common);
        }

        public void Activate()
        {
            foreach (var w in _weapons)
                w.Activate();
        }

        public void Deactivate()
        {
            foreach (var w in _weapons)
                w.Deactivate();
        }
    }
}
