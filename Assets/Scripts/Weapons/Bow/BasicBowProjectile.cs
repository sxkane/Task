using Player;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Weapons.Core;
using Weapons.Projectiles;
using Weapons.Modifiers;

namespace Weapons.Bow
{
    public class BasicBowProjectile : WeaponProjectile, IWeaponProjectileLauncher
    {
        private Vector2 _direction;
        private float _speed;
        private int _remainingPierce;
        private float _damageMultiplier;
        private readonly HashSet<EnemyController> _hitEnemies = new();

        public void Launch(WeaponRuntimeContext context, Transform target, Vector2 direction, float projectileSpeed)
        {
            Init(projectileSpeed, target, direction, context.Player, context.Weapon.Stats, context.RuntimeStats);
        }

        public void Init(float moveSpeed, Transform target, Vector2 defaultDir, PlayerController player, WeaponStats stats, WeaponRuntimeStats runtimeStats)
        {
            InitializeProjectile(player, stats, runtimeStats);

            _direction = target != null
                ? ((Vector2)(target.position - transform.position)).normalized
                : defaultDir.normalized;
            _speed = moveSpeed;
            _remainingPierce = runtimeStats != null ? runtimeStats.GetPierceCount() : 0;
            _damageMultiplier = 1f;
            _hitEnemies.Clear();
        }

        private void Update()
        {
            transform.position += (Vector3)_direction * (_speed * Time.deltaTime);
            TrackTravel();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!TryHitEnemy(collision, out var enemy))
                return;

            if (_hitEnemies.Contains(enemy))
                return;

            _hitEnemies.Add(enemy);
            var isCritical = RollCriticalHit();
            var damage = Mathf.RoundToInt(CalculateDamage(isCritical) * _damageMultiplier);
            PublishDamage(enemy, damage, _direction, isCritical);

            if (_remainingPierce <= 0)
            {
                ReturnToPool();
                return;
            }

            _remainingPierce--;
            var runtimeMultiplier = RuntimeStats != null
                ? RuntimeStats.GetStat(WeaponStatType.PierceDamageMultiplier).Value
                : 1f;
            _damageMultiplier *= Mathf.Clamp01(runtimeMultiplier);
        }
    }
}
