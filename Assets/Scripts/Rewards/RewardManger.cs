using System.Collections;
using System.Collections.Generic;
using Data;
using Events;
using Events.ShopEvents;
using Player;
using Rewards.Shops;
using Rewards.StatRewards;
using UnityEngine;
using Waves;
using Weapons;
using Weapons.Items;

namespace Rewards
{
    public class RewardManger : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private GameDatabase gameDatabase;

        [SerializeField] private int refreshCostStep = 1;

        [Header("Manager")]
        private PlayerController _player;
        private WaveManager _waveManager;
        private WeaponManager _weaponManager;
        private ItemManager _itemManager;

        [Header("Shop Items")]
        private List<StatReward> _statRewards;
        private List<ShopItem> _shopItems;
        private Coroutine _publishCoroutine;
        private bool _pendingPurchaseSucceeded;

        #region Life Cycle

        public void Initialize(PlayerManager playerManager, WeaponManager weaponManager, ItemManager itemManager,
            WaveManager waveManager)
        {
            _player = playerManager.Player;
            _waveManager = waveManager;
            _weaponManager = weaponManager;
            _itemManager = itemManager;
        }
        
        public void Activate()
        {
            EventBus.Subscribe<OnShopItemLockedEvent>(LockShopItem);
            EventBus.Subscribe<OnShopRefreshEvent>(RefreshShop);
            EventBus.Subscribe<OnShopPurchaseRequestedEvent>(HandlePurchaseRequested);

            _player.RuntimeData?.ResetRefreshCost();

            if (_publishCoroutine != null)
                StopCoroutine(_publishCoroutine);

            _publishCoroutine = StartCoroutine(PublishShopItemsNextFrame());
        }

        public void Deactivate()
        {
            EventBus.Unsubscribe<OnShopItemLockedEvent>(LockShopItem);
            EventBus.Unsubscribe<OnShopRefreshEvent>(RefreshShop);
            EventBus.Unsubscribe<OnShopPurchaseRequestedEvent>(HandlePurchaseRequested);

            if (_publishCoroutine != null)
            {
                StopCoroutine(_publishCoroutine);
                _publishCoroutine = null;
            }
        }

        private IEnumerator PublishShopItemsNextFrame()
        {
            yield return null;

            RefreshShopItems();
            GenerateShopItems();
            EventBus.Publish(new OnShopItemsGeneratedEvent(_shopItems));
            _publishCoroutine = null;
        }
        
        #endregion

        #region Shop Operation

        // generate items
        public void GenerateShopItems()
        {
            if (_shopItems == null)
            {
                _shopItems = new List<ShopItem>();
                for (int i = 0; i < 4; i++)
                {
                    _shopItems.Add(ItemGenerator.GetItemReward(_waveManager.CurrentWave + 1, _player.Stats.Luck,
                        gameDatabase));
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (_shopItems[i] != null)
                    continue;
                _shopItems[i] = ItemGenerator.GetItemReward(_waveManager.CurrentWave + 1, _player.Stats.Luck, gameDatabase);
            }
        }

        // lock
        public void LockShopItem(OnShopItemLockedEvent e)
        {
            for (int i = 0; i < _shopItems.Count; i++)
            {
                if (_shopItems[i] == null)
                    continue;

                if (_shopItems[i] == e.ShopItem)
                    _shopItems[i].isLocked = !_shopItems[i].isLocked;
            }
        }

        // refresh items
        public void RefreshShop(OnShopRefreshEvent e)
        {
            var runtimeData = _player.RuntimeData;
            if (runtimeData == null || !runtimeData.TrySpendCoins(runtimeData.RefreshCost))
                return;

            RefreshShopItems();
            runtimeData.IncreaseRefreshCost(refreshCostStep);
            EventBus.Publish(new OnShopItemsGeneratedEvent(_shopItems));
        }

        private void RefreshShopItems()
        {
            if (_shopItems == null)
                return; 
            for (var i = 0; i < _shopItems.Count; i++)
            {
                if (_shopItems[i] == null)
                {
                    _shopItems[i] =
                        ItemGenerator.GetItemReward(_waveManager.CurrentWave + 1, _player.Stats.Luck, gameDatabase);
                    continue;
                }

                if (_shopItems[i].isLocked)
                    continue;

                _shopItems[i] =
                    ItemGenerator.GetItemReward(_waveManager.CurrentWave + 1, _player.Stats.Luck, gameDatabase);
            }
        }

        // purchase
        private void HandlePurchaseRequested(OnShopPurchaseRequestedEvent e)
        {
            if (e.ShopItem == null || _player?.RuntimeData == null)
                return;

            int price = e.ShopItem.GetPrice();
            if (!_player.RuntimeData.CanAfford(price))
                return;

            _pendingPurchaseSucceeded = false;

            if (e.ShopItem.IsWeapon)
                _pendingPurchaseSucceeded = _weaponManager.TryAddWeapon(
                    e.ShopItem.weaponEntry.weaponData,
                    e.ShopItem.weaponEntry.rarity);
            else
                _pendingPurchaseSucceeded = _itemManager.TryAddItem(e.ShopItem.itemData);

            if (!_pendingPurchaseSucceeded)
                return;

            _player.RuntimeData.TrySpendCoins(price);

            int purchasedIndex = _shopItems.IndexOf(e.ShopItem);
            if (purchasedIndex >= 0)
                _shopItems[purchasedIndex] = null;

            Debug.Log("But item");
            GenerateShopItems();
            EventBus.Publish(new OnShopItemsGeneratedEvent(_shopItems));
        }
        
        #endregion
        
        private List<StatReward> GenerateStats()
        {
            _statRewards = new List<StatReward>();
            for (int i = 0; i < 4; i++)
            {
                _statRewards.Add(ItemGenerator.GetStatReward(_waveManager.CurrentWave + 1, _player.Stats.Luck));
            }

            return _statRewards;
        }
    }
}
