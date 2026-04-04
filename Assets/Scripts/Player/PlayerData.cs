using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player
{
    [CreateAssetMenu(menuName = "Game/player")]
    public class PlayerData : ScriptableObject
    {
        public int playerID;
        public string playerName;
        public Sprite playerIcon;
        public PlayerStats playerStats;
        public GameObject playerPrefab;
        public List<WeaponData> initialWeapons;
    }
}