using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager
    {
        private readonly List<EnemyController> _enemies = new();

        public void Register(EnemyController enemy)
        {
            if (!_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        public void Unregister(EnemyController enemy)
        {
            _enemies.Remove(enemy);
        }

        public EnemyController GetNearestEnemy(Vector2 pos)
        {
            EnemyController target = null;
            float distance = float.MaxValue;

            foreach (var e in _enemies)
            {
                if (e == null) continue;

                float dist = (pos - (Vector2)e.transform.position).sqrMagnitude;
                if (dist < distance)
                {
                    target = e;
                    distance = dist;
                }
            }

            return target;
        }
        
        public EnemyController GetRandomEnemy()
        {
            if (_enemies.Count == 0) return null;
            return _enemies[Random.Range(0, _enemies.Count)];
        }

        public void ClearAllEnemies()
        {
            foreach (var e in _enemies)
            {
                e.TakeDamage(100000);
            }
        }
    }
}