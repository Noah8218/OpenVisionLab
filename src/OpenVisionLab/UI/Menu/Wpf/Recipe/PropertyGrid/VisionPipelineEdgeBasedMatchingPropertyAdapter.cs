using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenCvSharp;
using System;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;
using static OpenVisionLab.VisionPipelineStepPropertyMapper;

namespace OpenVisionLab
{
    internal static class VisionPipelineEdgeBasedMatchingPropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            out object property)
        {
            property = null;
            switch (NormalizeToolType(step?.ToolType))
            {
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    property = AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineEdgeBasedMatchingProperty(name)
                    {
                        SCORE_MIN = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.SCORE_MIN), 0.75),
                        NUM_MATCH = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.NUM_MATCH), 1),
                        USE_UNIQUE_MATCH_VALIDATION = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION), false),
                        UNIQUE_MATCH_MIN_SCORE_MARGIN = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.UNIQUE_MATCH_MIN_SCORE_MARGIN), 0.03),
                        ALLOW_GLOBAL_POLARITY_REVERSAL = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.ALLOW_GLOBAL_POLARITY_REVERSAL), false),
                        PATTERN_PATH = GetString(step.Parameters, nameof(EdgeBasedMatchingProperty.PATTERN_PATH), GetString(step.Parameters, "TemplatePath", string.Empty)),
                        USE_FIND_ANGLE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_FIND_ANGLE), false),
                        FIND_ANGLE = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE), 1.0),
                        FIND_ANGLE_MAX = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MAX), 10),
                        FIND_ANGLE_MIN = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MIN), -10),
                        USE_COARSE_TO_FINE_ANGLE_SEARCH = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), false),
                        COARSE_ANGLE_STEP = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_STEP), 5.0),
                        COARSE_ANGLE_TOP_K = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_TOP_K), 3),
                        USE_FIND_SCALE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_FIND_SCALE), false),
                        FIND_SCALE_MIN = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_MIN), 0.9),
                        FIND_SCALE_MAX = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_MAX), 1.1),
                        FIND_SCALE_STEP = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_STEP), 0.05),
                        CANNY_LOW = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_LOW), 30),
                        CANNY_HIGH = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_HIGH), 90),
                        CANNY_APERTURE_SIZE = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_APERTURE_SIZE), 3),
                        USE_L2_GRADIENT = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_L2_GRADIENT), true),
                        CONTOUR_RETRIEVAL_MODE = GetEnum(step.Parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_RETRIEVAL_MODE), RetrievalModes.External),
                        CONTOUR_APPROXIMATION_MODE = GetEnum(step.Parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_APPROXIMATION_MODE), ContourApproximationModes.ApproxNone),
                        GREEDINESS = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.GREEDINESS), 0.9),
                        SEARCH_STEP = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.SEARCH_STEP), 2),
                        USE_POSITION_REFINE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_POSITION_REFINE), false),
                        USE_SUBPIXEL_REFINE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_SUBPIXEL_REFINE), false),
                        USE_PYRAMID_POSITION_PROPOSAL = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_PYRAMID_POSITION_PROPOSAL), false),
                        PYRAMID_POSITION_TOP_N = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.PYRAMID_POSITION_TOP_N), 6),
                        PYRAMID_POSITION_MIN_SCORE = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.PYRAMID_POSITION_MIN_SCORE), 0.70),
                        USE_HYBRID_VERIFY = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_HYBRID_VERIFY), false),
                        HYBRID_VERIFY_TOP_N = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_TOP_N), 5),
                        HYBRID_VERIFY_IMAGE_WEIGHT = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_IMAGE_WEIGHT), 0.35),
                        MAX_TEMPLATE_POINTS = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.MAX_TEMPLATE_POINTS), 300),
                        MIN_GRADIENT_MAGNITUDE = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.MIN_GRADIENT_MAGNITUDE), 1),
                        USE_DRAW_IMAGE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_DRAW_IMAGE), true)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
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
            if (property is not PipelineEdgeBasedMatchingProperty edgeBasedMatching)
            {
                step = null;
                return false;
            }

            step = VisionPipelineStepBuilder.FromProperty(
                edgeBasedMatching,
                inputLayer,
                outputLayer);
            return true;
        }

        public static bool IsProperty(object property)
        {
            return property is PipelineEdgeBasedMatchingProperty;
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
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineEdgeBasedMatchingProperty : EdgeBasedMatchingProperty, IPipelineStepMetadata
        {
            public PipelineEdgeBasedMatchingProperty(string name)
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
