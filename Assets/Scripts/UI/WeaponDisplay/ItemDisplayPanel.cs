using Data.Text;
using Items;
using TMPro;
using UnityEngine;

namespace UI.WeaponDisplay
{
    public class ItemDisplayPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDescription;

        private void Awake()
        {
            ResolveReferences();
        }

        public void Show(ItemData item)
        {
            Show(item, null, Vector2.zero);
        }

        public void Show(ItemData item, RectTransform target, Vector2 offset)
        {
            ResolveReferences();
            if (item == null)
            {
                Hide();
                return;
            }

            if (itemName != null)
                itemName.text = item.GetDisplayName();

            if (itemDescription != null)
                itemDescription.text = GameTextBuilder.BuildItem(item);

            if (target != null)
                SetPositionBesideTarget(target, offset);

            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SetPositionBesideTarget(RectTransform target, Vector2 offset)
        {
            var selfRect = transform as RectTransform;
            if (selfRect == null || selfRect.parent == null)
                return;

            var parentRect = selfRect.parent as RectTransform;
            if (parentRect == null)
                return;

            var rootCanvas = parentRect.GetComponentInParent<Canvas>();
            var camera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? rootCanvas.worldCamera
                : null;

            var worldCorners = new Vector3[4];
            target.GetWorldCorners(worldCorners);
            var anchorWorldPos = (worldCorners[2] + worldCorners[3]) * 0.5f;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, anchorWorldPos);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, camera, out var localPoint))
                selfRect.anchoredPosition = localPoint + offset;
        }

        private void ResolveReferences()
        {
            if (itemName != null && itemDescription != null)
                return;

            var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length > 0 && itemName == null)
                itemName = texts[0];
            if (texts.Length > 1 && itemDescription == null)
                itemDescription = texts[1];
        }
    }
}
