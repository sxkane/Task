using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Player
{
    [CreateAssetMenu(menuName = "Game/player")]
    public class PlayerData : ScriptableObject
    {
        [Header("Identity")]
        public int playerID;
        public string playerName;
        public Sprite playerIcon;
        [TextArea] public string summary;

        [Header("Runtime Template")]
        public PlayerStats playerStats;
        public GameObject playerPrefab;

        [Header("Starter Loadout")]
        public List<WeaponLoadoutEntry> starterWeapons;

        public bool IsValid()
        {
            return playerID >= 0 && !string.IsNullOrWhiteSpace(playerName) && playerPrefab != null;
        }

        public int GetDataId()
        {
            return playerID;
        }

        public string GetDisplayName()
        {
            return playerName;
        }

        public string GetValidationSourceName()
        {
            return string.IsNullOrWhiteSpace(playerName) ? name : playerName;
        }

        public Sprite GetIcon()
        {
            return playerIcon;
        }

        public string GetSummary()
        {
            return string.IsNullOrWhiteSpace(summary) ? string.Empty : summary.Trim();
        }

        public List<WeaponLoadoutEntry> GetStarterWeaponEntries()
        {
            return starterWeapons ?? new List<WeaponLoadoutEntry>();
        }
    }
}
