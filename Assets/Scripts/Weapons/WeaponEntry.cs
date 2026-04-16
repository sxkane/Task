using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public class WeaponEntry
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
            return weaponData != null ? weaponData.GetDisplayName() : string.Empty;
        }

        public string GetSummary()
        {
            return weaponData != null ? weaponData.GetSummary() : string.Empty;
        }

        public Sprite GetIcon()
        {
            return weaponData != null ? weaponData.GetIcon() : null;
        }

        public WeaponStats GetStats()
        {
            return weaponData != null ? weaponData.GetStats(rarity) : null;
        }

        public int GetPrice()
        {
            return GetStats()?.price ?? 0;
        }

        public int GetDataId()
        {
            return weaponData != null ? weaponData.GetDataId() : -1;
        }

        public Rarity GetRarity()
        {
            return rarity;
        }

        public GameObject GetPrefab()
        {
            return weaponData != null ? weaponData.weaponPrefab : null;
        }

        public string GetValidationSourceName()
        {
            return weaponData != null ? weaponData.GetValidationSourceName() : nameof(WeaponEntry);
        }

        public bool CanUpgrade()
        {
            if (!IsValid())
                return false;

            return weaponData.TryGetNextAvailableRarity(rarity, out _);
        }

        public WeaponEntry CreateUpgradedEntry()
        {
            if (!CanUpgrade())
                return null;

            weaponData.TryGetNextAvailableRarity(rarity, out var nextRarity);

            return new WeaponEntry
            {
                weaponData = weaponData,
                rarity = nextRarity
            };
        }
    }
}
