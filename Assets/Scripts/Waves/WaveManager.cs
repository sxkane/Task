using System;
using System.Collections.Generic;
using Core;
using Data;
using Enemy;
using Enemy.Movement;
using Enemy.Spawn;
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
        #region Inspector

        [Header("Wave Settings")]
        [SerializeField] private Vector2 spawnMin = new(-17f, -10f);
        [SerializeField] private Vector2 spawnMax = new(17f, 8f);
        [SerializeField] private GameObject spawnTelegraphPrefab;
        [SerializeField] private WaveDatabase waveDatabase;
        private List<WaveConfig> _waves = new();

        #endregion

        #region Runtime

        private GameSession _session;
        private PlayerManager _playerManager;
        private PoolManager _poolManager;
        private float _spawnProgress;
        private float _timer;
        private int _lastSecond = -1;
        private bool _isActive;
        private bool _isCompleting;
        private bool _bossSpawned;
        private int _spawnGeneration;
        private readonly List<int> _spawnWeightBuffer = new();

        public EnemyManager EnemyManager { get; private set; }
        public int CurrentWave { get; private set; }
        public bool IsFinalWave => CurrentWave >= _waves.Count - 1;
        public bool BossSpawned => _bossSpawned;
        public Action<bool> OnWaveCompleted;

        #endregion

        private void Update()
        {
            if (!_isActive)
                return;

            if (CurrentWave < 0 || CurrentWave >= _waves.Count)
            {
                _isActive = false;
                return;
            }

            var wave = _waves[CurrentWave];
            UpdateWaveTimer(wave);
            if (!_isActive)
                return;

            UpdateBossSpawn(wave);
            UpdateSpawner(wave);
        }

        public void Configure(GameSession session, PlayerManager playerManager)
        {
            _session = session;
            _playerManager = playerManager;
            _poolManager = PoolManager.Instance;
            EnemyWorldBounds.Configure(spawnMin, spawnMax);
            
            _waves = new List<WaveConfig>();
            foreach (var wave in waveDatabase.waves)
            {
                _waves.Add(wave);
            }
        }

        public void InitializeRun()
        {
            EnemyManager = new EnemyManager();
            _poolManager?.Configure(_session);
            CurrentWave = -1;
            _timer = 0f;
            _spawnProgress = 0f;
            _lastSecond = -1;
            _isActive = false;
            _bossSpawned = false;
            _isCompleting = false;
            _spawnGeneration = 0;
            _spawnWeightBuffer.Clear();
        }

        public void ResetRun()
        {
            EndPhase();
            EnemyManager?.ClearAllEnemies();
            EnemyManager = null;
            _poolManager?.ResetRun();
            CurrentWave = -1;
            _timer = 0f;
            _spawnProgress = 0f;
            _lastSecond = -1;
            _bossSpawned = false;
            _isCompleting = false;
            _spawnGeneration++;
            _spawnWeightBuffer.Clear();
        }

        public void BeginPhase()
        {
            StartNextWave();
        }

        public void EndPhase()
        {
            _isActive = false;
            _isCompleting = false;
            _spawnGeneration++;
        }

        public void CompletePhase()
        {
            _isActive = false;
            _isCompleting = true;
            _spawnGeneration++;
        }

        public void ResumePhase()
        {
            if (CurrentWave < 0 || CurrentWave >= _waves.Count)
                return;

            _isActive = true;
        }

        private void StartNextWave()
        {
            CurrentWave++;
            _timer = 0f;
            _spawnProgress = 0f;
            _lastSecond = -1;
            _bossSpawned = false;
            _isCompleting = false;
            _spawnGeneration++;
            _isActive = true;
        }

        private void UpdateWaveTimer(WaveConfig wave)
        {
            _timer += Time.deltaTime;

            var remaining = Mathf.Max(0f, wave.duration - _timer);
            var currentSecond = Mathf.CeilToInt(remaining);
            if (currentSecond != _lastSecond)
            {
                _lastSecond = currentSecond;
                EventBus.Publish(new WaveChangeSecondEvent(currentSecond));
            }

            if (_timer < wave.duration)
                return;

            _isActive = false;
            var isLastWave = CurrentWave >= _waves.Count - 1;
            OnWaveCompleted?.Invoke(isLastWave);
        }

        private void UpdateSpawner(WaveConfig wave)
        {
            if (_isCompleting)
                return;

            if (wave.duration - _timer <= 1f)
                return;

            var normalizedTime = GetNormalizedWaveTime(wave);
            var spawnRate = wave.EvaluateSpawnRate(normalizedTime);
            if (spawnRate <= 0f)
                return;

            var maxEnemiesAlive = wave.EvaluateMaxEnemiesAlive(normalizedTime);
            if (EnemyManager != null && EnemyManager.AliveEnemyCount >= maxEnemiesAlive)
                return;

            _spawnProgress += spawnRate * Time.deltaTime;

            while (_spawnProgress >= 1f)
            {
                if (EnemyManager != null && EnemyManager.AliveEnemyCount >= maxEnemiesAlive)
                    break;

                if (!SpawnEnemyFromPool(wave, normalizedTime))
                    break;

                _spawnProgress -= 1f;
            }
        }

        private void UpdateBossSpawn(WaveConfig wave)
        {
            if (_isCompleting)
                return;

            var normalizedTime = GetNormalizedWaveTime(wave);
            if (!wave.ShouldSpawnBoss(normalizedTime, _bossSpawned))
                return;

            if (wave.boss == null || wave.boss.bossPrefab == null)
                return;

            var bossCount = Mathf.Max(1, wave.boss.spawnCount);
            for (var i = 0; i < bossCount; i++)
                SpawnEnemy(wave.boss.bossPrefab, $"{wave.boss.bossPrefab.name}_Boss");

            _bossSpawned = true;
        }

        private bool SpawnEnemyFromPool(WaveConfig wave, float normalizedTime)
        {
            var currentPool = wave.GetCurrentEnemyPool(normalizedTime);
            if (currentPool == null || currentPool.Count == 0)
                return false;

            _spawnWeightBuffer.Clear();
            for (var i = 0; i < currentPool.Count; i++)
            {
                var entry = currentPool[i];
                if (entry == null || entry.enemyPrefab == null || entry.weight <= 0)
                    continue;

                for (var count = 0; count < entry.weight; count++)
                    _spawnWeightBuffer.Add(i);
            }

            if (_spawnWeightBuffer.Count == 0)
                return false;

            var randomIndex = _spawnWeightBuffer[Random.Range(0, _spawnWeightBuffer.Count)];
            var prefab = currentPool[randomIndex].enemyPrefab;
            SpawnEnemy(prefab, prefab.name);
            return true;
        }

        private void SpawnEnemy(GameObject prefab, string groupName)
        {
            if (prefab == null || _poolManager == null)
                return;

            var groupRoot = _session?.GetOrCreateGroupRoot(GameSessionRootType.Enemy, groupName);
            var groupConfig = prefab.GetComponent<EnemySpawnGroup>();

            if (groupConfig != null && groupConfig.enabled)
            {
                var anchor = GetRandomSpawnPosition();
                var spawnCount = groupConfig.GetSpawnCount();
                for (var i = 0; i < spawnCount; i++)
                {
                    var position = i == 0
                        ? anchor
                        : ClampToSpawnBounds(anchor + Random.insideUnitCircle * groupConfig.scatterRadius);
                    SpawnEnemyWithTelegraph(prefab, position, groupRoot);
                }

                return;
            }

            SpawnEnemyWithTelegraph(prefab, GetRandomSpawnPosition(), groupRoot);
        }

        private void SpawnEnemyWithTelegraph(GameObject prefab, Vector2 position, Transform groupRoot)
        {
            var player = _playerManager != null ? _playerManager.Player : null;
            if (player == null || prefab == null || _poolManager == null)
                return;

            var generation = _spawnGeneration;

            if (spawnTelegraphPrefab != null)
            {
                var telegraphObject = _poolManager.Spawn(
                    spawnTelegraphPrefab,
                    position,
                    Quaternion.identity,
                    groupRoot);
                var telegraph = telegraphObject.GetComponent<SpawnTelegraph>();
                if (telegraph != null)
                {
                    telegraph.Play(() =>
                    {
                        if (_isCompleting || generation != _spawnGeneration)
                            return;

                        SpawnEnemyInternal(prefab, position, groupRoot, player.transform);
                    });
                    return;
                }
            }

            if (_isCompleting || generation != _spawnGeneration)
                return;

            SpawnEnemyInternal(prefab, position, groupRoot, player.transform);
        }

        private void SpawnEnemyInternal(GameObject prefab, Vector2 position, Transform groupRoot, Transform playerTransform)
        {
            var enemyObject = _poolManager.Spawn(prefab, position, prefab.transform.rotation, groupRoot);
            var enemyController = enemyObject.GetComponent<EnemyController>();
            if (enemyController == null)
            {
                Debug.LogError($"Spawned enemy prefab '{prefab.name}' is missing EnemyController.");
                return;
            }

            enemyController.Initialize(playerTransform, EnemyManager, CurrentWave + 1);
        }

        private Vector2 GetRandomSpawnPosition()
        {
            return new Vector2(
                Random.Range(spawnMin.x, spawnMax.x),
                Random.Range(spawnMin.y, spawnMax.y));
        }

        private Vector2 ClampToSpawnBounds(Vector2 position)
        {
            position.x = Mathf.Clamp(position.x, spawnMin.x, spawnMax.x);
            position.y = Mathf.Clamp(position.y, spawnMin.y, spawnMax.y);
            return position;
        }

        private float GetNormalizedWaveTime(WaveConfig wave)
        {
            return wave == null || wave.duration <= 0f
                ? 0f
                : Mathf.Clamp01(_timer / wave.duration);
        }
    }
}
