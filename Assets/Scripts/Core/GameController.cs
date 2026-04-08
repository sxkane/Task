using System;
using System.Collections.Generic;
using Events;
using Events.EnemyEvents;
using GameFlow;
using GameFlow.Phase;
using Player;
using Rewards;
using UnityEngine;
using Waves;
using Weapons;
using Weapons.Items;

namespace Core
{
    /// <summary>
    /// 游戏流程控制类
    /// </summary>
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        
        [Header("Scene References")]
        [SerializeField] private GameSceneContext context;

        [Header("Pause")]
        [SerializeField] private float pauseToggleCooldown = 0.18f;
        
        public GameFlowStateMachine StateMachine { get; private set; }
        public PlayerData SelectedPlayer { get; private set; }
        public List<WeaponLoadoutEntry> SelectedWeapons { get; private set; }
        public GameInputHandler GameInputHandler { get; private set; }

        [Header("Game Manager")]
        public PlayerManager PlayerManager { get; private set; }
        public WeaponManager WeaponManager { get; private set; }
        public ItemManager ItemManager { get; private set; }
        public WaveManager WaveManager { get; private set; }
        public RewardManger RewardManager { get; private set; }

        public event Action<GamePhaseType> OnPhaseChanged;

        private float _nextPauseToggleTime;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            StateMachine = new GameFlowStateMachine();
            
            PlayerManager = GetComponent<PlayerManager>();
            WeaponManager = GetComponent<WeaponManager>();
            ItemManager = GetComponent<ItemManager>();
            WaveManager = GetComponent<WaveManager>();
            RewardManager = GetComponent<RewardManger>();
            GameInputHandler = GetComponent<GameInputHandler>();

            SelectedPlayer = GameRoot.Instance.CurrentSession.SelectedPlayer;
            SelectedWeapons = GameRoot.Instance.CurrentSession.SelectedWeapons;
            
            EventBus.Subscribe<OnEnemyDiedEvent>(OnEnemyDied);
            
            GameInputHandler?.Initialize();
            StateMachine.Initialize(new PreparingPhase(this));
            OnPhaseChanged?.Invoke(GamePhaseType.Preparing);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);
        }

        private void Update()
        {
            if (GameInputHandler == null || !GameInputHandler.ConsumePauseRequest())
                return;

            TogglePause();
        }

        public void ChangeState(GamePhaseType phaseType)
        {
            GamePhase gamePhase = phaseType switch
            {
                GamePhaseType.Battle => new BattlePhase(this),
                GamePhaseType.RewardAndShop => new RewardAndShopPhase(this),
                GamePhaseType.GameOver => new GameOverPhase(this),
                GamePhaseType.Preparing => new PreparingPhase(this),
                GamePhaseType.Pause => new PausePhase(this),
                _ => throw new Exception("Invalid state")
            };
            StateMachine.ChangePhase(gamePhase);
            OnPhaseChanged?.Invoke(phaseType);
        }

        public bool CanTogglePause()
        {
            if (Time.unscaledTime < _nextPauseToggleTime)
                return false;

            return StateMachine?.CurrentPhase is BattlePhase or PausePhase;
        }

        public void TogglePause()
        {
            if (!CanTogglePause())
                return;

            _nextPauseToggleTime = Time.unscaledTime + pauseToggleCooldown;

            if (StateMachine.CurrentPhase is BattlePhase)
                ChangeState(GamePhaseType.Pause);
            else if (StateMachine.CurrentPhase is PausePhase)
                ChangeState(GamePhaseType.Battle);
        }

        public void ResumeFromPause()
        {
            if (StateMachine?.CurrentPhase is not PausePhase)
                return;

            _nextPauseToggleTime = Time.unscaledTime + pauseToggleCooldown;
            ChangeState(GamePhaseType.Battle);
        }

        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void Resume()
        {
            Time.timeScale = 1;
        }

        public void EnablePlayerInput()
        {
            PlayerManager?.Player?.Input?.EnableInput();
        }

        public void DisablePlayerInput()
        {
            PlayerManager?.Player?.Input?.DisableInput();
        }

        private void OnEnemyDied(OnEnemyDiedEvent e)
        {
            var runtimeData = PlayerManager?.Player?.RuntimeData;
            if (runtimeData == null || e.Target == null)
                return;

            runtimeData.AddCoins(Mathf.RoundToInt(e.Target.Stats.CoinReward));
            runtimeData.AddExperience(Mathf.RoundToInt(e.Target.Stats.ExpReward));
        }
    }
}
