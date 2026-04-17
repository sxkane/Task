using UnityEngine;

namespace Enemy.Attack
{
    public class GiantSnakeAttack : SlimeAttack
    {
        [Header("Ramp")]
        [SerializeField] private float moveSpeedIncreasePerSecond = 0.12f;
        [SerializeField] private float maxMoveSpeedMultiplier = 3.5f;

        private float _aliveTime;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _aliveTime = 0f;
        }

        protected override void Update()
        {
            base.Update();

            if (Enemy == null || Enemy.Lifecycle == null || !Enemy.Lifecycle.IsActive)
                return;

            _aliveTime += Time.deltaTime;
        }

        public override float GetMovementSpeedMultiplier(float distanceToTarget)
        {
            var multiplier = 1f + _aliveTime * moveSpeedIncreasePerSecond;
            return Mathf.Min(maxMoveSpeedMultiplier, multiplier);
        }
    }
}
