using Enemy;
using Player;
using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        protected PlayerController Player;
        public WeaponLoadoutEntry Entry { get; private set; }
        public int WeaponID { get; private set; }
        public WeaponStats Stats { get; private set; }
        public EnemyManager EnemyManager { get; private set; }

        private bool _isActive;
        
        public bool BeginPhase() => _isActive = true;
        public bool EndPhase() => _isActive = false;

        public virtual void Configure(PlayerController player, WeaponLoadoutEntry entry, EnemyManager enemyManager)
        {
            Player = player;
            Entry = entry;
            WeaponID = entry != null ? entry.GetDataId() : -1;
            Stats = entry != null ? entry.GetStats() : null;
            EnemyManager = enemyManager;
        }

        public virtual void InitializeRun(WeaponLoadoutEntry runtimeEntry = null)
        {
            if (runtimeEntry != null)
            {
                Entry = runtimeEntry;
                WeaponID = runtimeEntry.GetDataId();
                Stats = runtimeEntry.GetStats();
            }
        }

        public virtual void InitializeRun(WeaponRuntimeEntry runtimeEntry)
        {
            InitializeRun(runtimeEntry as WeaponLoadoutEntry);
        }

        public virtual void ResetRun()
        {
            _isActive = false;
        }

        // Legacy wrappers to keep existing callers safe.
        public virtual void Init(PlayerController player, WeaponLoadoutEntry entry, EnemyManager enemyManager)
        {
            Configure(player, entry, enemyManager);
            InitializeRun(entry);
        }
        public bool Activate() => BeginPhase();
        public bool Deactivate() => EndPhase();
        
        protected virtual void Update()
        {
            if (!_isActive)
                return;
        }

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

        public void SetOffset(Vector2 offset)
        {
            transform.position = 
                Player.transform.position + (Vector3)offset;
        }

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
    }
}
