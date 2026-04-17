using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyDyingState : EnemyState
    {
        private float _timer;
        private bool _enteredDeathState;

        public EnemyDyingState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _timer = Enemy.DeathDespawnDelay;
            _enteredDeathState = false;
            Enemy.EnterDyingPhase();
            Enemy.Attack.CancelAttack();
            Enemy.Visual.Move(false);
            Enemy.Visual.PlayDeath();
            Enemy.Motor.Stop(0f);
        }

        public override void Update()
        {
            base.Update();

            if (Enemy.Visual != null && Enemy.Visual.IsPlayingDeathState())
                _enteredDeathState = true;

            if (_enteredDeathState && Enemy.Visual != null)
            {
                if (Enemy.Visual.HasDeathAnimationCompleted() || !Enemy.Visual.IsPlayingDeathState())
                {
                    Enemy.MarkDeathAnimationFinished();
                    Enemy.FinishDeath();
                    return;
                }
            }

            if (Enemy.IsDeathAnimationFinished())
            {
                Enemy.FinishDeath();
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                Enemy.FinishDeath();
        }
    }
}
