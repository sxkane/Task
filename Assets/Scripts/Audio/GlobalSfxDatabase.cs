using UnityEngine;

namespace GameAudio
{
    [CreateAssetMenu(menuName = "Game/Audio/Global Sfx Database")]
    public class GlobalSfxDatabase : ScriptableObject
    {
        [Header("UI")]
        public AudioClip buttonHover;
        public AudioClip buttonClick;

        [Header("Combat")]
        public AudioClip playerHit;
        public AudioClip enemyHit;
        public AudioClip enemyDeath;
        public AudioClip weaponAttack;

        [Header("Pickup")]
        public AudioClip pickupCoin;
        public AudioClip pickupExp;

        [Header("Flow")]
        public AudioClip shopRefresh;
        public AudioClip shopPurchase;
        public AudioClip upgradeSelect;

        [Header("Volume")]
        [Range(0f, 1f)] public float uiVolume = 1f;
        [Range(0f, 1f)] public float combatVolume = 1f;
        [Range(0f, 1f)] public float pickupVolume = 1f;
        [Range(0f, 1f)] public float actionVolume = 1f;

        [Header("Variation")]
        [Range(0f, 0.3f)] public float hitPitchJitter = 0.08f;
        [Range(0f, 0.3f)] public float pickupPitchJitter = 0.08f;
        [Range(0f, 0.3f)] public float actionPitchJitter = 0.06f;
        [Range(0f, 0.3f)] public float hitVolumeJitter = 0.08f;
        [Range(0f, 0.3f)] public float pickupVolumeJitter = 0.08f;
        [Range(0f, 0.3f)] public float actionVolumeJitter = 0.06f;
    }
}
