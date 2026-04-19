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

            var name = StatNameMapper.GetName(type);
            var text = StatValueUtility.FormatStatValue(type, value);
            var colorBasis = StatValueUtility.GetColorBasis(type, value);

            return $"{Colorize(colorBasis, text)} {name}";
        }

        public static string BuildModifierLine(float value, StatType type, StatModType modType)
        {
            if (Mathf.Approximately(value, 0f))
                return string.Empty;

            if (modType == StatModType.Flat)
                return BuildLine(value, type);

            var text = StatValueUtility.FormatModifierValue(value, modType);
            var name = StatNameMapper.GetName(type);
            var suffix = modType == StatModType.PercentMult ? "（乘区）" : string.Empty;
            return $"{Colorize(value, text)} {name}{suffix}";
        }

        public static string BuildCurrentValue(PlayerStats stats, StatType statType)
        {
            if (stats == null)
                return string.Empty;

            var value = stats.GetStat(statType).Value;
            var text = StatValueUtility.FormatStatValue(statType, value, includeSign: false);
            var colorBasis = StatValueUtility.GetColorBasis(statType, value);
            return Colorize(colorBasis, text);
        }
        
        public static string BuildTooltip(PlayerStats stats, StatType statType)
        {
            if (stats == null)
                return string.Empty;

            return statType switch
            {
                StatType.MaxHP => $"提高生存能力，增加 {BuildCurrentValue(stats, StatType.MaxHP)} 最大生命值。",
                StatType.HPRegen => $"每秒恢复 {ColorizeValue(stats.HpRegenPerSecond, true)} 生命值。",
                StatType.LifeSteal => $"攻击时有 {BuildCurrentValue(stats, StatType.LifeSteal)} 概率回复 1 点生命。",
                StatType.Armor =>
                    $"减少约 {ColorizeValue(Mathf.RoundToInt((1f - stats.DamageTakenMultiplier) * 100f))}% 所受伤害。",
                StatType.Dodge => $"有 {BuildCurrentValue(stats, StatType.Dodge)} 概率闪避伤害。",
                StatType.DamagePercent => $"总伤害提高 {BuildCurrentValue(stats, StatType.DamagePercent)}。",
                StatType.MeleeDamage => $"增加 {BuildCurrentValue(stats, StatType.MeleeDamage)} 近战伤害。",
                StatType.RangedDamage => $"增加 {BuildCurrentValue(stats, StatType.RangedDamage)} 远程伤害。",
                StatType.ElementalDamage => $"增加 {BuildCurrentValue(stats, StatType.ElementalDamage)} 元素伤害。",
                StatType.AttackSpeed => $"攻击速度提高 {BuildCurrentValue(stats, StatType.AttackSpeed)}。",
                StatType.CritChance => $"暴击率提高 {BuildCurrentValue(stats, StatType.CritChance)}。",
                StatType.Range => $"攻击范围增加 {BuildCurrentValue(stats, StatType.Range)}。",
                StatType.Knockback => $"击退提高 {BuildCurrentValue(stats, StatType.Knockback)}。",
                StatType.Speed => $"移动速度提高 {BuildCurrentValue(stats, StatType.Speed)}。",
                StatType.Luck => $"幸运提高 {BuildCurrentValue(stats, StatType.Luck)}。",
                StatType.Harvesting => $"每波结束获得额外 {BuildCurrentValue(stats, StatType.Harvesting)} 收获。",
                StatType.XPGain => $"经验获取提高 {BuildCurrentValue(stats, StatType.XPGain)}。",
                StatType.ConsumableHealing => $"消耗品回复提高 {BuildCurrentValue(stats, StatType.ConsumableHealing)}。",
                StatType.EnemyHealthPercent => $"敌人生命修正为 {BuildCurrentValue(stats, StatType.EnemyHealthPercent)}。",
                StatType.EnemySpeedPercent => $"敌人移速修正为 {BuildCurrentValue(stats, StatType.EnemySpeedPercent)}。",
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
