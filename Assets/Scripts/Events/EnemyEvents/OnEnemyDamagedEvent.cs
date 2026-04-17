using Enemy;

namespace Events.EnemyEvents
{
    public class OnEnemyDamagedEvent : IEvent
    {
        public EnemyController Target { get; }
        public int FinalDamage { get; }
        public bool WasKilled { get; }

        public OnEnemyDamagedEvent(EnemyController target, int finalDamage, bool wasKilled)
        {
            Target = target;
            FinalDamage = finalDamage;
            WasKilled = wasKilled;
        }
    }
}
