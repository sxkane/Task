using Enemy;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;

namespace Weapons.FireBall
{
    public class FireBallBullet : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _dir;
        
        private WeaponStats _stats;
        private float _bulletSpeed;

        [Header("Manager")]
        private PlayerController _player;
        private EnemyManager _enemyManager;

        [SerializeField] private float turnRate = 360f;
        [SerializeField] private float retargetInterval = 0.4f;

        private float _retargetTimer;
        
        public void Init(WeaponStats stats, float bulletSpeed, PlayerController player, EnemyManager enemyManager)
        {
            _stats = stats;
            _bulletSpeed =  bulletSpeed;
            _player = player;
            _enemyManager = enemyManager;
            
            var enemy = _enemyManager.GetNearestEnemy(_player.transform.position);

            if (enemy != null)
            {
                _target = enemy.transform;
                _dir = (_target.position - transform.position).normalized;
            }
            else
            {
                _dir = transform.up;
            }

            Invoke(nameof(ReturnToPool), 5f);
        }

        void Update()
        {
            UpdateTarget();
            Move();
        }

        void UpdateTarget()
        {
            if (_target != null && _target.gameObject.activeInHierarchy)
                return;

            _retargetTimer += Time.deltaTime;

            if (_retargetTimer >= retargetInterval)
            {
                _retargetTimer = 0;

                var enemy = _enemyManager.GetNearestEnemy(_player.transform.position);
                if (enemy != null)
                    _target = enemy.transform;
            }
        }

        private void Move()
        {
            if (_target != null)
            {
                Vector2 desiredDir =
                    ((Vector2)_target.position - (Vector2)transform.position).normalized;

                _dir = Vector3.RotateTowards(
                    _dir,
                    desiredDir,
                    turnRate * Mathf.Deg2Rad * Time.deltaTime,
                    0f);
            }

            transform.position += _dir * (_bulletSpeed * Time.deltaTime);
            transform.up = _dir;
        }

        void ReturnToPool()
        {
            PoolManager.Instance.Despawn(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                var playerStats = _player.Stats;
                int damage = DamageCalculator.CalculateBaseDamage(playerStats, _stats);

                if (Random.value < playerStats.CritChance + _stats.critChance)
                {
                    damage = Mathf.RoundToInt(damage * _stats.critDamage);
                }
                
                collision.GetComponent<EnemyController>().TakeDamage(damage);
                
                ReturnToPool();
            }
        }
    }
}