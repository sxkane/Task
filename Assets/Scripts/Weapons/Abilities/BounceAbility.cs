using UnityEngine;
using Stats;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.Abilities
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Bounce")]
    public class BounceAbility : WeaponAbility
    {
        [System.Serializable]
        private class BounceConfig : WeaponAbilityRarityConfigBase
        {
            public int bounceCount;
        }

        [SerializeField] private int bounceCount = 1;
        [SerializeField] private System.Collections.Generic.List<BounceConfig> rarityConfigs = new();

        public override void OnInitialize(WeaponRuntimeContext context)
        {
            if (context?.RuntimeStats == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            var value = config != null ? config.bounceCount : bounceCount;
            context.RuntimeStats.GetStat(WeaponStatType.BounceCount)
                .AddModifier(new Modifier(value, StatModType.Flat, this));
        }

        public override string BuildDescription(Rarity rarity)
        {
            var config = ResolveConfig(rarityConfigs, rarity);
            var value = config != null ? config.bounceCount : bounceCount;
            return $"- 弹跳 {value} 次";
        }
    }
}
