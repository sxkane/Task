using Player;

namespace Events
{
    public class OnPlayerDamageRequestedEvent : IEvent
    {
        public PlayerController Target { get; }
        public float RawDamage { get; }

        public OnPlayerDamageRequestedEvent(PlayerController target, float rawDamage)
        {
            Target = target;
            RawDamage = rawDamage;
        }
    }
}
