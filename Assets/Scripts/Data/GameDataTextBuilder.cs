using System.Collections.Generic;
using System.Text;
using Player;
using Stats;
using Weapons;
using Weapons.Items;

namespace Data
{
    public static class GameDataTextBuilder
    {
        public static string BuildPlayerDescription(PlayerData playerData)
        {
            if (playerData == null)
                return string.Empty;

            var sections = new List<string>();
            var stats = playerData.playerStats;

            if (stats != null)
            {
                stats.Initialize();
                var statLines = new List<string>();

                AppendValueLine(statLines, stats.MaxHp, "Max HP");
                AppendValueLine(statLines, stats.MeleeDamage, "Melee Damage");
                AppendValueLine(statLines, stats.RangedDamage, "Ranged Damage");
                AppendValueLine(statLines, stats.ElementalDamage, "Elemental Damage");
                AppendValueLine(statLines, stats.AttackSpeedPercent, "Attack Speed%");
                AppendValueLine(statLines, stats.CritChancePercent, "Crit Chance%");
                AppendValueLine(statLines, stats.SpeedPercent, "Speed%");
                AppendValueLine(statLines, stats.Armor, "Armor");
                AppendValueLine(statLines, stats.DodgePercent, "Dodge%");

                if (statLines.Count > 0)
                    sections.Add(string.Join("\n", statLines));
            }

            var starterWeapons = playerData.starterWeapons;
            if (starterWeapons.Count > 0)
            {
                var weaponNames = new List<string>();
                foreach (var entry in starterWeapons)
                {
                    if (entry?.weaponData == null)
                        continue;

                    weaponNames.Add($"{entry.weaponData.weaponName} ({entry.rarity})");
                }

                if (weaponNames.Count > 0)
                    sections.Add($"Starter Weapons: {string.Join(" / ", weaponNames)}");
            }

            return JoinSections(sections);
        }

        public static string BuildWeaponDescription(WeaponLoadoutEntry entry)
        {
            if (entry == null || entry.weaponData == null)
                return string.Empty;

            return BuildWeaponDescription(entry.weaponData, entry.rarity);
        }

        public static string BuildWeaponDescription(WeaponData weaponData, Rarity rarity)
        {
            if (weaponData == null)
                return string.Empty;

            var sections = new List<string>();

            string weaponSummary = weaponData.GetSummaryText();
            if (!string.IsNullOrWhiteSpace(weaponSummary))
                sections.Add(weaponSummary);

            var stats = weaponData.GetStats(rarity);
            if (stats != null)
            {
                var statLines = new List<string>();

                if (stats.damage != null)
                {
                    foreach (var damage in stats.damage)
                    {
                        if (damage.damage <= 0 && damage.percentage <= 0)
                            continue;

                        string damageType = damage.damageType switch
                        {
                            DamageType.Melee => "Melee",
                            DamageType.Ranged => "Ranged",
                            DamageType.Elemental => "Elemental",
                            _ => damage.damageType.ToString()
                        };

                        if (damage.damage > 0 && damage.percentage > 0)
                            statLines.Add($"+{damage.damage:0.#} {damageType} ({damage.percentage}%)");
                        else if (damage.damage > 0)
                            statLines.Add($"+{damage.damage:0.#} {damageType}");
                        else
                            statLines.Add($"+{damage.percentage}% {damageType}");
                    }
                }

                AppendValueLine(statLines, stats.attackSpeed, "Attack Speed");
                AppendValueLine(statLines, stats.critChance, "Crit Chance");
                AppendValueLine(statLines, stats.critDamage, "Crit Damage");
                AppendValueLine(statLines, stats.range, "Range");
                AppendValueLine(statLines, stats.knockback, "Knockback");

                if (statLines.Count > 0)
                    sections.Add(string.Join("\n", statLines));

                var effectLines = BuildEffectDescriptions(stats.effects);
                if (effectLines.Count > 0)
                    sections.Add(string.Join("\n", effectLines));
            }

            return JoinSections(sections);
        }

        public static string BuildItemDescription(ItemData itemData)
        {
            if (itemData == null)
                return string.Empty;

            var sections = new List<string>();
            string itemSummary = itemData.GetSummaryText();
            if (!string.IsNullOrWhiteSpace(itemSummary))
                sections.Add(itemSummary);

            var lines = new List<string>();
            if (itemData.modifies != null)
            {
                foreach (var modify in itemData.modifies)
                    lines.Add(FormatItemModify(modify));
            }

            if (lines.Count > 0)
                sections.Add(string.Join("\n", lines));

            var effectLines = BuildEffectDescriptions(itemData.effects);
            if (effectLines.Count > 0)
                sections.Add(string.Join("\n", effectLines));

            return JoinSections(sections);
        }

        private static void AppendValueLine(List<string> lines, float value, string label)
        {
            if (value == 0)
                return;

            string prefix = value > 0 ? "+" : string.Empty;
            lines.Add($"{prefix}{value:0.##} {label}");
        }

        private static string JoinSections(List<string> sections)
        {
            var builder = new StringBuilder();

            foreach (var section in sections)
            {
                if (string.IsNullOrWhiteSpace(section))
                    continue;

                if (builder.Length > 0)
                    builder.AppendLine().AppendLine();

                builder.Append(section);
            }

            return builder.ToString();
        }

        private static List<string> BuildEffectDescriptions(List<Effect> effects)
        {
            var lines = new List<string>();
            if (effects == null)
                return lines;

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;

                string description = effect.BuildDescription();
                if (string.IsNullOrWhiteSpace(description))
                    continue;

                lines.Add(description);
            }

            return lines;
        }

        private static string FormatItemModify(ItemModify modify)
        {
            string statName = modify.statType switch
            {
                StatType.MaxHP => "Max HP",
                StatType.HPRegen => "HP Regen",
                StatType.LifeSteal => "Life Steal",
                StatType.Armor => "Armor",
                StatType.Dodge => "Dodge",
                StatType.DamagePercent => "Damage",
                StatType.MeleeDamage => "Melee Damage",
                StatType.RangedDamage => "Ranged Damage",
                StatType.ElementalDamage => "Elemental Damage",
                StatType.AttackSpeed => "Attack Speed",
                StatType.CritChance => "Crit Chance",
                StatType.Range => "Range",
                StatType.Speed => "Speed",
                StatType.Luck => "Luck",
                StatType.Harvesting => "Harvesting",
                _ => modify.statType.ToString()
            };

            return modify.modType switch
            {
                StatModType.Flat => $"+{modify.value:0.#} {statName}",
                StatModType.PercentAdd => $"+{modify.value:0.#}% {statName}",
                StatModType.PercentMult => $"+{modify.value * 100f:0.#}% {statName}",
                _ => $"+{modify.value:0.#} {statName}"
            };
        }
    }
}
