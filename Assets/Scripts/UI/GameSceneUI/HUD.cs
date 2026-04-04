using Core;
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
        
        [Header("Hp")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Image hpFillImage;

        [Header("Progression")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider expBar;
        [SerializeField] private TextMeshProUGUI expText;

        private PlayerController _player;
        private PlayerRuntimeData _runtimeData;

        private void OnEnable()
        {
            EventBus.Subscribe<WaveChangeSecondEvent>(UpdateGameTimerText);
            EventBus.Subscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaveChangeSecondEvent>(UpdateGameTimerText);
            EventBus.Unsubscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
            UnbindRuntimeData();
        }

        private void Update()
        {
            if (_player == null && GameController.Instance?.PlayerManager?.Player != null)
            {
                _player = GameController.Instance.PlayerManager.Player;
                _runtimeData = _player.RuntimeData;
                BindRuntimeData();
                RefreshHealth();
                RefreshProgression();
            }
        }

        private void OnPlayerDamaged(OnPlayerDamagedEvent e)
        {
            if (_player == null || e.Target != _player)
                return;
            
            RefreshHealth();
        }

        private void RefreshHealth()
        {
            if (_player == null || hpBar == null || hpFillImage == null)
                return;
            
            hpBar.maxValue = _player.MaxHp;
            hpBar.value = Mathf.Clamp(_player.CurrentHp, 0, _player.MaxHp);
            hpText.text = $"{_player.CurrentHp} / {_player.MaxHp}";

            var amount = _player.MaxHp <= 0 ? 0f : _player.CurrentHp / (float)_player.MaxHp;
            if (amount > 0.6f)
                hpFillImage.color = new Color(0.25f, 0.8f, 0.35f, 1f);
            else if (amount > 0.3f)
                hpFillImage.color = new Color(0.95f, 0.72f, 0.2f, 1f);
            else
                hpFillImage.color = new Color(0.88f, 0.22f, 0.22f, 1f);
        }

        private void RefreshProgression()
        {
            if (_runtimeData == null)
                return;

            if (coinText != null)
                coinText.text = "X " + _runtimeData.Coins.ToString();

            if (levelText != null)
                levelText.text = $"Lv.{_runtimeData.Level + 1}";

            if (expBar != null)
            {
                expBar.maxValue = _runtimeData.NeedExperience;
                expBar.value = _runtimeData.Experience;
            }

            if (expText != null)
                expText.text = $"{_runtimeData.Experience} / {_runtimeData.NeedExperience}";
        }

        private void UpdateGameTimerText(WaveChangeSecondEvent e)
        {
            if (gameTimerText != null)
                gameTimerText.text = e.Timer.ToString();
        }

        private void BindRuntimeData()
        {
            if (_runtimeData == null)
                return;

            UnbindRuntimeData();

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
