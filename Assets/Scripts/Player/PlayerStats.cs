using System;
using System.Collections.Generic;
using Stats;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerStats
    {
        private Dictionary<StatType, Stat> _statsDict;
        private Dictionary<StatType, int> _statValueDict;
        private Dictionary<StatType, string> _statInfoDict;

        public void Initialize()
        {
            _statsDict = new Dictionary<StatType, Stat>()
            {
                { StatType.MaxHP, MaxHpStat },
                { StatType.HPRegen, HpRegenStat },
                { StatType.LifeSteal, LifeStealStat },
                { StatType.Armor, ArmorStat },
                { StatType.Dodge, DodgeStat },

                { StatType.DamagePercent, DamagePercentStat },
                { StatType.MeleeDamage, MeleeDamageStat },
                { StatType.RangedDamage, RangedDamageStat },
                { StatType.ElementalDamage, ElementalDamageStat },
                { StatType.AttackSpeed, AttackSpeedStat },
                { StatType.CritChance, CritChanceStat },
                { StatType.Range, RangeStat },

                { StatType.Speed, SpeedStat },
                { StatType.Luck, LuckStat },
                { StatType.Harvesting, HarvestingStat },
            };
            
            _statValueDict = new Dictionary<StatType, int>
            {
                { StatType.MaxHP, MaxHp },
                { StatType.HPRegen, HpRegen },
                { StatType.LifeSteal, LifeStealPercent },
                { StatType.Armor, Armor },
                { StatType.Dodge, DodgePercent },

                { StatType.DamagePercent, DamagePercent },
                { StatType.MeleeDamage, MeleeDamage },
                { StatType.RangedDamage, RangedDamage },
                { StatType.ElementalDamage, ElementalDamage },
                { StatType.AttackSpeed, AttackSpeedPercent },
                { StatType.CritChance, CritChancePercent },
                { StatType.Range, Range },

                { StatType.Speed, SpeedPercent },
                { StatType.Luck, Luck },
                { StatType.Harvesting, Harvesting },
            };

            _statInfoDict = new Dictionary<StatType, string>()
            {
                { StatType.MaxHP, MaxHpDetail },
                { StatType.HPRegen, HpRegenDetail },
                { StatType.LifeSteal, LifeStealDetail },
                { StatType.Armor, ArmorDetail },
                { StatType.Dodge, DodgeDetail },

                { StatType.DamagePercent, DamageDetail },
                { StatType.MeleeDamage, MeleeDamageDetail },
                { StatType.RangedDamage, RangedDamageDetail },
                { StatType.ElementalDamage, ElementalDamageDetail },
                { StatType.AttackSpeed, AttackSpeedDetail },
                { StatType.CritChance, CritChanceDetail },
                { StatType.Range, RangeDetail },

                { StatType.Speed, SpeedDetail },
                { StatType.Luck, LuckDetail },
                { StatType.Harvesting, HarvestingDetail },
            };
        }
        
        public Stat GetStat(StatType type) => _statsDict[type];
        public int GetStatValue(StatType type) => _statValueDict[type];
        public string GetStatInfo(StatType type) => _statInfoDict[type];

        #region ===== Survival =====

        /// <summary>
        /// 每次升级MaxHp+1，MaxHp等于能够承受最大伤害
        /// </summary>
        [Header("Survival")]
        public Stat MaxHpStat = new(10);

        /// <summary>
        /// 每 11.25 / (1.25 + hpRegen) 秒回1滴血
        /// 每秒回 hpRegen / 11.25 + 1 / 9 
        /// </summary>
        public Stat HpRegenStat = new(0);

        /// <summary>
        /// 百分比数值，有每次攻击lifeSteal%概率回复1点血，0.1秒内不能吸血
        /// </summary>
        public Stat LifeStealStat = new(0);

        /// <summary>
        /// 假如受到的总伤害为damage
        /// 设减伤为x
        /// 如果armor >= 0,  x = 1 / (1 + (armor / 15))   damage = damage * (1 - x)
        /// 否则 x = (15 - 2 * armor) / (15 - armor)      damage = damage * x
        /// 显示公式 Damage Reduction % = ROUND( (1 - DmgReceived%) * 100 )
        /// </summary>
        public Stat ArmorStat = new(0);

        /// <summary>
        /// 百分比数值，有x%的概率躲避伤害，60%为上限，如果为负，则什么都不发生
        /// </summary>
        public Stat DodgeStat = new(0);

        // ===== 数值属性 =====

        public int MaxHp => Mathf.RoundToInt(MaxHpStat.Value);
        public int HpRegen => Mathf.RoundToInt(HpRegenStat.Value);
        public int LifeStealPercent => Mathf.RoundToInt(LifeStealStat.Value);
        public int Armor => Mathf.RoundToInt(ArmorStat.Value);
        public int DodgePercent => Mathf.Clamp(Mathf.RoundToInt(DodgeStat.Value), 0, 60);

        // ===== 计算属性 =====

        public float HpRegenPerSecond => HpRegen / 11.25f + 1f / 9f;

        public float LifeStealChance =>
            Mathf.Max(0, LifeStealStat.Value) / 100f;

        public float DamageTakenMultiplier
        {
            get
            {
                if (Armor >= 0)
                    return 1f / (1f + Armor / 15f);

                return (15f - 2f * Armor) / (15f - Armor);
            }
        }

        public float DodgeChance => DodgePercent / 100f;

        // ===== 显示字符串 =====

        public string MaxHpDetail =>
            $"最大能够承受 {MaxHp} 点伤害";

        public string HpRegenDetail =>
            $"每秒回复 {HpRegenPerSecond} Hp";

        public string LifeStealDetail =>
            $"每次造成伤害，{LifeStealPercent}% 的概率回复 1 Hp";

        public string ArmorDetail =>
            $"收到伤害为实际伤害的 {Mathf.RoundToInt((1f - DamageTakenMultiplier) * 100f)} %";

        public string DodgeDetail =>
            $"{DodgePercent}% 的概率闪避伤害";

        #endregion


        #region ===== Offense =====

        /// <summary>
        /// 百分比数值，增加x%伤害
        /// </summary>
        [Header("Offense")]
        public Stat DamagePercentStat = new(0);

        /// <summary>
        /// 基于某一近战加成增加
        /// </summary>
        public Stat MeleeDamageStat = new(0);

        /// <summary>
        /// 基于某一远程加成增加
        /// </summary>
        public Stat RangedDamageStat = new(0);

        /// <summary>
        /// 基于某一元素加成增加
        /// </summary>
        public Stat ElementalDamageStat = new(0);

        /// <summary>
        /// 百分比数值，增加x%攻击速度
        /// </summary>
        public Stat AttackSpeedStat = new(0);

        /// <summary>
        /// 增加x%暴击率
        /// </summary>
        public Stat CritChanceStat = new(3);

        /// <summary>
        /// 增加范围
        /// </summary>
        public Stat RangeStat = new(0);

        // ===== 数值 =====

        public int DamagePercent => Mathf.RoundToInt(DamagePercentStat.Value);
        public int AttackSpeedPercent => Mathf.RoundToInt(AttackSpeedStat.Value);
        public int CritChancePercent => Mathf.RoundToInt(CritChanceStat.Value);

        public int MeleeDamage => Mathf.RoundToInt(MeleeDamageStat.Value);
        public int RangedDamage => Mathf.RoundToInt(RangedDamageStat.Value);
        public int ElementalDamage => Mathf.RoundToInt(ElementalDamageStat.Value);
        public int Range => Mathf.RoundToInt(RangeStat.Value);

        // ===== 乘区 =====

        public float DamageMultiplier => 1f + DamagePercent / 100f;
        public float AttackSpeedMultiplier => 1f + AttackSpeedStat.Value / 100f;
        public float CritChance => Mathf.Max(0, CritChanceStat.Value) / 100f;

        // ===== 显示 =====

        public string DamageDetail => $"伤害增加 {DamagePercent}%";
        public string AttackSpeedDetail => $"攻速增加 {AttackSpeedPercent}%";
        public string CritChanceDetail => $"{CritChancePercent}% 造成暴击伤害";
        public string MeleeDamageDetail => $"近战伤害增加 {MeleeDamage}";
        public string RangedDamageDetail => $"远程伤害增加 {RangedDamage}";
        public string ElementalDamageDetail => $"元素伤害增加 {ElementalDamage}";
        public string RangeDetail => $"范围增加 {Range}";

        #endregion


        #region ===== Utility =====

        [Header("Utility")]
        public Stat SpeedStat = new(0);
        public Stat LuckStat = new(0);
        public Stat HarvestingStat = new(0);
        public Stat MaxWeaponsStat = new(6);

        public int SpeedPercent => Mathf.RoundToInt(SpeedStat.Value);
        public int Luck => Mathf.RoundToInt(LuckStat.Value);
        public int Harvesting => Mathf.RoundToInt(HarvestingStat.Value);
        public int MaxWeapons => Mathf.RoundToInt(MaxWeaponsStat.Value);

        public float MoveSpeedMultiplier => 1f + SpeedPercent / 100f;

        public string SpeedDetail => $"速度增加 {SpeedPercent}%";
        public string LuckDetail => $"幸运增加 {Luck}";
        public string HarvestingDetail =>
            $"每回合结束获得 {Harvesting}经验和金币";

        #endregion
    }
}