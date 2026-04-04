using Rewards.Shops;

namespace Events.ShopEvents
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