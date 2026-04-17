using Enemy;
using Player;
using UnityEngine;
using Weapons.Effects;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        protected PlayerController Player { get; private set; }
        public WeaponEntry Entry { get; private set; }
        public int WeaponID { get; private set; }
        public WeaponStats Stats { get; private set; }
        public EnemyManager EnemyManager { get; private set; }
        protected Transform ProjectileRoot { get; private set; }
        public Vector2 Offset { get; private set; } = Vector2.zero;

        private bool _isActive;
        
        public void SetOffset(Vector2 offset)
        {
            Offset = offset;
            transform.position = Player.transform.position + (Vector3)offset;
        }

        protected virtual void Update()
        {
            if (!_isActive)
                return;
        }

        #region Weapon Loop

        public virtual void Configure(PlayerController player, WeaponEntry entry, EnemyManager enemyManager, Transform projectileRoot)
        {
            Player = player;
            Entry = entry;
            WeaponID = entry?.GetDataId() ?? -1;
            Stats = entry?.GetStats();
            EnemyManager = enemyManager;
            ProjectileRoot = projectileRoot;
        }

        public virtual void InitializeRun(WeaponEntry entry = null)
        {
            if (entry != null)
            {
                Entry = entry;
                WeaponID = entry.GetDataId();
                Stats = entry.GetStats();
            }
        }

        public virtual void ResetRun()
        {
            _isActive = false;
        }

        public virtual void CleanupRun()
        {
            _isActive = false;
        }

        public void BeginPhase()
        {
            _isActive = true;
        }

        public void EndPhase()
        {
            _isActive = false;
        }

        #endregion

        #region Weapon Upgrade

        public void Upgrade()
        {
            if (!CanUpgrade())
                return;

            var upgradedEntry = Entry.CreateUpgradedEntry();
            if (upgradedEntry == null)
                return;

            Entry = upgradedEntry;
            Stats = upgradedEntry.GetStats();
        }

        public bool CanUpgrade()
        {
            return Entry != null && Entry.CanUpgrade();
        }

        #endregion

        #region Weapon Effects

        public void ExecuteEffects(EffectTrigger trigger)
        {
            ExecuteEffects(trigger, EffectExecutionContext.ForWeapon(Player, this, EnemyManager));
        }

        public void ExecuteEffects(EffectTrigger trigger, EffectExecutionContext context)
        {
            if (Stats?.effects == null || context == null)
                return;

            foreach (var effect in Stats.effects)
            {
                if (effect == null)
                    continue;

                effect.Execute(context, trigger);
            }
        }

        #endregion
    }
}
