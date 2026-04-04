using Player;

namespace Events.PlayerEvents
{
    public class OnPlayerDamagedEvent : IEvent
    {
        public PlayerController Target { get; }
        public int FinalDamage { get; }
        public bool IsDodged { get; }

        public OnPlayerDamagedEvent(PlayerController target, int finalDamage, bool isDodged)
        {
            Target = target;
            FinalDamage = finalDamage;
            IsDodged = isDodged;
        }
    }
}
