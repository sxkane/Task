using Enemy;
using ObjectPool;
using Player;
using UnityEngine;
using Weapons.Effects;

namespace Weapons.Pistol
{
    public class PistolWeapon : Weapon
    {
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private float bulletSpeed;
        
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
            ExecuteEffects(EffectTrigger.OnWeaponAttack);

            var bulletObj = PoolManager.Instance.Spawn(
                circlePrefab,
                transform.position, 
                transform.rotation,
                ProjectileRoot);

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            Transform enemyTransform = null;
            if (enemy != null)
                enemyTransform = enemy.transform;
            
            bulletObj.GetComponent<PistolBullet>()
                .Init( bulletSpeed, enemyTransform, Player.AimDirection, Player, Stats);
        }
    }
}
