using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Attack
{
    public class UndeadMageAttack : EnemyAttack
    {
        [Header("Movement")]
        [SerializeField] private float fleeDistance = 6f;
        [SerializeField] private float idleDistance = 8f;
        [SerializeField] private float movementDecisionCooldown = 0.4f;

        [Header("Aura")]
        [SerializeField] private float auraRadius = 6f;
        [SerializeField] private float refreshInterval = 0.2f;

        private readonly List<EnemyController> _results = new();
        private readonly List<EnemyController> _toRemove = new();
        private readonly HashSet<EnemyController> _buffedTargets = new();
        private float _refreshTimer;
        private float _movementDecisionTimer;
        private Vector2 _cachedMoveDirection;

        public override bool UsesAttackState => false;
        public override bool ShouldStopMovementDuringAttack => false;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _refreshTimer = 0f;
            _movementDecisionTimer = 0f;
            _cachedMoveDirection = Vector2.zero;
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
            _movementDecisionTimer -= Time.deltaTime;
            if (_movementDecisionTimer > 0f)
                return _cachedMoveDirection;

            _movementDecisionTimer = movementDecisionCooldown;

            var away = currentPosition - targetPosition;
            var distance = away.magnitude;
            var nextDirection = Vector2.zero;

            if (distance < idleDistance && away.sqrMagnitude > 0.0001f)
                nextDirection = away.normalized;

            _cachedMoveDirection = Movement.EnemyWorldBounds.ClampDirection(currentPosition, nextDirection, 0.2f);
            return _cachedMoveDirection;
        }

        public override float GetMovementSpeedMultiplier(float distanceToTarget)
        {
            return distanceToTarget < idleDistance ? 1f : 0f;
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
                    target.RemoveUndeadMageSource(Enemy);
                _buffedTargets.Remove(target);
            }

            for (var i = 0; i < _results.Count; i++)
            {
                var target = _results[i];
                if (target == null || target == Enemy || !target.gameObject.activeInHierarchy || target.Stats == null || !target.Stats.IsAlive)
                    continue;

                if (_buffedTargets.Add(target))
                    target.AddUndeadMageSource(Enemy);
            }
        }

        private void OnDisable()
        {
            CleanupBuffs();
        }

        private void CleanupBuffs()
        {
            if (_buffedTargets.Count == 0)
                return;

            foreach (var target in _buffedTargets)
            {
                if (target != null)
                    target.RemoveUndeadMageSource(Enemy);
            }

            _buffedTargets.Clear();
        }
    }
}
