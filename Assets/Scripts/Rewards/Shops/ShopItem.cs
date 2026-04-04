using System;
using Weapons;
using Weapons.Items;

namespace Rewards.Shops
{
    [Serializable]
    public class ShopItem
    {
        public ShopItemType type;
        public Rarity rarity;
        public WeaponData weaponData;
        public ItemData itemData;
        public bool isLocked;

        public int GetPrice()
        {
            if (type == ShopItemType.Item)
                return itemData != null ? itemData.price : 0;

            return weaponData != null ? weaponData.GetStats(rarity)?.price ?? 0 : 0;
        }
    }
}
