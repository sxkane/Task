using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace UI.GameSceneUI.IconSlots
{
    public class RarityVisualDatabase : ScriptableObject
    {
        [SerializeField] private List<RarityVisual> rarityVisuals;

        private Dictionary<Rarity, RarityVisual> _dict;

        private void OnEnable()
        {
            _dict = new Dictionary<Rarity, RarityVisual>();

            foreach (var v in rarityVisuals)
            {
                if (v != null)
                    _dict[v.rarity] = v;
            }
        }

        public RarityVisual GetRarityVisual(Rarity rarity)
        {
            if (_dict != null && _dict.TryGetValue(rarity, out var v))
                return v;

            Debug.LogWarning($"No RarityVisual for {rarity}");
            return null;
        }
    }
}