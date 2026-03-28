using TMPro;
using UnityEngine;
using Events;

namespace UI.GameSceneUI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gameTimerText;
        
        private void OnEnable()
        {
            EventBus.Subscribe<WaveChangeSecondEvent>(UpdateGameTimerText);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaveChangeSecondEvent>(UpdateGameTimerText);
        }

        private void UpdateGameTimerText(WaveChangeSecondEvent e)
        {
            gameTimerText.text = e.Timer.ToString();
        }
    }
}