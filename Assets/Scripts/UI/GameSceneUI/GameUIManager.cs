using Core;
using GameFlow;
using Player;
using UI.GameSceneUI.Reward;
using UI.WeaponDisplay;
using UnityEngine;

namespace UI.GameSceneUI
{
    public class GameUIManager : MonoBehaviour
    {
        #region Inspector

        [Header("Pages")]
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject resultPanel;

        [Header("Views")]
        [SerializeField] private HUD hudView;
        [SerializeField] private PausePanel pausePanelView;
        [SerializeField] private UpgradePanel upgradePanelView;
        [SerializeField] private ShopPanel shopPanelView;
        [SerializeField] private ResultPanel resultPanelView;

        #endregion

        #region Runtime

        public PlayerController Player { get; private set; }

        private GameController _gameController;

        #endregion

        private void OnDisable()
        {
            if (_gameController != null)
                _gameController.OnPhaseChanged -= OnPhaseChange;
        }

        public void Configure(GameController gameController)
        {
            if (_gameController != null)
                _gameController.OnPhaseChanged -= OnPhaseChange;

            _gameController = gameController;

            hudView = hudView != null ? hudView : hud != null ? hud.GetComponent<HUD>() : null;
            pausePanelView = pausePanelView != null ? pausePanelView : pausePanel != null ? pausePanel.GetComponent<PausePanel>() : null;
            upgradePanelView = upgradePanelView != null ? upgradePanelView : upgradePanel != null ? upgradePanel.GetComponent<UpgradePanel>() : null;
            shopPanelView = shopPanelView != null ? shopPanelView : shopPanel != null ? shopPanel.GetComponent<ShopPanel>() : null;
            resultPanelView = resultPanelView != null ? resultPanelView : resultPanel != null ? resultPanel.GetComponent<ResultPanel>() : null;

            if (_gameController != null)
                _gameController.OnPhaseChanged += OnPhaseChange;

            hudView?.Configure(gameController);
            pausePanelView?.Configure(gameController, gameController != null ? gameController.Root : null);
            upgradePanelView?.Configure(gameController);
            shopPanelView?.Configure(gameController);
            resultPanelView?.Configure(gameController, gameController != null ? gameController.Root : null);
        }

        public void InitializeRun(PlayerManager playerManager)
        {
            Player = playerManager != null ? playerManager.Player : null;

            hudView?.InitializeRun(Player);
            upgradePanelView?.InitializeRun(Player);
            shopPanelView?.InitializeRun(Player != null ? Player.RuntimeData : null);
            pausePanelView?.InitializeRun(Player);

            PlayUI(hud);
        }

        public void ResetRun()
        {
            hudView?.ResetRun();
            upgradePanelView?.ResetRun();
            shopPanelView?.ResetRun();
            pausePanelView?.ResetRun();
            Player = null;
        }

        public void PlayUI(GameObject targetUi)
        {
            if (hud != null)
                hud.SetActive(false);

            if (pausePanel != null)
                pausePanel.SetActive(false);

            if (upgradePanel != null)
                upgradePanel.SetActive(false);

            if (shopPanel != null)
                shopPanel.SetActive(false);

            if (resultPanel != null)
                resultPanel.SetActive(false);

            if (targetUi != null)
                targetUi.SetActive(true);
        }

        private void OnPhaseChange(GamePhaseType phase)
        {
            switch (phase)
            {
                case GamePhaseType.Battle:
                    PlayUI(hud);
                    break;
                case GamePhaseType.GameOver:
                    resultPanelView?.RefreshResult();
                    PlayUI(resultPanel);
                    break;
                case GamePhaseType.Pause:
                    PlayUI(pausePanel);
                    break;
                case GamePhaseType.Upgrade:
                    PlayUI(upgradePanel);
                    break;
                case GamePhaseType.Shop:
                    PlayUI(shopPanel);
                    break;
                case GamePhaseType.Preparing:
                    PlayUI(hud);
                    break;
            }
        }
    }
}
