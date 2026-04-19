using System.Collections;
using System.Collections.Generic;
using Enemy;
using Events;
using Events.EnemyEvents;
using UnityEngine;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.Abilities.Melee
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Melee/Burn Spread On Hit")]
    public class BurnSpreadOnHitAbility : WeaponAbility
    {
        [System.Serializable]
        private class BurnSpreadConfig : WeaponAbilityRarityConfigBase
        {
            public float burnTickDamage;
            public int burnTickCount;
            public int spreadTargetCount;
        }

        [SerializeField] private float burnTickInterval = 0.35f;
        [SerializeField] private float burnRadius = 2.5f;
        [SerializeField] private List<BurnSpreadConfig> rarityConfigs = new();

        private readonly List<EnemyController> _spreadTargets = new();

        public override void OnInitialize(WeaponRuntimeContext context)
        {
            if (context?.RuntimeStats == null || context.Weapon?.Entry == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            context.RuntimeStats.GetStat(WeaponStatType.BurnSpreadCount).BaseValue = config != null ? config.spreadTargetCount : 0f;
        }

        public override void OnHit(WeaponRuntimeContext context, EnemyController enemy, Vector2 hitPosition, int damage, bool isCritical)
        {
            if (context?.Weapon?.Entry == null || enemy == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            if (config == null || config.burnTickDamage <= 0f || config.burnTickCount <= 0)
                return;

            ApplyBurn(context, enemy, config.burnTickDamage, config.burnTickCount);

            var extraTargets = Mathf.RoundToInt(context.RuntimeStats.GetStat(WeaponStatType.BurnSpreadCount).Value);
            if (extraTargets <= 0 || context.EnemyManager == null)
                return;

            context.EnemyManager.GetEnemiesInRadius(enemy.transform.position, burnRadius, _spreadTargets);
            var applied = 0;
            for (var i = 0; i < _spreadTargets.Count; i++)
            {
                var target = _spreadTargets[i];
                if (target == null || target == enemy || target.Stats == null || !target.Stats.IsAlive)
                    continue;

                ApplyBurn(context, target, config.burnTickDamage, config.burnTickCount);
                applied++;

                if (applied >= extraTargets)
                    break;
            }
        }

        private void ApplyBurn(WeaponRuntimeContext context, EnemyController enemy, float tickDamage, int tickCount)
        {
            WeaponAbilityRuntimeRunner.BeginRoutine(ApplyBurnRoutine(context, enemy, tickDamage, tickCount));
        }

        private IEnumerator ApplyBurnRoutine(WeaponRuntimeContext context, EnemyController enemy, float tickDamage, int tickCount)
        {
            for (var i = 0; i < tickCount; i++)
            {
                yield return new WaitForSeconds(burnTickInterval);

                if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.Stats == null || !enemy.Stats.IsAlive)
                    yield break;

                EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, tickDamage, default, 0f, false, context.Weapon));
                context.Player?.TryLifeStealOnHit();
            }
        }

        public override string BuildDescription(Rarity rarity)
        {
            var config = ResolveConfig(rarityConfigs, rarity);
            if (config == null || config.burnTickDamage <= 0f || config.burnTickCount <= 0)
                return string.Empty;

            return config.spreadTargetCount > 0
                ? $"- 造成 {config.burnTickDamage:0.#}x{config.burnTickCount}（100%）燃烧伤害，燃烧会扩散到附近额外的敌人 {config.spreadTargetCount} 个"
                : $"- 造成 {config.burnTickDamage:0.#}x{config.burnTickCount}（100%）燃烧伤害";
        }
    }
}
