namespace Enemy.EnemyStates
{
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            if (!Enemy.Attack.CanAttack())
            {
                Machine.ChangeState(Enemy.MoveState);
                return;
            }

            Enemy.Attack.StartAttack();
            Enemy.Rigidbody.linearVelocity *= 0.1f;
        }
    }
}