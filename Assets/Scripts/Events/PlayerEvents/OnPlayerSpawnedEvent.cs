using UnityEngine;

namespace Events
{
    public class OnPlayerSpawnedEvent : IEvent
    {
        public Transform PlayerTransform { get; private set; }

        public OnPlayerSpawnedEvent(Transform playerTransform)
        {
            PlayerTransform = playerTransform;
        }
    }
}