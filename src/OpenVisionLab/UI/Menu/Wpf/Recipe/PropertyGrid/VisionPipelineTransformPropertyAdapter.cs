using OpenVisionLab.Core;
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
    internal static class VisionPipelineTransformPropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            VisionPipelinePropertyContext context,
            out object property)
        {
            property = null;
            if (step == null)
            {
                return false;
            }

            switch (NormalizeToolType(step.ToolType))
            {
                case "rotatescale":
                case "rotateandscale":
                    property = AttachStepMetadata(
                        new PipelineRotateScaleToolProperty
                        {
                            Angle = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.Angle), 0d),
                            ScaleXPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleXPercent), 100d),
                            ScaleYPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleYPercent), 100d),
                            Interpolation = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.Interpolation), InterpolationFlags.Linear),
                            BorderType = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.BorderType), BorderTypes.Constant),
                            USE_FIXTURE_FRAME = GetBool(step.Parameters, VisionPipelineFixtureFrameService.ConsumeParameter, false),
                            FIXTURE_FRAME_NAME = GetString(step.Parameters, VisionPipelineFixtureFrameService.FrameNameParameter, string.Empty),
                            FIXTURE_APPLY_MODE = GetEnum(
                                step.Parameters,
                                VisionPipelineFixtureFrameService.ApplyModeParameter,
                                VisionPipelineFixtureApplyMode.TranslationRoi),
                            FIXTURE_MIN_VALID_PIXEL_RATIO = GetDouble(
                                step.Parameters,
                                VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter,
                                VisionPipelineFixtureFrameService.DefaultMinimumValidPixelRatio),
                            ALLOW_BRANCH_INPUT = GetBool(
                                step.Parameters,
                                VisionPipelineNormalizer.AllowBranchInputParameter,
                                false)
                        },
                        name,
                        step.InputLayer,
                        step.OutputLayer);
                    return true;
                case "affine":
                case "affinematrix":
                case "affinetransform":
                    property = AttachStepMetadata(
                        new PipelineAffineTransformToolProperty
                        {
                            Context = context ?? VisionPipelinePropertyContext.Empty,
                            UseDetectedSourcePoints = GetBool(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter,
                                false),
                            SourcePoint1Feature = GetString(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter,
                                string.Empty),
                            SourcePoint2Feature = GetString(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter,
                                string.Empty),
                            SourcePoint3Feature = GetString(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter,
                                string.Empty),
                            SourcePoint1X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint1X), 0d),
                            SourcePoint1Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint1Y), 0d),
                            SourcePoint2X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint2X), 100d),
                            SourcePoint2Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint2Y), 0d),
                            SourcePoint3X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint3X), 0d),
                            SourcePoint3Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint3Y), 100d),
                            DestinationPoint1X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint1X), 0d),
                            DestinationPoint1Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint1Y), 0d),
                            DestinationPoint2X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint2X), 100d),
                            DestinationPoint2Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint2Y), 0d),
                            DestinationPoint3X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint3X), 0d),
                            DestinationPoint3Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint3Y), 100d),
                            OutputWidth = GetInt(step.Parameters, nameof(AffineTransformToolProperty.OutputWidth), 0),
                            OutputHeight = GetInt(step.Parameters, nameof(AffineTransformToolProperty.OutputHeight), 0),
                            Interpolation = GetEnum(step.Parameters, nameof(AffineTransformToolProperty.Interpolation), InterpolationFlags.Linear),
                            BorderType = GetEnum(step.Parameters, nameof(AffineTransformToolProperty.BorderType), BorderTypes.Constant),
                            BorderValue = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.BorderValue), 0d),
                            MinimumSourceTriangleArea = GetDouble(
                                step.Parameters,
                                nameof(AffineTransformToolProperty.MinimumSourceTriangleArea),
                                1d),
                            MinimumDestinationTriangleArea = GetDouble(
                                step.Parameters,
                                nameof(AffineTransformToolProperty.MinimumDestinationTriangleArea),
                                1d),
                            MinimumValidPixelRatio = GetDouble(
                                step.Parameters,
                                nameof(AffineTransformToolProperty.MinimumValidPixelRatio),
                                0d)
                        },
                        name,
                        step.InputLayer,
                        step.OutputLayer);
                    return true;
                default:
                    return false;
            }
        }

        public static bool TryCreateStep(
            object property,
            string fallbackName,
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep step)
        {
            step = null;
            if (property is RotateScaleToolProperty rotateScale)
            {
                step = VisionPipelineStepBuilder.FromRotateScaleProperty(
                    rotateScale,
                    GetPropertyName(property, fallbackName),
                    inputLayer,
                    outputLayer);
                if (property is PipelineRotateScaleToolProperty normalizeFixture)
                {
                    normalizeFixture.ApplyFixtureParameters(step.Parameters);
                }

                return true;
            }

            if (property is not AffineTransformToolProperty affineTransform)
            {
                return false;
            }

            step = VisionPipelineStepBuilder.FromAffineTransformProperty(
                affineTransform,
                GetPropertyName(property, fallbackName),
                inputLayer,
                outputLayer);
            if (property is PipelineAffineTransformToolProperty affinePointBinding)
            {
                AddParameter(
                    step.Parameters,
                    VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter,
                    affinePointBinding.UseDetectedSourcePoints);
                step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] =
                    affinePointBinding.SourcePoint1Feature?.Trim() ?? string.Empty;
                step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] =
                    affinePointBinding.SourcePoint2Feature?.Trim() ?? string.Empty;
                step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] =
                    affinePointBinding.SourcePoint3Feature?.Trim() ?? string.Empty;
            }

            return true;
        }

        public static string ResolveMetricToolType(object property)
        {
            if (property is PipelineRotateScaleToolProperty)
            {
                return "RotateScale";
            }

            return property is PipelineAffineTransformToolProperty
                ? "AffineTransform"
                : string.Empty;
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

        private static string GetPropertyName(object property, string fallback)
        {
            return property is VisionPipelineStepPropertyMapper.IPipelineStepMetadata metadata
                && !string.IsNullOrWhiteSpace(metadata.PipelineStepName)
                    ? metadata.PipelineStepName
                    : fallback;
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

        private static void AddParameter(
            IDictionary<string, string> parameters,
            string key,
            object value)
        {
            parameters[key] =
                Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
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

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Source Binding", 0)]
        [CategoryOrder("Source Points", 1)]
        [CategoryOrder("Destination Points", 2)]
        [CategoryOrder("Output", 3)]
        [CategoryOrder("Sampling", 4)]
        [CategoryOrder("Validation Gates", 5)]
        [CategoryOrder("Acceptance", 20)]
        internal sealed class PipelineAffineTransformToolProperty :
            AffineTransformToolProperty,
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            [Browsable(false)]
            public VisionPipelinePropertyContext Context { get; set; } =
                VisionPipelinePropertyContext.Empty;

            [PropertyOrder(-3), Category("Step"), DisplayName("Step Name")]
            public string NAME { get; set; } = "AffineTransform";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2), Category("Step"), DisplayName("Input Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1), Category("Step"), DisplayName("Output Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
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
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineMetricNameConverter))]
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
        internal sealed class PipelineRotateScaleToolProperty :
            RotateScaleToolProperty,
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata
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
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
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
            [DisplayName("蹂댁젙 諛⑹떇")]
            [Description("NormalizeImage applies the inverse Matching pose to the complete source image. TranslationRoi is reserved for ROI-capable consumers.")]
            public VisionPipelineFixtureApplyMode FIXTURE_APPLY_MODE { get; set; } =
                VisionPipelineFixtureApplyMode.TranslationRoi;

            [PropertyOrder(3)]
            [Category("Fixture")]
            [DisplayName("理쒖냼 ?좏슚 鍮꾩쑉")]
            [Description("Fail NormalizeImage when transformed source coverage is below this 0..1 ratio.")]
            public double FIXTURE_MIN_VALID_PIXEL_RATIO { get; set; } =
                VisionPipelineFixtureFrameService.DefaultMinimumValidPixelRatio;

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

                parameters[VisionPipelineFixtureFrameService.ConsumeParameter] =
                    Convert.ToString(true, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.FrameNameParameter] =
                    FIXTURE_FRAME_NAME?.Trim() ?? string.Empty;
                parameters[VisionPipelineFixtureFrameService.ApplyModeParameter] =
                    Convert.ToString(FIXTURE_APPLY_MODE, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter] =
                    Convert.ToString(FIXTURE_MIN_VALID_PIXEL_RATIO, CultureInfo.InvariantCulture);
                parameters[VisionPipelineNormalizer.AllowBranchInputParameter] =
                    Convert.ToString(ALLOW_BRANCH_INPUT, CultureInfo.InvariantCulture);
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
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineMetricNameConverter))]
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
