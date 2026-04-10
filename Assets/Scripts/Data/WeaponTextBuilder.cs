using System.Collections.Generic;
using System.Text;
using Weapons;

namespace Data
{
    public static class WeaponTextBuilder
    {
        public static string BuildDescription(WeaponLoadoutEntry entry)
        {
            if (entry == null || !entry.IsValid())
                return string.Empty;

            return BuildDescription(entry.weaponData, entry.rarity);
        }

        public static string BuildDescription(WeaponData weaponData, Rarity rarity)
        {
            if (weaponData == null)
                return string.Empty;

            var sections = new List<string>();

            string summary = weaponData.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary))
                sections.Add(summary);

            var stats = weaponData.GetStats(rarity);
            if (stats != null)
            {
                var statLines = stats.BuildStatLines();
                if (statLines.Count > 0)
                    sections.Add(string.Join("\n", statLines));

                var effectLines = BuildEffectDescriptions(stats.effects);
                if (effectLines.Count > 0)
                    sections.Add(string.Join("\n", effectLines));
            }

            return JoinSections(sections);
        }

        private static List<string> BuildEffectDescriptions(List<Effect> effects)
        {
            var lines = new List<string>();
            if (effects == null)
                return lines;

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;

                if (!effect.IsValid())
                    continue;

                string description = effect.BuildDescription();
                if (string.IsNullOrWhiteSpace(description))
                    continue;

                lines.Add(description);
            }

            return lines;
        }

        private static string JoinSections(List<string> sections)
        {
            var builder = new StringBuilder();

            foreach (var section in sections)
            {
                if (string.IsNullOrWhiteSpace(section))
                    continue;

                if (builder.Length > 0)
                    builder.AppendLine().AppendLine();

                builder.Append(section);
            }

            return builder.ToString();
        }
    }
}
