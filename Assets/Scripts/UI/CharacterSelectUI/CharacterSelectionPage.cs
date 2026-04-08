using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace UI.CharacterSelectUI
{
    public class CharacterSelectionPage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerIconParent;
        [SerializeField] private PlayerIconSlot playerSlotPrefab;
        [SerializeField] private InformationSlot infoSlot;

        private readonly List<PlayerIconSlot> _slots = new();
        private SelectionSlotBase<PlayerData> _previewedSlot;
        private Action<PlayerData> _onCharacterConfirmed;
        private Action _onRandomConfirmed;

        public void Configure(
            IReadOnlyList<PlayerData> players,
            Action<PlayerData> onCharacterConfirmed,
            Action onRandomConfirmed)
        {
            _onCharacterConfirmed = onCharacterConfirmed;
            _onRandomConfirmed = onRandomConfirmed;

            RebuildSlots(players);
            PreviewDefaultSelection();
        }

        public void SetPageVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void PreviewCharacter(PlayerData player)
        {
            if (player == null)
            {
                PreviewDefaultSelection();
                return;
            }

            foreach (var slot in _slots)
            {
                if (slot != null && slot.Data == player)
                {
                    HandleSlotHovered(slot, player);
                    return;
                }
            }

            PreviewDefaultSelection();
        }

        private void RebuildSlots(IReadOnlyList<PlayerData> players)
        {
            foreach (Transform child in playerIconParent)
                Destroy(child.gameObject);

            _slots.Clear();

            var randomSlot = Instantiate(playerSlotPrefab, playerIconParent);
            randomSlot.Configure(null, true, HandleSlotHovered, HandleSlotClicked);
            _slots.Add(randomSlot);

            if (players == null)
                return;

            foreach (var player in players)
            {
                var slot = Instantiate(playerSlotPrefab, playerIconParent);
                slot.Configure(player, false, HandleSlotHovered, HandleSlotClicked);
                _slots.Add(slot);
            }
        }

        private void PreviewDefaultSelection()
        {
            if (_slots.Count == 0)
                return;

            HandleSlotHovered(_slots[0], null);
        }

        private void HandleSlotHovered(SelectionSlotBase<PlayerData> slot, PlayerData data)
        {
            _previewedSlot = slot;
            RefreshSelectionVisuals();

            if (slot != null && slot.IsRandomSlot)
                infoSlot.ShowRandomPlayer();
            else
                infoSlot.ShowPlayer(data);
        }

        private void HandleSlotClicked(SelectionSlotBase<PlayerData> slot, PlayerData data)
        {
            if (slot != null && slot.IsRandomSlot)
            {
                _onRandomConfirmed?.Invoke();
                return;
            }

            if (data == null)
                return;

            _onCharacterConfirmed?.Invoke(data);
        }

        private void RefreshSelectionVisuals()
        {
            foreach (var slot in _slots)
                slot.SetSelected(slot == _previewedSlot);
        }
    }
}
