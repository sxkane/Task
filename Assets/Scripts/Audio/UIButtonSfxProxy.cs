using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameAudio
{
    public class UIButtonSfxProxy : MonoBehaviour, IPointerEnterHandler
    {
        private Button _button;

        public void Configure(Button button)
        {
            if (_button == button)
                return;

            if (_button != null)
                _button.onClick.RemoveListener(HandleClick);

            _button = button;

            if (_button != null)
                _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(HandleClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GlobalSfxPlayer.Instance.PlayButtonHover();
        }

        private void HandleClick()
        {
            GlobalSfxPlayer.Instance.PlayButtonClick();
        }
    }
}
