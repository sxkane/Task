using System.Collections.Generic;
using Player;
using Weapons;

namespace Core
{
    public class GameSession
    {
        public PlayerData SelectedPlayer;
        
        // New semantic field.
        public List<WeaponSelectionEntry> SelectedWeaponSelections;
        // Legacy compatibility field.
        public List<WeaponLoadoutEntry> SelectedWeapons;

        public List<WeaponLoadoutEntry> GetSelectedWeaponEntries()
        {
            if (SelectedWeaponSelections != null && SelectedWeaponSelections.Count > 0)
                return new List<WeaponLoadoutEntry>(SelectedWeaponSelections);

            return SelectedWeapons ?? new List<WeaponLoadoutEntry>();
        }
    }
}
