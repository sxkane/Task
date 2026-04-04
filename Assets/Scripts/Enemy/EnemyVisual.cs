using System.Collections;
using Events;
using Events.EnemyEvents;
using UI.GameSceneUI;
using UnityEngine;

namespace Enemy
{
    public class EnemyVisual : MonoBehaviour
    {
        private static readonly Color FlashColor = new(1f, 1f, 1f, 1f);

        private EnemyController _enemy;
        private SpriteRenderer[] _spriteRenderers;
        private Color[] _baseColors;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _enemy = GetComponentInParent<EnemyController>();
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            _baseColors = new Color[_spriteRenderers.Length];

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _baseColors[i] = _spriteRenderers[i].color;
            }
        }

        void Update()
        {
            if (_enemy == null || _enemy.Target == null) return;

            float dirX = _enemy.Target.position.x - transform.position.x;

            transform.localScale = new Vector3(
                dirX >= 0 ? 1 : -1,
                1,
                1);
        }
        
        private void OnEnable()
        {
            EventBus.Subscribe<OnEnemyDamagedEvent>(OnEnemyDamaged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnEnemyDamagedEvent>(OnEnemyDamaged);
        }

        private void OnEnemyDamaged(OnEnemyDamagedEvent e)
        {
            if (_enemy == null || e.Target != _enemy)
                return;
            
            if (_flashRoutine != null)
                StopCoroutine(_flashRoutine);

            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            SetSpritesColor(FlashColor);
            yield return new WaitForSeconds(0.06f);
            RestoreSpritesColor();
            _flashRoutine = null;
        }

        private void SetSpritesColor(Color color)
        {
            foreach (var spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = color;
            }
        }

        private void RestoreSpritesColor()
        {
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] != null)
                    _spriteRenderers[i].color = _baseColors[i];
            }
        }
    }
}
