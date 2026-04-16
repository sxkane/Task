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
        public float ExpReward { get; private set; }

        public bool IsAlive => CurrentHP > 0;

        public void Initialize(EnemyStatTemplate template, int currentWave)
        {
            MaxHP = template.maxHP + (currentWave - 1) * template.hpPerWave;
            CurrentHP = MaxHP;

            MoveSpeed = template.moveSpeed;
            Damage = template.damage + (currentWave - 1) *  template.damagePerWave;
            AttackInterval = template.attackInterval;
            KnockbackResistance = template.knockbackResistance;
            CoinReward = template.coinReward;
            ExpReward = template.expReward;
        }

        public void TakeDamage(float amount)
        {
            CurrentHP -= amount;
        }
    }
}
