using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Game/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public int weaponID;
        public string weaponName;
        [TextArea] public string summary;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Visual")]
        public GameObject weaponPrefab;

        [Header("Rarity Settings")]
        public List<WeaponStats> rarityStats;

        public bool IsValid()
        {
            return weaponID >= 0 && !string.IsNullOrWhiteSpace(weaponName) && weaponPrefab != null;
        }

        public int GetDataId()
        {
            return weaponID;
        }

        public string GetDisplayName()
        {
            return weaponName;
        }

        public string GetValidationSourceName()
        {
            return string.IsNullOrWhiteSpace(weaponName) ? name : weaponName;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetSummary()
        {
            if (!string.IsNullOrWhiteSpace(summary))
                return summary.Trim();

            return string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
        }

        public WeaponStats GetStats(Rarity rarity)
        {
            if (rarityStats == null || rarityStats.Count == 0)
                return null;

            var found = rarityStats.Find(r => r.rarity == rarity);
            return found ?? rarityStats[0];
        }
    }
}
