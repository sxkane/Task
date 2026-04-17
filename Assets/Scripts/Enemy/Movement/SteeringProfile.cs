using System;

namespace Enemy.Movement
{
    [Serializable]
    public class SteeringProfile
    {
        public float neighborRadius = 0.8f;
        public float separationWeight = 1.8f;
        public float alignmentWeight = 0.4f;
        public float velocityLerp = 0.25f;
    }
}
