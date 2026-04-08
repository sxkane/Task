using System.Collections;
using Events;
using Events.EnemyEvents;
using UI.GameSceneUI;
using Unity.Properties;
using UnityEngine;

namespace Enemy
{
    public class EnemyVisual : MonoBehaviour
    {
        private static readonly Color FlashColor = new(1f, 1f, 1f, 1f);
        private static readonly int MoveString = Animator.StringToHash("Move");

        private EnemyController _enemy;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Coroutine _flashRoutine;
        
        private bool _initialize;

        void Update()
        {
            if (!_initialize)
                return;
            
            if (_enemy == null || _enemy.Target == null) return;

            float dirX = _enemy.Target.position.x - transform.position.x;

            transform.localScale = new Vector3(
                dirX >= 0 ? 1 : -1,
                1,
                1);
        }
        
        public void Move(bool flag)
        {
            _animator.SetBool(MoveString, flag);
        }
        
        private void OnEnable()
        {
            EventBus.Subscribe<OnEnemyDamagedEvent>(OnEnemyDamaged);
        }

        private void OnDisable()
        {
            _initialize = false;
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

        public void Initialize(EnemyController enemy)
        {
            _enemy = enemy;
            _animator = _enemy.Animator;
            _spriteRenderer = _enemy.SpriteRenderer; 
            
            _initialize = true;
        }
    }
}
