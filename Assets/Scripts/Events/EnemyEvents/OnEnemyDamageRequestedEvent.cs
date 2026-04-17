using Enemy;
using UnityEngine;

namespace Events.EnemyEvents
{
    public class OnEnemyDamageRequestedEvent : IEvent
    {
        public EnemyController Target { get; }
        public float Damage { get; }
        public Vector2 KnockbackDirection { get; }
        public float KnockbackForce { get; }

        public OnEnemyDamageRequestedEvent(
            EnemyController target,
            float damage,
            Vector2 knockbackDirection = default,
            float knockbackForce = 0f)
        {
            Target = target;
            Damage = damage;
            KnockbackDirection = knockbackDirection;
            KnockbackForce = knockbackForce;
        }
    }
}
