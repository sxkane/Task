using Enemy;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;

namespace Weapons.Pistol
{
    public class PistolBullet : MonoBehaviour
    {
        [SerializeField] private float lifetime = 3f;

        private Vector2 _dir;
        private float _speed;
        private PlayerController _player;
        private WeaponStats _stats;
        
        public void Init(float moveSpeed, Transform target, Vector2 defaultDir,
            PlayerController player, WeaponStats stats)
        {
            CancelInvoke();

            _dir = target != null
                ? ((Vector2)(target.position - transform.position)).normalized
                : defaultDir.normalized;

            _speed = moveSpeed;
            _player = player;
            _stats = stats;
            
            Invoke(nameof(ReturnToPool), lifetime);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        private void Update()
        {
            transform.position += (Vector3)_dir * (_speed * Time.deltaTime);
        }
        
        private void ReturnToPool()
        {
            PoolManager.Instance.Despawn(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<EnemyController>();
            if (enemy == null)
                return;

            var playerStats = _player.Stats;
            int damage = DamageCalculator.CalculateBaseDamage(playerStats, _stats);

            if (Random.value < playerStats.CritChance + _stats.critChance)
                damage = Mathf.RoundToInt(damage * _stats.critDamage);
            
            EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, damage));
            ReturnToPool();
        }
    }
}
