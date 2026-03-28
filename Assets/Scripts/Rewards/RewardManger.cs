using System;
using System.Collections.Generic;
using Data;
using Events;
using Player;
using Rewards.Shops;
using Rewards.StatRewards;
using UnityEngine;
using Waves;
using Weapons;

namespace Rewards
{
    public class RewardManger : MonoBehaviour
    {
        [SerializeField]
        private GameDatabase gameDatabase;

        private PlayerController _player;
        private WaveManager _wave;
        private WeaponManager _weapon;
        
        private List<StatReward> _statRewards;
        private List<ShopItem> _shopItems;

        private List<StatReward> GenerateStats()
        {
            _statRewards = new List<StatReward>();
            for (int i = 0; i < 4; i++)
            {
                _statRewards.Add(ItemGenerator.GetStatReward(_wave.CurrentWave + 1, _player.Stats.Luck));
            }

            return _statRewards;
        }
        
        public List<ShopItem> GenerateShopItems()
        {
            if (_shopItems != null)
            {
                RefreshShopItems();
                return _shopItems;
            }
            
            _shopItems = new List<ShopItem>();
            for (int i = 0; i < 4; i++)
            {
                _shopItems.Add(ItemGenerator.GetItemReward(_wave.CurrentWave + 1, _player.Stats.Luck, gameDatabase));
            }
            
            return _shopItems;
        }

        public void LockShopItem(OnShopItemLockedEvent e)
        {
            for (int i = 0; i < _shopItems.Count; i++)
            {
                if (_shopItems[i] == e.ShopItem)
                {
                    _shopItems[i].isLocked = !_shopItems[i].isLocked;
                }
            }
        }

        public void RefreshShopItems()
        {
            for (var i = 0; i < _shopItems.Count; i++)
            {
                if (_shopItems[i].isLocked) continue;
                _shopItems[i] =
                    ItemGenerator.GetItemReward(_wave.CurrentWave + 1, _player.Stats.Luck, gameDatabase);
            }
        }
        
        public void RefreshShop(OnShopRefreshEvent e)
        {
            RefreshShopItems();
            EventBus.Publish(new OnShopItemsGeneratedEvent(_shopItems));
        }

        public void Activate()
        {
            EventBus.Subscribe<OnShopItemLockedEvent>(LockShopItem);
            EventBus.Subscribe<OnShopRefreshEvent>(RefreshShop);
            
            var shopItems = GenerateShopItems();
            EventBus.Publish(new OnShopItemsGeneratedEvent(shopItems));
        }

        public void Deactivate()
        {
            EventBus.Unsubscribe<OnShopItemLockedEvent>(LockShopItem);
            EventBus.Unsubscribe<OnShopRefreshEvent>(RefreshShop);
        }

        public void Initialize(PlayerManager playerManager, WeaponManager weaponManager, WaveManager waveManager)
        {
            _player = playerManager.Player;
            _wave = waveManager;
            _weapon = weaponManager;

            var weapons = gameDatabase.weapons;
            foreach (var w in weapons)
            {
                _weapon.TryAddWeapon(w, Rarity.Common);
            }
        }
    }
}