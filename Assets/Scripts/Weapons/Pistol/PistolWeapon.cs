using Enemy;
using ObjectPool;
using Player;
using UnityEngine;

namespace Weapons.Pistol
{
    public class PistolWeapon : Weapon
    {
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private float bulletSpeed;
        
        private float _cooldown;
        private float _timer;
        
        public override void Init(PlayerController player, int weaponID, WeaponStats stats, EnemyManager enemyManager)
        {
            base.Init(player, weaponID, stats, enemyManager);

            _cooldown = Stats.attackSpeed;
        }
        
        protected override void Update()
        {
            base.Update();
            
            _timer += Time.deltaTime;
            if (_timer >= _cooldown)
            {
                _timer = 0;
                Attack();
            }
        }

        private void Attack()
        {
            var bulletObj = PoolManager.Instance.Spawn(
                circlePrefab,
                transform.position, 
                transform.rotation);

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            Transform enemyTransform = null;
            if (enemy != null)
                enemyTransform = enemy.transform;
            
            bulletObj.GetComponent<PistolBullet>()
                .Init( bulletSpeed, enemyTransform, Player.AimDirection, Player, Stats);
        }
    }
}