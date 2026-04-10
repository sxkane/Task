using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Rewards.Shops;
using Stats;
using UnityEngine;
using Weapons;
using Random = UnityEngine.Random;

namespace Rewards.StatRewards
{
    public static class ItemGenerator
    {
        // 顺序：Common(白)、Rare(蓝)、Epic(紫)、Legendary(红)
        private static readonly float[] MaxRarityRate = { 1.0f, 0.6f, 0.25f, 0.08f };    // 概率上限
        private static readonly float[] BaseRarityRate = { 1.0f, 0.0f, 0.0f, 0.0f };     // 基础概率
        private static readonly float[] AddRarityRate = { 0.0f, 0.06f, 0.02f, 0.0023f }; // 每波增加的概率
        private static readonly int[] MinWave = { 1, 2, 4, 8 };                          // 首次出现的波数

        private static readonly Dictionary<StatType, float[]> StatValues = new Dictionary<StatType, float[]>
        {
            { StatType.MaxHP, new[] { 3f, 6f, 9f, 12f } },
            { StatType.HPRegen, new[] { 2f, 3f, 4f, 5f } },
            { StatType.LifeSteal, new[] { 1f, 2f, 3f, 4f } },
            { StatType.DamagePercent, new[] { 5f, 8f, 12f, 16f } },
            { StatType.MeleeDamage, new[] { 2f, 4f, 6f, 8f } },
            { StatType.RangedDamage, new[] { 1f, 2f, 3f, 4f } },
            { StatType.ElementalDamage, new[] { 1f, 2f, 3f, 4f } },
            { StatType.AttackSpeed, new[] { 5f, 10f, 15f, 20f } },
            { StatType.CritChance, new[] { 3f, 5f, 7f, 9f } },
            { StatType.Range, new[] { 15f, 30f, 45f, 60f } },
            { StatType.Armor, new[] { 1f, 2f, 3f, 4f } },
            { StatType.Dodge, new[] { 3f, 6f, 9f, 12f } },
            { StatType.Speed, new[] { 3f, 6f, 9f, 12f } },
            { StatType.Luck, new[] { 5f, 10f, 15f, 20f } },
            { StatType.Harvesting, new[] { 5f, 8f, 10f, 12f } }
        };
        
        /// <summary>
        /// 根据当前波数和幸运值，随机生成对应的稀有度
        /// </summary>
        /// <param name="currentWave">当前波数（从1开始）</param>
        /// <param name="luck">幸运值（百分比，比如50代表50%幸运）</param>
        /// <returns>随机出的稀有度</returns>
        public static Rarity GetRarity(int currentWave, int luck)
        {
            float[] chance = new float[4];
            float luckMultiplier = 1.0f + (luck / 100.0f);
            
            for (int i = 0; i < chance.Length; i++)
            {
                if (currentWave < MinWave[i])
                {
                    chance[i] = 0f;
                }
                else
                {
                    // (每波增加量 × (当前波数 - 首次波数 - 1) + 基础概率) × 幸运倍率
                    chance[i] = (AddRarityRate[i] * (currentWave - MinWave[i] - 1) + BaseRarityRate[i]) * luckMultiplier;
                }
                chance[i] = Mathf.Clamp01(chance[i]);
                chance[i] = Mathf.Min(chance[i], MaxRarityRate[i]);
            }
            
            float remainingChance = 1.0f;
            for (int i = chance.Length - 1; i >= 0; i--)
            {
                chance[i] = Mathf.Min(chance[i], remainingChance);
                remainingChance -= chance[i];
            }
            
            float randomValue = Random.value;
            float cumulativeChance = 0f;
            for (int i = chance.Length - 1; i >= 0; i--)
            {
                cumulativeChance += chance[i];
                if (randomValue <= cumulativeChance)
                {
                    return (Rarity)i;
                }
            }
            
            return Rarity.Common;
        }

        public static StatReward GetStatReward(int currentWave, int luck)
        {
            Rarity rarity = GetRarity(currentWave, luck);
            
            List<StatType> allStats = new List<StatType>(Enum.GetValues(typeof(StatType)) as StatType[] ?? Array.Empty<StatType>());
            StatType selectedStat = allStats[Random.Range(0, allStats.Count)];
            
            int rarityLevel = (int)rarity;
            float statValue = StatValues[selectedStat][rarityLevel];
            
            return new StatReward
            {
                type = selectedStat,
                value = statValue
            };
        }

        public static ShopItem GetItemReward(int currentWave, int luck, GameDatabase data)
        {
            var weapons = data.weapons;
            var items = data.items;
            
            var rarity = GetRarity(currentWave, luck);

            if (Random.Range(0, 2) == 1)
            {
                return new ShopItem()
                {
                    type = ShopItemType.Item,
                    itemData = items[Random.Range(0, items.Count)],
                };
            }

            return new ShopItem()
            {
                type = ShopItemType.Weapon,
                weaponEntry = new WeaponLoadoutEntry
                {
                    weaponData = weapons[Random.Range(0, weapons.Count)],
                    rarity = rarity
                }
            };
        }
    }
}
