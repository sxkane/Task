using System;
using System.Collections.Generic;
using Audio;
using Enemy;
using Events;
using Events.EnemyEvents;
using GameAudio;
using Stats;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons.Melee
{
    public class MeleeWeaponBase : CooldownWeapon
    {
        [Header("Melee")]
        [SerializeField] private float weaponSpeed = 8f;

        private readonly HashSet<EnemyController> _hitEnemies = new();
        private Collider2D[] _hitColliders;
        private float _cooldownTimer;
        private bool _isThrusting;
        private bool _isReturning;
        private Vector2 _thrustDirection;
        private float _thrustDistance;
        private float _thrustTravel;
        private const float RangeScale = 0.03f;

        protected override void Update()
        {
            if (!IsWeaponActive || Stats == null)
                return;

            _cooldownTimer += Time.deltaTime;
            UpdateThrustMotion();

            var cooldown = RuntimeStats != null ? RuntimeStats.GetAttackInterval(Player?.Stats) : 0f;
            if (_cooldownTimer < cooldown || _isThrusting)
                return;

            if (TryStartAttack())
                _cooldownTimer = 0f;
        }

        public override void ResetRun()
        {
            base.ResetRun();
            StopThrust();
        }

        protected virtual void Awake()
        {
            _hitColliders = GetComponentsInChildren<Collider2D>(true);
            SetHitCollidersEnabled(true);
        }

        protected virtual void OnDisable()
        {
            StopThrust();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            TryHandleHit(collision);
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            TryHandleHit(collision);
        }

        protected virtual bool CanHitEnemy(EnemyController enemy)
        {
            if (enemy == null || enemy.Stats == null || !enemy.Stats.IsAlive)
                return false;

            var sqrDistance = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
            var attackRange = GetAttackRange();
            return sqrDistance <= attackRange * attackRange;
        }

        protected virtual void ExecuteAttackOnEnemy(EnemyController enemy, Vector2 attackDirection)
        {
            var isCritical = RollCriticalHit();
            var damage = CalculateMeleeDamage(isCritical);
            damage = ModifyDamageWithAbilities(enemy, enemy.transform.position, damage, isCritical);
            PublishMeleeDamage(enemy, damage, attackDirection, isCritical);
            NotifyAbilitiesHit(enemy, enemy.transform.position, damage, isCritical);
            OnEnemyHit(enemy, attackDirection, damage, isCritical);
        }

        protected virtual void OnEnemyHit(EnemyController enemy, Vector2 attackDirection, int damage, bool isCritical)
        {
        }

        protected int CalculateMeleeDamage(bool isCritical)
        {
            if (Player == null || Player.Stats == null || Stats == null)
                return 0;

            var playerStats = Player.Stats;
            var damage = RuntimeStats != null
                ? DamageCalculator.CalculateBaseDamage(playerStats, RuntimeStats)
                : DamageCalculator.CalculateBaseDamage(playerStats, Stats);

            var critDamage = RuntimeStats != null
                ? RuntimeStats.GetCritDamageMultiplier()
                : Mathf.Max(0f, Stats.critDamage);
            if (isCritical)
                damage = Mathf.RoundToInt(damage * critDamage);

            return damage;
        }

        protected bool RollCriticalHit()
        {
            if (Player == null || Player.Stats == null || Stats == null)
                return false;

            var playerCritChance = Player.Stats.CritChance;
            var weaponCritChance = RuntimeStats != null
                ? RuntimeStats.GetCritChanceRatio()
                : StatValueUtility.GetWeaponChance(Modifiers.WeaponStatType.CritChance, Stats.critChance);

            return Random.value < Mathf.Clamp01(playerCritChance + weaponCritChance);
        }

        protected void PublishMeleeDamage(EnemyController enemy, int damage, Vector2 attackDirection, bool isCritical = false)
        {
            var knockback = RuntimeStats != null ? RuntimeStats.GetKnockback() : Stats != null ? Stats.knockback : 0f;
            knockback += Player != null && Player.Stats != null ? Player.Stats.Knockback : 0f;
            EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, damage, attackDirection, knockback, isCritical, this));
            Player?.TryLifeStealOnHit();
        }

        protected override void Attack()
        {
        }

        private bool TryStartAttack()
        {
            if (EnemyManager == null || Player == null)
                return false;

            var primaryTarget = EnemyManager.GetNearestEnemy(Player.transform.position);
            if (primaryTarget == null)
                return false;

            var rawDirection = (Vector2)primaryTarget.transform.position - (Vector2)transform.position;
            var maxRange = GetAttackRange();
            if (rawDirection.sqrMagnitude > maxRange * maxRange)
                return false;

            _thrustDirection = rawDirection.normalized;
            _thrustDistance = maxRange;
            _thrustTravel = 0f;
            _isThrusting = true;
            _isReturning = false;

            _hitEnemies.Clear();
            FaceDirection(_thrustDirection);
            NotifyAbilitiesAttack();
            GlobalSfxPlayer.Instance.PlayWeaponAttack();
            return true;
        }

        private void UpdateThrustMotion()
        {
            if (!_isThrusting)
                return;

            var step = Mathf.Max(0.01f, weaponSpeed) * Time.deltaTime;

            if (!_isReturning)
            {
                _thrustTravel = Mathf.Min(_thrustTravel + step, _thrustDistance);
                SetRuntimeOffset(_thrustDirection * _thrustTravel);

                if (Mathf.Approximately(_thrustTravel, _thrustDistance))
                    _isReturning = true;

                return;
            }

            _thrustTravel = Mathf.Max(0f, _thrustTravel - step);
            SetRuntimeOffset(_thrustDirection * _thrustTravel);

            if (Mathf.Approximately(_thrustTravel, 0f))
                StopThrust();
        }

        private float GetAttackRange()
        {
            var runtimeRange = RuntimeStats != null ? RuntimeStats.GetRange(Player?.Stats) : Stats != null ? Stats.range : 0f;
            return Mathf.Max(0f, runtimeRange * RangeScale);
        }

        private void StopThrust()
        {
            _isThrusting = false;
            _isReturning = false;
            _thrustTravel = 0f;
            _thrustDistance = 0f;
            _hitEnemies.Clear();
            SetRuntimeOffset(Vector2.zero);
        }

        private void SetHitCollidersEnabled(bool colliderEnabled)
        {
            if (_hitColliders == null)
                return;

            for (var i = 0; i < _hitColliders.Length; i++)
            {
                var hitCollider = _hitColliders[i];
                if (hitCollider == null || !hitCollider.isTrigger)
                    continue;

                hitCollider.enabled = colliderEnabled;
            }
        }

        private static bool TryHitEnemy(Collider2D collision, out EnemyController enemy)
        {
            enemy = collision.GetComponent<EnemyController>();
            if (enemy == null)
                enemy = collision.GetComponentInParent<EnemyController>();
            return enemy != null;
        }

        private void TryHandleHit(Collider2D collision)
        {
            if (!_isThrusting)
                return;

            if (!TryHitEnemy(collision, out var enemy))
                return;

            if (!CanHitEnemy(enemy))
                return;

            if (_hitEnemies.Contains(enemy))
                return;

            _hitEnemies.Add(enemy);
            ExecuteAttackOnEnemy(enemy, transform.right);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var runtimeRange = RuntimeStats != null ? RuntimeStats.GetRange(Player?.Stats) : Stats != null ? Stats.range : 0f;
            Gizmos.DrawWireSphere(transform.position, RangeScale * runtimeRange);
        }
    }
}
