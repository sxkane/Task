using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public class WeaponStats
    {
        public Rarity rarity;

        [Header("Stats")]
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
    }

    [Serializable]
    public class WeaponDamage
    {
        public float damage;
        public DamageType damageType;
        // x %
        public int percentage;
    }
}