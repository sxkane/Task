using Stats;

namespace Data.Text
{
    public static class StatNameMapper
    {
        public static string GetName(StatType type)
        {
            return type switch
            {
                StatType.MaxHP => "最大生命",
                StatType.HPRegen => "生命回复",
                StatType.LifeSteal => "吸血",
                StatType.Armor => "护甲",
                StatType.Dodge => "闪避",
                StatType.DamagePercent => "伤害",
                StatType.MeleeDamage => "近战伤害",
                StatType.RangedDamage => "远程伤害",
                StatType.ElementalDamage => "元素伤害",
                StatType.AttackSpeed => "攻速",
                StatType.CritChance => "暴击率",
                StatType.Range => "范围",
                StatType.Speed => "移速",
                StatType.Luck => "幸运",
                StatType.Harvesting => "收获",
                _ => type.ToString()
            };
        }

        public static string GetNameWithUnit(StatType type)
        {
            return type switch
            {
                StatType.AttackSpeed => "攻速%",
                StatType.CritChance => "暴击率%",
                StatType.Speed => "移速%",
                StatType.Dodge => "闪避%",
                StatType.DamagePercent => "伤害%",
                _ => GetName(type)
            };
        }
    }
}