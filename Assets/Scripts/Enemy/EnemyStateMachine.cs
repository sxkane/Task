using Enemy.EnemyStates;

namespace Enemy
{
    public class EnemyStateMachine
    {
        public EnemyState currentState;
        
        public void Initialize(EnemyState newState)
        {
            currentState = newState;
            currentState.Enter();
        }

        public void ChangeState(EnemyState newState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }
}