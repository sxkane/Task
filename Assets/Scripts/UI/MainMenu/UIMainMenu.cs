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

        private void OnEnable()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void OnStartButtonClick()
        {
            GameRoot.Instance.EnterSelectScene();
        }

        
    }
}