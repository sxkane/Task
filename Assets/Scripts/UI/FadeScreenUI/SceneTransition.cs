using System.Collections;
using UnityEngine;

namespace UI.FadeScreenUI
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private FadeScreen fadeScreen;

        public IEnumerator FadeIn(float duration)
        {
            fadeScreen.FadeIn(duration);
            yield return new WaitForSecondsRealtime(duration);
        }

        public IEnumerator FadeOut(float duration)
        {
            fadeScreen.FadeOut(duration);
            yield return new WaitForSecondsRealtime(duration);
        }
    }
}
