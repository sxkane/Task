using System;
using Stats;

namespace Weapons.Modifiers
{
    [Serializable]
    public class WeaponStatModifierDefinition
    {
        public WeaponStatType statType;
        public float value;
        public StatModType modType = StatModType.Flat;
    }
}
