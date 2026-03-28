using Events;
using Rewards.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace UI.GameSceneUI
{
    public class RewardSlot : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI rewardText;
        public TextMeshProUGUI descriptionText;
        [SerializeField] private Button lockedButton;

        private ShopItem _item;
        
        public void Show(ShopItem item)
        {
            _item = item;
            
            if (item.type == ShopItemType.Item)
            {
                nameText.text = item.itemData.itemName;
                rewardText.text = item.itemData.price.ToString();
            }
            else
            {
                nameText.text = item.weaponData.weaponName;
                rewardText.text = item.weaponData.GetStats(item.rarity).price.ToString();
            }
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            lockedButton.onClick.AddListener(Lock);
        }

        private void OnDisable()
        {
            lockedButton.onClick.RemoveListener(Lock);
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
    }
}