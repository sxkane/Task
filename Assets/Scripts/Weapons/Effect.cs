using UnityEngine;

namespace Weapons
{
    public class Effect : ScriptableObject
    {
        public virtual void ExecuteEffect()
        {
            Debug.Log("Effect executed");
        }
    }
}