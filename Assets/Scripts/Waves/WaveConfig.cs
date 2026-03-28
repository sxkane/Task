using System;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(menuName = "Game/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Spawn")]
        public float spawnInterval = 1f;
        public int maxEnemiesAlive = 30;
        public float duration = 30f;

        [Header("Enemies")]
        public EnemyWeight[] enemies;
    }

    [Serializable]
    public class EnemyWeight
    {
        public GameObject enemyPrefab;
        public int weight = 1;
    }
}