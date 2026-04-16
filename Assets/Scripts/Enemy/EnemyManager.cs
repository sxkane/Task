using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager
    {
        private readonly List<EnemyController> _enemies = new();

        public int AliveEnemyCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < _enemies.Count; i++)
                {
                    var enemy = _enemies[i];
                    if (enemy != null && enemy.gameObject.activeInHierarchy && enemy.Stats != null && enemy.Stats.IsAlive)
                        count++;
                }

                return count;
            }
        }

        public void Register(EnemyController enemy)
        {
            if (enemy != null && !_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        public void Unregister(EnemyController enemy)
        {
            _enemies.Remove(enemy);
        }

        public EnemyController GetNearestEnemy(Vector2 position)
        {
            EnemyController target = null;
            var distance = float.MaxValue;

            foreach (var enemy in _enemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.Stats == null || !enemy.Stats.IsAlive)
                    continue;

                var currentDistance = (position - (Vector2)enemy.transform.position).sqrMagnitude;
                if (currentDistance < distance)
                {
                    target = enemy;
                    distance = currentDistance;
                }
            }

            return target;
        }

        public EnemyController GetRandomEnemy()
        {
            if (_enemies.Count == 0)
                return null;

            return _enemies[Random.Range(0, _enemies.Count)];
        }

        public void GetEnemiesInRadius(Vector2 center, float radius, List<EnemyController> results)
        {
            if (results == null)
                return;

            results.Clear();
            var sqrRadius = radius * radius;

            foreach (var enemy in _enemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.Stats == null || !enemy.Stats.IsAlive)
                    continue;

                var sqrDist = ((Vector2)enemy.transform.position - center).sqrMagnitude;
                if (sqrDist <= sqrRadius)
                    results.Add(enemy);
            }
        }

        public void ClearAllEnemies()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy && enemy.Stats != null && enemy.Stats.IsAlive)
                    enemy.TakeDamage(100000f);
            }
        }
    }
}
