using Core;
using Enemy;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using Player;
using UnityEngine;
using System.Collections.Generic;

namespace Drops
{
    public class EnemyDropManager : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject dropPrefab;

        [Header("Spawn Count")]
        [SerializeField] private int minDropCount = 1;
        [SerializeField] private int maxDropCount = 3;
        [SerializeField] private int rewardPerExtraDrop = 20;
        [SerializeField] private float scatterRadius = 0.5f;

        private GameSession _session;
        private PlayerManager _playerManager;
        private Transform _dropRoot;
        private bool _initialize;
        private readonly List<EnemyDropItem> _activeDrops = new();

        public void Configure(GameSession session, PlayerManager playerManager)
        {
            _session = session;
            _playerManager = playerManager;
            _dropRoot = _session?.GetOrCreateGroupRoot(GameSessionRootType.Drop, "EnemyDrops");
        }

        public void InitializeRun()
        {
            _initialize = true;
            EventBus.Subscribe<OnEnemyDiedEvent>(OnEnemyDied);
        }

        public void ResetRun()
        {
            _initialize = false;
            EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);
            _activeDrops.Clear();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);
        }

        private void OnEnemyDied(OnEnemyDiedEvent e)
        {
            if (!_initialize || e.Target == null)
                return;

            var player = _playerManager != null ? _playerManager.Player : null;
            var runtimeData = player != null ? player.RuntimeData : null;
            if (player == null || runtimeData == null || e.Target.Stats == null)
                return;

            var coinAmount = Mathf.RoundToInt(e.Target.Stats.CoinReward);
            var expAmount = Mathf.RoundToInt(e.Target.Stats.ExpReward);
            if (coinAmount <= 0 && expAmount <= 0)
                return;

            if (dropPrefab == null)
            {
                runtimeData.AddCoins(coinAmount);
                runtimeData.AddExperience(expAmount);
                return;
            }

            SpawnDrops(player, e.Target.transform.position, coinAmount, expAmount);
        }

        private void SpawnDrops(PlayerController player, Vector2 position, int totalCoin, int totalExp)
        {
            var totalReward = totalCoin + totalExp;
            var desiredCount = rewardPerExtraDrop > 0
                ? Mathf.Clamp(Mathf.CeilToInt(totalReward / (float)rewardPerExtraDrop), minDropCount, maxDropCount)
                : minDropCount;
            var dropCount = Mathf.Max(1, desiredCount);

            for (var i = 0; i < dropCount; i++)
            {
                var coinShare = SplitValue(totalCoin, dropCount, i);
                var expShare = SplitValue(totalExp, dropCount, i);
                var spawnOffset = Random.insideUnitCircle * scatterRadius;
                var dropObject = PoolManager.Instance.Spawn(
                    dropPrefab,
                    position + spawnOffset,
                    dropPrefab.transform.rotation,
                    _dropRoot);
                var dropItem = dropObject.GetComponent<EnemyDropItem>();
                if (dropItem != null)
                {
                    dropItem.Initialize(player, coinShare, expShare);
                    _activeDrops.Add(dropItem);
                }
            }
        }

        public void AttractAllDropsToPlayer()
        {
            CleanupInactiveDrops();

            for (var i = 0; i < _activeDrops.Count; i++)
            {
                var drop = _activeDrops[i];
                if (drop != null && drop.gameObject.activeInHierarchy)
                    drop.ForceAttract();
            }
        }

        public bool HasActiveDrops()
        {
            CleanupInactiveDrops();
            return _activeDrops.Count > 0;
        }

        private static int SplitValue(int total, int count, int index)
        {
            if (count <= 0)
                return total;

            var baseValue = total / count;
            var remainder = total % count;
            return baseValue + (index < remainder ? 1 : 0);
        }

        private void CleanupInactiveDrops()
        {
            for (var i = _activeDrops.Count - 1; i >= 0; i--)
            {
                var drop = _activeDrops[i];
                if (drop == null || !drop.gameObject.activeInHierarchy)
                    _activeDrops.RemoveAt(i);
            }
        }
    }
}
