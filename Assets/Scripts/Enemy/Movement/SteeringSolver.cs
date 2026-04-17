using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Movement
{
    public static class SteeringSolver
    {
        public static Vector2 Resolve(
            Rigidbody2D rigidbody,
            Vector2 desiredDirection,
            float moveSpeed,
            SteeringProfile profile,
            ContactFilter2D filter,
            List<Collider2D> results)
        {
            if (rigidbody == null)
                return Vector2.zero;

            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            var count = 0;

            results.Clear();
            Physics2D.OverlapCircle(
                rigidbody.position,
                profile.neighborRadius,
                filter,
                results);

            for (var i = 0; i < results.Count; i++)
            {
                var otherRb = results[i].attachedRigidbody;
                if (otherRb == null || otherRb == rigidbody)
                    continue;

                var diff = rigidbody.position - otherRb.position;
                var dist = diff.magnitude;
                if (dist <= 0.001f)
                    continue;

                separation += diff.normalized / dist;
                alignment += otherRb.linearVelocity;
                count++;
            }

            if (count > 0)
            {
                separation /= count;
                alignment /= count;
            }

            var finalDirection = desiredDirection
                                 + separation * profile.separationWeight
                                 + alignment.normalized * profile.alignmentWeight;

            if (finalDirection.sqrMagnitude > 0.001f)
                finalDirection.Normalize();

            return Vector2.Lerp(
                rigidbody.linearVelocity,
                finalDirection * moveSpeed,
                profile.velocityLerp);
        }
    }
}
