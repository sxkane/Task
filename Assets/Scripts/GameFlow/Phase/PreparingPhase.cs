using Core;
using Player;
using Rewards;
using Waves;
using Weapons;
using Weapons.Items;

namespace GameFlow.Phase
{
    public class PreparingPhase : GamePhase
    {
        public PreparingPhase(GameController game) : base(game)
        {
        }

        public override void Enter()
        {
            base.Enter();

            var selectedPlayer = Game.SelectedPlayer;
            Game.PlayerManager.Initialize(selectedPlayer);
            Game.WaveManager.Initialize(Game.PlayerManager);
            Game.WeaponManager.Initialize(Game.PlayerManager, Game.WaveManager, Game.SelectedWeapons);
            Game.ItemManager.Initialize(Game.PlayerManager);
            Game.RewardManager.Initialize(Game.PlayerManager, Game.WeaponManager, Game.ItemManager, Game.WaveManager);
            
            Game.ChangeState(GamePhaseType.Battle);
        }
    }
}
