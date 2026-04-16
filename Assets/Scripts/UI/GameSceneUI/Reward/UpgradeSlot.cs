using Events;
using Events.UpgradeEvents;
using Rewards.StatRewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI.Reward
{
    public class UpgradeSlot : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Visual")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Button selectButton;

        private RewardOption _option;

        private void OnEnable()
        {
            if (selectButton != null)
                selectButton.onClick.AddListener(Select);
        }

        private void OnDisable()
        {
            if (selectButton != null)
                selectButton.onClick.RemoveListener(Select);
        }

        public void Show(RewardOption option)
        {
            _option = option;

            if (titleText != null)
                titleText.text = option != null ? option.title : string.Empty;

            if (descriptionText != null)
                descriptionText.text = option != null ? option.description : string.Empty;

            if (iconImage != null)
                iconImage.sprite = option != null ? option.icon : null;

            gameObject.SetActive(true);
        }

        private void Select()
        {
            if (_option == null)
                return;

            EventBus.Publish(new OnUpgradeOptionSelectedEvent(_option));
        }
    }
}
