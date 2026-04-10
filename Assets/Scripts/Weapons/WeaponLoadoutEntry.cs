using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public class WeaponLoadoutEntry
    {
        [Header("Weapon Entry")]
        public WeaponData weaponData;
        public Rarity rarity = Rarity.Common;

        public bool IsValid()
        {
            return weaponData != null;
        }

        public string GetDisplayName()
        {
            return weaponData != null ? weaponData.weaponName : string.Empty;
        }

        public string GetSummary()
        {
            return weaponData != null ? weaponData.GetSummary() : string.Empty;
        }

        public Sprite GetIcon()
        {
            return weaponData != null ? weaponData.icon : null;
        }

        public WeaponStats GetStats()
        {
            return weaponData != null ? weaponData.GetStats(rarity) : null;
        }

        public int GetPrice()
        {
            return GetStats()?.price ?? 0;
        }
    }
}
