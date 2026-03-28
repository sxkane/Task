using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    [CreateAssetMenu(menuName = "Game/player")]
    public class PlayerData : ScriptableObject
    {
        public int playerID;
        public string playerName;
        public Image playerIcon;
        public PlayerStats playerStats;
        public GameObject playerPrefab;
    }
}