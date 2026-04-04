using Rewards.Shops;

namespace Events.ShopEvents
{
    public class OnShopPurchaseRequestedEvent : IEvent
    {
        public ShopItem ShopItem { get; }
        
        public OnShopPurchaseRequestedEvent(ShopItem shopItem)
        {
            ShopItem = shopItem;
        }
    }
}
