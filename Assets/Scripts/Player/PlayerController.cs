using Events;
using Events.PlayerEvents;
using Stats.Buffs;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerInputHandler Input { get; private set; }
        public PlayerMove Move { get; private set; }
        public PlayerVisual Visual { get; private set; }
        public PlayerRuntimeData RuntimeData { get; private set; }
        
        [Header("Player Stats")]
        public PlayerStats Stats { get; private set; }
        public BuffController Buffs { get; private set; }
        public int CurrentHp { get; private set; }
        public int MaxHp => Stats?.MaxHp ?? 0;
        public bool IsAlive => CurrentHp > 0;

        public Vector2 AimDirection { get; private set; } = Vector2.right;
        public bool FacingRight { get; private set; } = true;

        private float _regenTimer;
        private float _nextLifeStealTime;

        private void Update()
        {
            Buffs?.Tick(Time.deltaTime);
            UpdateRegeneration();

            Vector2 move = Input.MoveInput;

            if (move != Vector2.zero)
            {
                AimDirection = move.normalized;

                if (move.x != 0)
                    FacingRight = move.x > 0;
            }
        }

        public void Initialize(PlayerStats stats)
        {
            Stats = stats;
            Input = GetComponent<PlayerInputHandler>();
            Move = GetComponent<PlayerMove>();
            Visual = GetComponentInChildren<PlayerVisual>();
            RuntimeData = new PlayerRuntimeData();

            Stats.Initialize();
            Buffs = new BuffController(Stats);
            RuntimeData.InitializeRun();
            Input.Initialize();
            Move.Initialize();
            Visual.Initialize();
            CurrentHp = Stats.MaxHp;
            _regenTimer = 0f;
            _nextLifeStealTime = 0f;
            EventBus.Publish(new OnPlayerHealthChangedEvent(this));
        }

        public BuffInstance ApplyBuff(BuffData buffData, object source = null)
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

        public void RefillHealthToMax()
        {
            CurrentHp = MaxHp;
            EventBus.Publish(new OnPlayerHealthChangedEvent(this));
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || !IsAlive)
                return;

            CurrentHp = Mathf.Clamp(CurrentHp + amount, 0, MaxHp);
            EventBus.Publish(new OnPlayerHealthChangedEvent(this));
        }

        public void TryLifeStealOnHit()
        {
            if (!IsAlive || Time.time < _nextLifeStealTime)
                return;

            if (Random.value > Stats.LifeStealChance)
                return;

            Heal(1);
            _nextLifeStealTime = Time.time + 0.1f;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayerDamageRequestedEvent>(OnDamageRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnPlayerDamageRequestedEvent>(OnDamageRequested);
        }

        private void OnDamageRequested(OnPlayerDamageRequestedEvent e)
        {
            if (e.Target != this || !IsAlive)
                return;

            bool isDodged = Random.value < Stats.DodgeChance;
            int finalDamage = 0;

            if (!isDodged)
            {
                finalDamage = Mathf.Max(
                    1,
                    Mathf.RoundToInt(e.RawDamage * Stats.DamageTakenMultiplier));
                CurrentHp = Mathf.Clamp(CurrentHp - finalDamage, 0, Stats.MaxHp);
            }

            EventBus.Publish(new OnPlayerDamagedEvent(this, finalDamage, isDodged));
            EventBus.Publish(new OnPlayerHealthChangedEvent(this));

            if (CurrentHp <= 0)
                EventBus.Publish(new OnPlayerDiedEvent(this));
        }

        private void UpdateRegeneration()
        {
            if (!IsAlive || Stats == null || Stats.HpRegenPerSecond <= 0f)
                return;

            _regenTimer += Time.deltaTime * Stats.HpRegenPerSecond;
            while (_regenTimer >= 1f)
            {
                _regenTimer -= 1f;
                Heal(1);
            }
        }
    }
}
