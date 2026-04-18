using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;
using Weapons.Core;
using Weapons.Projectiles;

namespace Weapons.Arcane
{
    public class ApprenticeWandProjectile : WeaponProjectile, IWeaponProjectileLauncher
    {
        [SerializeField] private float turnRate = 540f;
        [SerializeField] private float retargetRadius = 12f;

        private readonly List<EnemyController> _searchResults = new();
        private readonly HashSet<EnemyController> _hitEnemies = new();
        private Transform _target;
        private Vector3 _direction;
        private float _speed;
        private int _remainingBounces;

        public void Launch(WeaponRuntimeContext context, Transform target, Vector2 direction, float projectileSpeed)
        {
            Init(projectileSpeed, target, direction, context.Player, context.Weapon.Stats, context.RuntimeStats, context.EnemyManager);
        }

        public void Init(
            float moveSpeed,
            Transform target,
            Vector2 defaultDir,
            PlayerController player,
            WeaponStats stats,
            WeaponRuntimeStats runtimeStats,
            EnemyManager enemyManager)
        {
            InitializeProjectile(player, stats, runtimeStats, enemyManager);

            _target = target;
            _direction = target != null
                ? ((Vector2)(target.position - transform.position)).normalized
                : defaultDir.normalized;
            _speed = moveSpeed;
            _remainingBounces = runtimeStats != null ? runtimeStats.GetBounceCount() : 0;
            _hitEnemies.Clear();
        }

        private void Update()
        {
            UpdateDirection();
            transform.position += _direction * (_speed * Time.deltaTime);
            transform.up = _direction;
            TrackTravel();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!TryHitEnemy(collision, out var enemy))
                return;

            if (_hitEnemies.Contains(enemy))
                return;

            _hitEnemies.Add(enemy);
            var isCritical = IsCriticalHit();
            var damage = CalculateDamage();
            PublishDamage(enemy, damage, _direction, isCritical);

            if (_remainingBounces <= 0)
            {
                ReturnToPool();
                return;
            }

            _remainingBounces--;
            _target = FindNextTarget();
            if (_target == null)
                ReturnToPool();
        }

        private void UpdateDirection()
        {
            if (_target == null || !_target.gameObject.activeInHierarchy)
                _target = FindNextTarget();

            if (_target == null)
                return;

            var desiredDirection = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            _direction = Vector3.RotateTowards(_direction, desiredDirection, turnRate * Mathf.Deg2Rad * Time.deltaTime, 0f);
        }

        private Transform FindNextTarget()
        {
            if (EnemyManager == null)
                return null;

            EnemyManager.GetEnemiesInRadius(transform.position, retargetRadius, _searchResults);

            EnemyController nearest = null;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < _searchResults.Count; i++)
            {
                var enemy = _searchResults[i];
                if (enemy == null || _hitEnemies.Contains(enemy))
                    continue;

                var distance = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest != null ? nearest.transform : null;
        }
    }
}
