namespace Enemy.Core
{
    public enum EnemyLifePhase
    {
        Spawning,
        Active,
        Dying,
        Despawned
    }

    public sealed class EnemyLifecycle
    {
        public EnemyLifePhase Phase { get; private set; } = EnemyLifePhase.Despawned;

        public bool IsActive => Phase == EnemyLifePhase.Active;
        public bool CanTakeDamage => Phase == EnemyLifePhase.Active;
        public bool CanDealDamage => Phase == EnemyLifePhase.Active;

        public void EnterSpawning()
        {
            Phase = EnemyLifePhase.Spawning;
        }

        public void EnterActive()
        {
            Phase = EnemyLifePhase.Active;
        }

        public void EnterDying()
        {
            Phase = EnemyLifePhase.Dying;
        }

        public void EnterDespawned()
        {
            Phase = EnemyLifePhase.Despawned;
        }
    }
}
