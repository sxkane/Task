using Data;
using Events;
using Events.ShopEvents;
using Rewards.Shops;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace UI.GameSceneUI
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
        
        private ShopItem _item;
        
        public void Show(ShopItem item)
        {
            _item = item;
            
            if (item.IsItem)
            {
                nameText.text = item.GetDisplayName();
                rewardText.text = item.itemData.price.ToString();
                descriptionText.text = GameDataTextBuilder.BuildItemDescription(item.itemData);
            }
            else
            {
                var weaponStats = item.weaponEntry.GetStats();
                nameText.text = item.GetDisplayName();
                rewardText.text = weaponStats.price.ToString();
                descriptionText.text = GameDataTextBuilder.BuildWeaponDescription(item.weaponEntry);
            }
            
            gameObject.SetActive(true);
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

        private void Lock()
        {
            var text = lockedButton.GetComponentInChildren<TextMeshProUGUI>();
            if (_item.isLocked)
                text.text = "UnLocked";
            else
                text.text = "Locked";
            
            EventBus.Publish(new OnShopItemLockedEvent(_item));
        }

        private void Buy()
        {
            if (_item == null)
                return;

            EventBus.Publish(new OnShopPurchaseRequestedEvent(_item));
        }
    }
}
