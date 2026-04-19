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

        [Header("Slots")]
        [SerializeField] private BonusDataSlot bonusSlot;
        [SerializeField] private BonusPanel bonusPanel;

        private ShopItem _item;
        private TextMeshProUGUI _lockButtonText;

        private void Awake()
        {
            _lockButtonText = lockedButton != null ? lockedButton.GetComponentInChildren<TextMeshProUGUI>() : null;
        }

        private void OnEnable()
        {
            if (lockedButton != null)
                lockedButton.onClick.AddListener(Lock);

            if (buyButton != null)
                buyButton.onClick.AddListener(Buy);
        }

        private void OnDisable()
        {
            if (lockedButton != null)
                lockedButton.onClick.RemoveListener(Lock);

            if (buyButton != null)
                buyButton.onClick.RemoveListener(Buy);
        }

        public void Show(ShopItem item)
        {
            _item = item;

            if (nameText != null)
                nameText.text = item.GetDisplayName();

            if (rewardText != null)
                rewardText.text = UIValueBuilder.Price(item.GetPrice());

            if (slot != null)
                slot.Set(item.GetIcon(), item.GetRarity());

            if (descriptionText != null)
            {
                descriptionText.text = item.IsItem
                    ? GameTextBuilder.BuildItem(item.itemData)
                    : GameTextBuilder.BuildWeapon(item.GetWeaponEntry());
            }

            RefreshDataBonus(item);
            RefreshLockState();
            gameObject.SetActive(true);
        }

        public void RefreshPriceState(bool canAfford)
        {
            if (rewardText == null)
                return;

            rewardText.color = canAfford ? StatTextBuilder.Positive : StatTextBuilder.Negative;
        }

        public ShopItem GetCurrentItem()
        {
            return _item;
        }

        private void RefreshDataBonus(ShopItem item)
        {
            if (bonusSlot == null)
                return;

            if (item != null && item.IsWeapon && item.weaponEntry?.weaponData != null)
                bonusSlot.Configure(item.weaponEntry.weaponData.bonusData, bonusPanel);
            else
                bonusSlot.Configure(null, bonusPanel);
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

            var isLocked = overrideLocked ?? (_item != null && _item.isLocked);
            _lockButtonText.text = UIValueBuilder.Lock(isLocked);
            _lockButtonText.color = isLocked ? StatTextBuilder.Negative : StatTextBuilder.Positive;
        }
    }
}
