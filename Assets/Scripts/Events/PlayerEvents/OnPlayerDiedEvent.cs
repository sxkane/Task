using Player;

namespace Events
{
    public class OnPlayerDiedEvent : IEvent
    {
        public PlayerController Target { get; }

        public OnPlayerDiedEvent(PlayerController target)
        {
            Target = target;
        }
    }
}
