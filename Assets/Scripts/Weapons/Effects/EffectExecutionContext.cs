using Items;
using Player;
using UnityEngine;

namespace Weapons.Effects
{
    public sealed class EffectExecutionContext
    {
        public PlayerController Player { get; private set; }
        public ItemData ItemData { get; private set; }
        public object SourceToken { get; private set; }

        public static EffectExecutionContext ForItem(PlayerController player, ItemData itemData, object sourceToken)
        {
            return new EffectExecutionContext
            {
                Player = player,
                ItemData = itemData,
                SourceToken = sourceToken
            };
        }
    }
}
