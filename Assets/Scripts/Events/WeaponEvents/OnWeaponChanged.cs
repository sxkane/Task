using System.Collections.Generic;
using Weapons;

namespace Events.WeaponEvents
{
    public class OnWeaponChanged : IEvent
    {
        public IReadOnlyDictionary<WeaponSetBonusData, int> BonusCount { get; }
        public List<Weapon> Weapons { get; }

        public OnWeaponChanged(IReadOnlyDictionary<WeaponSetBonusData, int> bonusCount, List<Weapon> weapons)
        {
            BonusCount = bonusCount;
            Weapons = weapons;
        }
    }
}
