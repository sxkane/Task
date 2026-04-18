using System;
using System.Collections.Generic;
using Enemy.Core;
using Enemy.EnemyStates;
using Enemy.Movement;
using Enemy.UI;
using Enemy.Buffs;
using Core;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IPoolable
    {
        [Header("Enemy Information")]
        [SerializeField] private EnemyStatTemplate template;
        [SerializeField] public LayerMask enemyLayer;
        [SerializeField] private SteeringProfile steeringProfile = new();
        [SerializeField] private float spawnActivationDelay = 0.05f;
        [SerializeField] private float deathDespawnDelay = 0.45f;

        [Header("States")]
        public EnemyStateMachine Machine { get; private set; }
        public EnemySpawningState SpawningState { get; private set; }
        public EnemyMoveState MoveState { get; private set; }
        public EnemyAttackState AttackState { get; private set; }
        public EnemyDyingState DyingState { get; private set; }

        [Header("Components")]
        public Transform Transform { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }

        [Header("Stats")]
        public Transform Target { get; private set; }
        public EnemyStats Stats { get; private set; }
        public EnemyVisual Visual { get; private set; }
        public EnemyHealthBar HealthBar { get; private set; }
        public Attack.EnemyAttack Attack { get; private set; }
        public EnemyAnimationFunction Function { get; private set; }
        public EnemyContext Context { get; private set; }
        public EnemyLifecycle Lifecycle { get; private set; }
        public EnemyMotor Motor { get; private set; }
        public EnemyBuffController Buffs { get; private set; }
        public int CurrentWave { get; private set; }
        public float SpawnActivationDelay => spawnActivationDelay;
        public float DeathDespawnDelay => deathDespawnDelay;

        [Header("Others")]
        private EnemyManager _enemyManager;
        private Collider2D[] _colliders;
        private readonly HashSet<EnemyController> _undeadMageSources = new();
        private bool _initialize;
        private bool _deathHandled;
        private bool _deathAnimationFinished;

        public void Initialize(Transform target, EnemyManager enemyManager, int currentWave)
        {
            Machine = new EnemyStateMachine();
            SpawningState = new EnemySpawningState(this, Machine);
            MoveState = new EnemyMoveState(this, Machine);
            AttackState = new EnemyAttackState(this, Machine);
            DyingState = new EnemyDyingState(this, Machine);

            Transform = GetComponent<Transform>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponentInChildren<Animator>();
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _colliders = GetComponentsInChildren<Collider2D>(true);

            Target = target;
            _enemyManager = enemyManager;
            CurrentWave = currentWave;
            Stats = new EnemyStats();
            Attack = GetComponent<Attack.EnemyAttack>();
            Visual = GetComponentInChildren<EnemyVisual>();
            HealthBar = GetComponentInChildren<EnemyHealthBar>(true);
            Function = GetComponentInChildren<EnemyAnimationFunction>();

            _enemyManager.Register(this);

            Stats.Initialize(template, currentWave);
            Lifecycle = new EnemyLifecycle();
            Motor = new EnemyMotor(this, steeringProfile, enemyLayer);
            Buffs = new EnemyBuffController(Stats);
            Context = new EnemyContext(this, Transform, Rigidbody, Animator, SpriteRenderer, Target, _enemyManager, Stats);

            Attack.Initialize(this);
            Visual.Initialize(this);
            InitializeHealthBar();
            Function.InitializeAnimationFunction(this);

            _deathHandled = false;
            _deathAnimationFinished = false;
            Machine.Initialize(SpawningState);
            _initialize = true;
        }

        private void FixedUpdate()
        {
            if (!_initialize)
                return;

            Machine.currentState.FixedUpdate();
            Motor.ClampPositionToBounds();
        }

        public void Update()
        {
            if (!_initialize)
                return;

            Buffs?.Tick(Time.deltaTime);
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
            if (e.Target != this || !Stats.IsAlive || Lifecycle == null || !Lifecycle.CanTakeDamage)
                return;

            var willBeKilled = Stats.CurrentHP - e.Damage <= 0f;
            Stats.TakeDamage(e.Damage);
            EventBus.Publish(new OnEnemyDamagedEvent(this, Mathf.RoundToInt(e.Damage), !Stats.IsAlive, e.IsCritical));

            if (!willBeKilled && e.KnockbackForce > 0f)
                Motor.ApplyKnockback(e.KnockbackDirection, e.KnockbackForce, Stats.KnockbackResistance);

            if (!Stats.IsAlive)
                BeginDeath();
        }

        public void TakeDamage(float damage)
        {
            EventBus.Publish(new OnEnemyDamageRequestedEvent(this, damage));
        }

        public void Heal(float amount)
        {
            if (Stats == null || !Stats.IsAlive || amount <= 0f)
                return;

            Stats.Heal(amount);
        }

        public EnemyBuffInstance ApplyBuff(EnemyBuffData buffData, object source = null)
        {
            return Buffs?.ApplyBuff(buffData, source);
        }

        public void RemoveBuffsFromSource(object source)
        {
            Buffs?.RemoveBuffsFromSource(source);
        }

        public void ClearBuffs()
        {
            Buffs?.Clear();
        }

        private void BeginDeath()
        {
            if (_deathHandled)
                return;

            _deathHandled = true;
            _deathAnimationFinished = false;
            _enemyManager?.Unregister(this);
            EventBus.Publish(new OnEnemyDiedEvent(this));
            ChangeState(EnemyStateEnum.Dying);
        }

        public void ChangeState(EnemyStateEnum state)
        {
            EnemyState newState = state switch
            {
                EnemyStateEnum.Spawning => SpawningState,
                EnemyStateEnum.Move => MoveState,
                EnemyStateEnum.Attack => AttackState,
                EnemyStateEnum.Dying => DyingState,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

            Machine.ChangeState(newState);
        }

        public void EnterSpawningPhase()
        {
            Lifecycle.EnterSpawning();
            SetCombatEnabled(false);
            Motor.Stop(0f);
        }

        public void FinishSpawn()
        {
            if (GameController.Instance != null && GameController.Instance.IsWaveCompleting)
            {
                FinishDeath();
                return;
            }

            Lifecycle.EnterActive();
            SetCombatEnabled(true);
            ChangeState(EnemyStateEnum.Move);
        }

        public void EnterDyingPhase()
        {
            Lifecycle.EnterDying();
            SetCombatEnabled(false);
        }

        public void FinishDeath()
        {
            if (Lifecycle != null)
                Lifecycle.EnterDespawned();

            PoolManager.Instance.Despawn(gameObject);
        }

        public void MarkDeathAnimationFinished()
        {
            _deathAnimationFinished = true;
        }

        public bool IsDeathAnimationFinished()
        {
            return _deathAnimationFinished;
        }

        public void AddUndeadMageSource(EnemyController source)
        {
            if (source == null || source == this)
                return;

            if (_undeadMageSources.Add(source))
                RefreshUndeadMageBuff();
        }

        public void RemoveUndeadMageSource(EnemyController source)
        {
            if (source == null)
                return;

            if (_undeadMageSources.Remove(source))
                RefreshUndeadMageBuff();
        }

        public void ClearUndeadMageSources()
        {
            if (_undeadMageSources.Count == 0)
                return;

            _undeadMageSources.Clear();
            RefreshUndeadMageBuff();
        }

        public void SetCombatEnabled(bool combatEnabled)
        {
            if (_colliders == null)
                return;

            for (var i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null && _colliders[i].isTrigger)
                    _colliders[i].enabled = combatEnabled;
            }
        }

        public void OnSpawned()
        {
            _initialize = false;
            _deathHandled = false;
            _deathAnimationFinished = false;
            _undeadMageSources.Clear();
            
            if (Rigidbody != null)
                Rigidbody.linearVelocity = Vector2.zero;
            
            if (Animator != null)
            {
                Animator.Rebind();     
                Animator.Update(0f);  
            }
        }

        public void OnDespawned()
        {
            _initialize = false;
            _deathHandled = false;
            _deathAnimationFinished = false;
            ClearUndeadMageSources();

            if (Rigidbody != null)
                Rigidbody.linearVelocity = Vector2.zero;
        }

        private void RefreshUndeadMageBuff()
        {
            Visual?.SetBuffOutline(_undeadMageSources.Count > 0);
        }

        private void InitializeHealthBar()
        {
            HealthBar?.Initialize(this);
        }
    }
}
