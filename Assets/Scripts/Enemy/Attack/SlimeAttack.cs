using Events;
using Player;
using UnityEngine;

namespace Enemy.Attack
{
    public class SlimeAttack : EnemyAttack
    {
        [SerializeField] private float attackRange = 1.2f;

        public override bool UsesAttackState => false;
        public override bool ShouldStopMovementDuringAttack => false;

        protected override void Update()
        {
            base.Update();

            if (Target == null || Enemy == null || Enemy.Lifecycle == null || !Enemy.Lifecycle.CanDealDamage)
                return;

            if (!CanAttack())
                return;

            ExecuteAttack();
            StartCooldown();
        }

        public override bool CanAttack()
        {
            if (!base.CanAttack())
                return false;

            var dist = Vector2.Distance(transform.position, Target.position);
            return dist <= attackRange;
        }

        protected override void ExecuteAttack()
        {
            var player = Target.GetComponent<PlayerController>();
            if (player == null)
                return;

            EventBus.Publish(new OnPlayerDamageRequestedEvent(player, Stats.Damage));
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
