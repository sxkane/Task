using System.Collections;
using System.Collections.Generic;
using Enemy;
using Events;
using Events.EnemyEvents;
using UnityEngine;

namespace Weapons.FireBall
{
    [CreateAssetMenu(menuName = "Game/Weapon Effects/Explosion On Hit")]
    public class ExplosionOnHitEffect : Effect
    {
        [System.Serializable]
        private class BurnProfile
        {
            public Rarity rarity;
            public int tickDamage;
            public int tickCount;
        }

        [Header("Explosion")]
        [SerializeField] private float explosionRadius = 1.8f;
        [SerializeField] private int explosionDamage = 8;

        [Header("Burn")]
        [SerializeField] private float burnTickInterval = 0.35f;
        [SerializeField] private List<BurnProfile> burnProfiles = new()
        {
            new BurnProfile { rarity = Rarity.Rare, tickDamage = 5, tickCount = 3 },
            new BurnProfile { rarity = Rarity.Epic, tickDamage = 6, tickCount = 4 },
            new BurnProfile { rarity = Rarity.Legendary, tickDamage = 8, tickCount = 5 }
        };

        private readonly List<EnemyController> _targets = new();

        public override void Execute(EffectExecutionContext context, EffectTrigger effectTrigger)
        {
            if (effectTrigger != EffectTrigger.OnWeaponHit)
                return;

            Apply(context);
        }

        protected override void Apply(EffectExecutionContext context)
        {
            if (context == null || context.EnemyManager == null)
                return;

            Vector2 center = context.HitEnemy != null
                ? context.HitEnemy.transform.position
                : context.HitPosition;

            _targets.Clear();
            context.EnemyManager.GetEnemiesInRadius(center, explosionRadius, _targets);

            if (_targets.Count == 0)
                return;

            BurnProfile burn = ResolveBurnProfile(context.Weapon);

            foreach (var enemy in _targets)
            {
                if (enemy == null)
                    continue;

                EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, explosionDamage));

                if (burn != null && burn.tickDamage > 0 && burn.tickCount > 0)
                    EffectRuntimeRunner.BeginRoutine(ApplyBurn(enemy, burn.tickDamage, burn.tickCount));
            }
        }

        public override string BuildDescription()
        {
            return $"Hit explodes in {explosionRadius:0.#}m, deals {explosionDamage} and burns by rarity profile";
        }

        private BurnProfile ResolveBurnProfile(Weapon weapon)
        {
            if (weapon?.Entry == null || burnProfiles == null || burnProfiles.Count == 0)
                return null;

            var rarity = weapon.Entry.rarity;
            for (int i = 0; i < burnProfiles.Count; i++)
            {
                if (burnProfiles[i] != null && burnProfiles[i].rarity == rarity)
                    return burnProfiles[i];
            }

            return null;
        }

        private IEnumerator ApplyBurn(EnemyController enemy, int tickDamage, int tickCount)
        {
            for (int i = 0; i < tickCount; i++)
            {
                yield return new WaitForSeconds(burnTickInterval);

                if (enemy == null || !enemy.gameObject.activeInHierarchy || !enemy.Stats.IsAlive)
                    yield break;

                EventBus.Publish(new OnEnemyDamageRequestedEvent(enemy, tickDamage));
            }
        }
    }
}
