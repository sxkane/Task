using Core;
using Player;
using UI.GameSceneUI.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class PausePanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button returnToMenuButton;
        
        [Header("References")]
        [SerializeField] private AttributePageUI attributePage;

        private GameController _gameController;
        private GameRoot _gameRoot;

        private void OnEnable()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClick);

            if (returnToMenuButton != null)
                returnToMenuButton.onClick.AddListener(OnReturnToMenuButtonClick);
        }

        private void OnDisable()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(OnResumeButtonClick);

            if (returnToMenuButton != null)
                returnToMenuButton.onClick.RemoveListener(OnReturnToMenuButtonClick);
        }

        public void Configure(GameController gameController, GameRoot gameRoot)
        {
            _gameController = gameController;
            _gameRoot = gameRoot;
        }

        public void InitializeRun(PlayerController player)
        {
            attributePage.InitializeRun(player.Stats);
        }

        public void ResetRun()
        {
            attributePage.ResetRun();
        }

        private void OnResumeButtonClick()
        {
            _gameController?.ResumeFromPause();
        }

        private void OnReturnToMenuButtonClick()
        {
            _gameRoot?.ReturnToMainMenu();
        }
    }
}
