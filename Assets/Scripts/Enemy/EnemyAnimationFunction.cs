using Enemy.Attack;
using Enemy.EnemyStates;
using UnityEngine;

namespace Enemy
{
    public class EnemyAnimationFunction : MonoBehaviour
    {
        private EnemyController _enemy;
        private EnemyAttack _attack;
        
        public void InitializeAnimationFunction(EnemyController enemy)
        {
            _enemy = enemy;
            _attack = _enemy.Attack;
        }

        public void EnemyAttack()
        {
            if (_enemy == null || _enemy.Lifecycle == null || !_enemy.Lifecycle.IsActive)
                return;

            _attack.DoAttack();
        }

        public void AttackEnd()
        {
            if (_enemy == null || _enemy.Lifecycle == null || !_enemy.Lifecycle.IsActive)
            {
                _attack.EndAttack();
                return;
            }

            _attack.EndAttack();
            if (_attack.UsesAttackState && _enemy != null && _enemy.Lifecycle != null && _enemy.Lifecycle.IsActive)
                _enemy.ChangeState(EnemyStateEnum.Move);
        }

        public void DeathEnd()
        {
            _enemy.MarkDeathAnimationFinished();
            _enemy.FinishDeath();
        }
    }
}
