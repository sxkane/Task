using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;
using Waves;
using Weapons;
using Weapons.Items;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Database/Game Database")]
    public class GameDatabase : ScriptableObject
    {
        [Header("Playable")]
        public List<PlayerData> players;
        public List<WeaponData> weapons;
        public List<ItemData> items;

        [Header("Battle Content")]
        public EnemyDatabase enemyDatabase;
        public WaveDatabase waveDatabase;

        public bool HasPlayableContent()
        {
            return players != null && players.Count > 0
                   && weapons != null && weapons.Count > 0
                   && items != null && items.Count > 0;
        }

        public bool HasBattleContent()
        {
            return enemyDatabase != null && enemyDatabase.HasContent()
                   && waveDatabase != null && waveDatabase.HasContent();
        }

        public List<PlayerData> GetPlayerEntries()
        {
            return players ?? new List<PlayerData>();
        }

        public List<WeaponData> GetWeaponEntries()
        {
            return weapons ?? new List<WeaponData>();
        }

        public List<ItemData> GetItemEntries()
        {
            return items ?? new List<ItemData>();
        }

        public List<EnemyStatTemplate> GetEnemyEntries()
        {
            return enemyDatabase != null ? enemyDatabase.GetEntries() : new List<EnemyStatTemplate>();
        }

        public List<WaveConfig> GetWaveEntries()
        {
            return waveDatabase != null ? waveDatabase.GetEntries() : new List<WaveConfig>();
        }

        public List<GameDataValidationIssue> ValidateContent()
        {
            return GameDataValidator.ValidateDatabase(this);
        }
    }
}
