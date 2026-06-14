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
            new VisionPipelineMetricDefinition { Name = EdgePointCount, DisplayName = "Edge Point Count", Description = "Total number of edge points." }
        };

        private static readonly Dictionary<string, string[]> ToolMetricNames = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["blob"] = new[] { ResultCount, AreaMin, AreaMax, AreaAvg, AngleMin, AngleMax, AngleAvg },
            ["contour"] = new[] { ResultCount, AreaMin, AreaMax, AreaAvg, AngleMin, AngleMax, AngleAvg },
            ["corner"] = new[] { ResultCount, AreaMin, AreaMax, AreaAvg },
            ["matching"] = new[] { ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg },
            ["templatematching"] = new[] { ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg },
            ["feature"] = new[] { ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg },
            ["featurematching"] = new[] { ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg },
            ["sift"] = new[] { ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg },
            ["mean"] = new[] { ResultCount, MeanValueMin, MeanValueMax, MeanValueAvg },
            ["line"] = new[] { ResultCount, EdgeCount, EdgePointCount },
            ["linegauge"] = new[] { ResultCount, EdgeCount, EdgePointCount },
            ["threshold"] = Array.Empty<string>(),
            ["morphology"] = Array.Empty<string>(),
            ["filter"] = Array.Empty<string>(),
            ["edgedetection"] = Array.Empty<string>(),
            ["edge"] = Array.Empty<string>()
        };

        private static readonly VisionPipelineAcceptancePreset[] Presets =
        {
            new VisionPipelineAcceptancePreset { Name = "Detect Count >= 1", MetricName = ResultCount, UseMinimum = true, Minimum = 1 },
            new VisionPipelineAcceptancePreset { Name = "Detect Count = 0", MetricName = ResultCount, UseMinimum = true, Minimum = 0, UseMaximum = true, Maximum = 0 },
            new VisionPipelineAcceptancePreset { Name = "Text/Symbol Count 35..80", MetricName = ResultCount, UseMinimum = true, Minimum = 35, UseMaximum = true, Maximum = 80, MaxElapsedMilliseconds = 1000 },
            new VisionPipelineAcceptancePreset { Name = "Area Avg 150..600", MetricName = AreaAvg, UseMinimum = true, Minimum = 150, UseMaximum = true, Maximum = 600 },
            new VisionPipelineAcceptancePreset { Name = "Fast Step <= 100 ms", MaxElapsedMilliseconds = 100 },
            new VisionPipelineAcceptancePreset { Name = "Best Score >= 80", MetricName = ScoreMax, UseMinimum = true, Minimum = 80 },
            new VisionPipelineAcceptancePreset { Name = "Mean <= 180", MetricName = MeanValueAvg, UseMaximum = true, Maximum = 180 },
            new VisionPipelineAcceptancePreset { Name = "Line Edge Count >= 1", MetricName = EdgeCount, UseMinimum = true, Minimum = 1 }
        };

        public static IReadOnlyList<string> GetMetricNames()
        {
            return MetricDefinitions.Select(metric => metric.Name).ToArray();
        }

        public static IReadOnlyList<VisionPipelineMetricDefinition> GetMetricDefinitions()
        {
            return MetricDefinitions;
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

        public static void ApplyPreset(VisionPipelineStep step, VisionPipelineAcceptancePreset preset)
        {
            if (step == null || preset == null)
            {
                return;
            }

            step.UseAcceptance = true;
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
