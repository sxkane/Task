using System;
using UnityEngine;

namespace Enemy.Attack
{
    public class GoblinSorcererAttack : EnemyAttack
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float attackRange = 6f;
        [SerializeField] private Transform firePoint;

        public override bool CanAttack()
        {
            if (!base.CanAttack())
                return false;

            float dist = Vector2.Distance(
                transform.position,
                Target.position);

            return dist <= attackRange;
        }

        protected override void ExecuteAttack()
        {
            if (bulletPrefab == null || firePoint == null || Target == null)
                return;

            Vector2 dir =
                (Target.position - firePoint.position).normalized;

            var bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity);

            bullet.GetComponent<GoblinSorcererBullet>().Init(dir, Stats.Damage);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawSphere(transform.position, attackRange);
        }
    }
}