using System.Collections;
using Events;
using Events.PlayerEvents;
using UI.GameSceneUI;
using UnityEngine;

namespace Player
{
    public class PlayerVisual : MonoBehaviour
    {
        private static readonly Color FlashColor = new(1f, 0.35f, 0.35f, 1f);

        private PlayerController _player;
        private Animator _animator;
        private SpriteRenderer[] _spriteRenderers;
        private Color[] _baseColors;

        private bool _initialized;
        private Coroutine _flashRoutine;
        
        private void Update()
        {
            if (!_initialized)
                return;
            
            transform.localScale = new Vector3(
                _player.FacingRight ? 1 : -1,
                1, 1);
        
            bool moving = _player.Input.MoveInput != Vector2.zero;
            _animator.SetBool("Move",  moving);
        }

        public void Initialize()
        {
            _player = GetComponentInParent<PlayerController>();
            _animator = GetComponent<Animator>();
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            _baseColors = new Color[_spriteRenderers.Length];

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _baseColors[i] = _spriteRenderers[i].color;
            }
            
            _initialized = true;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
        }

        private void OnPlayerDamaged(OnPlayerDamagedEvent e)
        {
            if (!_initialized || e.Target != _player)
                return;

            if (!e.IsDodged)
            {
                if (_flashRoutine != null)
                    StopCoroutine(_flashRoutine);

                _flashRoutine = StartCoroutine(FlashRoutine());
            }
        }

        private IEnumerator FlashRoutine()
        {
            SetSpritesColor(FlashColor);
            yield return new WaitForSeconds(0.08f);
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
