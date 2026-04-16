using Data;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Game/Enemy Template")]
    public class EnemyStatTemplate : GameDataAsset
    {
        [Header("Identity")]
        public int enemyID;
        public string enemyName;

        [Header("Combat")]
        public float maxHP = 10;
        public float hpPerWave = 2;
        public float moveSpeed = 3;
        public float damage = 2;
        public float damagePerWave = 1;
        public float attackInterval = 1.2f;

        [Header("Defense")]
        public float knockbackResistance = 0;

        [Header("Rewards")]
        public float coinReward = 1;
        public float expReward = 1;

        public override int DataId => enemyID;
        public override string DisplayName => enemyName;
        public override Sprite Icon => null;
        public override string Summary => GetSummary();
        public override string ValidationSourceName => GetValidationSourceName();

        public override bool IsValid()
        {
            return enemyID >= 0 && !string.IsNullOrWhiteSpace(enemyName);
        }

        public int GetDataId()
        {
            return enemyID;
        }

        public string GetDisplayName()
        {
            return enemyName;
        }

        public string GetSummary()
        {
            return string.Empty;
        }

        public string GetValidationSourceName()
        {
            return string.IsNullOrWhiteSpace(enemyName) ? name : enemyName;
        }
    }
}
