using System;
using Stats;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class EnemyStats
    {
        private readonly Stat _maxHpStat = new(0f);
        private readonly Stat _moveSpeedStat = new(0f);
        private readonly Stat _damageStat = new(0f);
        private readonly Stat _attackIntervalStat = new(1f);
        private readonly Stat _knockbackResistanceStat = new(0f);
        private readonly Stat _coinRewardStat = new(0f);
        private readonly Stat _expRewardStat = new(0f);

        private readonly object _baseMultiplierSource = new();
        private readonly object _supportAuraSource = new();

        public float MaxHP => _maxHpStat.Value;
        public float CurrentHP { get; private set; }
        public float MoveSpeed => _moveSpeedStat.Value;
        public float Damage => _damageStat.Value;
        public float AttackInterval => _attackIntervalStat.Value;
        public float KnockbackResistance => _knockbackResistanceStat.Value;
        public float CoinReward => _coinRewardStat.Value;
        public float ExpReward => _expRewardStat.Value;
        public bool IsAlive => CurrentHP > 0;

        public void Initialize(EnemyStatTemplate template, int currentWave)
        {
            var waveNumber = Mathf.Max(1, currentWave);

            _maxHpStat.BaseValue = template.maxHP + (waveNumber - 1) * template.hpPerWave;
            _moveSpeedStat.BaseValue = template.moveSpeed;
            _damageStat.BaseValue = template.damage + (waveNumber - 1) * template.damagePerWave;
            _attackIntervalStat.BaseValue = template.attackInterval;
            _knockbackResistanceStat.BaseValue = template.knockbackResistance;
            _coinRewardStat.BaseValue = template.coinReward;
            _expRewardStat.BaseValue = template.expReward;

            _maxHpStat.ClearModifiers();
            _moveSpeedStat.ClearModifiers();
            _damageStat.ClearModifiers();
            _attackIntervalStat.ClearModifiers();
            _knockbackResistanceStat.ClearModifiers();
            _coinRewardStat.ClearModifiers();
            _expRewardStat.ClearModifiers();

            CurrentHP = MaxHP;
        }

        public void SetMultipliers(
            float maxHpMultiplier = 1f,
            float moveSpeedMultiplier = 1f,
            float damageMultiplier = 1f,
            float attackIntervalMultiplier = 1f,
            bool preserveHealthPercent = true)
        {
            var healthPercent = MaxHP <= 0f ? 1f : CurrentHP / MaxHP;

            RemoveModifiersFromSource(_baseMultiplierSource);
            AddMultiplier(_maxHpStat, maxHpMultiplier, _baseMultiplierSource);
            AddMultiplier(_moveSpeedStat, moveSpeedMultiplier, _baseMultiplierSource);
            AddMultiplier(_damageStat, damageMultiplier, _baseMultiplierSource);
            AddMultiplier(_attackIntervalStat, attackIntervalMultiplier, _baseMultiplierSource);

            if (preserveHealthPercent)
                CurrentHP = MaxHP * healthPercent;
            else
                CurrentHP = Mathf.Min(CurrentHP, MaxHP);
        }

        public void SetSupportAuraMultipliers(
            float maxHpMultiplier = 1f,
            float moveSpeedMultiplier = 1f,
            float damageMultiplier = 1f,
            float attackIntervalMultiplier = 1f,
            bool preserveHealthPercent = true)
        {
            var healthPercent = MaxHP <= 0f ? 1f : CurrentHP / MaxHP;

            RemoveModifiersFromSource(_supportAuraSource);
            AddMultiplier(_maxHpStat, maxHpMultiplier, _supportAuraSource);
            AddMultiplier(_moveSpeedStat, moveSpeedMultiplier, _supportAuraSource);
            AddMultiplier(_damageStat, damageMultiplier, _supportAuraSource);
            AddMultiplier(_attackIntervalStat, attackIntervalMultiplier, _supportAuraSource);

            if (preserveHealthPercent)
                CurrentHP = MaxHP * healthPercent;
            else
                CurrentHP = Mathf.Min(CurrentHP, MaxHP);
        }

        public void Multiply(
            float maxHpMultiplier = 1f,
            float moveSpeedMultiplier = 1f,
            float damageMultiplier = 1f,
            float attackIntervalMultiplier = 1f,
            bool preserveHealthPercent = true)
        {
            SetMultipliers(
                maxHpMultiplier,
                moveSpeedMultiplier,
                damageMultiplier,
                attackIntervalMultiplier,
                preserveHealthPercent);
        }

        public void TakeDamage(float amount)
        {
            CurrentHP = Mathf.Max(0f, CurrentHP - amount);
        }

        public void Heal(float amount)
        {
            CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        }

        public Stat GetStat(EnemyStatType statType)
        {
            return statType switch
            {
                EnemyStatType.MaxHP => _maxHpStat,
                EnemyStatType.MoveSpeed => _moveSpeedStat,
                EnemyStatType.Damage => _damageStat,
                EnemyStatType.AttackInterval => _attackIntervalStat,
                EnemyStatType.KnockbackResistance => _knockbackResistanceStat,
                EnemyStatType.CoinReward => _coinRewardStat,
                EnemyStatType.ExpReward => _expRewardStat,
                _ => throw new ArgumentOutOfRangeException(nameof(statType), statType, null)
            };
        }

        public void RemoveModifiersFromSource(object source)
        {
            _maxHpStat.RemoveModifiersFromSource(source);
            _moveSpeedStat.RemoveModifiersFromSource(source);
            _damageStat.RemoveModifiersFromSource(source);
            _attackIntervalStat.RemoveModifiersFromSource(source);
            _knockbackResistanceStat.RemoveModifiersFromSource(source);
            _coinRewardStat.RemoveModifiersFromSource(source);
            _expRewardStat.RemoveModifiersFromSource(source);
        }

        private static void AddMultiplier(Stat stat, float multiplier, object source)
        {
            var clampedMultiplier = Mathf.Max(0f, multiplier);
            if (Mathf.Approximately(clampedMultiplier, 1f))
                return;

            stat.AddModifier(new Modifier(clampedMultiplier - 1f, StatModType.PercentMult, source));
        }
    }
}
