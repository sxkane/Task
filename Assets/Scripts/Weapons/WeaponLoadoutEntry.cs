using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public class WeaponLoadoutEntry
    {
        public WeaponData weaponData;
        public Rarity rarity = Rarity.Common;

        public WeaponStats GetStats()
        {
            return weaponData != null ? weaponData.GetStats(rarity) : null;
        }
    }
}
