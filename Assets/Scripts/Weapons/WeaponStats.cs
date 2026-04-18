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

        public bool IsValid()
        {
            return price >= 0;
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
    }
}
