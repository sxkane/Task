using Core;
using Events;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        #region Inspector

        [Header("Fallback Root")]
        [SerializeField] private Transform fallbackParent;

        #endregion

        #region Runtime

        public PlayerController Player { get; private set; }

        private GameSession _session;

        #endregion

        public void Configure(GameSession session)
        {
            _session = session;
        }

        public void InitializeRun(PlayerData data)
        {
            ResetRun();

            if (data == null || data.playerPrefab == null)
                return;

            var playerRoot = _session?.PlayerRoot != null ? _session.PlayerRoot : fallbackParent;
            var playerObject = Instantiate(data.playerPrefab, transform.position, transform.rotation, playerRoot);
            Player = playerObject.GetComponent<PlayerController>();
            Player.Initialize(data.playerStats);

            EventBus.Publish(new OnPlayerSpawnedEvent(Player.transform));
        }

        public void ResetRun()
        {
            if (Player == null)
                return;

            Destroy(Player.gameObject);
            Player = null;
        }
    }
}
