using Rewards.Shops;

namespace Events
{
    public class OnShopItemLockedEvent : IEvent
    {
        public ShopItem ShopItem { get; private set; }
        public OnShopItemLockedEvent(ShopItem shopItem)
        {
            ShopItem = shopItem;
        }
    }
}