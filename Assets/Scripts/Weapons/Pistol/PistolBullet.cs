using Player;
using UnityEngine;

namespace Weapons.Pistol
{
    public class PistolBullet : WeaponProjectile
    {
        private Vector2 _direction;
        private float _speed;

        public void Init(float moveSpeed, Transform target, Vector2 defaultDir, PlayerController player, WeaponStats stats)
        {
            InitializeProjectile(player, stats);

            _direction = target != null
                ? ((Vector2)(target.position - transform.position)).normalized
                : defaultDir.normalized;
            _speed = moveSpeed;
        }

        private void Update()
        {
            transform.position += (Vector3)_direction * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!TryHitEnemy(collision, out var enemy))
                return;

            var damage = CalculateDamage();
            PublishDamage(enemy, damage, _direction);
            ReturnToPool();
        }
    }
}
