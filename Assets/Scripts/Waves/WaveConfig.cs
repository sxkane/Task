using System;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(menuName = "Game/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Identity")]
        public int waveID;
        public string waveName;

        [Header("Spawn")]
        public float spawnInterval = 1f;
        public int maxEnemiesAlive = 30;
        public float duration = 30f;

        [Header("Enemies")]
        public EnemyWeight[] enemies;

        public bool IsValid()
        {
            return waveID >= 0 && duration > 0f && spawnInterval > 0f;
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
            return $"{duration:0.#}s / {spawnInterval:0.##} spawn interval / {maxEnemiesAlive} max alive";
        }

        public string GetValidationSourceName()
        {
            return GetDisplayName();
        }
    }

    [Serializable]
    public class EnemyWeight
    {
        public GameObject enemyPrefab;
        public int weight = 1;
    }
}
