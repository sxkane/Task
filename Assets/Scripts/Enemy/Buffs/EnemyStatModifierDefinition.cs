using System;
using Stats;

namespace Enemy.Buffs
{
    [Serializable]
    public class EnemyStatModifierDefinition
    {
        public EnemyStatType statType;
        public float value;
        public StatModType modifierType = StatModType.Flat;
    }
}
