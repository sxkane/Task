using Enemy;
using Player;
using UnityEngine;

namespace Weapons
{
    public abstract class CooldownWeapon : Weapon
    {
        private float _cooldown;
        private float _timer;

        public override void Configure(PlayerController player, WeaponEntry entry, EnemyManager enemyManager, Transform projectileRoot)
        {
            base.Configure(player, entry, enemyManager, projectileRoot);
            ResetCooldown();
        }

        public override void InitializeRun(WeaponEntry entry = null)
        {
            base.InitializeRun(entry);
            _timer = 0f;
            ResetCooldown();
        }

        protected override void Update()
        {
            base.Update();

            if (Stats == null)
                return;

            _timer += Time.deltaTime;
            if (_timer < _cooldown)
                return;

            _timer = 0f;
            Attack();
        }

        protected abstract void Attack();

        protected void ResetCooldown()
        {
            _cooldown = RuntimeStats != null ? RuntimeStats.GetAttackInterval(Player?.Stats) : 0f;
        }
    }
}
