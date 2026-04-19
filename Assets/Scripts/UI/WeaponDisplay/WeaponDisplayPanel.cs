using Core;
using Data.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using System;

namespace UI.WeaponDisplay
{
    public class WeaponDisplayPanel : MonoBehaviour
    {
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button sellButton;
        [SerializeField] private Button cancelButton;

        [SerializeField] private TextMeshProUGUI sellCoinText;
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private TextMeshProUGUI weaponDescription;

        private Weapon _currentWeapon;
        public Weapon CurrentWeapon => _currentWeapon;
        public event Action Closed;

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnEnable()
        {
            ResolveReferences();
            if (upgradeButton != null)
                upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
            if (sellButton != null)
                sellButton.onClick.AddListener(OnSellButtonClick);
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelButtonClick);
        }

        private void OnDisable()
        {
            if (upgradeButton != null)
                upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
            if (sellButton != null)
                sellButton.onClick.RemoveListener(OnSellButtonClick);
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancelButtonClick);
        }

        public void Show(Weapon weapon)
        {
            Show(weapon, null, Vector2.zero);
        }

        public void Show(Weapon weapon, RectTransform target, Vector2 offset)
        {
            _currentWeapon = weapon;
            
            if (_currentWeapon == null)
            {
                Hide();
                return;
            }

            if (weaponName != null)
                weaponName.text = _currentWeapon.Entry != null ? _currentWeapon.Entry.GetDisplayName() : "武器";

            if (weaponDescription != null)
                weaponDescription.text = _currentWeapon.Entry != null ? GameTextBuilder.BuildWeapon(_currentWeapon.Entry) : string.Empty;

            if (sellCoinText != null)
                sellCoinText.text = UIValueBuilder.Coin(_currentWeapon.Entry != null ? _currentWeapon.Entry.GetRecyclePrice() : 0);

            if (upgradeButton != null)
                upgradeButton.interactable = GameController.Instance?.WeaponManager != null
                                             && GameController.Instance.WeaponManager.CanUpgradeWeapon(_currentWeapon);

            if (target != null)
                SetPositionBesideTarget(target, offset);

            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _currentWeapon = null;
            Closed?.Invoke();
        }

        public void OnUpgradeButtonClick()
        {
            if (_currentWeapon == null || GameController.Instance?.WeaponManager == null)
                return;

            if (GameController.Instance.WeaponManager.TryUpgradeWeapon(_currentWeapon))
                Hide();
        }

        public void OnSellButtonClick()
        {
            if (_currentWeapon == null || GameController.Instance?.WeaponManager == null || GameController.Instance.PlayerManager?.Player?.RuntimeData == null)
                return;

            if (GameController.Instance.WeaponManager.TrySellWeapon(_currentWeapon, out var refund))
            {
                GameController.Instance.PlayerManager.Player.RuntimeData.AddCoins(refund);
                Hide();
            }
        }

        public void OnCancelButtonClick()
        {
            Hide();
        }

        private void SetPositionBesideTarget(RectTransform target, Vector2 offset)
        {
            var selfRect = transform as RectTransform;
            if (selfRect == null || selfRect.parent == null)
                return;

            var parentRect = selfRect.parent as RectTransform;
            if (parentRect == null)
                return;

            var rootCanvas = parentRect.GetComponentInParent<Canvas>();
            var camera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? rootCanvas.worldCamera
                : null;

            var worldCorners = new Vector3[4];
            target.GetWorldCorners(worldCorners);
            var anchorWorldPos = (worldCorners[2] + worldCorners[3]) * 0.5f;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, anchorWorldPos);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, camera, out var localPoint))
                selfRect.anchoredPosition = localPoint + offset;
        }

        private void ResolveReferences()
        {
            if (weaponName == null || weaponDescription == null || sellCoinText == null)
            {
                var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
                if (texts.Length > 0 && weaponName == null)
                    weaponName = texts[0];
                if (texts.Length > 1 && weaponDescription == null)
                    weaponDescription = texts[1];
                if (texts.Length > 2 && sellCoinText == null)
                    sellCoinText = texts[2];
            }

            if (upgradeButton == null || sellButton == null || cancelButton == null)
            {
                var buttons = GetComponentsInChildren<Button>(true);
                if (buttons.Length > 0 && upgradeButton == null)
                    upgradeButton = buttons[0];
                if (buttons.Length > 1 && sellButton == null)
                    sellButton = buttons[1];
                if (buttons.Length > 2 && cancelButton == null)
                    cancelButton = buttons[2];
            }
        }
    }
}
