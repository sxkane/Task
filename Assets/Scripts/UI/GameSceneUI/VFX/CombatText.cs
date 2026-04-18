using ObjectPool;
using TMPro;
using UnityEngine;

namespace UI.GameSceneUI.VFX
{
    public class CombatText : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float lifetime = 0.7f;
        [SerializeField] private float floatSpeed = 1.25f;

        private Color _baseColor;
        private float _remainingLifetime;
        private bool _initialized;

        public void Initialize(Vector3 worldPosition, string content, Color color)
        {
            transform.position = worldPosition;
            text.text = content;
            text.color = color;
            _baseColor = color;
            _remainingLifetime = lifetime;
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized)
                return;

            transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
            _remainingLifetime -= Time.deltaTime;

            if (text != null)
            {
                var color = _baseColor;
                color.a = Mathf.Clamp01(_remainingLifetime / lifetime);
                text.color = color;
            }

            if (_remainingLifetime <= 0f)
                PoolManager.Instance.Despawn(gameObject);
        }

        public void OnSpawned()
        {
            _initialized = false;
            _remainingLifetime = lifetime;
        }

        public void OnDespawned()
        {
            _initialized = false;
            _remainingLifetime = lifetime;
        }
    }
}
