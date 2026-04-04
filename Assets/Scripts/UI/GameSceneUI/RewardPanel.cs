using System.Collections.Generic;
using Core;
using Events;
using Events.ShopEvents;
using GameFlow;
using Player;
using Rewards.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class RewardPanel : MonoBehaviour
    {
        [Header("Slots")]
        [SerializeField] private List<RewardSlot> slots;

        [Header("Buttons")]
        [SerializeField] private Button nextWaveButton;
        [SerializeField] private Button freshButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI freshCostText;
        [SerializeField] private TextMeshProUGUI coinText;

        private PlayerRuntimeData _runtimeData;
        
        public void ShowItems(OnShopItemsGeneratedEvent e)
        {
            var items = e.ItemOptions;

            for (int i = 0; i < slots.Count; i++)
            {
                if (i < items.Count && items[i] != null)
                    slots[i].Show(items[i]);
            }
        }

        public void FreshItems()
        {
            EventBus.Publish(new OnShopRefreshEvent());
        }

        public void GoToNextWave()
        {
            GameController.Instance.ChangeState(GamePhaseType.Battle);
        }

        private void OnEnable()
        {
            nextWaveButton.onClick.AddListener(GoToNextWave);
            freshButton.onClick.AddListener(FreshItems);
            EventBus.Subscribe<OnShopItemsGeneratedEvent>(ShowItems);
            TryBindRuntimeData();
            RefreshCostText();
            RefreshCurrentCoins();
        }
        
        private void OnDisable()
        {
            nextWaveButton.onClick.RemoveListener(GoToNextWave);
            freshButton.onClick.RemoveListener(FreshItems);
            EventBus.Unsubscribe<OnShopItemsGeneratedEvent>(ShowItems);
            UnbindRuntimeData();
        }

        private void Update()
        {
            if (_runtimeData == null)
            {
                TryBindRuntimeData();
                RefreshCostText();
            }
        }

        private void TryBindRuntimeData()
        {
            var player = GameController.Instance?.PlayerManager?.Player;
            if (player?.RuntimeData == null || player.RuntimeData == _runtimeData)
                return;

            UnbindRuntimeData();
            _runtimeData = player.RuntimeData;
            _runtimeData.OnRefreshCostChanged += OnRefreshCostChanged;
            _runtimeData.OnCoinsChanged += OnCoinsChanged;
        }

        private void UnbindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            _runtimeData.OnRefreshCostChanged -= OnRefreshCostChanged;
            _runtimeData.OnCoinsChanged -= OnCoinsChanged;
            _runtimeData = null;
        }

        private void OnRefreshCostChanged(int cost)
        {
            RefreshCostText();
        }

        private void OnCoinsChanged(int coins)
        {
            RefreshCostText();
            RefreshCurrentCoins();
        }

        private void RefreshCostText()
        {
            if (freshCostText == null)
                return;

            if (_runtimeData == null)
            {
                freshCostText.text = "";
                return;
            }

            freshCostText.text = _runtimeData.RefreshCost.ToString();
        }

        private void RefreshCurrentCoins()
        {
            if (coinText == null)
                return;
            if (_runtimeData == null)
            {
                freshCostText.text = "";
                return;
            }
            
            coinText.text = _runtimeData.Coins.ToString();
        }
    }
}
