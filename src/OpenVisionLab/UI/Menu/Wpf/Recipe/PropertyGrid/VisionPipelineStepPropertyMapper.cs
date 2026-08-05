using OpenVisionLab.Core;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.Core.FormulaUtil;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static class VisionPipelineStepPropertyMapper
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
            if (VisionPipelineOverlayMergePropertyAdapter.TryCreateProperty(
                step,
                name,
                out object overlayMergeProperty))
            {
                return overlayMergeProperty;
            }

            if (VisionPipelineTransformPropertyAdapter.TryCreateProperty(
                step,
                name,
                context,
                out object transformProperty))
            {
                return transformProperty;
            }

            if (VisionPipelineMultiMatchMeanPropertyAdapter.TryCreateProperty(
                step,
                name,
                context,
                out object multiMatchMeanProperty))
            {
                return multiMatchMeanProperty;
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

            if (VisionPipelineLinePropertyAdapter.TryCreateProperty(
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

            if (VisionPipelineMatchingPropertyAdapter.TryCreateProperty(
                step,
                name,
                out object matchingProperty))
            {
                return matchingProperty;
            }

            if (VisionPipelineEdgeBasedMatchingPropertyAdapter.TryCreateProperty(
                step,
                name,
                out object edgeBasedMatchingProperty))
            {
                return edgeBasedMatchingProperty;
            }

            if (VisionPipelineFeatureMatchingPropertyAdapter.TryCreateProperty(
                step,
                name,
                out object featureMatchingProperty))
            {
                return featureMatchingProperty;
            }

            if (VisionPipelineObjectInspectionPropertyAdapter.TryCreateProperty(
                step,
                name,
                out object objectInspectionProperty))
            {
                return objectInspectionProperty;
            }

            if (VisionPipelineBasicImagePropertyAdapter.TryCreateProperty(
                step,
                name,
                out object basicImageProperty))
            {
                return basicImageProperty;
            }

            switch (toolType)
            {
                case "mean":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineMeanProperty(name)
                    {
                        MEAN_MAX = GetInt(step.Parameters, nameof(MeanProperty.MEAN_MAX), 240),
                        MEAN_MIN = GetInt(step.Parameters, nameof(MeanProperty.MEAN_MIN), 100),
                        MEAN_TYPES = GetEnum(step.Parameters, nameof(MeanProperty.MEAN_TYPES), MeanType.Mean)
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
            if (VisionPipelineOverlayMergePropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep overlayMergeStep))
            {
                mapped = overlayMergeStep;
            }
            else if (VisionPipelineMultiMatchMeanPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep multiMatchMeanStep))
            {
                mapped = multiMatchMeanStep;
            }
            else if (VisionPipelineReferenceDifferencePropertyAdapter.TryCreateStep(
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
            else if (VisionPipelineLinePropertyAdapter.TryCreateStep(
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
            else if (VisionPipelineMatchingPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep matchingStep))
            {
                mapped = matchingStep;
            }
            else if (VisionPipelineEdgeBasedMatchingPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep edgeBasedMatchingStep))
            {
                mapped = edgeBasedMatchingStep;
            }
            else if (VisionPipelineFeatureMatchingPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep featureMatchingStep))
            {
                mapped = featureMatchingStep;
            }
            else if (VisionPipelineObjectInspectionPropertyAdapter.TryCreateStep(
                property,
                inputLayer,
                outputLayer,
                out VisionPipelineStep objectInspectionStep))
            {
                mapped = objectInspectionStep;
            }
            else if (property is OpenCvPropertyBase openCvProperty)
            {
                mapped = VisionPipelineStepBuilder.FromProperty(openCvProperty, inputLayer, outputLayer);
            }
            else if (VisionPipelineBasicImagePropertyAdapter.TryCreateStep(
                property,
                target.Name,
                inputLayer,
                outputLayer,
                out VisionPipelineStep basicImageStep))
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
            return VisionPipelineLinePropertyAdapter.TryCreateLineGaugePair(
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
            if (VisionPipelineOverlayMergePropertyAdapter.IsProperty(instance))
            {
                return "OverlayMerge";
            }

            if (VisionPipelineMultiMatchMeanPropertyAdapter.IsProperty(instance))
            {
                return "MultiMatchMean";
            }

            if (VisionPipelineReferenceDifferencePropertyAdapter.IsProperty(instance))
            {
                return "ReferenceDifference";
            }

            if (VisionPipelinePinArrayGapPropertyAdapter.IsProperty(instance))
            {
                return "PinArrayGap";
            }

            if (VisionPipelineLinePropertyAdapter.IsProperty(instance))
            {
                return "LineGauge";
            }

            string geometryToolType =
                VisionPipelineGeometryPropertyAdapter.ResolveMetricToolType(instance);
            if (!string.IsNullOrWhiteSpace(geometryToolType))
            {
                return geometryToolType;
            }

            if (VisionPipelineMatchingPropertyAdapter.IsProperty(instance))
            {
                return "Matching";
            }

            if (VisionPipelineEdgeBasedMatchingPropertyAdapter.IsProperty(instance))
            {
                return "EdgeBasedMatching";
            }

            if (VisionPipelineFeatureMatchingPropertyAdapter.IsProperty(instance))
            {
                return "FeatureMatching";
            }

            string objectInspectionToolType =
                VisionPipelineObjectInspectionPropertyAdapter.ResolveMetricToolType(instance);
            if (!string.IsNullOrWhiteSpace(objectInspectionToolType))
            {
                return objectInspectionToolType;
            }

            string basicImageToolType =
                VisionPipelineBasicImagePropertyAdapter.ResolveMetricToolType(instance);
            if (!string.IsNullOrWhiteSpace(basicImageToolType))
            {
                return basicImageToolType;
            }

            string transformToolType =
                VisionPipelineTransformPropertyAdapter.ResolveMetricToolType(instance);
            if (!string.IsNullOrWhiteSpace(transformToolType))
            {
                return transformToolType;
            }

            return instance is PipelineMeanProperty ? "Mean" : string.Empty;
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



    }
}
