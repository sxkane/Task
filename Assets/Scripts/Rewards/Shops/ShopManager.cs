using System.Collections;
using System.Collections.Generic;
using Audio;
using Data;
using Events;
using Events.ShopEvents;
using GameAudio;
using Player;
using Rewards.StatRewards;
using UnityEngine;
using Weapons;
using Weapons.Items;

namespace Rewards.Shops
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private GameDatabase gameDatabase;
        [SerializeField] private int offerCount = 4;
        [SerializeField] private float shopPriceMultiplier = 1f;

        private PlayerManager _playerManager;
        private WeaponManager _weaponManager;
        private ItemManager _itemManager;
        private Waves.WaveManager _waveManager;
        private PlayerController _player;
        private readonly List<ShopItem> _shopItems = new();
        private Coroutine _publishCoroutine;

        public void Configure(PlayerManager playerManager, WeaponManager weaponManager, ItemManager itemManager, Waves.WaveManager waveManager)
        {
            _playerManager = playerManager;
            _weaponManager = weaponManager;
            _itemManager = itemManager;
            _waveManager = waveManager;
        }

        public void InitializeRun()
        {
            _player = _playerManager != null ? _playerManager.Player : null;
            _shopItems.Clear();
        }

        public void ResetRun()
        {
            EndPhase();

            if (_publishCoroutine != null)
            {
                StopCoroutine(_publishCoroutine);
                _publishCoroutine = null;
            }

            _shopItems.Clear();
            _player = null;
        }

        public void BeginPhase()
        {
            EventBus.Subscribe<OnShopItemLockedEvent>(LockShopItem);
            EventBus.Subscribe<OnShopRefreshEvent>(RefreshShop);
            EventBus.Subscribe<OnShopPurchaseRequestedEvent>(HandlePurchaseRequested);

            _player?.RuntimeData?.ResetRefreshCost(GetFirstRerollPrice());

            if (_publishCoroutine != null)
                StopCoroutine(_publishCoroutine);

            _publishCoroutine = StartCoroutine(PublishShopItemsNextFrame());
        }

        public void EndPhase()
        {
            EventBus.Unsubscribe<OnShopItemLockedEvent>(LockShopItem);
            EventBus.Unsubscribe<OnShopRefreshEvent>(RefreshShop);
            EventBus.Unsubscribe<OnShopPurchaseRequestedEvent>(HandlePurchaseRequested);

            if (_publishCoroutine == null)
                return;

            StopCoroutine(_publishCoroutine);
            _publishCoroutine = null;
        }

        private IEnumerator PublishShopItemsNextFrame()
        {
            yield return null;
            RefreshOffers(replaceUnlockedOnly: true);
            PublishOffers();
            _publishCoroutine = null;
        }

        private void RefreshOffers(bool replaceUnlockedOnly)
        {
            while (_shopItems.Count < offerCount)
                _shopItems.Add(null);

            for (var index = 0; index < offerCount; index++)
            {
                if (replaceUnlockedOnly && _shopItems[index] != null && _shopItems[index].isLocked)
                    continue;

                RefreshOfferAt(index);
            }
        }

        private void PublishOffers()
        {
            EventBus.Publish(new OnShopItemsGeneratedEvent(new List<ShopItem>(_shopItems)));
        }

        private void RefreshOfferAt(int index)
        {
            if (index < 0 || index >= offerCount)
                return;

            var excludedKeys = BuildExcludedOfferKeys(index);
            var offer = ItemGenerator.GetShopOffer(GetCurrentWaveIndex(), GetLuck(), gameDatabase, excludedKeys);
            if (offer != null)
                offer.ConfigureShopData(index, GetCurrentWaveIndex(), shopPriceMultiplier);

            _shopItems[index] = offer;
        }

        private void LockShopItem(OnShopItemLockedEvent eventData)
        {
            var slotIndex = ResolveSlotIndex(eventData.ShopItem);
            if (slotIndex < 0 || slotIndex >= _shopItems.Count || _shopItems[slotIndex] == null)
                return;

            _shopItems[slotIndex].isLocked = !_shopItems[slotIndex].isLocked;

            PublishOffers();
        }

        private void RefreshShop(OnShopRefreshEvent eventData)
        {
            var runtimeData = _player?.RuntimeData;
            if (runtimeData == null || !runtimeData.TrySpendCoins(runtimeData.RefreshCost))
                return;

            RefreshOffers(replaceUnlockedOnly: true);
            runtimeData.IncreaseRefreshCost(GetRerollIncrease());
            GlobalSfxPlayer.Instance.PlayShopRefresh();
            PublishOffers();
        }

        private void HandlePurchaseRequested(OnShopPurchaseRequestedEvent eventData)
        {
            if (eventData.ShopItem == null || _player?.RuntimeData == null)
                return;

            var price = eventData.ShopItem.GetPrice();
            if (!_player.RuntimeData.CanAfford(price))
                return;

            var purchaseSucceeded = eventData.ShopItem.IsItem
                ? _itemManager != null && _itemManager.TryAddItem(eventData.ShopItem.itemData)
                : _weaponManager != null && _weaponManager.TryAddWeapon(eventData.ShopItem.GetWeaponEntry());

            if (!purchaseSucceeded)
                return;

            _player.RuntimeData.TrySpendCoins(price);
            GlobalSfxPlayer.Instance.PlayShopPurchase();

            var purchasedIndex = ResolveSlotIndex(eventData.ShopItem);
            if (purchasedIndex >= 0)
                RefreshOfferAt(purchasedIndex);

            PublishOffers();
        }

        private int ResolveSlotIndex(ShopItem shopItem)
        {
            if (shopItem == null)
                return -1;

            if (shopItem.slotIndex >= 0 && shopItem.slotIndex < _shopItems.Count && _shopItems[shopItem.slotIndex] != null)
                return shopItem.slotIndex;

            return _shopItems.IndexOf(shopItem);
        }

        private int GetCurrentWaveIndex()
        {
            return _waveManager != null ? _waveManager.CurrentWave + 1 : 1;
        }

        private HashSet<string> BuildExcludedOfferKeys(int exceptIndex)
        {
            var excludedKeys = new HashSet<string>();
            for (var i = 0; i < _shopItems.Count; i++)
            {
                if (i == exceptIndex)
                    continue;

                var item = _shopItems[i];
                if (item == null)
                    continue;

                var key = ItemGenerator.GetShopItemKey(item);
                if (!string.IsNullOrWhiteSpace(key))
                    excludedKeys.Add(key);
            }

            return excludedKeys;
        }

        private int GetRerollIncrease()
        {
            return Mathf.Max(1, Mathf.FloorToInt(GetCurrentWaveIndex() * 0.4f));
        }

        private int GetFirstRerollPrice()
        {
            return Mathf.FloorToInt(GetCurrentWaveIndex() * 0.75f) + GetRerollIncrease();
        }

        private int GetLuck()
        {
            return _player != null && _player.Stats != null ? _player.Stats.Luck : 0;
        }
    }
}
