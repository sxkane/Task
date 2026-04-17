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

        public virtual bool UsesAttackState => true;
        public virtual bool ShouldStopMovementDuringAttack => UsesAttackState;
        public virtual bool IgnoreSteering => false;

        protected virtual void Update()
        {
            _cooldown -= Time.deltaTime;
        }

        public virtual bool CanAttack()
        {
            return Stats.IsAlive && _cooldown <= 0f && !_isAttacking;
        }

        public void StartAttack()
        {
            if (!CanAttack())
                return;

            _isAttacking = true;
            StartCooldown();
            Enemy.Animator.SetTrigger(AttackString);
        }

        public void EndAttack()
        {
            _isAttacking = false;
        }

        public void CancelAttack()
        {
            _isAttacking = false;

            if (Enemy != null && Enemy.Animator != null)
                Enemy.Animator.ResetTrigger(AttackString);
        }

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
            OnInitialized();
        }

        protected void StartCooldown()
        {
            _cooldown = Stats.AttackInterval;
        }

        public virtual Vector2 GetMovementDirection(Vector2 currentPosition, Vector2 targetPosition)
        {
            return (targetPosition - currentPosition).normalized;
        }

        public virtual float GetMovementSpeedMultiplier(float distanceToTarget)
        {
            return 1f;
        }

        protected virtual void OnInitialized()
        {
        }
    }
}
