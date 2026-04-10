using System;
using Weapons;
using Weapons.Items;

namespace Rewards.Shops
{
    [Serializable]
    public class ShopItem
    {
        public ShopItemType type;
        public WeaponSelectionEntry weaponSelectionEntry;
        public WeaponLoadoutEntry weaponEntry;
        public ItemData itemData;
        public bool isLocked;

        public bool IsWeapon => type == ShopItemType.Weapon;
        public bool IsItem => type == ShopItemType.Item;

        public WeaponLoadoutEntry GetWeaponEntry()
        {
            if (weaponSelectionEntry != null)
                return weaponSelectionEntry;

            return weaponEntry;
        }

        public int GetPrice()
        {
            if (IsItem)
                return itemData != null ? itemData.price : 0;

            return GetWeaponEntry() != null ? GetWeaponEntry().GetPrice() : 0;
        }

        public string GetDisplayName()
        {
            if (IsItem)
                return itemData != null ? itemData.GetDisplayName() : string.Empty;

            return GetWeaponEntry() != null ? GetWeaponEntry().GetDisplayName() : string.Empty;
        }

        public string GetSummary()
        {
            if (IsItem)
                return itemData != null ? itemData.GetSummary() : string.Empty;

            return GetWeaponEntry() != null ? GetWeaponEntry().GetSummary() : string.Empty;
        }
    }
}
