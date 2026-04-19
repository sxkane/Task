using Enemy;

namespace Events.EnemyEvents
{
    public class OnEnemyDiedEvent : IEvent
    {
        public EnemyController Target { get; }
        public Weapons.Weapon SourceWeapon { get; }

        public OnEnemyDiedEvent(EnemyController target, Weapons.Weapon sourceWeapon = null)
        {
            Target = target;
            SourceWeapon = sourceWeapon;
        }
    }
}
