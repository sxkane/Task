using System.Collections;
using System.Collections.Generic;
using Core;
using Enemy;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using UnityEngine;
using VFX;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.Abilities
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Explosion On Hit")]
    public class ExplosionOnHitAbility : WeaponAbility
    {
        [System.Serializable]
        private class ExplosionOnHitConfig : WeaponAbilityRarityConfigBase
        {
            public float explosionRadius;
            public float explosionDamage;
            public float burnTickDamage;
            public int burnTickCount;
        }

        [SerializeField] private float explosionRadius = 1.8f;
        [SerializeField] private float explosionDamage = 8f;
        [SerializeField] private float burnTickInterval = 0.35f;
        [SerializeField] private float burnTickDamage = 0f;
        [SerializeField] private int burnTickCount = 0;
        [SerializeField] private GameObject explosionVfxPrefab;
        [SerializeField] private List<ExplosionOnHitConfig> rarityConfigs = new();

        private readonly List<EnemyController> _targets = new();

        public override void OnInitialize(WeaponRuntimeContext context)
        {
            if (context?.RuntimeStats == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            var radius = config != null && config.explosionRadius > 0f ? config.explosionRadius : explosionRadius;
            context.RuntimeStats.GetStat(WeaponStatType.ExplosionRadius).BaseValue = radius;
        }

        public override void OnProjectileHit(WeaponRuntimeContext context, EnemyController enemy, Vector2 hitPosition)
        {
            if (context?.EnemyManager == null || context.Player == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            var radius = context.RuntimeStats.GetStat(WeaponStatType.ExplosionRadius).Value;
            var damageAmount = config != null && config.explosionDamage > 0f ? config.explosionDamage : explosionDamage;
            var burnDamage = config != null ? config.burnTickDamage : burnTickDamage;
            var burnCount = config != null ? config.burnTickCount : burnTickCount;

            SpawnExplosionVfx(hitPosition, radius);

            _targets.Clear();
            context.EnemyManager.GetEnemiesInRadius(hitPosition, radius, _targets);

            for (var i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                if (target == null)
                    continue;

                var knockbackDirection = ((Vector2)target.transform.position - hitPosition).normalized;
                EventBus.Publish(new OnEnemyDamageRequestedEvent(target, damageAmount, knockbackDirection, 0f));
                context.Player.TryLifeStealOnHit();

                if (burnDamage > 0f && burnCount > 0)
                    WeaponAbilityRuntimeRunner.BeginRoutine(ApplyBurn(context, target, burnDamage, burnCount));
            }
        }

        private IEnumerator ApplyBurn(WeaponRuntimeContext context, EnemyController enemy, float tickDamage, int tickCount)
        {
            for (var i = 0; i < tickCount; i++)
            {
                yield return new WaitForSeconds(burnTickInterval);

                if (enemy == null || !enemy.gameObject.activeInHierarchy || !enemy.Stats.IsAlive)
                    yield break;

                EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, tickDamage));
                context.Player?.TryLifeStealOnHit();
            }
        }

        private void SpawnExplosionVfx(Vector2 hitPosition, float radius)
        {
            if (explosionVfxPrefab == null)
                return;

            var parent = GameController.Instance?.Session?.GetOrCreateGroupRoot(GameSessionRootType.WorldVfx, "WeaponVfx");
            var vfxObject = PoolManager.Instance != null
                ? PoolManager.Instance.Spawn(explosionVfxPrefab, hitPosition, Quaternion.identity, parent)
                : Instantiate(explosionVfxPrefab, hitPosition, Quaternion.identity, parent);
            if (vfxObject.TryGetComponent<ExplosionVfx>(out var explosionVfx))
                explosionVfx.Initialize(radius);
        }

        public override string BuildDescription(Rarity rarity)
        {
            var config = ResolveConfig(rarityConfigs, rarity);
            var radius = config != null && config.explosionRadius > 0f ? config.explosionRadius : explosionRadius;
            var damageAmount = config != null && config.explosionDamage > 0f ? config.explosionDamage : explosionDamage;
            var burnDamage = config != null ? config.burnTickDamage : burnTickDamage;
            var burnCount = config != null ? config.burnTickCount : burnTickCount;

            if (burnDamage > 0f && burnCount > 0)
                return $"- 弹丸命中时爆炸，范围 {radius:0.#}，造成 {damageAmount:0.#} 伤害，并附加 {burnDamage:0.#}x{burnCount} 燃烧伤害";

            return $"- 弹丸命中时爆炸，范围 {radius:0.#}，造成 {damageAmount:0.#} 伤害";
        }
    }
}
