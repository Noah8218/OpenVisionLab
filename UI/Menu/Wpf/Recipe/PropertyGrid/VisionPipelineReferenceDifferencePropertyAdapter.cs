using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static class VisionPipelineReferenceDifferencePropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            out object property)
        {
            property = null;
            if (step == null
                || !string.Equals(
                    NormalizeToolType(step.ToolType),
                    "referencedifference",
                    StringComparison.Ordinal))
            {
                return false;
            }

            property = AttachStepMetadata(
                new ReferenceDifferenceProperty
                {
                    ReferencePath1 = GetString(step.Parameters, "ReferencePath1", GetReferencePath(step.Parameters, 0)),
                    ReferencePath2 = GetString(step.Parameters, "ReferencePath2", GetReferencePath(step.Parameters, 1)),
                    ReferencePath3 = GetString(step.Parameters, "ReferencePath3", GetReferencePath(step.Parameters, 2)),
                    ReferencePath4 = GetString(step.Parameters, "ReferencePath4", GetReferencePath(step.Parameters, 3)),
                    DifferenceThreshold = GetInt(step.Parameters, "DifferenceThreshold", 35),
                    MinimumDefectArea = GetInt(step.Parameters, "MinimumDefectArea", 80),
                    MaximumDefectArea = GetInt(step.Parameters, "MaximumDefectArea", 20000),
                    MorphologyKernel = GetInt(step.Parameters, "MorphologyKernel", 3),
                    IgnoreBorder = GetInt(step.Parameters, "IgnoreBorder", 8),
                    OrbFeatures = GetInt(step.Parameters, "OrbFeatures", 1600),
                    MatchRatio = GetDouble(step.Parameters, "MatchRatio", 0.75),
                    MinimumInliers = GetInt(step.Parameters, "MinimumInliers", 12),
                    RansacThreshold = GetDouble(step.Parameters, "RansacThreshold", 3.0)
                },
                name,
                step.InputLayer,
                step.OutputLayer);
            return true;
        }

        public static bool TryCreateStep(
            object property,
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep step)
        {
            step = null;
            if (!(property is ReferenceDifferenceProperty referenceDifference))
            {
                return false;
            }

            step = new VisionPipelineStep
            {
                Name = string.IsNullOrWhiteSpace(referenceDifference.NAME)
                    ? "ReferenceDifference"
                    : referenceDifference.NAME,
                ToolType = "ReferenceDifference",
                InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer,
                OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                    ? "ReferenceDifference_Output"
                    : outputLayer
            };

            AddParameter(step.Parameters, "ReferencePath1", referenceDifference.ReferencePath1);
            AddParameter(step.Parameters, "ReferencePath2", referenceDifference.ReferencePath2);
            AddParameter(step.Parameters, "ReferencePath3", referenceDifference.ReferencePath3);
            AddParameter(step.Parameters, "ReferencePath4", referenceDifference.ReferencePath4);
            AddParameter(step.Parameters, "DifferenceThreshold", referenceDifference.DifferenceThreshold);
            AddParameter(step.Parameters, "MinimumDefectArea", referenceDifference.MinimumDefectArea);
            AddParameter(step.Parameters, "MaximumDefectArea", referenceDifference.MaximumDefectArea);
            AddParameter(step.Parameters, "MorphologyKernel", referenceDifference.MorphologyKernel);
            AddParameter(step.Parameters, "IgnoreBorder", referenceDifference.IgnoreBorder);
            AddParameter(step.Parameters, "OrbFeatures", referenceDifference.OrbFeatures);
            AddParameter(step.Parameters, "MatchRatio", referenceDifference.MatchRatio);
            AddParameter(step.Parameters, "MinimumInliers", referenceDifference.MinimumInliers);
            AddParameter(step.Parameters, "RansacThreshold", referenceDifference.RansacThreshold);
            return true;
        }

        public static bool IsProperty(object property)
        {
            return property is ReferenceDifferenceProperty;
        }

        private static T AttachStepMetadata<T>(
            T property,
            string name,
            string inputLayer,
            string outputLayer)
            where T : VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            property.PipelineStepName = string.IsNullOrWhiteSpace(name)
                ? property.PipelineStepName
                : name;
            property.InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer;
            property.OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                ? "Pipeline_Output"
                : outputLayer;
            return property;
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .ToLowerInvariant();
        }

        private static string GetReferencePath(
            IDictionary<string, string> parameters,
            int index)
        {
            string[] paths = GetString(parameters, "ReferencePaths", string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return index >= 0 && index < paths.Length ? paths[index].Trim() : string.Empty;
        }

        private static string GetString(
            IDictionary<string, string> parameters,
            string key,
            string defaultValue)
        {
            string value = GetValue(parameters, key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static int GetInt(
            IDictionary<string, string> parameters,
            string key,
            int defaultValue)
        {
            return int.TryParse(
                GetValue(parameters, key),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result)
                    ? result
                    : defaultValue;
        }

        private static double GetDouble(
            IDictionary<string, string> parameters,
            string key,
            double defaultValue)
        {
            return double.TryParse(
                GetValue(parameters, key),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double result)
                    ? result
                    : defaultValue;
        }

        private static string GetValue(
            IDictionary<string, string> parameters,
            string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }

            return null;
        }

        private static void AddParameter(
            IDictionary<string, string> parameters,
            string key,
            object value)
        {
            parameters[key] =
                Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Reference", 0)]
        [CategoryOrder("Defect", 1)]
        [CategoryOrder("Registration", 2)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class ReferenceDifferenceProperty :
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "ReferenceDifference";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "ReferenceDifference_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Reference")]
            [DisplayName("Reference Path 1")]
            [Description("Required approved Good reference image. Preview/Run remains an explicit user action.")]
            public string ReferencePath1 { get; set; } = string.Empty;

            [PropertyOrder(1)]
            [Category("Reference")]
            [DisplayName("Reference Path 2")]
            public string ReferencePath2 { get; set; } = string.Empty;

            [PropertyOrder(2)]
            [Category("Reference")]
            [DisplayName("Reference Path 3")]
            public string ReferencePath3 { get; set; } = string.Empty;

            [PropertyOrder(3)]
            [Category("Reference")]
            [DisplayName("Reference Path 4")]
            public string ReferencePath4 { get; set; } = string.Empty;

            [PropertyOrder(0)]
            [Category("Defect")]
            [DisplayName("Difference Threshold")]
            public int DifferenceThreshold { get; set; } = 35;

            [PropertyOrder(1)]
            [Category("Defect")]
            [DisplayName("Minimum Defect Area")]
            public int MinimumDefectArea { get; set; } = 80;

            [PropertyOrder(2)]
            [Category("Defect")]
            [DisplayName("Maximum Defect Area")]
            public int MaximumDefectArea { get; set; } = 20000;

            [PropertyOrder(3)]
            [Category("Defect")]
            [DisplayName("Morphology Kernel")]
            public int MorphologyKernel { get; set; } = 3;

            [PropertyOrder(4)]
            [Category("Defect")]
            [DisplayName("Ignore Border")]
            public int IgnoreBorder { get; set; } = 8;

            [PropertyOrder(0)]
            [Category("Registration")]
            [DisplayName("ORB Features")]
            public int OrbFeatures { get; set; } = 1600;

            [PropertyOrder(1)]
            [Category("Registration")]
            [DisplayName("Match Ratio")]
            public double MatchRatio { get; set; } = 0.75;

            [PropertyOrder(2)]
            [Category("Registration")]
            [DisplayName("Minimum Inliers")]
            public int MinimumInliers { get; set; } = 12;

            [PropertyOrder(3)]
            [Category("Registration")]
            [DisplayName("RANSAC Threshold")]
            public double RansacThreshold { get; set; } = 3.0;

            [PropertyOrder(1)]
            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; }

            [PropertyOrder(2)]
            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;

            [PropertyOrder(3)]
            [Category("Acceptance")]
            [DisplayName("Max Elapsed (ms)")]
            public double MaxElapsedMilliseconds { get; set; }

            [PropertyOrder(4)]
            [Category("Acceptance")]
            [DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;

            [PropertyOrder(5)]
            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            public double AcceptanceMetricMaximum { get; set; }
        }
    }
}
