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
        public int slotIndex = -1;
        public int waveNumber = 1;
        public float shopPriceMultiplier = 1f;

        public bool IsWeapon => type == ShopItemType.Weapon;
        public bool IsItem => type == ShopItemType.Item;

        public void ConfigureShopData(int slot, int wave, float priceMultiplier)
        {
            slotIndex = slot;
            waveNumber = Mathf.Max(1, wave);
            shopPriceMultiplier = Mathf.Max(0f, priceMultiplier);
        }

        public WeaponEntry GetWeaponEntry()
        {
            return weaponEntry;
        }

        public int GetBasePrice()
        {
            if (IsItem)
                return itemData != null ? itemData.price : 0;

            return GetWeaponEntry() != null ? GetWeaponEntry().GetBasePrice() : 0;
        }

        public int GetPrice()
        {
            var basePrice = Mathf.Max(0, GetBasePrice());
            var finalPrice = (basePrice + waveNumber + (basePrice * 0.1f * waveNumber)) * shopPriceMultiplier;
            return Mathf.Max(0, Mathf.FloorToInt(finalPrice));
        }

        public int GetRecyclePrice()
        {
            if (IsItem)
                return itemData != null ? itemData.GetRecyclePrice() : 0;

            return Mathf.Max(0, Mathf.FloorToInt(GetPrice() * 0.25f));
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
