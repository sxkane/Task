using System;
using System.Collections.Generic;
using Stats;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Database/Stat Icon Database")]
    public class StatIconDatabase : ScriptableObject
    {
        [Serializable]
        public class StatIconEntry
        {
            public StatType statType;
            public Sprite icon;
        }

        [SerializeField] private List<StatIconEntry> statIcons = new();

        private static StatIconDatabase _defaultInstance;
        private Dictionary<StatType, Sprite> _iconMap;

        public Sprite GetIcon(StatType statType)
        {
            EnsureCache();
            return _iconMap.TryGetValue(statType, out var icon) ? icon : null;
        }

        private void EnsureCache()
        {
            if (_iconMap != null)
                return;
            _iconMap = new Dictionary<StatType, Sprite>();
            foreach (var statIcon in statIcons)
            {
                _iconMap.Add(statIcon.statType, statIcon.icon);
            }
        }
    }
}
