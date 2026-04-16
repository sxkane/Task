using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private Button button;

        private GameRoot _gameRoot;

        private void OnEnable()
        {
            button.onClick.AddListener(OnBackButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnBackButtonClick);
        }

        public void Configure(GameRoot gameRoot)
        {
            _gameRoot = gameRoot;
        }

        public void OnBackButtonClick()
        {
            _gameRoot?.ReturnToMainMenu();
        }
    }
}
