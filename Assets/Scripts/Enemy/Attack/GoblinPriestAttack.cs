using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Attack
{
    public class GoblinPriestAttack : EnemyAttack
    {
        [Header("Movement")]
        [SerializeField] private float orbitDistance = 4.5f;
        [SerializeField] private float moveDecisionCooldown = 0.35f;
        [SerializeField] private float orbitWeight = 1.2f;
        [SerializeField] private float seekWeight = 0.55f;

        [Header("Heal")]
        [SerializeField] private float healRadius = 4f;
        [SerializeField] private float healInterval = 2.8f;
        [SerializeField] private float baseHealAmount = 100f;
        [SerializeField] private float healPerWave = 10f;

        private readonly List<EnemyController> _targets = new();
        private float _healTimer;
        private float _moveTimer;
        private Vector2 _cachedDirection;

        public override bool UsesAttackState => false;
        public override bool ShouldStopMovementDuringAttack => false;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _healTimer = healInterval;
            _moveTimer = 0f;
            _cachedDirection = Vector2.zero;
        }

        protected override void Update()
        {
            base.Update();

            if (Enemy == null || Enemy.Lifecycle == null || !Enemy.Lifecycle.IsActive || Enemy.Context?.EnemyManager == null)
                return;

            _healTimer -= Time.deltaTime;
            if (_healTimer > 0f)
                return;

            _healTimer = healInterval;
            HealNearbyEnemies();
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
            _moveTimer -= Time.deltaTime;
            if (_moveTimer > 0f)
                return _cachedDirection;

            _moveTimer = moveDecisionCooldown;

            var toTarget = targetPosition - currentPosition;
            if (toTarget.sqrMagnitude <= 0.001f)
            {
                _cachedDirection = Vector2.zero;
                return _cachedDirection;
            }

            var distance = toTarget.magnitude;
            var tangent = new Vector2(-toTarget.y, toTarget.x).normalized;
            var seek = distance > orbitDistance
                ? toTarget.normalized * seekWeight
                : -toTarget.normalized * seekWeight * 0.5f;

            _cachedDirection = (tangent * orbitWeight + seek).normalized;
            return _cachedDirection;
        }

        private void HealNearbyEnemies()
        {
            Enemy.Context.EnemyManager.GetEnemiesInRadius(Enemy.Transform.position, healRadius, _targets);
            if (_targets.Count == 0)
                return;

            var healAmount = baseHealAmount + Mathf.Max(0, Enemy.CurrentWave - 1) * healPerWave;

            for (var i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                if (target == null || target == Enemy || !target.gameObject.activeInHierarchy || !target.Stats.IsAlive)
                    continue;

                target.Heal(healAmount);
            }
        }
    }
}
