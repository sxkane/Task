using System.Collections;
using Events;
using Events.EnemyEvents;
using ObjectPool;
using UnityEngine;

namespace Enemy
{
    public class EnemyVisual : MonoBehaviour, IPoolable
    {
        private static readonly Color HurtColor = new(1f, 0.28f, 0.28f, 1f);
        private static readonly Color BuffOutlineColor = new(1f, 0.2f, 0.2f, 1f);
        private static readonly int MoveString = Animator.StringToHash("Move");
        private static readonly int DeadString = Animator.StringToHash("Dead");

        [Header("Buff Outline")]
        [SerializeField] private GameObject buffOutlineRoot;
        [SerializeField] private SpriteRenderer[] buffOutlineRenderers;

        [Header("Damage VFX")]
        [SerializeField] private string deathStateName = "Dead";

        private EnemyController _enemy;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer[] _allSpriteRenderers;
        private Coroutine _flashRoutine;
        private CachedPose[] _cachedPoses;
        private bool _cachedPoseInitialized;
        private Vector3 _defaultLocalScale;
        private bool _initialize;

        private struct CachedPose
        {
            public Transform Transform;
            public Vector3 LocalPosition;
            public Quaternion LocalRotation;
        }

        private void Awake()
        {
            CacheInitialPose();
            _allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            _defaultLocalScale = transform.localScale;
            if (_defaultLocalScale == Vector3.zero)
                _defaultLocalScale = Vector3.one;
        }

        private void Update()
        {
            if (!_initialize)
                return;

            if (_enemy == null || _enemy.Target == null || _enemy.Lifecycle == null || !_enemy.Lifecycle.IsActive)
                return;

            var dirX = _enemy.Target.position.x - _enemy.Transform.position.x;
            
            if (dirX > 0.02f)
                SetFacingRight(true);
            else if (dirX < -0.02f)
                SetFacingRight(false);
        }

        public void Move(bool flag)
        {
            _animator.SetBool(MoveString, flag);
        }

        public void PlayDeath()
        {
            if (_animator == null)
                return;

            _animator.ResetTrigger(DeadString);
            _animator.SetBool(MoveString, false);
            _animator.SetTrigger(DeadString);
            if (!string.IsNullOrWhiteSpace(deathStateName))
                _animator.Play(deathStateName, 0, 0f);
            _animator.Update(0f);
        }

        public bool IsPlayingDeathState()
        {
            if (_animator == null || string.IsNullOrWhiteSpace(deathStateName))
                return false;

            return _animator.GetCurrentAnimatorStateInfo(0).IsName(deathStateName);
        }

        public bool HasDeathAnimationCompleted()
        {
            if (_animator == null || string.IsNullOrWhiteSpace(deathStateName))
                return false;

            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(deathStateName) && stateInfo.normalizedTime >= 0.98f;
        }

        public void SetBuffOutline(bool buffEnabled)
        {
            if (buffOutlineRoot != null)
                buffOutlineRoot.SetActive(buffEnabled);

            if (buffOutlineRenderers == null)
                return;

            for (var i = 0; i < buffOutlineRenderers.Length; i++)
            {
                if (buffOutlineRenderers[i] == null)
                    continue;

                buffOutlineRenderers[i].enabled = buffEnabled;
                buffOutlineRenderers[i].color = BuffOutlineColor;
            }
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
            var original = _spriteRenderer.color;
            const float flashDuration = 0.08f;
            const float lerpSpeed = 10f;

            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                _spriteRenderer.color = Color.Lerp(original, HurtColor, t);
                yield return null;
            }

            _spriteRenderer.color = HurtColor;
            yield return new WaitForSeconds(flashDuration);

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                _spriteRenderer.color = Color.Lerp(HurtColor, original, t);
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

            ResetPose();

            if (_animator != null)
            {
                _animator.Rebind();
                _animator.Update(0f);
                _animator.SetBool(MoveString, false);
            }

            if (_spriteRenderer != null)
                _spriteRenderer.color = Color.white;

            ResetRuntimeVisuals();

            _initialize = true;
        }

        public void OnSpawned()
        {
            ResetPose();
            ResetRuntimeVisuals();
        }

        public void OnDespawned()
        {
            ResetPose();
            ResetRuntimeVisuals();
        }

        private void CacheInitialPose()
        {
            if (_cachedPoseInitialized)
                return;

            var transforms = GetComponentsInChildren<Transform>(true);
            _cachedPoses = new CachedPose[transforms.Length];

            for (var i = 0; i < transforms.Length; i++)
            {
                _cachedPoses[i] = new CachedPose
                {
                    Transform = transforms[i],
                    LocalPosition = transforms[i].localPosition,
                    LocalRotation = transforms[i].localRotation
                };
            }

            _cachedPoseInitialized = true;
        }

        private void ResetPose()
        {
            CacheInitialPose();

            if (_cachedPoses == null)
                return;

            for (var i = 0; i < _cachedPoses.Length; i++)
            {
                if (_cachedPoses[i].Transform == null)
                    continue;

                _cachedPoses[i].Transform.localPosition = _cachedPoses[i].LocalPosition;
                _cachedPoses[i].Transform.localRotation = _cachedPoses[i].LocalRotation;
            }

            transform.localScale = _defaultLocalScale;
        }

        private void SetFacingRight(bool facingRight)
        {
            if (_allSpriteRenderers == null)
                _allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            var flipX = !facingRight;
            for (var i = 0; i < _allSpriteRenderers.Length; i++)
            {
                if (_allSpriteRenderers[i] != null)
                    _allSpriteRenderers[i].flipX = flipX;
            }
        }

        private void ResetRuntimeVisuals()
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
                _flashRoutine = null;
            }

            if (_spriteRenderer != null)
                _spriteRenderer.color = Color.white;

            SetBuffOutline(false);
            SetFacingRight(true);
        }
    }
}
