using System.Collections.Generic;
using Weapons.Items;

namespace Events.DisplayEvent
{
    public class OnItemsDisplay : IEvent
    {
        public List<ItemData> Items { get; private set; }

        public OnItemsDisplay(List<ItemData> item)
        {
            Items = item;
        }
    }
}