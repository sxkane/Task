using System.Collections.Generic;
using Data;
using Data.Text;
using Events;
using Events.DisplayEvent;
using Events.UpgradeEvents;
using Player;
using Rewards.StatRewards;
using Stats;
using UnityEngine;
using Waves;

namespace Rewards.Upgrades
{
    public class UpgradeManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int optionCount = 4;
        [SerializeField] private StatIconDatabase statIconDatabase;

        private PlayerManager _playerManager;
        private WaveManager _waveManager;
        private PlayerController _player;
        private readonly List<RewardOption> _currentOptions = new();

        public event System.Action SequenceCompleted;

        public void Configure(PlayerManager playerManager, WaveManager waveManager)
        {
            _playerManager = playerManager;
            _waveManager = waveManager;
        }

        public void InitializeRun()
        {
            _player = _playerManager != null ? _playerManager.Player : null;
            _currentOptions.Clear();
        }

        public void ResetRun()
        {
            EndPhase();
            _currentOptions.Clear();
            _player = null;
        }

        public bool HasPendingSelections()
        {
            return _player != null
                   && _player.RuntimeData != null
                   && _player.RuntimeData.HasPendingUpgradeSelections();
        }

        public void BeginPhase()
        {
            EventBus.Subscribe<OnUpgradeOptionSelectedEvent>(HandleUpgradeSelected);
            PublishNextOptions();
        }

        public void EndPhase()
        {
            EventBus.Unsubscribe<OnUpgradeOptionSelectedEvent>(HandleUpgradeSelected);
        }

        private void PublishNextOptions()
        {
            if (_player == null || _player.RuntimeData == null)
            {
                SequenceCompleted?.Invoke();
                return;
            }

            if (!_player.RuntimeData.HasPendingUpgradeSelections())
            {
                SequenceCompleted?.Invoke();
                return;
            }

            _currentOptions.Clear();
            var selectedStatTypes = new HashSet<StatType>();

            for (var index = 0; index < optionCount; index++)
            {
                var option = ItemGenerator.GetUpgradeOption(
                    GetCurrentWaveIndex(),
                    _player.Stats.Luck,
                    statIconDatabase,
                    selectedStatTypes);

                option.title = UpgradeTextBuilder.BuildTitle(option.reward);
                option.description = UpgradeTextBuilder.BuildDescription(option.reward);
                _currentOptions.Add(option);
                selectedStatTypes.Add(option.reward.type);
            }

            EventBus.Publish(new OnUpgradeOptionsGeneratedEvent(
                new List<RewardOption>(_currentOptions),
                _player.RuntimeData.PendingUpgradeSelections));
        }

        private void HandleUpgradeSelected(OnUpgradeOptionSelectedEvent eventData)
        {
            if (_player == null || _player.RuntimeData == null || eventData.Option?.reward == null)
                return;

            ApplyReward(eventData.Option.reward);
            if (!_player.RuntimeData.TryConsumePendingUpgradeSelection())
                return;

            PublishNextOptions();
        }

        private void ApplyReward(StatReward reward)
        {
            var stat = _player.Stats.GetStat(reward.type);
            stat.AddModifier(StatValueUtility.CreatePlayerModifier(reward.type, reward.value, StatModType.Flat, reward));
        }

        private int GetCurrentWaveIndex()
        {
            return _waveManager != null ? _waveManager.CurrentWave + 1 : 1;
        }
    }
}
