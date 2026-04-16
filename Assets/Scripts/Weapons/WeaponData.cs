using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Game/Weapon")]
    public class WeaponData : GameDataAsset
    {
        [Header("Identity")]
        public int weaponID;
        public string weaponName;
        [TextArea] public string summary;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Visual")]
        public GameObject weaponPrefab;

        [Header("Set Tags")]
        public List<WeaponTag> tags = new();

        [Header("Rarity Settings")]
        public List<WeaponStats> rarityStats;

        public override int DataId => weaponID;
        public override string DisplayName => weaponName;
        public override Sprite Icon => icon;
        public override string Summary => GetSummary();
        public override string ValidationSourceName => GetValidationSourceName();

        public override bool IsValid()
        {
            return weaponID >= 0
                   && !string.IsNullOrWhiteSpace(weaponName)
                   && weaponPrefab != null
                   && rarityStats != null
                   && rarityStats.Count > 0;
        }

        public int GetDataId()
        {
            return weaponID;
        }

        public string GetDisplayName()
        {
            return weaponName;
        }

        public string GetValidationSourceName()
        {
            return string.IsNullOrWhiteSpace(weaponName) ? name : weaponName;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetSummary()
        {
            if (!string.IsNullOrWhiteSpace(summary))
                return summary.Trim();

            return string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
        }

        public WeaponStats GetStats(Rarity rarity)
        {
            if (rarityStats == null || rarityStats.Count == 0)
                return null;

            var found = rarityStats.Find(r => r.rarity == rarity);
            if (found != null)
                return found;

            var normalized = GetClosestAvailableRarity(rarity);
            return rarityStats.Find(r => r != null && r.rarity == normalized);
        }

        public bool HasRarity(Rarity rarity)
        {
            if (rarityStats == null)
                return false;

            return rarityStats.Exists(r => r != null && r.rarity == rarity);
        }

        public WeaponEntry CreateEntry(Rarity rarity)
        {
            var normalizedRarity = GetClosestAvailableRarity(rarity);
            return new WeaponEntry
            {
                weaponData = this,
                rarity = normalizedRarity
            };
        }

        public bool TryCreateEntry(Rarity rarity, out WeaponEntry entry)
        {
            entry = null;
            if (!IsValid())
                return false;

            var normalizedRarity = GetClosestAvailableRarity(rarity);
            if (!HasRarity(normalizedRarity))
                return false;

            entry = CreateEntry(normalizedRarity);
            return true;
        }

        public WeaponEntry CreateDefaultEntry()
        {
            return CreateEntry(GetLowestAvailableRarity());
        }

        public Rarity GetClosestAvailableRarity(Rarity requested)
        {
            if (HasRarity(requested))
                return requested;

            for (int i = (int)requested + 1; i <= (int)Rarity.Legendary; i++)
            {
                var candidate = (Rarity)i;
                if (HasRarity(candidate))
                    return candidate;
            }

            for (int i = (int)requested - 1; i >= (int)Rarity.Common; i--)
            {
                var candidate = (Rarity)i;
                if (HasRarity(candidate))
                    return candidate;
            }

            return requested;
        }

        public Rarity GetLowestAvailableRarity()
        {
            for (int i = (int)Rarity.Common; i <= (int)Rarity.Legendary; i++)
            {
                var candidate = (Rarity)i;
                if (HasRarity(candidate))
                    return candidate;
            }

            return Rarity.Common;
        }

        public bool TryGetNextAvailableRarity(Rarity current, out Rarity next)
        {
            for (int i = (int)current + 1; i <= (int)Rarity.Legendary; i++)
            {
                var candidate = (Rarity)i;
                if (!HasRarity(candidate))
                    continue;

                next = candidate;
                return true;
            }

            next = current;
            return false;
        }

        public IReadOnlyList<WeaponTag> GetTags()
        {
            return tags;
        }
    }
}
