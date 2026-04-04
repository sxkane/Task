using Events;
using UnityEngine;

namespace Core
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Bounds")]
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayerSpawnedEvent>(BindCamera);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnPlayerSpawnedEvent>(BindCamera);
        }
    
        private void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 targetPos = target.position;
            float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
            float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

            Vector3 desiredPos = new Vector3(clampedX, clampedY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        }

        private void BindCamera(OnPlayerSpawnedEvent e)
        {
            target = e.PlayerTransform;
        }
    }
}
