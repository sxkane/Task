namespace Events.PlayerEvents
{
    public class OnPlayerHealthChangedEvent : IEvent
    {
        public Player.PlayerController Target { get; }

        public OnPlayerHealthChangedEvent(Player.PlayerController target)
        {
            Target = target;
        }
    }
}
