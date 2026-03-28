using System;
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
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        public GameFlowStateMachine StateMachine { get; private set; }
        public PlayerData SelectedPlayer { get; private set; }
        public PlayerManager PlayerManager { get; private set; }
        public WeaponManager WeaponManager { get; private set; }
        public ItemManager ItemManager { get; private set; }
        public WaveManager WaveManager { get; private set; }
        public RewardManger RewardManager { get; private set; }

        public event Action<GamePhaseType> OnPhaseChanged;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            StateMachine = new GameFlowStateMachine();
            
            PlayerManager = GetComponent<PlayerManager>();
            WeaponManager = GetComponent<WeaponManager>();
            ItemManager = GetComponent<ItemManager>();
            WaveManager = GetComponent<WaveManager>();
            RewardManager = GetComponent<RewardManger>();

            SelectedPlayer = GameRoot.Instance.CurrentSession.SelectedPlayer;
            StateMachine.Initialize(new PreparingPhase(this));
            OnPhaseChanged?.Invoke(GamePhaseType.Preparing);
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

        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void Resume()
        {
            Time.timeScale = 1;
        }
    }
}