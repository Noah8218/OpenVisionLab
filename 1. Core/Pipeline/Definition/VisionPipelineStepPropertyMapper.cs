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
        private static Func<GeometryMeasurementMode, bool, IEnumerable<string>> geometryFeatureAccessor
            = (_, __) => Enumerable.Empty<string>();
        private static Func<IEnumerable<string>> pointFeatureAccessor = () => Enumerable.Empty<string>();

        public static void SetLayerNameContext(Func<IEnumerable<string>> accessor)
        {
            layerNameAccessor = accessor ?? (() => Enumerable.Empty<string>());
        }

        public static void SetGeometryFeatureContext(Func<GeometryMeasurementMode, bool, IEnumerable<string>> accessor)
        {
            geometryFeatureAccessor = accessor ?? ((_, __) => Enumerable.Empty<string>());
        }

        public static void SetPointFeatureContext(Func<IEnumerable<string>> accessor)
        {
            pointFeatureAccessor = accessor ?? (() => Enumerable.Empty<string>());
        }

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
                        MAX_AREA = GetInt(step.Parameters, nameof(BlobProperty.MAX_AREA), 1000000),
                        MIN_WIDTH = GetInt(step.Parameters, nameof(BlobProperty.MIN_WIDTH), 0),
                        MAX_WIDTH = GetInt(step.Parameters, nameof(BlobProperty.MAX_WIDTH), 1000000),
                        MIN_HEIGHT = GetInt(step.Parameters, nameof(BlobProperty.MIN_HEIGHT), 0),
                        MAX_HEIGHT = GetInt(step.Parameters, nameof(BlobProperty.MAX_HEIGHT), 1000000),
                        USE_FIXTURE_FRAME = GetBool(step.Parameters, VisionPipelineFixtureFrameService.ConsumeParameter, false),
                        FIXTURE_FRAME_NAME = GetString(step.Parameters, VisionPipelineFixtureFrameService.FrameNameParameter, string.Empty),
                        ALLOW_BRANCH_INPUT = GetBool(step.Parameters, VisionPipelineNormalizer.AllowBranchInputParameter, false)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "contour":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineContourProperty(name)
                    {
                        USE_APPROXPOLYDP = GetBool(step.Parameters, nameof(ContourProperty.USE_APPROXPOLYDP), false),
                        USE_DRAW_IMAGE = GetBool(step.Parameters, nameof(ContourProperty.USE_DRAW_IMAGE), false),
                        DrawMode = GetEnum(step.Parameters, nameof(ContourProperty.DrawMode), ContourDrawMode.Outline),
                        ApproximationModes = GetEnum(step.Parameters, nameof(ContourProperty.ApproximationModes), ContourApproximationModes.ApproxSimple),
                        DetectMode = GetEnum(step.Parameters, nameof(ContourProperty.DetectMode), RetrievalModes.External),
                        EPSILON = GetDouble(step.Parameters, nameof(ContourProperty.EPSILON), 0.01),
                        MIN_AREA = GetInt(step.Parameters, nameof(ContourProperty.MIN_AREA), 200),
                        MAX_AREA = GetInt(step.Parameters, nameof(ContourProperty.MAX_AREA), 1000000),
                        MIN_WIDTH = GetInt(step.Parameters, nameof(ContourProperty.MIN_WIDTH), 0),
                        MAX_WIDTH = GetInt(step.Parameters, nameof(ContourProperty.MAX_WIDTH), 1000000),
                        MIN_HEIGHT = GetInt(step.Parameters, nameof(ContourProperty.MIN_HEIGHT), 0),
                        MAX_HEIGHT = GetInt(step.Parameters, nameof(ContourProperty.MAX_HEIGHT), 1000000),
                        DrawThickness = GetInt(step.Parameters, nameof(ContourProperty.DrawThickness), 2),
                        ClrGridHtml = GetString(step.Parameters, nameof(ContourProperty.ClrGridHtml), "#ff0000")
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
                case "linedistance":
                case "lineintersection":
                    return AttachStepMetadata(CreatePipelineLinePairProperty(step, name), name, step.InputLayer, step.OutputLayer);
                case "pinarraygap":
                case "adjacentpingap":
                    return AttachStepMetadata(new PipelinePinArrayGapProperty(step, name), name, step.InputLayer, step.OutputLayer);
                case "geometrymeasure":
                case "geometricmeasurement":
                    return AttachStepMetadata(new PipelineGeometryMeasureProperty(step, name), name, step.InputLayer, step.OutputLayer);
                case "circlegauge":
                    return AttachStepMetadata(new PipelineCircleGaugeProperty(step, name), name, step.InputLayer, step.OutputLayer);
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
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineEdgeBasedMatchingProperty(name)
                    {
                        SCORE_MIN = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.SCORE_MIN), 0.75),
                        NUM_MATCH = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.NUM_MATCH), 1),
                        USE_UNIQUE_MATCH_VALIDATION = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION), false),
                        UNIQUE_MATCH_MIN_SCORE_MARGIN = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.UNIQUE_MATCH_MIN_SCORE_MARGIN), 0.03),
                        PATTERN_PATH = GetString(step.Parameters, nameof(EdgeBasedMatchingProperty.PATTERN_PATH), GetString(step.Parameters, "TemplatePath", string.Empty)),
                        USE_FIND_ANGLE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_FIND_ANGLE), false),
                        FIND_ANGLE = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE), 1.0),
                        FIND_ANGLE_MAX = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MAX), 10),
                        FIND_ANGLE_MIN = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MIN), -10),
                        USE_COARSE_TO_FINE_ANGLE_SEARCH = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), false),
                        COARSE_ANGLE_STEP = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_STEP), 5.0),
                        COARSE_ANGLE_TOP_K = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_TOP_K), 3),
                        CANNY_LOW = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_LOW), 30),
                        CANNY_HIGH = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_HIGH), 90),
                        CANNY_APERTURE_SIZE = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_APERTURE_SIZE), 3),
                        USE_L2_GRADIENT = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_L2_GRADIENT), true),
                        CONTOUR_RETRIEVAL_MODE = GetEnum(step.Parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_RETRIEVAL_MODE), RetrievalModes.External),
                        CONTOUR_APPROXIMATION_MODE = GetEnum(step.Parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_APPROXIMATION_MODE), ContourApproximationModes.ApproxNone),
                        GREEDINESS = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.GREEDINESS), 0.9),
                        SEARCH_STEP = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.SEARCH_STEP), 2),
                        USE_POSITION_REFINE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_POSITION_REFINE), false),
                        USE_HYBRID_VERIFY = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_HYBRID_VERIFY), false),
                        HYBRID_VERIFY_TOP_N = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_TOP_N), 5),
                        HYBRID_VERIFY_IMAGE_WEIGHT = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_IMAGE_WEIGHT), 0.35),
                        MAX_TEMPLATE_POINTS = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.MAX_TEMPLATE_POINTS), 300),
                        MIN_GRADIENT_MAGNITUDE = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.MIN_GRADIENT_MAGNITUDE), 1),
                        USE_DRAW_IMAGE = GetBool(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_DRAW_IMAGE), true)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "mean":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineMeanProperty(name)
                    {
                        MEAN_MAX = GetInt(step.Parameters, nameof(MeanProperty.MEAN_MAX), 240),
                        MEAN_MIN = GetInt(step.Parameters, nameof(MeanProperty.MEAN_MIN), 100),
                        MEAN_TYPES = GetEnum(step.Parameters, nameof(MeanProperty.MEAN_TYPES), MeanType.Mean)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "referencedifference":
                    return AttachStepMetadata(new PipelineReferenceDifferenceProperty
                    {
                        ReferencePath1 = GetString(step.Parameters, "ReferencePath1", GetReferencePath(step.Parameters, 0)),
                        ReferencePath2 = GetString(step.Parameters, "ReferencePath2", GetReferencePath(step.Parameters, 1)),
                        ReferencePath3 = GetString(step.Parameters, "ReferencePath3", GetReferencePath(step.Parameters, 2)),
                        ReferencePath4 = GetString(step.Parameters, "ReferencePath4", GetReferencePath(step.Parameters, 3)),
                        DifferenceThreshold = GetInt(step.Parameters, "DifferenceThreshold", 35),
                        MinimumDefectArea = GetInt(step.Parameters, "MinimumDefectArea", 80),
                        MaximumDefectArea = GetInt(step.Parameters, "MaximumDefectArea", 20000),
                        MorphologyKernel = GetInt(step.Parameters, "MorphologyKernel", 3),
                        IgnoreBorder = GetInt(step.Parameters, "IgnoreBorder", 8),
                        OrbFeatures = GetInt(step.Parameters, "OrbFeatures", 1600),
                        MatchRatio = GetDouble(step.Parameters, "MatchRatio", 0.75),
                        MinimumInliers = GetInt(step.Parameters, "MinimumInliers", 12),
                        RansacThreshold = GetDouble(step.Parameters, "RansacThreshold", 3.0)
                    }, name, step.InputLayer, step.OutputLayer);
                case "rotatescale":
                case "rotateandscale":
                    return AttachStepMetadata(new PipelineRotateScaleToolProperty
                    {
                        Angle = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.Angle), 0d),
                        ScaleXPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleXPercent), 100d),
                        ScaleYPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleYPercent), 100d),
                        Interpolation = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.Interpolation), InterpolationFlags.Linear),
                        BorderType = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.BorderType), BorderTypes.Constant),
                        USE_FIXTURE_FRAME = GetBool(step.Parameters, VisionPipelineFixtureFrameService.ConsumeParameter, false),
                        FIXTURE_FRAME_NAME = GetString(step.Parameters, VisionPipelineFixtureFrameService.FrameNameParameter, string.Empty),
                        FIXTURE_APPLY_MODE = GetEnum(step.Parameters, VisionPipelineFixtureFrameService.ApplyModeParameter, VisionPipelineFixtureApplyMode.TranslationRoi),
                        FIXTURE_MIN_VALID_PIXEL_RATIO = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter, VisionPipelineFixtureFrameService.DefaultMinimumValidPixelRatio),
                        ALLOW_BRANCH_INPUT = GetBool(step.Parameters, VisionPipelineNormalizer.AllowBranchInputParameter, false)
                    }, name, step.InputLayer, step.OutputLayer);
                case "affine":
                case "affinematrix":
                case "affinetransform":
                    return AttachStepMetadata(new PipelineAffineTransformToolProperty
                    {
                        UseDetectedSourcePoints = GetBool(step.Parameters, VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter, false),
                        SourcePoint1Feature = GetString(step.Parameters, VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter, string.Empty),
                        SourcePoint2Feature = GetString(step.Parameters, VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter, string.Empty),
                        SourcePoint3Feature = GetString(step.Parameters, VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter, string.Empty),
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
                        MinimumSourceTriangleArea = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.MinimumSourceTriangleArea), 1d),
                        MinimumDestinationTriangleArea = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.MinimumDestinationTriangleArea), 1d),
                        MinimumValidPixelRatio = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.MinimumValidPixelRatio), 0d)
                    }, name, step.InputLayer, step.OutputLayer);
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
            if (property is PipelineReferenceDifferenceProperty referenceDifference)
            {
                mapped = referenceDifference.ToStep(inputLayer, outputLayer);
            }
            else if (property is PipelinePinArrayGapProperty pinArrayGap)
            {
                mapped = pinArrayGap.ToStep(inputLayer, outputLayer);
            }
            else if (property is PipelineLinePairProperty linePair)
            {
                mapped = linePair.ToStep(inputLayer, outputLayer);
            }
            else if (property is PipelineGeometryMeasureProperty geometryMeasure)
            {
                mapped = geometryMeasure.ToStep(inputLayer, outputLayer);
            }
            else if (property is PipelineCircleGaugeProperty circleGauge)
            {
                mapped = circleGauge.ToStep(inputLayer, outputLayer);
            }
            else if (property is OpenCvPropertyBase openCvProperty)
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
            else if (property is RotateScaleToolProperty rotateScale)
            {
                mapped = VisionPipelineStepBuilder.FromRotateScaleProperty(rotateScale, GetPropertyName(property, target.Name), inputLayer, outputLayer);
            }
            else if (property is AffineTransformToolProperty affineTransform)
            {
                mapped = VisionPipelineStepBuilder.FromAffineTransformProperty(
                    affineTransform,
                    GetPropertyName(property, target.Name),
                    inputLayer,
                    outputLayer);
            }

            if (mapped == null)
            {
                return false;
            }

            if (property is PipelineMatchingProperty matchingFixture)
            {
                matchingFixture.ApplyFixtureParameters(mapped.Parameters);
            }
            else if (property is PipelineBlobProperty blobFixture)
            {
                blobFixture.ApplyFixtureParameters(mapped.Parameters);
            }
            else if (property is PipelineRotateScaleToolProperty normalizeFixture)
            {
                normalizeFixture.ApplyFixtureParameters(mapped.Parameters);
            }
            else if (property is PipelineAffineTransformToolProperty affinePointBinding)
            {
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter,
                    affinePointBinding.UseDetectedSourcePoints);
                mapped.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] =
                    affinePointBinding.SourcePoint1Feature?.Trim() ?? string.Empty;
                mapped.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] =
                    affinePointBinding.SourcePoint2Feature?.Trim() ?? string.Empty;
                mapped.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] =
                    affinePointBinding.SourcePoint3Feature?.Trim() ?? string.Empty;
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

        private static PipelineLinePairProperty CreatePipelineLinePairProperty(VisionPipelineStep step, string name)
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

        private static int GetPrefixedInt(IDictionary<string, string> parameters, string prefix, string key, int defaultValue)
        {
            string value = GetPrefixedValue(parameters, prefix, key);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : GetInt(parameters, key, defaultValue);
        }

        private static double GetPrefixedDouble(IDictionary<string, string> parameters, string prefix, string key, double defaultValue)
        {
            string value = GetPrefixedValue(parameters, prefix, key);
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                ? result
                : GetDouble(parameters, key, defaultValue);
        }

        private static bool GetPrefixedBool(IDictionary<string, string> parameters, string prefix, string key, bool defaultValue)
        {
            string value = GetPrefixedValue(parameters, prefix, key);
            return bool.TryParse(value, out bool result)
                ? result
                : GetBool(parameters, key, defaultValue);
        }

        private static TEnum GetPrefixedEnum<TEnum>(IDictionary<string, string> parameters, string prefix, string key, TEnum defaultValue)
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

        private static void ApplyPrefixedOpenCvProperty(
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

        public sealed class PipelineGeometryFeatureConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (!(context?.Instance is PipelineGeometryMeasureProperty property))
                {
                    return new StandardValuesCollection(Array.Empty<string>());
                }

                bool sourceA = string.Equals(
                    context.PropertyDescriptor?.Name,
                    nameof(PipelineGeometryMeasureProperty.SourceA),
                    StringComparison.Ordinal);
                string[] values = geometryFeatureAccessor(property.MeasurementMode, sourceA)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                return new StandardValuesCollection(values);
            }
        }

        public sealed class PipelinePointFeatureConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (!(context?.Instance is PipelineAffineTransformToolProperty))
                {
                    return new StandardValuesCollection(Array.Empty<string>());
                }

                string[] values = pointFeatureAccessor()
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
            switch (instance)
            {
                case PipelineBlobProperty _:
                    return "Blob";
                case PipelineContourProperty _:
                    return "Contour";
                case PipelineLineGaugeProperty _:
                case PipelineLinePairProperty _:
                    return "LineGauge";
                case PipelinePinArrayGapProperty _:
                    return "PinArrayGap";
                case PipelineGeometryMeasureProperty _:
                    return "GeometryMeasure";
                case PipelineCircleGaugeProperty _:
                    return "CircleGauge";
                case PipelineMatchingProperty _:
                    return "Matching";
                case PipelineEdgeBasedMatchingProperty _:
                    return "EdgeBasedMatching";
                case PipelineMeanProperty _:
                    return "Mean";
                case PipelineReferenceDifferenceProperty _:
                    return "ReferenceDifference";
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
        [CategoryOrder("Blob Parameter", 0)]
        [CategoryOrder("Fixture", 5)]
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

            [PropertyOrder(0)]
            [Category("Fixture")]
            [DisplayName("Use Fixture Frame")]
            [Description("Move this step's single CvROI by a previously published Matching fixture frame. Preview/Run remains explicit.")]
            public bool USE_FIXTURE_FRAME { get; set; }

            [PropertyOrder(1)]
            [Category("Fixture")]
            [DisplayName("Fixture Frame Name")]
            [Description("Name of the earlier Matching fixture frame. The producer and consumer names must match.")]
            public string FIXTURE_FRAME_NAME { get; set; } = string.Empty;

            [PropertyOrder(2)]
            [Category("Fixture")]
            [DisplayName("Allow Branch Input")]
            [Description("Explicitly allow this step to read the same source layer as the Matching fixture producer.")]
            public bool ALLOW_BRANCH_INPUT { get; set; }

            public void ApplyFixtureParameters(IDictionary<string, string> parameters)
            {
                if (parameters == null)
                {
                    return;
                }

                if (USE_FIXTURE_FRAME)
                {
                    parameters[VisionPipelineFixtureFrameService.ConsumeParameter] = Convert.ToString(true, CultureInfo.InvariantCulture);
                    parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = FIXTURE_FRAME_NAME?.Trim() ?? string.Empty;
                }

                if (ALLOW_BRANCH_INPUT)
                {
                    parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = Convert.ToString(true, CultureInfo.InvariantCulture);
                }
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
        private sealed class PipelineLinePairProperty : IPipelineStepMetadata
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

        private static void AddParameter(IDictionary<string, string> parameters, string key, object value)
        {
            parameters[key] = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private abstract class PipelineGeometryPropertyBase : IPipelineStepMetadata
        {
            protected PipelineGeometryPropertyBase(VisionPipelineStep step, string name)
            {
                BaselineParameters = step?.Parameters == null
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(step.Parameters, StringComparer.OrdinalIgnoreCase);
                PipelineStepName = name;
            }

            protected Dictionary<string, string> BaselineParameters { get; }
            [Category("Step"), DisplayName("Step Name"), PropertyOrder(-3)] public string PipelineStepName { get; set; }
            [Category("Step"), DisplayName("Input Layer"), TypeConverter(typeof(PipelineLayerNameConverter)), PropertyOrder(-2)] public string InputLayer { get; set; } = "Main";
            [Category("Step"), DisplayName("Output Layer"), TypeConverter(typeof(PipelineLayerNameConverter)), PropertyOrder(-1)] public string OutputLayer { get; set; } = "Geometry_Output";
            [Category("Step"), DisplayName("Enabled"), PropertyOrder(0)] public bool Enabled { get; set; } = true;
            [Category("Acceptance"), DisplayName("Use Acceptance"), PropertyOrder(1)] public bool UseAcceptance { get; set; }
            [Category("Acceptance"), DisplayName("Expected Success"), PropertyOrder(2)] public bool ExpectedSuccess { get; set; } = true;
            [Category("Acceptance"), DisplayName("Max Elapsed (ms)"), PropertyOrder(3)] public double MaxElapsedMilliseconds { get; set; }
            [Category("Acceptance"), DisplayName("Required Message"), PropertyOrder(4)] public string RequiredMessageText { get; set; } = string.Empty;
            [Category("Acceptance"), DisplayName("Acceptance Metric"), TypeConverter(typeof(PipelineMetricNameConverter)), PropertyOrder(5)] public string AcceptanceMetricName { get; set; } = string.Empty;
            [Browsable(false)] public bool UseAcceptanceMetricMinimum { get; set; }
            [Category("Acceptance"), DisplayName("Metric range"), PropertyEditor(typeof(WpgMetricRangeEditor)), MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum)), PropertyOrder(7)] public double AcceptanceMetricMinimum { get; set; }
            [Browsable(false)] public bool UseAcceptanceMetricMaximum { get; set; }
            [Browsable(false)] public double AcceptanceMetricMaximum { get; set; }

            protected VisionPipelineStep CreateStep(string toolType, string inputLayer, string outputLayer)
            {
                VisionPipelineStep mapped = new VisionPipelineStep
                {
                    Name = string.IsNullOrWhiteSpace(PipelineStepName) ? toolType : PipelineStepName,
                    ToolType = toolType,
                    InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer,
                    OutputLayer = string.IsNullOrWhiteSpace(outputLayer) ? toolType + "_Output" : outputLayer
                };
                foreach (KeyValuePair<string, string> item in BaselineParameters) mapped.Parameters[item.Key] = item.Value;
                return mapped;
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Sources", 0)]
        [CategoryOrder("Geometry Gates", 1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineGeometryMeasureProperty : PipelineGeometryPropertyBase
        {
            public PipelineGeometryMeasureProperty(VisionPipelineStep step, string name) : base(step, name)
            {
                OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer) ? "GeometryMeasure_Output" : step.OutputLayer;
                MeasurementMode = GetEnum(step?.Parameters, VisionPipelineGeometryMeasureService.ModeParameter, GeometryMeasurementMode.PointPointDistance);
                SourceA = JoinGeometryReference(GetString(step?.Parameters, VisionPipelineGeometryMeasureService.SourceStepAParameter, string.Empty), GetString(step?.Parameters, VisionPipelineGeometryMeasureService.SourceFeatureAParameter, string.Empty));
                SourceB = JoinGeometryReference(GetString(step?.Parameters, VisionPipelineGeometryMeasureService.SourceStepBParameter, string.Empty), GetString(step?.Parameters, VisionPipelineGeometryMeasureService.SourceFeatureBParameter, string.Empty));
                MaximumParallelAngleDeltaDeg = GetDouble(step?.Parameters, VisionPipelineGeometryMeasureService.MaximumParallelAngleDeltaParameter, 2D);
                MaximumExtensionAPx = GetDouble(step?.Parameters, VisionPipelineGeometryMeasureService.MaximumExtensionAParameter, 100D);
                MaximumExtensionBPx = GetDouble(step?.Parameters, VisionPipelineGeometryMeasureService.MaximumExtensionBParameter, 100D);
                RequireResultInImage = GetBool(step?.Parameters, VisionPipelineGeometryMeasureService.RequireResultInImageParameter, true);
                UseResultRoi = GetBool(step?.Parameters, "USE_ROI", false);
                ResultRoi = GetRect(step?.Parameters, "CvROI", default);
            }

            [Category("Sources"), DisplayName("Measurement mode"), PropertyOrder(0)] public GeometryMeasurementMode MeasurementMode { get; set; }
            [Category("Sources"), DisplayName("Source A"), Description("Compatible typed feature from an earlier enabled Step."), TypeConverter(typeof(PipelineGeometryFeatureConverter)), PropertyOrder(1)] public string SourceA { get; set; } = string.Empty;
            [Category("Sources"), DisplayName("Source B"), Description("Compatible typed feature from an earlier enabled Step."), TypeConverter(typeof(PipelineGeometryFeatureConverter)), PropertyOrder(2)] public string SourceB { get; set; } = string.Empty;
            [Category("Geometry Gates"), DisplayName("Maximum parallel delta (deg)"), PropertyOrder(0)] public double MaximumParallelAngleDeltaDeg { get; set; } = 2D;
            [Category("Geometry Gates"), DisplayName("Maximum extension A (px)"), PropertyOrder(1)] public double MaximumExtensionAPx { get; set; } = 100D;
            [Category("Geometry Gates"), DisplayName("Maximum extension B (px)"), PropertyOrder(2)] public double MaximumExtensionBPx { get; set; } = 100D;
            [Category("Geometry Gates"), DisplayName("Require result in image"), PropertyOrder(3)] public bool RequireResultInImage { get; set; } = true;
            [Category("Geometry Gates"), DisplayName("Use result ROI"), PropertyOrder(4)] public bool UseResultRoi { get; set; }
            [Category("Geometry Gates"), DisplayName("Result ROI"), PropertyOrder(5)] public Rect ResultRoi { get; set; }

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                SplitGeometryReference(SourceA, out string stepA, out string featureA);
                SplitGeometryReference(SourceB, out string stepB, out string featureB);
                VisionPipelineStep mapped = CreateStep("GeometryMeasure", inputLayer, outputLayer);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.ModeParameter, MeasurementMode);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.SourceStepAParameter, stepA);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.SourceFeatureAParameter, featureA);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.SourceStepBParameter, stepB);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.SourceFeatureBParameter, featureB);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.MaximumParallelAngleDeltaParameter, MaximumParallelAngleDeltaDeg);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.MaximumExtensionAParameter, MaximumExtensionAPx);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.MaximumExtensionBParameter, MaximumExtensionBPx);
                AddParameter(mapped.Parameters, VisionPipelineGeometryMeasureService.RequireResultInImageParameter, RequireResultInImage);
                AddParameter(mapped.Parameters, "USE_ROI", UseResultRoi);
                AddParameter(mapped.Parameters, "CvROI", FormatGeometryRect(ResultRoi));
                AddParameter(mapped.Parameters, VisionPipelineNormalizer.AllowBranchInputParameter, true);
                return mapped;
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Annular Sector", 0)]
        [CategoryOrder("Edge Fit", 1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineCircleGaugeProperty : PipelineGeometryPropertyBase
        {
            public PipelineCircleGaugeProperty(VisionPipelineStep step, string name) : base(step, name)
            {
                OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer) ? "CircleGauge_Output" : step.OutputLayer;
                UseRoi = GetBool(step?.Parameters, "USE_ROI", true); Roi = GetRect(step?.Parameters, "CvROI", default);
                CenterX = GetDouble(step?.Parameters, "CENTER_X", 0D); CenterY = GetDouble(step?.Parameters, "CENTER_Y", 0D);
                MinimumRadius = GetDouble(step?.Parameters, "RADIUS_MIN", 20D); MaximumRadius = GetDouble(step?.Parameters, "RADIUS_MAX", 60D);
                StartAngleDeg = GetDouble(step?.Parameters, "START_ANGLE_DEG", 0D); SweepAngleDeg = GetDouble(step?.Parameters, "SWEEP_ANGLE_DEG", 360D);
                ScanCount = GetInt(step?.Parameters, "SCAN_COUNT", 180); EdgePolarity = GetEnum(step?.Parameters, "EDGE_POLARITY", CircleGaugeEdgePolarity.Either);
                MinimumContrast = GetDouble(step?.Parameters, "MIN_CONTRAST", 12D); MinimumSupportRatio = GetDouble(step?.Parameters, "MIN_SUPPORT_RATIO", 0.6D);
                MaximumFitResidualPx = GetDouble(step?.Parameters, "MAX_FIT_RESIDUAL_PX", 2D);
            }
            [Category("Annular Sector"), DisplayName("Use ROI"), PropertyOrder(0)] public bool UseRoi { get; set; }
            [Category("Annular Sector"), DisplayName("ROI"), PropertyOrder(1)] public Rect Roi { get; set; }
            [Category("Annular Sector"), DisplayName("Center X"), PropertyOrder(2)] public double CenterX { get; set; }
            [Category("Annular Sector"), DisplayName("Center Y"), PropertyOrder(3)] public double CenterY { get; set; }
            [Category("Annular Sector"), DisplayName("Minimum radius (px)"), PropertyOrder(4)] public double MinimumRadius { get; set; }
            [Category("Annular Sector"), DisplayName("Maximum radius (px)"), PropertyOrder(5)] public double MaximumRadius { get; set; }
            [Category("Annular Sector"), DisplayName("Start angle (deg)"), PropertyOrder(6)] public double StartAngleDeg { get; set; }
            [Category("Annular Sector"), DisplayName("Sweep angle (deg)"), PropertyOrder(7)] public double SweepAngleDeg { get; set; }
            [Category("Annular Sector"), DisplayName("Radial scan count"), PropertyOrder(8)] public int ScanCount { get; set; }
            [Category("Edge Fit"), DisplayName("Edge polarity"), PropertyOrder(0)] public CircleGaugeEdgePolarity EdgePolarity { get; set; }
            [Category("Edge Fit"), DisplayName("Minimum contrast"), PropertyOrder(1)] public double MinimumContrast { get; set; }
            [Category("Edge Fit"), DisplayName("Minimum support ratio"), PropertyOrder(2)] public double MinimumSupportRatio { get; set; }
            [Category("Edge Fit"), DisplayName("Maximum fit residual (px)"), PropertyOrder(3)] public double MaximumFitResidualPx { get; set; }
            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                VisionPipelineStep mapped = CreateStep("CircleGauge", inputLayer, outputLayer);
                AddParameter(mapped.Parameters, "USE_ROI", UseRoi); AddParameter(mapped.Parameters, "CvROI", FormatGeometryRect(Roi));
                AddParameter(mapped.Parameters, "CENTER_X", CenterX); AddParameter(mapped.Parameters, "CENTER_Y", CenterY);
                AddParameter(mapped.Parameters, "RADIUS_MIN", MinimumRadius); AddParameter(mapped.Parameters, "RADIUS_MAX", MaximumRadius);
                AddParameter(mapped.Parameters, "START_ANGLE_DEG", StartAngleDeg); AddParameter(mapped.Parameters, "SWEEP_ANGLE_DEG", SweepAngleDeg);
                AddParameter(mapped.Parameters, "SCAN_COUNT", ScanCount); AddParameter(mapped.Parameters, "EDGE_POLARITY", EdgePolarity);
                AddParameter(mapped.Parameters, "MIN_CONTRAST", MinimumContrast); AddParameter(mapped.Parameters, "MIN_SUPPORT_RATIO", MinimumSupportRatio);
                AddParameter(mapped.Parameters, "MAX_FIT_RESIDUAL_PX", MaximumFitResidualPx);
                return mapped;
            }
        }

        private static string JoinGeometryReference(string step, string feature)
        {
            return string.IsNullOrWhiteSpace(step) || string.IsNullOrWhiteSpace(feature) ? string.Empty : step.Trim() + "/" + feature.Trim();
        }

        private static void SplitGeometryReference(string reference, out string step, out string feature)
        {
            int slash = (reference ?? string.Empty).LastIndexOf('/');
            step = slash > 0 ? reference.Substring(0, slash).Trim() : string.Empty;
            feature = slash > 0 && slash < reference.Length - 1 ? reference.Substring(slash + 1).Trim() : string.Empty;
        }

        private static string FormatGeometryRect(Rect roi)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", roi.X, roi.Y, roi.Width, roi.Height);
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Measurement", 0)]
        [CategoryOrder("ROI", 1)]
        [CategoryOrder("Pin Detection", 2)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelinePinArrayGapProperty : IPipelineStepMetadata
        {
            private readonly Dictionary<string, string> baselineParameters;
            private readonly string toolType;

            public PipelinePinArrayGapProperty(VisionPipelineStep step, string name)
            {
                baselineParameters = step?.Parameters == null
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(step.Parameters, StringComparer.OrdinalIgnoreCase);
                toolType = string.IsNullOrWhiteSpace(step?.ToolType) ? "PinArrayGap" : step.ToolType.Trim();
                NAME = string.IsNullOrWhiteSpace(name) ? "PinArrayGap" : name;
                MeasurementMode = GetEnum(step?.Parameters, "MeasurementMode", PinArrayGapMeasurementMode.EdgeGap);
                UseRoi = GetBool(step?.Parameters, "USE_ROI", false);
                Roi = GetRect(step?.Parameters, "CvROI", default);
                DarkThreshold = GetInt(step?.Parameters, "DarkThreshold", 128);
                MinimumDarkCoverageRatio = GetDouble(step?.Parameters, "MinDarkCoverageRatio", 0.55D);
                MinimumPinWidth = GetInt(step?.Parameters, "MinPinWidth", 5);
                MaximumPinBreakWidth = GetInt(step?.Parameters, "MaxPinBreakWidth", 2);
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
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "PinArrayGap_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Measurement")]
            [DisplayName("Measurement mode")]
            [Description("EdgeGap measures adjacent empty clearance. CenterPitch measures adjacent detected pin centers. Both modes are pixel-based in this editor.")]
            public PinArrayGapMeasurementMode MeasurementMode { get; set; } = PinArrayGapMeasurementMode.EdgeGap;

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
            [TypeConverter(typeof(PipelineMetricNameConverter))]
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
                    InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer,
                    OutputLayer = string.IsNullOrWhiteSpace(outputLayer) ? "PinArrayGap_Output" : outputLayer
                };
                foreach (KeyValuePair<string, string> parameter in baselineParameters)
                {
                    step.Parameters[parameter.Key] = parameter.Value;
                }

                AddParameter(step.Parameters, "MeasurementMode", MeasurementMode);
                AddParameter(step.Parameters, "USE_ROI", UseRoi);
                AddParameter(step.Parameters, "CvROI", string.Format(
                    CultureInfo.InvariantCulture,
                    "{0},{1},{2},{3}",
                    Roi.X,
                    Roi.Y,
                    Roi.Width,
                    Roi.Height));
                AddParameter(step.Parameters, "DarkThreshold", DarkThreshold);
                AddParameter(step.Parameters, "MinDarkCoverageRatio", MinimumDarkCoverageRatio);
                AddParameter(step.Parameters, "MinPinWidth", MinimumPinWidth);
                AddParameter(step.Parameters, "MaxPinBreakWidth", MaximumPinBreakWidth);
                AddParameter(step.Parameters, "MinGapWidth", MinimumGapWidth);
                return step;
            }
        }

        private static string GetReferencePath(IDictionary<string, string> parameters, int index)
        {
            string[] paths = GetString(parameters, "ReferencePaths", string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return index >= 0 && index < paths.Length ? paths[index].Trim() : string.Empty;
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
        [CategoryOrder("Reference", 0)]
        [CategoryOrder("Defect", 1)]
        [CategoryOrder("Registration", 2)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class PipelineReferenceDifferenceProperty : IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "ReferenceDifference";

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
            public string OutputLayer { get; set; } = "ReferenceDifference_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Reference")]
            [DisplayName("Reference Path 1")]
            [Description("Required approved Good reference image. Preview/Run remains an explicit user action.")]
            public string ReferencePath1 { get; set; } = string.Empty;

            [PropertyOrder(1)]
            [Category("Reference")]
            [DisplayName("Reference Path 2")]
            public string ReferencePath2 { get; set; } = string.Empty;

            [PropertyOrder(2)]
            [Category("Reference")]
            [DisplayName("Reference Path 3")]
            public string ReferencePath3 { get; set; } = string.Empty;

            [PropertyOrder(3)]
            [Category("Reference")]
            [DisplayName("Reference Path 4")]
            public string ReferencePath4 { get; set; } = string.Empty;

            [PropertyOrder(0)]
            [Category("Defect")]
            [DisplayName("Difference Threshold")]
            public int DifferenceThreshold { get; set; } = 35;

            [PropertyOrder(1)]
            [Category("Defect")]
            [DisplayName("Minimum Defect Area")]
            public int MinimumDefectArea { get; set; } = 80;

            [PropertyOrder(2)]
            [Category("Defect")]
            [DisplayName("Maximum Defect Area")]
            public int MaximumDefectArea { get; set; } = 20000;

            [PropertyOrder(3)]
            [Category("Defect")]
            [DisplayName("Morphology Kernel")]
            public int MorphologyKernel { get; set; } = 3;

            [PropertyOrder(4)]
            [Category("Defect")]
            [DisplayName("Ignore Border")]
            public int IgnoreBorder { get; set; } = 8;

            [PropertyOrder(0)]
            [Category("Registration")]
            [DisplayName("ORB Features")]
            public int OrbFeatures { get; set; } = 1600;

            [PropertyOrder(1)]
            [Category("Registration")]
            [DisplayName("Match Ratio")]
            public double MatchRatio { get; set; } = 0.75;

            [PropertyOrder(2)]
            [Category("Registration")]
            [DisplayName("Minimum Inliers")]
            public int MinimumInliers { get; set; } = 12;

            [PropertyOrder(3)]
            [Category("Registration")]
            [DisplayName("RANSAC Threshold")]
            public double RansacThreshold { get; set; } = 3.0;

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
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            public double AcceptanceMetricMaximum { get; set; }

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                VisionPipelineStep step = new VisionPipelineStep
                {
                    Name = string.IsNullOrWhiteSpace(NAME) ? "ReferenceDifference" : NAME,
                    ToolType = "ReferenceDifference",
                    InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer,
                    OutputLayer = string.IsNullOrWhiteSpace(outputLayer) ? "ReferenceDifference_Output" : outputLayer
                };

                AddParameter(step.Parameters, "ReferencePath1", ReferencePath1);
                AddParameter(step.Parameters, "ReferencePath2", ReferencePath2);
                AddParameter(step.Parameters, "ReferencePath3", ReferencePath3);
                AddParameter(step.Parameters, "ReferencePath4", ReferencePath4);
                AddParameter(step.Parameters, "DifferenceThreshold", DifferenceThreshold);
                AddParameter(step.Parameters, "MinimumDefectArea", MinimumDefectArea);
                AddParameter(step.Parameters, "MaximumDefectArea", MaximumDefectArea);
                AddParameter(step.Parameters, "MorphologyKernel", MorphologyKernel);
                AddParameter(step.Parameters, "IgnoreBorder", IgnoreBorder);
                AddParameter(step.Parameters, "OrbFeatures", OrbFeatures);
                AddParameter(step.Parameters, "MatchRatio", MatchRatio);
                AddParameter(step.Parameters, "MinimumInliers", MinimumInliers);
                AddParameter(step.Parameters, "RansacThreshold", RansacThreshold);
                return step;
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
        private sealed class PipelineAffineTransformToolProperty : AffineTransformToolProperty, IPipelineStepMetadata
        {
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
        private sealed class PipelineRotateScaleToolProperty : RotateScaleToolProperty, IPipelineStepMetadata
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

