using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemySpawningState : EnemyState
    {
        private float _timer;

        public EnemySpawningState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _timer = Enemy.SpawnActivationDelay;
            Enemy.EnterSpawningPhase();
            Enemy.Visual.Move(false);
        }

        public override void Update()
        {
            base.Update();

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                Enemy.FinishSpawn();
        }
    }
}
