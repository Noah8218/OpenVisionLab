using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.WpfPropertyGrid;
using static Lib.Common.FormulaUtil;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static class VisionPipelineStepPropertyMapper
    {
        private static Func<IEnumerable<string>> layerNameAccessor = () => Enumerable.Empty<string>();

        public static void SetLayerNameContext(Func<IEnumerable<string>> accessor)
        {
            layerNameAccessor = accessor ?? (() => Enumerable.Empty<string>());
        }

        public static object CreateProperty(VisionPipelineStep step)
        {
            object property = CreatePropertyCore(step);
            if (property is IPipelineStepMetadata metadata && step != null)
            {
                metadata.Enabled = step.Enabled;
                metadata.UseAcceptance = step.UseAcceptance;
                metadata.ExpectedSuccess = step.ExpectedSuccess;
                metadata.MaxElapsedMilliseconds = step.MaxElapsedMilliseconds;
                metadata.RequiredMessageText = step.RequiredMessageText;
                metadata.AcceptanceMetricName = step.AcceptanceMetricName;
                metadata.UseAcceptanceMetricMinimum = step.UseAcceptanceMetricMinimum;
                metadata.AcceptanceMetricMinimum = step.AcceptanceMetricMinimum;
                metadata.UseAcceptanceMetricMaximum = step.UseAcceptanceMetricMaximum;
                metadata.AcceptanceMetricMaximum = step.AcceptanceMetricMaximum;
            }

            return property;
        }

        private static object CreatePropertyCore(VisionPipelineStep step)
        {
            if (step == null)
            {
                return null;
            }

            string name = GetStepName(step);
            string toolType = NormalizeToolType(step.ToolType);

            switch (toolType)
            {
                case "threshold":
                    return AttachStepMetadata(new PipelineThresholdToolProperty
                    {
                        Mode = GetEnum(step.Parameters, nameof(ThresholdToolProperty.Mode), ThresholdToolMode.Threshold),
                        Threshold = GetDouble(step.Parameters, nameof(ThresholdToolProperty.Threshold), 127),
                        MaxValue = GetDouble(step.Parameters, nameof(ThresholdToolProperty.MaxValue), 255),
                        ThresholdType = GetEnum(step.Parameters, nameof(ThresholdToolProperty.ThresholdType), ThresholdTypes.Binary),
                        RangeMin = GetInt(step.Parameters, nameof(ThresholdToolProperty.RangeMin), 1),
                        RangeMax = GetInt(step.Parameters, nameof(ThresholdToolProperty.RangeMax), 255),
                        Invert = GetBool(step.Parameters, nameof(ThresholdToolProperty.Invert), false),
                        AdaptiveType = GetEnum(step.Parameters, nameof(ThresholdToolProperty.AdaptiveType), AdaptiveThresholdTypes.MeanC),
                        AdaptiveThresholdType = GetEnum(step.Parameters, nameof(ThresholdToolProperty.AdaptiveThresholdType), ThresholdTypes.Binary),
                        BlockSize = GetInt(step.Parameters, nameof(ThresholdToolProperty.BlockSize), 25),
                        Weight = GetInt(step.Parameters, nameof(ThresholdToolProperty.Weight), 5)
                    }, name, step.InputLayer, step.OutputLayer);
                case "morphology":
                    return AttachStepMetadata(new PipelineMorphologyToolProperty
                    {
                        Shape = GetEnum(step.Parameters, nameof(MorphologyToolProperty.Shape), MorphShapes.Rect),
                        Operator = GetEnum(step.Parameters, nameof(MorphologyToolProperty.Operator), MorphTypes.Erode),
                        KernelWidth = GetInt(step.Parameters, nameof(MorphologyToolProperty.KernelWidth), 3),
                        KernelHeight = GetInt(step.Parameters, nameof(MorphologyToolProperty.KernelHeight), 3),
                        Iterations = GetInt(step.Parameters, nameof(MorphologyToolProperty.Iterations), 1)
                    }, name, step.InputLayer, step.OutputLayer);
                case "filter":
                    return AttachStepMetadata(new PipelineFilterToolProperty
                    {
                        FilterType = GetEnum(step.Parameters, nameof(FilterToolProperty.FilterType), FilterToolType.Blur),
                        KernelWidth = GetInt(step.Parameters, nameof(FilterToolProperty.KernelWidth), 3),
                        KernelHeight = GetInt(step.Parameters, nameof(FilterToolProperty.KernelHeight), 3),
                        MedianKernelSize = GetInt(step.Parameters, nameof(FilterToolProperty.MedianKernelSize), 3),
                        Diameter = GetInt(step.Parameters, nameof(FilterToolProperty.Diameter), 3),
                        SigmaColor = GetInt(step.Parameters, nameof(FilterToolProperty.SigmaColor), 3),
                        SigmaSpace = GetInt(step.Parameters, nameof(FilterToolProperty.SigmaSpace), 3),
                        BorderType = GetEnum(step.Parameters, nameof(FilterToolProperty.BorderType), BorderTypes.Reflect101)
                    }, name, step.InputLayer, step.OutputLayer);
                case "edgedetection":
                case "edge":
                    return AttachStepMetadata(new PipelineEdgeDetectionToolProperty
                    {
                        EdgeType = GetEnum(step.Parameters, nameof(EdgeDetectionToolProperty.EdgeType), EdgeDetectionToolType.Canny),
                        CannyThresholdLow = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.CannyThresholdLow), 100),
                        CannyThresholdHigh = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.CannyThresholdHigh), 200),
                        CannyApertureSize = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.CannyApertureSize), 3),
                        UseL2Gradient = GetBool(step.Parameters, nameof(EdgeDetectionToolProperty.UseL2Gradient), true),
                        SobelDegreeX = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.SobelDegreeX), 0),
                        SobelDegreeY = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.SobelDegreeY), 0),
                        SobelKernelSize = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.SobelKernelSize), 1),
                        ScharrDegreeX = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.ScharrDegreeX), 0),
                        ScharrDegreeY = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.ScharrDegreeY), 0),
                        LaplacianKernelSize = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.LaplacianKernelSize), 1)
                    }, name, step.InputLayer, step.OutputLayer);
                case "blob":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineBlobProperty(name)
                    {
                        MIN_AREA = GetInt(step.Parameters, nameof(BlobProperty.MIN_AREA), 200),
                        MAX_AREA = GetInt(step.Parameters, nameof(BlobProperty.MAX_AREA), 1000000)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "contour":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineContourProperty(name)
                    {
                        USE_APPROXPOLYDP = GetBool(step.Parameters, nameof(ContourProperty.USE_APPROXPOLYDP), false),
                        USE_DRAW_IMAGE = GetBool(step.Parameters, nameof(ContourProperty.USE_DRAW_IMAGE), false),
                        ApproximationModes = GetEnum(step.Parameters, nameof(ContourProperty.ApproximationModes), ContourApproximationModes.ApproxSimple),
                        DetectMode = GetEnum(step.Parameters, nameof(ContourProperty.DetectMode), RetrievalModes.List),
                        EPSILON = GetDouble(step.Parameters, nameof(ContourProperty.EPSILON), 0.01),
                        MIN_AREA = GetInt(step.Parameters, nameof(ContourProperty.MIN_AREA), 200),
                        MAX_AREA = GetInt(step.Parameters, nameof(ContourProperty.MAX_AREA), 1000000),
                        DrawThickness = GetInt(step.Parameters, nameof(ContourProperty.DrawThickness), 2),
                        ClrGridHtml = GetString(step.Parameters, nameof(ContourProperty.ClrGridHtml), "#000000")
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "line":
                case "linegauge":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineLineGaugeProperty(name)
                    {
                        PRJ_PORALITY = GetEnum(step.Parameters, nameof(LineGaugeProperty.PRJ_PORALITY), PROJECTION_POLARITY.BTOW),
                        PRJ_DIR = GetEnum(step.Parameters, nameof(LineGaugeProperty.PRJ_DIR), PROJECTION_DIR.X_LTOR),
                        CONTRAST = GetDouble(step.Parameters, nameof(LineGaugeProperty.CONTRAST), 30),
                        THICKNESS = GetDouble(step.Parameters, nameof(LineGaugeProperty.THICKNESS), 5),
                        SAMPLING_STEP = GetDouble(step.Parameters, nameof(LineGaugeProperty.SAMPLING_STEP), 10),
                        VER_PRJ_DIR = GetEnum(step.Parameters, nameof(LineGaugeProperty.VER_PRJ_DIR), PROJECTION_DIR.X_LTOR),
                        POINT_RANGE = GetInt(step.Parameters, nameof(LineGaugeProperty.POINT_RANGE), 10),
                        USE_MANUAL_ANGLE = GetBool(step.Parameters, nameof(LineGaugeProperty.USE_MANUAL_ANGLE), false),
                        MANUAL_ANGLE_VALUE = GetDouble(step.Parameters, nameof(LineGaugeProperty.MANUAL_ANGLE_VALUE), 0),
                        USE_EXTEND_FIT_LINE = GetBool(step.Parameters, nameof(LineGaugeProperty.USE_EXTEND_FIT_LINE), false),
                        EXTEND_FIT_LINE_VALUE = GetInt(step.Parameters, nameof(LineGaugeProperty.EXTEND_FIT_LINE_VALUE), 100),
                        AVERAGE_Diff = GetDouble(step.Parameters, nameof(LineGaugeProperty.AVERAGE_Diff), 100),
                        USE_AVERAGE_FILTER = GetBool(step.Parameters, nameof(LineGaugeProperty.USE_AVERAGE_FILTER), false),
                        AVERAGE_FILTER_TYPE = GetEnum(step.Parameters, nameof(LineGaugeProperty.AVERAGE_FILTER_TYPE), LineGaugeProperty.AVERAGE_FILTER_TYPES.Y),
                        SHOW_VERTICAL_LINE = GetBool(step.Parameters, nameof(LineGaugeProperty.SHOW_VERTICAL_LINE), true),
                        SHOW_EDGE = GetBool(step.Parameters, nameof(LineGaugeProperty.SHOW_EDGE), true),
                        SHOW_CONTOUR = GetBool(step.Parameters, nameof(LineGaugeProperty.SHOW_CONTOUR), true),
                        SHOW_FITLINE = GetBool(step.Parameters, nameof(LineGaugeProperty.SHOW_FITLINE), true)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "matching":
                case "templatematching":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineMatchingProperty(name)
                    {
                        MATCH_MODE = GetEnum(step.Parameters, nameof(MatchingProperty.MATCH_MODE), TemplateMatchModes.CCoeffNormed),
                        SCORE_MIN = GetDouble(step.Parameters, nameof(MatchingProperty.SCORE_MIN), 0.6),
                        MAGNIFIATION = GetDouble(step.Parameters, nameof(MatchingProperty.MAGNIFIATION), 1),
                        NUM_MATCH = GetInt(step.Parameters, nameof(MatchingProperty.NUM_MATCH), 3),
                        USE_FIND_ANGLE = GetBool(step.Parameters, nameof(MatchingProperty.USE_FIND_ANGLE), true),
                        FIND_ANGLE = GetDouble(step.Parameters, nameof(MatchingProperty.FIND_ANGLE), 0.1),
                        FIND_ANGLE_MAX = GetInt(step.Parameters, nameof(MatchingProperty.FIND_ANGLE_MAX), 10),
                        FIND_ANGLE_MIN = GetInt(step.Parameters, nameof(MatchingProperty.FIND_ANGLE_MIN), -10),
                        PATTERN_PATH = GetString(step.Parameters, nameof(MatchingProperty.PATTERN_PATH), GetString(step.Parameters, "TemplatePath", string.Empty)),
                        USE_CANNY = GetBool(step.Parameters, nameof(MatchingProperty.USE_CANNY), false),
                        CANNY_HIGH = GetInt(step.Parameters, nameof(MatchingProperty.CANNY_HIGH), 60),
                        CANNY_LOW = GetInt(step.Parameters, nameof(MatchingProperty.CANNY_LOW), 30),
                        USE_PADDING_COLOR_WHITE = GetBool(step.Parameters, nameof(MatchingProperty.USE_PADDING_COLOR_WHITE), false)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "mean":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineMeanProperty(name)
                    {
                        MEAN_MAX = GetInt(step.Parameters, nameof(MeanProperty.MEAN_MAX), 240),
                        MEAN_MIN = GetInt(step.Parameters, nameof(MeanProperty.MEAN_MIN), 100),
                        MEAN_TYPES = GetEnum(step.Parameters, nameof(MeanProperty.MEAN_TYPES), MeanType.Mean)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "feature":
                case "featurematching":
                case "sift":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineFeatureMatchingProperty(name)
                    {
                        SCORE_MIN = GetDouble(step.Parameters, nameof(FeatureMatchingProperty.SCORE_MIN), 0.6),
                        RANSAC_REPROJ_THRESHOLD = GetDouble(step.Parameters, nameof(FeatureMatchingProperty.RANSAC_REPROJ_THRESHOLD), 3),
                        PATTERN_PATH = GetString(step.Parameters, nameof(FeatureMatchingProperty.PATTERN_PATH), GetString(step.Parameters, "TemplatePath", string.Empty))
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                default:
                    return null;
            }
        }

        public static bool ApplyProperty(VisionPipelineStep target, object property)
        {
            if (target == null || property == null)
            {
                return false;
            }

            string inputLayer = target.InputLayer;
            string outputLayer = target.OutputLayer;
            bool enabled = target.Enabled;
            bool useAcceptance = target.UseAcceptance;
            bool expectedSuccess = target.ExpectedSuccess;
            double maxElapsedMilliseconds = target.MaxElapsedMilliseconds;
            string requiredMessageText = target.RequiredMessageText;
            string acceptanceMetricName = target.AcceptanceMetricName;
            bool useAcceptanceMetricMinimum = target.UseAcceptanceMetricMinimum;
            double acceptanceMetricMinimum = target.AcceptanceMetricMinimum;
            bool useAcceptanceMetricMaximum = target.UseAcceptanceMetricMaximum;
            double acceptanceMetricMaximum = target.AcceptanceMetricMaximum;
            if (property is IPipelineStepMetadata metadata)
            {
                inputLayer = string.IsNullOrWhiteSpace(metadata.InputLayer) ? target.InputLayer : metadata.InputLayer;
                outputLayer = string.IsNullOrWhiteSpace(metadata.OutputLayer) ? target.OutputLayer : metadata.OutputLayer;
                enabled = metadata.Enabled;
                useAcceptance = metadata.UseAcceptance;
                expectedSuccess = metadata.ExpectedSuccess;
                maxElapsedMilliseconds = metadata.MaxElapsedMilliseconds;
                requiredMessageText = metadata.RequiredMessageText ?? string.Empty;
                acceptanceMetricName = metadata.AcceptanceMetricName ?? string.Empty;
                useAcceptanceMetricMinimum = metadata.UseAcceptanceMetricMinimum;
                acceptanceMetricMinimum = metadata.AcceptanceMetricMinimum;
                useAcceptanceMetricMaximum = metadata.UseAcceptanceMetricMaximum;
                acceptanceMetricMaximum = metadata.AcceptanceMetricMaximum;
            }

            VisionPipelineStep mapped = null;
            if (property is OpenCvPropertyBase openCvProperty)
            {
                mapped = VisionPipelineStepBuilder.FromProperty(openCvProperty, inputLayer, outputLayer);
            }
            else if (property is ThresholdToolProperty threshold)
            {
                mapped = VisionPipelineStepBuilder.FromThresholdProperty(threshold, GetPropertyName(property, target.Name), inputLayer, outputLayer);
            }
            else if (property is MorphologyToolProperty morphology)
            {
                mapped = VisionPipelineStepBuilder.FromMorphologyProperty(morphology, GetPropertyName(property, target.Name), inputLayer, outputLayer);
            }
            else if (property is FilterToolProperty filter)
            {
                mapped = VisionPipelineStepBuilder.FromFilterProperty(filter, GetPropertyName(property, target.Name), inputLayer, outputLayer);
            }
            else if (property is EdgeDetectionToolProperty edgeDetection)
            {
                mapped = VisionPipelineStepBuilder.FromEdgeDetectionProperty(edgeDetection, GetPropertyName(property, target.Name), inputLayer, outputLayer);
            }

            if (mapped == null)
            {
                return false;
            }

            CopyStep(target, mapped);
            target.Enabled = enabled;
            target.UseAcceptance = useAcceptance;
            target.ExpectedSuccess = expectedSuccess;
            target.MaxElapsedMilliseconds = maxElapsedMilliseconds;
            target.RequiredMessageText = requiredMessageText ?? string.Empty;
            target.AcceptanceMetricName = acceptanceMetricName ?? string.Empty;
            target.UseAcceptanceMetricMinimum = useAcceptanceMetricMinimum;
            target.AcceptanceMetricMinimum = acceptanceMetricMinimum;
            target.UseAcceptanceMetricMaximum = useAcceptanceMetricMaximum;
            target.AcceptanceMetricMaximum = acceptanceMetricMaximum;
            return true;
        }

        private static T AttachStepMetadata<T>(T property, string name, string inputLayer, string outputLayer)
            where T : IPipelineStepMetadata
        {
            property.PipelineStepName = string.IsNullOrWhiteSpace(name) ? property.PipelineStepName : name;
            property.InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer;
            property.OutputLayer = string.IsNullOrWhiteSpace(outputLayer) ? "Pipeline_Output" : outputLayer;
            return property;
        }

        private static T ApplyCommonOpenCvProperty<T>(T property, IDictionary<string, string> parameters)
            where T : OpenCvPropertyBase
        {
            property.PIXELPERMM = GetDouble(parameters, nameof(property.PIXELPERMM), property.PIXELPERMM);
            property.USE_THRESHOLD = GetBool(parameters, nameof(property.USE_THRESHOLD), property.USE_THRESHOLD);
            property.USE_BITWISENOT = GetBool(parameters, nameof(property.USE_BITWISENOT), property.USE_BITWISENOT);
            property.THRESHOLD_TYPES = GetEnum(parameters, nameof(property.THRESHOLD_TYPES), property.THRESHOLD_TYPES);
            property.THRESHOLD = GetDouble(parameters, nameof(property.THRESHOLD), property.THRESHOLD);
            property.USE_ADAPTIVE_THRESHOLD = GetBool(parameters, nameof(property.USE_ADAPTIVE_THRESHOLD), property.USE_ADAPTIVE_THRESHOLD);
            property.ADAPTIVE_THRESHOLD = GetDouble(parameters, nameof(property.ADAPTIVE_THRESHOLD), property.ADAPTIVE_THRESHOLD);
            property.ADAPTIVE_THRESHOLD_TYPES = GetEnum(parameters, nameof(property.ADAPTIVE_THRESHOLD_TYPES), property.ADAPTIVE_THRESHOLD_TYPES);
            property.ADAPTIVE_THRESHOLD_ALGORITHM = GetEnum(parameters, nameof(property.ADAPTIVE_THRESHOLD_ALGORITHM), property.ADAPTIVE_THRESHOLD_ALGORITHM);
            property.BlockSize = GetInt(parameters, nameof(property.BlockSize), property.BlockSize);
            property.Weight = GetInt(parameters, nameof(property.Weight), property.Weight);
            property.USE_ROI = GetBool(parameters, nameof(property.USE_ROI), property.USE_ROI);
            property.USE_MULTI_ROI = GetBool(parameters, nameof(property.USE_MULTI_ROI), property.USE_MULTI_ROI);
            property.CvROI = GetRect(parameters, nameof(property.CvROI), property.CvROI);
            property.CvROIS = GetRectList(parameters, nameof(property.CvROIS), property.CvROIS);
            property.CvMASKS = GetRectList(parameters, nameof(property.CvMASKS), property.CvMASKS);
            return property;
        }

        private static void CopyStep(VisionPipelineStep target, VisionPipelineStep source)
        {
            target.Name = source.Name;
            target.ToolType = source.ToolType;
            target.InputLayer = source.InputLayer;
            target.OutputLayer = source.OutputLayer;
            target.Parameters.Clear();

            foreach (KeyValuePair<string, string> parameter in source.Parameters)
            {
                target.Parameters[parameter.Key] = parameter.Value;
            }
        }

        private static string GetStepName(VisionPipelineStep step)
        {
            return GetString(step.Parameters, "Name", step.Name);
        }

        private static string GetPropertyName(object property, string fallback)
        {
            if (property is IPipelineStepMetadata metadata)
            {
                return string.IsNullOrWhiteSpace(metadata.PipelineStepName) ? fallback : metadata.PipelineStepName;
            }

            return fallback;
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }

        private static string GetValue(IDictionary<string, string> parameters, string key)
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

        private static string GetString(IDictionary<string, string> parameters, string key, string defaultValue)
        {
            string value = GetValue(parameters, key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static int GetInt(IDictionary<string, string> parameters, string key, int defaultValue)
        {
            string value = GetValue(parameters, key);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : defaultValue;
        }

        private static double GetDouble(IDictionary<string, string> parameters, string key, double defaultValue)
        {
            string value = GetValue(parameters, key);
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                ? result
                : defaultValue;
        }

        private static bool GetBool(IDictionary<string, string> parameters, string key, bool defaultValue)
        {
            string value = GetValue(parameters, key);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        private static TEnum GetEnum<TEnum>(IDictionary<string, string> parameters, string key, TEnum defaultValue)
            where TEnum : struct
        {
            string value = GetValue(parameters, key);
            return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
        }

        private static Rect GetRect(IDictionary<string, string> parameters, string key, Rect defaultValue)
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

        private static List<Rect> GetRectList(IDictionary<string, string> parameters, string key, List<Rect> defaultValue)
        {
            string value = GetValue(parameters, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue ?? new List<Rect>();
            }

            List<Rect> rects = value
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => GetRect(new Dictionary<string, string> { [key] = part }, key, default))
                .ToList();

            return rects;
        }

        private interface IPipelineStepMetadata
        {
            string PipelineStepName { get; set; }
            bool Enabled { get; set; }
            string InputLayer { get; set; }
            string OutputLayer { get; set; }
            bool UseAcceptance { get; set; }
            bool ExpectedSuccess { get; set; }
            double MaxElapsedMilliseconds { get; set; }
            string RequiredMessageText { get; set; }
            string AcceptanceMetricName { get; set; }
            bool UseAcceptanceMetricMinimum { get; set; }
            double AcceptanceMetricMinimum { get; set; }
            bool UseAcceptanceMetricMaximum { get; set; }
            double AcceptanceMetricMaximum { get; set; }
        }

        public sealed class PipelineLayerNameConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                string[] values = layerNameAccessor()
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                return new StandardValuesCollection(values);
            }
        }

        public sealed class PipelineMetricNameConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                string toolType = ResolveMetricToolType(context?.Instance);
                IEnumerable<string> metricNames = string.IsNullOrWhiteSpace(toolType)
                    ? VisionPipelineKnownMetrics.GetMetricNames()
                    : VisionPipelineKnownMetrics.GetMetricNamesForTool(toolType);
                return new StandardValuesCollection(metricNames.ToArray());
            }
        }

        private static string ResolveMetricToolType(object instance)
        {
            switch (instance)
            {
                case PipelineBlobProperty _:
                    return "Blob";
                case PipelineContourProperty _:
                    return "Contour";
                case PipelineLineGaugeProperty _:
                    return "LineGauge";
                case PipelineMatchingProperty _:
                    return "Matching";
                case PipelineMeanProperty _:
                    return "Mean";
                case PipelineFeatureMatchingProperty _:
                    return "FeatureMatching";
                case PipelineThresholdToolProperty _:
                    return "Threshold";
                case PipelineMorphologyToolProperty _:
                    return "Morphology";
                case PipelineFilterToolProperty _:
                    return "Filter";
                case PipelineEdgeDetectionToolProperty _:
                    return "EdgeDetection";
                default:
                    return string.Empty;
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Blob Parameter", 0)]
        [CategoryOrder("ROI", 10)]
        [CategoryOrder("Threshold", 20)]
        [CategoryOrder("Parameter", 30)]
        [CategoryOrder("Acceptance", 40)]
        private sealed class PipelineBlobProperty : BlobProperty, IPipelineStepMetadata
        {
            public PipelineBlobProperty(string name)
                : base(name)
            {
            }

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Contour", 0)]
        [CategoryOrder("ROI", 10)]
        [CategoryOrder("Threshold", 20)]
        [CategoryOrder("Parameter", 30)]
        [CategoryOrder("Acceptance", 40)]
        private sealed class PipelineContourProperty : ContourProperty, IPipelineStepMetadata
        {
            public PipelineContourProperty(string name)
                : base(name)
            {
            }

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineLineGaugeProperty : LineGaugeProperty, IPipelineStepMetadata
        {
            public PipelineLineGaugeProperty(string name)
                : base(name)
            {
            }

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineMatchingProperty : MatchingProperty, IPipelineStepMetadata
        {
            public PipelineMatchingProperty(string name)
                : base(name)
            {
            }

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineMeanProperty : MeanProperty, IPipelineStepMetadata
        {
            public PipelineMeanProperty(string name)
                : base(name)
            {
            }

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineFeatureMatchingProperty : FeatureMatchingProperty, IPipelineStepMetadata
        {
            public PipelineFeatureMatchingProperty(string name)
                : base(name)
            {
            }

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Threshold", 0)]
        [CategoryOrder("Range", 1)]
        [CategoryOrder("Adaptive Threshold", 2)]
        private sealed class PipelineThresholdToolProperty : ThresholdToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "Threshold";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Threshold")]
            [DisplayName("Mode")]
            public new ThresholdToolMode Mode
            {
                get => base.Mode;
                set => base.Mode = value;
            }

            [PropertyOrder(1)]
            [PropertyEditor(typeof(WpgThresholdEditor))]
            [ThresholdEditor(0, 255, 1, 0, nameof(Invert))]
            [NumberRange(0, 255, 1, 0)]
            [Category("Threshold")]
            [DisplayName("Threshold")]
            public new double Threshold
            {
                get => base.Threshold;
                set => base.Threshold = value;
            }

            [PropertyOrder(2)]
            [PropertyEditor(typeof(WpgDoubleEditor))]
            [NumberRange(0, 255, 1, 0)]
            [Category("Threshold")]
            [DisplayName("Max value")]
            public new double MaxValue
            {
                get => base.MaxValue;
                set => base.MaxValue = value;
            }

            [PropertyOrder(3)]
            [Category("Threshold")]
            [DisplayName("Threshold type")]
            public new ThresholdTypes ThresholdType
            {
                get => base.ThresholdType;
                set => base.ThresholdType = value;
            }

            [PropertyOrder(0)]
            [PropertyEditor(typeof(WpgRangeEditor))]
            [RangeEditor(0, 255, 1, 0, nameof(RangeMin), nameof(RangeMax), nameof(Invert))]
            [Category("Range")]
            [DisplayName("Range min")]
            public new int RangeMin
            {
                get => base.RangeMin;
                set => base.RangeMin = value;
            }

            [PropertyOrder(1)]
            [Browsable(false)]
            [Category("Range")]
            [DisplayName("Range max")]
            public new int RangeMax
            {
                get => base.RangeMax;
                set => base.RangeMax = value;
            }

            [PropertyOrder(2)]
            [Browsable(false)]
            [Category("Range")]
            [DisplayName("Invert")]
            public new bool Invert
            {
                get => base.Invert;
                set => base.Invert = value;
            }

            [PropertyOrder(0)]
            [Category("Adaptive Threshold")]
            [DisplayName("Algorithm")]
            public new AdaptiveThresholdTypes AdaptiveType
            {
                get => base.AdaptiveType;
                set => base.AdaptiveType = value;
            }

            [PropertyOrder(1)]
            [Category("Adaptive Threshold")]
            [DisplayName("Threshold type")]
            public new ThresholdTypes AdaptiveThresholdType
            {
                get => base.AdaptiveThresholdType;
                set => base.AdaptiveThresholdType = value;
            }

            [PropertyOrder(2)]
            [Category("Adaptive Threshold")]
            [DisplayName("Block size")]
            public new int BlockSize
            {
                get => base.BlockSize;
                set => base.BlockSize = value;
            }

            [PropertyOrder(3)]
            [Category("Adaptive Threshold")]
            [DisplayName("Weight")]
            public new int Weight
            {
                get => base.Weight;
                set => base.Weight = value;
            }

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Morphology", 0)]
        private sealed class PipelineMorphologyToolProperty : MorphologyToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "Morphology";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Morphology")]
            [DisplayName("Shape")]
            public new MorphShapes Shape
            {
                get => base.Shape;
                set => base.Shape = value;
            }

            [PropertyOrder(1)]
            [Category("Morphology")]
            [DisplayName("Operator")]
            public new MorphTypes Operator
            {
                get => base.Operator;
                set => base.Operator = value;
            }

            [PropertyOrder(2)]
            [Category("Morphology")]
            [DisplayName("Kernel width")]
            public new int KernelWidth
            {
                get => base.KernelWidth;
                set => base.KernelWidth = value;
            }

            [PropertyOrder(3)]
            [Category("Morphology")]
            [DisplayName("Kernel height")]
            public new int KernelHeight
            {
                get => base.KernelHeight;
                set => base.KernelHeight = value;
            }

            [PropertyOrder(4)]
            [Category("Morphology")]
            [DisplayName("Iterations")]
            public new int Iterations
            {
                get => base.Iterations;
                set => base.Iterations = value;
            }

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Filter", 0)]
        [CategoryOrder("Kernel", 1)]
        [CategoryOrder("Bilateral", 2)]
        private sealed class PipelineFilterToolProperty : FilterToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "Filter";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Filter")]
            [DisplayName("Filter type")]
            public new FilterToolType FilterType
            {
                get => base.FilterType;
                set => base.FilterType = value;
            }

            [PropertyOrder(1)]
            [Category("Filter")]
            [DisplayName("Border type")]
            public new BorderTypes BorderType
            {
                get => base.BorderType;
                set => base.BorderType = value;
            }

            [PropertyOrder(0)]
            [Category("Kernel")]
            [DisplayName("Kernel width")]
            public new int KernelWidth
            {
                get => base.KernelWidth;
                set => base.KernelWidth = value;
            }

            [PropertyOrder(1)]
            [Category("Kernel")]
            [DisplayName("Kernel height")]
            public new int KernelHeight
            {
                get => base.KernelHeight;
                set => base.KernelHeight = value;
            }

            [PropertyOrder(2)]
            [Category("Kernel")]
            [DisplayName("Median kernel size")]
            public new int MedianKernelSize
            {
                get => base.MedianKernelSize;
                set => base.MedianKernelSize = value;
            }

            [PropertyOrder(0)]
            [Category("Bilateral")]
            [DisplayName("Diameter")]
            public new int Diameter
            {
                get => base.Diameter;
                set => base.Diameter = value;
            }

            [PropertyOrder(1)]
            [Category("Bilateral")]
            [DisplayName("Sigma color")]
            public new int SigmaColor
            {
                get => base.SigmaColor;
                set => base.SigmaColor = value;
            }

            [PropertyOrder(2)]
            [Category("Bilateral")]
            [DisplayName("Sigma space")]
            public new int SigmaSpace
            {
                get => base.SigmaSpace;
                set => base.SigmaSpace = value;
            }

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Edge", 0)]
        [CategoryOrder("Canny", 1)]
        [CategoryOrder("Sobel", 2)]
        [CategoryOrder("Scharr", 3)]
        [CategoryOrder("Laplacian", 4)]
        private sealed class PipelineEdgeDetectionToolProperty : EdgeDetectionToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "EdgeDetection";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Edge")]
            [DisplayName("Edge type")]
            public new EdgeDetectionToolType EdgeType
            {
                get => base.EdgeType;
                set => base.EdgeType = value;
            }

            [PropertyOrder(0)]
            [Category("Canny")]
            [DisplayName("Low threshold")]
            public new int CannyThresholdLow
            {
                get => base.CannyThresholdLow;
                set => base.CannyThresholdLow = value;
            }

            [PropertyOrder(1)]
            [Category("Canny")]
            [DisplayName("High threshold")]
            public new int CannyThresholdHigh
            {
                get => base.CannyThresholdHigh;
                set => base.CannyThresholdHigh = value;
            }

            [PropertyOrder(2)]
            [Category("Canny")]
            [DisplayName("Aperture size")]
            public new int CannyApertureSize
            {
                get => base.CannyApertureSize;
                set => base.CannyApertureSize = value;
            }

            [PropertyOrder(3)]
            [Category("Canny")]
            [DisplayName("Use L2 gradient")]
            public new bool UseL2Gradient
            {
                get => base.UseL2Gradient;
                set => base.UseL2Gradient = value;
            }

            [PropertyOrder(0)]
            [Category("Sobel")]
            [DisplayName("Degree X")]
            public new int SobelDegreeX
            {
                get => base.SobelDegreeX;
                set => base.SobelDegreeX = value;
            }

            [PropertyOrder(1)]
            [Category("Sobel")]
            [DisplayName("Degree Y")]
            public new int SobelDegreeY
            {
                get => base.SobelDegreeY;
                set => base.SobelDegreeY = value;
            }

            [PropertyOrder(2)]
            [Category("Sobel")]
            [DisplayName("Kernel size")]
            public new int SobelKernelSize
            {
                get => base.SobelKernelSize;
                set => base.SobelKernelSize = value;
            }

            [PropertyOrder(0)]
            [Category("Scharr")]
            [DisplayName("Degree X")]
            public new int ScharrDegreeX
            {
                get => base.ScharrDegreeX;
                set => base.ScharrDegreeX = value;
            }

            [PropertyOrder(1)]
            [Category("Scharr")]
            [DisplayName("Degree Y")]
            public new int ScharrDegreeY
            {
                get => base.ScharrDegreeY;
                set => base.ScharrDegreeY = value;
            }

            [PropertyOrder(0)]
            [Category("Laplacian")]
            [DisplayName("Kernel size")]
            public new int LaplacianKernelSize
            {
                get => base.LaplacianKernelSize;
                set => base.LaplacianKernelSize = value;
            }

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }
    }
}

