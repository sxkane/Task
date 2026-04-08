using System.Collections.Generic;
using Core;
using Data;
using Player;
using UnityEngine;
using Weapons;
using Random = UnityEngine.Random;

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
            characterSelectionPage.Configure(gameDatabase.players, HandleCharacterConfirmed, HandleRandomPlayer);
            weaponSelectionPage.Configure(HandleBackToCharacterSelection, HandleWeaponConfirmed);
            OpenCharacterSelectionPage();
        }

        private void HandleCharacterConfirmed(PlayerData player)
        {
            _confirmedPlayer = player;
            OpenWeaponSelectionPage(player);
        }

        private void HandleWeaponConfirmed(WeaponLoadoutEntry weapon)
        {
            if (_confirmedPlayer == null)
                return;

            StartGame(_confirmedPlayer, weapon);
        }
        
        private void HandleRandomPlayer()
        {
            if (gameDatabase.players == null || gameDatabase.players.Count == 0)
                return;

            _confirmedPlayer = gameDatabase.players[Random.Range(0, gameDatabase.players.Count)];

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

        private static WeaponLoadoutEntry GetRandomStarterWeapon(PlayerData player)
        {
            var starterWeapons = player != null ? player.starterWeapons : null;
            if (starterWeapons == null || starterWeapons.Count == 0)
                return null;

            return starterWeapons[Random.Range(0, starterWeapons.Count)];
        }

        private static void StartGame(PlayerData player, WeaponLoadoutEntry weapon)
        {
            var selectedWeapons = new List<WeaponLoadoutEntry>();
            if (weapon != null)
                selectedWeapons.Add(weapon);

            GameRoot.Instance.StartGame(new GameSession
            {
                SelectedPlayer = player,
                SelectedWeapons = selectedWeapons
            });
        }
    }
}
