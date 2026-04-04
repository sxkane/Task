using System;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.CharacterSelectUI
{
    public class PlayerIconSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject selectedFrame;
        [SerializeField] private Image icon;

        private PlayerData _data;
        private Action<PlayerIconSlot, PlayerData> _onClick;
            
        public void Initialize(PlayerData data, Action<PlayerIconSlot, PlayerData> onClick)
        {
            _data = data;
            _onClick = onClick;
            SetSelected(false);

            if (data != null)
                icon.sprite = data.playerIcon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke(this, _data);
        }
        
        public void SetSelected(bool value)
        {
            selectedFrame.SetActive(value);
        }
    }
}
