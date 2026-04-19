using System;
using System.Collections.Generic;
using Data;
using Items.Abilities;
using Stats;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace Items
{
    [CreateAssetMenu(menuName = "Game/Item")]
    public class ItemData : GameDataAsset
    {
        [Header("Identity")]
        public int itemID;
        public Sprite itemIcon;
        public string itemName;

        [Header("Economy")]
        public Rarity rarity;
        public int price;

        [Header("Presentation")]
        [TextArea] public string summary;
        [TextArea] public string description;

        [Header("Effects")]
        public List<ItemModify> modifies;
        [FormerlySerializedAs("effects")]
        public List<ItemAbility> abilities;

        public override int DataId => itemID;
        public override string DisplayName => itemName;
        public override Sprite Icon => itemIcon;
        public override string Summary => GetSummary();
        public override string ValidationSourceName => GetValidationSourceName();

        public override bool IsValid()
        {
            return itemID >= 0 && !string.IsNullOrWhiteSpace(itemName);
        }

        public int GetDataId()
        {
            return itemID;
        }

        public string GetDisplayName()
        {
            return itemName;
        }

        public string GetValidationSourceName()
        {
            return string.IsNullOrWhiteSpace(itemName) ? name : itemName;
        }

        public Sprite GetIcon()
        {
            return itemIcon != null ? itemIcon : null;
        }

        public string GetSummary()
        {
            if (!string.IsNullOrWhiteSpace(summary))
                return summary.Trim();

            return string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
        }

        public Rarity GetRarity()
        {
            return rarity;
        }

        public int GetRecyclePrice()
        {
            return Mathf.Max(0, Mathf.FloorToInt(price * 0.25f));
        }
    }

    [Serializable]
    public class ItemModify
    {
        public StatType statType;
        public float value;
        public StatModType modType;
    }
}
