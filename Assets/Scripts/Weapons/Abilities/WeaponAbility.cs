using Enemy;
using UnityEngine;
using Weapons.Core;

namespace Weapons.Abilities
{
    public abstract class WeaponAbility : ScriptableObject
    {
        public virtual void OnInitialize(WeaponRuntimeContext context)
        {
        }

        public virtual void OnBeginPhase(WeaponRuntimeContext context)
        {
        }

        public virtual void OnEndPhase(WeaponRuntimeContext context)
        {
        }

        public virtual void OnAttack(WeaponRuntimeContext context)
        {
        }

        public virtual void OnProjectileHit(WeaponRuntimeContext context, EnemyController enemy, Vector2 hitPosition)
        {
        }

        public virtual int ModifyDamage(WeaponRuntimeContext context, EnemyController enemy, Vector2 hitPosition, int damage, bool isCritical)
        {
            return damage;
        }

        public virtual void OnHit(WeaponRuntimeContext context, EnemyController enemy, Vector2 hitPosition, int damage, bool isCritical)
        {
        }

        public virtual void OnKill(WeaponRuntimeContext context, EnemyController enemy, bool isCritical)
        {
        }

        public virtual string BuildDescription(Rarity rarity)
        {
            return string.Empty;
        }

        protected TConfig ResolveConfig<TConfig>(System.Collections.Generic.IReadOnlyList<TConfig> configs, Rarity rarity)
            where TConfig : WeaponAbilityRarityConfigBase
        {
            if (configs == null || configs.Count == 0)
                return null;

            for (var i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config != null && config.rarity == rarity)
                    return config;
            }

            return null;
        }
    }
}
