using Core;
using GameFlow;
using GameFlow.Phase;
using Player;
using UnityEngine;

namespace UI.GameSceneUI
{
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance;
        
        public GameObject hud;
        public GameObject pausePanel;
        public GameObject rewardPanel;  
        public GameObject resultPanel;
        
        public PlayerController Player { get; private set; }
        
        [SerializeField] public StatTooltip statTooltip;
        [SerializeField] public UIStatSlot[] statSlots;

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
            GameController.Instance.OnPhaseChanged += OnPhaseChange;
        }
        
        private void OnDisable()
        {
            GameController.Instance.OnPhaseChanged -=  OnPhaseChange;   
        }

        public void PlayUI(GameObject ui)
        {
            hud.SetActive(false);
            pausePanel.SetActive(false);
            rewardPanel.SetActive(false);
            resultPanel.SetActive(false);

            if (ui == null)
                return;
            
            ui.SetActive(true);
        }
        
        public void Initialize(PlayerManager playerManager)
        {
            Player = playerManager.Player;
            
            foreach (var statSlot in statSlots)
            {
                statSlot.Initialize();
            }
            
            PlayUI(hud);
        }

        private void OnPhaseChange(GamePhaseType phase)
        {
            if (phase == GamePhaseType.Battle)
            {
                PlayUI(hud);
            }
            else if (phase == GamePhaseType.GameOver)
            {
                PlayUI(resultPanel);
            }
            else if (phase == GamePhaseType.Pause)
            {
                PlayUI(pausePanel);
            }
            else if (phase == GamePhaseType.RewardAndShop)
            {
                PlayUI(rewardPanel);
            }
            else if (phase == GamePhaseType.Preparing)
            {
                Initialize(GameController.Instance.PlayerManager);
            }
        }
    }
}
