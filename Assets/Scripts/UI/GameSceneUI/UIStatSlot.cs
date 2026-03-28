using Player;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class UIStatSlot: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string attributeName;
        [SerializeField] private StatType statType;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI attributeNameText;
        [SerializeField] private TextMeshProUGUI attributeValueText;
        private string _statIntroductionText;

        private PlayerStats _stat;
        private GameUIManager _gameUIManager;

        public void Initialize()
        {
            _gameUIManager = GetComponentInParent<GameUIManager>();
            _stat = _gameUIManager.Player.Stats;
            Bind(_stat);
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
            attributeValueText.text = _stat.GetStatValue(statType).ToString("F1");
            _statIntroductionText = _stat.GetStatInfo(statType);
        }

        private void OnDestroy()
        {
            if (_stat != null)
            {
                var stat = _stat.GetStat(statType);
                stat.OnValueChanged += Refresh;
            }
        }
        
        private void OnValidate()
        {
            transform.name = "Player Stat - " + statType;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _gameUIManager.statTooltip.Show(_statIntroductionText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _gameUIManager.statTooltip.Hide();   
        }
    }
}