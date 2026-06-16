using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineMetricDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    internal sealed class VisionPipelineAcceptancePreset
    {
        public string Name { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
        public string[] ToolTypes { get; set; } = Array.Empty<string>();
        public bool UseMinimum { get; set; }
        public double Minimum { get; set; }
        public bool UseMaximum { get; set; }
        public double Maximum { get; set; }
        public double MaxElapsedMilliseconds { get; set; }
    }

    internal static class VisionPipelineKnownMetrics
    {
        public const string ResultCount = "ResultCount";
        public const string AreaMin = "AreaMin";
        public const string AreaMax = "AreaMax";
        public const string AreaAvg = "AreaAvg";
        public const string ScoreMin = "ScoreMin";
        public const string ScoreMax = "ScoreMax";
        public const string ScoreAvg = "ScoreAvg";
        public const string AngleMin = "AngleMin";
        public const string AngleMax = "AngleMax";
        public const string AngleAvg = "AngleAvg";
        public const string MeanValueMin = "MeanValueMin";
        public const string MeanValueMax = "MeanValueMax";
        public const string MeanValueAvg = "MeanValueAvg";
        public const string EdgeCount = "EdgeCount";
        public const string EdgePointCount = "EdgePointCount";
        public const string LineLengthMin = "LineLengthMin";
        public const string LineLengthMax = "LineLengthMax";
        public const string LineLengthAvg = "LineLengthAvg";
        public const string LineLengthMmMin = "LineLengthMmMin";
        public const string LineLengthMmMax = "LineLengthMmMax";
        public const string LineLengthMmAvg = "LineLengthMmAvg";
        public const string LineAngleMin = "LineAngleMin";
        public const string LineAngleMax = "LineAngleMax";
        public const string LineAngleAvg = "LineAngleAvg";
        public const string MergeOverlayCount = "MergeOverlayCount";
        public const string MergeSourceCount = "MergeSourceCount";
        public const string BoundsWidthMin = "BoundsWidthMin";
        public const string BoundsWidthMax = "BoundsWidthMax";
        public const string BoundsWidthAvg = "BoundsWidthAvg";
        public const string BoundsWidthMmMin = "BoundsWidthMmMin";
        public const string BoundsWidthMmMax = "BoundsWidthMmMax";
        public const string BoundsWidthMmAvg = "BoundsWidthMmAvg";
        public const string BoundsHeightMin = "BoundsHeightMin";
        public const string BoundsHeightMax = "BoundsHeightMax";
        public const string BoundsHeightAvg = "BoundsHeightAvg";
        public const string BoundsHeightMmMin = "BoundsHeightMmMin";
        public const string BoundsHeightMmMax = "BoundsHeightMmMax";
        public const string BoundsHeightMmAvg = "BoundsHeightMmAvg";
        public const string SourceImageWidth = "SourceImageWidth";
        public const string SourceImageHeight = "SourceImageHeight";
        public const string SourceImageChannels = "SourceImageChannels";
        public const string ResultImageWidth = "ResultImageWidth";
        public const string ResultImageHeight = "ResultImageHeight";
        public const string ResultImageChannels = "ResultImageChannels";

        private static readonly VisionPipelineMetricDefinition[] MetricDefinitions =
        {
            new VisionPipelineMetricDefinition { Name = ResultCount, DisplayName = "Result Count", Description = "Number of result items detected by the tool." },
            new VisionPipelineMetricDefinition { Name = AreaMin, DisplayName = "Area Min", Description = "Minimum detected area." },
            new VisionPipelineMetricDefinition { Name = AreaMax, DisplayName = "Area Max", Description = "Maximum detected area." },
            new VisionPipelineMetricDefinition { Name = AreaAvg, DisplayName = "Area Avg", Description = "Average detected area." },
            new VisionPipelineMetricDefinition { Name = ScoreMin, DisplayName = "Score Min", Description = "Minimum matching score." },
            new VisionPipelineMetricDefinition { Name = ScoreMax, DisplayName = "Score Max", Description = "Maximum matching score." },
            new VisionPipelineMetricDefinition { Name = ScoreAvg, DisplayName = "Score Avg", Description = "Average matching score." },
            new VisionPipelineMetricDefinition { Name = AngleMin, DisplayName = "Angle Min", Description = "Minimum result angle." },
            new VisionPipelineMetricDefinition { Name = AngleMax, DisplayName = "Angle Max", Description = "Maximum result angle." },
            new VisionPipelineMetricDefinition { Name = AngleAvg, DisplayName = "Angle Avg", Description = "Average result angle." },
            new VisionPipelineMetricDefinition { Name = MeanValueMin, DisplayName = "Mean Min", Description = "Minimum mean value." },
            new VisionPipelineMetricDefinition { Name = MeanValueMax, DisplayName = "Mean Max", Description = "Maximum mean value." },
            new VisionPipelineMetricDefinition { Name = MeanValueAvg, DisplayName = "Mean Avg", Description = "Average mean value." },
            new VisionPipelineMetricDefinition { Name = EdgeCount, DisplayName = "Edge Count", Description = "Number of edge groups." },
            new VisionPipelineMetricDefinition { Name = EdgePointCount, DisplayName = "Edge Point Count", Description = "Total number of edge points." },
            new VisionPipelineMetricDefinition { Name = LineLengthMin, DisplayName = "Line Length Min", Description = "Minimum fitted line overlay length." },
            new VisionPipelineMetricDefinition { Name = LineLengthMax, DisplayName = "Line Length Max", Description = "Maximum fitted line overlay length." },
            new VisionPipelineMetricDefinition { Name = LineLengthAvg, DisplayName = "Line Length Avg", Description = "Average fitted line overlay length." },
            new VisionPipelineMetricDefinition { Name = LineLengthMmMin, DisplayName = "Line Length Min (mm)", Description = "Minimum fitted line overlay length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = LineLengthMmMax, DisplayName = "Line Length Max (mm)", Description = "Maximum fitted line overlay length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = LineLengthMmAvg, DisplayName = "Line Length Avg (mm)", Description = "Average fitted line overlay length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = LineAngleMin, DisplayName = "Line Angle Min", Description = "Minimum fitted line overlay angle in degrees." },
            new VisionPipelineMetricDefinition { Name = LineAngleMax, DisplayName = "Line Angle Max", Description = "Maximum fitted line overlay angle in degrees." },
            new VisionPipelineMetricDefinition { Name = LineAngleAvg, DisplayName = "Line Angle Avg", Description = "Average fitted line overlay angle in degrees." },
            new VisionPipelineMetricDefinition { Name = MergeOverlayCount, DisplayName = "Merge Overlay Count", Description = "Number of overlays collected into the merged result." },
            new VisionPipelineMetricDefinition { Name = MergeSourceCount, DisplayName = "Merge Source Count", Description = "Number of previous steps that contributed overlays." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMin, DisplayName = "Bounds Width Min", Description = "Minimum rectangle overlay width." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMax, DisplayName = "Bounds Width Max", Description = "Maximum rectangle overlay width." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthAvg, DisplayName = "Bounds Width Avg", Description = "Average rectangle overlay width." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMmMin, DisplayName = "Bounds Width Min (mm)", Description = "Minimum rectangle overlay width converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMmMax, DisplayName = "Bounds Width Max (mm)", Description = "Maximum rectangle overlay width converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMmAvg, DisplayName = "Bounds Width Avg (mm)", Description = "Average rectangle overlay width converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMin, DisplayName = "Bounds Height Min", Description = "Minimum rectangle overlay height." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMax, DisplayName = "Bounds Height Max", Description = "Maximum rectangle overlay height." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightAvg, DisplayName = "Bounds Height Avg", Description = "Average rectangle overlay height." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMmMin, DisplayName = "Bounds Height Min (mm)", Description = "Minimum rectangle overlay height converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMmMax, DisplayName = "Bounds Height Max (mm)", Description = "Maximum rectangle overlay height converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMmAvg, DisplayName = "Bounds Height Avg (mm)", Description = "Average rectangle overlay height converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = SourceImageWidth, DisplayName = "Source Width", Description = "Input image width used by the tool." },
            new VisionPipelineMetricDefinition { Name = SourceImageHeight, DisplayName = "Source Height", Description = "Input image height used by the tool." },
            new VisionPipelineMetricDefinition { Name = SourceImageChannels, DisplayName = "Source Channels", Description = "Input image channel count used by the tool." },
            new VisionPipelineMetricDefinition { Name = ResultImageWidth, DisplayName = "Result Width", Description = "Result image width returned by the tool." },
            new VisionPipelineMetricDefinition { Name = ResultImageHeight, DisplayName = "Result Height", Description = "Result image height returned by the tool." },
            new VisionPipelineMetricDefinition { Name = ResultImageChannels, DisplayName = "Result Channels", Description = "Result image channel count returned by the tool." }
        };

        private static readonly string[] ImageMetricNames =
        {
            SourceImageWidth,
            SourceImageHeight,
            SourceImageChannels,
            ResultImageWidth,
            ResultImageHeight,
            ResultImageChannels
        };

        private static readonly string[] RectangleOverlayMetricNames =
        {
            BoundsWidthMin,
            BoundsWidthMax,
            BoundsWidthAvg,
            BoundsWidthMmMin,
            BoundsWidthMmMax,
            BoundsWidthMmAvg,
            BoundsHeightMin,
            BoundsHeightMax,
            BoundsHeightAvg,
            BoundsHeightMmMin,
            BoundsHeightMmMax,
            BoundsHeightMmAvg
        };

        private static readonly string[] LineOverlayMetricNames =
        {
            LineLengthMin,
            LineLengthMax,
            LineLengthAvg,
            LineLengthMmMin,
            LineLengthMmMax,
            LineLengthMmAvg,
            LineAngleMin,
            LineAngleMax,
            LineAngleAvg
        };

        private static readonly Dictionary<string, string[]> ToolMetricNames = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["blob"] = WithImageAndRectangleMetrics(ResultCount, AreaMin, AreaMax, AreaAvg, AngleMin, AngleMax, AngleAvg),
            ["contour"] = WithImageAndRectangleMetrics(ResultCount, AreaMin, AreaMax, AreaAvg, AngleMin, AngleMax, AngleAvg),
            ["corner"] = WithImageAndRectangleMetrics(ResultCount, AreaMin, AreaMax, AreaAvg),
            ["matching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["templatematching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["feature"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["featurematching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["sift"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["mean"] = WithImageAndRectangleMetrics(ResultCount, MeanValueMin, MeanValueMax, MeanValueAvg),
            ["line"] = WithImageAndLineMetrics(ResultCount, EdgeCount, EdgePointCount),
            ["linegauge"] = WithImageAndLineMetrics(ResultCount, EdgeCount, EdgePointCount),
            ["threshold"] = ImageMetricNames,
            ["morphology"] = ImageMetricNames,
            ["filter"] = ImageMetricNames,
            ["edgedetection"] = ImageMetricNames,
            ["edge"] = ImageMetricNames,
            ["rotatescale"] = ImageMetricNames,
            ["rotateandscale"] = ImageMetricNames,
            ["overlaymerge"] = WithImageAndRectangleMetrics(ResultCount, MergeOverlayCount, MergeSourceCount),
            ["resultmerge"] = WithImageAndRectangleMetrics(ResultCount, MergeOverlayCount, MergeSourceCount),
            ["mergeresult"] = WithImageAndRectangleMetrics(ResultCount, MergeOverlayCount, MergeSourceCount)
        };

        private static readonly VisionPipelineAcceptancePreset[] Presets =
        {
            new VisionPipelineAcceptancePreset { Name = "Fast Step <= 100 ms", MaxElapsedMilliseconds = 100 },
            new VisionPipelineAcceptancePreset { Name = "Detect Count >= 1", MetricName = ResultCount, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 1 },
            new VisionPipelineAcceptancePreset { Name = "Detect Count = 0", MetricName = ResultCount, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 0, UseMaximum = true, Maximum = 0 },
            new VisionPipelineAcceptancePreset { Name = "Text/Symbol Count 35..80", MetricName = ResultCount, ToolTypes = new[] { "contour", "blob" }, UseMinimum = true, Minimum = 35, UseMaximum = true, Maximum = 80, MaxElapsedMilliseconds = 1000 },
            new VisionPipelineAcceptancePreset { Name = "Area Avg 150..600", MetricName = AreaAvg, ToolTypes = new[] { "blob", "contour", "corner" }, UseMinimum = true, Minimum = 150, UseMaximum = true, Maximum = 600 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width <= 20 px", MetricName = BoundsWidthMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "feature", "featurematching", "sift" }, UseMaximum = true, Maximum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width >= 20 px", MetricName = BoundsWidthMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height <= 20 px", MetricName = BoundsHeightMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "feature", "featurematching", "sift" }, UseMaximum = true, Maximum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height >= 20 px", MetricName = BoundsHeightMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Best Score >= 80", MetricName = ScoreMax, ToolTypes = new[] { "matching", "templatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 80 },
            new VisionPipelineAcceptancePreset { Name = "Best Score >= 60", MetricName = ScoreMax, ToolTypes = new[] { "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 60 },
            new VisionPipelineAcceptancePreset { Name = "Mean <= 180", MetricName = MeanValueAvg, ToolTypes = new[] { "mean" }, UseMaximum = true, Maximum = 180 },
            new VisionPipelineAcceptancePreset { Name = "Line Edge Count >= 1", MetricName = EdgeCount, ToolTypes = new[] { "line", "linegauge" }, UseMinimum = true, Minimum = 1 },
            new VisionPipelineAcceptancePreset { Name = "Fitted Line Length >= 100 px", MetricName = LineLengthMax, ToolTypes = new[] { "line", "linegauge" }, UseMinimum = true, Minimum = 100 },
            new VisionPipelineAcceptancePreset { Name = "Fitted Line Length >= 3 mm", MetricName = LineLengthMmMax, ToolTypes = new[] { "line", "linegauge" }, UseMinimum = true, Minimum = 3 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width <= 0.12 mm", MetricName = BoundsWidthMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMaximum = true, Maximum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width >= 0.12 mm", MetricName = BoundsWidthMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMinimum = true, Minimum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height <= 0.12 mm", MetricName = BoundsHeightMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMaximum = true, Maximum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height >= 0.12 mm", MetricName = BoundsHeightMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMinimum = true, Minimum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Merged Overlay Count >= 1", MetricName = MergeOverlayCount, ToolTypes = new[] { "overlaymerge", "resultmerge", "mergeresult" }, UseMinimum = true, Minimum = 1 }
        };

        public static IReadOnlyList<string> GetMetricNames()
        {
            return MetricDefinitions.Select(metric => metric.Name).ToArray();
        }

        private static string[] WithImageMetrics(params string[] metricNames)
        {
            return (metricNames ?? Array.Empty<string>())
                .Concat(ImageMetricNames)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string[] WithImageAndRectangleMetrics(params string[] metricNames)
        {
            return (metricNames ?? Array.Empty<string>())
                .Concat(RectangleOverlayMetricNames)
                .Concat(ImageMetricNames)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string[] WithImageAndLineMetrics(params string[] metricNames)
        {
            return (metricNames ?? Array.Empty<string>())
                .Concat(LineOverlayMetricNames)
                .Concat(ImageMetricNames)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static IReadOnlyList<VisionPipelineMetricDefinition> GetMetricDefinitions()
        {
            return MetricDefinitions;
        }

        public static string GetDisplayName(string metricName)
        {
            if (string.IsNullOrWhiteSpace(metricName))
            {
                return string.Empty;
            }

            VisionPipelineMetricDefinition definition = MetricDefinitions.FirstOrDefault(metric =>
                string.Equals(metric.Name, metricName, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(definition?.DisplayName)
                ? metricName
                : definition.DisplayName;
        }

        public static IReadOnlyList<string> GetMetricNamesForTool(string toolType)
        {
            string normalized = NormalizeToolType(toolType);
            return ToolMetricNames.TryGetValue(normalized, out string[] metricNames)
                ? metricNames
                : GetMetricNames();
        }

        public static bool IsKnownMetric(string metricName)
        {
            return MetricDefinitions.Any(metric =>
                string.Equals(metric.Name, metricName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsMetricRecommendedForTool(string toolType, string metricName)
        {
            if (string.IsNullOrWhiteSpace(metricName))
            {
                return true;
            }

            string normalized = NormalizeToolType(toolType);
            if (!ToolMetricNames.TryGetValue(normalized, out string[] metricNames))
            {
                return true;
            }

            return metricNames.Any(metric =>
                string.Equals(metric, metricName, StringComparison.OrdinalIgnoreCase));
        }

        public static string FormatMetricListForTool(string toolType)
        {
            IReadOnlyList<string> metricNames = GetMetricNamesForTool(toolType);
            return metricNames.Count == 0 ? "(none)" : string.Join(", ", metricNames);
        }

        public static IReadOnlyList<VisionPipelineAcceptancePreset> GetPresets()
        {
            return Presets;
        }

        public static IReadOnlyList<VisionPipelineAcceptancePreset> GetPresetsForTool(string toolType)
        {
            string normalized = NormalizeToolType(toolType);
            return Presets
                .Where(preset => AppliesToTool(preset, normalized))
                .ToArray();
        }

        public static void ApplyPreset(VisionPipelineStep step, VisionPipelineAcceptancePreset preset)
        {
            if (step == null || preset == null)
            {
                return;
            }

            step.UseAcceptance = true;
            step.ExpectedSuccess = true;
            step.RequiredMessageText = string.Empty;
            step.AcceptanceMetricName = preset.MetricName;
            step.UseAcceptanceMetricMinimum = preset.UseMinimum;
            step.AcceptanceMetricMinimum = preset.Minimum;
            step.UseAcceptanceMetricMaximum = preset.UseMaximum;
            step.AcceptanceMetricMaximum = preset.Maximum;
            step.MaxElapsedMilliseconds = preset.MaxElapsedMilliseconds;
        }

        public static void ClearAcceptance(VisionPipelineStep step)
        {
            if (step == null)
            {
                return;
            }

            step.UseAcceptance = false;
            step.ExpectedSuccess = true;
            step.MaxElapsedMilliseconds = 0;
            step.RequiredMessageText = string.Empty;
            step.AcceptanceMetricName = string.Empty;
            step.UseAcceptanceMetricMinimum = false;
            step.AcceptanceMetricMinimum = 0;
            step.UseAcceptanceMetricMaximum = false;
            step.AcceptanceMetricMaximum = 0;
        }

        public static string FormatMetrics(IDictionary<string, double> metrics)
        {
            if (metrics == null || metrics.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(
                ", ",
                OrderMetrics(metrics)
                    .Select(metric => $"{metric.Key}={metric.Value:0.###}"));
        }

        public static IEnumerable<KeyValuePair<string, double>> OrderMetrics(IDictionary<string, double> metrics)
        {
            if (metrics == null)
            {
                return Enumerable.Empty<KeyValuePair<string, double>>();
            }

            Dictionary<string, int> orderMap = MetricDefinitions
                .Select((metric, index) => new { metric.Name, Index = index })
                .ToDictionary(metric => metric.Name, metric => metric.Index, StringComparer.OrdinalIgnoreCase);

            return metrics
                .Where(metric => !string.IsNullOrWhiteSpace(metric.Key))
                .OrderBy(metric => orderMap.TryGetValue(metric.Key, out int index) ? index : int.MaxValue)
                .ThenBy(metric => metric.Key);
        }

        private static bool AppliesToTool(VisionPipelineAcceptancePreset preset, string normalizedToolType)
        {
            if (preset == null)
            {
                return false;
            }

            if (preset.ToolTypes == null || preset.ToolTypes.Length == 0)
            {
                return true;
            }

            return preset.ToolTypes.Any(toolType =>
                string.Equals(NormalizeToolType(toolType), normalizedToolType, StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeToolType(string toolType)
        {
            return (toolType ?? string.Empty)
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .Trim()
                .ToLowerInvariant();
        }
    }
}
