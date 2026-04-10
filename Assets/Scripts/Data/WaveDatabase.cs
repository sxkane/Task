using System.Collections.Generic;
using UnityEngine;
using Waves;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Database/Wave Database")]
    public class WaveDatabase : ScriptableObject
    {
        [Header("Wave Entries")]
        public List<WaveConfig> waves;

        public bool HasContent()
        {
            return waves != null && waves.Count > 0;
        }

        public List<WaveConfig> GetEntries()
        {
            return waves ?? new List<WaveConfig>();
        }

        public List<GameDataValidationIssue> ValidateContent()
        {
            return GameDataValidator.ValidateWaves(GetEntries());
        }
    }
}
