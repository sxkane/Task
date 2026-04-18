using Enemy;
using Player;
using UnityEngine;

namespace Weapons.Core
{
    public sealed class WeaponRuntimeContext
    {
        public Weapon Weapon { get; }
        public PlayerController Player { get; }
        public EnemyManager EnemyManager { get; }
        public Transform ProjectileRoot { get; }
        public WeaponRuntimeStats RuntimeStats { get; }

        public WeaponRuntimeContext(
            Weapon weapon,
            PlayerController player,
            EnemyManager enemyManager,
            Transform projectileRoot,
            WeaponRuntimeStats runtimeStats)
        {
            Weapon = weapon;
            Player = player;
            EnemyManager = enemyManager;
            ProjectileRoot = projectileRoot;
            RuntimeStats = runtimeStats;
        }
    }
}
