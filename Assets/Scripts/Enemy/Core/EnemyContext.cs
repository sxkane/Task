using UnityEngine;

namespace Enemy.Core
{
    public sealed class EnemyContext
    {
        public EnemyController Controller { get; }
        public Transform Transform { get; }
        public Rigidbody2D Rigidbody { get; }
        public Animator Animator { get; }
        public SpriteRenderer SpriteRenderer { get; }
        public Transform Target { get; }
        public EnemyManager EnemyManager { get; }
        public EnemyStats Stats { get; }

        public EnemyContext(
            EnemyController controller,
            Transform transform,
            Rigidbody2D rigidbody,
            Animator animator,
            SpriteRenderer spriteRenderer,
            Transform target,
            EnemyManager enemyManager,
            EnemyStats stats)
        {
            Controller = controller;
            Transform = transform;
            Rigidbody = rigidbody;
            Animator = animator;
            SpriteRenderer = spriteRenderer;
            Target = target;
            EnemyManager = enemyManager;
            Stats = stats;
        }
    }
}
