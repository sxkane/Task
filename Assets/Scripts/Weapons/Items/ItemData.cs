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
        public int itemID;
        public Image itemIcon;
        public string itemName;
        public Rarity rarity;
        public int price;
        public string description;

        public List<ItemModify> modifies;
        
        public List<Effect> effects;
    }

    [Serializable]
    public class ItemModify
    {
        public StatType statType;
        public float value;
        public StatModType modType;
    }
}