using UnityEngine;

namespace Enemy
{
    public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private float speed = 8f;
        [SerializeField] private float lifetime = 4f;

        private Vector2 _direction;
        private float _damage;

        public void Init(Vector2 dir, float damage)
        {
            _direction = dir;
            _damage = damage;

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.Translate(_direction * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player.PlayerController>() != null)
            {
                Debug.Log("Player hit by bullet!");

                // TODO: 玩家受伤系统

                Destroy(gameObject);
            }
        }
    }
}