namespace Enemy.Buffs
{
    public sealed class EnemyBuffInstance
    {
        public EnemyBuffData Data { get; }
        public object Source { get; }
        public float RemainingDuration { get; private set; }

        public bool IsExpired => Data != null && Data.Duration > 0f && RemainingDuration <= 0f;

        public EnemyBuffInstance(EnemyBuffData data, object source)
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
