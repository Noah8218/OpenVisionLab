using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static Lib.Common.FormulaUtil;

namespace OpenVisionLab
{
    internal static class VisionPipelineAppToolFactory
    {
        public static IVisionTool Create(VisionPipelineStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            string toolType = NormalizeToolType(step.ToolType);

            switch (toolType)
            {
                case "blob":
                    return CreateBlobTool(step.Parameters);
                case "contour":
                    return CreateContourTool(step.Parameters);
                case "line":
                case "linegauge":
                    return CreateLineGaugeTool(step.Parameters);
                case "linedistance":
                case "linedistancegauge":
                    return CreateLineDistanceTool(step.Parameters);
                case "pinarraygap":
                case "adjacentpingap":
                    return new VisionPipelinePinArrayGapTool(
                        GetString(step.Parameters, "Name", "PipelinePinArrayGap"),
                        step.Parameters);
                case "curvebandprofile":
                case "darkbandcurve":
                    return new VisionPipelineCurveBandProfileTool(
                        GetString(step.Parameters, "Name", "PipelineCurveBandProfile"),
                        step.Parameters);
                case "outercornerintersection":
                case "brightobjectcorner":
                    return new VisionPipelineOuterCornerIntersectionTool(
                        GetString(step.Parameters, "Name", "PipelineOuterCornerIntersection"),
                        step.Parameters);
                case "lineintersection":
                case "lineintersectiongauge":
                    return CreateLineIntersectionTool(step.Parameters);
                case "circlegauge":
                    return new VisionPipelineCircleGaugeTool(step);
                case "matching":
                case "templatematching":
                    return CreateMatchingTool(step.Parameters);
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return CreateEdgeBasedMatchingTool(step.Parameters);
                case "mean":
                    return CreateMeanTool(step.Parameters);
                case "hsv":
                case "hsvmask":
                case "colorhsv":
                case "colormask":
                    return CreateHsvMaskTool(step.Parameters);
                case "edge":
                case "edgedetection":
                    return CreateEdgeDetectionTool(step.Parameters);
                case "rotatescale":
                case "rotateandscale":
                    return CreateRotateScaleTool(step.Parameters);
                case "feature":
                case "featurematching":
                case "sift":
                    return CreateFeatureMatchingTool(step.Parameters);
                case "referencedifference":
                    return CreateReferenceDifferenceTool(step.Parameters);
                default:
                    return VisionPipelineToolFactory.Create(step);
            }
        }

