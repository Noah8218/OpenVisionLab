using OpenVisionLab.Vision2D.Pipeline;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static class VisionPipelinePinArrayGapPropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            out object property)
        {
            property = null;
            string toolType = NormalizeToolType(step?.ToolType);
            if (toolType != "pinarraygap" && toolType != "adjacentpingap")
            {
                return false;
            }

            property = AttachStepMetadata(
                new PinArrayGapProperty(step, name),
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
            if (property is PinArrayGapProperty pinArrayGap)
            {
                step = pinArrayGap.ToStep(inputLayer, outputLayer);
                return true;
            }

            step = null;
            return false;
        }

        public static bool IsProperty(object property)
        {
            return property is PinArrayGapProperty;
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

        private static bool GetBool(
            IDictionary<string, string> parameters,
            string key,
            bool defaultValue)
        {
            return bool.TryParse(GetValue(parameters, key), out bool result)
                ? result
                : defaultValue;
        }

        private static TEnum GetEnum<TEnum>(
            IDictionary<string, string> parameters,
            string key,
            TEnum defaultValue)
            where TEnum : struct
        {
            return Enum.TryParse(GetValue(parameters, key), true, out TEnum result)
                ? result
                : defaultValue;
        }

        private static Rect GetRect(
            IDictionary<string, string> parameters,
            string key,
            Rect defaultValue)
        {
            string value = GetValue(parameters, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            string[] parts = value.Split(',');
            if (parts.Length != 4)
            {
                return defaultValue;
            }

            return int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                && int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                && int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height)
                    ? new Rect(x, y, width, height)
                    : defaultValue;
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
        [CategoryOrder("Measurement", 0)]
        [CategoryOrder("ROI", 1)]
        [CategoryOrder("Pin Detection", 2)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PinArrayGapProperty :
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            private readonly Dictionary<string, string> baselineParameters;
            private readonly string toolType;

            public PinArrayGapProperty(VisionPipelineStep step, string name)
            {
                baselineParameters = step?.Parameters == null
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(
                        step.Parameters,
                        StringComparer.OrdinalIgnoreCase);
                toolType = string.IsNullOrWhiteSpace(step?.ToolType)
                    ? "PinArrayGap"
                    : step.ToolType.Trim();
                NAME = string.IsNullOrWhiteSpace(name) ? "PinArrayGap" : name;
                MeasurementMode = GetEnum(
                    step?.Parameters,
                    "MeasurementMode",
                    PinArrayGapMeasurementMode.EdgeGap);
                UseRoi = GetBool(step?.Parameters, "USE_ROI", false);
                Roi = GetRect(step?.Parameters, "CvROI", default);
                DarkThreshold = GetInt(step?.Parameters, "DarkThreshold", 128);
                MinimumDarkCoverageRatio = GetDouble(
                    step?.Parameters,
                    "MinDarkCoverageRatio",
                    0.55D);
                MinimumPinWidth = GetInt(step?.Parameters, "MinPinWidth", 5);
                MaximumPinBreakWidth = GetInt(
                    step?.Parameters,
                    "MaxPinBreakWidth",
                    2);
                MinimumGapWidth = GetInt(step?.Parameters, "MinGapWidth", 3);
            }

            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; }

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
            public string OutputLayer { get; set; } = "PinArrayGap_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Measurement")]
            [DisplayName("Measurement mode")]
            [Description("EdgeGap measures adjacent empty clearance. CenterPitch measures adjacent detected pin centers. Both modes are pixel-based in this editor.")]
            public PinArrayGapMeasurementMode MeasurementMode { get; set; } =
                PinArrayGapMeasurementMode.EdgeGap;

            [PropertyOrder(0)]
            [Category("ROI")]
            [DisplayName("Use row ROI")]
            [Description("PinArrayGap requires one reviewed ROI containing exactly one dark pin row.")]
            public bool UseRoi { get; set; }

            [PropertyOrder(1)]
            [Category("ROI")]
            [DisplayName("Row ROI")]
            public Rect Roi { get; set; }

            [PropertyOrder(0)]
            [Category("Pin Detection")]
            [DisplayName("Dark threshold")]
            public int DarkThreshold { get; set; } = 128;

            [PropertyOrder(1)]
            [Category("Pin Detection")]
            [DisplayName("Minimum dark coverage ratio")]
            [Description("Minimum vertical dark-pixel coverage required for a column to belong to a pin.")]
            public double MinimumDarkCoverageRatio { get; set; } = 0.55D;

            [PropertyOrder(2)]
            [Category("Pin Detection")]
            [DisplayName("Minimum pin width")]
            public int MinimumPinWidth { get; set; } = 5;

            [PropertyOrder(3)]
            [Category("Pin Detection")]
            [DisplayName("Maximum pin break width")]
            [Description("Merge dark column runs separated by no more than this many pixels.")]
            public int MaximumPinBreakWidth { get; set; } = 2;

            [PropertyOrder(4)]
            [Category("Pin Detection")]
            [DisplayName("Minimum edge gap width")]
            [Description("Minimum empty clearance used by EdgeGap mode. CenterPitch does not use this filter.")]
            public int MinimumGapWidth { get; set; } = 3;

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

            [Browsable(false)]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            public double AcceptanceMetricMinimum { get; set; }

            [Browsable(false)]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [Browsable(false)]
            public double AcceptanceMetricMaximum { get; set; }

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                VisionPipelineStep step = new VisionPipelineStep
                {
                    Name = string.IsNullOrWhiteSpace(NAME) ? "PinArrayGap" : NAME,
                    ToolType = toolType,
                    InputLayer = string.IsNullOrWhiteSpace(inputLayer)
                        ? "Main"
                        : inputLayer,
                    OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                        ? "PinArrayGap_Output"
                        : outputLayer
                };
                foreach (KeyValuePair<string, string> parameter in baselineParameters)
                {
                    step.Parameters[parameter.Key] = parameter.Value;
                }

                AddParameter(step.Parameters, "MeasurementMode", MeasurementMode);
                AddParameter(step.Parameters, "USE_ROI", UseRoi);
                AddParameter(
                    step.Parameters,
                    "CvROI",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0},{1},{2},{3}",
                        Roi.X,
                        Roi.Y,
                        Roi.Width,
                        Roi.Height));
                AddParameter(step.Parameters, "DarkThreshold", DarkThreshold);
                AddParameter(
                    step.Parameters,
                    "MinDarkCoverageRatio",
                    MinimumDarkCoverageRatio);
                AddParameter(step.Parameters, "MinPinWidth", MinimumPinWidth);
                AddParameter(
                    step.Parameters,
                    "MaxPinBreakWidth",
                    MaximumPinBreakWidth);
                AddParameter(step.Parameters, "MinGapWidth", MinimumGapWidth);
                return step;
            }
        }
    }
}
