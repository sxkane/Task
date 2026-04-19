using UnityEngine;
using Weapons.Core;

namespace Weapons.Abilities.Melee
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Melee/Bonus Experience On Kill")]
    public class BonusExperienceOnKillAbility : WeaponAbility
    {
        [System.Serializable]
        private class ExperienceGainConfig : WeaponAbilityRarityConfigBase
        {
            public float bonusPercent;
        }

        [SerializeField] private System.Collections.Generic.List<ExperienceGainConfig> rarityConfigs = new();

        public override void OnKill(WeaponRuntimeContext context, Enemy.EnemyController enemy, bool isCritical)
        {
            if (context?.Player?.RuntimeData == null || context.Weapon?.Entry == null || enemy?.Stats == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            if (config == null || config.bonusPercent <= 0f)
                return;

            var bonusExp = Mathf.RoundToInt(enemy.Stats.ExpReward * config.bonusPercent / 100f);
            if (bonusExp > 0)
                context.Player.RuntimeData.AddExperience(bonusExp);
        }

        public override string BuildDescription(Rarity rarity)
        {
            var config = ResolveConfig(rarityConfigs, rarity);
            if (config == null || config.bonusPercent <= 0f)
                return string.Empty;

            return $"- +{config.bonusPercent:0.#}% XP Gain";
        }
    }
}
