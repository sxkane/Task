using Enemy;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;
using Weapons.Effects;

namespace Weapons.FireBall
{
    public class FireBallBullet : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float turnRate = 360f;
        [SerializeField] private float retargetInterval = 0.4f;
        [SerializeField] private float lifetime = 5f;

        private Transform _target;
        private Weapon _ownerWeapon;
        private Vector3 _dir;
        private WeaponStats _stats;
        private float _bulletSpeed;
        private float _retargetTimer;

        [Header("Runtime References")]
        private PlayerController _player;
        private EnemyManager _enemyManager;

        public void Init(Weapon ownerWeapon, WeaponStats stats, float bulletSpeed, PlayerController player, EnemyManager enemyManager)
        {
            CancelInvoke();

            _ownerWeapon = ownerWeapon;
            _stats = stats;
            _bulletSpeed = bulletSpeed;
            _player = player;
            _enemyManager = enemyManager;
            _target = null;
            _retargetTimer = 0f;

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

            Invoke(nameof(ReturnToPool), lifetime);
        }

        private void OnDisable()
        {
            CancelInvoke();
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

            if (_retargetTimer < retargetInterval)
                return;

            _retargetTimer = 0;

            var enemy = _enemyManager.GetNearestEnemy(_player.transform.position);
            if (enemy != null)
                _target = enemy.transform;
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

            var effectContext = EffectExecutionContext.ForWeaponHit(
                _player,
                _ownerWeapon,
                _enemyManager,
                enemy,
                transform.position);
            _ownerWeapon?.ExecuteEffects(EffectTrigger.OnWeaponHit, effectContext);
            ReturnToPool();
        }
    }
}
