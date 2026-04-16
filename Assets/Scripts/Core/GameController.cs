using System;
using System.Collections.Generic;
using Events;
using Events.EnemyEvents;
using GameFlow;
using GameFlow.Phase;
using Player;
using Rewards.Shops;
using Rewards.Upgrades;
using UI.GameSceneUI;
using UnityEngine;
using Waves;
using Weapons;
using Weapons.Items;

namespace Core
{
    public class GameController : MonoBehaviour
    {
        #region Singleton

        public static GameController Instance;

        #endregion

        #region Inspector

        [Header("Scene References")]
        [SerializeField] private GameSceneContext sceneContext;
        [SerializeField] private GameInputHandler gameInputHandler;
        [SerializeField] private GameUIManager gameUIManager;

        [Header("Managers")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private ItemManager itemManager;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private UpgradeManager upgradeManager;
        [SerializeField] private ShopManager shopManager;

        [Header("Pause")]
        [SerializeField] private float pauseToggleCooldown = 0.18f;

        #endregion

        #region Runtime

        public GameFlowStateMachine StateMachine { get; private set; }
        public PlayerData SelectedPlayer { get; private set; }
        public List<WeaponEntry> SelectedWeapons { get; private set; }
        public GameInputHandler GameInputHandler { get; private set; }
        public GameRoot Root { get; private set; }
        public GameSceneContext SceneContext => sceneContext;
        public GameSession Session { get; private set; }

        public PlayerManager PlayerManager { get; private set; }
        public WeaponManager WeaponManager { get; private set; }
        public ItemManager ItemManager { get; private set; }
        public WaveManager WaveManager { get; private set; }
        public UpgradeManager UpgradeManager { get; private set; }
        public ShopManager ShopManager { get; private set; }

        public event Action<GamePhaseType> OnPhaseChanged;

        private float _nextPauseToggleTime;

        #endregion

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
            Root = GameRoot.Instance;
            Session = Root != null ? Root.CurrentSession : null;

            if (Session == null || !Session.IsValid())
            {
                Debug.LogError("GameController.Start could not find a valid GameSession.");
                return;
            }

            Session.ConfigureSceneRoots(sceneContext);
            Configure(Session);
            
            BeginPhase(GamePhaseType.Preparing);
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

        #region Lifecycle

        public void Configure(GameSession session)
        {
            Session = session;
            StateMachine = new GameFlowStateMachine();

            PlayerManager = playerManager != null ? playerManager : GetComponent<PlayerManager>();
            WeaponManager = weaponManager != null ? weaponManager : GetComponent<WeaponManager>();
            ItemManager = itemManager != null ? itemManager : GetComponent<ItemManager>();
            WaveManager = waveManager != null ? waveManager : GetComponent<WaveManager>();
            UpgradeManager = upgradeManager != null ? upgradeManager : GetComponent<UpgradeManager>();
            ShopManager = shopManager != null ? shopManager : GetComponent<ShopManager>();
            GameInputHandler = gameInputHandler != null ? gameInputHandler : GetComponent<GameInputHandler>();
            gameUIManager = gameUIManager != null ? gameUIManager : GetComponentInChildren<GameUIManager>(true);
            
            PlayerManager?.Configure(Session);
            WaveManager?.Configure(Session, PlayerManager);
            WeaponManager?.Configure(Session, PlayerManager, WaveManager);
            ItemManager?.Configure(PlayerManager);
            UpgradeManager?.Configure(PlayerManager, WaveManager);
            ShopManager?.Configure(PlayerManager, WeaponManager, WaveManager);
            GameInputHandler?.Configure(this);
            gameUIManager?.Configure(this);
        }

        public void InitializeRun()
        {
            if (Session == null || !Session.IsValid())
            {
                Debug.LogError("GameController.InitializeRun called without a valid GameSession.");
                return;
            }

            SelectedPlayer = Session.SelectedPlayer;
            SelectedWeapons = Session.SelectedWeapons ?? new List<WeaponEntry>();

            PlayerManager?.InitializeRun(SelectedPlayer);
            WaveManager?.InitializeRun();
            WeaponManager?.InitializeRun(SelectedWeapons);
            ItemManager?.InitializeRun();
            UpgradeManager?.InitializeRun();
            ShopManager?.InitializeRun();
            GameInputHandler?.InitializeRun();
            gameUIManager?.InitializeRun(PlayerManager);

            EventBus.Subscribe<OnEnemyDiedEvent>(OnEnemyDied);
        }

        public void BeginPhase(GamePhaseType phaseType)
        {
            StateMachine.Initialize(CreatePhase(phaseType));
            OnPhaseChanged?.Invoke(phaseType);
        }

        public void ResetRun()
        {
            if (StateMachine?.CurrentPhase != null)
                StateMachine.CurrentPhase.Exit();

            EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);

            gameUIManager?.ResetRun();
            ShopManager?.ResetRun();
            UpgradeManager?.ResetRun();
            WeaponManager?.ResetRun();
            ItemManager?.ResetRun();
            WaveManager?.ResetRun();
            PlayerManager?.ResetRun();
            GameInputHandler?.ResetRun();

            _nextPauseToggleTime = 0f;
            StateMachine = null;
            SelectedPlayer = null;
            SelectedWeapons = null;
        }

        #endregion

        public void ChangeState(GamePhaseType phaseType)
        {
            OnPhaseChanged?.Invoke(phaseType);
            StateMachine.ChangePhase(CreatePhase(phaseType));
        }

        #region Phase

        private GamePhase CreatePhase(GamePhaseType phaseType)
        {
            return phaseType switch
            {
                GamePhaseType.Battle => new BattlePhase(this),
                GamePhaseType.Upgrade => new UpgradePhase(this),
                GamePhaseType.Shop => new ShopPhase(this),
                GamePhaseType.GameOver => new GameOverPhase(this),
                GamePhaseType.Preparing => new PreparingPhase(this),
                GamePhaseType.Pause => new PausePhase(this),
                _ => throw new Exception("Invalid state")
            };
        }

        #endregion

        #region Pause

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
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            Time.timeScale = 1f;
        }

        #endregion

        #region Player Input

        public void EnablePlayerInput()
        {
            PlayerManager?.Player?.Input?.EnableInput();
        }

        public void DisablePlayerInput()
        {
            PlayerManager?.Player?.Input?.DisableInput();
        }

        #endregion

        private void OnEnemyDied(OnEnemyDiedEvent eventData)
        {
            var runtimeData = PlayerManager?.Player?.RuntimeData;
            if (runtimeData == null || eventData.Target == null)
                return;

            runtimeData.AddCoins(Mathf.RoundToInt(eventData.Target.Stats.CoinReward));
            runtimeData.AddExperience(Mathf.RoundToInt(eventData.Target.Stats.ExpReward));
        }
    }
}
