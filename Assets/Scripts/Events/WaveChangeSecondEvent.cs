namespace Events
{
    public struct WaveChangeSecondEvent : IEvent
    {
        public int Timer { get; private set; }
        
        public WaveChangeSecondEvent(int timer)
        {
            Timer = timer;
        }
    }
}