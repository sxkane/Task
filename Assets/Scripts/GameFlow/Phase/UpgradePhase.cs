using Core;

namespace GameFlow.Phase
{
    public class UpgradePhase : GamePhase
    {
        public UpgradePhase(GameController game) : base(game)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Game.DisablePlayerInput();
            Game.Pause();

            if (Game.UpgradeManager == null || !Game.UpgradeManager.HasPendingSelections())
            {
                Game.ChangeState(GamePhaseType.Shop);
                return;
            }

            Game.UpgradeManager.SequenceCompleted += HandleSequenceCompleted;
            Game.UpgradeManager.BeginPhase();
        }

        public override void Exit()
        {
            Game.UpgradeManager.SequenceCompleted -= HandleSequenceCompleted;
            Game.UpgradeManager.EndPhase();
            Game.Resume();
            Game.EnablePlayerInput();
        }

        private void HandleSequenceCompleted()
        {
            Game.ChangeState(GamePhaseType.Shop);
        }
    }
}
