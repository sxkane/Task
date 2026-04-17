using System.Collections.Generic;
using ObjectPool;
using UnityEngine;

namespace Enemy.Attack
{
    public class DemonBossAttack : EnemyAttack
    {
        private enum DashPhase
        {
            None,
            Charging,
            Dashing
        }

        [Header("Dash")]
        [SerializeField] private float minDashCooldown = 2.5f;
        [SerializeField] private float maxDashCooldown = 3.5f;
        [SerializeField] private float chargeDuration = 0.55f;
        [SerializeField] private float dashDuration = 0.4f;
        [SerializeField] private float dashSpeedMultiplier = 4.5f;
        [SerializeField] private float dashTriggerRange = 10f;

        [Header("Orbit Projectiles")]
        [SerializeField] private GameObject orbitProjectilePrefab;
        [SerializeField] private int orbitProjectileCount = 4;
        [SerializeField] private float orbitRadius = 1.8f;
        [SerializeField] private float orbitAngularSpeed = 2.5f;

        [Header("Phase 2 Burst")]
        [SerializeField] private GameObject burstProjectilePrefab;
        [SerializeField] private int burstProjectileCount = 8;
        [SerializeField] private float minBurstCooldown = 1.8f;
        [SerializeField] private float maxBurstCooldown = 2.8f;

        private readonly List<GameObject> _orbitProjectiles = new();
        private DashPhase _dashPhase;
        private float _phaseTimer;
        private float _dashCooldownTimer;
        private float _burstCooldownTimer;
        private Vector2 _dashDirection;
        private bool _phaseTwo;

        public override bool UsesAttackState => false;
        public override bool ShouldStopMovementDuringAttack => false;
        public override bool IgnoreSteering => _dashPhase == DashPhase.Dashing;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _dashPhase = DashPhase.None;
            _phaseTimer = 0f;
            _dashCooldownTimer = Random.Range(minDashCooldown, maxDashCooldown);
            _burstCooldownTimer = Random.Range(minBurstCooldown, maxBurstCooldown);
            _phaseTwo = false;
            SpawnOrbitProjectiles();
        }

        protected override void Update()
        {
            base.Update();

            if (Enemy == null || Enemy.Lifecycle == null || !Enemy.Lifecycle.IsActive || Target == null)
                return;

            if (!_phaseTwo && Stats.CurrentHP <= Stats.MaxHP * 0.5f)
            {
                _phaseTwo = true;
                _dashPhase = DashPhase.None;
                _phaseTimer = 0f;
            }

            if (_phaseTwo)
            {
                UpdatePhaseTwo();
                return;
            }

            UpdateDashPhase();
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
            if (_phaseTwo)
                return (targetPosition - currentPosition).normalized;

            if (_dashPhase == DashPhase.Dashing)
                return _dashDirection;

            return (targetPosition - currentPosition).normalized;
        }

        public override float GetMovementSpeedMultiplier(float distanceToTarget)
        {
            if (_phaseTwo)
                return 1.1f;

            return _dashPhase switch
            {
                DashPhase.Charging => 0f,
                DashPhase.Dashing => dashSpeedMultiplier,
                _ => 1f
            };
        }

        private void UpdateDashPhase()
        {
            switch (_dashPhase)
            {
                case DashPhase.Charging:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                    {
                        _dashPhase = DashPhase.Dashing;
                        _phaseTimer = dashDuration;
                    }
                    break;
                case DashPhase.Dashing:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                    {
                        _dashPhase = DashPhase.None;
                        _dashCooldownTimer = Random.Range(minDashCooldown, maxDashCooldown);
                    }
                    break;
                default:
                    _dashCooldownTimer -= Time.deltaTime;
                    if (_dashCooldownTimer <= 0f && Vector2.Distance(Enemy.Transform.position, Target.position) <= dashTriggerRange)
                    {
                        _dashPhase = DashPhase.Charging;
                        _phaseTimer = chargeDuration;
                        _dashDirection = ((Vector2)Target.position - (Vector2)Enemy.Transform.position).normalized;
                        if (_dashDirection.sqrMagnitude <= 0.001f)
                            _dashDirection = Vector2.right;
                    }
                    break;
            }
        }

        private void UpdatePhaseTwo()
        {
            _burstCooldownTimer -= Time.deltaTime;
            if (_burstCooldownTimer > 0f)
                return;

            _burstCooldownTimer = Random.Range(minBurstCooldown, maxBurstCooldown);
            FireRadialBurst();
        }

        private void SpawnOrbitProjectiles()
        {
            CleanupOrbitProjectiles();

            if (orbitProjectilePrefab == null || Enemy == null)
                return;

            for (var i = 0; i < orbitProjectileCount; i++)
            {
                var angle = Mathf.PI * 2f * i / Mathf.Max(1, orbitProjectileCount);
                var projectile = PoolManager.Instance.Spawn(
                    orbitProjectilePrefab,
                    Enemy.Transform.position,
                    orbitProjectilePrefab.transform.rotation,
                    Enemy.Transform.parent);
                var orbit = projectile.GetComponent<DemonBossOrbitProjectile>();
                if (orbit != null)
                {
                    orbit.Init(Enemy.Transform, angle, orbitRadius, orbitAngularSpeed);
                    _orbitProjectiles.Add(projectile);
                }
            }
        }

        private void FireRadialBurst()
        {
            if (burstProjectilePrefab == null || Enemy == null)
                return;

            for (var i = 0; i < burstProjectileCount; i++)
            {
                var angle = Mathf.PI * 2f * i / Mathf.Max(1, burstProjectileCount);
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var projectile = PoolManager.Instance.Spawn(
                    burstProjectilePrefab,
                    Enemy.Transform.position,
                    burstProjectilePrefab.transform.rotation,
                    Enemy.Transform.parent);
                var demonProjectile = projectile.GetComponent<DemonBossProjectile>();
                if (demonProjectile != null)
                    demonProjectile.Init(direction, Stats.Damage);
            }
        }

        private void OnDisable()
        {
            CleanupOrbitProjectiles();
        }

        private void CleanupOrbitProjectiles()
        {
            for (var i = 0; i < _orbitProjectiles.Count; i++)
            {
                if (_orbitProjectiles[i] != null && _orbitProjectiles[i].activeInHierarchy)
                    PoolManager.Instance.Despawn(_orbitProjectiles[i]);
            }

            _orbitProjectiles.Clear();
        }
    }
}
