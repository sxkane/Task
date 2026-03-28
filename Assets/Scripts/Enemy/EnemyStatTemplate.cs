using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Game/Enemy Template")]
    public class EnemyStatTemplate : ScriptableObject
    {
        public float maxHP = 10;
        public float moveSpeed = 3;
        public float damage = 2;
        public float attackInterval = 1.2f;

        public float knockbackResistance = 0;
        public float coinReward = 1;
    }
}