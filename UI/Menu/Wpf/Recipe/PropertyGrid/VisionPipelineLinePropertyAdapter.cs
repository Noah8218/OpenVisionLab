using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using static Lib.Common.FormulaUtil;
using static OpenVisionLab.PropertyGridEditorFactory;
using static OpenVisionLab.VisionPipelineStepPropertyMapper;

namespace OpenVisionLab
{
    internal static class VisionPipelineLinePropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            out object property)
        {
            property = null;
            string toolType = NormalizeToolType(step?.ToolType);
            if (toolType == "line" || toolType == "linegauge")
            {
                property = AttachStepMetadata(
                    CreateSingleLineGaugeProperty(step, name),
                    name,
                    step.InputLayer,
                    step.OutputLayer);
                return true;
            }

            if (toolType != "linedistance" && toolType != "lineintersection")
            {
                return false;
            }

            property = AttachStepMetadata(
                CreatePropertyCore(step, name),
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
            if (property is PipelineLineGaugeProperty lineGauge)
            {
                step = VisionPipelineStepBuilder.FromProperty(
                    lineGauge,
                    inputLayer,
                    outputLayer);
                return true;
            }

            if (property is PipelineLinePairProperty pair)
            {
                step = pair.ToStep(inputLayer, outputLayer);
                return true;
            }

            step = null;
            return false;
        }

        public static bool IsProperty(object property)
        {
            return property is PipelineLineGaugeProperty
                || property is PipelineLinePairProperty;
        }

        public static bool TryCreateLineGaugePair(
            object property,
            out LineGaugeProperty left,
            out LineGaugeProperty right)
        {
            if (property is PipelineLinePairProperty pair)
            {
                left = pair.CreateLeftProperty();
                right = pair.CreateRightProperty();
                return true;
            }

            left = null;
            right = null;
            return false;
        }

        private static PipelineLineGaugeProperty CreateSingleLineGaugeProperty(
            VisionPipelineStep step,
            string name)
        {
            return ApplyCommonOpenCvProperty(new PipelineLineGaugeProperty(name)
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
            }, step.Parameters);
        }

        private static PipelineLinePairProperty CreatePropertyCore(
            VisionPipelineStep step,
            string name)
        {
            string toolType = string.IsNullOrWhiteSpace(step?.ToolType) ? "LineDistance" : step.ToolType.Trim();
            LineGaugeProperty left = CreatePrefixedLineGaugeProperty(
                step?.Parameters,
                "Left",
                name + "_Left",
                PROJECTION_DIR.X_LTOR);
            LineGaugeProperty right = CreatePrefixedLineGaugeProperty(
                step?.Parameters,
                "Right",
                name + "_Right",
                PROJECTION_DIR.X_RTOL);

            return new PipelineLinePairProperty(name, toolType, left, right)
            {
                Purpose = GetString(step?.Parameters, "LinePurpose", toolType),
                UseGapEdgePair = GetBool(step?.Parameters, VisionPipelineGapEdgePairTool.UseParameter, false),
                GapCannyLow = GetInt(step?.Parameters, "CANNY_LOW", 10),
                GapCannyHigh = GetInt(step?.Parameters, "CANNY_HIGH", 45),
                GapMinimumPixels = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MinimumGapParameter, 12D),
                GapMaximumPixels = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MaximumGapParameter, 60D),
                GapMaximumAngleDegrees = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MaximumAngleParameter, 8D),
                GapMaximumParallelDeltaDegrees = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MaximumParallelDeltaParameter, 4D),
                GapMinimumSupportRatio = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MinimumSupportRatioParameter, 0.26D),
                GapMinimumDarkContrast = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MinimumDarkContrastParameter, 8D),
                GapMinimumDarkCoverageRatio = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MinimumDarkCoverageParameter, 0.25D),
                GapMinimumScoreMargin = GetDouble(step?.Parameters, VisionPipelineGapEdgePairTool.MinimumScoreMarginParameter, 0.05D)
            };
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

        private static LineGaugeProperty CreatePrefixedLineGaugeProperty(
            IDictionary<string, string> parameters,
            string prefix,
            string name,
            PROJECTION_DIR defaultDirection)
        {
            LineGaugeProperty property = new LineGaugeProperty(name)
            {
                PRJ_PORALITY = GetPrefixedEnum(parameters, prefix, nameof(LineGaugeProperty.PRJ_PORALITY), PROJECTION_POLARITY.BTOW),
                PRJ_DIR = GetPrefixedEnum(parameters, prefix, nameof(LineGaugeProperty.PRJ_DIR), defaultDirection),
                CONTRAST = GetPrefixedDouble(parameters, prefix, nameof(LineGaugeProperty.CONTRAST), 30),
                THICKNESS = GetPrefixedDouble(parameters, prefix, nameof(LineGaugeProperty.THICKNESS), 5),
                SAMPLING_STEP = GetPrefixedDouble(parameters, prefix, nameof(LineGaugeProperty.SAMPLING_STEP), 10),
                VER_PRJ_DIR = GetPrefixedEnum(parameters, prefix, nameof(LineGaugeProperty.VER_PRJ_DIR), PROJECTION_DIR.X_LTOR),
                POINT_RANGE = GetPrefixedInt(parameters, prefix, nameof(LineGaugeProperty.POINT_RANGE), 10),
                USE_MANUAL_ANGLE = GetPrefixedBool(parameters, prefix, nameof(LineGaugeProperty.USE_MANUAL_ANGLE), false),
                MANUAL_ANGLE_VALUE = GetPrefixedDouble(parameters, prefix, nameof(LineGaugeProperty.MANUAL_ANGLE_VALUE), 0),
                USE_EXTEND_FIT_LINE = GetPrefixedBool(parameters, prefix, nameof(LineGaugeProperty.USE_EXTEND_FIT_LINE), false),
                EXTEND_FIT_LINE_VALUE = GetPrefixedInt(parameters, prefix, nameof(LineGaugeProperty.EXTEND_FIT_LINE_VALUE), 100),
                AVERAGE_Diff = GetPrefixedDouble(parameters, prefix, nameof(LineGaugeProperty.AVERAGE_Diff), 100),
                USE_AVERAGE_FILTER = GetPrefixedBool(parameters, prefix, nameof(LineGaugeProperty.USE_AVERAGE_FILTER), false),
                AVERAGE_FILTER_TYPE = GetPrefixedEnum(parameters, prefix, nameof(LineGaugeProperty.AVERAGE_FILTER_TYPE), LineGaugeProperty.AVERAGE_FILTER_TYPES.Y),
                SHOW_VERTICAL_LINE = GetPrefixedBool(parameters, prefix, nameof(LineGaugeProperty.SHOW_VERTICAL_LINE), true),
                SHOW_EDGE = GetPrefixedBool(parameters, prefix, nameof(LineGaugeProperty.SHOW_EDGE), true),
                SHOW_CONTOUR = GetPrefixedBool(parameters, prefix, nameof(LineGaugeProperty.SHOW_CONTOUR), true),
                SHOW_FITLINE = GetPrefixedBool(parameters, prefix, nameof(LineGaugeProperty.SHOW_FITLINE), true)
            };

            ApplyCommonOpenCvProperty(property, parameters);
            ApplyPrefixedOpenCvProperty(property, parameters, prefix);
            return property;
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
        [CategoryOrder("Line Pair", 0)]
        [CategoryOrder("Gap Edge Pair", 5)]
        [CategoryOrder("Left Line", 10)]
        [CategoryOrder("Right Line", 11)]
        [CategoryOrder("Threshold", 20)]
        [CategoryOrder("Acceptance", 40)]
        private sealed class PipelineLinePairProperty :
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            private readonly LineGaugeProperty leftBaseline;
            private readonly LineGaugeProperty rightBaseline;

            public PipelineLinePairProperty(
                string name,
                string toolType,
                LineGaugeProperty left,
                LineGaugeProperty right)
            {
                leftBaseline = (LineGaugeProperty)(left?.DeepCopy() ?? new LineGaugeProperty(name + "_Left"));
                rightBaseline = (LineGaugeProperty)(right?.DeepCopy() ?? new LineGaugeProperty(name + "_Right"));
                PipelineStepName = string.IsNullOrWhiteSpace(name) ? "LineDistance" : name;
                ToolType = string.IsNullOrWhiteSpace(toolType) ? "LineDistance" : toolType.Trim();
                Purpose = ToolType;
                PixelPerMm = left?.PIXELPERMM ?? 1D;
                UseRoi = left?.USE_ROI ?? false;
                Roi = left?.CvROI ?? default;
                UseThreshold = left?.USE_THRESHOLD ?? false;
                UseBitwiseNot = left?.USE_BITWISENOT ?? false;
                ThresholdType = left?.THRESHOLD_TYPES ?? ThresholdTypes.Binary;
                Threshold = left?.THRESHOLD ?? 127D;
                UseAdaptiveThreshold = left?.USE_ADAPTIVE_THRESHOLD ?? false;
                AdaptiveThreshold = left?.ADAPTIVE_THRESHOLD ?? 127D;
                Polarity = left?.PRJ_PORALITY ?? PROJECTION_POLARITY.BTOW;
                Contrast = left?.CONTRAST ?? 30D;
                Thickness = left?.THICKNESS ?? 5D;
                SamplingStep = left?.SAMPLING_STEP ?? 10D;
                VerticalProjectionDirection = left?.VER_PRJ_DIR ?? PROJECTION_DIR.X_LTOR;
                PointRange = left?.POINT_RANGE ?? 10;
                UseManualAngle = left?.USE_MANUAL_ANGLE ?? false;
                ManualAngleValue = left?.MANUAL_ANGLE_VALUE ?? 0D;
                UseExtendFitLine = left?.USE_EXTEND_FIT_LINE ?? false;
                ExtendFitLineValue = left?.EXTEND_FIT_LINE_VALUE ?? 100;
                UseAverageFilter = left?.USE_AVERAGE_FILTER ?? false;
                AverageDiff = left?.AVERAGE_Diff ?? 100D;
                AverageFilterType = left?.AVERAGE_FILTER_TYPE ?? LineGaugeProperty.AVERAGE_FILTER_TYPES.Y;
                ShowVerticalLine = left?.SHOW_VERTICAL_LINE ?? true;
                ShowEdge = left?.SHOW_EDGE ?? true;
                ShowContour = left?.SHOW_CONTOUR ?? true;
                ShowFitLine = left?.SHOW_FITLINE ?? true;
                LeftDirection = left?.PRJ_DIR ?? PROJECTION_DIR.X_LTOR;
                RightDirection = right?.PRJ_DIR ?? PROJECTION_DIR.X_RTOL;
                RightUseRoi = right?.USE_ROI ?? false;
                RightRoi = right?.CvROI ?? default;
                RightPolarity = right?.PRJ_PORALITY ?? PROJECTION_POLARITY.BTOW;
                RightVerticalProjectionDirection = right?.VER_PRJ_DIR ?? PROJECTION_DIR.X_LTOR;
                RightUseManualAngle = right?.USE_MANUAL_ANGLE ?? false;
                RightManualAngleValue = right?.MANUAL_ANGLE_VALUE ?? 0D;
            }

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
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Line Pair")]
            [DisplayName("Tool Type")]
            public string ToolType { get; set; }

            [PropertyOrder(1)]
            [Category("Line Pair")]
            [DisplayName("Purpose")]
            public string Purpose { get; set; }

            [PropertyOrder(2)]
            [Category("Line Pair")]
            [DisplayName("Pixel per mm")]
            public double PixelPerMm { get; set; }

            [PropertyOrder(0)]
            [Category("Gap Edge Pair")]
            [DisplayName("Use dark Gap edge pair")]
            [Description("Find and measure one long dark band's upper/lower edges inside the reviewed ROI. Ambiguous pairs fail closed.")]
            public bool UseGapEdgePair { get; set; }

            [PropertyOrder(1)]
            [Category("Gap Edge Pair")]
            [DisplayName("Canny low")]
            [Description("Lower edge threshold used to collect candidate boundaries inside the coarse ROI.")]
            public int GapCannyLow { get; set; } = 10;

            [PropertyOrder(2)]
            [Category("Gap Edge Pair")]
            [DisplayName("Canny high")]
            [Description("Upper edge threshold used to collect candidate boundaries inside the coarse ROI.")]
            public int GapCannyHigh { get; set; } = 45;

            [PropertyOrder(3)]
            [Category("Gap Edge Pair")]
            [DisplayName("Expected minimum thickness (px)")]
            [Description("Reject edge pairs whose upper-to-lower separation is smaller than the expected dark-band thickness.")]
            public double GapMinimumPixels { get; set; } = 12D;

            [PropertyOrder(4)]
            [Category("Gap Edge Pair")]
            [DisplayName("Expected maximum thickness (px)")]
            [Description("Reject edge pairs whose upper-to-lower separation is larger than the expected dark-band thickness.")]
            public double GapMaximumPixels { get; set; } = 60D;

            [PropertyOrder(5)]
            [Category("Gap Edge Pair")]
            [DisplayName("Maximum edge tilt (deg)")]
            [Description("Reject candidates whose absolute angle is too far from the expected horizontal band direction.")]
            public double GapMaximumAngleDegrees { get; set; } = 8D;

            [PropertyOrder(6)]
            [Category("Gap Edge Pair")]
            [DisplayName("Maximum parallel delta (deg)")]
            [Description("Maximum allowed angle difference between the selected upper and lower edges.")]
            public double GapMaximumParallelDeltaDegrees { get; set; } = 4D;

            [PropertyOrder(7)]
            [Category("Gap Edge Pair")]
            [DisplayName("Minimum shared support ratio")]
            [Description("Minimum shared horizontal edge support divided by coarse ROI width.")]
            public double GapMinimumSupportRatio { get; set; } = 0.26D;

            [PropertyOrder(8)]
            [Category("Gap Edge Pair")]
            [DisplayName("Minimum local dark contrast (GV)")]
            [Description("Minimum surrounding-minus-band gray-value contrast required for a dark-band pair.")]
            public double GapMinimumDarkContrast { get; set; } = 8D;

            [PropertyOrder(9)]
            [Category("Gap Edge Pair")]
            [DisplayName("Minimum local dark coverage")]
            [Description("Minimum fraction of sampled columns that must support local dark-band contrast.")]
            public double GapMinimumDarkCoverageRatio { get; set; } = 0.25D;

            [PropertyOrder(10)]
            [Category("Gap Edge Pair")]
            [DisplayName("Minimum distinct-pair score margin")]
            [Description("Reject the result when the selected pair is not sufficiently better than the next physically distinct pair.")]
            public double GapMinimumScoreMargin { get; set; } = 0.05D;

            [PropertyOrder(0)]
            [Category("Left Line")]
            [DisplayName("Line A use ROI")]
            public bool UseRoi { get; set; }

            [PropertyOrder(1)]
            [Category("Left Line")]
            [DisplayName("Line A ROI")]
            public Rect Roi { get; set; }

            [PropertyOrder(2)]
            [Category("Left Line")]
            [DisplayName("Line A projection direction")]
            public PROJECTION_DIR LeftDirection { get; set; }

            [PropertyOrder(0)]
            [Category("Right Line")]
            [DisplayName("Line B use ROI")]
            public bool RightUseRoi { get; set; }

            [PropertyOrder(1)]
            [Category("Right Line")]
            [DisplayName("Line B ROI")]
            public Rect RightRoi { get; set; }

            [PropertyOrder(2)]
            [Category("Right Line")]
            [DisplayName("Line B projection direction")]
            public PROJECTION_DIR RightDirection { get; set; }

            [PropertyOrder(3)]
            [Category("Left Line")]
            [DisplayName("Line A polarity")]
            public PROJECTION_POLARITY Polarity { get; set; }

            [PropertyOrder(3)]
            [Category("Right Line")]
            [DisplayName("Line B polarity")]
            public PROJECTION_POLARITY RightPolarity { get; set; }

            [PropertyOrder(2)]
            [Category("Line Pair")]
            [DisplayName("Contrast")]
            public double Contrast { get; set; }

            [PropertyOrder(3)]
            [Category("Line Pair")]
            [DisplayName("Thickness")]
            public double Thickness { get; set; }

            [PropertyOrder(4)]
            [Category("Line Pair")]
            [DisplayName("Sampling step")]
            public double SamplingStep { get; set; }

            [PropertyOrder(4)]
            [Category("Left Line")]
            [DisplayName("Line A vertical projection")]
            public PROJECTION_DIR VerticalProjectionDirection { get; set; }

            [PropertyOrder(4)]
            [Category("Right Line")]
            [DisplayName("Line B vertical projection")]
            public PROJECTION_DIR RightVerticalProjectionDirection { get; set; }

            [PropertyOrder(6)]
            [Category("Line Pair")]
            [DisplayName("Point range")]
            public int PointRange { get; set; }

            [PropertyOrder(5)]
            [Category("Left Line")]
            [DisplayName("Line A use manual angle")]
            public bool UseManualAngle { get; set; }

            [PropertyOrder(6)]
            [Category("Left Line")]
            [DisplayName("Line A manual angle")]
            public double ManualAngleValue { get; set; }

            [PropertyOrder(5)]
            [Category("Right Line")]
            [DisplayName("Line B use manual angle")]
            public bool RightUseManualAngle { get; set; }

            [PropertyOrder(6)]
            [Category("Right Line")]
            [DisplayName("Line B manual angle")]
            public double RightManualAngleValue { get; set; }

            [PropertyOrder(9)]
            [Category("Line Pair")]
            [DisplayName("Extend fit line")]
            public bool UseExtendFitLine { get; set; }

            [PropertyOrder(10)]
            [Category("Line Pair")]
            [DisplayName("Extend fit value")]
            public int ExtendFitLineValue { get; set; }

            [PropertyOrder(11)]
            [Category("Line Pair")]
            [DisplayName("Use average filter")]
            public bool UseAverageFilter { get; set; }

            [PropertyOrder(12)]
            [Category("Line Pair")]
            [DisplayName("Average diff")]
            public double AverageDiff { get; set; }

            [PropertyOrder(13)]
            [Category("Line Pair")]
            [DisplayName("Average filter type")]
            public LineGaugeProperty.AVERAGE_FILTER_TYPES AverageFilterType { get; set; }

            [PropertyOrder(14)]
            [Category("Line Pair")]
            [DisplayName("Show vertical line")]
            public bool ShowVerticalLine { get; set; }

            [PropertyOrder(15)]
            [Category("Line Pair")]
            [DisplayName("Show edge")]
            public bool ShowEdge { get; set; }

            [PropertyOrder(16)]
            [Category("Line Pair")]
            [DisplayName("Show contour")]
            public bool ShowContour { get; set; }

            [PropertyOrder(17)]
            [Category("Line Pair")]
            [DisplayName("Show fit line")]
            public bool ShowFitLine { get; set; }

            [PropertyOrder(0)]
            [Category("Threshold")]
            [DisplayName("Use threshold")]
            public bool UseThreshold { get; set; }

            [PropertyOrder(1)]
            [Category("Threshold")]
            [DisplayName("Use bitwise not")]
            public bool UseBitwiseNot { get; set; }

            [PropertyOrder(2)]
            [Category("Threshold")]
            [DisplayName("Threshold type")]
            public ThresholdTypes ThresholdType { get; set; }

            [PropertyOrder(3)]
            [Category("Threshold")]
            [DisplayName("Threshold")]
            public double Threshold { get; set; }

            [PropertyOrder(4)]
            [Category("Threshold")]
            [DisplayName("Use adaptive threshold")]
            public bool UseAdaptiveThreshold { get; set; }

            [PropertyOrder(5)]
            [Category("Threshold")]
            [DisplayName("Adaptive threshold")]
            public double AdaptiveThreshold { get; set; }

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

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                VisionPipelineStep step = VisionPipelineStepBuilder.FromLineGaugePair(
                    PipelineStepName,
                    string.IsNullOrWhiteSpace(ToolType) ? "LineDistance" : ToolType,
                    CreateLeftProperty(),
                    CreateRightProperty(),
                    inputLayer,
                    outputLayer,
                    Purpose);
                if (UseGapEdgePair)
                {
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.UseParameter, true);
                    AddParameter(step.Parameters, "CANNY_LOW", GapCannyLow);
                    AddParameter(step.Parameters, "CANNY_HIGH", GapCannyHigh);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MinimumGapParameter, GapMinimumPixels);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MaximumGapParameter, GapMaximumPixels);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MaximumAngleParameter, GapMaximumAngleDegrees);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MaximumParallelDeltaParameter, GapMaximumParallelDeltaDegrees);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MinimumSupportRatioParameter, GapMinimumSupportRatio);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MinimumDarkContrastParameter, GapMinimumDarkContrast);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MinimumDarkCoverageParameter, GapMinimumDarkCoverageRatio);
                    AddParameter(step.Parameters, VisionPipelineGapEdgePairTool.MinimumScoreMarginParameter, GapMinimumScoreMargin);
                }

                return step;
            }

            public LineGaugeProperty CreateLeftProperty()
            {
                return CreateLineProperty(leftBaseline, false);
            }

            public LineGaugeProperty CreateRightProperty()
            {
                return CreateLineProperty(rightBaseline, true);
            }

            private LineGaugeProperty CreateLineProperty(LineGaugeProperty baseline, bool right)
            {
                LineGaugeProperty property = (LineGaugeProperty)baseline.DeepCopy();
                property.USE_ROI = right ? RightUseRoi : UseRoi;
                property.CvROI = right ? RightRoi : Roi;
                property.PRJ_PORALITY = right ? RightPolarity : Polarity;
                property.PRJ_DIR = right ? RightDirection : LeftDirection;
                property.VER_PRJ_DIR = right ? RightVerticalProjectionDirection : VerticalProjectionDirection;
                property.USE_MANUAL_ANGLE = right ? RightUseManualAngle : UseManualAngle;
                property.MANUAL_ANGLE_VALUE = right ? RightManualAngleValue : ManualAngleValue;

                if (!right || PixelPerMm != leftBaseline.PIXELPERMM)
                {
                    property.PIXELPERMM = PixelPerMm;
                }

                if (!right || UseThreshold != leftBaseline.USE_THRESHOLD) property.USE_THRESHOLD = UseThreshold;
                if (!right || UseBitwiseNot != leftBaseline.USE_BITWISENOT) property.USE_BITWISENOT = UseBitwiseNot;
                if (!right || ThresholdType != leftBaseline.THRESHOLD_TYPES) property.THRESHOLD_TYPES = ThresholdType;
                if (!right || Threshold != leftBaseline.THRESHOLD) property.THRESHOLD = Threshold;
                if (!right || UseAdaptiveThreshold != leftBaseline.USE_ADAPTIVE_THRESHOLD) property.USE_ADAPTIVE_THRESHOLD = UseAdaptiveThreshold;
                if (!right || AdaptiveThreshold != leftBaseline.ADAPTIVE_THRESHOLD) property.ADAPTIVE_THRESHOLD = AdaptiveThreshold;
                if (!right || Contrast != leftBaseline.CONTRAST) property.CONTRAST = Contrast;
                if (!right || Thickness != leftBaseline.THICKNESS) property.THICKNESS = Thickness;
                if (!right || SamplingStep != leftBaseline.SAMPLING_STEP) property.SAMPLING_STEP = SamplingStep;
                if (!right || PointRange != leftBaseline.POINT_RANGE) property.POINT_RANGE = PointRange;
                if (!right || UseExtendFitLine != leftBaseline.USE_EXTEND_FIT_LINE) property.USE_EXTEND_FIT_LINE = UseExtendFitLine;
                if (!right || ExtendFitLineValue != leftBaseline.EXTEND_FIT_LINE_VALUE) property.EXTEND_FIT_LINE_VALUE = ExtendFitLineValue;
                if (!right || UseAverageFilter != leftBaseline.USE_AVERAGE_FILTER) property.USE_AVERAGE_FILTER = UseAverageFilter;
                if (!right || AverageDiff != leftBaseline.AVERAGE_Diff) property.AVERAGE_Diff = AverageDiff;
                if (!right || AverageFilterType != leftBaseline.AVERAGE_FILTER_TYPE) property.AVERAGE_FILTER_TYPE = AverageFilterType;
                if (!right || ShowVerticalLine != leftBaseline.SHOW_VERTICAL_LINE) property.SHOW_VERTICAL_LINE = ShowVerticalLine;
                if (!right || ShowEdge != leftBaseline.SHOW_EDGE) property.SHOW_EDGE = ShowEdge;
                if (!right || ShowContour != leftBaseline.SHOW_CONTOUR) property.SHOW_CONTOUR = ShowContour;
                if (!right || ShowFitLine != leftBaseline.SHOW_FITLINE) property.SHOW_FITLINE = ShowFitLine;
                return property;
            }
        }

    }
}
