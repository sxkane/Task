using System.Collections.Generic;

namespace Stats.Buffs
{
    public static class StatModifierApplicator
    {
        public static void ApplyModifiers(IBuffStatSource target, IReadOnlyList<StatModifierDefinition> modifiers, object source)
        {
            if (target == null || modifiers == null || source == null)
                return;

            for (var i = 0; i < modifiers.Count; i++)
            {
                var modifier = modifiers[i];
                if (!target.TryGetStat(modifier.statKey, out var stat))
                    continue;

                stat.AddModifier(new Modifier(modifier.value, modifier.modifierType, source));
            }
        }
    }
}
