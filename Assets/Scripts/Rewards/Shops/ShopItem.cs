using System;
using Weapons;
using Weapons.Items;

namespace Rewards.Shops
{
    [Serializable]
    public class ShopItem
    {
        public ShopItemType type;
        public WeaponLoadoutEntry weaponEntry;
        public ItemData itemData;
        public bool isLocked;

        public bool IsWeapon => type == ShopItemType.Weapon;
        public bool IsItem => type == ShopItemType.Item;

        public int GetPrice()
        {
            if (IsItem)
                return itemData != null ? itemData.price : 0;

            return weaponEntry != null ? weaponEntry.GetPrice() : 0;
        }

        public string GetDisplayName()
        {
            if (IsItem)
                return itemData != null ? itemData.GetDisplayName() : string.Empty;

            return weaponEntry != null ? weaponEntry.GetDisplayName() : string.Empty;
        }

        public string GetSummary()
        {
            if (IsItem)
                return itemData != null ? itemData.GetSummary() : string.Empty;

            return weaponEntry != null ? weaponEntry.GetSummary() : string.Empty;
        }
    }
}
