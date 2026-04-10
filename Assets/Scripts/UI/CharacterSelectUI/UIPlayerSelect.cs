using System.Collections.Generic;
using Core;
using Data;
using Player;
using UnityEngine;
using Weapons;

namespace UI.CharacterSelectUI
{
    public class UIPlayerSelect : MonoBehaviour
    {
        [Header("Pages")]
        [SerializeField] private CharacterSelectionPage characterSelectionPage;
        [SerializeField] private WeaponSelectionPage weaponSelectionPage;

        [Header("Data")]
        [SerializeField] private GameDatabase gameDatabase;

        private PlayerData _confirmedPlayer;

        private void Start()
        {
            characterSelectionPage.Configure(gameDatabase.GetPlayerEntries(), HandleCharacterConfirmed, HandleRandomPlayer);
            weaponSelectionPage.Configure(HandleBackToCharacterSelection, HandleWeaponConfirmed);
            OpenCharacterSelectionPage();
        }

        private void HandleCharacterConfirmed(PlayerData player)
        {
            _confirmedPlayer = player;
            OpenWeaponSelectionPage(player);
        }

        private void HandleWeaponConfirmed(WeaponSelectionEntry weapon)
        {
            if (_confirmedPlayer == null)
                return;

            StartGame(_confirmedPlayer, weapon);
        }
        
        private void HandleRandomPlayer()
        {
            var players = gameDatabase.GetPlayerEntries();
            if (players.Count == 0)
                return;

            _confirmedPlayer = players[UnityEngine.Random.Range(0, players.Count)];

            OpenWeaponSelectionPage(_confirmedPlayer);
        }

        private void HandleBackToCharacterSelection()
        {
            OpenCharacterSelectionPage();
        }

        private void OpenCharacterSelectionPage()
        {
            characterSelectionPage.PreviewCharacter(_confirmedPlayer);
            characterSelectionPage.SetPageVisible(true);
            weaponSelectionPage.SetPageVisible(false);
        }

        private void OpenWeaponSelectionPage(PlayerData player)
        {
            weaponSelectionPage.ShowStarterWeapons(player);
            characterSelectionPage.SetPageVisible(false);
            weaponSelectionPage.SetPageVisible(true);
        }

        private static void StartGame(PlayerData player, WeaponSelectionEntry weapon)
        {
            var selectedWeapons = new List<WeaponSelectionEntry>();
            if (weapon != null)
                selectedWeapons.Add(weapon);

            GameRoot.Instance.StartGame(new GameSession
            {
                SelectedPlayer = player,
                SelectedWeaponSelections = selectedWeapons
            });
        }
    }
}
