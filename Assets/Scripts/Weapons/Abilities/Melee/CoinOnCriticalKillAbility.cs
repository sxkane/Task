using UnityEngine;
using Weapons.Core;

namespace Weapons.Abilities.Melee
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Melee/Coin On Critical Kill")]
    public class CoinOnCriticalKillAbility : WeaponAbility
    {
        [System.Serializable]
        private class CoinRewardConfig : WeaponAbilityRarityConfigBase
        {
            public float coinChancePercent;
        }

        [SerializeField] private System.Collections.Generic.List<CoinRewardConfig> rarityConfigs = new();
        [SerializeField] private int rewardCoinAmount = 1;

        public override void OnKill(WeaponRuntimeContext context, Enemy.EnemyController enemy, bool isCritical)
        {
            if (!isCritical || context?.Player?.RuntimeData == null || context.Weapon?.Entry == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            var chance = config != null ? config.coinChancePercent : 0f;
            if (chance <= 0f)
                return;

            if (Random.value <= chance / 100f)
                context.Player.RuntimeData.AddCoins(rewardCoinAmount);
        }

        public override string BuildDescription(Rarity rarity)
        {
            var config = ResolveConfig(rarityConfigs, rarity);
            if (config == null || config.coinChancePercent <= 0f)
                return string.Empty;

            return $"- 用这把武器暴击击杀敌人时，{config.coinChancePercent:0.#}% 概率获得 {rewardCoinAmount} 金币";
        }
    }
}
