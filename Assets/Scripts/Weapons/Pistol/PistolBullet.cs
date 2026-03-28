using Enemy;
using ObjectPool;
using Player;
using Stats;
using UnityEngine;

namespace Weapons.Pistol
{
    public class PistolBullet : MonoBehaviour
    {
        private Vector2 _dir;
        private float _speed;
        private PlayerController _player;
        private WeaponStats _stats;
        
        public void Init(float moveSpeed, Transform target, Vector2 defaultDir, 
            PlayerController player, WeaponStats stats)
        {
            if (target != null)
                _dir = target.position - transform.position;
            else
                _dir = defaultDir;
            
            _speed = moveSpeed;
            _player = player;
            _stats = stats;
            
            Invoke(nameof(ReturnToPool), moveSpeed);
        }

        public void Update()
        {
            transform.position += (Vector3)_dir * (_speed * Time.deltaTime);
        }
        
        private void ReturnToPool()
        {
            PoolManager.Instance.Despawn(gameObject);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                var playerStats = _player.Stats;
                int damage = DamageCalculator.CalculateBaseDamage(playerStats, _stats);

                if (Random.value < playerStats.CritChance + _stats.critChance)
                {
                    damage = Mathf.RoundToInt(damage * _stats.critDamage);
                }
                
                collision.GetComponent<EnemyController>().TakeDamage(damage);
                
                ReturnToPool();
            }
        }
    }
}