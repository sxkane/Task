using System.Collections.Generic;
using System.Text;
using Events;
using Events.WeaponEvents;
using TMPro;
using UI.GameSceneUI.Reward;
using UnityEngine;
using UnityEngine.EventSystems;
using Weapons;

namespace UI.GameSceneUI.IconSlots
{
    public class BonusDataSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private RectTransform hoverTarget;
        [SerializeField] private Vector2 panelOffset = new(24f, 0f);

        private List<WeaponSetBonusData> _bonusList;
        private IReadOnlyDictionary<WeaponSetBonusData, int> _bonusCounts;
        private BonusPanel _bonusPanel;

        public bool ShouldHandleHover { get; private set; }

        private void Awake()
        {
            if (hoverTarget == null)
                hoverTarget = transform as RectTransform;
        }

        public void Configure(List<WeaponSetBonusData> bonus = null, BonusPanel panel = null)
        {
            _bonusPanel = panel;

            if (bonus == null || bonus.Count == 0)
            {
                _bonusList = null;
                ShouldHandleHover = false;
                if (title != null)
                    title.SetText("道具");
                return;
            }

            _bonusList = bonus;
            ShouldHandleHover = _bonusPanel != null;

            if (title == null)
                return;

            var sb = new StringBuilder();
            for (var i = 0; i < bonus.Count; i++)
            {
                var entry = bonus[i];
                if (entry == null)
                    continue;

                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(entry.DisplayName);
            }

            title.text = sb.ToString();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnWeaponChanged>(OnWeaponChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnWeaponChanged>(OnWeaponChanged);
            _bonusPanel?.Hide();
        }

        private void OnWeaponChanged(OnWeaponChanged eventData)
        {
            _bonusCounts = eventData.BonusCount;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!ShouldHandleHover || _bonusPanel == null || _bonusList == null || _bonusList.Count == 0 || hoverTarget == null)
                return;

            _bonusPanel.Show(_bonusList, _bonusCounts, hoverTarget, panelOffset);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!ShouldHandleHover || _bonusPanel == null)
                return;

            _bonusPanel.Hide();
        }
    }
}
