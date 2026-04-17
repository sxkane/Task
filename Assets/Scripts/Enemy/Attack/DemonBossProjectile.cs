using UnityEngine;

namespace Enemy.Attack
{
    public class DemonBossProjectile : EnemyProjectile
    {
        [SerializeField] private float speed = 7f;

        public void Init(Vector2 direction, float damage)
        {
            InitializeProjectile(direction, damage);
        }

        private void Update()
        {
            transform.position += (Vector3)(Direction * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            DealDamageToPlayer(other);
        }
    }
}
