using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerController Player { get; private set; } 

        public void Initialize(PlayerData data)
        {
            var playerObj = Instantiate(
                data.playerPrefab,
                transform.position,
                transform.rotation);
            
            Player = playerObj.GetComponent<PlayerController>();
            Player.Initialize(data.playerStats);
        }
    }
}