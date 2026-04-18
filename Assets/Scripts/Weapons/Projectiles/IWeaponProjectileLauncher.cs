using UnityEngine;
using Weapons.Core;

namespace Weapons.Projectiles
{
    public interface IWeaponProjectileLauncher
    {
        void Launch(WeaponRuntimeContext context, Transform target, Vector2 direction, float projectileSpeed);
    }
}
