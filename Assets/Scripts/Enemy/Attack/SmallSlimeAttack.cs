using Events;
using Player;
using UnityEngine;

namespace Enemy.Attack
{
    public class SmallSlimeAttack : EnemyAttack
    {
        [SerializeField] private float attackRange = 1.2f;

        public override bool CanAttack()
        {
            if (!base.CanAttack())
                return false;

            float dist = Vector2.Distance(
                transform.position,
                Target.position);

            return dist <= attackRange;
        }

        protected override void ExecuteAttack()
        {
            var player = Target.GetComponent<PlayerController>();
            if (player == null) return;

            EventBus.Publish(
                new OnPlayerDamageRequestedEvent(player, Stats.Damage));
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawSphere(transform.position, attackRange);
        }
    }
}
