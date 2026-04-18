using System.Collections;
using UnityEngine;

namespace Weapons.Abilities
{
    public class WeaponAbilityRuntimeRunner : MonoBehaviour
    {
        private static WeaponAbilityRuntimeRunner _instance;

        public static Coroutine BeginRoutine(IEnumerator routine)
        {
            if (routine == null)
                return null;

            EnsureInstance();
            return _instance.StartCoroutine(routine);
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
                return;

            var host = new GameObject("[WeaponAbilityRuntimeRunner]");
            Object.DontDestroyOnLoad(host);
            _instance = host.AddComponent<WeaponAbilityRuntimeRunner>();
        }
    }
}
