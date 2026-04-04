using Core;
using Events;

namespace GameFlow.Phase
{
    public class BattlePhase : GamePhase
    {
        public BattlePhase(GameController game) : base(game)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Game.Resume();
            Game.EnablePlayerInput();
            Game.GameInputHandler?.EnableInput();
            
            EventScribe();

            if (Game.StateMachine.PreviousPhase is PausePhase)
                Game.WaveManager.ResumeWave();
            else
                Game.WaveManager.StartNextWave();

            Game.WeaponManager.Activate();
        }

        public override void Exit()
        {
            base.Exit();
            
            EventUnScribe();
            Game.WaveManager.Deactivate();
            Game.WeaponManager.Deactivate();
        }

        private void EventScribe()
        {
            Game.WaveManager.OnWaveCompleted += WaveComplete;
            EventBus.Subscribe<OnPlayerDiedEvent>(OnPlayerDied);
        }

        private void EventUnScribe()
        {
            Game.WaveManager.OnWaveCompleted -= WaveComplete;
            EventBus.Unsubscribe<OnPlayerDiedEvent>(OnPlayerDied);
        }
        
        private void WaveComplete(bool isFinalWave)
        {
            if (!isFinalWave)
                Game.ChangeState(GamePhaseType.RewardAndShop);
            else
                Game.ChangeState(GamePhaseType.GameOver);
        }
        
        private void OnPlayerDied(OnPlayerDiedEvent e)
        {
            if (Game.PlayerManager?.Player == e.Target)
                Game.ChangeState(GamePhaseType.GameOver);
        }
    }
}