        private static IVisionTool CreateBlobTool(IDictionary<string, string> parameters)
        {
            BlobProperty property = new BlobProperty(GetString(parameters, "Name", "PipelineBlob"))
            {
                MIN_AREA = GetInt(parameters, nameof(BlobProperty.MIN_AREA), 200),
                MAX_AREA = GetInt(parameters, nameof(BlobProperty.MAX_AREA), 1000000),
                MIN_WIDTH = GetInt(parameters, nameof(BlobProperty.MIN_WIDTH), 0),
                MAX_WIDTH = GetInt(parameters, nameof(BlobProperty.MAX_WIDTH), 1000000),
                MIN_HEIGHT = GetInt(parameters, nameof(BlobProperty.MIN_HEIGHT), 0),
                MAX_HEIGHT = GetInt(parameters, nameof(BlobProperty.MAX_HEIGHT), 1000000)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            BlobTool tool = new BlobTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateContourTool(IDictionary<string, string> parameters)
        {
            ContourProperty property = new ContourProperty(GetString(parameters, "Name", "PipelineContour"))
            {
                USE_APPROXPOLYDP = GetBool(parameters, nameof(ContourProperty.USE_APPROXPOLYDP), false),
                USE_DRAW_IMAGE = GetBool(parameters, nameof(ContourProperty.USE_DRAW_IMAGE), false),
                DrawMode = GetEnum(parameters, nameof(ContourProperty.DrawMode), ContourDrawMode.Outline),
                ApproximationModes = GetEnum(parameters, nameof(ContourProperty.ApproximationModes), ContourApproximationModes.ApproxSimple),
                DetectMode = GetEnum(parameters, nameof(ContourProperty.DetectMode), RetrievalModes.External),
                EPSILON = GetDouble(parameters, nameof(ContourProperty.EPSILON), 0.01),
                MIN_AREA = GetInt(parameters, nameof(ContourProperty.MIN_AREA), 200),
                MAX_AREA = GetInt(parameters, nameof(ContourProperty.MAX_AREA), 1000000),
                MIN_WIDTH = GetInt(parameters, nameof(ContourProperty.MIN_WIDTH), 0),
                MAX_WIDTH = GetInt(parameters, nameof(ContourProperty.MAX_WIDTH), 1000000),
                MIN_HEIGHT = GetInt(parameters, nameof(ContourProperty.MIN_HEIGHT), 0),
                MAX_HEIGHT = GetInt(parameters, nameof(ContourProperty.MAX_HEIGHT), 1000000),
                ClrGridHtml = GetString(parameters, nameof(ContourProperty.ClrGridHtml), "#ff0000"),
                DrawThickness = GetInt(parameters, nameof(ContourProperty.DrawThickness), 2)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            ContourTool tool = new ContourTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateLineGaugeTool(IDictionary<string, string> parameters)
        {
            LineGaugeProperty property = new LineGaugeProperty(GetString(parameters, "Name", "PipelineLineGauge"))
            {
                PRJ_PORALITY = GetEnum(parameters, nameof(LineGaugeProperty.PRJ_PORALITY), PROJECTION_POLARITY.BTOW),
                PRJ_DIR = GetEnum(parameters, nameof(LineGaugeProperty.PRJ_DIR), PROJECTION_DIR.X_LTOR),
                CONTRAST = GetDouble(parameters, nameof(LineGaugeProperty.CONTRAST), 30),
                THICKNESS = GetDouble(parameters, nameof(LineGaugeProperty.THICKNESS), 5),
                SAMPLING_STEP = GetDouble(parameters, nameof(LineGaugeProperty.SAMPLING_STEP), 10),
                VER_PRJ_DIR = GetEnum(parameters, nameof(LineGaugeProperty.VER_PRJ_DIR), PROJECTION_DIR.X_LTOR),
                POINT_RANGE = GetInt(parameters, nameof(LineGaugeProperty.POINT_RANGE), 10),
                USE_MANUAL_ANGLE = GetBool(parameters, nameof(LineGaugeProperty.USE_MANUAL_ANGLE), false),
                MANUAL_ANGLE_VALUE = GetDouble(parameters, nameof(LineGaugeProperty.MANUAL_ANGLE_VALUE), 0),
                USE_EXTEND_FIT_LINE = GetBool(parameters, nameof(LineGaugeProperty.USE_EXTEND_FIT_LINE), false),
                EXTEND_FIT_LINE_VALUE = GetInt(parameters, nameof(LineGaugeProperty.EXTEND_FIT_LINE_VALUE), 100),
                AVERAGE_Diff = GetDouble(parameters, nameof(LineGaugeProperty.AVERAGE_Diff), 100),
                USE_AVERAGE_FILTER = GetBool(parameters, nameof(LineGaugeProperty.USE_AVERAGE_FILTER), false),
                AVERAGE_FILTER_TYPE = GetEnum(parameters, nameof(LineGaugeProperty.AVERAGE_FILTER_TYPE), LineGaugeProperty.AVERAGE_FILTER_TYPES.Y),
                SHOW_VERTICAL_LINE = GetBool(parameters, nameof(LineGaugeProperty.SHOW_VERTICAL_LINE), true),
                SHOW_EDGE = GetBool(parameters, nameof(LineGaugeProperty.SHOW_EDGE), true),
                SHOW_CONTOUR = GetBool(parameters, nameof(LineGaugeProperty.SHOW_CONTOUR), true),
                SHOW_FITLINE = GetBool(parameters, nameof(LineGaugeProperty.SHOW_FITLINE), true)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            LineGaugeTool tool = new LineGaugeTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateLineDistanceTool(IDictionary<string, string> parameters)
        {
            string name = GetString(parameters, "Name", "PipelineLineDistance");
            LineGaugeProperty left = CreateLineDistanceGaugeProperty(
                parameters,
                "Left",
                name + "_Left",
                PROJECTION_DIR.X_LTOR);
            LineGaugeProperty right = CreateLineDistanceGaugeProperty(
                parameters,
                "Right",
                name + "_Right",
                PROJECTION_DIR.X_RTOL);

            return new VisionPipelineLineDistanceTool(name, left, right, parameters);
        }

        private static IVisionTool CreateLineIntersectionTool(IDictionary<string, string> parameters)
        {
            string name = GetString(parameters, "Name", "PipelineLineIntersection");
            LineGaugeProperty left = CreateLineDistanceGaugeProperty(
                parameters,
                "Left",
                name + "_Left",
                PROJECTION_DIR.X_LTOR);
            LineGaugeProperty right = CreateLineDistanceGaugeProperty(
                parameters,
                "Right",
                name + "_Right",
                PROJECTION_DIR.X_RTOL);

            return new VisionPipelineLineIntersectionTool(name, left, right);
        }

        private static LineGaugeProperty CreateLineDistanceGaugeProperty(
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

        private static IVisionTool CreateMatchingTool(IDictionary<string, string> parameters)
        {
            MatchingProperty property = new MatchingProperty(GetString(parameters, "Name", "PipelineMatching"))
            {
                MATCH_MODE = GetEnum(parameters, nameof(MatchingProperty.MATCH_MODE), TemplateMatchModes.CCoeffNormed),
                SCORE_MIN = GetDouble(parameters, nameof(MatchingProperty.SCORE_MIN), 0.6),
                MAGNIFIATION = GetDouble(parameters, nameof(MatchingProperty.MAGNIFIATION), 1),
                NUM_MATCH = GetInt(parameters, nameof(MatchingProperty.NUM_MATCH), 3),
                USE_FIND_ANGLE = GetBool(parameters, nameof(MatchingProperty.USE_FIND_ANGLE), true),
                FIND_ANGLE = GetDouble(parameters, nameof(MatchingProperty.FIND_ANGLE), 0.1),
                FIND_ANGLE_MAX = GetInt(parameters, nameof(MatchingProperty.FIND_ANGLE_MAX), 10),
                FIND_ANGLE_MIN = GetInt(parameters, nameof(MatchingProperty.FIND_ANGLE_MIN), -10),
                USE_COARSE_TO_FINE_ANGLE_SEARCH = GetBool(parameters, nameof(MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), false),
                COARSE_ANGLE_STEP = GetDouble(parameters, nameof(MatchingProperty.COARSE_ANGLE_STEP), 5.0),
                COARSE_ANGLE_TOP_K = GetInt(parameters, nameof(MatchingProperty.COARSE_ANGLE_TOP_K), 3),
                USE_FIND_SCALE = GetBool(parameters, nameof(MatchingProperty.USE_FIND_SCALE), false),
                FIND_SCALE_MIN = GetDouble(parameters, nameof(MatchingProperty.FIND_SCALE_MIN), 0.9),
                FIND_SCALE_MAX = GetDouble(parameters, nameof(MatchingProperty.FIND_SCALE_MAX), 1.1),
                FIND_SCALE_STEP = GetDouble(parameters, nameof(MatchingProperty.FIND_SCALE_STEP), 0.05),
                PATTERN_PATH = GetString(parameters, nameof(MatchingProperty.PATTERN_PATH), string.Empty),
                USE_CANNY = GetBool(parameters, nameof(MatchingProperty.USE_CANNY), false),
                CANNY_HIGH = GetInt(parameters, nameof(MatchingProperty.CANNY_HIGH), 60),
                CANNY_LOW = GetInt(parameters, nameof(MatchingProperty.CANNY_LOW), 30),
                USE_PADDING_COLOR_WHITE = GetBool(parameters, nameof(MatchingProperty.USE_PADDING_COLOR_WHITE), false)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            string templatePath = ResolveTemplatePath(GetString(parameters, "TemplatePath", property.PATTERN_PATH));
            if (!string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath))
            {
                property.ImageTemplate = Cv2.ImRead(templatePath);
            }

            MatchingTool tool = new MatchingTool();
            tool.SetProperty(property);
            if (!Lib.OpenCV.OpenCvHelper.IsImageEmpty(property.ImageTemplate))
            {
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return tool;
        }

        private static IVisionTool CreateEdgeBasedMatchingTool(IDictionary<string, string> parameters)
        {
            EdgeBasedMatchingProperty property = new EdgeBasedMatchingProperty(GetString(parameters, "Name", "PipelineEdgeBasedMatching"))
            {
                SCORE_MIN = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.SCORE_MIN), 0.75),
                NUM_MATCH = GetInt(parameters, nameof(EdgeBasedMatchingProperty.NUM_MATCH), 1),
                USE_UNIQUE_MATCH_VALIDATION = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION), false),
                UNIQUE_MATCH_MIN_SCORE_MARGIN = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.UNIQUE_MATCH_MIN_SCORE_MARGIN), 0.03),
                ALLOW_GLOBAL_POLARITY_REVERSAL = GetBool(parameters, nameof(EdgeBasedMatchingProperty.ALLOW_GLOBAL_POLARITY_REVERSAL), false),
                PATTERN_PATH = GetString(parameters, nameof(EdgeBasedMatchingProperty.PATTERN_PATH), GetString(parameters, "TemplatePath", string.Empty)),
                USE_FIND_ANGLE = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_FIND_ANGLE), false),
                FIND_ANGLE = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE), 1.0),
                FIND_ANGLE_MAX = GetInt(parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MAX), 10),
                FIND_ANGLE_MIN = GetInt(parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MIN), -10),
                USE_COARSE_TO_FINE_ANGLE_SEARCH = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), false),
                COARSE_ANGLE_STEP = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_STEP), 5.0),
                COARSE_ANGLE_TOP_K = GetInt(parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_TOP_K), 3),
                USE_FIND_SCALE = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_FIND_SCALE), false),
                FIND_SCALE_MIN = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_MIN), 0.9),
                FIND_SCALE_MAX = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_MAX), 1.1),
                FIND_SCALE_STEP = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_STEP), 0.05),
                CANNY_LOW = GetInt(parameters, nameof(EdgeBasedMatchingProperty.CANNY_LOW), 30),
                CANNY_HIGH = GetInt(parameters, nameof(EdgeBasedMatchingProperty.CANNY_HIGH), 90),
                CANNY_APERTURE_SIZE = GetInt(parameters, nameof(EdgeBasedMatchingProperty.CANNY_APERTURE_SIZE), 3),
                USE_L2_GRADIENT = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_L2_GRADIENT), true),
                CONTOUR_RETRIEVAL_MODE = GetEnum(parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_RETRIEVAL_MODE), RetrievalModes.External),
                CONTOUR_APPROXIMATION_MODE = GetEnum(parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_APPROXIMATION_MODE), ContourApproximationModes.ApproxNone),
                GREEDINESS = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.GREEDINESS), 0.9),
                SEARCH_STEP = GetInt(parameters, nameof(EdgeBasedMatchingProperty.SEARCH_STEP), 2),
                USE_POSITION_REFINE = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_POSITION_REFINE), false),
                USE_SUBPIXEL_REFINE = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_SUBPIXEL_REFINE), false),
                USE_PYRAMID_POSITION_PROPOSAL = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_PYRAMID_POSITION_PROPOSAL), false),
                PYRAMID_POSITION_TOP_N = GetInt(parameters, nameof(EdgeBasedMatchingProperty.PYRAMID_POSITION_TOP_N), 6),
                PYRAMID_POSITION_MIN_SCORE = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.PYRAMID_POSITION_MIN_SCORE), 0.70),
                USE_HYBRID_VERIFY = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_HYBRID_VERIFY), false),
                HYBRID_VERIFY_TOP_N = GetInt(parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_TOP_N), 5),
                HYBRID_VERIFY_IMAGE_WEIGHT = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_IMAGE_WEIGHT), 0.35),
                MAX_TEMPLATE_POINTS = GetInt(parameters, nameof(EdgeBasedMatchingProperty.MAX_TEMPLATE_POINTS), 300),
                MIN_GRADIENT_MAGNITUDE = GetDouble(parameters, nameof(EdgeBasedMatchingProperty.MIN_GRADIENT_MAGNITUDE), 1),
                USE_DRAW_IMAGE = GetBool(parameters, nameof(EdgeBasedMatchingProperty.USE_DRAW_IMAGE), true)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            EdgeBasedTemplateMatchingTool tool = new EdgeBasedTemplateMatchingTool();
            tool.SetProperty(property);

            string templatePath = ResolveTemplatePath(GetString(parameters, "TemplatePath", property.PATTERN_PATH));
            if (!string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath))
            {
                property.PATTERN_PATH = templatePath;
                property.ImageTemplate = Cv2.ImRead(templatePath);
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return tool;
        }

        private static IVisionTool CreateEdgeDetectionTool(IDictionary<string, string> parameters)
        {
            return new VisionPipelineEdgeDetectionTool(
                GetString(parameters, "Name", "PipelineEdgeDetection"),
                parameters);
        }

        private static IVisionTool CreateHsvMaskTool(IDictionary<string, string> parameters)
        {
            return new VisionPipelineHsvMaskTool(
                GetString(parameters, "Name", "PipelineHsvMask"),
                parameters);
        }

        private static IVisionTool CreateMeanTool(IDictionary<string, string> parameters)
        {
            MeanProperty property = new MeanProperty(GetString(parameters, "Name", "PipelineMean"))
            {
                MEAN_MAX = GetInt(parameters, nameof(MeanProperty.MEAN_MAX), 240),
                MEAN_MIN = GetInt(parameters, nameof(MeanProperty.MEAN_MIN), 100),
                MEAN_TYPES = GetEnum(parameters, nameof(MeanProperty.MEAN_TYPES), MeanType.Mean)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            MeanTool tool = new MeanTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateRotateScaleTool(IDictionary<string, string> parameters)
        {
            if (VisionPipelineFixtureFrameService.IsNormalizeImageParameters(parameters))
            {
                return new VisionPipelineNormalizeImageTool(
                    GetString(parameters, "Name", "NormalizeImage"),
                    parameters);
            }

            RotateScaleToolProperty property = new RotateScaleToolProperty
            {
                Angle = GetDouble(parameters, nameof(RotateScaleToolProperty.Angle), 0d),
                ScaleXPercent = GetDouble(parameters, nameof(RotateScaleToolProperty.ScaleXPercent), 100d),
                ScaleYPercent = GetDouble(parameters, nameof(RotateScaleToolProperty.ScaleYPercent), 100d),
                Interpolation = GetEnum(parameters, nameof(RotateScaleToolProperty.Interpolation), InterpolationFlags.Linear),
                BorderType = GetEnum(parameters, nameof(RotateScaleToolProperty.BorderType), BorderTypes.Constant)
            };

            RotateScaleTool tool = new RotateScaleTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateFeatureMatchingTool(IDictionary<string, string> parameters)
        {
            FeatureMatchingProperty property = new FeatureMatchingProperty(GetString(parameters, "Name", "PipelineFeatureMatching"))
            {
                SCORE_MIN = GetDouble(parameters, nameof(FeatureMatchingProperty.SCORE_MIN), 0.6),
                RANSAC_REPROJ_THRESHOLD = GetDouble(parameters, nameof(FeatureMatchingProperty.RANSAC_REPROJ_THRESHOLD), 3),
                PATTERN_PATH = GetString(parameters, nameof(FeatureMatchingProperty.PATTERN_PATH), string.Empty)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            SiftTool tool = new SiftTool();
            tool.SetProperty(property);

            string templatePath = ResolveTemplatePath(GetString(parameters, "TemplatePath", property.PATTERN_PATH));
            if (!string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath))
            {
                property.ImageTemplate = Cv2.ImRead(templatePath);
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return tool;
        }

        private static IVisionTool CreateReferenceDifferenceTool(IDictionary<string, string> parameters)
        {
            Dictionary<string, string> resolved = new Dictionary<string, string>(
                parameters ?? new Dictionary<string, string>(),
                StringComparer.OrdinalIgnoreCase);
            for (int index = 1; index <= 4; index++)
            {
                string key = "ReferencePath" + index.ToString(CultureInfo.InvariantCulture);
                string value = GetString(parameters, key, string.Empty);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    resolved[key] = ResolveTemplatePath(value);
                }
            }

            string legacyPaths = GetString(parameters, "ReferencePaths", string.Empty);
            if (!string.IsNullOrWhiteSpace(legacyPaths))
            {
                resolved["ReferencePaths"] = string.Join(
                    ";",
                    legacyPaths
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(ResolveTemplatePath));
            }

            return new VisionPipelineReferenceDifferenceTool(
                GetString(parameters, "Name", "PipelineReferenceDifference"),
                resolved);
        }

        internal static string ResolveTemplatePath(string value)
        {
            string candidate = (value ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(candidate))
            {
                return Path.GetFullPath(candidate);
            }

            string resolvedPath =
                AppPathService.ResolveExistingDataOrInstallationPath(candidate);
            if (File.Exists(resolvedPath))
            {
                return resolvedPath;
            }

            return Path.GetFullPath(candidate);
        }

        private static void ApplyCommonOpenCvProperty(OpenVisionLab.Vision._1._Tools.OpenCV.OpenCvPropertyBase property, IDictionary<string, string> parameters)
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

            return value
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => GetRect(new Dictionary<string, string> { [key] = part }, key, default))
                .ToList();
        }
    }
}
