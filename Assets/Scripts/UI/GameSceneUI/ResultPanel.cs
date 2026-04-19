using Core;
using Data.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;

        private GameController _gameController;
        private GameRoot _gameRoot;

        private void OnEnable()
        {
            button.onClick.AddListener(OnBackButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnBackButtonClick);
        }

        public void Configure(GameController gameController, GameRoot gameRoot)
        {
            _gameController = gameController;
            _gameRoot = gameRoot;
        }

        public void RefreshResult()
        {
            if (text == null)
                return;

            var isVictory = _gameController != null && _gameController.IsVictory;
            text.text = isVictory ? "Win" : "YouLoss";
            text.color = isVictory ? StatTextBuilder.Positive : StatTextBuilder.Negative;
        }

        public void OnBackButtonClick()
        {
            _gameRoot?.ReturnToMainMenu();
        }
    }
}
