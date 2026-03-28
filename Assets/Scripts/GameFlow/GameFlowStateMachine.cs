using GameFlow.Phase;

namespace GameFlow
{
    public class GameFlowStateMachine
    {
        public GamePhase CurrentPhase;

        public void Initialize(GamePhase newPhase)
        {
            CurrentPhase = newPhase;
            CurrentPhase.Enter();
        }
        
        public void ChangePhase(GamePhase newPhase)
        {
            CurrentPhase.Exit();
            CurrentPhase = newPhase;
            CurrentPhase.Enter();
        }
    }
}