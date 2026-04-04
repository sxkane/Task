using Core;

namespace GameFlow.Phase
{
    public class RewardAndShopPhase : GamePhase
    {
        public RewardAndShopPhase(GameController game) : base(game)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            Game.DisablePlayerInput();
            Game.Pause();
            Game.RewardManager.Activate();
        }

        public override void Exit()
        {
            Game.EnablePlayerInput();
            Game.RewardManager.Deactivate();
            Game.Resume();
        }

        private void ChangeToBattle()
        {
            Game.ChangeState(GamePhaseType.Battle);
        }
    }
}
