using Items;
using Player;
using UnityEngine;

namespace Weapons.Effects
{
    public sealed class EffectExecutionContext
    {
        public PlayerController Player { get; private set; }
        public ItemData ItemData { get; private set; }

        public static EffectExecutionContext ForItem(PlayerController player, ItemData itemData)
        {
            return new EffectExecutionContext
            {
                Player = player,
                ItemData = itemData
            };
        }
    }
}
