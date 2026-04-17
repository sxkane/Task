using Events;
using Events.EnemyEvents;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy.UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider hpBar;
        [SerializeField] private Image hpFillImage;
        [SerializeField] private Vector3 worldOffset = new(0f, 1.2f, 0f);
        [SerializeField] private float visibleDuration = 2f;

        private EnemyController _enemy;
        private Transform _enemyTransform;
        private CanvasGroup _canvasGroup;
        private bool _initialize;
        private float _visibleTimer;

        public void Initialize(EnemyController enemy)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _enemy = enemy;
            _enemyTransform = enemy != null ? enemy.Transform : null;
            _initialize = _enemy != null;
            _visibleTimer = 0f;

            RefreshPosition();
            RefreshHealth();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnEnemyDamagedEvent>(OnEnemyDamaged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnEnemyDamagedEvent>(OnEnemyDamaged);
        }

        private void LateUpdate()
        {
            if (!_initialize || _enemy == null || _enemyTransform == null)
                return;

            _visibleTimer -= Time.deltaTime;
            RefreshPosition();
            RefreshHealth();
        }

        private void OnEnemyDamaged(OnEnemyDamagedEvent e)
        {
            if (!_initialize || _enemy == null || e.Target != _enemy)
                return;

            if (e.WasKilled)
            {
                _visibleTimer = 0f;
                return;
            }

            _visibleTimer = visibleDuration;
            RefreshHealth();
        }

        private void RefreshPosition()
        {
            if (_enemyTransform == null)
                return;

            transform.position = _enemyTransform.position + worldOffset;
            transform.rotation = Quaternion.identity;
        }

        private void RefreshHealth()
        {
            if (_enemy == null || _enemy.Stats == null || hpBar == null)
                return;

            var maxHp = Mathf.Max(1f, _enemy.Stats.MaxHP);
            var currentHp = Mathf.Clamp(_enemy.Stats.CurrentHP, 0f, maxHp);
            hpBar.maxValue = maxHp;
            hpBar.value = currentHp;

            if (hpFillImage != null)
            {
                var amount = currentHp / maxHp;
                hpFillImage.color = amount > 0.6f
                    ? new Color(0.31f, 0.84f, 0.37f, 1f)
                    : amount > 0.3f
                        ? new Color(0.95f, 0.72f, 0.2f, 1f)
                        : new Color(0.91f, 0.24f, 0.2f, 1f);
            }

            if (_canvasGroup != null)
                _canvasGroup.alpha = _visibleTimer > 0f ? 1f : 0f;
            else
                gameObject.SetActive(_visibleTimer > 0f);
        }
    }
}
