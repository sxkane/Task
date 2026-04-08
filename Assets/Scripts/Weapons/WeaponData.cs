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

        public string GetSummaryText()
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
