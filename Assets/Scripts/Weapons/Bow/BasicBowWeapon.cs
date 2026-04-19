using Audio;
using ObjectPool;
using GameAudio;
using UnityEngine;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.Bow
{
    public class BasicBowWeapon : CooldownWeapon
    {
        [SerializeField] protected GameObject arrowPrefab;

        protected override void Attack()
        {
            NotifyAbilitiesAttack();

            if (Abilities != null && Abilities.Count > 0)
                return;

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            var enemyTransform = enemy != null ? enemy.transform : null;
            var direction = enemyTransform != null
                ? ((Vector2)(enemyTransform.position - transform.position)).normalized
                : Player.AimDirection;
            FaceDirection(direction);
            GlobalSfxPlayer.Instance.PlayWeaponAttack();
            SpawnArrow(transform.position, enemyTransform, Player.AimDirection);
        }

        protected void SpawnArrow(Vector3 spawnPosition, Transform enemyTransform, Vector2 fallbackDirection)
        {
            var projectileObject = PoolManager.Instance.Spawn(
                arrowPrefab,
                spawnPosition,
                transform.rotation,
                ProjectileRoot);

            var runtimeProjectileSpeed = RuntimeStats != null ? RuntimeStats.GetStat(WeaponStatType.ProjectileSpeed).Value : 0f;

            projectileObject.GetComponent<BasicBowProjectile>()
                .Init(
                    runtimeProjectileSpeed,
                    enemyTransform,
                    fallbackDirection,
                    Player,
                    Stats,
                    RuntimeStats);
        }
    }
}
