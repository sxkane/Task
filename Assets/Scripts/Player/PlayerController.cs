using Events;
using Events.PlayerEvents;
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
        public int CurrentHp { get; private set; }
        public int MaxHp => Stats?.MaxHp ?? 0;
        public bool IsAlive => CurrentHp > 0;

        public Vector2 AimDirection { get; private set; } = Vector2.right;
        public bool FacingRight { get; private set; } = true;

        private void Update()
        {
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
            RuntimeData.InitializeRun();
            Input.Initialize();
            Move.Initialize();
            Visual.Initialize();
            CurrentHp = Stats.MaxHp;
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

            if (CurrentHp <= 0)
                EventBus.Publish(new OnPlayerDiedEvent(this));
        }
    }
}