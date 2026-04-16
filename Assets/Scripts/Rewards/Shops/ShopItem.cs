using System;
using Items;
using UnityEngine;
using Weapons;
using Weapons.Items;

namespace Rewards.Shops
{
    [Serializable]
    public class ShopItem
    {
        public ShopItemType type;
        public WeaponEntry weaponEntry;
        public ItemData itemData;
        public bool isLocked;

        public bool IsWeapon => type == ShopItemType.Weapon;
        public bool IsItem => type == ShopItemType.Item;

        public WeaponEntry GetWeaponEntry()
        {
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

        public Sprite GetIcon()
        {
            if (IsItem)
                return itemData.GetIcon();
            
            return weaponEntry.GetIcon();
        }

        public Rarity GetRarity()
        {
            if (IsItem)
                return itemData.GetRarity();

            return weaponEntry.GetRarity();
        }
    }
}
