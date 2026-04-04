using GameFlow.Phase;

namespace GameFlow
{
    public class GameFlowStateMachine
    {
        public GamePhase CurrentPhase { get; private set; }
        public GamePhase PreviousPhase { get; private set; }

        public void Initialize(GamePhase newPhase)
        {
            CurrentPhase = newPhase;
            CurrentPhase.Enter();
        }
        
        public void ChangePhase(GamePhase newPhase)
        {
            PreviousPhase = CurrentPhase;
            CurrentPhase.Exit();
            CurrentPhase = newPhase;
            CurrentPhase.Enter();
        }
    }
}
