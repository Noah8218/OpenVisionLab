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
                        MAX_AREA = GetInt(step.Parameters, nameof(BlobProperty.MAX_AREA), 1000000),
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
                        FIXTURE_MAX_ANGLE_DELTA = GetDouble(step.Parameters, VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter, 2D)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineEdgeBasedMatchingProperty(name)
                    {
                        SCORE_MIN = GetDouble(step.Parameters, nameof(EdgeBasedMatchingProperty.SCORE_MIN), 0.75),
                        NUM_MATCH = GetInt(step.Parameters, nameof(EdgeBasedMatchingProperty.NUM_MATCH), 1),
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
                case "rotatescale":
                case "rotateandscale":
                    return AttachStepMetadata(new PipelineRotateScaleToolProperty
                    {
                        Angle = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.Angle), 0d),
                        ScaleXPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleXPercent), 100d),
                        ScaleYPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleYPercent), 100d),
                        Interpolation = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.Interpolation), InterpolationFlags.Linear),
                        BorderType = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.BorderType), BorderTypes.Constant)
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
            if (property is PipelineLinePairProperty linePair)
            {
                mapped = linePair.ToStep(inputLayer, outputLayer);
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
                Purpose = GetString(step?.Parameters, "LinePurpose", toolType)
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
        [CategoryOrder("Left Line", 10)]
        [CategoryOrder("Right Line", 11)]
        [CategoryOrder("Threshold", 20)]
        [CategoryOrder("Acceptance", 40)]
        private sealed class PipelineLinePairProperty : IPipelineStepMetadata
        {
            public PipelineLinePairProperty(
                string name,
                string toolType,
                LineGaugeProperty left,
                LineGaugeProperty right)
            {
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

            [PropertyOrder(3)]
            [Category("Line Pair")]
            [DisplayName("Use ROI")]
            public bool UseRoi { get; set; }

            [PropertyOrder(4)]
            [Category("Line Pair")]
            [DisplayName("ROI")]
            public Rect Roi { get; set; }

            [PropertyOrder(0)]
            [Category("Left Line")]
            [DisplayName("Projection direction")]
            public PROJECTION_DIR LeftDirection { get; set; }

            [PropertyOrder(0)]
            [Category("Right Line")]
            [DisplayName("Projection direction")]
            public PROJECTION_DIR RightDirection { get; set; }

            [PropertyOrder(1)]
            [Category("Line Pair")]
            [DisplayName("Polarity")]
            public PROJECTION_POLARITY Polarity { get; set; }

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

            [PropertyOrder(5)]
            [Category("Line Pair")]
            [DisplayName("Vertical projection")]
            public PROJECTION_DIR VerticalProjectionDirection { get; set; }

            [PropertyOrder(6)]
            [Category("Line Pair")]
            [DisplayName("Point range")]
            public int PointRange { get; set; }

            [PropertyOrder(7)]
            [Category("Line Pair")]
            [DisplayName("Use manual angle")]
            public bool UseManualAngle { get; set; }

            [PropertyOrder(8)]
            [Category("Line Pair")]
            [DisplayName("Manual angle")]
            public double ManualAngleValue { get; set; }

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
                return VisionPipelineStepBuilder.FromLineGaugePair(
                    PipelineStepName,
                    string.IsNullOrWhiteSpace(ToolType) ? "LineDistance" : ToolType,
                    CreateLeftProperty(),
                    CreateRightProperty(),
                    inputLayer,
                    outputLayer,
                    Purpose);
            }

            public LineGaugeProperty CreateLeftProperty()
            {
                return CreateLineProperty(PipelineStepName + "_Left", LeftDirection);
            }

            public LineGaugeProperty CreateRightProperty()
            {
                return CreateLineProperty(PipelineStepName + "_Right", RightDirection);
            }

            private LineGaugeProperty CreateLineProperty(string name, PROJECTION_DIR direction)
            {
                return new LineGaugeProperty(name)
                {
                    PIXELPERMM = PixelPerMm,
                    USE_ROI = UseRoi,
                    CvROI = Roi,
                    USE_THRESHOLD = UseThreshold,
                    USE_BITWISENOT = UseBitwiseNot,
                    THRESHOLD_TYPES = ThresholdType,
                    THRESHOLD = Threshold,
                    USE_ADAPTIVE_THRESHOLD = UseAdaptiveThreshold,
                    ADAPTIVE_THRESHOLD = AdaptiveThreshold,
                    PRJ_PORALITY = Polarity,
                    PRJ_DIR = direction,
                    CONTRAST = Contrast,
                    THICKNESS = Thickness,
                    SAMPLING_STEP = SamplingStep,
                    VER_PRJ_DIR = VerticalProjectionDirection,
                    POINT_RANGE = PointRange,
                    USE_MANUAL_ANGLE = UseManualAngle,
                    MANUAL_ANGLE_VALUE = ManualAngleValue,
                    USE_EXTEND_FIT_LINE = UseExtendFitLine,
                    EXTEND_FIT_LINE_VALUE = ExtendFitLineValue,
                    USE_AVERAGE_FILTER = UseAverageFilter,
                    AVERAGE_Diff = AverageDiff,
                    AVERAGE_FILTER_TYPE = AverageFilterType,
                    SHOW_VERTICAL_LINE = ShowVerticalLine,
                    SHOW_EDGE = ShowEdge,
                    SHOW_CONTOUR = ShowContour,
                    SHOW_FITLINE = ShowFitLine
                };
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
            [DisplayName("Maximum Angle Delta")]
            [Description("Fail instead of translating the ROI when the angle change exceeds this degree limit.")]
            public double FIXTURE_MAX_ANGLE_DELTA { get; set; } = 2D;

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
                parameters[VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter] = Convert.ToString(FIXTURE_MAX_ANGLE_DELTA, CultureInfo.InvariantCulture);
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
        [CategoryOrder("Transform", 0)]
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

