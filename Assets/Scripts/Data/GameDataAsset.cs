using System;
using UnityEngine;

namespace Data
{
    public abstract class GameDataAsset : ScriptableObject
    {
        public abstract int DataId { get; }
        public abstract string DisplayName { get; }
        public abstract Sprite Icon { get; }
        public abstract string Summary { get; }

        public virtual string ValidationSourceName =>
            string.IsNullOrWhiteSpace(DisplayName) ? name : DisplayName;

        public abstract bool IsValid();
    }
}
