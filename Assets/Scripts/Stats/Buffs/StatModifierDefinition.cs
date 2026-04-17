using System;

namespace Stats.Buffs
{
    [Serializable]
    public class StatModifierDefinition
    {
        public string statKey;
        public float value;
        public StatModType modifierType = StatModType.Flat;
    }
}
