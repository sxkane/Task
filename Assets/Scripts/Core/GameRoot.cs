using System.Collections;
using Events;
using UI;
using UI.FadeScreenUI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// 游戏全局场景管理类
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance;

        [SerializeField] private SceneTransition transition;
        public GameSession CurrentSession;

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
            StartCoroutine(Load("Select Scene"));
        }

        public void StartGame(GameSession session)
        {
            CurrentSession = session;
            StartCoroutine(Load("Game Scene", true));
        }

        public void ReturnToMainMenu()
        {
            EventBus.Clear();
            StartCoroutine(Load("Main Menu", true));
        }
        
        public IEnumerator Load(string sceneName, bool needFade = false)
        {
            Time.timeScale = 1f;

            if (needFade && transition != null)
                yield return transition.FadeOut(0.3f);

            yield return SceneManager.LoadSceneAsync(sceneName);

            if (needFade && transition != null)
                yield return transition.FadeIn(0.3f);
        }
    }
}
