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
        private SpriteRenderer _spriteRenderer;

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
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
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
            Color original = _spriteRenderer.color;

            float flashDuration = 0.08f;
            float lerpSpeed = 10f;
            
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                _spriteRenderer.color = Color.Lerp(original, FlashColor, t);
                yield return null;
            }

            _spriteRenderer.color = FlashColor;

            yield return new WaitForSeconds(flashDuration);
            
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                _spriteRenderer.color = Color.Lerp(FlashColor, original, t);
                yield return null;
            }

            _spriteRenderer.color = original;
            _flashRoutine = null;
        }
    }
}
