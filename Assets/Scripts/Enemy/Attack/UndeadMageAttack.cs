using System.Collections.Generic;
using Enemy.Buffs;
using Stats;
using UnityEngine;

namespace Enemy.Attack
{
    public class UndeadMageAttack : EnemyAttack
    {
        [Header("Movement")]
        [SerializeField] private float fleeDistance = 6f;
        [SerializeField] private float idleDistance = 8f;
        [SerializeField] private float teleportCooldown = 0.4f;
        [SerializeField] private float minTeleportDistance = 2.5f;
        [SerializeField] private float maxTeleportDistance = 4.5f;
        [SerializeField] private float teleportScatterRadius = 1f;

        [Header("Aura")]
        [SerializeField] private float auraRadius = 6f;
        [SerializeField] private float refreshInterval = 0.2f;
        [SerializeField] private EnemyBuffData auraBuffData;

        private readonly List<EnemyController> _results = new();
        private readonly List<EnemyController> _toRemove = new();
        private readonly HashSet<EnemyController> _buffedTargets = new();
        private float _refreshTimer;
        private float _teleportTimer;
        private EnemyBuffData _runtimeAuraBuffData;

        public override bool UsesAttackState => false;
        public override bool ShouldStopMovementDuringAttack => false;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _refreshTimer = 0f;
            _teleportTimer = 0f;
            EnsureAuraBuffData();
            CleanupBuffs();
        }

        protected override void Update()
        {
            base.Update();

            if (Enemy == null || Enemy.Lifecycle == null || !Enemy.Lifecycle.IsActive || Enemy.Context?.EnemyManager == null)
            {
                CleanupBuffs();
                return;
            }

            UpdateTeleport();

            _refreshTimer -= Time.deltaTime;
            if (_refreshTimer > 0f)
                return;

            _refreshTimer = refreshInterval;
            RefreshAura();
        }

        public override bool CanAttack()
        {
            return false;
        }

        protected override void ExecuteAttack()
        {
        }

        public override Vector2 GetMovementDirection(Vector2 currentPosition, Vector2 targetPosition)
        {
            return Vector2.zero;
        }

        public override float GetMovementSpeedMultiplier(float distanceToTarget)
        {
            return 0f;
        }

        private void UpdateTeleport()
        {
            if (Enemy.Target == null || Enemy.Rigidbody == null)
                return;

            _teleportTimer -= Time.deltaTime;
            if (_teleportTimer > 0f)
                return;

            var currentPosition = Enemy.Rigidbody.position;
            var targetPosition = (Vector2)Enemy.Target.position;
            var away = currentPosition - targetPosition;
            var distance = away.magnitude;

            if (distance >= idleDistance)
                return;

            _teleportTimer = teleportCooldown;
            TeleportAway(currentPosition, targetPosition, away);
        }

        private void TeleportAway(Vector2 currentPosition, Vector2 targetPosition, Vector2 away)
        {
            var awayDirection = away.sqrMagnitude <= 0.0001f
                ? Random.insideUnitCircle.normalized
                : away.normalized;

            if (awayDirection.sqrMagnitude <= 0.0001f)
                awayDirection = Vector2.right;

            var teleportDistance = Random.Range(minTeleportDistance, maxTeleportDistance);
            var candidate = currentPosition
                            + awayDirection * teleportDistance
                            + Random.insideUnitCircle * teleportScatterRadius;
            candidate = Movement.EnemyWorldBounds.Clamp(candidate);

            var toTarget = candidate - targetPosition;
            if (toTarget.magnitude < fleeDistance)
            {
                var fallbackDirection = toTarget.sqrMagnitude <= 0.0001f ? awayDirection : toTarget.normalized;
                candidate = targetPosition + fallbackDirection * fleeDistance;
                candidate = Movement.EnemyWorldBounds.Clamp(candidate);
            }

            Enemy.Rigidbody.position = candidate;
            Enemy.Transform.position = candidate;
            Enemy.Rigidbody.linearVelocity = Vector2.zero;
        }

        private void RefreshAura()
        {
            Enemy.Context.EnemyManager.GetEnemiesInRadius(Enemy.Transform.position, auraRadius, _results);
            _toRemove.Clear();

            foreach (var buffed in _buffedTargets)
            {
                if (buffed == null || !_results.Contains(buffed) || buffed == Enemy || !buffed.gameObject.activeInHierarchy || !buffed.Stats.IsAlive)
                    _toRemove.Add(buffed);
            }

            for (var i = 0; i < _toRemove.Count; i++)
            {
                var target = _toRemove[i];
                if (target != null)
                {
                    target.RemoveUndeadMageSource(Enemy);
                    target.RemoveBuffsFromSource(Enemy);
                }

                _buffedTargets.Remove(target);
            }

            for (var i = 0; i < _results.Count; i++)
            {
                var target = _results[i];
                if (target == null || target == Enemy || !target.gameObject.activeInHierarchy || target.Stats == null || !target.Stats.IsAlive)
                    continue;

                if (_buffedTargets.Add(target))
                {
                    target.AddUndeadMageSource(Enemy);
                    target.ApplyBuff(GetAuraBuffData(), Enemy);
                }
                else
                {
                    target.ApplyBuff(GetAuraBuffData(), Enemy);
                }
            }
        }

        private void OnDisable()
        {
            CleanupBuffs();

            if (_runtimeAuraBuffData != null)
            {
                Destroy(_runtimeAuraBuffData);
                _runtimeAuraBuffData = null;
            }
        }

        private void CleanupBuffs()
        {
            if (_buffedTargets.Count == 0)
                return;

            foreach (var target in _buffedTargets)
            {
                if (target != null)
                {
                    target.RemoveUndeadMageSource(Enemy);
                    target.RemoveBuffsFromSource(Enemy);
                }
            }

            _buffedTargets.Clear();
        }

        private EnemyBuffData GetAuraBuffData()
        {
            return auraBuffData != null ? auraBuffData : _runtimeAuraBuffData;
        }

        private void EnsureAuraBuffData()
        {
            if (auraBuffData != null || _runtimeAuraBuffData != null)
                return;

            _runtimeAuraBuffData = ScriptableObject.CreateInstance<EnemyBuffData>();
            _runtimeAuraBuffData.name = "UndeadMageAuraRuntime";
            _runtimeAuraBuffData.hideFlags = HideFlags.HideAndDontSave;
            _runtimeAuraBuffData.InitializeRuntime(
                "enemy.undead_mage_aura",
                refreshInterval * 2f + 0.1f,
                true,
                new List<EnemyStatModifierDefinition>
                {
                    new() { statType = EnemyStatType.MaxHP, value = 1.5f, modifierType = StatModType.PercentMult },
                    new() { statType = EnemyStatType.Damage, value = 0.25f, modifierType = StatModType.PercentMult },
                    new() { statType = EnemyStatType.MoveSpeed, value = 0.5f, modifierType = StatModType.PercentMult }
                });
        }
    }
}
