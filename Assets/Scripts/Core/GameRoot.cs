using System.Collections;
using Events;
using UI.FadeScreenUI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameRoot : MonoBehaviour
    {
        #region Singleton

        public static GameRoot Instance;

        #endregion

        #region Inspector

        [Header("Transition")]
        [SerializeField] private SceneTransition transition;

        #endregion

        #region Runtime

        public GameSession CurrentSession { get; private set; }

        #endregion

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void EnterSelectScene()
        {
            CurrentSession = null;
            StartCoroutine(Load("Select Scene", true));
        }

        public void StartGame(GameSession session)
        {
            CurrentSession = session;
            StartCoroutine(Load("Game Scene", true));
        }

        public void ReturnToMainMenu()
        {
            EventBus.Clear();
            CurrentSession = null;
            StartCoroutine(Load("Main Menu", true));
        }

        public IEnumerator Load(string sceneName, bool needFade = false)
        {
            Time.timeScale = 1f;

            if (needFade && transition != null)
                yield return transition.FadeOut(0.8f);

            yield return SceneManager.LoadSceneAsync(sceneName);

            if (needFade && transition != null)
                yield return transition.FadeIn(0.8f);
        }
    }
}
