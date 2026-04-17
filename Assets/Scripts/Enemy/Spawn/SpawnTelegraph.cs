using System;
using System.Collections;
using ObjectPool;
using UnityEngine;

namespace Enemy.Spawn
{
    public class SpawnTelegraph : MonoBehaviour, IPoolable
    {
        [SerializeField] private GameObject visualRoot;
        [SerializeField] private SpriteRenderer[] flashRenderers;
        [SerializeField] private float duration = 0.8f;
        [SerializeField] private float startBlinkInterval = 0.18f;
        [SerializeField] private float endBlinkInterval = 0.04f;

        private Coroutine _routine;
        private Action _onComplete;

        public void Play(Action onComplete)
        {
            _onComplete = onComplete;

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayRoutine());
        }

        public void OnSpawned()
        {
            SetVisual(true);
        }

        public void OnDespawned()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _onComplete = null;
            SetVisual(true);
        }

        private IEnumerator PlayRoutine()
        {
            var elapsed = 0f;
            var visible = true;

            while (elapsed < duration)
            {
                var normalized = duration <= 0f ? 1f : elapsed / duration;
                var interval = Mathf.Lerp(startBlinkInterval, endBlinkInterval, normalized);
                visible = !visible;
                SetVisual(visible);
                yield return new WaitForSeconds(interval);
                elapsed += interval;
            }

            SetVisual(true);
            var onComplete = _onComplete;
            _onComplete = null;
            _routine = null;
            onComplete?.Invoke();
            PoolManager.Instance.Despawn(gameObject);
        }

        private void SetVisual(bool visible)
        {
            if (visualRoot != null && visualRoot != gameObject)
                visualRoot.SetActive(visible);

            if (flashRenderers == null)
                return;

            for (var i = 0; i < flashRenderers.Length; i++)
            {
                if (flashRenderers[i] != null)
                    flashRenderers[i].enabled = visible;
            }
        }
    }
}
