using Audio;
using GameAudio;

namespace Weapons.Bow
{
    public class DoubleBowWeapon : BasicBowWeapon
    {
        [UnityEngine.SerializeField] private float arrowSeparation = 0.18f;

        protected override void Attack()
        {
            NotifyAbilitiesAttack();

            if (Abilities != null && Abilities.Count > 0)
                return;

            var enemy = EnemyManager.GetNearestEnemy(Player.transform.position);
            var enemyTransform = enemy != null ? enemy.transform : null;
            var direction = enemyTransform != null
                ? ((UnityEngine.Vector2)(enemyTransform.position - transform.position)).normalized
                : Player.AimDirection;
            FaceDirection(direction);
            GlobalSfxPlayer.Instance.PlayWeaponAttack();

            var sideOffset = (UnityEngine.Vector2)(-transform.up) * (arrowSeparation * 0.5f);
            SpawnArrow(transform.position + (UnityEngine.Vector3)sideOffset, enemyTransform, Player.AimDirection);
            SpawnArrow(transform.position - (UnityEngine.Vector3)sideOffset, enemyTransform, Player.AimDirection);
        }
    }
}
