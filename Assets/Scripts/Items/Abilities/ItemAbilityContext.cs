using Player;
using UnityEngine;

namespace Items.Abilities
{
    public sealed class ItemAbilityContext
    {
        public PlayerController Player { get; private set; }
        public ItemData ItemData { get; private set; }
        public object SourceToken { get; private set; }

        public static ItemAbilityContext ForItem(PlayerController player, ItemData itemData, object sourceToken)
        {
            return new ItemAbilityContext
            {
                Player = player,
                ItemData = itemData,
                SourceToken = sourceToken
            };
        }
    }
}
