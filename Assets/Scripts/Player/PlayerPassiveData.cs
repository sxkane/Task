using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "Game/Player Passive Data")]
    public class PlayerPassiveData : ScriptableObject
    {
        [SerializeField] private string passiveId;
        [SerializeField] private string displayName;
        [SerializeField] private List<PlayerStatModifierDefinition> modifiers = new();

        public string PassiveId => string.IsNullOrWhiteSpace(passiveId) ? name : passiveId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public IReadOnlyList<PlayerStatModifierDefinition> Modifiers => modifiers;
    }
}
