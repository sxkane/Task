using Audio;
using ObjectPool;
using GameAudio;
using UnityEngine;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.Arcane
{
    public class ApprenticeWandWeapon : CooldownWeapon
    {
        [SerializeField] private GameObject projectilePrefab;

        protected override void Attack()
        {
            NotifyAbilitiesAttack();

            if (Abilities != null && Abilities.Count > 0)
                return;

            var projectileObject = PoolManager.Instance.Spawn(
                projectilePrefab,
                transform.position,
                transform.rotation,
                ProjectileRoot);

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            var enemyTransform = enemy != null ? enemy.transform : null;
            var direction = enemyTransform != null
                ? ((Vector2)(enemyTransform.position - transform.position)).normalized
                : Player.AimDirection;
            FaceDirection(direction);
            GlobalSfxPlayer.Instance.PlayWeaponAttack();
            var runtimeProjectileSpeed = RuntimeStats != null ? RuntimeStats.GetStat(WeaponStatType.ProjectileSpeed).Value : 0f;

            projectileObject.GetComponent<ApprenticeWandProjectile>()
                .Init(
                    runtimeProjectileSpeed,
                    enemyTransform,
                    Player.AimDirection,
                    Player,
                    Stats,
                    RuntimeStats,
                    EnemyManager);
        }
    }
}
