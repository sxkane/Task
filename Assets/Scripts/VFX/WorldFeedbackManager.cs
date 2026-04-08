using Events;
using Events.EnemyEvents;
using Events.PlayerEvents;
using UI.GameSceneUI;
using UnityEngine;

namespace VFX
{
    public class WorldFeedbackManager : MonoBehaviour
    {
        [SerializeField] private GameObject combatTextPrefab;
        
        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayerDamagedEvent>(SpawnCombatText);
            EventBus.Subscribe<OnEnemyDamagedEvent>(SpawnCombatText);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnPlayerDamagedEvent>(SpawnCombatText);
            EventBus.Unsubscribe<OnEnemyDamagedEvent>(SpawnCombatText);
        }

        private void SpawnCombatText(OnPlayerDamagedEvent e)
        {
            var text = e.IsDodged ? "DODGE" : $"-{e.FinalDamage}";
            var color = e.IsDodged ? new Color(1f, 0.92f, 0.3f, 1f) : new Color(1f, 0.45f, 0.45f, 1f);
            
            var obj = Instantiate(combatTextPrefab, transform);
            var combatText = obj.GetComponent<CombatText>();
            combatText.Initialize(e.Target.transform.position + Vector3.up * 1.4f, text, color);
        }

        private void SpawnCombatText(OnEnemyDamagedEvent e)
        {
            var text = $"-{e.FinalDamage}";
            var color = new Color(1f, 0.45f, 0.45f, 1f);
            
            var obj = Instantiate(combatTextPrefab, transform);
            var combatText = obj.GetComponent<CombatText>();
            combatText.Initialize(e.Target.transform.position + Vector3.up * 1.4f, text, color);
        }
    }
}