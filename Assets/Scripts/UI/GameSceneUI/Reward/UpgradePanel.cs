using System.Collections.Generic;
using Core;
using Events;
using Events.UpgradeEvents;
using Player;
using TMPro;
using UI.GameSceneUI.Stats;
using UnityEngine;

namespace UI.GameSceneUI.Reward
{
    public class UpgradePanel : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI remainingText;

        [Header("Slots")]
        [SerializeField] private List<UpgradeSlot> slots;
        
        [Header("References")]
        [SerializeField] private AttributePageUI attributePage;

        private PlayerController _player;
        private PlayerRuntimeData _runtimeData;

        private void OnEnable()
        {
            EventBus.Subscribe<OnUpgradeOptionsGeneratedEvent>(ShowOptions);
            RefreshRemainingSelections();
            BindRuntimeData();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnUpgradeOptionsGeneratedEvent>(ShowOptions);
            UnbindRuntimeData();
        }

        public void Configure(GameController gameController)
        {
        }

        public void InitializeRun(PlayerController player)
        {
            UnbindRuntimeData();
            _player = player;
            _runtimeData = player.RuntimeData;
            BindRuntimeData();
            RefreshRemainingSelections();
            attributePage.InitializeRun(_player.Stats);
        }

        public void ResetRun()
        {
            attributePage.ResetRun();
            UnbindRuntimeData();
            _runtimeData = null;
        }

        private void ShowOptions(OnUpgradeOptionsGeneratedEvent eventData)
        {
            if (titleText != null)
                titleText.text = "升级";

            if (remainingText != null)
                remainingText.text = $"剩余升级数量: {eventData.RemainingSelections}";

            for (var index = 0; index < slots.Count; index++)
            {
                if (index < eventData.Options.Count && eventData.Options[index] != null)
                    slots[index].Show(eventData.Options[index]);
                else if (slots[index] != null)
                    slots[index].gameObject.SetActive(false);
            }
        }

        private void BindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            _runtimeData.OnPendingUpgradeSelectionsChanged += OnPendingSelectionsChanged;
        }

        private void UnbindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            _runtimeData.OnPendingUpgradeSelectionsChanged -= OnPendingSelectionsChanged;
        }

        private void OnPendingSelectionsChanged(int remainingSelections)
        {
            RefreshRemainingSelections();
        }

        private void RefreshRemainingSelections()
        {
            if (remainingText == null)
                return;

            remainingText.text = _runtimeData == null
                ? string.Empty
                : $"Remaining: {_runtimeData.PendingUpgradeSelections}";
        }
    }
}
