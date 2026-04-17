using Events;
using ObjectPool;
using UnityEngine;

namespace Enemy.Attack
{
    public class DemonBossOrbitProjectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float contactDamage = 8f;

        private Transform _owner;
        private float _angle;
        private float _radius;
        private float _angularSpeed;

        public void Init(Transform owner, float angle, float radius, float angularSpeed)
        {
            _owner = owner;
            _angle = angle;
            _radius = radius;
            _angularSpeed = angularSpeed;
            UpdatePosition();
        }

        public void OnSpawned()
        {
        }

        public void OnDespawned()
        {
            _owner = null;
        }

        private void Update()
        {
            if (_owner == null || !_owner.gameObject.activeInHierarchy)
            {
                PoolManager.Instance.Despawn(gameObject);
                return;
            }

            _angle += _angularSpeed * Time.deltaTime;
            UpdatePosition();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponentInParent<Player.PlayerController>();
            if (player == null)
                return;

            EventBus.Publish(new OnPlayerDamageRequestedEvent(player, contactDamage));
        }

        private void UpdatePosition()
        {
            var offset = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle)) * _radius;
            transform.position = _owner.position + (Vector3)offset;
        }
    }
}
