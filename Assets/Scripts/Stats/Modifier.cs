using System;

namespace Stats
{
    public enum StatModType
    {
        Flat,
        PercentAdd,      // 同乘区相加
        PercentMult,     // 独立乘区
    }

    [Serializable]
    public class Modifier
    {
        public float value;
        public StatModType type;
        public object Source;

        public Modifier(float value, StatModType type, object source = null)
        {
            this.value = value;
            this.type = type;
            this.Source = source;
        }
    }
}