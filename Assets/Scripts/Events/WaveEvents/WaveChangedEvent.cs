using Events;

namespace Events.WaveEvents
{
    public struct WaveChangedEvent : IEvent
    {
        public int CurrentWave { get; }
        public int TotalWaves { get; }

        public WaveChangedEvent(int currentWave, int totalWaves)
        {
            CurrentWave = currentWave;
            TotalWaves = totalWaves;
        }
    }
}
