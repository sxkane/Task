using System;
using UnityEngine;
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
    }
}