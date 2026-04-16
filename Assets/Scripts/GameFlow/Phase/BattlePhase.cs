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

            SubscribeEvents();

            if (Game.StateMachine.PreviousPhase is PausePhase)
                Game.WaveManager.ResumePhase();
            else
                Game.WaveManager.BeginPhase();

            Game.WeaponManager.BeginPhase();
        }

        public override void Exit()
        {
            base.Exit();

            UnsubscribeEvents();
            Game.WaveManager.EndPhase();
            Game.WeaponManager.EndPhase();
        }

        private void SubscribeEvents()
        {
            Game.WaveManager.OnWaveCompleted += WaveComplete;
            EventBus.Subscribe<OnPlayerDiedEvent>(OnPlayerDied);
        }

        private void UnsubscribeEvents()
        {
            Game.WaveManager.OnWaveCompleted -= WaveComplete;
            EventBus.Unsubscribe<OnPlayerDiedEvent>(OnPlayerDied);
        }

        private void WaveComplete(bool isFinalWave)
        {
            if (!isFinalWave)
                if (Game.UpgradeManager != null && Game.UpgradeManager.HasPendingSelections())
                    Game.ChangeState(GamePhaseType.Upgrade);
                else
                    Game.ChangeState(GamePhaseType.Shop);
            else
                Game.ChangeState(GamePhaseType.GameOver);
        }

        private void OnPlayerDied(OnPlayerDiedEvent eventData)
        {
            if (Game.PlayerManager?.Player == eventData.Target)
                Game.ChangeState(GamePhaseType.GameOver);
        }
    }
}
