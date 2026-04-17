using System.Collections.Generic;
using UnityEngine;

namespace Stats.Buffs
{
    [CreateAssetMenu(menuName = "Game/Buff Data")]
    public class BuffData : ScriptableObject
    {
        [SerializeField] private string buffId;
        [SerializeField] private float duration = 3f;
        [SerializeField] private bool refreshDurationOnReapply = true;
        [SerializeField] private List<StatModifierDefinition> modifiers = new();

        public string BuffId => string.IsNullOrWhiteSpace(buffId) ? name : buffId;
        public float Duration => duration;
        public bool RefreshDurationOnReapply => refreshDurationOnReapply;
        public IReadOnlyList<StatModifierDefinition> Modifiers => modifiers;

        public void InitializeRuntime(
            string runtimeBuffId,
            float runtimeDuration,
            bool runtimeRefreshDurationOnReapply,
            List<StatModifierDefinition> runtimeModifiers)
        {
            buffId = runtimeBuffId;
            duration = runtimeDuration;
            refreshDurationOnReapply = runtimeRefreshDurationOnReapply;
            modifiers = runtimeModifiers ?? new List<StatModifierDefinition>();
        }
    }
}
