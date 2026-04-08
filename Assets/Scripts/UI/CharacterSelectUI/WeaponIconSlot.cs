using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI.CharacterSelectUI
{
    public class WeaponIconSlot : SelectionSlotBase<WeaponLoadoutEntry>
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

            weaponIcon.sprite = data?.weaponData != null ? data.weaponData.icon : null;
        }
    }
}
