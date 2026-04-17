using Enemy;
using Player;
using UnityEngine;
using Weapons.Effects;

namespace Weapons.FireBall
{
    public class FireBallBullet : WeaponProjectile
    {
        [Header("Movement")]
        [SerializeField] private float turnRate = 360f;
        [SerializeField] private float retargetInterval = 0.4f;

        private Transform _target;
        private Weapon _ownerWeapon;
        private Vector3 _direction;
        private float _bulletSpeed;
        private float _retargetTimer;

        public void Init(Weapon ownerWeapon, WeaponStats stats, float bulletSpeed, PlayerController player, EnemyManager enemyManager)
        {
            InitializeProjectile(player, stats, enemyManager);

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
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!TryHitEnemy(collision, out var enemy))
                return;

            var damage = CalculateDamage();
            PublishDamage(enemy, damage, _direction);

            var effectContext = EffectExecutionContext.ForWeaponHit(
                Player,
                _ownerWeapon,
                EnemyManager,
                enemy,
                transform.position);
            _ownerWeapon?.ExecuteEffects(EffectTrigger.OnWeaponHit, effectContext);
            ReturnToPool();
        }
    }
}
