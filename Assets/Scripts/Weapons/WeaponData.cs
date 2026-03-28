using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Game/Weapon")]
    public class WeaponData : ScriptableObject
    {
        public int weaponID;
        public string weaponName;

        [Header("Visual")]
        public GameObject weaponPrefab;

        [Header("Rarity Settings")]
        public List<WeaponStats> rarityStats;

        public WeaponStats GetStats(Rarity rarity)
        {
            if (rarityStats == null || rarityStats.Count == 0)
                return null;

            var found = rarityStats.Find(r => r.rarity == rarity);
            return found ?? rarityStats[0];
        }
    }
}