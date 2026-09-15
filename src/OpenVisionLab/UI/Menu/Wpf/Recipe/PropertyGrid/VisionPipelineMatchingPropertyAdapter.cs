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

        internal static string ResolveSampleTemplatePath(MatchingProperty source)
        {
            return VisionPipelineAppToolFactory.ResolveTemplatePath(source?.PATTERN_PATH);
        }

        internal static void ApplySampleProperty(MatchingProperty source, MatchingProperty target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.PIXELPERMM = source.PIXELPERMM;
            target.USE_THRESHOLD = source.USE_THRESHOLD;
            target.USE_BITWISENOT = source.USE_BITWISENOT;
            target.THRESHOLD_TYPES = source.THRESHOLD_TYPES;
            target.THRESHOLD = source.THRESHOLD;
            target.USE_ADAPTIVE_THRESHOLD = source.USE_ADAPTIVE_THRESHOLD;
            target.ADAPTIVE_THRESHOLD = source.ADAPTIVE_THRESHOLD;
            target.ADAPTIVE_THRESHOLD_TYPES = source.ADAPTIVE_THRESHOLD_TYPES;
            target.ADAPTIVE_THRESHOLD_ALGORITHM = source.ADAPTIVE_THRESHOLD_ALGORITHM;
            target.BlockSize = source.BlockSize;
            target.Weight = source.Weight;
            target.USE_ROI = source.USE_ROI;
            target.CvROI = source.CvROI;
            target.USE_MULTI_ROI = source.USE_MULTI_ROI;
            target.CvROIS = source.CvROIS == null
                ? new List<Rect>()
                : new List<Rect>(source.CvROIS);
            target.USE_MASKING = source.USE_MASKING;
            target.CvMASKS = source.CvMASKS == null
                ? new List<Rect>()
                : new List<Rect>(source.CvMASKS);
            target.AUTO_PREVIEW = false;
            target.MATCH_MODE = source.MATCH_MODE;
            target.SCORE_MIN = source.SCORE_MIN;
            target.NUM_MATCH = source.NUM_MATCH;
            target.MAGNIFIATION = source.MAGNIFIATION;
            target.USE_FIND_ANGLE = source.USE_FIND_ANGLE;
            target.FIND_ANGLE = source.FIND_ANGLE;
            target.FIND_ANGLE_MIN = source.FIND_ANGLE_MIN;
            target.FIND_ANGLE_MAX = source.FIND_ANGLE_MAX;
            target.USE_COARSE_TO_FINE_ANGLE_SEARCH = source.USE_COARSE_TO_FINE_ANGLE_SEARCH;
            target.COARSE_ANGLE_STEP = source.COARSE_ANGLE_STEP;
            target.COARSE_ANGLE_TOP_K = source.COARSE_ANGLE_TOP_K;
            target.USE_FIND_SCALE = source.USE_FIND_SCALE;
            target.FIND_SCALE_MIN = source.FIND_SCALE_MIN;
            target.FIND_SCALE_MAX = source.FIND_SCALE_MAX;
            target.FIND_SCALE_STEP = source.FIND_SCALE_STEP;
            target.USE_PYRAMID_POSITION_PROPOSAL = source.USE_PYRAMID_POSITION_PROPOSAL;
            target.PYRAMID_POSITION_TOP_N = source.PYRAMID_POSITION_TOP_N;
            target.PYRAMID_POSITION_MIN_SCORE = source.PYRAMID_POSITION_MIN_SCORE;
            target.USE_CANNY = source.USE_CANNY;
            target.CANNY_LOW = source.CANNY_LOW;
            target.CANNY_HIGH = source.CANNY_HIGH;
            target.USE_PADDING_COLOR_WHITE = source.USE_PADDING_COLOR_WHITE;
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
