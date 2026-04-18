using UnityEngine;
using ObjectPool;

namespace VFX
{
    public class ExplosionVfx : MonoBehaviour, IPoolable
    {
        [SerializeField] private ParticleSystem particleSystemMain;
        [SerializeField] private Transform optionalRing;
        [SerializeField] private float baseRadius = 1f;
        [SerializeField] private float lifetime = 1f;

        private Vector3 _defaultRingScale = Vector3.one;

        private void Awake()
        {
            if (optionalRing != null)
                _defaultRingScale = optionalRing.localScale;
        }

        public void Initialize(float radius)
        {
            CancelInvoke();

            var scale = baseRadius > 0.001f ? radius / baseRadius : 1f;

            if (optionalRing != null)
                optionalRing.localScale = _defaultRingScale * scale * 2f;

            if (particleSystemMain != null)
            {
                var shape = particleSystemMain.shape;
                shape.radius = radius;
                particleSystemMain.Play(true);
            }

            Invoke(nameof(ReturnToPool), lifetime);
        }

        public void OnSpawned()
        {
            CancelInvoke();
        }

        public void OnDespawned()
        {
            CancelInvoke();

            if (optionalRing != null)
                optionalRing.localScale = _defaultRingScale;

            if (particleSystemMain != null)
                particleSystemMain.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void ReturnToPool()
        {
            PoolManager.Instance.Despawn(gameObject);
        }
    }
}
