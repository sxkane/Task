using Data;
using Data.Text;
using Player;
using TMPro;
using UI.GameSceneUI.IconSlots;
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
        
        [Header("Icon")]
        [SerializeField] private IconSlot iconSlot;

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
            descriptionText.text = GameTextBuilder.BuildPlayer(data);
            iconSlot.Set(data.GetIcon(), Rarity.Common);
        }

        public void ShowWeapon(WeaponEntry data)
        {
            if (data == null || !data.IsValid())
            {
                Clear();
                return;
            }

            titleText.text = data.GetDisplayName();
            descriptionText.text = GameTextBuilder.BuildWeapon(data);
            iconSlot.Set(data.GetIcon(), data.rarity);
        }

        public void ShowRandomPlayer()
        {
            titleText.text = "???";
            descriptionText.text = "以随机角色开始。";
            iconSlot.Set(randomIconSprite, Rarity.Common);
        }

        public void ShowRandomWeapon()
        {
            titleText.text = "???";
            descriptionText.text = "为该角色随机选择一把初始武器。";
            iconSlot.Set(randomIconSprite, Rarity.Common);
        }

        public void Clear()
        {
            titleText.text = string.Empty;
            descriptionText.text = string.Empty;
            
            iconSlot.Clear();
        }
    }
}
