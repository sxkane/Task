using System.Collections.Generic;
using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyMoveState : EnemyState
    {
        [Header("Neighbour Detection")]
        private float _neighborRadius = 0.8f;

        [Header("Boids Weights")]
        private float _separationWeight = 1.8f;
        private float _alignmentWeight = 0.4f;

        [Header("Components")]
        private Transform _transform;
        private Rigidbody2D _rb;
        private EnemyStats _stats;

        [Header("Player")]
        private Transform _target;
        
        [Header("Others")]
        private bool _initialize;

        private LayerMask enemyLayer;
        private readonly List<Collider2D> _results = new();
        private ContactFilter2D _filter;

        public EnemyMoveState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController,
            stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _transform = Enemy.transform;
            _rb = Enemy.Rigidbody;
            _stats = Enemy.Stats;

            _target = Enemy.Target;
            enemyLayer = Enemy.enemyLayer;
            Init();

            Enemy.Visual.Move(true);
        }

        private void Init()
        {
            _filter = new ContactFilter2D();
            _filter.SetLayerMask(enemyLayer);
            _filter.useTriggers = true;
        }

        public override void Update()
        {
            base.Update();
            
            if (Enemy.Attack.CanAttack())
            {
                Machine.ChangeState(Enemy.AttackState);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_target == null)
                return;

            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            int count = 0;

            _results.Clear();

            Physics2D.OverlapCircle(
                _transform.position,
                _neighborRadius,
                _filter,
                _results);
            
            foreach (var hit in _results)
            {
                var otherRb = hit.attachedRigidbody;

                if (otherRb == null || otherRb == _rb)
                    continue;

                Vector2 diff = _rb.position - otherRb.position;
                float dist = diff.magnitude;
                if (dist == 0) continue;

                // Separation
                separation += diff.normalized / dist;

                // Alignment
                alignment += otherRb.linearVelocity;

                count++;
            }

            if (count > 0)
            {
                separation /= count;
                alignment /= count;
            }

            Vector2 seek =
                ((Vector2)_target.position - _rb.position).normalized;

            Vector2 finalDir =
                seek
                + separation * _separationWeight
                + alignment.normalized * _alignmentWeight;

            finalDir.Normalize();

            _rb.linearVelocity = Vector2.Lerp(
                _rb.linearVelocity,
                finalDir * _stats.MoveSpeed,
                0.25f);
        }

        public override void Exit()
        {
            base.Exit();

            Enemy.Visual.Move(false);
        }
    }
}