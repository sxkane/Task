using System;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance;

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
            SceneManager.LoadScene("Select Scene");
        }

        public void StartGame(GameSession session)
        {
            CurrentSession = session;
            SceneManager.LoadScene("Game Scene");
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}