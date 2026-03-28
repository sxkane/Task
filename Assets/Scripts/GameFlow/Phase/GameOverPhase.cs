using Core;
using UnityEngine.SceneManagement;

namespace GameFlow.Phase
{
    public class GameOverPhase : GamePhase
    {
        public GameOverPhase(GameController game) : base(game)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            
            Game.Pause();
        }

        public override void Exit()
        {
            base.Exit();
            
            Game.Pause();
        }
    }
}