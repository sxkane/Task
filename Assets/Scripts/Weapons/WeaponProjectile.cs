using Enemy;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;
using Weapons.Core;
using Weapons.Modifiers;

namespace Weapons
{
    public abstract class WeaponProjectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 3f;

        protected PlayerController Player { get; private set; }
        protected WeaponStats Stats { get; private set; }
        protected WeaponRuntimeStats RuntimeStats { get; private set; }
        protected EnemyManager EnemyManager { get; private set; }
        protected float MaxTravelDistance { get; private set; }

        private Vector3 _lastPosition;
        private float _travelledDistance;

        protected void InitializeProjectile(PlayerController player, WeaponStats stats, WeaponRuntimeStats runtimeStats, EnemyManager enemyManager = null)
        {
            CancelInvoke();
            Player = player;
            Stats = stats;
            RuntimeStats = runtimeStats;
            EnemyManager = enemyManager;
            MaxTravelDistance = RuntimeStats != null ? RuntimeStats.GetRange(player != null ? player.Stats : null) : stats != null ? stats.range : 0f;
            _lastPosition = transform.position;
            _travelledDistance = 0f;
            Invoke(nameof(ReturnToPool), lifetime);
        }

        protected int CalculateDamage()
        {
            if (Player == null || Player.Stats == null || Stats == null)
                return 0;

            var playerStats = Player.Stats;
            var damage = RuntimeStats != null
                ? DamageCalculator.CalculateBaseDamage(playerStats, RuntimeStats)
                : DamageCalculator.CalculateBaseDamage(playerStats, Stats);

            var critChance = RuntimeStats != null ? RuntimeStats.GetStat(WeaponStatType.CritChance).Value / 100f : Stats.critChance / 100f;
            var critDamage = RuntimeStats != null ? RuntimeStats.GetStat(WeaponStatType.CritDamage).Value : Stats.critDamage;
            if (Random.value < playerStats.CritChance + critChance)
                damage = Mathf.RoundToInt(damage * critDamage);

            return damage;
        }

        protected bool IsCriticalHit()
        {
            if (Player == null || Player.Stats == null || Stats == null)
                return false;

            var critChance = RuntimeStats != null ? RuntimeStats.GetStat(WeaponStatType.CritChance).Value / 100f : Stats.critChance / 100f;
            return Random.value < Player.Stats.CritChance + critChance;
        }

        protected bool TryHitEnemy(Collider2D collision, out EnemyController enemy)
        {
            enemy = collision.GetComponent<EnemyController>();
            return enemy != null;
        }

        protected void PublishDamage(EnemyController enemy, int damage, Vector2 knockbackDirection, bool isCritical = false)
        {
            var knockback = RuntimeStats != null ? RuntimeStats.GetKnockback() : Stats != null ? Stats.knockback : 0f;
            EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, damage, knockbackDirection, knockback, isCritical));
            Player?.TryLifeStealOnHit();
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

        protected void TrackTravel()
        {
            if (MaxTravelDistance <= 0f)
            {
                _lastPosition = transform.position;
                return;
            }

            _travelledDistance += Vector3.Distance(_lastPosition, transform.position);
            _lastPosition = transform.position;

            if (_travelledDistance >= MaxTravelDistance)
                ReturnToPool();
        }
    }
}
