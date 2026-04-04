using System.Collections.Generic;
using Events;
using Events.ShopEvents;
using Rewards.Shops;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Weapons;
using Weapons.Items;
using Button = UnityEngine.UI.Button;

namespace UI.GameSceneUI
{
    public class RewardSlot : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Buttons")]
        [SerializeField] private Button lockedButton;
        [SerializeField] private Button buyButton;
        
        private ShopItem _item;
        
        public void Show(ShopItem item)
        {
            _item = item;
            
            if (item.type == ShopItemType.Item)
            {
                nameText.text = item.itemData.itemName;
                rewardText.text = item.itemData.price.ToString();
            }
            else
            {
                var weaponStats = item.weaponData.GetStats(item.rarity);
                nameText.text = item.weaponData.weaponName;
                rewardText.text = weaponStats.price.ToString();
            }
            
            gameObject.SetActive(true);
        }
        
        private void OnEnable()
        {
            lockedButton.onClick.AddListener(Lock);
            buyButton.onClick.AddListener(Buy);
        }

        private void OnDisable()
        {
            lockedButton.onClick.RemoveListener(Lock);
            buyButton.onClick.RemoveListener(Buy);
        }

        private void Lock()
        {
            var text = lockedButton.GetComponentInChildren<TextMeshProUGUI>();
            if (_item.isLocked)
                text.text = "UnLocked";
            else
                text.text = "Locked";
            
            EventBus.Publish(new OnShopItemLockedEvent(_item));
        }

        private void Buy()
        {
            if (_item == null)
                return;

            EventBus.Publish(new OnShopPurchaseRequestedEvent(_item));
        }

        private static string BuildWeaponDescription(WeaponData weaponData, WeaponStats weaponStats)
        {
            var lines = new List<string>();

            if (!string.IsNullOrWhiteSpace(weaponData.description))
                lines.Add(weaponData.description.Trim());

            foreach (var damage in weaponStats.damage)
            {
                if (damage.damage <= 0 && damage.percentage <= 0)
                    continue;

                string damageType = damage.damageType switch
                {
                    DamageType.Melee => "Melee",
                    DamageType.Ranged => "Ranged",
                    DamageType.Elemental => "Elemental",
                    _ => damage.damageType.ToString()
                };

                if (damage.damage > 0 && damage.percentage > 0)
                    lines.Add($"+{damage.damage:0.#} {damageType} ({damage.percentage}%)");
                else if (damage.damage > 0)
                    lines.Add($"+{damage.damage:0.#} {damageType}");
                else
                    lines.Add($"+{damage.percentage}% {damageType}");
            }

            if (weaponStats.attackSpeed > 0)
                lines.Add($"+{weaponStats.attackSpeed:0.##} Attack Speed");

            if (weaponStats.critChance > 0)
                lines.Add($"+{weaponStats.critChance:0.##} Crit Chance");

            if (weaponStats.critDamage > 0)
                lines.Add($"+{weaponStats.critDamage:0.##} Crit Damage");

            if (weaponStats.range > 0)
                lines.Add($"+{weaponStats.range:0.##} Range");

            if (weaponStats.knockback > 0)
                lines.Add($"+{weaponStats.knockback:0.##} Knockback");

            return string.Join("\n", lines);
        }

        private static string FormatItemModify(ItemModify modify)
        {
            string statName = modify.statType switch
            {
                StatType.MaxHP => "Max HP",
                StatType.HPRegen => "HP Regen",
                StatType.LifeSteal => "Life Steal",
                StatType.Armor => "Armor",
                StatType.Dodge => "Dodge",
                StatType.DamagePercent => "Damage",
                StatType.MeleeDamage => "Melee Damage",
                StatType.RangedDamage => "Ranged Damage",
                StatType.ElementalDamage => "Elemental Damage",
                StatType.AttackSpeed => "Attack Speed",
                StatType.CritChance => "Crit Chance",
                StatType.Range => "Range",
                StatType.Speed => "Speed",
                StatType.Luck => "Luck",
                StatType.Harvesting => "Harvesting",
                _ => modify.statType.ToString()
            };

            return modify.modType switch
            {
                StatModType.Flat => $"+{modify.value:0.#} {statName}",
                StatModType.PercentAdd => $"+{modify.value:0.#}% {statName}",
                StatModType.PercentMult => $"+{modify.value * 100f:0.#}% {statName}",
                _ => $"+{modify.value:0.#} {statName}"
            };
        }
    }
}
