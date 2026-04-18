using System;
using Player;
using UnityEngine;
using Weapons;
using Weapons.Core;

namespace Stats
{
    public static class DamageCalculator
    {
        public static int CalculateBaseDamage(PlayerStats playerStats, WeaponStats weaponStats)
        {
            var finalDamage = 0;
            foreach (var damage in weaponStats.damage)
            {
                switch (damage.damageType)
                {
                    case DamageType.Melee:
                        finalDamage += Mathf.RoundToInt(damage.damage + playerStats.MeleeDamage * (damage.percentage / 100f));
                        break;
                    case DamageType.Ranged:
                        finalDamage += Mathf.RoundToInt(damage.damage + playerStats.RangedDamage * (damage.percentage / 100f));
                        break;
                    case DamageType.Elemental:
                        finalDamage += Mathf.RoundToInt(damage.damage + playerStats.ElementalDamage * (damage.percentage / 100f));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            finalDamage = Mathf.RoundToInt(finalDamage * playerStats.DamageMultiplier);
            return finalDamage;
        }

        public static int CalculateBaseDamage(PlayerStats playerStats, WeaponRuntimeStats runtimeStats)
        {
            if (playerStats == null || runtimeStats == null)
                return 0;

            var finalDamage = 0;
            var entries = runtimeStats.DamageEntries;
            for (var i = 0; i < entries.Count; i++)
            {
                var damage = entries[i];
                var baseDamage = damage.damage.Value;
                var percentage = damage.percentage.Value / 100f;

                switch (damage.damageType)
                {
                    case DamageType.Melee:
                        finalDamage += Mathf.RoundToInt(baseDamage + playerStats.MeleeDamage * percentage);
                        break;
                    case DamageType.Ranged:
                        finalDamage += Mathf.RoundToInt(baseDamage + playerStats.RangedDamage * percentage);
                        break;
                    case DamageType.Elemental:
                        finalDamage += Mathf.RoundToInt(baseDamage + playerStats.ElementalDamage * percentage);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            finalDamage = Mathf.RoundToInt(finalDamage * playerStats.DamageMultiplier);
            return finalDamage;
        }
    }
}
