using Enemy;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;
using Weapons.Effects;

namespace Weapons.FireBall
{
    public class FireBallWeapon : Weapon
    {
        [SerializeField] private GameObject fireBallPrefab;
        [SerializeField] private float bulletSpeed = 4;
        [SerializeField] private Transform firePoint;
        
        private float _cooldown;
        private float _timer;

        public override void Configure(PlayerController player, WeaponEntry entry, EnemyManager enemyManager, Transform projectileRoot)
        {
            base.Configure(player, entry, enemyManager, projectileRoot);
            _cooldown = Stats.attackSpeed;
        }

        public override void InitializeRun(WeaponEntry entry = null)
        {
            base.InitializeRun(entry);
            _timer = 0f;
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
            var target = EnemyManager.GetNearestEnemy(Player.transform.position);

            if (target != null)
            {
                Vector2 dir = (target.transform.position - transform.position).normalized;

                // Vector3 newDir = Vector3.RotateTowards(
                //     transform.right,
                //     dir,
                //     rotateSpeed * Mathf.Deg2Rad * Time.deltaTime,
                //     0f);

                transform.right = dir;
            }

            ExecuteEffects(EffectTrigger.OnWeaponAttack);

            var bulletObj = PoolManager.Instance.Spawn(
                fireBallPrefab,
                firePoint.position, 
                firePoint.rotation,
                ProjectileRoot);

            bulletObj.GetComponent<FireBallBullet>()
                .Init(this, Stats, bulletSpeed, Player, EnemyManager);
        }
    }
}
