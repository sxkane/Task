using System.Collections;
using System.Collections.Generic;
using Data;
using Events;
using Events.ShopEvents;
using Player;
using Rewards.StatRewards;
using UnityEngine;
using Weapons;

namespace Rewards.Shops
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private GameDatabase gameDatabase;
        [SerializeField] private int refreshCostStep = 1;
        [SerializeField] private int offerCount = 4;

        private PlayerManager _playerManager;
        private WeaponManager _weaponManager;
        private Waves.WaveManager _waveManager;
        private PlayerController _player;
        private readonly List<ShopItem> _shopItems = new();
        private Coroutine _publishCoroutine;

        public void Configure(PlayerManager playerManager, WeaponManager weaponManager, Waves.WaveManager waveManager)
        {
            _playerManager = playerManager;
            _weaponManager = weaponManager;
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

            _player?.RuntimeData?.ResetRefreshCost();

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
            RefreshOffers(replaceUnlockedOnly: false);
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

                _shopItems[index] = ItemGenerator.GetWeaponShopOffer(GetCurrentWaveIndex(), GetLuck(), gameDatabase);
            }
        }

        private void PublishOffers()
        {
            EventBus.Publish(new OnShopItemsGeneratedEvent(new List<ShopItem>(_shopItems)));
        }

        private void LockShopItem(OnShopItemLockedEvent eventData)
        {
            for (var index = 0; index < _shopItems.Count; index++)
            {
                if (_shopItems[index] == eventData.ShopItem)
                    _shopItems[index].isLocked = !_shopItems[index].isLocked;
            }

            PublishOffers();
        }

        private void RefreshShop(OnShopRefreshEvent eventData)
        {
            var runtimeData = _player?.RuntimeData;
            if (runtimeData == null || !runtimeData.TrySpendCoins(runtimeData.RefreshCost))
                return;

            RefreshOffers(replaceUnlockedOnly: true);
            runtimeData.IncreaseRefreshCost(refreshCostStep);
            PublishOffers();
        }

        private void HandlePurchaseRequested(OnShopPurchaseRequestedEvent eventData)
        {
            if (eventData.ShopItem == null || _player?.RuntimeData == null)
                return;

            var price = eventData.ShopItem.GetPrice();
            if (!_player.RuntimeData.CanAfford(price))
                return;

            if (!_weaponManager.TryAddWeapon(eventData.ShopItem.GetWeaponEntry()))
                return;

            _player.RuntimeData.TrySpendCoins(price);

            var purchasedIndex = _shopItems.IndexOf(eventData.ShopItem);
            if (purchasedIndex >= 0)
                _shopItems[purchasedIndex] = null;

            RefreshOffers(replaceUnlockedOnly: false);
            PublishOffers();
        }

        private int GetCurrentWaveIndex()
        {
            return _waveManager != null ? _waveManager.CurrentWave + 1 : 1;
        }

        private int GetLuck()
        {
            return _player != null && _player.Stats != null ? _player.Stats.Luck : 0;
        }
    }
}
