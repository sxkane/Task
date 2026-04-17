using Core;
using Events;
using Stats;
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
            ApplyPassiveData(data);
            Player.RefillHealthToMax();

            EventBus.Publish(new OnPlayerSpawnedEvent(Player.transform));
        }

        public void ResetRun()
        {
            if (Player == null)
                return;

            Destroy(Player.gameObject);
            Player = null;
        }

        private void ApplyPassiveData(PlayerData data)
        {
            if (Player == null || data == null)
                return;

            var passiveData = data.GetPassiveData();
            if (passiveData == null)
                return;

            for (var i = 0; i < passiveData.Modifiers.Count; i++)
            {
                var modifier = passiveData.Modifiers[i];
                var stat = Player.Stats.GetStat(modifier.statType);
                stat.AddModifier(new Modifier(modifier.value, modifier.modifierType, passiveData));
            }
        }
    }
}
