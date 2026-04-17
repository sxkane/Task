using UnityEngine;

namespace Enemy.Attack
{
    public class GoblinSorcererBullet : EnemyProjectile
    {
        [SerializeField] private float speed = 8f;

        public void Init(Vector2 dir, float damage)
        {
            InitializeProjectile(dir, damage);
        }

        private void Update()
        {
            transform.Translate(Direction * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            DealDamageToPlayer(other);
        }
    }
}
