using System.Collections;
using UnityEngine;

namespace UI.FadeScreenUI
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        public void FadeIn(float duration)
        {
            StartFade(1f, 0f, duration);
        }

        public void FadeOut(float duration)
        {
            StartFade(0f, 1f, duration);
        }

        private void StartFade(float from, float to, float duration)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeRoutine(from, to, duration));
        }

        private IEnumerator FadeRoutine(float from, float to, float duration)
        {
            float time = 0f;
            
            canvasGroup.blocksRaycasts = true;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / duration;

                canvasGroup.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }

            canvasGroup.alpha = to;
            
            if (to == 0f)
                canvasGroup.blocksRaycasts = false;
        }
    }
}