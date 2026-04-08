using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelectUI
{
    public class PlayerIconSlot : SelectionSlotBase<PlayerData>
    {
        [Header("Visuals")]
        [SerializeField] private Image icon;

        protected override void RefreshVisuals(PlayerData data, bool isRandomSlot)
        {
            if (icon != null && data != null)
                icon.sprite = isRandomSlot ? randomIcon : data.playerIcon;
        }
    }
}
