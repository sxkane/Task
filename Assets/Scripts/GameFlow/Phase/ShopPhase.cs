using Core;

namespace GameFlow.Phase
{
    public class ShopPhase : GamePhase
    {
        public ShopPhase(GameController game) : base(game)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Game.DisablePlayerInput();
            Game.Pause();
            Game.ShopManager.BeginPhase();
        }

        public override void Exit()
        {
            Game.ShopManager.EndPhase();
            Game.Resume();
            Game.EnablePlayerInput();
        }
    }
}
