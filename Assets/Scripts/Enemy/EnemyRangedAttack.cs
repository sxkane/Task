using UnityEngine;

namespace Enemy
{
    public class EnemyRangedAttack : EnemyAttack
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float attackRange = 6f;
        [SerializeField] private Transform firePoint;

        protected override bool CanAttack()
        {
            float dist = Vector2.Distance(
                transform.position,
                Target.position);

            return dist <= attackRange;
        }

        protected override void Attack()
        {
            if (bulletPrefab == null || firePoint == null)
                return;

            Vector2 dir =
                (Target.position - firePoint.position).normalized;

            var bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity);

            bullet.GetComponent<EnemyBullet>().Init(dir, Stats.Damage);
        }
    }
}