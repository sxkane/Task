using System.Collections.Generic;
using Events;
using Events.DisplayEvent;
using Items;
using Player;
using Stats;
using UnityEngine;
using Weapons.Effects;

namespace Weapons.Items
{
    public class ItemManager : MonoBehaviour
    {
        #region Runtime

        private PlayerManager _playerManager;
        private PlayerController _player;
        private readonly List<ItemData> _items = new();

        #endregion

        public void Configure(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void InitializeRun()
        {
            _player = _playerManager != null ? _playerManager.Player : null;
            _items.Clear();
        }

        public void ResetRun()
        {
            RemoveAllItems();
            _player = null;
        }

        public void BeginPhase()
        {
        }

        public void EndPhase()
        {
        }

        public void AddItem(ItemData item)
        {
            if (_player == null || item == null)
                return;

            if (item.modifies != null)
            {
                foreach (var modify in item.modifies)
                {
                    var stat = _player.Stats.GetStat(modify.statType);
                    var modifier = new Modifier(modify.value, modify.modType, item);
                    stat.AddModifier(modifier);
                }
            }

            if (item.effects != null)
            {
                var context = EffectExecutionContext.ForItem(_player, item);
                foreach (var effect in item.effects)
                {
                    if (effect == null)
                        continue;

                    effect.Execute(context, EffectTrigger.OnItemAdded);
                }
            }

            _items.Add(item);
        }

        public bool TryAddItem(ItemData item)
        {
            AddItem(item);
            return true;
        }

        public void RemoveItem(ItemData item)
        {
            if (_player == null || item == null)
                return;

            if (item.modifies != null)
            {
                foreach (var modify in item.modifies)
                {
                    var stat = _player.Stats.GetStat(modify.statType);
                    stat.RemoveModifiersFromSource(item);
                }
            }

            _items.Remove(item);
        }

        public void RemoveAllItems()
        {
            for (var index = _items.Count - 1; index >= 0; index--)
                RemoveItem(_items[index]);

            _items.Clear();
        }

        private void ShowAllItems()
        {
            EventBus.Publish(new OnItemsDisplay(_items));
        }
    }
}
