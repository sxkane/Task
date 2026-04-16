using UnityEngine;
using Weapons;

namespace UI.GameSceneUI.IconSlots
{
    [CreateAssetMenu(menuName = "Game/Rarity Visual")]
    public class RarityVisual : ScriptableObject
    {
        public Rarity rarity;

        [Header("Gradient")]
        public Color topColor;
        public Color bottomColor = Color.black;

        [Header("Glow")]
        public bool useGlow;
        public Color glowColor;
        public float glowIntensity = 1f;
    }
}