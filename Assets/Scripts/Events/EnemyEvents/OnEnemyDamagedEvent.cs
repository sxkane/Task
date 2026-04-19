using Enemy;

namespace Events.EnemyEvents
{
    public class OnEnemyDamagedEvent : IEvent
    {
        public EnemyController Target { get; }
        public int FinalDamage { get; }
        public bool WasKilled { get; }
        public bool IsCritical { get; }
        public Weapons.Weapon SourceWeapon { get; }

        public OnEnemyDamagedEvent(EnemyController target, int finalDamage, bool wasKilled, bool isCritical, Weapons.Weapon sourceWeapon = null)
        {
            Target = target;
            FinalDamage = finalDamage;
            WasKilled = wasKilled;
            IsCritical = isCritical;
            SourceWeapon = sourceWeapon;
        }
    }
}
