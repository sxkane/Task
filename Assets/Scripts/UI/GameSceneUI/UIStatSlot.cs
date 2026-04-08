using Data;
using Player;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.GameSceneUI
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
        private GameUIManager _gameUIManager;

        public void Initialize()
        {
            _gameUIManager = GetComponentInParent<GameUIManager>();
            _stats = _gameUIManager.Player.Stats;
            Bind(_stats);
        }
        
        public void Bind(PlayerStats stats)
        {
            var stat = stats.GetStat(statType);
            stat.OnValueChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            attributeNameText.text = attributeName;
            attributeValueText.text = _stats.GetStatValue(statType).ToString("F1");
            _statIntroductionText = StatTextBuilder.BuildDescription(_stats, statType);
        }

        private void OnDestroy()
        {
            if (_stats != null)
            {
                var stat = _stats.GetStat(statType);
                stat.OnValueChanged -= Refresh;
            }
        }
        
        private void OnValidate()
        {
            transform.name = "Player Stat - " + statType;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            GameUIManager.Instance.tooltip.Show(_statIntroductionText, transform as RectTransform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameUIManager.Instance.tooltip.Hide();   
        }
    }
}
