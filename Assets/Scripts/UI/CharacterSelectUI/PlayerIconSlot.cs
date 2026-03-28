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
        private Action<PlayerData> _onClick;
            
        public void Initialize(PlayerData data, Action<PlayerData> onClick)
        {
            _data = data;
            _onClick = onClick;

            if (data != null)
                icon = data.playerIcon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke(_data);
        }
        
        public void SetSelected(bool value)
        {
            selectedFrame.SetActive(value);
        }
    }
}