using UnityEngine;

namespace Enemy.Spawn
{
    public class EnemySpawnGroup : MonoBehaviour
    {
        [Min(1)] public int minCount = 3;
        [Min(1)] public int maxCount = 5;
        [Min(0f)] public float scatterRadius = 1.2f;

        public int GetSpawnCount()
        {
            var clampedMax = Mathf.Max(minCount, maxCount);
            return Random.Range(minCount, clampedMax + 1);
        }
    }
}
