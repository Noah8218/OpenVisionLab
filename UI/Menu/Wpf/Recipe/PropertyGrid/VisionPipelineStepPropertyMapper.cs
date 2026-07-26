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
    internal static partial class VisionPipelineStepPropertyMapper
    {
        public static IEnumerable<string> GetCompatibleGeometryFeatureReferences(
            VisionPipeline pipeline,
            int currentStepIndex,
            GeometryMeasurementMode mode,
            bool sourceA)
        {
            VisionPipelineGeometryKind required = mode == GeometryMeasurementMode.PointPointDistance
                ? VisionPipelineGeometryKind.Point
                : mode == GeometryMeasurementMode.PointLineDistance
                    ? (sourceA ? VisionPipelineGeometryKind.Point : VisionPipelineGeometryKind.Segment)
                    : mode == GeometryMeasurementMode.CircleSegmentClearance
                        ? (sourceA ? VisionPipelineGeometryKind.Circle : VisionPipelineGeometryKind.Segment)
                        : VisionPipelineGeometryKind.Segment;
            return (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Take(Math.Max(0, currentStepIndex))
                .Where(candidate => candidate?.Enabled == true)
                .SelectMany(GetDeclaredGeometryFeatures)
                .Where(item => item.Kind == required)
                .Select(item => item.Reference)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static IEnumerable<string> GetCompatiblePointFeatureReferences(
            VisionPipeline pipeline,
            int currentStepIndex)
        {
            VisionPipelineStep consumer = pipeline?.Steps != null
                && currentStepIndex >= 0
                && currentStepIndex < pipeline.Steps.Count
                    ? pipeline.Steps[currentStepIndex]
                    : null;
            return (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Take(Math.Max(0, currentStepIndex))
                .Where(candidate => candidate?.Enabled == true
                    && consumer != null
                    && string.Equals(candidate.InputLayer, consumer.InputLayer, StringComparison.OrdinalIgnoreCase))
                .SelectMany(GetDeclaredGeometryFeatures)
                .Where(item => item.Kind == VisionPipelineGeometryKind.Point)
                .Select(item => item.Reference)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static object CreateProperty(VisionPipelineStep step)
        {
            return CreateProperty(step, VisionPipelinePropertyContext.Empty);
        }

        public static object CreateProperty(VisionPipelineStep step, VisionPipelinePropertyContext context)
        {
            object property = CreatePropertyCore(step, context ?? VisionPipelinePropertyContext.Empty);
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

        private static object CreatePropertyCore(VisionPipelineStep step, VisionPipelinePropertyContext context)
        {
            if (step == null)
            {
                return null;
            }

            string name = GetStepName(step);
            string toolType = NormalizeToolType(step.ToolType);
            if (VisionPipelineTransformPropertyAdapter.TryCreateProperty(
                step,
                name,
                context,
                out object transformProperty))
            {
                return transformProperty;
            }

            if (VisionPipelineReferenceDifferencePropertyAdapter.TryCreateProperty(
                step,
                name,
                out object referenceDifferenceProperty))
            {
                return referenceDifferenceProperty;
            }

            if (VisionPipelinePinArrayGapPropertyAdapter.TryCreateProperty(
                step,
                name,
                out object pinArrayGapProperty))
            {
                return pinArrayGapProperty;
            }

            if (VisionPipelineLinePairPropertyAdapter.TryCreateProperty(
                step,
                name,
                out object linePairProperty))
            {
                return linePairProperty;
            }

            if (VisionPipelineGeometryPropertyAdapter.TryCreateProperty(
                step,
                name,
                context,
                out object geometryProperty))
            {
                return geometryProperty;
            }

            switch (toolType)
            {
                case "threshold":
                case "morphology":
                case "filter":
                case "edgedetection":
                case "edge":
                    return CreateBasicImageProperty(step, name, toolType);
                case "blob":
                case "contour":
                    return CreateObjectInspectionProperty(step, name, toolType);
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
                    return CreateMatchingProperty(step, name);
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return CreateEdgeBasedMatchingProperty(step, name);
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
                    return CreateFeatureMatchingProperty(step, name);
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
            if (VisionPipelineReferenceDifferencePropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep referenceDifferenceStep))
            {
                mapped = referenceDifferenceStep;
            }
            else if (VisionPipelinePinArrayGapPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep pinArrayGapStep))
            {
                mapped = pinArrayGapStep;
            }
            else if (VisionPipelineLinePairPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep linePairStep))
            {
                mapped = linePairStep;
            }
            else if (VisionPipelineGeometryPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep geometryStep))
            {
                mapped = geometryStep;
            }
            else if (property is OpenCvPropertyBase openCvProperty)
            {
                mapped = VisionPipelineStepBuilder.FromProperty(openCvProperty, inputLayer, outputLayer);
            }
            else if (TryApplyBasicImageProperty(property, target.Name, inputLayer, outputLayer, out VisionPipelineStep basicImageStep))
            {
                mapped = basicImageStep;
            }
            else if (VisionPipelineTransformPropertyAdapter.TryCreateStep(
                property,
                target.Name,
                inputLayer,
                outputLayer,
                out VisionPipelineStep transformStep))
            {
                mapped = transformStep;
            }

            if (mapped == null)
            {
                return false;
            }

            ApplyMatchingParameters(property, mapped.Parameters);
            ApplyObjectInspectionParameters(property, mapped.Parameters);
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

        public static bool TryCreateLineGaugePair(
            object property,
            out LineGaugeProperty left,
            out LineGaugeProperty right)
        {
            return VisionPipelineLinePairPropertyAdapter.TryCreateLineGaugePair(
                property,
                out left,
                out right);
        }


        private static T AttachStepMetadata<T>(T property, string name, string inputLayer, string outputLayer)
            where T : IPipelineStepMetadata
        {
            property.PipelineStepName = string.IsNullOrWhiteSpace(name) ? property.PipelineStepName : name;
            property.InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer;
            property.OutputLayer = string.IsNullOrWhiteSpace(outputLayer) ? "Pipeline_Output" : outputLayer;
            return property;
        }

        internal static T ApplyCommonOpenCvProperty<T>(T property, IDictionary<string, string> parameters)
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
            property.USE_MASKING = GetBool(parameters, nameof(property.USE_MASKING), property.USE_MASKING);
            property.CvROI = GetRect(parameters, nameof(property.CvROI), property.CvROI);
            property.CvROIS = GetRectList(parameters, nameof(property.CvROIS), property.CvROIS);
            property.CvMASKS = GetRectList(parameters, nameof(property.CvMASKS), property.CvMASKS);
            property.USE_MASKING |= property.CvMASKS?.Count > 0;
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

        internal static string GetString(IDictionary<string, string> parameters, string key, string defaultValue)
        {
            string value = GetValue(parameters, key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        internal static int GetInt(IDictionary<string, string> parameters, string key, int defaultValue)
        {
            string value = GetValue(parameters, key);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : defaultValue;
        }

        internal static double GetDouble(IDictionary<string, string> parameters, string key, double defaultValue)
        {
            string value = GetValue(parameters, key);
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                ? result
                : defaultValue;
        }

        internal static bool GetBool(IDictionary<string, string> parameters, string key, bool defaultValue)
        {
            string value = GetValue(parameters, key);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        internal static TEnum GetEnum<TEnum>(IDictionary<string, string> parameters, string key, TEnum defaultValue)
            where TEnum : struct
        {
            string value = GetValue(parameters, key);
            return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
        }

        internal static int GetPrefixedInt(IDictionary<string, string> parameters, string prefix, string key, int defaultValue)
        {
            string value = GetPrefixedValue(parameters, prefix, key);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : GetInt(parameters, key, defaultValue);
        }

        internal static double GetPrefixedDouble(IDictionary<string, string> parameters, string prefix, string key, double defaultValue)
        {
            string value = GetPrefixedValue(parameters, prefix, key);
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                ? result
                : GetDouble(parameters, key, defaultValue);
        }

        internal static bool GetPrefixedBool(IDictionary<string, string> parameters, string prefix, string key, bool defaultValue)
        {
            string value = GetPrefixedValue(parameters, prefix, key);
            return bool.TryParse(value, out bool result)
                ? result
                : GetBool(parameters, key, defaultValue);
        }

        internal static TEnum GetPrefixedEnum<TEnum>(IDictionary<string, string> parameters, string prefix, string key, TEnum defaultValue)
            where TEnum : struct
        {
            string value = GetPrefixedValue(parameters, prefix, key);
            return Enum.TryParse(value, true, out TEnum result)
                ? result
                : GetEnum(parameters, key, defaultValue);
        }

        private static string GetPrefixedValue(IDictionary<string, string> parameters, string prefix, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            string[] candidates =
            {
                prefix + key,
                prefix + "_" + key,
                prefix + "." + key
            };

            foreach (string candidate in candidates)
            {
                string value = GetValue(parameters, candidate);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        internal static void ApplyPrefixedOpenCvProperty(
            OpenCvPropertyBase property,
            IDictionary<string, string> parameters,
            string prefix)
        {
            property.PIXELPERMM = GetPrefixedDouble(parameters, prefix, nameof(property.PIXELPERMM), property.PIXELPERMM);
            property.USE_THRESHOLD = GetPrefixedBool(parameters, prefix, nameof(property.USE_THRESHOLD), property.USE_THRESHOLD);
            property.USE_BITWISENOT = GetPrefixedBool(parameters, prefix, nameof(property.USE_BITWISENOT), property.USE_BITWISENOT);
            property.THRESHOLD_TYPES = GetPrefixedEnum(parameters, prefix, nameof(property.THRESHOLD_TYPES), property.THRESHOLD_TYPES);
            property.THRESHOLD = GetPrefixedDouble(parameters, prefix, nameof(property.THRESHOLD), property.THRESHOLD);
            property.USE_ADAPTIVE_THRESHOLD = GetPrefixedBool(parameters, prefix, nameof(property.USE_ADAPTIVE_THRESHOLD), property.USE_ADAPTIVE_THRESHOLD);
            property.ADAPTIVE_THRESHOLD = GetPrefixedDouble(parameters, prefix, nameof(property.ADAPTIVE_THRESHOLD), property.ADAPTIVE_THRESHOLD);
            property.ADAPTIVE_THRESHOLD_TYPES = GetPrefixedEnum(parameters, prefix, nameof(property.ADAPTIVE_THRESHOLD_TYPES), property.ADAPTIVE_THRESHOLD_TYPES);
            property.ADAPTIVE_THRESHOLD_ALGORITHM = GetPrefixedEnum(parameters, prefix, nameof(property.ADAPTIVE_THRESHOLD_ALGORITHM), property.ADAPTIVE_THRESHOLD_ALGORITHM);
            property.BlockSize = GetPrefixedInt(parameters, prefix, nameof(property.BlockSize), property.BlockSize);
            property.Weight = GetPrefixedInt(parameters, prefix, nameof(property.Weight), property.Weight);
            property.USE_ROI = GetPrefixedBool(parameters, prefix, nameof(property.USE_ROI), property.USE_ROI);
            property.USE_MULTI_ROI = GetPrefixedBool(parameters, prefix, nameof(property.USE_MULTI_ROI), property.USE_MULTI_ROI);
            property.USE_MASKING = GetPrefixedBool(parameters, prefix, nameof(property.USE_MASKING), property.USE_MASKING);
            property.CvROI = GetRect(parameters, prefix + nameof(property.CvROI), property.CvROI);
            property.CvROIS = GetRectList(parameters, prefix + nameof(property.CvROIS), property.CvROIS);
            property.CvMASKS = GetRectList(parameters, prefix + nameof(property.CvMASKS), property.CvMASKS);
            property.USE_MASKING |= property.CvMASKS?.Count > 0;
        }

        internal static void AddParameter(
            IDictionary<string, string> parameters,
            string key,
            object value)
        {
            parameters[key] =
                Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        internal static Rect GetRect(IDictionary<string, string> parameters, string key, Rect defaultValue)
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

        internal interface IPipelineStepMetadata
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
                return new StandardValuesCollection(Array.Empty<string>());
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

        public sealed class PipelinePointFeatureConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (!(context?.Instance is PipelineAffineTransformToolProperty property))
                {
                    return new StandardValuesCollection(Array.Empty<string>());
                }

                string[] values = property.Context.GetCompatiblePointFeatureReferences()
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                return new StandardValuesCollection(values);
            }
        }

        private static IEnumerable<(string Reference, VisionPipelineGeometryKind Kind)> GetDeclaredGeometryFeatures(VisionPipelineStep step)
        {
            string prefix = (step?.Name ?? string.Empty) + "/";
            foreach ((string FeatureName, VisionPipelineGeometryKind Kind) item
                in VisionPipelineGeometryFeatureCatalog.GetDeclaredFeatures(step))
            {
                yield return (prefix + item.FeatureName, item.Kind);
            }
        }

        private static string ResolveMetricToolType(object instance)
        {
            if (VisionPipelineReferenceDifferencePropertyAdapter.IsProperty(instance))
            {
                return "ReferenceDifference";
            }

            if (VisionPipelinePinArrayGapPropertyAdapter.IsProperty(instance))
            {
                return "PinArrayGap";
            }

            if (VisionPipelineLinePairPropertyAdapter.IsProperty(instance))
            {
                return "LineGauge";
            }

            string geometryToolType =
                VisionPipelineGeometryPropertyAdapter.ResolveMetricToolType(instance);
            if (!string.IsNullOrWhiteSpace(geometryToolType))
            {
                return geometryToolType;
            }

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
                case PipelineEdgeBasedMatchingProperty _:
                    return "EdgeBasedMatching";
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
                case PipelineRotateScaleToolProperty _:
                    return "RotateScale";
                case PipelineAffineTransformToolProperty _:
                    return "AffineTransform";
                default:
                    return string.Empty;
            }
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
        [CategoryOrder("Source Binding", 0)]
        [CategoryOrder("Source Points", 1)]
        [CategoryOrder("Destination Points", 2)]
        [CategoryOrder("Output", 3)]
        [CategoryOrder("Sampling", 4)]
        [CategoryOrder("Validation Gates", 5)]
        [CategoryOrder("Acceptance", 20)]
        internal sealed class PipelineAffineTransformToolProperty : AffineTransformToolProperty, IPipelineStepMetadata
        {
            [Browsable(false)]
            public VisionPipelinePropertyContext Context { get; set; } = VisionPipelinePropertyContext.Empty;

            [PropertyOrder(-3), Category("Step"), DisplayName("Step Name")]
            public string NAME { get; set; } = "AffineTransform";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2), Category("Step"), DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1), Category("Step"), DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0), Category("Step"), DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0), Category("Source Binding"), DisplayName("Use detected Point features")]
            [Description("When enabled, resolve three earlier accepted Point features at Run time and ignore the fixed source coordinates below.")]
            public bool UseDetectedSourcePoints { get; set; }

            [PropertyOrder(1), Category("Source Binding"), DisplayName("Source point 1 feature")]
            [TypeConverter(typeof(PipelinePointFeatureConverter))]
            public string SourcePoint1Feature { get; set; } = string.Empty;

            [PropertyOrder(2), Category("Source Binding"), DisplayName("Source point 2 feature")]
            [TypeConverter(typeof(PipelinePointFeatureConverter))]
            public string SourcePoint2Feature { get; set; } = string.Empty;

            [PropertyOrder(3), Category("Source Binding"), DisplayName("Source point 3 feature")]
            [TypeConverter(typeof(PipelinePointFeatureConverter))]
            public string SourcePoint3Feature { get; set; } = string.Empty;

            [PropertyOrder(0), Category("Source Points"), DisplayName("Source point 1 X")]
            public new double SourcePoint1X { get => base.SourcePoint1X; set => base.SourcePoint1X = value; }
            [PropertyOrder(1), Category("Source Points"), DisplayName("Source point 1 Y")]
            public new double SourcePoint1Y { get => base.SourcePoint1Y; set => base.SourcePoint1Y = value; }
            [PropertyOrder(2), Category("Source Points"), DisplayName("Source point 2 X")]
            public new double SourcePoint2X { get => base.SourcePoint2X; set => base.SourcePoint2X = value; }
            [PropertyOrder(3), Category("Source Points"), DisplayName("Source point 2 Y")]
            public new double SourcePoint2Y { get => base.SourcePoint2Y; set => base.SourcePoint2Y = value; }
            [PropertyOrder(4), Category("Source Points"), DisplayName("Source point 3 X")]
            public new double SourcePoint3X { get => base.SourcePoint3X; set => base.SourcePoint3X = value; }
            [PropertyOrder(5), Category("Source Points"), DisplayName("Source point 3 Y")]
            public new double SourcePoint3Y { get => base.SourcePoint3Y; set => base.SourcePoint3Y = value; }

            [PropertyOrder(0), Category("Destination Points"), DisplayName("Destination point 1 X")]
            public new double DestinationPoint1X { get => base.DestinationPoint1X; set => base.DestinationPoint1X = value; }
            [PropertyOrder(1), Category("Destination Points"), DisplayName("Destination point 1 Y")]
            public new double DestinationPoint1Y { get => base.DestinationPoint1Y; set => base.DestinationPoint1Y = value; }
            [PropertyOrder(2), Category("Destination Points"), DisplayName("Destination point 2 X")]
            public new double DestinationPoint2X { get => base.DestinationPoint2X; set => base.DestinationPoint2X = value; }
            [PropertyOrder(3), Category("Destination Points"), DisplayName("Destination point 2 Y")]
            public new double DestinationPoint2Y { get => base.DestinationPoint2Y; set => base.DestinationPoint2Y = value; }
            [PropertyOrder(4), Category("Destination Points"), DisplayName("Destination point 3 X")]
            public new double DestinationPoint3X { get => base.DestinationPoint3X; set => base.DestinationPoint3X = value; }
            [PropertyOrder(5), Category("Destination Points"), DisplayName("Destination point 3 Y")]
            public new double DestinationPoint3Y { get => base.DestinationPoint3Y; set => base.DestinationPoint3Y = value; }

            [PropertyOrder(0), Category("Output"), DisplayName("Output width")]
            public new int OutputWidth { get => base.OutputWidth; set => base.OutputWidth = value; }
            [PropertyOrder(1), Category("Output"), DisplayName("Output height")]
            public new int OutputHeight { get => base.OutputHeight; set => base.OutputHeight = value; }

            [PropertyOrder(0), Category("Sampling"), DisplayName("Interpolation")]
            public new InterpolationFlags Interpolation { get => base.Interpolation; set => base.Interpolation = value; }
            [PropertyOrder(1), Category("Sampling"), DisplayName("Border type")]
            public new BorderTypes BorderType { get => base.BorderType; set => base.BorderType = value; }
            [PropertyOrder(2), Category("Sampling"), DisplayName("Border value")]
            public new double BorderValue { get => base.BorderValue; set => base.BorderValue = value; }

            [PropertyOrder(0), Category("Validation Gates"), DisplayName("Minimum source triangle area")]
            public new double MinimumSourceTriangleArea { get => base.MinimumSourceTriangleArea; set => base.MinimumSourceTriangleArea = value; }
            [PropertyOrder(1), Category("Validation Gates"), DisplayName("Minimum destination triangle area")]
            public new double MinimumDestinationTriangleArea { get => base.MinimumDestinationTriangleArea; set => base.MinimumDestinationTriangleArea = value; }
            [PropertyOrder(2), Category("Validation Gates"), DisplayName("Minimum valid pixel ratio")]
            public new double MinimumValidPixelRatio { get => base.MinimumValidPixelRatio; set => base.MinimumValidPixelRatio = value; }

            [PropertyOrder(1), Category("Acceptance"), DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; }
            [PropertyOrder(2), Category("Acceptance"), DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;
            [PropertyOrder(3), Category("Acceptance"), DisplayName("Max Elapsed (ms)")]
            public double MaxElapsedMilliseconds { get; set; }
            [PropertyOrder(4), Category("Acceptance"), DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;
            [PropertyOrder(5), Category("Acceptance"), DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;
            [PropertyOrder(6), Browsable(false), Category("Acceptance")]
            public bool UseAcceptanceMetricMinimum { get; set; }
            [PropertyOrder(7), Category("Acceptance"), DisplayName("Metric range")]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            public double AcceptanceMetricMinimum { get; set; }
            [PropertyOrder(8), Browsable(false), Category("Acceptance")]
            public bool UseAcceptanceMetricMaximum { get; set; }
            [PropertyOrder(9), Browsable(false), Category("Acceptance")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Transform", 0)]
        [CategoryOrder("Fixture", 10)]
        [CategoryOrder("Acceptance", 20)]
        internal sealed class PipelineRotateScaleToolProperty : RotateScaleToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "RotateScale";

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
            [Category("Transform")]
            [DisplayName("Angle")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(-180, 180, 1, 1)]
            [Description("Rotation angle in degrees. Use small changes while previewing alignment-sensitive images.")]
            public new double Angle
            {
                get => base.Angle;
                set => base.Angle = value;
            }

            [PropertyOrder(1)]
            [Category("Transform")]
            [DisplayName("Scale X (%)")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 300, 1, 1)]
            [Description("Horizontal scale percent. Values must stay greater than 0.")]
            public new double ScaleXPercent
            {
                get => base.ScaleXPercent;
                set => base.ScaleXPercent = value;
            }

            [PropertyOrder(2)]
            [Category("Transform")]
            [DisplayName("Scale Y (%)")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 300, 1, 1)]
            [Description("Vertical scale percent. Values must stay greater than 0.")]
            public new double ScaleYPercent
            {
                get => base.ScaleYPercent;
                set => base.ScaleYPercent = value;
            }

            [PropertyOrder(3)]
            [Category("Transform")]
            [DisplayName("Interpolation")]
            public new InterpolationFlags Interpolation
            {
                get => base.Interpolation;
                set => base.Interpolation = value;
            }

            [PropertyOrder(4)]
            [Category("Transform")]
            [DisplayName("Border type")]
            public new BorderTypes BorderType
            {
                get => base.BorderType;
                set => base.BorderType = value;
            }

            [PropertyOrder(0)]
            [Category("Fixture")]
            [DisplayName("Use Fixture Frame")]
            [Description("Use a previously published Matching fixture frame. Fixed Angle/Scale values remain unchanged when this is off.")]
            public bool USE_FIXTURE_FRAME { get; set; }

            [PropertyOrder(1)]
            [Category("Fixture")]
            [DisplayName("Fixture Frame Name")]
            public string FIXTURE_FRAME_NAME { get; set; } = string.Empty;

            [PropertyOrder(2)]
            [Category("Fixture")]
            [DisplayName("보정 방식")]
            [Description("NormalizeImage applies the inverse Matching pose to the complete source image. TranslationRoi is reserved for ROI-capable consumers.")]
            public VisionPipelineFixtureApplyMode FIXTURE_APPLY_MODE { get; set; } = VisionPipelineFixtureApplyMode.TranslationRoi;

            [PropertyOrder(3)]
            [Category("Fixture")]
            [DisplayName("최소 유효 비율")]
            [Description("Fail NormalizeImage when transformed source coverage is below this 0..1 ratio.")]
            public double FIXTURE_MIN_VALID_PIXEL_RATIO { get; set; } = VisionPipelineFixtureFrameService.DefaultMinimumValidPixelRatio;

            [PropertyOrder(4)]
            [Category("Fixture")]
            [DisplayName("Allow Branch Input")]
            [Description("Confirms that this normalization intentionally reads the same source layer as Matching instead of the previous Step output.")]
            public bool ALLOW_BRANCH_INPUT { get; set; }

            public void ApplyFixtureParameters(IDictionary<string, string> parameters)
            {
                if (parameters == null || !USE_FIXTURE_FRAME)
                {
                    return;
                }

                parameters[VisionPipelineFixtureFrameService.ConsumeParameter] = Convert.ToString(true, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = FIXTURE_FRAME_NAME?.Trim() ?? string.Empty;
                parameters[VisionPipelineFixtureFrameService.ApplyModeParameter] = Convert.ToString(FIXTURE_APPLY_MODE, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter] = Convert.ToString(FIXTURE_MIN_VALID_PIXEL_RATIO, CultureInfo.InvariantCulture);
                parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = Convert.ToString(ALLOW_BRANCH_INPUT, CultureInfo.InvariantCulture);
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
            [Description("Layer image used as this step input. Linked steps normally use the previous step output.")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [Description("Layer name that stores this step result. Use a unique name when the result must be reviewed later.")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Threshold")]
            [DisplayName("Mode")]
            [Description("Threshold uses one gray value, Range uses lower and upper gray limits, Adaptive calculates a local threshold.")]
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
            [Description("Single threshold value. Pixels are classified by this gray level and the selected threshold type.")]
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
            [Description("Combined range threshold. Adjust Min and Max together; Invert selects pixels outside the range.")]
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
            [Description("Adaptive threshold algorithm. MeanC is stable for broad lighting changes; GaussianC gives more local weighting.")]
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
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(3, 255, 2, 0)]
            [Description("Adaptive window size. Use odd values; larger windows are smoother and slower.")]
            public new int BlockSize
            {
                get => base.BlockSize;
                set => base.BlockSize = value;
            }

            [PropertyOrder(3)]
            [Category("Adaptive Threshold")]
            [DisplayName("Weight")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(-50, 50, 1, 0)]
            [Description("Adaptive correction value. Positive values make the result stricter.")]
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
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Morphology kernel width. Increase to connect nearby pixels or remove wider noise.")]
            public new int KernelWidth
            {
                get => base.KernelWidth;
                set => base.KernelWidth = value;
            }

            [PropertyOrder(3)]
            [Category("Morphology")]
            [DisplayName("Kernel height")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Morphology kernel height. Increase to connect vertical gaps or remove taller noise.")]
            public new int KernelHeight
            {
                get => base.KernelHeight;
                set => base.KernelHeight = value;
            }

            [PropertyOrder(4)]
            [Category("Morphology")]
            [DisplayName("Iterations")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 20, 1, 0)]
            [Description("Number of repeated morphology operations. Keep this low unless the preview proves it is needed.")]
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
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Filter kernel width used by blur-like filters.")]
            public new int KernelWidth
            {
                get => base.KernelWidth;
                set => base.KernelWidth = value;
            }

            [PropertyOrder(1)]
            [Category("Kernel")]
            [DisplayName("Kernel height")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Filter kernel height used by blur-like filters.")]
            public new int KernelHeight
            {
                get => base.KernelHeight;
                set => base.KernelHeight = value;
            }

            [PropertyOrder(2)]
            [Category("Kernel")]
            [DisplayName("Median kernel size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(3, 99, 2, 0)]
            [Description("Median blur kernel size. Use odd values such as 3, 5, 7.")]
            public new int MedianKernelSize
            {
                get => base.MedianKernelSize;
                set => base.MedianKernelSize = value;
            }

            [PropertyOrder(0)]
            [Category("Bilateral")]
            [DisplayName("Diameter")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Bilateral filter neighborhood diameter.")]
            public new int Diameter
            {
                get => base.Diameter;
                set => base.Diameter = value;
            }

            [PropertyOrder(1)]
            [Category("Bilateral")]
            [DisplayName("Sigma color")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            [Description("Bilateral color sigma. Larger values smooth across stronger intensity differences.")]
            public new int SigmaColor
            {
                get => base.SigmaColor;
                set => base.SigmaColor = value;
            }

            [PropertyOrder(2)]
            [Category("Bilateral")]
            [DisplayName("Sigma space")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            [Description("Bilateral spatial sigma. Larger values use a wider spatial neighborhood.")]
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
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            public new int CannyThresholdLow
            {
                get => base.CannyThresholdLow;
                set => base.CannyThresholdLow = value;
            }

            [PropertyOrder(1)]
            [Category("Canny")]
            [DisplayName("High threshold")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            public new int CannyThresholdHigh
            {
                get => base.CannyThresholdHigh;
                set => base.CannyThresholdHigh = value;
            }

            [PropertyOrder(2)]
            [Category("Canny")]
            [DisplayName("Aperture size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(3, 7, 2, 0)]
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
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 2, 1, 0)]
            public new int SobelDegreeX
            {
                get => base.SobelDegreeX;
                set => base.SobelDegreeX = value;
            }

            [PropertyOrder(1)]
            [Category("Sobel")]
            [DisplayName("Degree Y")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 2, 1, 0)]
            public new int SobelDegreeY
            {
                get => base.SobelDegreeY;
                set => base.SobelDegreeY = value;
            }

            [PropertyOrder(2)]
            [Category("Sobel")]
            [DisplayName("Kernel size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 31, 2, 0)]
            public new int SobelKernelSize
            {
                get => base.SobelKernelSize;
                set => base.SobelKernelSize = value;
            }

            [PropertyOrder(0)]
            [Category("Scharr")]
            [DisplayName("Degree X")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 1, 1, 0)]
            public new int ScharrDegreeX
            {
                get => base.ScharrDegreeX;
                set => base.ScharrDegreeX = value;
            }

            [PropertyOrder(1)]
            [Category("Scharr")]
            [DisplayName("Degree Y")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 1, 1, 0)]
            public new int ScharrDegreeY
            {
                get => base.ScharrDegreeY;
                set => base.ScharrDegreeY = value;
            }

            [PropertyOrder(0)]
            [Category("Laplacian")]
            [DisplayName("Kernel size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 31, 2, 0)]
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

