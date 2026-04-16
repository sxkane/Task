using Events;
using UnityEngine;

namespace Enemy.Attack
{
    public class GoblinSorcererBullet : MonoBehaviour
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
            var player = other.GetComponentInParent<Player.PlayerController>();
            
            Debug.Log(other.name);
            Debug.Log(other.gameObject.layer);
            if (player == null)
                return;

            EventBus.Publish(new OnPlayerDamageRequestedEvent(player, _damage));
            Destroy(gameObject);
        }
    }
}
