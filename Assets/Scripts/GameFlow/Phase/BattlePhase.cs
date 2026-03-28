using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            
            EventScribe();
            Game.WaveManager.Activate();
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
        }

        private void EventUnScribe()
        {
            Game.WaveManager.OnWaveCompleted -= WaveComplete;
        }
        

        private void WaveComplete(bool isFinalWave)
        {
            if (!isFinalWave)
            {
                Game.ChangeState(GamePhaseType.RewardAndShop);
            }
            else
            {
                Game.ChangeState(GamePhaseType.GameOver);
            }
        }
    }
}