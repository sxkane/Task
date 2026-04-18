using System.Collections.Generic;
using Stats;
using Weapons.Core;

namespace Weapons.Modifiers
{
    public static class WeaponStatModifierApplicator
    {
        public static void ApplyModifiers(WeaponRuntimeStats runtimeStats, IReadOnlyList<WeaponStatModifierDefinition> modifiers, object source)
        {
            if (runtimeStats == null || modifiers == null || source == null)
                return;

            for (var i = 0; i < modifiers.Count; i++)
            {
                var modifier = modifiers[i];
                runtimeStats.GetStat(modifier.statType).AddModifier(new Modifier(modifier.value, modifier.modType, source));
            }
        }
    }
}
