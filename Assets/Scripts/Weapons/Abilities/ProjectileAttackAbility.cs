using ObjectPool;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Core;
using Weapons.Modifiers;
using Weapons.Projectiles;

namespace Weapons.Abilities
{
    [CreateAssetMenu(menuName = "Game/Weapon Ability/Projectile Attack")]
    public class ProjectileAttackAbility : WeaponAbility
    {
        [System.Serializable]
        private class ProjectileAttackConfig : WeaponAbilityRarityConfigBase
        {
            public float projectileSpeed;
        }

        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private bool aimAtNearestEnemy = true;
        [FormerlySerializedAs("fallbackProjectileSpeed")]
        [SerializeField] private float projectileSpeed = 8f;
        [SerializeField] private System.Collections.Generic.List<ProjectileAttackConfig> rarityConfigs = new();

        public override void OnInitialize(WeaponRuntimeContext context)
        {
            if (context?.RuntimeStats == null)
                return;

            var config = ResolveConfig(rarityConfigs, context.Weapon.Entry.rarity);
            context.RuntimeStats.GetStat(WeaponStatType.ProjectileSpeed).BaseValue =
                config != null && config.projectileSpeed > 0f
                    ? config.projectileSpeed
                    : projectileSpeed;
        }

        public override void OnAttack(WeaponRuntimeContext context)
        {
            if (context?.Player == null || projectilePrefab == null)
                return;

            var spawnTransform = context.Weapon.transform;
            var target = aimAtNearestEnemy && context.EnemyManager != null
                ? context.EnemyManager.GetNearestEnemy(context.Player.transform.position)
                : null;
            var targetTransform = target != null ? target.transform : null;
            var direction = targetTransform != null
                ? ((Vector2)(targetTransform.position - spawnTransform.position)).normalized
                : context.Player.AimDirection.normalized;
            context.Weapon.FaceDirectionFromAbility(direction);
            var projectileSpeed = context.RuntimeStats.GetStat(WeaponStatType.ProjectileSpeed).Value;

            var projectileObject = PoolManager.Instance.Spawn(
                projectilePrefab,
                spawnTransform.position,
                spawnTransform.rotation,
                context.ProjectileRoot);

            if (projectileObject.TryGetComponent<IWeaponProjectileLauncher>(out var projectileLauncher))
                projectileLauncher.Launch(context, targetTransform, direction, projectileSpeed);
        }
    }
}
