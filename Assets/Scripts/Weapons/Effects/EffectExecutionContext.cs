using Enemy;
using Items;
using Player;
using UnityEngine;
using Weapons.Items;

namespace Weapons.Effects
{
    public sealed class EffectExecutionContext
    {
        public PlayerController Player { get; private set; }
        public Weapon Weapon { get; private set; }
        public ItemData ItemData { get; private set; }
        public EnemyManager EnemyManager { get; private set; }
        public EnemyController HitEnemy { get; private set; }
        public Vector2 HitPosition { get; private set; }

        public static EffectExecutionContext ForWeapon(PlayerController player, Weapon weapon, EnemyManager enemyManager)
        {
            return new EffectExecutionContext
            {
                Player = player,
                Weapon = weapon,
                EnemyManager = enemyManager,
                HitPosition = weapon != null ? weapon.transform.position : Vector2.zero
            };
        }

        public static EffectExecutionContext ForWeaponHit(
            PlayerController player,
            Weapon weapon,
            EnemyManager enemyManager,
            EnemyController hitEnemy,
            Vector2 hitPosition)
        {
            return new EffectExecutionContext
            {
                Player = player,
                Weapon = weapon,
                EnemyManager = enemyManager,
                HitEnemy = hitEnemy,
                HitPosition = hitPosition
            };
        }

        public static EffectExecutionContext ForItem(PlayerController player, ItemData itemData)
        {
            return new EffectExecutionContext
            {
                Player = player,
                ItemData = itemData
            };
        }
    }
}
