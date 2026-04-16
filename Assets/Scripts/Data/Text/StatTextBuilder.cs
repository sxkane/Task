using System.Globalization;
using Player;
using Stats;
using UnityEngine;

namespace Data.Text
{
    public static class StatTextBuilder
    {
        public static readonly Color Positive = new(0.25f, 0.8f, 0.35f);
        public static readonly Color Negative = new(0.88f, 0.22f, 0.22f);

        private static readonly string PosHex = "#" + ColorUtility.ToHtmlStringRGB(Positive);
        private static readonly string NegHex = "#" + ColorUtility.ToHtmlStringRGB(Negative);
        
        public static string BuildLine(float value, StatType type)
        {
            if (Mathf.Approximately(value, 0)) return string.Empty;

            var name = StatNameMapper.GetNameWithUnit(type);
            var prefix = value > 0 ? "+" : "";
            var text = $"{prefix}{value:0.##}";

            return $"{Colorize(value, text)} {name}";
        }
        
        public static string BuildTooltip(PlayerStats stats, StatType statType)
        {
            if (stats == null)
                return string.Empty;

            return statType switch
            {
                StatType.MaxHP => $"提高生存能力，增加 {ColorizeValue(stats.MaxHp)} 最大生命值。",
                StatType.HPRegen => $"每秒恢复 {ColorizeValue(stats.HpRegenPerSecond, true)} 生命值。",
                StatType.LifeSteal => $"攻击时有 {ColorizeValue(stats.LifeStealPercent)}% 概率回复 1 点生命。",
                StatType.Armor =>
                    $"减少约 {ColorizeValue(Mathf.RoundToInt((1f - stats.DamageTakenMultiplier) * 100f))}% 所受伤害。",
                StatType.Dodge => $"有 {ColorizeValue(stats.DodgePercent)}% 概率闪避伤害。",
                StatType.DamagePercent => $"总伤害提高 {ColorizeValue(stats.DamagePercent)}%。",
                StatType.MeleeDamage => $"增加 {ColorizeValue(stats.MeleeDamage)} 近战伤害。",
                StatType.RangedDamage => $"增加 {ColorizeValue(stats.RangedDamage)} 远程伤害。",
                StatType.ElementalDamage => $"增加 {ColorizeValue(stats.ElementalDamage)} 元素伤害。",
                StatType.AttackSpeed => $"攻击速度提高 {ColorizeValue(stats.AttackSpeedPercent)}%。",
                StatType.CritChance => $"暴击率提高 {ColorizeValue(stats.CritChancePercent)}%。",
                StatType.Range => $"攻击范围增加 {ColorizeValue(stats.Range)}。",
                StatType.Speed => $"移动速度提高 {ColorizeValue(stats.SpeedPercent)}%。",
                StatType.Luck => $"幸运提高 {ColorizeValue(stats.Luck)}。",
                StatType.Harvesting => $"每波结束获得额外 {ColorizeValue(stats.Harvesting)} 收获。",
                _ => StatNameMapper.GetName(statType)
            };
        }
        

        public static string ColorizeValue(float value, bool formatFloat = false)
        {
            var text = formatFloat
                ? value.ToString("0.##", CultureInfo.InvariantCulture)
                : value.ToString(CultureInfo.InvariantCulture);

            return Colorize(value, text);
        }

        public static string ColorizeValue(int value)
        {
            return Colorize(value, value.ToString());
        }

        public static string Colorize(float value, string text)
        {
            var hex = value >= 0 ? PosHex : NegHex;
            return $"<color={hex}>{text}</color>";
        }
    }
}