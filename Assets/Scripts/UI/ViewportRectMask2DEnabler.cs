using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ViewportRectMask2DEnabler : MonoBehaviour
    {
        private Mask _mask;
        private RectMask2D _rectMask;

        private void Awake()
        {
            EnsureViewportMask();
        }

        private void OnEnable()
        {
            EnsureViewportMask();
        }

        private void EnsureViewportMask()
        {
            _mask = _mask != null ? _mask : GetComponent<Mask>();
            if (_mask != null)
                _mask.enabled = false;

            _rectMask = _rectMask != null ? _rectMask : GetComponent<RectMask2D>();
            if (_rectMask == null)
                _rectMask = gameObject.AddComponent<RectMask2D>();

            _rectMask.enabled = true;
        }
    }
}
