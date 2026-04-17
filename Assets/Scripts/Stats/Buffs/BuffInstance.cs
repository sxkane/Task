namespace Stats.Buffs
{
    public sealed class BuffInstance
    {
        public BuffData Data { get; }
        public object Source { get; }
        public float RemainingDuration { get; private set; }

        public bool IsExpired => Data != null && Data.Duration > 0f && RemainingDuration <= 0f;

        public BuffInstance(BuffData data, object source)
        {
            Data = data;
            Source = source ?? data;
            RemainingDuration = data != null ? data.Duration : 0f;
        }

        public void Tick(float deltaTime)
        {
            if (Data == null || Data.Duration <= 0f)
                return;

            RemainingDuration -= deltaTime;
        }

        public void Refresh()
        {
            if (Data == null)
                return;

            RemainingDuration = Data.Duration;
        }
    }
}
