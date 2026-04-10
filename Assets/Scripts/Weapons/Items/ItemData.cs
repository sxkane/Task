using System;
using System.Collections.Generic;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Weapons.Items
{
    [CreateAssetMenu(menuName = "Game/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public int itemID;
        public Image itemIcon;
        public string itemName;

        [Header("Economy")]
        public Rarity rarity;
        public int price;

        [Header("Presentation")]
        [TextArea] public string summary;
        [TextArea] public string description;

        [Header("Effects")]
        public List<ItemModify> modifies;
        
        public List<Effect> effects;

        public bool IsValid()
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
            return itemIcon != null ? itemIcon.sprite : null;
        }

        public string GetSummary()
        {
            if (!string.IsNullOrWhiteSpace(summary))
                return summary.Trim();

            return string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
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
