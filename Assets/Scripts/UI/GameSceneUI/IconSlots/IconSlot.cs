using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapons;

namespace UI.GameSceneUI.IconSlots
{
    public class IconSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private static readonly int TopColor = Shader.PropertyToID("_TopColor");
        private static readonly int BottomColor = Shader.PropertyToID("_BottomColor");

        [Header("Core")]
        [SerializeField] private Image icon;
        [SerializeField] private Image rarityBackground;
        [SerializeField] private Image glow;

        [Header("Config")]
        [SerializeField] private RarityVisualDatabase rarityVisuals;

        private Material _bgMaterial;
        private bool _useFlatBackgroundInMaskedView;

        public Action<PointerEventData> OnEnter;
        public Action<PointerEventData> OnExit;
        public Action<PointerEventData> OnClick;

        private void Awake()
        {
            _useFlatBackgroundInMaskedView = GetComponentInParent<RectMask2D>() != null;

            if (_useFlatBackgroundInMaskedView && rarityBackground != null)
            {
                rarityBackground.material = null;
                return;
            }

            if (rarityBackground != null && rarityBackground.material != null)
            {
                _bgMaterial = Instantiate(rarityBackground.material);
                rarityBackground.material = _bgMaterial;
            }
        }

        public void Set(Sprite sprite, Rarity rarity)
        {
            SetIcon(sprite);
            ApplyRarity(rarity);
        }

        public void SetIcon(Sprite sprite)
        {
            if (icon != null)
                icon.sprite = sprite;
        }

        public void ApplyRarity(Rarity rarity)
        {
            var config = rarityVisuals.GetRarityVisual(rarity);
            if (config == null)
                return;

            if (_bgMaterial != null)
            {
                _bgMaterial.SetColor(TopColor, config.topColor);
                _bgMaterial.SetColor(BottomColor, config.bottomColor);
            }
            else if (rarityBackground != null)
            {
                rarityBackground.color = config.topColor;
            }

            if (glow != null)
            {
                glow.gameObject.SetActive(config.useGlow);
                if (config.useGlow)
                    glow.color = config.glowColor * config.glowIntensity;
            }
        }

        public void Clear()
        {
            if (icon != null)
                icon.sprite = null;

            if (glow != null)
                glow.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData);
        }
    }
}
