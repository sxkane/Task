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

        public override void Configure(PlayerController player, WeaponLoadoutEntry entry, EnemyManager enemyManager)
        {
            base.Configure(player, entry, enemyManager);
            _cooldown = Stats.attackSpeed;
        }

        public override void InitializeRun(WeaponLoadoutEntry runtimeEntry = null)
        {
            base.InitializeRun(runtimeEntry);
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
            ExecuteEffects(EffectTrigger.OnWeaponAttack);

            var bulletObj = PoolManager.Instance.Spawn(
                trianglePrefab,
                transform.position, 
                transform.rotation);
            bulletObj.GetComponent<FireBallBullet>()
                .Init(this, Stats, bulletSpeed, Player, EnemyManager);
        }
    }
}
