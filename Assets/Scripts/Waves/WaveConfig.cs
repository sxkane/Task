using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(menuName = "Game/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Identity")]
        public int waveID;
        public string waveName;

        [Header("Timeline")]
        public float duration = 60f;
        public AnimationCurve spawnRateCurve = AnimationCurve.Linear(0f, 1f, 1f, 2f);
        public int maxEnemiesAlive = 30;

        [Header("Default Pool")]
        public List<EnemySpawnEntry> enemyPool = new();

        [Header("Spawn Windows")]
        public List<WaveSpawnWindow> spawnWindows = new();

        [Header("Boss")]
        public BossSpawnConfig boss;

        public bool IsValid()
        {
            return waveID >= 0 && duration > 0f && GetDefaultPoolCount() > 0;
        }

        public int GetDataId()
        {
            return waveID;
        }

        public string GetDisplayName()
        {
            return string.IsNullOrWhiteSpace(waveName) ? $"Wave {waveID}" : waveName;
        }

        public string GetSummary()
        {
            return $"{duration:0.#}s / curve-driven spawn / {maxEnemiesAlive} max alive";
        }

        public string GetValidationSourceName()
        {
            return GetDisplayName();
        }

        public float EvaluateSpawnRate(float normalizedTime)
        {
            var clampedTime = Mathf.Clamp01(normalizedTime);
            var baseRate = spawnRateCurve != null ? Mathf.Max(0f, spawnRateCurve.Evaluate(clampedTime)) : 0f;
            var currentWindow = GetCurrentWindow(clampedTime);
            if (currentWindow == null)
                return baseRate;

            var windowTime = currentWindow.GetLocalNormalizedTime(clampedTime);
            var multiplier = currentWindow.spawnRateMultiplierCurve != null
                ? Mathf.Max(0f, currentWindow.spawnRateMultiplierCurve.Evaluate(windowTime))
                : 1f;

            return baseRate * multiplier;
        }

        public int EvaluateMaxEnemiesAlive(float normalizedTime)
        {
            var currentWindow = GetCurrentWindow(normalizedTime);
            if (currentWindow == null || currentWindow.maxEnemiesAliveOverride <= 0)
                return maxEnemiesAlive;

            return currentWindow.maxEnemiesAliveOverride;
        }

        public IReadOnlyList<EnemySpawnEntry> GetCurrentEnemyPool(float normalizedTime)
        {
            var currentWindow = GetCurrentWindow(normalizedTime);
            if (currentWindow != null && currentWindow.enemyPool != null && currentWindow.enemyPool.Count > 0)
                return currentWindow.enemyPool;

            return enemyPool;
        }

        public int GetDefaultPoolCount()
        {
            var count = 0;

            if (enemyPool != null)
            {
                for (var i = 0; i < enemyPool.Count; i++)
                {
                    if (enemyPool[i] != null && enemyPool[i].enemyPrefab != null && enemyPool[i].weight > 0)
                        count++;
                }
            }

            if (count > 0)
                return count;

            if (spawnWindows == null)
                return 0;

            for (var i = 0; i < spawnWindows.Count; i++)
            {
                var window = spawnWindows[i];
                if (window == null || window.enemyPool == null)
                    continue;

                for (var j = 0; j < window.enemyPool.Count; j++)
                {
                    if (window.enemyPool[j] != null && window.enemyPool[j].enemyPrefab != null && window.enemyPool[j].weight > 0)
                        return 1;
                }
            }

            return 0;
        }

        public bool ShouldSpawnBoss(float normalizedTime, bool bossAlreadySpawned)
        {
            return !bossAlreadySpawned
                   && boss != null
                   && boss.enabled
                   && boss.bossPrefab != null
                   && normalizedTime >= boss.spawnAtNormalizedTime;
        }

        private WaveSpawnWindow GetCurrentWindow(float normalizedTime)
        {
            if (spawnWindows == null)
                return null;

            var clampedTime = Mathf.Clamp01(normalizedTime);
            for (var i = 0; i < spawnWindows.Count; i++)
            {
                var window = spawnWindows[i];
                if (window != null && window.Contains(clampedTime))
                    return window;
            }

            return null;
        }
    }

    [Serializable]
    public class EnemySpawnEntry
    {
        public GameObject enemyPrefab;
        public int weight = 1;
    }

    [Serializable]
    public class WaveSpawnWindow
    {
        public string name;
        [Range(0f, 1f)] public float startNormalizedTime;
        [Range(0f, 1f)] public float endNormalizedTime = 1f;
        public AnimationCurve spawnRateMultiplierCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public int maxEnemiesAliveOverride = -1;
        public List<EnemySpawnEntry> enemyPool = new();

        public bool Contains(float normalizedTime)
        {
            return normalizedTime >= startNormalizedTime && normalizedTime <= endNormalizedTime;
        }

        public float GetLocalNormalizedTime(float normalizedTime)
        {
            var length = Mathf.Max(0.0001f, endNormalizedTime - startNormalizedTime);
            return Mathf.Clamp01((normalizedTime - startNormalizedTime) / length);
        }
    }

    [Serializable]
    public class BossSpawnConfig
    {
        public bool enabled;
        [Range(0f, 1f)] public float spawnAtNormalizedTime = 0.9f;
        public GameObject bossPrefab;
        public int spawnCount = 1;
    }
}
