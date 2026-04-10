using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Game/Weapon Effect")]
    public class Effect : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string effectId;

        [Header("Presentation")]
        [SerializeField] private string displayName;
        [SerializeField] private string description;

        [Header("Execution")]
        [SerializeField] private EffectTrigger trigger = EffectTrigger.Manual;

        public string EffectId => string.IsNullOrWhiteSpace(effectId) ? name : effectId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public EffectTrigger Trigger => trigger;

        public virtual void Execute(EffectExecutionContext context, EffectTrigger effectTrigger)
        {
            if (!CanExecute(effectTrigger))
                return;

            Apply(context);
        }

        public bool CanExecute(EffectTrigger effectTrigger)
        {
            return trigger == EffectTrigger.Manual || trigger == effectTrigger;
        }

        protected virtual void Apply(EffectExecutionContext context)
        {
            Debug.Log($"Effect executed: {DisplayName}");
        }

        public virtual string BuildDescription()
        {
            if (!string.IsNullOrWhiteSpace(description))
                return description.Trim();

            return DisplayName;
        }

        public virtual bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EffectId);
        }
    }
}
