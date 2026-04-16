using Core;

namespace GameFlow.Phase
{
    public class PreparingPhase : GamePhase
    {
        public PreparingPhase(GameController game) : base(game)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            Game.InitializeRun();
            Game.ChangeState(GamePhaseType.Battle);
        }
    }
}
