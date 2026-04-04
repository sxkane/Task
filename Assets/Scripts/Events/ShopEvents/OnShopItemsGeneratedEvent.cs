using System.Collections.Generic;
using Rewards.Shops;

namespace Events.ShopEvents
{
    public class OnShopItemsGeneratedEvent : IEvent
    {
        public List<ShopItem> ItemOptions { get; private set; }

        public OnShopItemsGeneratedEvent(List<ShopItem> itemOptions)
        {
            ItemOptions = itemOptions;
        }
    }
}