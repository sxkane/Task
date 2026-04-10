using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Database/Enemy Database")]
    public class EnemyDatabase : ScriptableObject
    {
        [Header("Enemy Entries")]
        public List<EnemyStatTemplate> enemies;

        public bool HasContent()
        {
            return enemies != null && enemies.Count > 0;
        }

        public List<EnemyStatTemplate> GetEntries()
        {
            return enemies ?? new List<EnemyStatTemplate>();
        }

        public List<GameDataValidationIssue> ValidateContent()
        {
            return GameDataValidator.ValidateEnemies(GetEntries());
        }
    }
}
