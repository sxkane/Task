using Enemy;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponProjectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 3f;

        protected PlayerController Player { get; private set; }
        protected WeaponStats Stats { get; private set; }
        protected EnemyManager EnemyManager { get; private set; }

        protected void InitializeProjectile(PlayerController player, WeaponStats stats, EnemyManager enemyManager = null)
        {
            CancelInvoke();
            Player = player;
            Stats = stats;
            EnemyManager = enemyManager;
            Invoke(nameof(ReturnToPool), lifetime);
        }

        protected int CalculateDamage()
        {
            if (Player == null || Player.Stats == null || Stats == null)
                return 0;

            var playerStats = Player.Stats;
            var damage = DamageCalculator.CalculateBaseDamage(playerStats, Stats);

            if (Random.value < playerStats.CritChance + Stats.critChance)
                damage = Mathf.RoundToInt(damage * Stats.critDamage);

            return damage;
        }

        protected bool TryHitEnemy(Collider2D collision, out EnemyController enemy)
        {
            enemy = collision.GetComponent<EnemyController>();
            return enemy != null;
        }

        protected void PublishDamage(EnemyController enemy, int damage, Vector2 knockbackDirection)
        {
            EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, damage, knockbackDirection, Stats != null ? Stats.knockback : 0f));
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

        protected virtual void OnDisable()
        {
            CancelInvoke();
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
