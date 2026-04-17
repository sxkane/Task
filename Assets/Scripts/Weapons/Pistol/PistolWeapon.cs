using ObjectPool;
using UnityEngine;
using Weapons.Effects;

namespace Weapons.Pistol
{
    public class PistolWeapon : CooldownWeapon
    {
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private float bulletSpeed;

        protected override void Attack()
        {
            ExecuteEffects(EffectTrigger.OnWeaponAttack);

            var bulletObj = PoolManager.Instance.Spawn(
                circlePrefab,
                transform.position,
                transform.rotation,
                ProjectileRoot);

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            var enemyTransform = enemy != null ? enemy.transform : null;

            bulletObj.GetComponent<PistolBullet>()
                .Init(bulletSpeed, enemyTransform, Player.AimDirection, Player, Stats);
        }
    }
}
