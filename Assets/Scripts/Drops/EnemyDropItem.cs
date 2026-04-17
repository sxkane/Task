using ObjectPool;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Drops
{
    public class EnemyDropItem : MonoBehaviour, IPoolable
    {
        [Header("Motion")]
        [SerializeField] private float minLaunchSpeed = 1.8f;
        [SerializeField] private float maxLaunchSpeed = 3.4f;
        [SerializeField] private float settleDamping = 4f;
        [SerializeField] private float pickupDelay = 0.2f;
        [SerializeField] private float attractRadius = 3.5f;
        [SerializeField] private float pickupRadius = 0.45f;
        [SerializeField] private float attractSpeed = 7f;
        [SerializeField] private float forcedAttractSpeed = 32f;

        [Header("Optional Visuals")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float minVisualScale = 0.9f;
        [SerializeField] private float maxVisualScale = 1.15f;
        [SerializeField] private ParticleSystem spawnParticles;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [FormerlySerializedAs("randomSprites")]
        [SerializeField] private Sprite[] spriteVariants;

        private PlayerController _player;
        private PlayerRuntimeData _runtimeData;
        private Vector2 _velocity;
        private float _pickupDelayTimer;
        private int _coinAmount;
        private int _expAmount;
        private bool _initialize;
        private bool _forceAttract;
        private Vector3 _defaultVisualScale = Vector3.one;
        private Sprite _defaultSprite;

        private void Awake()
        {
            if (visualRoot != null)
                _defaultVisualScale = visualRoot.localScale;

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null)
                _defaultSprite = spriteRenderer.sprite;
        }

        private void Update()
        {
            if (!_initialize || _player == null || _runtimeData == null)
                return;

            _pickupDelayTimer -= Time.deltaTime;

            var currentPosition = (Vector2)transform.position;
            var targetPosition = (Vector2)_player.transform.position;
            var distanceToPlayer = Vector2.Distance(currentPosition, targetPosition);

            if ((_pickupDelayTimer <= 0f && distanceToPlayer <= attractRadius) || _forceAttract)
            {
                var direction = (targetPosition - currentPosition).normalized;
                var speed = _forceAttract ? forcedAttractSpeed : attractSpeed;
                _velocity = Vector2.Lerp(_velocity, direction * speed, 12f * Time.deltaTime);
            }
            else
            {
                _velocity = Vector2.Lerp(_velocity, Vector2.zero, settleDamping * Time.deltaTime);
            }

            transform.position += (Vector3)(_velocity * Time.deltaTime);

            if (_pickupDelayTimer <= 0f && distanceToPlayer <= pickupRadius)
                Collect();
        }

        public void Initialize(PlayerController player, int coinAmount, int expAmount)
        {
            _player = player;
            _runtimeData = player != null ? player.RuntimeData : null;
            _coinAmount = Mathf.Max(0, coinAmount);
            _expAmount = Mathf.Max(0, expAmount);
            _pickupDelayTimer = pickupDelay;
            _velocity = Random.insideUnitCircle.normalized * Random.Range(minLaunchSpeed, maxLaunchSpeed);
            _forceAttract = false;
            _initialize = _player != null && _runtimeData != null;

            if (visualRoot != null)
            {
                var scaleMultiplier = Random.Range(minVisualScale, maxVisualScale);
                visualRoot.localScale = _defaultVisualScale * scaleMultiplier;
                visualRoot.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            }

            ApplyRandomSprite();

            if (spawnParticles != null)
                spawnParticles.Play(true);
        }

        public void OnSpawned()
        {
            _initialize = false;
            _velocity = Vector2.zero;
            _pickupDelayTimer = pickupDelay;
            _coinAmount = 0;
            _expAmount = 0;
            _forceAttract = false;

            if (visualRoot != null)
            {
                visualRoot.localScale = _defaultVisualScale;
                visualRoot.localRotation = Quaternion.identity;
            }

            if (spriteRenderer != null)
                spriteRenderer.sprite = _defaultSprite;
        }

        public void OnDespawned()
        {
            _initialize = false;
            _player = null;
            _runtimeData = null;
            _velocity = Vector2.zero;
            _coinAmount = 0;
            _expAmount = 0;
            _forceAttract = false;

            if (visualRoot != null)
            {
                visualRoot.localScale = _defaultVisualScale;
                visualRoot.localRotation = Quaternion.identity;
            }

            if (spriteRenderer != null)
                spriteRenderer.sprite = _defaultSprite;

            if (spawnParticles != null)
                spawnParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void Collect()
        {
            if (_runtimeData == null)
            {
                PoolManager.Instance.Despawn(gameObject);
                return;
            }

            _runtimeData.AddCoins(_coinAmount);
            _runtimeData.AddExperience(_expAmount);

            PoolManager.Instance.Despawn(gameObject);
        }

        public void ForceAttract()
        {
            _pickupDelayTimer = 0f;
            _forceAttract = true;
        }

        private void ApplyRandomSprite()
        {
            if (spriteRenderer == null || spriteVariants == null || spriteVariants.Length == 0)
                return;

            var sprite = spriteVariants[Random.Range(0, spriteVariants.Length)];
            if (sprite != null)
                spriteRenderer.sprite = sprite;
        }
    }
}
