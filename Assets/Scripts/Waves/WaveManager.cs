using System;
using System.Collections.Generic;
using Enemy;
using Events;
using Events.WaveEvents;
using ObjectPool;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Waves
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private List<WaveConfig> waves = new();
        [SerializeField] private float radius;
        
        private Transform _player;
        public EnemyManager EnemyManager;
        
        private float _spawnTimer;
        private float _timer;
        private int _lastSecond = -1;
        
        private PoolManager pool;
        
        public int CurrentWave { get; private set; }
        private bool _isActive;
        
        private readonly List<int> _randomEnemyIndex = new();
        
        public Action<bool> OnWaveCompleted;

        private void Update()
        {
            if (!_isActive)
                return;

            if (CurrentWave >= waves.Count)
            {
                _isActive = false;
                return;
            }

            WaveConfig wave = waves[CurrentWave];
            
            UpdateWaveTimer(wave);
            UpdateSpawner(wave);
        }

        private void UpdateWaveTimer(WaveConfig wave)
        {
            _timer += Time.deltaTime;
            
            float remaining = wave.duration - _timer;
            int currentSecond = Mathf.CeilToInt(remaining);
            if (currentSecond != _lastSecond)
            {
                _lastSecond = currentSecond;
                EventBus.Publish(new WaveChangeSecondEvent(currentSecond));
            }

            if (_timer < wave.duration)
                return;
            
            _isActive = false;

            bool isLastWave = CurrentWave >= waves.Count - 1;

            OnWaveCompleted?.Invoke(isLastWave);
        }
        
        private void UpdateSpawner(WaveConfig wave)
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= wave.spawnInterval)
            {
                _spawnTimer = 0;
                SpawnEnemy(wave);
            }
        }
        
        private void SpawnEnemy(WaveConfig wave)
        {
            Vector2 pos = (Vector2)_player.position +
                          Random.insideUnitCircle.normalized * radius;

            GameObject prefab = GetRandomEnemy(wave);
            
            var enemy = pool.Spawn(prefab, pos, Quaternion.identity);
            
            enemy.GetComponent<EnemyController>().Initialize(_player, EnemyManager);
        }

        private GameObject GetRandomEnemy(WaveConfig wave)
        {
            int index = _randomEnemyIndex[Random.Range(0, _randomEnemyIndex.Count)];
            return wave.enemies[index].enemyPrefab;
        }

        private void BuildRandomPool(int waveIndex)
        {
            _randomEnemyIndex.Clear();

            if (waveIndex >= waves.Count)
                return;

            var enemies = waves[waveIndex].enemies;

            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < enemies[i].weight; j++)
                    _randomEnemyIndex.Add(i);
            }
        }
        
        public void Initialize(PlayerManager playerManager)
        {   
            _player = playerManager.Player.transform;
            EnemyManager = new EnemyManager();
            CurrentWave = -1;
            
            pool = PoolManager.Instance;
        }

        public void StartNextWave()
        {
            CurrentWave++;
            _timer = 0;
            _spawnTimer = 0;
            _lastSecond = -1;
            BuildRandomPool(CurrentWave);
            _isActive = true;
        }

        public void ResumeWave()
        {
            if (CurrentWave < 0 || CurrentWave >= waves.Count)
                return;

            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
        }
    }
}
