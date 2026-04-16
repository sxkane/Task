using System.Collections.Generic;
using Core;
using Data;
using Data.Text;
using Events;
using Events.ShopEvents;
using GameFlow;
using Player;
using TMPro;
using UI.GameSceneUI.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI.Reward
{
    public class ShopPanel : MonoBehaviour
    {
        [Header("Slots")]
        [SerializeField] private List<RewardSlot> slots;

        [Header("Buttons")]
        [SerializeField] private Button nextWaveButton;
        [SerializeField] private Button freshButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI freshCostText;
        [SerializeField] private TextMeshProUGUI coinText;

        [Header("References")]
        [SerializeField] private AttributePageUI attributePage;
        
        private GameController _gameController;
        private PlayerRuntimeData _runtimeData;

        private void OnEnable()
        {
            if (nextWaveButton != null)
                nextWaveButton.onClick.AddListener(GoToNextWave);

            if (freshButton != null)
                freshButton.onClick.AddListener(FreshItems);
            EventBus.Subscribe<OnShopItemsGeneratedEvent>(ShowItems);
            RefreshCostText();
            RefreshCurrentCoins();
            
            BindRuntimeData();
        }

        private void OnDisable()
        {
            if (nextWaveButton != null)
                nextWaveButton.onClick.RemoveListener(GoToNextWave);

            if (freshButton != null)
                freshButton.onClick.RemoveListener(FreshItems);
            EventBus.Unsubscribe<OnShopItemsGeneratedEvent>(ShowItems);
            UnbindRuntimeData();
        }

        public void Configure(GameController gameController)
        {
            _gameController = gameController;
        }

        public void InitializeRun(PlayerRuntimeData runtimeData)
        {
            UnbindRuntimeData();
            _runtimeData = runtimeData;
            BindRuntimeData();
            RefreshCostText();
            RefreshCurrentCoins();
            var stat = _gameController.PlayerManager.Player.Stats;
            attributePage?.InitializeRun(stat);
        }

        public void ResetRun()
        {
            attributePage.ResetRun();
            UnbindRuntimeData();
            _runtimeData = null;
        }

        public void ShowItems(OnShopItemsGeneratedEvent eventData)
        {
            var items = eventData.ItemOptions;

            for (var index = 0; index < slots.Count; index++)
            {
                if (index < items.Count && items[index] != null)
                    slots[index].Show(items[index]);
                else if (slots[index] != null)
                    slots[index].gameObject.SetActive(false);
            }
        }

        public void FreshItems()
        {
            EventBus.Publish(new OnShopRefreshEvent());
        }

        public void GoToNextWave()
        {
            _gameController?.ChangeState(GamePhaseType.Battle);
        }

        private void BindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            _runtimeData.OnRefreshCostChanged += OnRefreshCostChanged;
            _runtimeData.OnCoinsChanged += OnCoinsChanged;
        }

        private void UnbindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            _runtimeData.OnRefreshCostChanged -= OnRefreshCostChanged;
            _runtimeData.OnCoinsChanged -= OnCoinsChanged;
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

            freshCostText.text = _runtimeData == null
                ? string.Empty
                : UIValueBuilder.Price(_runtimeData.RefreshCost);
        }

        private void RefreshCurrentCoins()
        {
            if (coinText == null)
                return;

            coinText.text = _runtimeData == null
                ? string.Empty
                : UIValueBuilder.Coin(_runtimeData.Coins);
        }
    }
}
