using Events;
using ObjectPool;
using UnityEngine;

namespace Enemy.Attack
{
    public abstract class EnemyProjectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 4f;

        protected Vector2 Direction { get; private set; }
        protected float Damage { get; private set; }

        protected void InitializeProjectile(Vector2 direction, float damage)
        {
            CancelInvoke();
            Direction = direction.sqrMagnitude <= 0.001f ? Vector2.right : direction.normalized;
            Damage = damage;
            Invoke(nameof(ReturnToPool), lifetime);
        }

        protected void DealDamageToPlayer(Collider2D other)
        {
            var player = other.GetComponentInParent<Player.PlayerController>();
            if (player == null)
                return;

            EventBus.Publish(new OnPlayerDamageRequestedEvent(player, Damage));
            ReturnToPool();
        }

        public void OnSpawned()
        {
            CancelInvoke();
            OnProjectileSpawned();
        }

        public void OnDespawned()
        {
            CancelInvoke();
            OnProjectileDespawned();
        }

        protected virtual void OnProjectileSpawned()
        {
        }

        protected virtual void OnProjectileDespawned()
        {
        }

        protected void ReturnToPool()
        {
            PoolManager.Instance.Despawn(gameObject);
        }
    }
}
