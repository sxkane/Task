namespace Core
{
    public interface IConfigurable<in TContext>
    {
        void Configure(TContext context);
    }

    public interface IRunLifecycle
    {
        void InitializeRun();
        void ResetRun();
    }

    public interface IPhaseLifecycle
    {
        void BeginPhase();
        void EndPhase();
    }
}
