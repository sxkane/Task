using System;
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

        public GameDatabase gameDatabase;
        
        private PlayerData _selectedPlayer;

        public Action<PlayerData> PlayerSelected;
        public Action StartButtonClicked;

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

            foreach (var player in allPlayers)
            {
                var slot = Instantiate(playerSlotPrefab, playerIconParent);
                slot.Initialize(player, OnPlayerSelected);
            }

            OnPlayerSelected(null);
        }

        private void OnEnable()
        {
            startButton.onClick.AddListener(OnClickStart);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(OnClickStart);
        }

        private void OnPlayerSelected(PlayerData data)
        {
            if (data == null)
            {
                _selectedPlayer = null;
                infoSlot.ShowRandomPlayer();
            }
            else
            {
                _selectedPlayer = data;
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