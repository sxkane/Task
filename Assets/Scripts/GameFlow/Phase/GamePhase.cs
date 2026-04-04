using Core;

namespace GameFlow.Phase
{
    public abstract class GamePhase
    {
        protected readonly GameController Game;

        protected GamePhase(GameController game)
        {
            Game = game;
        }
        
        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }
    }
}