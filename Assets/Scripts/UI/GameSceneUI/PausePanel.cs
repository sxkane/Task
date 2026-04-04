using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class PausePanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button returnToMenuButton;

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

        private void OnResumeButtonClick()
        {
            GameController.Instance?.ResumeFromPause();
        }

        private void OnReturnToMenuButtonClick()
        {
            GameRoot.Instance?.ReturnToMainMenu();
        }
    }
}
