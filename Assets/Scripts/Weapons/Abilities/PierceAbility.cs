using Stats;
using UnityEngine;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.Abilities
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Pierce")]
    public class PierceAbility : WeaponAbility
    {
        [System.Serializable]
        private class PierceConfig : WeaponAbilityRarityConfigBase
        {
            public int pierceCount = 1;
            public float damageMultiplierAfterPierce = 0.5f;
        }

        [SerializeField] private int pierceCount = 1;
        [SerializeField] private float damageMultiplierAfterPierce = 0.5f;
        [SerializeField] private System.Collections.Generic.List<PierceConfig> rarityConfigs = new();

        public override void OnInitialize(WeaponRuntimeContext context)
        {
            if (context?.RuntimeStats == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            var configuredPierceCount = config != null ? config.pierceCount : pierceCount;
            var configuredDamageMultiplier = config != null && config.damageMultiplierAfterPierce > 0f
                ? config.damageMultiplierAfterPierce
                : damageMultiplierAfterPierce;

            context.RuntimeStats.GetStat(WeaponStatType.PierceCount)
                .AddModifier(new Modifier(configuredPierceCount, StatModType.Flat, this));

            context.RuntimeStats.GetStat(WeaponStatType.PierceDamageMultiplier)
                .BaseValue = configuredDamageMultiplier;
        }

        public override string BuildDescription(Rarity rarity)
        {
            var config = ResolveConfig(rarityConfigs, rarity);
            var count = config != null ? config.pierceCount : pierceCount;
            var multiplier = config != null && config.damageMultiplierAfterPierce > 0f
                ? config.damageMultiplierAfterPierce
                : damageMultiplierAfterPierce;

            var reductionPercent = Mathf.RoundToInt((1f - multiplier) * 100f);
            return $"- 刺穿 {count} 个敌人，每次穿刺后伤害减少 {reductionPercent}%";
        }
    }
}
