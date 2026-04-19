using System;
using System.Collections.Generic;
using Data;
using Items;
using Rewards.Shops;
using Stats;
using UnityEngine;
using Weapons;
using Random = UnityEngine.Random;

namespace Rewards.StatRewards
{
    public static class ItemGenerator
    {
        private static readonly float[] MaxRarityRate = { 1.0f, 0.6f, 0.25f, 0.08f };
        private static readonly float[] BaseRarityRate = { 1.0f, 0.0f, 0.0f, 0.0f };
        private static readonly float[] AddRarityRate = { 0.0f, 0.06f, 0.02f, 0.0023f };
        private static readonly int[] MinWave = { 1, 2, 4, 8 };

        private static readonly Dictionary<StatType, float[]> StatValues = new()
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
                    return (Rarity)i;
            }

            return Rarity.Common;
        }

        public static StatReward GetStatReward(int currentWave, int luck)
        {
            return GetStatReward(currentWave, luck, null);
        }

        public static StatReward GetStatReward(int currentWave, int luck, ISet<StatType> excludedTypes)
        {
            var rarity = GetRarity(currentWave, luck);
            var availableStats = new List<StatType>();

            foreach (var statType in StatValues.Keys)
            {
                if (excludedTypes != null && excludedTypes.Contains(statType))
                    continue;

                availableStats.Add(statType);
            }

            if (availableStats.Count == 0)
                availableStats.AddRange(StatValues.Keys);

            var selectedStat = availableStats[Random.Range(0, availableStats.Count)];
            var rarityLevel = (int)rarity;
            var statValue = StatValues[selectedStat][rarityLevel];

            return new StatReward
            {
                type = selectedStat,
                value = statValue
            };
        }

        public static RewardOption GetUpgradeOption(int currentWave, int luck, StatIconDatabase statIconDatabase)
        {
            return GetUpgradeOption(currentWave, luck, statIconDatabase, null);
        }

        public static RewardOption GetUpgradeOption(int currentWave, int luck, StatIconDatabase statIconDatabase, ISet<StatType> excludedTypes)
        {
            var rarity = GetRarity(currentWave, luck);
            var reward = GetStatReward(currentWave, luck, excludedTypes);

            return new RewardOption
            {
                title = $"{rarity} Upgrade",
                description = $"{reward.type} +{reward.value:0.#}",
                icon = statIconDatabase != null ? statIconDatabase.GetIcon(reward.type) : null,
                reward = reward
            };
        }

        public static ShopItem GetShopOffer(int currentWave, int luck, GameDatabase data, ISet<string> excludedKeys = null)
        {
            if (data == null)
                return null;

            var rarity = GetRarity(currentWave, luck);
            var weaponCandidates = BuildWeaponCandidates(data.weapons, rarity, excludedKeys);
            var itemCandidates = BuildItemCandidates(data.items, rarity, excludedKeys);

            var canOfferWeapon = weaponCandidates.Count > 0;
            var canOfferItem = itemCandidates.Count > 0;
            if (!canOfferWeapon && !canOfferItem)
                return null;

            var offerItem = canOfferItem && (!canOfferWeapon || Random.value < 0.5f);
            return offerItem
                ? itemCandidates[Random.Range(0, itemCandidates.Count)]
                : weaponCandidates[Random.Range(0, weaponCandidates.Count)];
        }

        public static string GetShopItemKey(ShopItem item)
        {
            if (item == null)
                return string.Empty;

            if (item.IsItem)
                return $"item:{item.itemData?.GetDataId() ?? -1}";

            return item.weaponEntry != null
                ? $"weapon:{item.weaponEntry.GetDataId()}:{(int)item.weaponEntry.GetRarity()}"
                : string.Empty;
        }

        private static List<ShopItem> BuildWeaponCandidates(List<WeaponData> weapons, Rarity rarity, ISet<string> excludedKeys)
        {
            var candidates = new List<ShopItem>();
            if (weapons == null)
                return candidates;

            for (var i = 0; i < weapons.Count; i++)
            {
                var candidate = weapons[i];
                if (candidate == null || !candidate.HasRarity(rarity))
                    continue;

                var entry = candidate.CreateEntry(rarity);
                if (entry == null || !entry.IsValid())
                    continue;

                var item = new ShopItem
                {
                    type = ShopItemType.Weapon,
                    weaponEntry = entry
                };

                var key = GetShopItemKey(item);
                if (excludedKeys != null && excludedKeys.Contains(key))
                    continue;

                candidates.Add(item);
            }

            return candidates;
        }

        private static List<ShopItem> BuildItemCandidates(List<ItemData> items, Rarity rarity, ISet<string> excludedKeys)
        {
            var candidates = new List<ShopItem>();
            if (items == null)
                return candidates;

            for (var i = 0; i < items.Count; i++)
            {
                var candidate = items[i];
                if (candidate == null || candidate.GetRarity() != rarity)
                    continue;

                var item = new ShopItem
                {
                    type = ShopItemType.Item,
                    itemData = candidate
                };

                var key = GetShopItemKey(item);
                if (excludedKeys != null && excludedKeys.Contains(key))
                    continue;

                candidates.Add(item);
            }

            return candidates;
        }
    }
}
