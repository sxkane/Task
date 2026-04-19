using Stats;
using UnityEngine;

namespace Items.Abilities
{
    [CreateAssetMenu(menuName = "Game/Item Ability/Enemy Count Scaled Bonus")]
    public class EnemyCountScaledBonusItemAbility : ItemAbility
    {
        [SerializeField] private StatType statType;
        [SerializeField] private float valuePerEnemy = 1f;
        [SerializeField] private StatModType modType = StatModType.Flat;
        [SerializeField] private int maxEnemyCount = -1;

        private readonly System.Collections.Generic.Dictionary<object, float> _lastValues = new();

        public override void OnInitialize(ItemAbilityContext context)
        {
            _lastValues[context.SourceToken] = 0f;
        }

        public override void OnRemoved(ItemAbilityContext context)
        {
            RemoveModifier(context);
            _lastValues.Remove(context.SourceToken);
        }

        public override void OnUpdate(ItemAbilityContext context, float deltaTime)
        {
            if (context?.Player?.Stats == null || Core.GameController.Instance?.WaveManager?.EnemyManager == null)
                return;

            var enemyCount = Core.GameController.Instance.WaveManager.EnemyManager.AliveEnemyCount;
            if (maxEnemyCount >= 0)
                enemyCount = Mathf.Min(enemyCount, maxEnemyCount);

            var newValue = enemyCount * valuePerEnemy;
            if (_lastValues.TryGetValue(context.SourceToken, out var lastValue) && Mathf.Approximately(lastValue, newValue))
                return;

            RemoveModifier(context);
            if (!Mathf.Approximately(newValue, 0f))
            {
                var stat = context.Player.Stats.GetStat(statType);
                stat.AddModifier(StatValueUtility.CreatePlayerModifier(statType, newValue, modType, context.SourceToken));
            }

            _lastValues[context.SourceToken] = newValue;
        }

        private void RemoveModifier(ItemAbilityContext context)
        {
            if (context?.Player?.Stats == null)
                return;

            context.Player.Stats.GetStat(statType).RemoveModifiersFromSource(context.SourceToken);
        }
    }
}
