using Enemy;
using Player;
using UnityEngine;
using Weapons.Core;
using Weapons.Projectiles;

namespace Weapons.FireBall
{
    public class FireBallBullet : WeaponProjectile, IWeaponProjectileLauncher
    {
        [Header("Movement")]
        [SerializeField] private float turnRate = 360f;
        [SerializeField] private float retargetInterval = 0.4f;

        private Transform _target;
        private Weapon _ownerWeapon;
        private Vector3 _direction;
        private float _bulletSpeed;
        private float _retargetTimer;

        public void Launch(WeaponRuntimeContext context, Transform target, Vector2 direction, float projectileSpeed)
        {
            Init(context.Weapon, context.Weapon.Stats, context.RuntimeStats, projectileSpeed, context.Player, context.EnemyManager);
        }

        public void Init(Weapon ownerWeapon, WeaponStats stats, WeaponRuntimeStats runtimeStats, float bulletSpeed, PlayerController player, EnemyManager enemyManager)
        {
            InitializeProjectile(player, stats, runtimeStats, enemyManager);

            _ownerWeapon = ownerWeapon;
            _bulletSpeed = bulletSpeed;
            _target = null;
            _retargetTimer = 0f;

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            if (enemy != null)
            {
                _target = enemy.transform;
                _direction = (_target.position - transform.position).normalized;
            }
            else
            {
                _direction = transform.up;
            }
        }

        private void Update()
        {
            UpdateTarget();
            Move();
        }

        private void UpdateTarget()
        {
            if (_target != null && _target.gameObject.activeInHierarchy)
                return;

            _retargetTimer += Time.deltaTime;
            if (_retargetTimer < retargetInterval || EnemyManager == null || Player == null)
                return;

            _retargetTimer = 0f;
            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            if (enemy != null)
                _target = enemy.transform;
        }

        private void Move()
        {
            if (_target != null)
            {
                var desiredDirection = ((Vector2)_target.position - (Vector2)transform.position).normalized;
                _direction = Vector3.RotateTowards(
                    _direction,
                    desiredDirection,
                    turnRate * Mathf.Deg2Rad * Time.deltaTime,
                    0f);
            }

            transform.position += _direction * (_bulletSpeed * Time.deltaTime);
            transform.up = _direction;
            TrackTravel();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!TryHitEnemy(collision, out var enemy))
                return;

            var isCritical = IsCriticalHit();
            var damage = CalculateDamage();
            PublishDamage(enemy, damage, _direction, isCritical);
            _ownerWeapon?.NotifyProjectileHit(enemy, transform.position);
            ReturnToPool();
        }
    }
}
