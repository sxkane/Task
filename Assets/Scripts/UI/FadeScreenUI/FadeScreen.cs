using UnityEngine;

namespace UI.FadeScreenUI
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        private static readonly int In = Animator.StringToHash("FadeIn");
        private static readonly int Out = Animator.StringToHash("FadeOut");

        private void Awake()
        {
            if (animator != null)
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        
        public void FadeIn() => animator.SetTrigger(In);
        public void FadeOut() => animator.SetTrigger(Out);
    }
}
