using UnityEngine;

namespace Enemy
{
    public class EnemyVisual : MonoBehaviour
    {
        private EnemyController _enemy;

        private void Start()
        {
            _enemy = GetComponentInParent<EnemyController>();
        }

        void Update()
        {
            if (_enemy.Target == null) return;

            float dirX = _enemy.Target.position.x - transform.position.x;

            transform.localScale = new Vector3(
                dirX >= 0 ? 1 : -1,
                1,
                1);
        }
    }
}