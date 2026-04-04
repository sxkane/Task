using Enemy;

namespace Events.EnemyEvents
{
    public class OnEnemyDiedEvent : IEvent
    {
        public EnemyController Target { get; }

        public OnEnemyDiedEvent(EnemyController target)
        {
            Target = target;
        }
    }
}
