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
            Game.Pause();
        }

        public override void Exit()
        {
            Game.Resume();
        }
    }
}