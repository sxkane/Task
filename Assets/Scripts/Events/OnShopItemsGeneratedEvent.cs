using System.Collections.Generic;
using Rewards.Shops;
using Rewards.StatRewards;

namespace Events
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