using Data.Text;
using Events;
using Events.ShopEvents;
using Rewards.Shops;
using TMPro;
using UI.GameSceneUI.IconSlots;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace UI.GameSceneUI.Reward
{
    public class RewardSlot : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Buttons")]
        [SerializeField] private Button lockedButton;
        [SerializeField] private Button buyButton;

        [Header("Image")]
        [SerializeField] private IconSlot slot;

        private ShopItem _item;
        private TextMeshProUGUI _lockButtonText;

        private void Awake()
        {
            _lockButtonText = lockedButton != null ? lockedButton.GetComponentInChildren<TextMeshProUGUI>() : null;
        }

        private void OnEnable()
        {
            lockedButton.onClick.AddListener(Lock);
            buyButton.onClick.AddListener(Buy);
        }

        private void OnDisable()
        {
            lockedButton.onClick.RemoveListener(Lock);
            buyButton.onClick.RemoveListener(Buy);
        }

        public void Show(ShopItem item)
        {
            _item = item;

            nameText.text = item.GetDisplayName();
            rewardText.text = UIValueBuilder.Price(item.GetPrice());
            slot.Set(item.GetIcon(), item.GetRarity());
            descriptionText.text = item.IsItem
                ? GameTextBuilder.BuildItem(item.itemData)
                :GameTextBuilder.BuildWeapon(item.GetWeaponEntry());

            RefreshLockState();
            gameObject.SetActive(true);
        }

        private void Lock()
        {
            if (_item == null)
                return;

            EventBus.Publish(new OnShopItemLockedEvent(_item));
            RefreshLockState();
        }

        private void Buy()
        {
            if (_item == null)
                return;

            EventBus.Publish(new OnShopPurchaseRequestedEvent(_item));
        }

        private void RefreshLockState(bool? overrideLocked = null)
        {
            if (_lockButtonText == null)
                return;

            var isLocked = overrideLocked ?? _item != null && _item.isLocked;
            _lockButtonText.text = UIValueBuilder.Lock(isLocked);
            _lockButtonText.color = isLocked ? StatTextBuilder.Negative : StatTextBuilder.Positive;
        }
    }
}
