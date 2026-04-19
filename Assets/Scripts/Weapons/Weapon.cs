using System.Collections.Generic;
using Enemy;
using Events;
using Events.EnemyEvents;
using Player;
using UnityEngine;
using Weapons.Abilities;
using Weapons.Core;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        protected PlayerController Player { get; private set; }
        public WeaponEntry Entry { get; private set; }
        public int WeaponID { get; private set; }
        public WeaponStats Stats { get; private set; }
        public WeaponRuntimeStats RuntimeStats { get; private set; }
        public WeaponRuntimeContext RuntimeContext { get; private set; }
        public IReadOnlyList<WeaponAbility> Abilities => _abilities;
        public EnemyManager EnemyManager { get; private set; }
        protected Transform ProjectileRoot { get; private set; }
        public Vector2 Offset { get; private set; } = Vector2.zero;
        public Vector2 RuntimeOffset { get; private set; } = Vector2.zero;
        protected bool IsWeaponActive => _isActive;

        private bool _isActive;
        private readonly List<WeaponAbility> _abilities = new();
        
        public void SetOffset(Vector2 offset)
        {
            Offset = offset;
            transform.position = Player.transform.position + (Vector3)(Offset + RuntimeOffset);
        }

        protected void SetRuntimeOffset(Vector2 runtimeOffset)
        {
            RuntimeOffset = runtimeOffset;
            if (Player != null)
                transform.position = Player.transform.position + (Vector3)(Offset + RuntimeOffset);
        }

        protected void FaceDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            transform.right = direction.normalized;
        }

        public void FaceDirectionFromAbility(Vector2 direction)
        {
            FaceDirection(direction);
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
            RebuildRuntime();
        }

        public virtual void InitializeRun(WeaponEntry entry = null)
        {
            if (entry != null)
            {
                Entry = entry;
                WeaponID = entry.GetDataId();
                Stats = entry.GetStats();
            }

            RebuildRuntime();
        }

        public virtual void ResetRun()
        {
            _isActive = false;
            RuntimeOffset = Vector2.zero;
        }

        public virtual void CleanupRun()
        {
            _isActive = false;
            RuntimeOffset = Vector2.zero;
        }

        public void BeginPhase()
        {
            _isActive = true;
            NotifyAbilitiesBeginPhase();
        }

        public void EndPhase()
        {
            _isActive = false;
            NotifyAbilitiesEndPhase();
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
            RebuildRuntime();
        }

        public bool CanUpgrade()
        {
            return Entry != null && Entry.CanUpgrade();
        }

        #endregion

        protected void NotifyAbilitiesAttack()
        {
            for (var i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (ability == null)
                    continue;

                ability.OnAttack(RuntimeContext);
            }
        }

        public void NotifyProjectileHit(EnemyController enemy, Vector2 hitPosition)
        {
            for (var i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (ability == null)
                    continue;

                ability.OnProjectileHit(RuntimeContext, enemy, hitPosition);
            }
        }

        protected int ModifyDamageWithAbilities(EnemyController enemy, Vector2 hitPosition, int damage, bool isCritical)
        {
            var modifiedDamage = damage;
            for (var i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (ability == null)
                    continue;

                modifiedDamage = ability.ModifyDamage(RuntimeContext, enemy, hitPosition, modifiedDamage, isCritical);
            }

            return modifiedDamage;
        }

        protected void NotifyAbilitiesHit(EnemyController enemy, Vector2 hitPosition, int damage, bool isCritical)
        {
            for (var i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (ability == null)
                    continue;

                ability.OnHit(RuntimeContext, enemy, hitPosition, damage, isCritical);
            }
        }

        private void RebuildRuntime()
        {
            RuntimeStats ??= new WeaponRuntimeStats();
            RuntimeStats.Initialize(Stats);
            ConfigureRuntimeDefaults();
            RuntimeContext = new WeaponRuntimeContext(this, Player, EnemyManager, ProjectileRoot, RuntimeStats);
            RebuildAbilities();
        }

        protected virtual void ConfigureRuntimeDefaults()
        {
        }

        private void RebuildAbilities()
        {
            _abilities.Clear();

            var dataAbilities = Entry?.weaponData?.GetAbilities();
            if (dataAbilities == null)
                return;

            for (var i = 0; i < dataAbilities.Count; i++)
            {
                var ability = dataAbilities[i];
                if (ability == null)
                    continue;

                _abilities.Add(ability);
                ability.OnInitialize(RuntimeContext);
            }
        }

        private void NotifyAbilitiesBeginPhase()
        {
            for (var i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (ability == null)
                    continue;

                ability.OnBeginPhase(RuntimeContext);
            }
        }

        private void NotifyAbilitiesEndPhase()
        {
            for (var i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (ability == null)
                    continue;

                ability.OnEndPhase(RuntimeContext);
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnEnemyDamagedEvent>(HandleEnemyDamaged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnEnemyDamagedEvent>(HandleEnemyDamaged);
        }

        private void HandleEnemyDamaged(OnEnemyDamagedEvent eventData)
        {
            if (eventData == null || eventData.SourceWeapon != this || !eventData.WasKilled)
                return;

            for (var i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (ability == null)
                    continue;

                ability.OnKill(RuntimeContext, eventData.Target, eventData.IsCritical);
            }
        }
    }
}
