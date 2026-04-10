using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public class WeaponStats
    {
        [Header("Identity")]
        public Rarity rarity;

        [Header("Combat")]
        public List<WeaponDamage> damage;
        public float attackSpeed;
        public float critChance;
        public float critDamage;
        public float range;
        public float knockback;

        [Header("Economy")]
        public int price;
    
        [Header("Effects")]
        public List<Effect> effects;

        public bool IsValid()
        {
            return price >= 0;
        }

        public List<string> BuildStatLines()
        {
            var lines = new List<string>();

            if (damage != null)
            {
                foreach (var damageEntry in damage)
                {
                    if (damageEntry == null)
                        continue;

                    string line = damageEntry.BuildDescriptionLine();
                    if (!string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }

            AppendValueLine(lines, attackSpeed, "Attack Speed");
            AppendValueLine(lines, critChance, "Crit Chance");
            AppendValueLine(lines, critDamage, "Crit Damage");
            AppendValueLine(lines, range, "Range");
            AppendValueLine(lines, knockback, "Knockback");

            return lines;
        }

        private static void AppendValueLine(List<string> lines, float value, string label)
        {
            if (value == 0)
                return;

            string prefix = value > 0 ? "+" : string.Empty;
            lines.Add($"{prefix}{value:0.##} {label}");
        }
    }

    [Serializable]
    public class WeaponDamage
    {
        public float damage;
        public DamageType damageType;
        // x %
        public int percentage;

        public bool HasValue()
        {
            return damage > 0 || percentage > 0;
        }

        public string BuildDescriptionLine()
        {
            if (!HasValue())
                return string.Empty;

            string damageTypeName = damageType switch
            {
                DamageType.Melee => "Melee",
                DamageType.Ranged => "Ranged",
                DamageType.Elemental => "Elemental",
                _ => damageType.ToString()
            };

            if (damage > 0 && percentage > 0)
                return $"+{damage:0.#} {damageTypeName} ({percentage}%)";

            if (damage > 0)
                return $"+{damage:0.#} {damageTypeName}";

            return $"+{percentage}% {damageTypeName}";
        }
    }
}
