using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Movement
{
    public sealed class EnemyMotor
    {
        private readonly EnemyController _enemy;
        private readonly SteeringProfile _profile;
        private readonly ContactFilter2D _filter;
        private readonly List<Collider2D> _results = new();
        private Vector2 _knockbackVelocity;

        public EnemyMotor(EnemyController enemy, SteeringProfile profile, LayerMask enemyLayer)
        {
            _enemy = enemy;
            _profile = profile ?? new SteeringProfile();
            _filter = new ContactFilter2D();
            _filter.SetLayerMask(enemyLayer);
            _filter.useTriggers = true;
        }

        public void Move(Vector2 desiredDirection, float speedMultiplier = 1f)
        {
            if (_enemy?.Rigidbody == null || _enemy.Stats == null)
                return;

            desiredDirection = EnemyWorldBounds.ClampDirection(_enemy.Rigidbody.position, desiredDirection);

            var velocity = SteeringSolver.Resolve(
                _enemy.Rigidbody,
                desiredDirection,
                _enemy.Stats.MoveSpeed * Mathf.Max(0f, speedMultiplier),
                _profile,
                _filter,
                _results);

            _enemy.Rigidbody.linearVelocity = velocity + _knockbackVelocity;
            DecayKnockback();
        }

        public void MoveRaw(Vector2 desiredDirection, float speedMultiplier = 1f)
        {
            if (_enemy?.Rigidbody == null || _enemy.Stats == null)
                return;

            desiredDirection = EnemyWorldBounds.ClampDirection(_enemy.Rigidbody.position, desiredDirection);
            var finalDirection = desiredDirection.sqrMagnitude <= 0.0001f
                ? Vector2.zero
                : desiredDirection.normalized;

            _enemy.Rigidbody.linearVelocity = finalDirection * (_enemy.Stats.MoveSpeed * Mathf.Max(0f, speedMultiplier)) + _knockbackVelocity;
            DecayKnockback();
        }

        public void Stop(float damping = 0.1f)
        {
            if (_enemy?.Rigidbody == null)
                return;

            _enemy.Rigidbody.linearVelocity = _enemy.Rigidbody.linearVelocity * Mathf.Clamp01(damping) + _knockbackVelocity;
            DecayKnockback();
        }

        public void ClampPositionToBounds()
        {
            if (_enemy?.Rigidbody == null || !EnemyWorldBounds.IsConfigured)
                return;

            var clampedPosition = EnemyWorldBounds.Clamp(_enemy.Rigidbody.position);
            var currentVelocity = _enemy.Rigidbody.linearVelocity;

            if (!Mathf.Approximately(clampedPosition.x, _enemy.Rigidbody.position.x))
                currentVelocity.x = 0f;

            if (!Mathf.Approximately(clampedPosition.y, _enemy.Rigidbody.position.y))
                currentVelocity.y = 0f;

            _enemy.Rigidbody.position = clampedPosition;
            _enemy.Rigidbody.linearVelocity = currentVelocity;
        }

        public void ApplyKnockback(Vector2 direction, float force, float resistance)
        {
            if (_enemy?.Rigidbody == null || force <= 0f || direction.sqrMagnitude <= 0.0001f)
                return;

            var finalForce = force * Mathf.Clamp01(1f - resistance);
            if (finalForce <= 0.001f)
                return;

            _knockbackVelocity += direction.normalized * finalForce;
            _enemy.Rigidbody.linearVelocity += direction.normalized * finalForce;
        }

        private void DecayKnockback()
        {
            _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, 10f * Time.fixedDeltaTime);
        }
    }
}
