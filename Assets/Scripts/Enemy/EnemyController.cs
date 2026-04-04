using Events;
using Events.EnemyEvents;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public Transform Target { get; private set; }
        public EnemyStats Stats { get; private set; }

        private EnemyManager _enemyManager;

        [SerializeField] private EnemyStatTemplate template;

        private void Awake()
        {
            Stats = new EnemyStats();
            Stats.Init(template);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnEnemyDamageRequestedEvent>(OnDamageRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnEnemyDamageRequestedEvent>(OnDamageRequested);
        }

        public void Initialize(Transform target, EnemyManager enemyManager)
        {
            Target = target;
            _enemyManager = enemyManager;
            _enemyManager.Register(this);
        }

        private void OnDamageRequested(OnEnemyDamageRequestedEvent e)
        {
            if (e.Target != this || !Stats.IsAlive)
                return;

            Stats.TakeDamage(e.Damage);
            EventBus.Publish(new OnEnemyDamagedEvent(this, Mathf.RoundToInt(e.Damage)));

            if (!Stats.IsAlive)
                Die();
        }

        public void TakeDamage(float damage)
        {
            EventBus.Publish(new OnEnemyDamageRequestedEvent(this, damage));
        }

        private void Die()
        {
            // TODO: 播放死亡特效 / 掉落经验

            _enemyManager?.Unregister(this);
            EventBus.Publish(new OnEnemyDiedEvent(this));
            Destroy(gameObject);
        }
    }
}
