using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyMoveState : EnemyState
    {
        private Rigidbody2D _rb;
        private Transform _target;

        public EnemyMoveState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _rb = Enemy.Rigidbody;
            _target = Enemy.Target;
            Enemy.Visual.Move(true);
        }

        public override void Update()
        {
            base.Update();

            if (Enemy.Attack.UsesAttackState && Enemy.Attack.CanAttack())
                Machine.ChangeState(Enemy.AttackState);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_target == null)
                return;

            var navigationDirection = Enemy.Attack.GetMovementDirection(_rb.position, _target.position);
            var distanceToTarget = Vector2.Distance(_rb.position, _target.position);
            var speedMultiplier = Enemy.Attack.GetMovementSpeedMultiplier(distanceToTarget);

            if (Enemy.Attack.IgnoreSteering)
                Enemy.Motor.MoveRaw(navigationDirection, speedMultiplier);
            else
                Enemy.Motor.Move(navigationDirection, speedMultiplier);
        }

        public override void Exit()
        {
            base.Exit();
            Enemy.Visual.Move(false);
        }
    }
}
