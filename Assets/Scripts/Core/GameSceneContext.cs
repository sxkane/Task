using UI.GameSceneUI;
using UnityEngine;

namespace Core
{
    public class GameSceneContext : MonoBehaviour
    {
        [Header("World Roots")]
        [SerializeField] private Transform playerRoot;
        [SerializeField] private Transform weaponRoot;
        [SerializeField] private Transform enemyRoot;
        [SerializeField] private Transform dropRoot;
        [SerializeField] private Transform worldVfxRoot;
        [SerializeField] private Transform worldTextRoot;
        
        public Transform PlayerRoot => playerRoot;
        public Transform WeaponRoot => weaponRoot;
        public Transform EnemyRoot => enemyRoot;
        public Transform DropRoot => dropRoot;
        public Transform WorldVfxRoot => worldVfxRoot;
        public Transform WorldTextRoot => worldTextRoot;
    }
}