using Enemy;

namespace Events.EnemyEvents
{
    public class OnEnemyDamageRequestedEvent : IEvent
    {
        public EnemyController Target { get; }
        public float Damage { get; }

        public OnEnemyDamageRequestedEvent(EnemyController target, float damage)
        {
            Target = target;
            Damage = damage;
        }
    }
}
