using System;

namespace Enemy
{
    [Serializable]
    public class EnemyStats
    {
        public float MaxHP { get; private set; }
        public float CurrentHP { get; private set; }

        public float MoveSpeed { get; private set; }
        public float Damage { get; private set; }
        public float AttackInterval { get; private set; }

        public float KnockbackResistance { get; private set; }
        public float CoinReward { get; private set; }

        public bool IsAlive => CurrentHP > 0;

        public void Init(EnemyStatTemplate template)
        {
            MaxHP = template.maxHP;
            CurrentHP = MaxHP;

            MoveSpeed = template.moveSpeed;
            Damage = template.damage;
            AttackInterval = template.attackInterval;
            KnockbackResistance = template.knockbackResistance;
            CoinReward = template.coinReward;
        }

        public void TakeDamage(float amount)
        {
            CurrentHP -= amount;
        }
    }
}