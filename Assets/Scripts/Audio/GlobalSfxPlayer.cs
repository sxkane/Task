using Events;
using Events.EnemyEvents;
using Events.PlayerEvents;
using Events.UpgradeEvents;
using GameAudio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Audio
{
    public class GlobalSfxPlayer : MonoBehaviour
    {
        private const int SourcePoolSize = 8;
        
        private AudioSource[] _sources;
        private int _nextSourceIndex;
        [SerializeField] private GlobalSfxDatabase database;

        public static GlobalSfxPlayer Instance;

        public static void BindButton(Button button)
        {
            if (button == null)
                return;

            var proxy = button.GetComponent<UIButtonSfxProxy>();
            if (proxy == null)
                proxy = button.gameObject.AddComponent<UIButtonSfxProxy>();

            proxy.Configure(button);
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(gameObject);
            CreateSourcePool();
            BindButtonsInScene();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Subscribe<OnEnemyDamagedEvent>(OnEnemyDamaged);
            EventBus.Subscribe<OnEnemyDiedEvent>(OnEnemyDied);
            EventBus.Subscribe<OnUpgradeOptionSelectedEvent>(OnUpgradeSelected);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnPlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Unsubscribe<OnEnemyDamagedEvent>(OnEnemyDamaged);
            EventBus.Unsubscribe<OnEnemyDiedEvent>(OnEnemyDied);
            EventBus.Unsubscribe<OnUpgradeOptionSelectedEvent>(OnUpgradeSelected);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void PlayButtonHover()
        {
            PlayClip(database != null ? database.buttonHover : null, database != null ? database.uiVolume : 1f);
        }

        public void PlayButtonClick()
        {
            PlayClip(database != null ? database.buttonClick : null, database != null ? database.uiVolume : 1f);
        }

        public void PlayPickupCoin()
        {
            if (database == null)
                return;

            PlayClip(database.pickupCoin, database.pickupVolume, database.pickupPitchJitter, database.pickupVolumeJitter);
        }

        public void PlayPickupExp()
        {
            if (database == null)
                return;

            PlayClip(database.pickupExp, database.pickupVolume, database.pickupPitchJitter, database.pickupVolumeJitter);
        }

        public void PlayWeaponAttack()
        {
            if (database == null)
                return;

            PlayClip(database.weaponAttack, database.actionVolume, database.actionPitchJitter, database.actionVolumeJitter);
        }

        public void PlayShopRefresh()
        {
            if (database == null)
                return;

            PlayClip(database.shopRefresh, database.uiVolume);
        }

        public void PlayShopPurchase()
        {
            if (database == null)
                return;

            PlayClip(database.shopPurchase, database.uiVolume);
        }

        private void OnPlayerDamaged(OnPlayerDamagedEvent eventData)
        {
            if (eventData == null || eventData.IsDodged || eventData.FinalDamage <= 0 || database == null)
                return;

            PlayClip(database.playerHit, database.combatVolume, database.hitPitchJitter, database.hitVolumeJitter);
        }

        private void OnEnemyDamaged(OnEnemyDamagedEvent eventData)
        {
            if (eventData == null || eventData.FinalDamage <= 0 || database == null)
                return;

            PlayClip(database.enemyHit, database.combatVolume, database.hitPitchJitter, database.hitVolumeJitter);
        }

        private void OnEnemyDied(OnEnemyDiedEvent eventData)
        {
            if (eventData == null || database == null)
                return;

            PlayClip(database.enemyDeath, database.combatVolume, database.actionPitchJitter, database.actionVolumeJitter);
        }

        private void OnUpgradeSelected(OnUpgradeOptionSelectedEvent eventData)
        {
            if (eventData == null || database == null)
                return;

            PlayClip(database.upgradeSelect, database.uiVolume);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BindButtonsInScene();
        }

        private void BindButtonsInScene()
        {
            var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < buttons.Length; i++)
                BindButton(buttons[i]);
        }

        private void CreateSourcePool()
        {
            if (_sources != null && _sources.Length > 0)
                return;

            _sources = new AudioSource[SourcePoolSize];
            for (var i = 0; i < SourcePoolSize; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                _sources[i] = source;
            }
        }

        private void PlayClip(AudioClip clip, float baseVolume, float pitchJitter = 0f, float volumeJitter = 0f)
        {
            if (clip == null)
                return;

            CreateSourcePool();
            var source = _sources[_nextSourceIndex];
            _nextSourceIndex = (_nextSourceIndex + 1) % _sources.Length;

            source.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
            var volume = baseVolume * (1f + Random.Range(-volumeJitter, volumeJitter));
            source.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }
}
