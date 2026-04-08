using Player;
using Stats;
using UnityEngine;

namespace Data
{
    public static class StatTextBuilder
    {
        public static string BuildDescription(PlayerStats stats, StatType statType)
        {
            if (stats == null)
                return string.Empty;

            return statType switch
            {
                StatType.MaxHP => $"最大能够承受 {stats.MaxHp} 点伤害",
                StatType.HPRegen => $"每秒回复 {stats.HpRegenPerSecond:0.##} Hp",
                StatType.LifeSteal => $"每次造成伤害，{stats.LifeStealPercent}% 的概率回复 1 Hp",
                StatType.Armor => $"受到伤害降低 {Mathf.RoundToInt((1f - stats.DamageTakenMultiplier) * 100f)}%",
                StatType.Dodge => $"{stats.DodgePercent}% 的概率闪避伤害",

                StatType.DamagePercent => $"伤害增加 {stats.DamagePercent}%",
                StatType.MeleeDamage => $"近战伤害增加 {stats.MeleeDamage}",
                StatType.RangedDamage => $"远程伤害增加 {stats.RangedDamage}",
                StatType.ElementalDamage => $"元素伤害增加 {stats.ElementalDamage}",
                StatType.AttackSpeed => $"攻速增加 {stats.AttackSpeedPercent}%",
                StatType.CritChance => $"{stats.CritChancePercent}% 的概率造成暴击伤害",
                StatType.Range => $"范围增加 {stats.Range}",

                StatType.Speed => $"速度增加 {stats.SpeedPercent}%",
                StatType.Luck => $"幸运增加 {stats.Luck}",
                StatType.Harvesting => $"每回合结束获得 {stats.Harvesting} 经验和金币",
                _ => statType.ToString()
            };
        }
    }
}
