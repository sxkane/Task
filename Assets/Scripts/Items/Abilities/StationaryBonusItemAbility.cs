using System.Collections.Generic;
using Stats;
using UnityEngine;

namespace Items.Abilities
{
    [CreateAssetMenu(menuName = "Game/Item Ability/Stationary Bonus")]
    public class StationaryBonusItemAbility : ItemAbility
    {
        [SerializeField] private List<ItemModify> stationaryBonuses = new();
        [SerializeField] private float movementThreshold = 0.01f;

        private readonly Dictionary<object, bool> _activeStates = new();

        public override void OnInitialize(ItemAbilityContext context)
        {
            _activeStates[context.SourceToken] = false;
        }

        public override void OnRemoved(ItemAbilityContext context)
        {
            ApplyBonuses(context, false);
            _activeStates.Remove(context.SourceToken);
        }

        public override void OnUpdate(ItemAbilityContext context, float deltaTime)
        {
            if (context?.Player?.Input == null)
                return;

            var isStationary = context.Player.Input.MoveInput.sqrMagnitude <= movementThreshold * movementThreshold;
            var isActive = _activeStates.TryGetValue(context.SourceToken, out var state) && state;
            if (isStationary == isActive)
                return;

            ApplyBonuses(context, isStationary);
            _activeStates[context.SourceToken] = isStationary;
        }

        private void ApplyBonuses(ItemAbilityContext context, bool enabled)
        {
            if (context?.Player?.Stats == null || stationaryBonuses == null)
                return;

            for (var i = 0; i < stationaryBonuses.Count; i++)
            {
                var bonus = stationaryBonuses[i];
                if (bonus == null)
                    continue;

                var stat = context.Player.Stats.GetStat(bonus.statType);
                if (enabled)
                    stat.AddModifier(StatValueUtility.CreatePlayerModifier(bonus.statType, bonus.value, bonus.modType, context.SourceToken));
                else
                    stat.RemoveModifiersFromSource(context.SourceToken);
            }
        }
    }
}
