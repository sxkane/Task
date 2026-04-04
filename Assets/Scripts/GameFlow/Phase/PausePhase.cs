using Core;

namespace GameFlow.Phase
{
    public class PausePhase : GamePhase
    {
        public PausePhase(GameController game) : base(game)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Game.DisablePlayerInput();
            Game.Pause();
        }

        public override void Exit()
        {
            base.Exit();

            Game.EnablePlayerInput();
            Game.Resume();
        }
    }
}
