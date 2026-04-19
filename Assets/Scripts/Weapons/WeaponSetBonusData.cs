using System;
using System.Collections.Generic;
using System.Text;
using Stats;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Modifiers;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Game/Weapon Set Bonus")]
    public class WeaponSetBonusData : ScriptableObject
    {
        [Serializable]
        public class SetStatModifier
        {
            public StatType statType;
            public float value;
            public StatModType modType;
        }

        [Serializable]
        public class SetWeaponModifier
        {
            public WeaponStatType statType;
            public float value;
            public StatModType modType;
        }

        [Serializable]
        public class SetTier
        {
            [Range(2, 6)] public int requiredCount = 2;
            [FormerlySerializedAs("modifiers")]
            public List<SetStatModifier> playerModifiers = new();
            public List<SetWeaponModifier> weaponModifiers = new();

            public int RequiredCount => Mathf.Clamp(requiredCount, 2, 6);
        }

        [Header("Identity")]
        [SerializeField] private string setId;
        [SerializeField] private string displayName;

        [Header("Tier Rules")]
        [SerializeField] private List<SetTier> tiers = new();

        public string SetId => string.IsNullOrWhiteSpace(setId) ? name : setId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public IReadOnlyList<SetTier> Tiers => tiers;
        
        public bool IsValid()
        {
            return tiers != null && tiers.Count > 0;
        }

        public SetTier ResolveActiveTier(int tagCount)
        {
            if (!IsValid())
                return null;

            SetTier best = null;
            int bestRequired = int.MinValue;

            for (int i = 0; i < tiers.Count; i++)
            {
                var tier = tiers[i];
                if (tier == null)
                    continue;

                var requiredCount = tier.RequiredCount;
                if (requiredCount > tagCount)
                    continue;

                if (requiredCount > bestRequired)
                {
                    best = tier;
                    bestRequired = requiredCount;
                }
            }

            return best;
        }
    }
}
