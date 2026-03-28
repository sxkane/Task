using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttack : EnemyAttack
    {
        [SerializeField] private float attackRange = 1.2f;

        protected override bool CanAttack()
        {
            float dist = Vector2.Distance(
                transform.position,
                Target.position);

            return dist <= attackRange;
        }

        protected override void Attack()
        {
            var player = Target.GetComponent<Player.PlayerController>();

            if (player == null) return;

            // TODO: 玩家受伤系统
            Debug.Log("Player hit by melee enemy!");

            // player.Health.TakeDamage(stats.Damage);
        }
    }
}