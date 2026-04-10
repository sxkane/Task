using Data;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI.CharacterSelectUI
{
    public class InformationSlot : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image icon;
        
        [Header("Random")]
        [SerializeField] private Sprite randomIconSprite;

        public void ShowPlayer(PlayerData data)
        {
            if (data == null)
            {
                ShowRandomPlayer();
                return;
            }

            titleText.text = data.GetDisplayName();
            descriptionText.text = GameDataTextBuilder.BuildPlayerDescription(data);
            icon.sprite = data.GetIcon();
        }

        public void ShowWeapon(WeaponLoadoutEntry data)
        {
            if (data == null || !data.IsValid())
            {
                Clear();
                return;
            }

            titleText.text = data.GetDisplayName();
            descriptionText.text = GameDataTextBuilder.BuildWeaponDescription(data);
            icon.sprite = data.GetIcon();
        }

        public void ShowRandomPlayer()
        {
            titleText.text = "???";
            descriptionText.text = "Click to start with a random character and one random starter weapon.";
            icon.sprite = randomIconSprite;
        }

        public void ShowRandomWeapon()
        {
            titleText.text = "???";
            descriptionText.text = "Choose a random starter weapon from this character.";
            icon.sprite = randomIconSprite;
        }

        public void Clear()
        {
            titleText.text = string.Empty;
            descriptionText.text = string.Empty;
            icon.sprite = null;
        }
    }
}
