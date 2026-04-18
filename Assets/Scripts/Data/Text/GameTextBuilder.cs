using System.Collections.Generic;
using System.Text;
using Enemy;
using Items;
using Player;
using Stats;
using Weapons;

namespace Data.Text
{
    public static class GameTextBuilder
    {
        public static string BuildPlayer(PlayerData data)
        {
            if (data?.playerStats == null) return string.Empty;
            
            var lines = new List<string>();
            
            lines.Add(data.GetSummary());
            
            var passiveData = data.GetPassiveData();
            if (passiveData != null)
            {
                foreach (var modifier in passiveData.Modifiers)
                {
                    var description = StatTextBuilder.BuildLine(modifier.value, modifier.statType);
                    lines.Add(description);
                }
            }

            lines.RemoveAll(string.IsNullOrWhiteSpace);
            return string.Join("\n", lines);
        }

        public static string BuildEnemy(EnemyStatTemplate e)
        {
            if (e == null) return string.Empty;

            var lines = new List<string>
            {
                StatTextBuilder.BuildLine(e.maxHP, StatType.MaxHP),
                StatTextBuilder.BuildLine(e.moveSpeed, StatType.Speed),
                StatTextBuilder.BuildLine(e.damage, StatType.DamagePercent),
            };

            lines.RemoveAll(string.IsNullOrWhiteSpace);
            return string.Join("\n", lines);
        }

        public static string BuildWeapon(WeaponEntry entry)
        {
            if (entry == null || !entry.IsValid())
                return string.Empty;

            return BuildWeapon(entry.weaponData, entry.rarity);
        }

        public static string BuildWeapon(WeaponData weaponData, Rarity rarity)
        {
            if (weaponData == null)
                return string.Empty;

            var sections = new List<string>();

            var summary = weaponData.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary))
                sections.Add(summary);

            var stats = weaponData.GetStats(rarity);
            if (stats != null)
            {
                var statLines = BuildStatLines(stats);
                if (statLines.Count > 0)
                    sections.Add(string.Join("\n", statLines));
            }

            var abilityLines = BuildAbilityLines(weaponData, rarity);
            if (abilityLines.Count > 0)
                sections.Add(string.Join("\n", abilityLines));

            return JoinSections(sections);
        }

        public static string BuildItem(ItemData itemData)
        {
            if (itemData == null)
                return string.Empty;

            var sections = new List<string>();

            var summary = itemData.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary))
                sections.Add(summary);

            if (itemData.modifies != null)
            {
                var statLines = new List<string>();

                foreach (var modify in itemData.modifies)
                    statLines.Add(BuildItemModify(modify));

                statLines.RemoveAll(string.IsNullOrWhiteSpace);

                if (statLines.Count > 0)
                    sections.Add(string.Join("\n", statLines));
            }

            var effectLines = BuildEffectLines(itemData.effects);
            if (effectLines.Count > 0)
                sections.Add(string.Join("\n", effectLines));

            return JoinSections(sections);
        }

        private static List<string> BuildStatLines(WeaponStats stats)
        {
            var lines = new List<string>();

            if (stats == null) return lines;
            
            if (stats.damage != null)
            {
                foreach (var dmg in stats.damage)
                {
                    if (dmg == null) continue;

                    var line = BuildDamageLine(dmg);
                    if (!string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
            
            lines.Add(StatTextBuilder.BuildLine(stats.attackSpeed, StatType.AttackSpeed));
            lines.Add(StatTextBuilder.BuildLine(stats.critChance, StatType.CritChance));
            lines.Add(StatTextBuilder.BuildLine(stats.critDamage, StatType.DamagePercent));
            lines.Add(StatTextBuilder.BuildLine(stats.range, StatType.Range));
            lines.Add(StatTextBuilder.BuildLine(stats.knockback, StatType.Range));

            lines.RemoveAll(string.IsNullOrWhiteSpace);
            return lines;
        }

        private static List<string> BuildAbilityLines(WeaponData weaponData, Rarity rarity)
        {
            var lines = new List<string>();
            var abilities = weaponData != null ? weaponData.GetAbilities() : null;
            if (abilities == null)
                return lines;

            for (var i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];
                if (ability == null)
                    continue;

                var line = ability.BuildDescription(rarity);
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }

            return lines;
        }
        
        private static string BuildDamageLine(WeaponDamage dmg)
        {
            if (dmg == null) return string.Empty;

            var typeName = dmg.damageType switch
            {
                DamageType.Melee => "近战伤害",
                DamageType.Ranged => "远程伤害",
                DamageType.Elemental => "元素伤害",
                _ => "伤害"
            };

            if (dmg.damage > 0 && dmg.percentage > 0)
                return $"{StatTextBuilder.Colorize(dmg.damage, "+" + dmg.damage.ToString("0.#"))} {typeName}（{dmg.percentage}%）";

            if (dmg.damage > 0)
                return $"{StatTextBuilder.Colorize(dmg.damage, "+" + dmg.damage.ToString("0.#"))} {typeName}";

            if (dmg.percentage > 0)
                return $"{StatTextBuilder.Colorize(dmg.percentage, "+" + dmg.percentage)}% {typeName}";

            return string.Empty;
        }

        private static List<string> BuildEffectLines(List<Weapons.Effects.Effect> effects)
        {
            var lines = new List<string>();
            if (effects == null) return lines;

            foreach (var effect in effects)
            {
                if (effect == null || !effect.IsValid())
                    continue;

                var desc = effect.BuildDescription();
                if (!string.IsNullOrWhiteSpace(desc))
                    lines.Add(desc);
            }

            return lines;
        }

        private static string BuildItemModify(ItemModify modify)
        {
            if (modify == null) return string.Empty;

            var name = StatNameMapper.GetName(modify.statType);

            return modify.modType switch
            {
                StatModType.Flat => StatTextBuilder.BuildLine(modify.value, modify.statType),
                StatModType.PercentAdd => StatTextBuilder.BuildLine(modify.value, modify.statType),
                StatModType.PercentMult => StatTextBuilder.BuildLine(modify.value * 100f, modify.statType),
                _ => StatTextBuilder.BuildLine(modify.value, modify.statType)
            };
        }

        private static string JoinSections(List<string> sections)
        {
            var sb = new StringBuilder();

            foreach (var s in sections)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;

                if (sb.Length > 0)
                    sb.AppendLine().AppendLine();

                sb.Append(s);
            }

            return sb.ToString();
        }

        private static string ColorizeSignedLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return string.Empty;

            if (line[0] != '+' && line[0] != '-')
                return line;

            var index = line.IndexOf(' ');
            if (index <= 0) return line;

            var value = line.Substring(0, index);
            var suffix = line.Substring(index);

            var sign = line[0] == '-' ? -1f : 1f;
            return $"{StatTextBuilder.Colorize(sign, value)}{suffix}";
        }
    }
}
