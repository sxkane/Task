using System.Collections;
using UnityEngine;

namespace UI.FadeScreenUI
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private FadeScreen fadeScreen;

        public IEnumerator FadeOut(float duration)
        {
            fadeScreen.FadeOut();
            yield return new WaitForSecondsRealtime(duration);
        }

        public IEnumerator FadeIn(float duration)
        {
            fadeScreen.FadeIn();
            yield return new WaitForSecondsRealtime(duration);
        }
    }
}
