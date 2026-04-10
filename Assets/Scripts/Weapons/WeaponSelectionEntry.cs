using System;

namespace Weapons
{
    /// <summary>
    /// 新命名：用于“选择/商店候选”的武器条目。
    /// 当前继承 WeaponLoadoutEntry 以保持资源与旧调用兼容。
    /// </summary>
    [Serializable]
    public class WeaponSelectionEntry : WeaponLoadoutEntry
    {
    }
}
