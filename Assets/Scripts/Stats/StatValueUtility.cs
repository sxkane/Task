using UnityEngine;
using Weapons.Modifiers;

namespace Stats
{
    public enum StatValueKind
    {
        Flat,
        PercentPoints,
        Multiplier,
        Seconds
    }

    public static class StatValueUtility
    {
        public static StatValueKind GetValueKind(StatType type)
        {
            return type switch
            {
                StatType.LifeSteal => StatValueKind.PercentPoints,
                StatType.Dodge => StatValueKind.PercentPoints,
                StatType.DamagePercent => StatValueKind.PercentPoints,
                StatType.AttackSpeed => StatValueKind.PercentPoints,
                StatType.CritChance => StatValueKind.PercentPoints,
                StatType.Speed => StatValueKind.PercentPoints,
                StatType.XPGain => StatValueKind.PercentPoints,
                StatType.EnemyHealthPercent => StatValueKind.PercentPoints,
                StatType.EnemySpeedPercent => StatValueKind.PercentPoints,
                _ => StatValueKind.Flat
            };
        }

        public static StatValueKind GetValueKind(WeaponStatType type)
        {
            return type switch
            {
                WeaponStatType.AttackInterval => StatValueKind.Seconds,
                WeaponStatType.CritChance => StatValueKind.PercentPoints,
                WeaponStatType.CritDamage => StatValueKind.Multiplier,
                WeaponStatType.PierceDamageMultiplier => StatValueKind.Multiplier,
                _ => StatValueKind.Flat
            };
        }

        public static float PercentPointsToRatio(float percentPoints)
        {
            return percentPoints / 100f;
        }

        public static float PercentPointsToMultiplier(float percentPoints, float baseMultiplier = 1f)
        {
            return baseMultiplier + PercentPointsToRatio(percentPoints);
        }

        public static float PercentPointsToChance(float percentPoints)
        {
            return Mathf.Clamp01(PercentPointsToRatio(percentPoints));
        }

        public static float GetPlayerChance(StatType type, float rawValue)
        {
            return GetValueKind(type) == StatValueKind.PercentPoints
                ? PercentPointsToChance(rawValue)
                : Mathf.Clamp01(rawValue);
        }

        public static float GetPlayerMultiplier(StatType type, float rawValue, float baseMultiplier = 1f)
        {
            return GetValueKind(type) == StatValueKind.PercentPoints
                ? PercentPointsToMultiplier(rawValue, baseMultiplier)
                : rawValue;
        }

        public static float GetWeaponChance(WeaponStatType type, float rawValue)
        {
            return GetValueKind(type) == StatValueKind.PercentPoints
                ? PercentPointsToChance(rawValue)
                : Mathf.Clamp01(rawValue);
        }

        public static float GetWeaponMultiplier(WeaponStatType type, float rawValue, float baseMultiplier = 1f)
        {
            return GetValueKind(type) switch
            {
                StatValueKind.PercentPoints => PercentPointsToMultiplier(rawValue, baseMultiplier),
                StatValueKind.Multiplier => rawValue,
                _ => rawValue
            };
        }

        public static Modifier CreatePlayerModifier(StatType statType, float configuredValue, StatModType modType, object source = null)
        {
            return new Modifier(NormalizePlayerModifierValue(statType, configuredValue, modType), modType, source);
        }

        public static Modifier CreateWeaponModifier(WeaponStatType statType, float configuredValue, StatModType modType, object source = null)
        {
            return new Modifier(NormalizeWeaponModifierValue(statType, configuredValue, modType), modType, source);
        }

        public static float NormalizePlayerModifierValue(StatType statType, float configuredValue, StatModType modType)
        {
            return NormalizeModifierValue(configuredValue, modType);
        }

        public static float NormalizeWeaponModifierValue(WeaponStatType statType, float configuredValue, StatModType modType)
        {
            return NormalizeModifierValue(configuredValue, modType);
        }

        public static string FormatStatValue(StatType type, float value, bool includeSign = true)
        {
            return FormatValue(value, GetValueKind(type), includeSign);
        }

        public static string FormatWeaponStatValue(WeaponStatType type, float value, bool includeSign = true)
        {
            return FormatValue(value, GetValueKind(type), includeSign);
        }

        public static string FormatModifierValue(float value, StatModType modType, bool includeSign = true)
        {
            return modType switch
            {
                StatModType.Flat => FormatSignedNumber(value, includeSign),
                StatModType.PercentAdd => FormatPercent(value, includeSign),
                StatModType.PercentMult => FormatPercent(value, includeSign),
                _ => FormatSignedNumber(value, includeSign)
            };
        }

        public static float GetColorBasis(StatType type, float value)
        {
            return GetColorBasis(GetValueKind(type), value);
        }

        public static float GetColorBasis(WeaponStatType type, float value)
        {
            return GetColorBasis(GetValueKind(type), value);
        }

        private static float NormalizeModifierValue(float configuredValue, StatModType modType)
        {
            return modType switch
            {
                StatModType.Flat => configuredValue,
                StatModType.PercentAdd => PercentPointsToRatio(configuredValue),
                StatModType.PercentMult => PercentPointsToRatio(configuredValue),
                _ => configuredValue
            };
        }

        private static string FormatValue(float value, StatValueKind kind, bool includeSign)
        {
            return kind switch
            {
                StatValueKind.PercentPoints => FormatPercent(value, includeSign),
                StatValueKind.Multiplier => $"x{value:0.##}",
                StatValueKind.Seconds => $"{value:0.##}s",
                _ => FormatSignedNumber(value, includeSign)
            };
        }

        private static float GetColorBasis(StatValueKind kind, float value)
        {
            return kind switch
            {
                StatValueKind.Multiplier => value - 1f,
                StatValueKind.Seconds => 0f,
                _ => value
            };
        }

        private static string FormatPercent(float value, bool includeSign)
        {
            return $"{FormatSignedNumber(value, includeSign)}%";
        }

        private static string FormatSignedNumber(float value, bool includeSign)
        {
            var prefix = includeSign && value > 0f ? "+" : string.Empty;
            return $"{prefix}{value:0.##}";
        }
    }
}
