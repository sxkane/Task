using Data;
using Data.Text;
using Player;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.GameSceneUI.Stats
{
    public class UIStatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Display")]
        [SerializeField] private string attributeName;
        [SerializeField] private StatType statType;
        [SerializeField] private TextMeshProUGUI attributeNameText;
        [SerializeField] private TextMeshProUGUI attributeValueText;

        private string _statIntroductionText;
        private PlayerStats _stats;
        private StatTooltip _statTooltip;

        public void InitializeRun(PlayerStats stats, StatTooltip tooltip)
        {
            ResetRun();

            _stats = stats;
            _statTooltip = tooltip;

            if (_stats == null)
                return;

            _stats.GetStat(statType).OnValueChanged += Refresh;
            Refresh();
        }

        public void ResetRun()
        {
            if (_stats != null)
                _stats.GetStat(statType).OnValueChanged -= Refresh;

            _stats = null;
            _statTooltip = null;
            _statIntroductionText = string.Empty;
        }

        private void Refresh()
        {
            if (_stats == null)
                return;

            attributeNameText.text = attributeName;
            attributeValueText.text = StatTextBuilder.BuildCurrentValue(_stats, statType);
            _statIntroductionText = StatTextBuilder.BuildTooltip(_stats, statType);
        }

        private void OnDestroy()
        {
            ResetRun();
        }

        private void OnValidate()
        {
            transform.name = "Player Stat - " + statType;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _statTooltip?.Show(_statIntroductionText, transform as RectTransform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _statTooltip?.Hide();
        }
    }
}
