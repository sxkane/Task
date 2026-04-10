using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class EffectRuntimeRunner : MonoBehaviour
    {
        private static EffectRuntimeRunner _instance;

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

            var host = new GameObject("[EffectRuntimeRunner]");
            DontDestroyOnLoad(host);
            _instance = host.AddComponent<EffectRuntimeRunner>();
        }
    }
}
