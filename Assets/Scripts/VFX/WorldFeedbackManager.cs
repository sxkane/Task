using Core;
using Events;
using Events.EnemyEvents;
using Events.PlayerEvents;
using ObjectPool;
using UI.GameSceneUI.VFX;
using UnityEngine;

namespace VFX
{
    public class WorldFeedbackManager : MonoBehaviour
    {
        [SerializeField] private GameObject combatTextPrefab;

        private static readonly Color PlayerDamageColor = new(1f, 0.35f, 0.35f, 1f);
        private static readonly Color EnemyDamageColor = new(1f, 1f, 1f, 1f);
        private static readonly Color CriticalDamageColor = new(1f, 0.92f, 0.3f, 1f);

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
            var color = e.IsDodged ? CriticalDamageColor : PlayerDamageColor;
            SpawnText(e.Target.transform.position + Vector3.up * 1.4f, text, color);
        }

        private void SpawnCombatText(OnEnemyDamagedEvent e)
        {
            var text = $"-{e.FinalDamage}";
            var color = e.IsCritical ? CriticalDamageColor : EnemyDamageColor;
            SpawnText(e.Target.transform.position + Vector3.up * 1.4f, text, color);
        }

        private void SpawnText(Vector3 worldPosition, string content, Color color)
        {
            if (combatTextPrefab == null)
                return;

            var parent = GameController.Instance?.Session?.GetOrCreateGroupRoot(GameSessionRootType.WorldText, "CombatText");
            var obj = PoolManager.Instance != null
                ? PoolManager.Instance.Spawn(combatTextPrefab, worldPosition, Quaternion.identity, parent)
                : Instantiate(combatTextPrefab, worldPosition, Quaternion.identity, parent);
            var combatText = obj.GetComponent<CombatText>();
            if (combatText != null)
                combatText.Initialize(worldPosition, content, color);
        }
    }
}
