using System;
using System.Collections.Generic;
using Player;
using Stats;
using UnityEngine;
using UnityEngine.Serialization;
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

        public string GetSummaryText()
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
