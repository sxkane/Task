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
            _attack.DoAttack();
        }

        public void AttackEnd()
        {
            _attack.EndAttack();
            _enemy.ChangeState(EnemyStateEnum.Move);
        }
    }
}