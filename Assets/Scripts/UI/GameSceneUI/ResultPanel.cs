using System;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private Button button;
        
        private void OnEnable()
        {
            button.onClick.AddListener(OnBackButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnBackButtonClick);
        }

        public void OnBackButtonClick()
        {
            GameRoot.Instance.ReturnToMainMenu();
        }
    }
}
