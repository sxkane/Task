using Enemy;
using Player;
using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        protected PlayerController Player;
        public int WeaponID { get; private set; }
        public WeaponStats Stats { get; private set; }
        public EnemyManager EnemyManager { get; private set; }

        private bool _isActive;
        
        public bool Activate() => _isActive = true;
        public bool Deactivate() => _isActive = false;

        public virtual void Init(PlayerController player, int weaponID, WeaponStats stats, EnemyManager enemyManager)
        {
            Player = player;
            WeaponID = weaponID;
            Stats = stats;
            EnemyManager = enemyManager;
        }
        
        protected virtual void Update()
        {
            if (!_isActive)
                return;
        }

        public void Upgrade(WeaponData data)
        {
            if (data.weaponID != WeaponID)
                return;
            
            if (!CanUpgrade())
                return;
            
            var nextStats =
                data.GetStats(Stats.rarity + 1);

            if (nextStats != null)
                Stats = nextStats;
        }

        public bool CanUpgrade()
        {
            return Stats != null && Stats.rarity != Rarity.Legendary;
        }

        public void SetOffset(Vector2 offset)
        {
            transform.position = 
                Player.transform.position + (Vector3)offset;
        }

        public void ExecuteEffect()
        {
            if (Stats.effects == null) return;
            
            foreach (var effect in Stats.effects)
            {
                effect.ExecuteEffect();
            }
        }
    }
}