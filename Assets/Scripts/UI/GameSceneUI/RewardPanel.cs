using System;
using System.Collections.Generic;
using Core;
using Events;
using GameFlow;
using Rewards.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class RewardPanel : MonoBehaviour
    {
        [SerializeField] private List<RewardSlot> slots;
        [SerializeField] private Button nextWaveButton;
        [SerializeField] private Button freshButton;
        
        public void ShowItems(OnShopItemsGeneratedEvent e)
        {
            var items = e.ItemOptions;
            for (int i = 0; i < items.Count; i++)
            {
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
        }
        
        private void OnDisable()
        {
            nextWaveButton.onClick.RemoveListener(GoToNextWave);
        }
    }
}