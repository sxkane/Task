using System.Collections.Generic;
using System.Text;
using Data.Text;
using Stats;
using TMPro;
using UnityEngine;
using Weapons;
using Weapons.Modifiers;

namespace UI.GameSceneUI.Reward
{
    public class BonusPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI bonusDescription;
        
        public void Show(
            List<WeaponSetBonusData> data,
            IReadOnlyDictionary<WeaponSetBonusData, int> bonusCount,
            RectTransform target,
            Vector2 offset)
        {
            if (data == null || data.Count == 0 || target == null)
            {
                Hide();
                return;
            }

            if (bonusDescription != null)
                bonusDescription.text = BuildBonusDescription(data, bonusCount);

            SetPositionBesideTarget(target, offset);
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
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

        private static string BuildBonusDescription(List<WeaponSetBonusData> data, IReadOnlyDictionary<WeaponSetBonusData, int> bonusCount)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < data.Count; i++)
            {
                var bonus = data[i];
                if (bonus == null)
                    continue;

                var currentCount = 0;
                bonusCount?.TryGetValue(bonus, out currentCount);

                if (sb.Length > 0)
                    sb.AppendLine().AppendLine();

                sb.AppendLine(bonus.DisplayName);

                if (bonus.Tiers == null)
                    continue;

                for (var tierIndex = 0; tierIndex < bonus.Tiers.Count; tierIndex++)
                {
                    var tier = bonus.Tiers[tierIndex];
                    if (tier == null)
                        continue;

                    var requiredCount = tier.RequiredCount;
                    var color = currentCount >= requiredCount ? "#FFFFFF" : "#7F7F7F";
                    sb.AppendLine($"<color={color}>({requiredCount}) {BuildTierText(tier)}</color>");
                }
            }

            return sb.ToString().TrimEnd();
        }

        private static string BuildTierText(WeaponSetBonusData.SetTier tier)
        {
            var parts = new List<string>();

            if (tier.playerModifiers != null)
            {
                for (var i = 0; i < tier.playerModifiers.Count; i++)
                {
                    var modifier = tier.playerModifiers[i];
                    if (modifier == null)
                        continue;

                    parts.Add($"{StatValueUtility.FormatModifierValue(modifier.value, modifier.modType)} {StatNameMapper.GetName(modifier.statType)}");
                }
            }

            if (tier.weaponModifiers != null)
            {
                for (var i = 0; i < tier.weaponModifiers.Count; i++)
                {
                    var modifier = tier.weaponModifiers[i];
                    if (modifier == null)
                        continue;

                    parts.Add($"{StatValueUtility.FormatModifierValue(modifier.value, modifier.modType)} {GetWeaponStatName(modifier.statType)}");
                }
            }

            return parts.Count == 0 ? "无效果" : string.Join(" / ", parts);
        }

        private static string GetWeaponStatName(WeaponStatType statType)
        {
            return statType switch
            {
                WeaponStatType.AttackInterval => "攻击间隔",
                WeaponStatType.CritChance => "暴击率",
                WeaponStatType.CritDamage => "暴击伤害",
                WeaponStatType.Range => "射程",
                WeaponStatType.Knockback => "击退",
                WeaponStatType.ProjectileSpeed => "弹速",
                WeaponStatType.PierceCount => "穿透次数",
                WeaponStatType.PierceDamageMultiplier => "穿透伤害",
                WeaponStatType.BounceCount => "弹跳次数",
                WeaponStatType.ExplosionRadius => "爆炸范围",
                WeaponStatType.BurnSpreadCount => "燃烧扩散",
                WeaponStatType.MeleeDamage => "近战伤害",
                WeaponStatType.RangedDamage => "远程伤害",
                WeaponStatType.ElementalDamage => "元素伤害",
                _ => statType.ToString()
            };
        }
    }
}
