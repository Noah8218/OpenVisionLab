using OpenVisionLab.Vision2D.Pipeline;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static class VisionPipelineMultiMatchMeanPropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            VisionPipelinePropertyContext context,
            out object property)
        {
            property = null;
            if (!VisionPipelineMultiMatchMeanService.IsMultiMatchMean(
                    step?.ToolType))
            {
                return false;
            }

            property = new MultiMatchMeanProperty(
                step,
                name,
                context ?? VisionPipelinePropertyContext.Empty);
            return true;
        }

        public static bool TryCreateStep(
            object property,
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep step)
        {
            if (property is MultiMatchMeanProperty multiMatch)
            {
                step = multiMatch.ToStep(inputLayer, outputLayer);
                return true;
            }

            step = null;
            return false;
        }

        public static bool IsProperty(object property)
        {
            return property is MultiMatchMeanProperty;
        }

        public sealed class MultiMatchSourceConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(
                ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(
                ITypeDescriptorContext context)
            {
                return false;
            }

            public override StandardValuesCollection GetStandardValues(
                ITypeDescriptorContext context)
            {
                return context?.Instance is MultiMatchMeanProperty property
                    ? new StandardValuesCollection(
                        property.Context
                            .GetCompatibleMultiMatchSourceSteps()
                            .ToArray())
                    : new StandardValuesCollection(Array.Empty<string>());
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Source instances", 0)]
        [CategoryOrder("Reference frame", 1)]
        [CategoryOrder("Relative Mean ROI", 2)]
        [CategoryOrder("Instance contract", 3)]
        [CategoryOrder("Pose gates", 4)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class MultiMatchMeanProperty :
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            private readonly Dictionary<string, string> baselineParameters;

            public MultiMatchMeanProperty(
                VisionPipelineStep step,
                string name,
                VisionPipelinePropertyContext context)
            {
                Context = context;
                baselineParameters = new Dictionary<string, string>(
                    step?.Parameters ?? new Dictionary<string, string>(),
                    StringComparer.OrdinalIgnoreCase);
                PipelineStepName = string.IsNullOrWhiteSpace(name)
                    ? "MultiMatchMean"
                    : name;
                InputLayer = string.IsNullOrWhiteSpace(step?.InputLayer)
                    ? "Main"
                    : step.InputLayer;
                OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer)
                    ? "MultiMatchMean_Output"
                    : step.OutputLayer;
                SourceStep = GetString(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.SourceStepParameter,
                    string.Empty);
                ReferenceX = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.ReferenceXParameter,
                    0D);
                ReferenceY = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.ReferenceYParameter,
                    0D);
                ReferenceAngleDeg = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.ReferenceAngleParameter,
                    0D);
                ReferenceScale = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.ReferenceScaleParameter,
                    1D);
                ReferenceImageWidth = GetInt(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.ReferenceImageWidthParameter,
                    0);
                ReferenceImageHeight = GetInt(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.ReferenceImageHeightParameter,
                    0);
                RelativeRoi = GetRect(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.RelativeRoiParameter,
                    new Rect());
                MinimumInstances = GetInt(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MinimumInstancesParameter,
                    1);
                MaximumInstances = GetInt(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MaximumInstancesParameter,
                    8);
                RowTolerancePx = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.RowToleranceParameter,
                    20D);
                MaximumOverlapRatio = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MaximumOverlapParameter,
                    0.20D);
                MinimumMean = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MinimumMeanParameter,
                    0D);
                MaximumMean = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MaximumMeanParameter,
                    255D);
                RequireAll = GetBool(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.RequireAllParameter,
                    true);
                MinimumPassCount = GetInt(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MinimumPassCountParameter,
                    1);
                MaximumAngleDeltaDeg = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MaximumAngleDeltaParameter,
                    10D);
                MinimumScaleRatio = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MinimumScaleRatioParameter,
                    0.8D);
                MaximumScaleRatio = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MaximumScaleRatioParameter,
                    1.2D);
                MinimumValidPixelRatio = GetDouble(
                    step?.Parameters,
                    VisionPipelineMultiMatchMeanService.MinimumValidPixelRatioParameter,
                    0.5D);
            }

            [Browsable(false)]
            public VisionPipelinePropertyContext Context { get; }

            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string PipelineStepName { get; set; }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "MultiMatchMean_Output";

            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [Category("Source instances")]
            [DisplayName("Matching source Step")]
            [Description("Exact earlier Matching/EdgeBasedMatching Step with NUM_MATCH >= 2 on the same input layer.")]
            [TypeConverter(typeof(MultiMatchSourceConverter))]
            public string SourceStep { get; set; } = string.Empty;

            [Category("Reference frame")]
            [DisplayName("Reference center X")]
            public double ReferenceX { get; set; }

            [Category("Reference frame")]
            [DisplayName("Reference center Y")]
            public double ReferenceY { get; set; }

            [Category("Reference frame")]
            [DisplayName("Reference angle (deg)")]
            public double ReferenceAngleDeg { get; set; }

            [Category("Reference frame")]
            [DisplayName("Reference scale")]
            public double ReferenceScale { get; set; } = 1D;

            [Category("Reference frame")]
            [DisplayName("Reference image width")]
            public int ReferenceImageWidth { get; set; }

            [Category("Reference frame")]
            [DisplayName("Reference image height")]
            public int ReferenceImageHeight { get; set; }

            [Category("Relative Mean ROI")]
            [DisplayName("Reference ROI")]
            [Description("Fixed ROI in the taught reference image. Every accepted match is normalized before this same ROI is measured.")]
            public Rect RelativeRoi { get; set; }

            [Category("Relative Mean ROI")]
            [DisplayName("Minimum mean")]
            public double MinimumMean { get; set; }

            [Category("Relative Mean ROI")]
            [DisplayName("Maximum mean")]
            public double MaximumMean { get; set; } = 255D;

            [Category("Instance contract")]
            [DisplayName("Minimum instances")]
            public int MinimumInstances { get; set; } = 1;

            [Category("Instance contract")]
            [DisplayName("Maximum instances")]
            public int MaximumInstances { get; set; } = 8;

            [Category("Instance contract")]
            [DisplayName("Row tolerance (px)")]
            [Description("Centers within this vertical tolerance form one row, then receive stable left-to-right IDs.")]
            public double RowTolerancePx { get; set; } = 20D;

            [Category("Instance contract")]
            [DisplayName("Maximum overlap ratio")]
            public double MaximumOverlapRatio { get; set; } = 0.20D;

            [Category("Instance contract")]
            [DisplayName("Require all")]
            public bool RequireAll { get; set; } = true;

            [Category("Instance contract")]
            [DisplayName("Minimum pass count")]
            [Description("Used when Require all is false.")]
            public int MinimumPassCount { get; set; } = 1;

            [Category("Pose gates")]
            [DisplayName("Maximum angle delta (deg)")]
            public double MaximumAngleDeltaDeg { get; set; } = 10D;

            [Category("Pose gates")]
            [DisplayName("Minimum scale ratio")]
            public double MinimumScaleRatio { get; set; } = 0.8D;

            [Category("Pose gates")]
            [DisplayName("Maximum scale ratio")]
            public double MaximumScaleRatio { get; set; } = 1.2D;

            [Category("Pose gates")]
            [DisplayName("Minimum valid pixel ratio")]
            public double MinimumValidPixelRatio { get; set; } = 0.5D;

            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; } = true;

            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;

            [Category("Acceptance")]
            [DisplayName("Maximum Time (ms)")]
            public double MaxElapsedMilliseconds { get; set; }

            [Category("Acceptance")]
            [DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;

            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; }
                = VisionPipelineMultiMatchMeanService.InstanceAggregatePassedMetric;

            [Category("Acceptance")]
            [DisplayName("Use Metric Minimum")]
            public bool UseAcceptanceMetricMinimum { get; set; } = true;

            [Category("Acceptance")]
            [DisplayName("Metric Minimum")]
            public double AcceptanceMetricMinimum { get; set; } = 1D;

            [Category("Acceptance")]
            [DisplayName("Use Metric Maximum")]
            public bool UseAcceptanceMetricMaximum { get; set; } = true;

            [Category("Acceptance")]
            [DisplayName("Metric Maximum")]
            public double AcceptanceMetricMaximum { get; set; } = 1D;

            public VisionPipelineStep ToStep(
                string inputLayer,
                string outputLayer)
            {
                VisionPipelineStep step = new VisionPipelineStep
                {
                    Name = string.IsNullOrWhiteSpace(PipelineStepName)
                        ? "MultiMatchMean"
                        : PipelineStepName,
                    ToolType = "MultiMatchMean",
                    InputLayer = string.IsNullOrWhiteSpace(inputLayer)
                        ? InputLayer
                        : inputLayer,
                    OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                        ? OutputLayer
                        : outputLayer
                };
                foreach (KeyValuePair<string, string> item in baselineParameters)
                {
                    step.Parameters[item.Key] = item.Value;
                }

                Set(step, VisionPipelineMultiMatchMeanService.SourceStepParameter, SourceStep);
                Set(step, VisionPipelineMultiMatchMeanService.ReferenceXParameter, ReferenceX);
                Set(step, VisionPipelineMultiMatchMeanService.ReferenceYParameter, ReferenceY);
                Set(step, VisionPipelineMultiMatchMeanService.ReferenceAngleParameter, ReferenceAngleDeg);
                Set(step, VisionPipelineMultiMatchMeanService.ReferenceScaleParameter, ReferenceScale);
                Set(step, VisionPipelineMultiMatchMeanService.ReferenceImageWidthParameter, ReferenceImageWidth);
                Set(step, VisionPipelineMultiMatchMeanService.ReferenceImageHeightParameter, ReferenceImageHeight);
                Set(step, VisionPipelineMultiMatchMeanService.RelativeRoiParameter, Format(RelativeRoi));
                Set(step, VisionPipelineMultiMatchMeanService.MinimumInstancesParameter, MinimumInstances);
                Set(step, VisionPipelineMultiMatchMeanService.MaximumInstancesParameter, MaximumInstances);
                Set(step, VisionPipelineMultiMatchMeanService.RowToleranceParameter, RowTolerancePx);
                Set(step, VisionPipelineMultiMatchMeanService.MaximumOverlapParameter, MaximumOverlapRatio);
                Set(step, VisionPipelineMultiMatchMeanService.MinimumMeanParameter, MinimumMean);
                Set(step, VisionPipelineMultiMatchMeanService.MaximumMeanParameter, MaximumMean);
                Set(step, VisionPipelineMultiMatchMeanService.RequireAllParameter, RequireAll);
                Set(step, VisionPipelineMultiMatchMeanService.MinimumPassCountParameter, MinimumPassCount);
                Set(step, VisionPipelineMultiMatchMeanService.MaximumAngleDeltaParameter, MaximumAngleDeltaDeg);
                Set(step, VisionPipelineMultiMatchMeanService.MinimumScaleRatioParameter, MinimumScaleRatio);
                Set(step, VisionPipelineMultiMatchMeanService.MaximumScaleRatioParameter, MaximumScaleRatio);
                Set(step, VisionPipelineMultiMatchMeanService.MinimumValidPixelRatioParameter, MinimumValidPixelRatio);
                return step;
            }
        }

        private static string GetString(
            IDictionary<string, string> parameters,
            string key,
            string fallback)
        {
            return parameters != null
                && parameters.TryGetValue(key, out string value)
                && !string.IsNullOrWhiteSpace(value)
                    ? value.Trim()
                    : fallback;
        }

        private static int GetInt(
            IDictionary<string, string> parameters,
            string key,
            int fallback)
        {
            return int.TryParse(
                GetString(parameters, key, string.Empty),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int value)
                    ? value
                    : fallback;
        }

        private static double GetDouble(
            IDictionary<string, string> parameters,
            string key,
            double fallback)
        {
            return double.TryParse(
                GetString(parameters, key, string.Empty),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double value)
                    ? value
                    : fallback;
        }

        private static bool GetBool(
            IDictionary<string, string> parameters,
            string key,
            bool fallback)
        {
            return bool.TryParse(
                GetString(parameters, key, string.Empty),
                out bool value)
                    ? value
                    : fallback;
        }

        private static Rect GetRect(
            IDictionary<string, string> parameters,
            string key,
            Rect fallback)
        {
            string[] parts = GetString(parameters, key, string.Empty).Split(',');
            return parts.Length == 4
                && int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                && int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                && int.TryParse(parts[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                && int.TryParse(parts[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int height)
                    ? new Rect(x, y, width, height)
                    : fallback;
        }

        private static string Format(Rect rect)
        {
            return string.Join(
                ",",
                rect.X.ToString(CultureInfo.InvariantCulture),
                rect.Y.ToString(CultureInfo.InvariantCulture),
                rect.Width.ToString(CultureInfo.InvariantCulture),
                rect.Height.ToString(CultureInfo.InvariantCulture));
        }

        private static void Set(
            VisionPipelineStep step,
            string key,
            object value)
        {
            step.Parameters[key] =
                Convert.ToString(value, CultureInfo.InvariantCulture)
                ?? string.Empty;
        }
    }
}
