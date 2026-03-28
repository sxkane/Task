using System;
using Player;
using UnityEngine;
using Weapons;

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
                        finalDamage += Mathf.RoundToInt(playerStats.MeleeDamage * (damage.percentage / 100f));
                        break;
                    case DamageType.Ranged:
                        finalDamage += Mathf.RoundToInt(playerStats.RangedDamage * (damage.percentage / 100f));
                        break;
                    case DamageType.Elemental:
                        finalDamage += Mathf.RoundToInt(playerStats.ElementalDamage * (damage.percentage / 100f));
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