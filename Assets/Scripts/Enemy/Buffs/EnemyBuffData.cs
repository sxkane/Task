using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Buffs
{
    [CreateAssetMenu(menuName = "Game/Enemy Buff Data")]
    public class EnemyBuffData : ScriptableObject
    {
        [SerializeField] private string buffId;
        [SerializeField] private float duration = 3f;
        [SerializeField] private bool refreshDurationOnReapply = true;
        [SerializeField] private List<EnemyStatModifierDefinition> modifiers = new();

        public string BuffId => string.IsNullOrWhiteSpace(buffId) ? name : buffId;
        public float Duration => duration;
        public bool RefreshDurationOnReapply => refreshDurationOnReapply;
        public IReadOnlyList<EnemyStatModifierDefinition> Modifiers => modifiers;

        public void InitializeRuntime(
            string runtimeBuffId,
            float runtimeDuration,
            bool runtimeRefreshDurationOnReapply,
            List<EnemyStatModifierDefinition> runtimeModifiers)
        {
            buffId = runtimeBuffId;
            duration = runtimeDuration;
            refreshDurationOnReapply = runtimeRefreshDurationOnReapply;
            modifiers = runtimeModifiers ?? new List<EnemyStatModifierDefinition>();
        }
    }
}
