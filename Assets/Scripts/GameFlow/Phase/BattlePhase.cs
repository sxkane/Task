using Core;
using Events;
using Events.EnemyEvents;

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
            EventBus.Subscribe<OnEnemyDiedEvent>(OnEnemyDied);
        }

        private void UnsubscribeEvents()
        {
            Game.WaveManager.OnWaveCompleted -= WaveComplete;
            EventBus.Unsubscribe<OnPlayerDiedEvent>(OnPlayerDied);
            EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);
        }

        private void WaveComplete(bool isFinalWave)
        {
            Game.BeginWaveCompletion(isFinalWave);
        }

        private void OnPlayerDied(OnPlayerDiedEvent eventData)
        {
            if (Game.PlayerManager?.Player == eventData.Target)
            {
                Game.IsVictory = false;
                Game.ChangeState(GamePhaseType.GameOver);
            }
        }

        private void OnEnemyDied(OnEnemyDiedEvent eventData)
        {
            if (eventData.Target == null || Game.WaveManager == null)
                return;

            if (!Game.WaveManager.IsFinalWave || !Game.WaveManager.BossSpawned)
                return;

            if (Game.WaveManager.EnemyManager != null && Game.WaveManager.EnemyManager.AliveEnemyCount <= 0)
                Game.BeginWaveCompletion(true);
        }
    }
}
