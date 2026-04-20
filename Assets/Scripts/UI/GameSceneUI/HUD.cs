using Core;
using Data.Text;
using Events;
using Events.PlayerEvents;
using Events.WaveEvents;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class HUD : MonoBehaviour
    {
        [Header("Combat")]
        [SerializeField] private TextMeshProUGUI gameTimerText;
        [SerializeField] private TextMeshProUGUI currentWaveText;
        [SerializeField] private TextMeshProUGUI totalWaveText;

        [Header("Hp")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Image hpFillImage;

        [Header("Progression")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider expBar;
        [SerializeField] private TextMeshProUGUI expText;

        private GameController _gameController;
        private PlayerController _player;
        private PlayerRuntimeData _runtimeData;

        private void OnEnable()
        {
            EventBus.Subscribe<WaveChangeSecondEvent>(UpdateGameTimerText);
            EventBus.Subscribe<WaveChangedEvent>(UpdateWaveText);
            EventBus.Subscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Subscribe<OnPlayerHealthChangedEvent>(OnPlayerHealthChanged);
            BindRuntimeData();
            RefreshView();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaveChangeSecondEvent>(UpdateGameTimerText);
            EventBus.Unsubscribe<WaveChangedEvent>(UpdateWaveText);
            EventBus.Unsubscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Unsubscribe<OnPlayerHealthChangedEvent>(OnPlayerHealthChanged);
            UnbindRuntimeData();
        }

        public void Configure(GameController gameController)
        {
            _gameController = gameController;
        }

        public void InitializeRun(PlayerController player)
        {
            UnbindRuntimeData();

            _player = player;
            _runtimeData = player != null ? player.RuntimeData : null;

            BindRuntimeData();
            RefreshHealth();
            RefreshProgression();
            RefreshWaveText();
        }

        public void ResetRun()
        {
            UnbindRuntimeData();
            _player = null;
            _runtimeData = null;
            RefreshWaveText();
        }

        public void RefreshView()
        {
            RefreshHealth();
            RefreshProgression();
            RefreshWaveText();
        }

        private void OnPlayerDamaged(OnPlayerDamagedEvent eventData)
        {
            if (_player == null || eventData.Target != _player)
                return;

            RefreshHealth();
        }

        private void OnPlayerHealthChanged(OnPlayerHealthChangedEvent eventData)
        {
            if (_player == null || eventData.Target != _player)
                return;

            RefreshHealth();
        }

        private void RefreshHealth()
        {
            if (_player == null || hpBar == null || hpFillImage == null)
                return;

            hpBar.maxValue = _player.MaxHp;
            hpBar.value = Mathf.Clamp(_player.CurrentHp, 0, _player.MaxHp);
            hpText.text = UIValueBuilder.Health(_player.CurrentHp, _player.MaxHp);

            var amount = _player.MaxHp <= 0 ? 0f : _player.CurrentHp / (float)_player.MaxHp;
            if (amount > 0.6f)
                hpFillImage.color = StatTextBuilder.Positive;
            else if (amount > 0.3f)
                hpFillImage.color = new Color(0.95f, 0.72f, 0.2f, 1f);
            else
                hpFillImage.color = StatTextBuilder.Negative;
        }

        private void RefreshProgression()
        {
            if (_runtimeData == null)
                return;

            if (coinText != null)
                coinText.text = UIValueBuilder.Coin(_runtimeData.Coins);

            if (levelText != null)
                levelText.text = UIValueBuilder.Level(_runtimeData.Level + 1);

            if (expBar != null)
            {
                expBar.maxValue = _runtimeData.NeedExperience;
                expBar.value = _runtimeData.Experience;
            }

            if (expText != null)
                expText.text = UIValueBuilder.Progress(_runtimeData.Experience, _runtimeData.NeedExperience);
        }

        private void UpdateGameTimerText(WaveChangeSecondEvent eventData)
        {
            if (gameTimerText != null)
                gameTimerText.text = UIValueBuilder.Timer(eventData.Timer);

            RefreshWaveText();
        }

        private void UpdateWaveText(WaveChangedEvent eventData)
        {
            if (currentWaveText != null)
                currentWaveText.text = "当前波次：" + Mathf.Max(0, eventData.CurrentWave);

            if (totalWaveText != null)
                totalWaveText.text = "总波次：" + Mathf.Max(0, eventData.TotalWaves);
        }

        private void RefreshWaveText()
        {
            var waveManager = _gameController != null ? _gameController.WaveManager : null;
            var currentWave = waveManager != null ? Mathf.Max(0, waveManager.CurrentWave + 1) : 0;
            var totalWaves = waveManager != null ? Mathf.Max(0, waveManager.TotalWaves) : 0;

            if (currentWaveText != null)
                currentWaveText.text = "当前波次：" + currentWave;

            if (totalWaveText != null)
                totalWaveText.text = "总波次：" + totalWaves;
        }

        private void BindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            _runtimeData.OnCoinsChanged += OnCoinsChanged;
            _runtimeData.OnExpChanged += OnExpChanged;
            _runtimeData.OnLevelUp += OnLevelUp;
        }

        private void UnbindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            _runtimeData.OnCoinsChanged -= OnCoinsChanged;
            _runtimeData.OnExpChanged -= OnExpChanged;
            _runtimeData.OnLevelUp -= OnLevelUp;
        }

        private void OnCoinsChanged(int amount)
        {
            RefreshProgression();
        }

        private void OnExpChanged(int exp, int needExp)
        {
            RefreshProgression();
        }

        private void OnLevelUp(int level)
        {
            RefreshProgression();
        }
    }
}
