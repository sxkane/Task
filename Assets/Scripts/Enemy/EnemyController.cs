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

        public void Initialize(Transform target, EnemyManager enemyManager)
        {
            Target = target;
            _enemyManager = enemyManager;
            _enemyManager.Register(this);
        }

        public void TakeDamage(float damage)
        {
            Stats.TakeDamage(damage);

            if (!Stats.IsAlive)
            {
                _enemyManager.Unregister(this);
                Die();
            }
        }

        private void Die()
        {
            // TODO: 播放死亡特效 / 掉落经验

            Destroy(gameObject);
        }
    }
}