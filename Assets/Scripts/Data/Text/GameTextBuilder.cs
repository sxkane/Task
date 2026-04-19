using System.Collections.Generic;
using System.Text;
using Enemy;
using Items;
using Player;
using Stats;
using UnityEngine;
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
                    var description = StatTextBuilder.BuildModifierLine(modifier.value, modifier.statType, modifier.modifierType);
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
                $"{StatTextBuilder.Colorize(e.maxHP, e.maxHP.ToString("0.##"))} 生命",
                $"{StatTextBuilder.Colorize(e.moveSpeed, e.moveSpeed.ToString("0.##"))} 移速",
                $"{StatTextBuilder.Colorize(e.damage, e.damage.ToString("0.##"))} 伤害",
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

            var abilityLines = BuildAbilityLines(itemData.abilities);
            if (abilityLines.Count > 0)
                sections.Add(string.Join("\n", abilityLines));

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
            
            lines.Add(BuildWeaponStatLine(Weapons.Modifiers.WeaponStatType.AttackInterval, stats.attackSpeed));
            lines.Add(BuildWeaponStatLine(Weapons.Modifiers.WeaponStatType.CritChance, stats.critChance));
            lines.Add(BuildWeaponStatLine(Weapons.Modifiers.WeaponStatType.CritDamage, stats.critDamage));
            lines.Add(BuildWeaponStatLine(Weapons.Modifiers.WeaponStatType.Range, stats.range));
            lines.Add(BuildWeaponStatLine(Weapons.Modifiers.WeaponStatType.Knockback, stats.knockback));

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

        private static string BuildWeaponStatLine(Weapons.Modifiers.WeaponStatType statType, float value)
        {
            if (Mathf.Approximately(value, 0f))
                return string.Empty;

            var label = GetWeaponStatName(statType);
            var text = StatValueUtility.FormatWeaponStatValue(statType, value);
            var colorBasis = StatValueUtility.GetColorBasis(statType, value);
            return colorBasis == 0f && statType == Weapons.Modifiers.WeaponStatType.AttackInterval
                ? $"{text} {label}"
                : $"{StatTextBuilder.Colorize(colorBasis, text)} {label}";
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

        private static List<string> BuildAbilityLines(List<Items.Abilities.ItemAbility> abilities)
        {
            var lines = new List<string>();
            if (abilities == null) return lines;

            foreach (var ability in abilities)
            {
                if (ability == null || !ability.IsValid())
                    continue;

                var desc = ability.BuildDescription();
                if (!string.IsNullOrWhiteSpace(desc))
                    lines.Add(desc);
            }

            return lines;
        }

        private static string BuildItemModify(ItemModify modify)
        {
            if (modify == null) return string.Empty;

            return StatTextBuilder.BuildModifierLine(modify.value, modify.statType, modify.modType);
        }

        private static string GetWeaponStatName(Weapons.Modifiers.WeaponStatType statType)
        {
            return statType switch
            {
                Weapons.Modifiers.WeaponStatType.AttackInterval => "攻击间隔",
                Weapons.Modifiers.WeaponStatType.CritChance => "暴击率",
                Weapons.Modifiers.WeaponStatType.CritDamage => "暴击伤害",
                Weapons.Modifiers.WeaponStatType.Range => "射程",
                Weapons.Modifiers.WeaponStatType.Knockback => "击退",
                Weapons.Modifiers.WeaponStatType.ProjectileSpeed => "弹速",
                Weapons.Modifiers.WeaponStatType.PierceCount => "穿透次数",
                Weapons.Modifiers.WeaponStatType.PierceDamageMultiplier => "穿透伤害",
                Weapons.Modifiers.WeaponStatType.BounceCount => "弹跳次数",
                Weapons.Modifiers.WeaponStatType.ExplosionRadius => "爆炸范围",
                Weapons.Modifiers.WeaponStatType.BurnSpreadCount => "燃烧扩散",
                Weapons.Modifiers.WeaponStatType.MeleeDamage => "近战伤害",
                Weapons.Modifiers.WeaponStatType.RangedDamage => "远程伤害",
                Weapons.Modifiers.WeaponStatType.ElementalDamage => "元素伤害",
                _ => statType.ToString()
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
