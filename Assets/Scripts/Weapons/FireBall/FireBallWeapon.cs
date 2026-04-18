using ObjectPool;
using UnityEngine;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons.FireBall
{
    public class FireBallWeapon : CooldownWeapon
    {
        [SerializeField] private GameObject fireBallPrefab;
        [SerializeField] private Transform firePoint;

        protected override void Attack()
        {
            var target = EnemyManager.GetNearestEnemy(Player.transform.position);

            if (target != null)
            {
                var dir = (target.transform.position - transform.position).normalized;
                FaceDirection(dir);
            }

            NotifyAbilitiesAttack();

            if (Abilities != null && Abilities.Count > 0)
                return;

            var bulletObj = PoolManager.Instance.Spawn(
                fireBallPrefab,
                firePoint.position,
                firePoint.rotation,
                ProjectileRoot);
            var projectileSpeed = RuntimeStats != null ? RuntimeStats.GetStat(WeaponStatType.ProjectileSpeed).Value : 0f;

            bulletObj.GetComponent<FireBallBullet>()
                .Init(this, Stats, RuntimeStats, projectileSpeed, Player, EnemyManager);
        }
    }
}
