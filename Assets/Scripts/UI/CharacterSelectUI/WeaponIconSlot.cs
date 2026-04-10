using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI.CharacterSelectUI
{
    public class WeaponIconSlot : SelectionSlotBase<WeaponSelectionEntry>
    {
        [Header("Visuals")]
        [SerializeField] private Image weaponIcon;

        protected override void RefreshVisuals(WeaponLoadoutEntry data, bool isRandomSlot)
        {
            if (weaponIcon == null)
                return;

            if (isRandomSlot)
            {
                weaponIcon.sprite = randomIcon;
                return;
            }

            weaponIcon.sprite = data != null ? data.GetIcon() : null;
        }
    }
}
