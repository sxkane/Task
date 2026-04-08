using UnityEngine;

namespace Enemy.Attack
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        private static readonly int AttackString = Animator.StringToHash("Attack");
        
        protected EnemyController Enemy;
        protected EnemyStats Stats;
        protected Transform Target;

        private float _cooldown;
        private bool _isAttacking;

        protected virtual void Update()
        {
            _cooldown -= Time.deltaTime;
        }

        public virtual bool CanAttack()
        {
            return Stats.IsAlive && _cooldown <= 0f && !_isAttacking;
        }

        // 视觉
        public void StartAttack()
        {
            if (!CanAttack())
                return;

            _isAttacking = true;
            Enemy.Animator.SetTrigger(AttackString);
        }
        
        public void EndAttack()
        {
            _cooldown = Stats.AttackInterval;
            _isAttacking = false;
        }
        
        // 逻辑
        public void DoAttack()
        {
            ExecuteAttack();
        }

        protected abstract void ExecuteAttack();

        public void Initialize(EnemyController enemy)
        {
            Enemy = enemy;
            Stats = Enemy.Stats;
            Target = Enemy.Target;
        }
    }
}