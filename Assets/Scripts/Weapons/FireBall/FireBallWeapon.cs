using ObjectPool;
using UnityEngine;
using Weapons.Effects;

namespace Weapons.FireBall
{
    public class FireBallWeapon : CooldownWeapon
    {
        [SerializeField] private GameObject fireBallPrefab;
        [SerializeField] private float bulletSpeed = 4f;
        [SerializeField] private Transform firePoint;

        protected override void Attack()
        {
            var target = EnemyManager.GetNearestEnemy(Player.transform.position);

            if (target != null)
            {
                var dir = (target.transform.position - transform.position).normalized;
                transform.right = dir;
            }

            ExecuteEffects(EffectTrigger.OnWeaponAttack);

            var bulletObj = PoolManager.Instance.Spawn(
                fireBallPrefab,
                firePoint.position,
                firePoint.rotation,
                ProjectileRoot);

            bulletObj.GetComponent<FireBallBullet>()
                .Init(this, Stats, bulletSpeed, Player, EnemyManager);
        }
    }
}
