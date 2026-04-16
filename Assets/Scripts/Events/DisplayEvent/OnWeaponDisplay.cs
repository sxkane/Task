using System.Collections.Generic;
using Weapons;

namespace Events.DisplayEvent
{
    public class OnWeaponDisplay
    {
        public List<WeaponEntry> Weapons { get; private set; }

        public OnWeaponDisplay(List<WeaponEntry> weapons)
        {
            Weapons = weapons;
        }
    }
}