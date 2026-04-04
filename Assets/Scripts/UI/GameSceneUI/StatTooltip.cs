using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class StatTooltip : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform panel;
        [SerializeField] private TextMeshProUGUI text;

        [Header("Layout")]
        [SerializeField] private float verticalOffset = 18f;
        [SerializeField] private float horizontalPadding = 12f;
        [SerializeField] private float verticalPadding = 12f;
        
        private RectTransform _rootRect;

        private void Awake()
        {
            _rootRect = transform.parent as RectTransform;

            if (panel == null)
                panel = transform as RectTransform;
        }
        
        public void Show(string newText, RectTransform target)
        {
            if (target == null || panel == null || text == null || _rootRect == null)
                return;
            
            text.text = newText;
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            PositionTooltip(target);
        }

        public void Hide()
        {
            text.text = "";
            gameObject.SetActive(false);
        }

        private void PositionTooltip(RectTransform target)
        {
            transform.position = target.position + new Vector3(horizontalPadding, -verticalOffset, 0);
        }
    }
}
