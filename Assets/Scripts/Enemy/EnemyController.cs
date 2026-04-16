using System;
using System.Diagnostics;
using Enemy.EnemyStates;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using TMPro;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Enemy Information")]
        [SerializeField] private EnemyStatTemplate template;
        [SerializeField] public LayerMask enemyLayer;
        
        [Header("States")]
        public EnemyStateMachine Machine { get; private set; }
        public EnemyMoveState MoveState { get; private set; }
        public EnemyAttackState AttackState { get; private set; }
        
        [Header("Components")]
        public Transform Transform { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        
        [Header("Stats")]
        public Transform Target { get; private set; }
        public EnemyStats Stats { get; private set; }
        public EnemyVisual Visual { get; private set; }
        public Attack.EnemyAttack Attack { get; private set; }
        public EnemyAnimationFunction Function { get; private set; }

        [Header("Others")]
        private EnemyManager _enemyManager;
        private bool _initialize;

        public void Initialize(Transform target, EnemyManager enemyManager, int currentWave)
        {
            // States
            Machine = new EnemyStateMachine();
            MoveState = new EnemyMoveState(this, Machine);
            AttackState = new EnemyAttackState(this, Machine);
            
            // Components
            Transform = GetComponent<Transform>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponentInChildren<Animator>();
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            // Stats
            Target = target;
            _enemyManager = enemyManager;
            Stats = new EnemyStats();
            Attack = GetComponent<Attack.EnemyAttack>();
            Visual = GetComponentInChildren<EnemyVisual>();
            Function = GetComponentInChildren<EnemyAnimationFunction>();

            // Initialize
            _enemyManager.Register(this);
            
            Stats.Initialize(template, currentWave);
            Attack.Initialize(this);
            Visual.Initialize(this);
            Function.InitializeAnimationFunction(this);
            
            Machine.Initialize(MoveState);
            _initialize = true;
        }

        private void FixedUpdate()
        {
            if (!_initialize)
                return;
            
            Machine.currentState.FixedUpdate();
        }
        
        public void Update()
        {
            if (!_initialize)
                return;
            
            Machine.currentState.Update();
        }
        
        private void OnEnable()
        {
            EventBus.Subscribe<OnEnemyDamageRequestedEvent>(OnDamageRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnEnemyDamageRequestedEvent>(OnDamageRequested);
        }

        private void OnDamageRequested(OnEnemyDamageRequestedEvent e)
        {
            if (e.Target != this || !Stats.IsAlive)
                return;
            
            Stats.TakeDamage(e.Damage);
            EventBus.Publish(new OnEnemyDamagedEvent(this, Mathf.RoundToInt(e.Damage)));

            if (!Stats.IsAlive)
                Die();
        }
        
        public void TakeDamage(float damage)
        {
            EventBus.Publish(new OnEnemyDamageRequestedEvent(this, damage));
        }

        private void Die()
        {
            _enemyManager?.Unregister(this);
            EventBus.Publish(new OnEnemyDiedEvent(this));
            PoolManager.Instance.Despawn(gameObject);
        }

        public void ChangeState(EnemyStateEnum state)
        {
            EnemyState newState = state switch
            {
                EnemyStateEnum.Move => MoveState,
                EnemyStateEnum.Attack => AttackState,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
            Machine.ChangeState(newState);
        }
    }
}
