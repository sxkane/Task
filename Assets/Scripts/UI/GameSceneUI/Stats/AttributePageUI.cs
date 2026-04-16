using Player;
using UnityEngine;

namespace UI.GameSceneUI.Stats
{
    public class AttributePageUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIStatSlot[] slots;
        [SerializeField] private StatTooltip tooltip;

        public void InitializeRun(PlayerStats stats)
        {
            
            if (tooltip != null)
                tooltip.Hide();

            if (slots == null)
                return;

            foreach (var statSlot in slots)
                statSlot.InitializeRun(stats, tooltip);
        }

        public void ResetRun()
        {
            if (tooltip != null)
                tooltip.Hide();

            if (slots == null)
                return;

            foreach (var statSlot in slots)
                statSlot.ResetRun();
        }
    }
}
