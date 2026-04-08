using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.CharacterSelectUI
{
    public abstract class SelectionSlotBase<TData> : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject selectedFrame;
        [SerializeField] protected Sprite randomIcon;
        

        private TData _data;
        private bool _isRandomSlot;
        private Action<SelectionSlotBase<TData>, TData> _onHover;
        private Action<SelectionSlotBase<TData>, TData> _onClick;

        public TData Data => _data;
        public bool IsRandomSlot => _isRandomSlot;

        public void Configure(
            TData data,
            bool isRandomSlot,
            Action<SelectionSlotBase<TData>, TData> onHover,
            Action<SelectionSlotBase<TData>, TData> onClick)
        {
            _data = data;
            _isRandomSlot = isRandomSlot;
            _onHover = onHover;
            _onClick = onClick;

            SetSelected(false);
            RefreshVisuals(data, isRandomSlot);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _onHover?.Invoke(this, _data);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke(this, _data);
        }

        public void SetSelected(bool value)
        {
            if (selectedFrame != null)
                selectedFrame.SetActive(value);
        }

        protected abstract void RefreshVisuals(TData data, bool isRandomSlot);
    }
}
