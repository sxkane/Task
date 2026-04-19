using System.Collections.Generic;
using Events;
using Events.WeaponEvents;
using UI.GameSceneUI.IconSlots;
using UI.GameSceneUI.Reward;
using UnityEngine;
using Weapons;

namespace UI.WeaponDisplay
{
    public class WeaponDisplayGroup : MonoBehaviour
    {
        [SerializeField] private WeaponDisplayPanel weaponPanel;
        [SerializeField] private BonusPanel bonusPanel;
        [SerializeField] private List<IconSlot> weaponSlots;
        [SerializeField] private Vector2 weaponOffset;
        [SerializeField] private Vector2 bonusOffset;

        private IReadOnlyDictionary<WeaponSetBonusData, int> _bonusCount;
        private List<Weapon> _weapons = new();
        private Weapon _lockedWeapon;
        private bool _panelLocked;

        private void OnEnable()
        {
            EventBus.Subscribe<OnWeaponChanged>(OnWeaponChanged);
            if (weaponPanel != null)
                weaponPanel.Closed += OnWeaponPanelClosed;
            RefreshWeaponSlots();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnWeaponChanged>(OnWeaponChanged);
            if (weaponPanel != null)
                weaponPanel.Closed -= OnWeaponPanelClosed;
            ClearSlotCallbacks(weaponSlots);
            _lockedWeapon = null;
            _panelLocked = false;
        }

        private void OnWeaponChanged(OnWeaponChanged eventData)
        {
            _bonusCount = eventData.BonusCount;
            _weapons = eventData.Weapons ?? new List<Weapon>();
            RefreshWeaponSlots();
        }

        private void RefreshWeaponSlots()
        {
            if (weaponSlots == null)
                return;

            for (var i = 0; i < weaponSlots.Count; i++)
            {
                var slot = weaponSlots[i];
                if (slot == null)
                    continue;

                if (i < _weapons.Count && _weapons[i] != null && _weapons[i].Entry != null)
                {
                    var weapon = _weapons[i];
                    slot.gameObject.SetActive(true);
                    slot.Set(weapon.Entry.GetIcon(), weapon.Entry.GetRarity());
                    slot.OnClick = _ => LockWeaponPanel(weapon, slot);
                    slot.OnEnter = _ => { ShowBonusPanel(weapon, slot);
                        PreviewWeaponPanel(weapon, slot); 
                    };
                    slot.OnExit = _ =>
                    {
                        bonusPanel?.Hide();
                        if (!_panelLocked)
                            weaponPanel?.Hide();
                    };
                }
                else
                {
                    slot.Clear();
                    slot.gameObject.SetActive(false);
                    slot.OnClick = null;
                    slot.OnEnter = null;
                    slot.OnExit = null;
                }
            }
        }

        private void ShowBonusPanel(Weapon weapon, IconSlot slot)
        {
            if (weapon == null || slot == null || bonusPanel == null || weapon.Entry?.weaponData == null)
                return;

            var bonusData = weapon.Entry.weaponData.bonusData;
            if (bonusData == null || bonusData.Count == 0)
                return;

            bonusPanel.Show(bonusData, _bonusCount, slot.transform as RectTransform, bonusOffset);
        }

        private void PreviewWeaponPanel(Weapon weapon, IconSlot slot)
        {
            if (_panelLocked || weaponPanel == null || slot == null)
                return;

            weaponPanel.Show(weapon, slot.transform as RectTransform, weaponOffset);
        }

        private void LockWeaponPanel(Weapon weapon, IconSlot slot)
        {
            if (weaponPanel == null || slot == null)
                return;

            _lockedWeapon = weapon;
            _panelLocked = true;
            weaponPanel.Show(weapon, slot.transform as RectTransform, weaponOffset);
        }

        private void OnWeaponPanelClosed()
        {
            _lockedWeapon = null;
            _panelLocked = false;
        }

        private static void ClearSlotCallbacks(List<IconSlot> slots)
        {
            if (slots == null)
                return;

            for (var i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (slot == null)
                    continue;

                slot.OnClick = null;
                slot.OnEnter = null;
                slot.OnExit = null;
            }
        }
    }
}
