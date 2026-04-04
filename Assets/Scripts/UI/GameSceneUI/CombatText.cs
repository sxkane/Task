using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.GameSceneUI
{
    public class CombatText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float lifetime = 0.7f;
        [SerializeField] private float floatSpeed = 1.25f;
        
        private Color _baseColor;
        
        private bool _initialized;

        public void Initialize(Vector3 worldPosition, string content, Color color)
        {
            transform.position = worldPosition;
            text.text = content;
            text.color = color;

            _baseColor = color;
            
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized)
                return;
            
            transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
            lifetime -= Time.deltaTime;

            if (text != null)
            {
                var color = _baseColor;
                color.a = Mathf.Clamp01(lifetime / 0.7f);
                text.color = color;
            }

            if (lifetime <= 0f)
                Destroy(gameObject);
        }
    }
}
