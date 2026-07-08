using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionWorkspaceSampleLearnPathOption
    {
        private OpenVisionWorkspaceSampleLearnPathOption(
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
            LocalText("{0}개", "{0} samples"),
            SampleCount);

        public bool Matches(VisionPipelineSampleCatalogItem sample)
        {
            return OpenVisionWorkspaceSampleLearnPathClassifier.Matches(Id, sample);
        }

        public static IReadOnlyList<OpenVisionWorkspaceSampleLearnPathOption> Create(
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples)
        {
            IReadOnlyList<VisionPipelineSampleCatalogItem> source = samples ?? Array.Empty<VisionPipelineSampleCatalogItem>();
            List<OpenVisionWorkspaceSampleLearnPathOption> options = new List<OpenVisionWorkspaceSampleLearnPathOption>
            {
                new OpenVisionWorkspaceSampleLearnPathOption(
                    "all",
                    LocalText("전체", "All"),
                    LocalText("모든 검증 샘플", "All verification samples"),
                    source.Count)
            };

            AddIfNotEmpty(options, source, "preprocess", LocalText("전처리", "Preprocess"), LocalText("Threshold, Filter, Morphology로 검출 전 입력 정리", "Threshold, Filter, and Morphology before detection"));
            AddIfNotEmpty(options, source, "geometry", LocalText("Geometry", "Geometry"), LocalText("RotateScale transform and ROI coordinate review", "RotateScale transform and ROI coordinate review"));
            AddIfNotEmpty(options, source, "matching", LocalText("Matching 전체", "All matching"), LocalText("Template, Feature, Edge 기반 매칭 전체", "Template, feature, and edge matching"));
            AddIfNotEmpty(options, source, "template-matching", LocalText("Template", "Template"), LocalText("Template Matching으로 기준 형상/마크 찾기", "Find a reference shape or mark with Template Matching"));
            AddIfNotEmpty(options, source, "edge-matching", LocalText("Edge", "Edge"), LocalText("EdgeBasedMatching으로 형상 edge 비교", "Compare edge geometry with EdgeBasedMatching"));
            AddIfNotEmpty(options, source, "feature-matching", LocalText("Feature", "Feature"), LocalText("FeatureMatching으로 특징점 비교", "Compare keypoints with FeatureMatching"));
            AddIfNotEmpty(options, source, "blob", LocalText("Blob", "Blob"), LocalText("입자, 얼룩, 밀도 검출", "Particle, stain, and density detection"));
            AddIfNotEmpty(options, source, "contour", LocalText("Contour", "Contour"), LocalText("형상, 개수, 영역 검출", "Shape, count, and region detection"));
            AddIfNotEmpty(options, source, "line", LocalText("Line", "Line"), LocalText("거리, 각도, 직선 측정", "Distance, angle, and line measurement"));
            AddIfNotEmpty(options, source, "mean", LocalText("Mean", "Mean"), LocalText("밝기와 히스토그램 변화 측정", "Brightness and histogram drift measurement"));
            AddIfNotEmpty(options, source, "color-hsv", LocalText("Color / HSV", "Color / HSV"), LocalText("HSV 색상 범위 마스크 검증", "HSV color-range mask checks"));
            AddIfNotEmpty(options, source, "pair", LocalText("Good/Bad", "Good/Bad"), LocalText("OK/NG 샘플 쌍 검증", "OK/NG sample-pair validation"));

            return options;
        }

        private static void AddIfNotEmpty(
            ICollection<OpenVisionWorkspaceSampleLearnPathOption> options,
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples,
            string id,
            string displayName,
            string description)
        {
            int count = samples.Count(sample => OpenVisionWorkspaceSampleLearnPathClassifier.Matches(id, sample));
            if (count <= 0)
            {
                return;
            }

            options.Add(new OpenVisionWorkspaceSampleLearnPathOption(id, displayName, description, count));
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }

    internal static class OpenVisionWorkspaceSampleLearnPathClassifier
    {
        public static bool Matches(string learnPathId, VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return false;
            }

            string id = string.IsNullOrWhiteSpace(learnPathId) ? "all" : learnPathId.Trim();
            if (string.Equals(id, "all", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string flow = sample.ToolFlowText ?? string.Empty;
            string category = sample.Category ?? string.Empty;
            string name = sample.SampleName ?? string.Empty;
            string goal = sample.Goal ?? string.Empty;

            if (string.Equals(id, "matching", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, string.Empty, string.Empty, "Matching", "FeatureMatching", "EdgeBased", "Template matching", "Feature matching");
            }

            if (string.Equals(id, "template-matching", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "TemplateMatching", "Template Matching", "Image Matching")
                    || (ContainsAny(flow, category, name, goal, "Matching")
                        && !ContainsAny(flow, category, name, goal, "Feature", "EdgeBased", "Edge Based"));
            }

            if (string.Equals(id, "edge-matching", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "EdgeBased", "Edge Based", "EdgeBasedMatching");
            }

            if (string.Equals(id, "feature-matching", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "FeatureMatching", "Feature Matching", "Feature");
            }

            if (string.Equals(id, "preprocess", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, string.Empty, string.Empty, string.Empty, "Threshold", "Filter", "Morphology", "EdgeDetection")
                    || ContainsAny(category, name, goal, string.Empty, "Preprocess", "Binary", "Clean");
            }

            if (string.Equals(id, "geometry", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "RotateScale", "RotateAndScale", "Resize");
            }

            if (string.Equals(id, "blob", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "Blob", "Particle", "Density", "Stain");
            }

            if (string.Equals(id, "contour", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "Contour", "Shape", "Region", "Surface", "Fiducial");
            }

            if (string.Equals(id, "line", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "LineDistance", "LineGauge", "Distance", "Angle", "Measure");
            }

            if (string.Equals(id, "mean", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "Mean", "Brightness", "Histogram");
            }

            if (string.Equals(id, "color-hsv", StringComparison.OrdinalIgnoreCase))
            {
                return ContainsAny(flow, category, name, goal, "HSV", "Color range", "ColorPatch", "Color Patch");
            }

            if (string.Equals(id, "pair", StringComparison.OrdinalIgnoreCase))
            {
                return sample.HasPair;
            }

            return true;
        }

        private static bool ContainsAny(
            string first,
            string second,
            string third,
            string fourth,
            params string[] tokens)
        {
            foreach (string token in tokens)
            {
                if (Contains(first, token)
                    || Contains(second, token)
                    || Contains(third, token)
                    || Contains(fourth, token))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool Contains(string text, string token)
        {
            return !string.IsNullOrWhiteSpace(text)
                && !string.IsNullOrWhiteSpace(token)
                && text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
