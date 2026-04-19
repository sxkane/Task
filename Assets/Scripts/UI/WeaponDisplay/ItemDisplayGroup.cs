using System.Collections.Generic;
using Core;
using Events;
using Events.DisplayEvent;
using Items;
using UI.GameSceneUI.IconSlots;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WeaponDisplay
{
    public class ItemDisplayGroup : MonoBehaviour
    {
        [SerializeField] private ItemDisplayPanel itemPanel;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private Transform parent;
        [SerializeField] private Vector2 itemOffset;

        private readonly List<IconSlot> _itemSlots = new();
        private List<ItemData> _items = new();

        private void OnEnable()
        {
            EventBus.Subscribe<OnItemsDisplay>(OnItemsDisplay);
            var itemManager = GameController.Instance != null ? GameController.Instance.ItemManager : null;
            _items = itemManager != null ? itemManager.GetItemsSnapshot() : new List<ItemData>();
            RefreshItemSlots();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnItemsDisplay>(OnItemsDisplay);
            ClearSlotCallbacks();
        }

        private void OnItemsDisplay(OnItemsDisplay eventData)
        {
            _items = eventData.Items ?? new List<ItemData>();
            RefreshItemSlots();
        }

        private void RefreshItemSlots()
        {
            EnsureSlotCount(_items.Count);

            for (var i = 0; i < _itemSlots.Count; i++)
            {
                var slot = _itemSlots[i];
                if (slot == null)
                    continue;

                if (i < _items.Count && _items[i] != null)
                {
                    var item = _items[i];
                    slot.gameObject.SetActive(true);
                    slot.Set(item.GetIcon(), item.GetRarity());
                    slot.OnClick = null;
                    slot.OnEnter = _ => itemPanel?.Show(item, slot.transform as RectTransform, itemOffset);
                    slot.OnExit = _ => itemPanel?.Hide();
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

            if (parent is RectTransform parentRect)
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        }

        private void EnsureSlotCount(int count)
        {
            if (itemSlotPrefab == null || parent == null)
                return;

            while (_itemSlots.Count < count)
            {
                var obj = Object.Instantiate(itemSlotPrefab, parent, false);
                if (obj.transform is RectTransform rectTransform)
                {
                    rectTransform.localScale = Vector3.one;
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.localRotation = Quaternion.identity;
                }

                var slot = obj.GetComponent<IconSlot>();
                if (slot == null)
                    slot = obj.GetComponentInChildren<IconSlot>(true);

                _itemSlots.Add(slot);
            }
        }

        private void ClearSlotCallbacks()
        {
            for (var i = 0; i < _itemSlots.Count; i++)
            {
                var slot = _itemSlots[i];
                if (slot == null)
                    continue;

                slot.OnClick = null;
                slot.OnEnter = null;
                slot.OnExit = null;
            }
        }
    }
}
