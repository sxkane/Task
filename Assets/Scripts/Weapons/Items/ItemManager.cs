using System.Collections.Generic;
using Player;
using Stats;
using UnityEngine;

namespace Weapons.Items
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance;
        
        private PlayerController _player;
        private readonly List<ItemData> _items = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void AddItem(ItemData item)
        {
            foreach (var modify in item.modifies)
            {
                var stat = _player.Stats.GetStat(modify.statType);

                var modifier = new Modifier(
                    modify.value,
                    modify.modType,
                    item
                );

                stat.AddModifier(modifier);
            }
            _items.Add(item);
        }

        public void RemoveItem(ItemData item)
        {
            foreach (var modify in item.modifies)
            {
                var stat = _player.Stats.GetStat(modify.statType);

                stat.RemoveModifiersFromSource(item);
            }
            _items.Remove(item);
        }
        
        public void Initialize(PlayerManager player)
        {
            _player = player.Player;
        }
    }
}