using UnityEngine;

namespace Enemy
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        protected EnemyController Enemy;
        protected EnemyStats Stats;
        protected Transform Target;

        private float _cooldown;

        protected virtual void Awake()
        {
            Enemy = GetComponent<EnemyController>();
        }

        protected virtual void Start()
        {
            Stats = Enemy.Stats;
            Target = Enemy.Target;
        }

        protected virtual void Update()
        {
            if (Target == null || !Stats.IsAlive)
                return;

            _cooldown -= Time.deltaTime;

            if (_cooldown <= 0f)
            {
                if (CanAttack())
                {
                    Attack();
                    _cooldown = Stats.AttackInterval;
                }
            }
        }

        protected abstract bool CanAttack();
        protected abstract void Attack();
    }
}