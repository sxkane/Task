using System.Collections.Generic;
using Player;
using UnityEngine;
using Weapons;

namespace Core
{
    public class GameSession
    {
        #region Selection

        public PlayerData SelectedPlayer;
        public List<WeaponEntry> SelectedWeapons;

        #endregion

        #region Roots

        public Transform PlayerRoot { get; private set; }
        public Transform WeaponRoot { get; private set; }
        public Transform EnemyRoot { get; private set; }
        public Transform DropRoot { get; private set; }
        public Transform WorldVfxRoot { get; private set; }
        public Transform WorldTextRoot { get; private set; }

        #endregion

        #region Helpers

        public bool IsValid()
        {
            return SelectedPlayer != null;
        }

        public void ConfigureSceneRoots(GameSceneContext sceneContext)
        {
            if (sceneContext == null)
                return;

            PlayerRoot = sceneContext.PlayerRoot;
            WeaponRoot = sceneContext.WeaponRoot;
            EnemyRoot = sceneContext.EnemyRoot;
            DropRoot = sceneContext.DropRoot;
            WorldVfxRoot = sceneContext.WorldVfxRoot;
            WorldTextRoot = sceneContext.WorldTextRoot;
        }

        public Transform GetRoot(GameSessionRootType rootType)
        {
            return rootType switch
            {
                GameSessionRootType.Player => PlayerRoot,
                GameSessionRootType.Weapon => WeaponRoot,
                GameSessionRootType.Enemy => EnemyRoot,
                GameSessionRootType.Drop => DropRoot,
                GameSessionRootType.WorldVfx => WorldVfxRoot,
                GameSessionRootType.WorldText => WorldTextRoot,
                _ => null
            };
        }

        public Transform GetOrCreateGroupRoot(GameSessionRootType rootType, string groupName)
        {
            var root = GetRoot(rootType);
            if (root == null)
                return null;

            if (string.IsNullOrWhiteSpace(groupName))
                return root;

            var child = root.Find(groupName);
            if (child != null)
                return child;

            var groupObject = new GameObject(groupName);
            var groupTransform = groupObject.transform;
            groupTransform.SetParent(root, false);
            return groupTransform;
        }

        public static GameSession Create(PlayerData player, List<WeaponEntry> weapons)
        {
            return new GameSession
            {
                SelectedPlayer = player,
                SelectedWeapons = weapons ?? new List<WeaponEntry>()
            };
        }

        #endregion
    }

    public enum GameSessionRootType
    {
        Player,
        Weapon,
        Enemy,
        Drop,
        WorldVfx,
        WorldText
    }
}
