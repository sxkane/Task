using Enemy;

namespace Events.EnemyEvents
{
    public class OnEnemyDamagedEvent : IEvent
    {
        public EnemyController Target { get; }
        public int FinalDamage { get; }

        public OnEnemyDamagedEvent(EnemyController target, int finalDamage)
        {
            Target = target;
            FinalDamage = finalDamage;
        }
    }
}
