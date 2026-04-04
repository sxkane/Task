using System.Collections.Generic;
using Core;
using Data;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.CharacterSelectUI
{
    public class UIPlayerSelect : MonoBehaviour
    {
        public static UIPlayerSelect Instance;

        [SerializeField] private Button startButton;
        [SerializeField] private Transform playerIconParent;
        [SerializeField] private PlayerIconSlot playerSlotPrefab;
        [SerializeField] private PlayerInformationSlot infoSlot;
        [SerializeField] private GameDatabase gameDatabase;
        
        private readonly List<PlayerIconSlot> _slots = new();
        private PlayerData _selectedPlayer;
        private PlayerIconSlot _selectedSlot;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            var allPlayers = gameDatabase.players;

            var randomSlot = Instantiate(playerSlotPrefab, playerIconParent);
            randomSlot.Initialize(null, OnPlayerSelected);
            _slots.Add(randomSlot);

            foreach (var player in allPlayers)
            {
                var slot = Instantiate(playerSlotPrefab, playerIconParent);
                slot.Initialize(player, OnPlayerSelected);
                _slots.Add(slot);
            }

            OnPlayerSelected(null, null);
        }

        private void OnEnable()
        {
            startButton.onClick.AddListener(OnClickStart);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(OnClickStart);
        }

        private void OnPlayerSelected(PlayerIconSlot slot, PlayerData data)
        {
            _selectedSlot = slot;
            _selectedPlayer = data;

            foreach (var iconSlot in _slots)
            {
                iconSlot.SetSelected(iconSlot == _selectedSlot && _selectedSlot != null);
            }

            if (data == null)
            {
                infoSlot.ShowRandomPlayer();
            }
            else
            {
                infoSlot.ShowPlayer(data.playerName, "");
            }
        }

        private void OnClickStart()
        {
            SelectPlayer();
            GameRoot.Instance.StartGame(new GameSession { SelectedPlayer = _selectedPlayer });
        }

        private void SelectPlayer()
        {
            if (_selectedPlayer != null) 
                return;

            var allPlayers = gameDatabase.players;
            _selectedPlayer = allPlayers[Random.Range(0, allPlayers.Count)];
        }
    }
}
