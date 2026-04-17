using System;
using Stats;

namespace Player
{
    [Serializable]
    public class PlayerStatModifierDefinition
    {
        public StatType statType;
        public float value;
        public StatModType modifierType = StatModType.Flat;
    }
}
