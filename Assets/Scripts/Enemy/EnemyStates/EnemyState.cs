namespace Enemy.EnemyStates
{
    public abstract class EnemyState
    {
        protected readonly EnemyController Enemy;
        protected EnemyStateMachine Machine;

        public EnemyState(EnemyController enemyController, EnemyStateMachine stateMachine)
        {
            Enemy = enemyController;
            Machine = stateMachine;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Exit()
        {
        }
    }
}