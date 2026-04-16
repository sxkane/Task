using Core;
using UI.FadeScreenUI;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private FadeScreen fadeScreen;
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;

        private void OnEnable()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(OnStartButtonClick);
            exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        private void OnStartButtonClick()
        {
            GameRoot.Instance.EnterSelectScene();
        }

        private void OnExitButtonClick()
        {
            Application.Quit();
        }
    }
}