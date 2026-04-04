using System;
using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        public PlayerController Player { get; private set; } 

        public void Initialize(PlayerData data)
        {
            var playerObj = Instantiate(
                data.playerPrefab,
                transform.position,
                transform.rotation);
            
            Player = playerObj.GetComponent<PlayerController>();
            Player.Initialize(data.playerStats);
            Player.transform.SetParent(parent);
            EventBus.Publish(new OnPlayerSpawnedEvent(Player.transform));
        }
    }
}