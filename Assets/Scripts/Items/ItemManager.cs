using System.Collections.Generic;
using Events;
using Events.DisplayEvent;
using Events.EnemyEvents;
using Events.PlayerEvents;
using Items;
using Items.Abilities;
using Player;
using Stats;
using UnityEngine;

namespace Weapons.Items
{
    public class ItemManager : MonoBehaviour
    {
        private sealed class ItemRuntimeEntry
        {
            public ItemData Data;
            public object SourceToken;
            public ItemAbilityContext Context;
        }

        #region Runtime

        private PlayerManager _playerManager;
        private PlayerController _player;
        private readonly List<ItemRuntimeEntry> _items = new();

        #endregion

        public void Configure(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void InitializeRun()
        {
            _player = _playerManager != null ? _playerManager.Player : null;
            _items.Clear();
            EventBus.Subscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Subscribe<OnEnemyDiedEvent>(OnEnemyDied);
            ShowAllItems();
        }

        public void ResetRun()
        {
            RemoveAllItems();
            EventBus.Unsubscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);
            _player = null;
            ShowAllItems();
        }

        public void BeginPhase()
        {
        }

        public void EndPhase()
        {
        }

        private void Update()
        {
            if (_player == null || _items.Count == 0)
                return;

            for (var i = 0; i < _items.Count; i++)
            {
                var runtimeEntry = _items[i];
                if (runtimeEntry?.Data?.abilities == null)
                    continue;

                for (var abilityIndex = 0; abilityIndex < runtimeEntry.Data.abilities.Count; abilityIndex++)
                {
                    var ability = runtimeEntry.Data.abilities[abilityIndex];
                    if (ability == null)
                        continue;

                    ability.OnUpdate(runtimeEntry.Context, Time.deltaTime);
                }
            }
        }

        public void AddItem(ItemData item)
        {
            if (_player == null || item == null)
                return;

            var sourceToken = new object();
            var context = ItemAbilityContext.ForItem(_player, item, sourceToken);

            if (item.modifies != null)
            {
                foreach (var modify in item.modifies)
                {
                    var stat = _player.Stats.GetStat(modify.statType);
                    var modifier = StatValueUtility.CreatePlayerModifier(modify.statType, modify.value, modify.modType, sourceToken);
                    stat.AddModifier(modifier);
                }
            }

            if (item.abilities != null)
            {
                foreach (var ability in item.abilities)
                {
                    if (ability == null)
                        continue;

                    ability.OnInitialize(context);
                }
            }

            _items.Add(new ItemRuntimeEntry
            {
                Data = item,
                SourceToken = sourceToken,
                Context = context
            });

            ShowAllItems();
        }

        public bool TryAddItem(ItemData item)
        {
            AddItem(item);
            return true;
        }

        public List<ItemData> GetItemsSnapshot()
        {
            var itemData = new List<ItemData>(_items.Count);
            for (var i = 0; i < _items.Count; i++)
            {
                if (_items[i]?.Data != null)
                    itemData.Add(_items[i].Data);
            }

            return itemData;
        }

        public void RemoveItem(ItemData item)
        {
            if (_player == null || item == null)
                return;

            var index = _items.FindIndex(entry => entry.Data == item);
            if (index < 0)
                return;

            var runtimeEntry = _items[index];

            if (item.modifies != null)
            {
                foreach (var modify in item.modifies)
                {
                    var stat = _player.Stats.GetStat(modify.statType);
                    stat.RemoveModifiersFromSource(runtimeEntry.SourceToken);
                }
            }

            if (item.abilities != null)
            {
                foreach (var ability in item.abilities)
                {
                    if (ability == null)
                        continue;

                    ability.OnRemoved(runtimeEntry.Context);
                }
            }

            _items.RemoveAt(index);
            ShowAllItems();
        }

        public void RemoveAllItems()
        {
            for (var index = _items.Count - 1; index >= 0; index--)
                RemoveItem(_items[index].Data);

            _items.Clear();
        }

        private void ShowAllItems()
        {
            var itemData = new List<ItemData>(_items.Count);
            for (var i = 0; i < _items.Count; i++)
                itemData.Add(_items[i].Data);

            EventBus.Publish(new OnItemsDisplay(itemData));
        }

        private void OnPlayerDamaged(OnPlayerDamagedEvent eventData)
        {
            if (_player == null || eventData.Target != _player)
                return;

            for (var i = 0; i < _items.Count; i++)
            {
                var runtimeEntry = _items[i];
                if (runtimeEntry?.Data?.abilities == null)
                    continue;

                for (var abilityIndex = 0; abilityIndex < runtimeEntry.Data.abilities.Count; abilityIndex++)
                {
                    var ability = runtimeEntry.Data.abilities[abilityIndex];
                    if (ability == null)
                        continue;

                    ability.OnPlayerDamaged(runtimeEntry.Context, eventData);
                }
            }
        }

        private void OnEnemyDied(OnEnemyDiedEvent eventData)
        {
            if (_player == null)
                return;

            for (var i = 0; i < _items.Count; i++)
            {
                var runtimeEntry = _items[i];
                if (runtimeEntry?.Data?.abilities == null)
                    continue;

                for (var abilityIndex = 0; abilityIndex < runtimeEntry.Data.abilities.Count; abilityIndex++)
                {
                    var ability = runtimeEntry.Data.abilities[abilityIndex];
                    if (ability == null)
                        continue;

                    ability.OnEnemyDied(runtimeEntry.Context, eventData);
                }
            }
        }
    }
}
