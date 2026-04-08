using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI.CharacterSelectUI
{
    public class WeaponSelectionPage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform weaponSlotParent;
        [SerializeField] private WeaponIconSlot weaponSlotPrefab;
        [SerializeField] private InformationSlot weaponInfoSlot;
        [SerializeField] private InformationSlot playerInfoSlot;
        [SerializeField] private Button backButton;

        private readonly List<WeaponIconSlot> _slots = new();
        private readonly List<WeaponLoadoutEntry> _starterWeapons = new();
        private SelectionSlotBase<WeaponLoadoutEntry> _previewedSlot;
        private PlayerData _currentPlayer;
        private Action _onBackRequested;
        private Action<WeaponLoadoutEntry> _onWeaponConfirmed;

        private void OnEnable()
        {
            if (backButton != null)
                backButton.onClick.AddListener(HandleBackRequested);
        }

        private void OnDisable()
        {
            if (backButton != null)
                backButton.onClick.RemoveListener(HandleBackRequested);
        }

        public void Configure(Action onBackRequested, Action<WeaponLoadoutEntry> onWeaponConfirmed)
        {
            _onBackRequested = onBackRequested;
            _onWeaponConfirmed = onWeaponConfirmed;
        }

        public void SetPageVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public PlayerData CurrentPlayer => _currentPlayer;

        public void ShowStarterWeapons(PlayerData player)
        {
            _currentPlayer = player;
            playerInfoSlot.ShowPlayer(player);
            RebuildSlots(player);

            if (_slots.Count > 0)
                HandleSlotHovered(_slots[0], _slots[0].Data);
            else
                weaponInfoSlot.Clear();
        }

        private void RebuildSlots(PlayerData player)
        {
            foreach (Transform child in weaponSlotParent)
                Destroy(child.gameObject);

            _slots.Clear();
            _starterWeapons.Clear();
            _previewedSlot = null;

            var randomSlot = Instantiate(weaponSlotPrefab, weaponSlotParent);
            randomSlot.Configure(null, true, HandleSlotHovered, HandleSlotClicked);
            _slots.Add(randomSlot);

            if (player == null)
                return;

            foreach (var starterWeapon in player.starterWeapons)
            {
                if (starterWeapon?.weaponData == null)
                    continue;

                _starterWeapons.Add(starterWeapon);
                var slot = Instantiate(weaponSlotPrefab, weaponSlotParent);
                slot.Configure(starterWeapon, false, HandleSlotHovered, HandleSlotClicked);
                _slots.Add(slot);
            }
        }

        private void HandleSlotHovered(SelectionSlotBase<WeaponLoadoutEntry> slot, WeaponLoadoutEntry data)
        {
            _previewedSlot = slot;
            UpdateSelectionVisuals();

            if (slot != null && slot.IsRandomSlot)
                weaponInfoSlot.ShowRandomWeapon();
            else
                weaponInfoSlot.ShowWeapon(data);
        }

        private void HandleSlotClicked(SelectionSlotBase<WeaponLoadoutEntry> slot, WeaponLoadoutEntry data)
        {
            _previewedSlot = slot;
            UpdateSelectionVisuals();

            if (slot != null && slot.IsRandomSlot)
            {
                var randomWeapon = GetRandomStarterWeapon();
                if (randomWeapon != null)
                    _onWeaponConfirmed?.Invoke(randomWeapon);

                return;
            }

            if (data != null)
                _onWeaponConfirmed?.Invoke(data);
        }

        private void HandleBackRequested()
        {
            _previewedSlot = null;
            weaponInfoSlot.Clear();
            _onBackRequested?.Invoke();
        }

        private void UpdateSelectionVisuals()
        {
            foreach (var slot in _slots)
                slot.SetSelected(slot == _previewedSlot);
        }

        private WeaponLoadoutEntry GetRandomStarterWeapon()
        {
            if (_starterWeapons.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, _starterWeapons.Count);
            return _starterWeapons[randomIndex];
        }
    }
}
