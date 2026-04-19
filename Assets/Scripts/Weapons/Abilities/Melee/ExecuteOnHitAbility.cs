using Enemy;
using UnityEngine;
using Weapons.Core;

namespace Weapons.Abilities.Melee
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Melee/Execute On Hit")]
    public class ExecuteOnHitAbility : WeaponAbility
    {
        [System.Serializable]
        private class ExecuteChanceConfig : WeaponAbilityRarityConfigBase
        {
            public float executeChancePercent;
        }

        [SerializeField] private System.Collections.Generic.List<ExecuteChanceConfig> rarityConfigs = new();
        [SerializeField] private int executionDamage = 999999;

        public override int ModifyDamage(WeaponRuntimeContext context, EnemyController enemy, Vector2 hitPosition, int damage, bool isCritical)
        {
            if (context?.Weapon?.Entry == null)
                return damage;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            var executeChance = config != null ? config.executeChancePercent : 0f;
            if (executeChance <= 0f)
                return damage;

            return Random.value < executeChance / 100f ? executionDamage : damage;
        }

        public override string BuildDescription(Rarity rarity)
        {
            var config = ResolveConfig(rarityConfigs, rarity);
            if (config == null || config.executeChancePercent <= 0f)
                return string.Empty;

            return $"- {config.executeChancePercent:0.#}% 的几率击中目标时一击必杀";
        }
    }
}
