using Enemy;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;

namespace Weapons.FireBall
{
    public class FireBallWeapon : Weapon
    {
        [SerializeField] private GameObject trianglePrefab;
        [SerializeField] private float bulletSpeed = 4;
        
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
                trianglePrefab,
                transform.position, 
                transform.rotation);
            bulletObj.GetComponent<FireBallBullet>()
                .Init(Stats, bulletSpeed, Player, EnemyManager);
        }
    }
}