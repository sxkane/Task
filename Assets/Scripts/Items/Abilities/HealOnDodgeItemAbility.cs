using Events.PlayerEvents;
using UnityEngine;

namespace Items.Abilities
{
    [CreateAssetMenu(menuName = "Game/Item Ability/Heal On Dodge")]
    public class HealOnDodgeItemAbility : ItemAbility
    {
        [SerializeField] private float chancePercent = 50f;
        [SerializeField] private int healAmount = 5;

        public override void OnPlayerDamaged(ItemAbilityContext context, OnPlayerDamagedEvent eventData)
        {
            if (context?.Player == null || eventData == null || !eventData.IsDodged)
                return;

            if (Random.value <= chancePercent / 100f)
                context.Player.Heal(healAmount);
        }
    }
}
