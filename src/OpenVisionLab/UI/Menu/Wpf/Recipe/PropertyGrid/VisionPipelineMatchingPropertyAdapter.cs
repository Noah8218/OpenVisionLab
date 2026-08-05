using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;
using static OpenVisionLab.VisionPipelineStepPropertyMapper;

namespace OpenVisionLab
{
    internal static class VisionPipelineMatchingPropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            out object property)
        {
            property = null;
            switch (NormalizeToolType(step?.ToolType))
            {
                case "matching":
                case "templatematching":
                    property = AttachStepMetadata(
                        ApplyCommonOpenCvProperty(new PipelineMatchingProperty(name)
                        {
                            MATCH_MODE = GetEnum(step.Parameters, nameof(MatchingProperty.MATCH_MODE), TemplateMatchModes.CCoeffNormed),
                            SCORE_MIN = GetDouble(step.Parameters, nameof(MatchingProperty.SCORE_MIN), 0.6),
                            MAGNIFIATION = GetDouble(step.Parameters, nameof(MatchingProperty.MAGNIFIATION), 1),
                            NUM_MATCH = GetInt(step.Parameters, nameof(MatchingProperty.NUM_MATCH), 3),
                            USE_FIND_ANGLE = GetBool(step.Parameters, nameof(MatchingProperty.USE_FIND_ANGLE), true),
                            FIND_ANGLE = GetDouble(step.Parameters, nameof(MatchingProperty.FIND_ANGLE), 0.1),
                            FIND_ANGLE_MAX = GetInt(step.Parameters, nameof(MatchingProperty.FIND_ANGLE_MAX), 10),
                            FIND_ANGLE_MIN = GetInt(step.Parameters, nameof(MatchingProperty.FIND_ANGLE_MIN), -10),
                            USE_COARSE_TO_FINE_ANGLE_SEARCH = GetBool(step.Parameters, nameof(MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), false),
                            COARSE_ANGLE_STEP = GetDouble(step.Parameters, nameof(MatchingProperty.COARSE_ANGLE_STEP), 5.0),
                            COARSE_ANGLE_TOP_K = GetInt(step.Parameters, nameof(MatchingProperty.COARSE_ANGLE_TOP_K), 3),
                            USE_FIND_SCALE = GetBool(step.Parameters, nameof(MatchingProperty.USE_FIND_SCALE), false),
                            FIND_SCALE_MIN = GetDouble(step.Parameters, nameof(MatchingProperty.FIND_SCALE_MIN), 0.9),
                            FIND_SCALE_MAX = GetDouble(step.Parameters, nameof(MatchingProperty.FIND_SCALE_MAX), 1.1),
                            FIND_SCALE_STEP = GetDouble(step.Parameters, nameof(MatchingProperty.FIND_SCALE_STEP), 0.05),
                            PATTERN_PATH = GetString(step.Parameters, nameof(MatchingProperty.PATTERN_PATH), GetString(step.Parameters, "TemplatePath", string.Empty)),
                            USE_CANNY = GetBool(step.Parameters, nameof(MatchingProperty.USE_CANNY), false),
                            CANNY_HIGH = GetInt(step.Parameters, nameof(MatchingProperty.CANNY_HIGH), 60),
                            CANNY_LOW = GetInt(step.Parameters, nameof(MatchingProperty.CANNY_LOW), 30),
                            USE_PADDING_COLOR_WHITE = GetBool(step.Parameters, nameof(MatchingProperty.USE_PADDING_COLOR_WHITE), false),
                            USE_AS_FIXTURE_FRAME = GetBool(step.Parameters, VisionPipelineFixtureFrameService.PublishParameter, false),
                            FIXTURE_FRAME_NAME = GetString(step.Parameters, VisionPipelineFixtureFrameService.FrameNameParameter, string.Empty),
                            FIXTURE_REFERENCE_X = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.ReferenceXParameter, 0D),
                            FIXTURE_REFERENCE_Y = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.ReferenceYParameter, 0D),
                            FIXTURE_REFERENCE_ANGLE = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.ReferenceAngleParameter, 0D),
                            FIXTURE_REFERENCE_SCALE = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.ReferenceScaleParameter, 1D),
                            FIXTURE_MAX_ANGLE_DELTA = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter, 2D),
                            FIXTURE_MIN_SCALE_RATIO = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.MinimumScaleRatioParameter, 0D),
                            FIXTURE_MAX_SCALE_RATIO = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.MaximumScaleRatioParameter, 0D),
                            FIXTURE_REFERENCE_IMAGE_WIDTH = GetInt(step.Parameters, VisionPipelineFixtureFrameService.ReferenceImageWidthParameter, 0),
                            FIXTURE_REFERENCE_IMAGE_HEIGHT = GetInt(step.Parameters, VisionPipelineFixtureFrameService.ReferenceImageHeightParameter, 0)
                        }, step.Parameters),
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
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep step)
        {
            if (property is not PipelineMatchingProperty matching)
            {
                step = null;
                return false;
            }

            step = VisionPipelineStepBuilder.FromProperty(
                matching,
                inputLayer,
                outputLayer);
            matching.ApplyFixtureParameters(step.Parameters);
            return true;
        }

        public static bool IsProperty(object property)
        {
            return property is PipelineMatchingProperty;
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

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Fixture", 10)]
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

            [PropertyOrder(0)]
            [Category("Fixture")]
            [DisplayName("Publish Fixture Frame")]
            [Description("Publish this single Matching result as a translation-only fixture frame. NUM_MATCH must be 1.")]
            public bool USE_AS_FIXTURE_FRAME { get; set; }

            [PropertyOrder(1)]
            [Category("Fixture")]
            [DisplayName("Fixture Frame Name")]
            [Description("Name used by later fixture consumers in this pipeline.")]
            public string FIXTURE_FRAME_NAME { get; set; } = string.Empty;

            [PropertyOrder(2)]
            [Category("Fixture")]
            [DisplayName("Reference Center X")]
            [Description("Matching center X in the taught reference image, in pixels.")]
            public double FIXTURE_REFERENCE_X { get; set; }

            [PropertyOrder(3)]
            [Category("Fixture")]
            [DisplayName("Reference Center Y")]
            [Description("Matching center Y in the taught reference image, in pixels.")]
            public double FIXTURE_REFERENCE_Y { get; set; }

            [PropertyOrder(4)]
            [Category("Fixture")]
            [DisplayName("Reference Angle")]
            [Description("Matching angle in the taught reference image, in degrees.")]
            public double FIXTURE_REFERENCE_ANGLE { get; set; }

            [PropertyOrder(5)]
            [Category("Fixture")]
            [DisplayName("Reference Scale")]
            [Description("Matching scale in the taught reference image. Use 1.0 for the original template size.")]
            public double FIXTURE_REFERENCE_SCALE { get; set; } = 1D;

            [PropertyOrder(6)]
            [Category("Fixture")]
            [DisplayName("Maximum Angle Delta")]
            [Description("Fail fixture application when the angle change exceeds this degree limit.")]
            public double FIXTURE_MAX_ANGLE_DELTA { get; set; } = 2D;

            [PropertyOrder(7)]
            [Category("Fixture")]
            [DisplayName("Minimum Scale Ratio")]
            [Description("Optional fail-closed minimum current/reference scale ratio. Set both scale-ratio limits above zero to enable.")]
            public double FIXTURE_MIN_SCALE_RATIO { get; set; }

            [PropertyOrder(8)]
            [Category("Fixture")]
            [DisplayName("Maximum Scale Ratio")]
            [Description("Optional fail-closed maximum current/reference scale ratio. Set both scale-ratio limits above zero to enable.")]
            public double FIXTURE_MAX_SCALE_RATIO { get; set; }

            [PropertyOrder(9)]
            [Category("Fixture")]
            [DisplayName("Reference Image Width")]
            [Description("Width of the operator-reviewed reference image. Required by NormalizeImage consumers.")]
            public int FIXTURE_REFERENCE_IMAGE_WIDTH { get; set; }

            [PropertyOrder(10)]
            [Category("Fixture")]
            [DisplayName("Reference Image Height")]
            [Description("Height of the operator-reviewed reference image. Required by NormalizeImage consumers.")]
            public int FIXTURE_REFERENCE_IMAGE_HEIGHT { get; set; }

            public void ApplyFixtureParameters(IDictionary<string, string> parameters)
            {
                if (parameters == null || !USE_AS_FIXTURE_FRAME)
                {
                    return;
                }

                parameters[VisionPipelineFixtureFrameService.PublishParameter] = Convert.ToString(true, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = FIXTURE_FRAME_NAME?.Trim() ?? string.Empty;
                parameters[VisionPipelineFixtureFrameService.ReferenceXParameter] = Convert.ToString(FIXTURE_REFERENCE_X, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.ReferenceYParameter] = Convert.ToString(FIXTURE_REFERENCE_Y, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.ReferenceAngleParameter] = Convert.ToString(FIXTURE_REFERENCE_ANGLE, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.ReferenceScaleParameter] = Convert.ToString(FIXTURE_REFERENCE_SCALE, CultureInfo.InvariantCulture);
                parameters[VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter] = Convert.ToString(FIXTURE_MAX_ANGLE_DELTA, CultureInfo.InvariantCulture);
                if (FIXTURE_MIN_SCALE_RATIO > 0D && FIXTURE_MAX_SCALE_RATIO > 0D)
                {
                    parameters[VisionPipelineFixtureFrameService.MinimumScaleRatioParameter] = Convert.ToString(FIXTURE_MIN_SCALE_RATIO, CultureInfo.InvariantCulture);
                    parameters[VisionPipelineFixtureFrameService.MaximumScaleRatioParameter] = Convert.ToString(FIXTURE_MAX_SCALE_RATIO, CultureInfo.InvariantCulture);
                }
                if (FIXTURE_REFERENCE_IMAGE_WIDTH > 0 && FIXTURE_REFERENCE_IMAGE_HEIGHT > 0)
                {
                    parameters[VisionPipelineFixtureFrameService.ReferenceImageWidthParameter] = Convert.ToString(FIXTURE_REFERENCE_IMAGE_WIDTH, CultureInfo.InvariantCulture);
                    parameters[VisionPipelineFixtureFrameService.ReferenceImageHeightParameter] = Convert.ToString(FIXTURE_REFERENCE_IMAGE_HEIGHT, CultureInfo.InvariantCulture);
                }
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
