using ObjectPool;
using UnityEngine;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.Bow
{
    public class BasicBowWeapon : CooldownWeapon
    {
        [SerializeField] private GameObject arrowPrefab;

        protected override void Attack()
        {
            NotifyAbilitiesAttack();

            if (Abilities != null && Abilities.Count > 0)
                return;

            var projectileObject = PoolManager.Instance.Spawn(
                arrowPrefab,
                transform.position,
                transform.rotation,
                ProjectileRoot);

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            var enemyTransform = enemy != null ? enemy.transform : null;
            var direction = enemyTransform != null
                ? ((Vector2)(enemyTransform.position - transform.position)).normalized
                : Player.AimDirection;
            FaceDirection(direction);
            var runtimeProjectileSpeed = RuntimeStats != null ? RuntimeStats.GetStat(WeaponStatType.ProjectileSpeed).Value : 0f;

            projectileObject.GetComponent<BasicBowProjectile>()
                .Init(
                    runtimeProjectileSpeed,
                    enemyTransform,
                    Player.AimDirection,
                    Player,
                    Stats,
                    RuntimeStats);
        }
    }
}
