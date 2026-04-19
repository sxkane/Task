using Events.EnemyEvents;
using Events.PlayerEvents;
using UnityEngine;

namespace Items.Abilities
{
    public abstract class ItemAbility : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private string description;

        public virtual void OnInitialize(ItemAbilityContext context)
        {
        }

        public virtual void OnRemoved(ItemAbilityContext context)
        {
        }

        public virtual void OnUpdate(ItemAbilityContext context, float deltaTime)
        {
        }

        public virtual void OnPlayerDamaged(ItemAbilityContext context, OnPlayerDamagedEvent eventData)
        {
        }

        public virtual void OnEnemyDied(ItemAbilityContext context, OnEnemyDiedEvent eventData)
        {
        }

        public virtual string BuildDescription()
        {
            if (!string.IsNullOrWhiteSpace(description))
                return description.Trim();

            return string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        }

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
