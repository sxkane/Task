using UnityEngine;

namespace Enemy.Attack
{
    public class GiantRatAttack : SlimeAttack
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
        [SerializeField] private float chargeDuration = 0.45f;
        [SerializeField] private float dashDuration = 0.3f;
        [SerializeField] private float dashSpeedMultiplier = 4f;
        [SerializeField] private float dashTriggerRange = 6f;

        private DashPhase _phase;
        private float _phaseTimer;
        private float _dashCooldownTimer;
        private Vector2 _dashDirection;

        public override bool IgnoreSteering => _phase == DashPhase.Dashing;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _phase = DashPhase.None;
            _phaseTimer = 0f;
            _dashCooldownTimer = Random.Range(minDashCooldown, maxDashCooldown);
        }

        protected override void Update()
        {
            base.Update();

            if (Enemy == null || Enemy.Lifecycle == null || !Enemy.Lifecycle.IsActive || Target == null)
                return;

            switch (_phase)
            {
                case DashPhase.Charging:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                    {
                        _phase = DashPhase.Dashing;
                        _phaseTimer = dashDuration;
                    }
                    break;
                case DashPhase.Dashing:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                    {
                        _phase = DashPhase.None;
                        _dashCooldownTimer = Random.Range(minDashCooldown, maxDashCooldown);
                    }
                    break;
                default:
                    _dashCooldownTimer -= Time.deltaTime;
                    if (_dashCooldownTimer <= 0f && Vector2.Distance(transform.position, Target.position) <= dashTriggerRange)
                    {
                        _phase = DashPhase.Charging;
                        _phaseTimer = chargeDuration;
                        _dashDirection = ((Vector2)Target.position - (Vector2)transform.position).normalized;
                        if (_dashDirection.sqrMagnitude <= 0.001f)
                            _dashDirection = Vector2.right;
                    }
                    break;
            }
        }

        public override Vector2 GetMovementDirection(Vector2 currentPosition, Vector2 targetPosition)
        {
            return _phase == DashPhase.Dashing
                ? _dashDirection
                : base.GetMovementDirection(currentPosition, targetPosition);
        }

        public override float GetMovementSpeedMultiplier(float distanceToTarget)
        {
            return _phase switch
            {
                DashPhase.Charging => 0f,
                DashPhase.Dashing => dashSpeedMultiplier,
                _ => base.GetMovementSpeedMultiplier(distanceToTarget)
            };
        }
    }
}
