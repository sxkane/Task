using ObjectPool;
using UnityEngine;

namespace Enemy.Attack
{
    public class GoblinSorcererAttack : EnemyAttack
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float attackRange = 6f;
        [SerializeField] private float retreatDistance = 3f;
        [SerializeField] private float chaseDistance = 7f;
        [SerializeField] private float holdSpeedMultiplier = 0.25f;
        [SerializeField] private Transform firePoint;

        public override bool CanAttack()
        {
            if (!base.CanAttack())
                return false;

            var dist = Vector2.Distance(transform.position, Target.position);
            return dist <= attackRange;
        }

        protected override void ExecuteAttack()
        {
            if (bulletPrefab == null || firePoint == null || Target == null)
                return;

            var dir = (Target.position - firePoint.position).normalized;
            var bullet = PoolManager.Instance.Spawn(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<GoblinSorcererBullet>().Init(dir, Stats.Damage);
        }

        public override Vector2 GetMovementDirection(Vector2 currentPosition, Vector2 targetPosition)
        {
            var toTarget = targetPosition - currentPosition;
            var distance = toTarget.magnitude;

            if (distance < retreatDistance)
                return -toTarget.normalized;

            if (distance > chaseDistance)
                return toTarget.normalized;

            return Vector2.zero;
        }

        public override float GetMovementSpeedMultiplier(float distanceToTarget)
        {
            return distanceToTarget >= retreatDistance && distanceToTarget <= chaseDistance
                ? holdSpeedMultiplier
                : 1f;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, retreatDistance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
