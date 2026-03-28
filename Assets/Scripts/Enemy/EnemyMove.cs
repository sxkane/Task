using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMove : MonoBehaviour
    {
        [Header("Neighbour Detection")]
        [SerializeField] private float neighborRadius = 0.8f;

        [Header("Boids Weights")]
        [SerializeField] private float separationWeight = 1.8f;
        [SerializeField] private float alignmentWeight = 0.4f;

        private Rigidbody2D _rb;
        private EnemyController _enemy;
        private EnemyStats _stats;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _enemy = GetComponent<EnemyController>();
            _stats = _enemy.Stats;
        }

        private void FixedUpdate()
        {
            if (_enemy.Target == null)
                return;

            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            int count = 0;

            var hits = Physics2D.OverlapCircleAll(transform.position, neighborRadius);

            foreach (var hit in hits)
            {
                var otherRb = hit.attachedRigidbody;

                if (otherRb == null || otherRb == _rb)
                    continue;

                Vector2 diff = _rb.position - otherRb.position;
                float dist = diff.magnitude;
                if (dist == 0) continue;

                // Separation
                separation += diff.normalized / dist;

                // Alignment
                alignment += otherRb.linearVelocity;

                count++;
            }

            if (count > 0)
            {
                separation /= count;
                alignment /= count;
            }

            Vector2 seek =
                ((Vector2)_enemy.Target.position - _rb.position).normalized;

            Vector2 finalDir =
                seek
                + separation * separationWeight
                + alignment.normalized * alignmentWeight;

            finalDir.Normalize();
            
            _rb.linearVelocity = Vector2.Lerp(
                _rb.linearVelocity,
                finalDir * _stats.MoveSpeed,
                0.25f);
        }
    }
}