using System.Collections.Generic;
using Weapons;

namespace Events.DisplayEvent
{
    public class OnWeaponDisplay
    {
        public List<WeaponLoadoutEntry> Weapons { get; private set; }

        public OnWeaponDisplay(List<WeaponLoadoutEntry> weapons)
        {
            Weapons = weapons;
        }
    }
}