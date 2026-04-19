using System;
using System.Collections.Generic;
using Player;
using Stats;
using UnityEngine;
using Weapons.Modifiers;

namespace Weapons.Core
{
    [Serializable]
    public class WeaponRuntimeStats
    {
        [Serializable]
        public sealed class DamageEntry
        {
            public DamageType damageType;
            public Stat damage;
            public Stat percentage;

            public DamageEntry(DamageType damageType, float baseDamage, int percentage)
            {
                this.damageType = damageType;
                damage = new Stat(baseDamage);
                this.percentage = new Stat(percentage);
            }
        }

        private readonly Dictionary<WeaponStatType, Stat> _stats = new();
        private readonly List<DamageEntry> _damageEntries = new();

        public IReadOnlyList<DamageEntry> DamageEntries => _damageEntries;

        public void Initialize(WeaponStats sourceStats)
        {
            _stats.Clear();
            _damageEntries.Clear();

            _stats[WeaponStatType.AttackInterval] = new Stat(sourceStats != null ? sourceStats.attackSpeed : 0f);
            _stats[WeaponStatType.CritChance] = new Stat(sourceStats != null ? sourceStats.critChance : 0f);
            _stats[WeaponStatType.CritDamage] = new Stat(sourceStats != null ? sourceStats.critDamage : 1f);
            _stats[WeaponStatType.Range] = new Stat(sourceStats != null ? sourceStats.range : 0f);
            _stats[WeaponStatType.Knockback] = new Stat(sourceStats != null ? sourceStats.knockback : 0f);
            _stats[WeaponStatType.ProjectileSpeed] = new Stat(0f);
            _stats[WeaponStatType.PierceCount] = new Stat(0f);
            _stats[WeaponStatType.PierceDamageMultiplier] = new Stat(1f);
            _stats[WeaponStatType.BounceCount] = new Stat(0f);
            _stats[WeaponStatType.ExplosionRadius] = new Stat(0f);
            _stats[WeaponStatType.BurnSpreadCount] = new Stat(0f);
            _stats[WeaponStatType.MeleeDamage] = new Stat(0f);
            _stats[WeaponStatType.RangedDamage] = new Stat(0f);
            _stats[WeaponStatType.ElementalDamage] = new Stat(0f);

            if (sourceStats?.damage == null)
                return;

            for (var i = 0; i < sourceStats.damage.Count; i++)
            {
                var damage = sourceStats.damage[i];
                if (damage == null)
                    continue;

                _damageEntries.Add(new DamageEntry(damage.damageType, damage.damage, damage.percentage));
            }
        }

        public Stat GetStat(WeaponStatType statType)
        {
            return _stats.TryGetValue(statType, out var stat)
                ? stat
                : throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
        }

        public float GetAttackInterval(PlayerStats playerStats)
        {
            var interval = GetStat(WeaponStatType.AttackInterval).Value;
            if (playerStats == null)
                return interval;

            return Mathf.Max(0.05f, interval / Mathf.Max(0.01f, playerStats.AttackSpeedMultiplier));
        }

        public float GetRange(PlayerStats playerStats)
        {
            var range = GetStat(WeaponStatType.Range).Value;
            return playerStats != null ? range + playerStats.Range : range;
        }

        public float GetKnockback()
        {
            return GetStat(WeaponStatType.Knockback).Value;
        }

        public float GetCritChanceRatio()
        {
            return StatValueUtility.GetWeaponChance(WeaponStatType.CritChance, GetStat(WeaponStatType.CritChance).Value);
        }

        public float GetCritDamageMultiplier()
        {
            return Mathf.Max(0f, StatValueUtility.GetWeaponMultiplier(WeaponStatType.CritDamage, GetStat(WeaponStatType.CritDamage).Value));
        }

        public int GetPierceCount()
        {
            return Mathf.RoundToInt(GetStat(WeaponStatType.PierceCount).Value);
        }

        public int GetBounceCount()
        {
            return Mathf.RoundToInt(GetStat(WeaponStatType.BounceCount).Value);
        }

        public void RemoveModifiersFromSource(object source)
        {
            foreach (var pair in _stats)
                pair.Value.RemoveModifiersFromSource(source);

            for (var i = 0; i < _damageEntries.Count; i++)
            {
                _damageEntries[i].damage.RemoveModifiersFromSource(source);
                _damageEntries[i].percentage.RemoveModifiersFromSource(source);
            }
        }
    }
}
