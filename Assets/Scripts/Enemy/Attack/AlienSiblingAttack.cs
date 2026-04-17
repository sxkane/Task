using Events;
using Events.EnemyEvents;
using ObjectPool;
using UnityEngine;
using Core;

namespace Enemy.Attack
{
    public class AlienSiblingAttack : AlienAttack
    {
        [SerializeField] private GameObject alienPrefab;
        [SerializeField] private int spawnCount = 2;
        [SerializeField] private float spawnScatterRadius = 0.6f;

        private bool _subscribed;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            EnsureSubscribed();
        }

        private void OnEnable()
        {
            EnsureSubscribed();
        }

        private void OnDisable()
        {
            if (_subscribed)
            {
                EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);
                _subscribed = false;
            }
        }

        private void EnsureSubscribed()
        {
            if (_subscribed)
                return;

            EventBus.Subscribe<OnEnemyDiedEvent>(OnEnemyDied);
            _subscribed = true;
        }

        private void OnEnemyDied(OnEnemyDiedEvent e)
        {
            if (Enemy == null || e.Target != Enemy || alienPrefab == null || Enemy.Target == null || Enemy.Context?.EnemyManager == null)
                return;

            if (GameController.Instance != null && GameController.Instance.IsWaveCompleting)
                return;

            var parent = Enemy.Transform.parent;
            for (var i = 0; i < spawnCount; i++)
            {
                var offset = Random.insideUnitCircle * spawnScatterRadius;
                var spawnPosition = (Vector2)Enemy.Transform.position + offset;
                var alienObject = PoolManager.Instance.Spawn(alienPrefab, spawnPosition, alienPrefab.transform.rotation, parent);
                var alienController = alienObject.GetComponent<EnemyController>();
                if (alienController != null)
                    alienController.Initialize(Enemy.Target, Enemy.Context.EnemyManager, Enemy.CurrentWave);
            }
        }
    }
}
