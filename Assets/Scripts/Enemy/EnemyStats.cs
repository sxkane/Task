using System;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class EnemyStats
    {
        private float _baseMaxHp;
        private float _baseMoveSpeed;
        private float _baseDamage;
        private float _baseAttackInterval;
        private float _baseKnockbackResistance;
        private float _baseCoinReward;
        private float _baseExpReward;

        private float _maxHpMultiplier = 1f;
        private float _moveSpeedMultiplier = 1f;
        private float _damageMultiplier = 1f;
        private float _attackIntervalMultiplier = 1f;
        private float _supportMaxHpMultiplier = 1f;
        private float _supportMoveSpeedMultiplier = 1f;
        private float _supportDamageMultiplier = 1f;
        private float _supportAttackIntervalMultiplier = 1f;

        public float MaxHP => _baseMaxHp * _maxHpMultiplier * _supportMaxHpMultiplier;
        public float CurrentHP { get; private set; }
        public float MoveSpeed => _baseMoveSpeed * _moveSpeedMultiplier * _supportMoveSpeedMultiplier;
        public float Damage => _baseDamage * _damageMultiplier * _supportDamageMultiplier;
        public float AttackInterval => _baseAttackInterval * _attackIntervalMultiplier * _supportAttackIntervalMultiplier;
        public float KnockbackResistance => _baseKnockbackResistance;
        public float CoinReward => _baseCoinReward;
        public float ExpReward => _baseExpReward;
        public bool IsAlive => CurrentHP > 0;

        public void Initialize(EnemyStatTemplate template, int currentWave)
        {
            var waveNumber = Mathf.Max(1, currentWave);

            _baseMaxHp = template.maxHP + (waveNumber - 1) * template.hpPerWave;
            _baseMoveSpeed = template.moveSpeed;
            _baseDamage = template.damage + (waveNumber - 1) * template.damagePerWave;
            _baseAttackInterval = template.attackInterval;
            _baseKnockbackResistance = template.knockbackResistance;
            _baseCoinReward = template.coinReward;
            _baseExpReward = template.expReward;

            _maxHpMultiplier = 1f;
            _moveSpeedMultiplier = 1f;
            _damageMultiplier = 1f;
            _attackIntervalMultiplier = 1f;
            _supportMaxHpMultiplier = 1f;
            _supportMoveSpeedMultiplier = 1f;
            _supportDamageMultiplier = 1f;
            _supportAttackIntervalMultiplier = 1f;

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

            _maxHpMultiplier = Mathf.Max(0f, maxHpMultiplier);
            _moveSpeedMultiplier = Mathf.Max(0f, moveSpeedMultiplier);
            _damageMultiplier = Mathf.Max(0f, damageMultiplier);
            _attackIntervalMultiplier = Mathf.Max(0.05f, attackIntervalMultiplier);

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

            _supportMaxHpMultiplier = Mathf.Max(0f, maxHpMultiplier);
            _supportMoveSpeedMultiplier = Mathf.Max(0f, moveSpeedMultiplier);
            _supportDamageMultiplier = Mathf.Max(0f, damageMultiplier);
            _supportAttackIntervalMultiplier = Mathf.Max(0.05f, attackIntervalMultiplier);

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
                _maxHpMultiplier * maxHpMultiplier,
                _moveSpeedMultiplier * moveSpeedMultiplier,
                _damageMultiplier * damageMultiplier,
                _attackIntervalMultiplier * attackIntervalMultiplier,
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
    }
}
