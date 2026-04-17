using UnityEngine;

namespace Enemy.Movement
{
    public static class EnemyWorldBounds
    {
        public static bool IsConfigured { get; private set; }
        public static Vector2 Min { get; private set; }
        public static Vector2 Max { get; private set; }

        public static void Configure(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
            IsConfigured = true;
        }

        public static Vector2 Clamp(Vector2 position)
        {
            if (!IsConfigured)
                return position;

            return new Vector2(
                Mathf.Clamp(position.x, Min.x, Max.x),
                Mathf.Clamp(position.y, Min.y, Max.y));
        }

        public static Vector2 ClampDirection(Vector2 position, Vector2 desiredDirection, float skin = 0.05f)
        {
            if (!IsConfigured || desiredDirection.sqrMagnitude <= 0.0001f)
                return desiredDirection;

            var direction = desiredDirection;

            if (position.x <= Min.x + skin && direction.x < 0f)
                direction.x = 0f;
            else if (position.x >= Max.x - skin && direction.x > 0f)
                direction.x = 0f;

            if (position.y <= Min.y + skin && direction.y < 0f)
                direction.y = 0f;
            else if (position.y >= Max.y - skin && direction.y > 0f)
                direction.y = 0f;

            return direction.sqrMagnitude <= 0.0001f ? Vector2.zero : direction.normalized;
        }
    }
}
