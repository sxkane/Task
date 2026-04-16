using System.Collections.Generic;
using Enemy;
using Items;
using Player;
using Waves;
using Weapons;
using Weapons.Items;

namespace Data
{
    public static class GameDataValidator
    {
        public static List<GameDataValidationIssue> ValidateDatabase(GameDatabase database)
        {
            var issues = new List<GameDataValidationIssue>();

            if (database == null)
            {
                issues.Add(new GameDataValidationIssue("Database", "GameDatabase", "Database reference is null."));
                return issues;
            }

            ValidatePlayers(database.GetPlayerEntries(), issues);
            ValidateWeapons(database.GetWeaponEntries(), issues);
            ValidateItems(database.GetItemEntries(), issues);
            ValidateEnemiesInternal(database.GetEnemyEntries(), issues);
            ValidateWavesInternal(database.GetWaveEntries(), issues);
            return issues;
        }

        public static List<GameDataValidationIssue> ValidateEnemies(IEnumerable<EnemyStatTemplate> enemyTemplates)
        {
            var issues = new List<GameDataValidationIssue>();
            ValidateEnemiesInternal(enemyTemplates, issues);
            return issues;
        }

        public static List<GameDataValidationIssue> ValidateWaves(IEnumerable<WaveConfig> waves)
        {
            var issues = new List<GameDataValidationIssue>();
            ValidateWavesInternal(waves, issues);
            return issues;
        }

        private static void ValidatePlayers(IEnumerable<PlayerData> players, List<GameDataValidationIssue> issues)
        {
            var usedIds = new HashSet<int>();

            if (players == null)
            {
                issues.Add(new GameDataValidationIssue("Player", "Players", "Player list is null."));
                return;
            }

            foreach (var player in players)
            {
                if (player == null)
                {
                    issues.Add(new GameDataValidationIssue("Player", "Players", "Found null player entry."));
                    continue;
                }

                if (!player.IsValid())
                    issues.Add(new GameDataValidationIssue("Player", player.GetValidationSourceName(), "Missing required identity or player prefab."));

                if (!usedIds.Add(player.GetDataId()))
                    issues.Add(new GameDataValidationIssue("Player", player.GetValidationSourceName(), $"Duplicate playerID: {player.GetDataId()}."));

                foreach (var starterWeapon in player.GetStarterWeaponEntries())
                {
                    if (starterWeapon == null || !starterWeapon.IsValid())
                        issues.Add(new GameDataValidationIssue("Player", player.GetValidationSourceName(), "Contains invalid starter weapon entry."));
                }
            }
        }

        private static void ValidateWeapons(IEnumerable<WeaponData> weapons, List<GameDataValidationIssue> issues)
        {
            var usedIds = new HashSet<int>();

            if (weapons == null)
            {
                issues.Add(new GameDataValidationIssue("Weapon", "Weapons", "Weapon list is null."));
                return;
            }

            foreach (var weapon in weapons)
            {
                if (weapon == null)
                {
                    issues.Add(new GameDataValidationIssue("Weapon", "Weapons", "Found null weapon entry."));
                    continue;
                }

                if (!weapon.IsValid())
                    issues.Add(new GameDataValidationIssue("Weapon", weapon.GetValidationSourceName(), "Missing required identity or weapon prefab."));

                if (!usedIds.Add(weapon.GetDataId()))
                    issues.Add(new GameDataValidationIssue("Weapon", weapon.GetValidationSourceName(), $"Duplicate weaponID: {weapon.GetDataId()}."));

                if (weapon.rarityStats == null || weapon.rarityStats.Count == 0)
                    issues.Add(new GameDataValidationIssue("Weapon", weapon.GetValidationSourceName(), "No rarity stats configured."));
            }
        }

        private static void ValidateItems(IEnumerable<ItemData> items, List<GameDataValidationIssue> issues)
        {
            var usedIds = new HashSet<int>();

            if (items == null)
            {
                issues.Add(new GameDataValidationIssue("Item", "Items", "Item list is null."));
                return;
            }

            foreach (var item in items)
            {
                if (item == null)
                {
                    issues.Add(new GameDataValidationIssue("Item", "Items", "Found null item entry."));
                    continue;
                }

                if (!item.IsValid())
                    issues.Add(new GameDataValidationIssue("Item", item.GetValidationSourceName(), "Missing required identity."));

                if (!usedIds.Add(item.GetDataId()))
                    issues.Add(new GameDataValidationIssue("Item", item.GetValidationSourceName(), $"Duplicate itemID: {item.GetDataId()}."));
            }
        }

        private static void ValidateEnemiesInternal(IEnumerable<EnemyStatTemplate> enemyTemplates, List<GameDataValidationIssue> issues)
        {
            var usedIds = new HashSet<int>();

            if (enemyTemplates == null)
            {
                issues.Add(new GameDataValidationIssue("Enemy", "Enemies", "Enemy template list is null."));
                return;
            }

            foreach (var enemyTemplate in enemyTemplates)
            {
                if (enemyTemplate == null)
                {
                    issues.Add(new GameDataValidationIssue("Enemy", "Enemies", "Found null enemy template."));
                    continue;
                }

                if (!enemyTemplate.IsValid())
                    issues.Add(new GameDataValidationIssue("Enemy", enemyTemplate.GetValidationSourceName(), "Missing required identity."));

                if (!usedIds.Add(enemyTemplate.GetDataId()))
                    issues.Add(new GameDataValidationIssue("Enemy", enemyTemplate.GetValidationSourceName(), $"Duplicate enemyID: {enemyTemplate.GetDataId()}."));
            }
        }

        private static void ValidateWavesInternal(IEnumerable<WaveConfig> waves, List<GameDataValidationIssue> issues)
        {
            var usedIds = new HashSet<int>();

            if (waves == null)
            {
                issues.Add(new GameDataValidationIssue("Wave", "Waves", "Wave list is null."));
                return;
            }

            foreach (var wave in waves)
            {
                if (wave == null)
                {
                    issues.Add(new GameDataValidationIssue("Wave", "Waves", "Found null wave config."));
                    continue;
                }

                if (!wave.IsValid())
                    issues.Add(new GameDataValidationIssue("Wave", wave.GetValidationSourceName(), "Invalid identity, duration, or enemy pool configuration."));

                if (!usedIds.Add(wave.GetDataId()))
                    issues.Add(new GameDataValidationIssue("Wave", wave.GetValidationSourceName(), $"Duplicate waveID: {wave.GetDataId()}."));

                if (wave.GetDefaultPoolCount() <= 0)
                    issues.Add(new GameDataValidationIssue("Wave", wave.GetValidationSourceName(), "No enemy entries configured."));
            }
        }
    }
}
