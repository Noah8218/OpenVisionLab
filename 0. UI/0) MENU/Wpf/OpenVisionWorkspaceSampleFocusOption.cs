using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionWorkspaceSampleFocusOption
    {
        private OpenVisionWorkspaceSampleFocusOption(
            string id,
            string displayName,
            string description,
            int sampleCount)
        {
            Id = id ?? string.Empty;
            DisplayName = displayName ?? string.Empty;
            Description = description ?? string.Empty;
            SampleCount = sampleCount;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public int SampleCount { get; }

        public string SampleCountText => string.Format(
            CultureInfo.CurrentCulture,
            LocalText("{0}\uac1c", "{0} samples"),
            SampleCount);

        public bool Matches(VisionPipelineSampleCatalogItem sample)
        {
            return Matches(Id, sample);
        }

        public static IReadOnlyList<OpenVisionWorkspaceSampleFocusOption> Create(
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples)
        {
            IReadOnlyList<VisionPipelineSampleCatalogItem> source = samples ?? Array.Empty<VisionPipelineSampleCatalogItem>();
            List<OpenVisionWorkspaceSampleFocusOption> options = new List<OpenVisionWorkspaceSampleFocusOption>
            {
                new OpenVisionWorkspaceSampleFocusOption(
                    "all",
                    LocalText("\uc804\uccb4", "All"),
                    LocalText("\ud604\uc7ac \uce74\ud0c8\ub85c\uadf8\uc758 \ubaa8\ub4e0 \uc0d8\ud50c\uc744 \ud45c\uc2dc\ud569\ub2c8\ub2e4.", "Show every sample in the selected catalog."),
                    source.Count)
            };

            AddIfNotEmpty(options, source, "battery", LocalText("\uc774\ucc28\uc804\uc9c0", "Battery"), LocalText("\ud0ed, \uc6a9\uc811, \ud30c\uc6b0\uce58, \uc804\uadf9 \uac80\uc0ac \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Tab, weld, pouch, and electrode inspection samples."));
            AddIfNotEmpty(options, source, "display", LocalText("\ub514\uc2a4\ud50c\ub808\uc774", "Display"), LocalText("\ud328\ub110, \uc5bc\ub77c\uc778, \uc2a4\ud06c\ub798\uce58, \ud328\ub4dc \uac80\uc0ac \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Panel, alignment, scratch, and pad inspection samples."));
            AddIfNotEmpty(options, source, "semiconductor", LocalText("\ubc18\ub3c4\uccb4", "Semiconductor"), LocalText("\ud328\ub4dc, \ub9ac\ub4dc, \ud328\ud0a4\uc9c0, \uc6e8\uc774\ud37c \uac80\uc0ac \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Pad, lead, package, and wafer inspection samples."));
            AddIfNotEmpty(options, source, "field", LocalText("\ud604\uc7a5\ud615", "Field"), LocalText("\ud604\uc7a5\uac10 \uc788\ub294 \uc81c\ud488 \uc774\ubbf8\uc9c0\ub85c \ub808\uc2dc\ud53c \ucd08\uc548\uc744 \uc7a1\ub294 Explore \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Field-style Explore samples for initial recipe setup."));
            AddIfNotEmpty(options, source, "matching", LocalText("\ub9e4\uce6d", "Matching"), LocalText("\uc774\ubbf8\uc9c0/\uc5e3\uc9c0/\ud2b9\uc9d5 \ub9e4\uce6d \uacc4\uc5f4 \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Image, edge, and feature matching samples."));
            AddIfNotEmpty(options, source, "blob", LocalText("\ube14\ub78d", "Blob"), LocalText("\uc785\uc790, \uc5bc\ub8e9, \ud6c4\ubcf4 \uac1c\uc218 \uac80\uc0ac \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Particle, stain, and candidate-count samples."));
            AddIfNotEmpty(options, source, "contour", LocalText("\ucee8\ud22c\uc5b4", "Contour"), LocalText("\ud615\uc0c1, \uc2a4\ud06c\ub798\uce58, \uce69, \uc724\uacfd \uac80\uc0ac \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Shape, scratch, chip, and outline samples."));
            AddIfNotEmpty(options, source, "line", LocalText("\uce58\uc218", "Measure"), LocalText("\uac70\ub9ac, \ud3ed, \uc815\ub82c \uce21\uc815 \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Distance, width, and alignment measurement samples."));
            AddIfNotEmpty(options, source, "mean", LocalText("\ubc1d\uae30", "Brightness"), LocalText("\ubc1d\uae30\uc640 \uc0c9\uc0c1 \ubcc0\ud654 \uac80\uc0ac \uc0d8\ud50c\uc785\ub2c8\ub2e4.", "Brightness and color drift samples."));

            return options;
        }

        public static bool Matches(string focusId, VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return false;
            }

            string id = string.IsNullOrWhiteSpace(focusId) ? "all" : focusId.Trim();
            if (string.Equals(id, "all", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string category = sample.Category ?? string.Empty;
            string flow = sample.ToolFlowText ?? string.Empty;
            string name = sample.SampleName ?? string.Empty;
            string goal = sample.Goal ?? string.Empty;

            if (string.Equals(id, "battery", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(category, name, goal, "Secondary Battery", "Battery_");
            }

            if (string.Equals(id, "display", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(category, name, goal, "Display", "Display_");
            }

            if (string.Equals(id, "semiconductor", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(category, name, goal, "Semiconductor", "Semiconductor_");
            }

            if (string.Equals(id, "field", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(sample.ValidationMode?.Trim(), "Explore", StringComparison.OrdinalIgnoreCase)
                    && ContainsAny(category, name, goal, "Product Field", "Field_", "field");
            }

            return OpenVisionWorkspaceSampleLearnPathClassifier.Matches(id, sample)
                || ContainsAny(flow, category, goal, id);
        }

        private static void AddIfNotEmpty(
            ICollection<OpenVisionWorkspaceSampleFocusOption> options,
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples,
            string id,
            string displayName,
            string description)
        {
            int count = samples.Count(sample => Matches(id, sample));
            if (count <= 0)
            {
                return;
            }

            options.Add(new OpenVisionWorkspaceSampleFocusOption(id, displayName, description, count));
        }

        private static bool ContainsAny(string first, string second, string third, params string[] tokens)
        {
            return tokens.Any(token => Contains(first, token) || Contains(second, token) || Contains(third, token));
        }

        private static bool Contains(string text, string token)
        {
            return !string.IsNullOrWhiteSpace(text)
                && !string.IsNullOrWhiteSpace(token)
                && text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }
}
