using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class VisionPipelineStepParameterSchema
    {
        private static readonly Dictionary<string, Type> ParameterTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["Mode"] = typeof(ThresholdToolMode),
            ["ThresholdType"] = typeof(ThresholdTypes),
            ["AdaptiveThresholdType"] = typeof(ThresholdTypes),
            ["THRESHOLD_TYPES"] = typeof(ThresholdTypes),
            ["ADAPTIVE_THRESHOLD_TYPES"] = typeof(ThresholdTypes),
            ["AdaptiveType"] = typeof(AdaptiveThresholdTypes),
            ["ADAPTIVE_THRESHOLD_ALGORITHM"] = typeof(AdaptiveThresholdTypes),
            ["Shape"] = typeof(MorphShapes),
            ["Operator"] = typeof(MorphTypes),
            ["FilterType"] = typeof(FilterToolType),
            ["BorderType"] = typeof(BorderTypes),
            ["Interpolation"] = typeof(InterpolationFlags),
            ["EdgeType"] = typeof(EdgeDetectionToolType),
            ["DrawMode"] = typeof(ContourDrawMode),
            ["ApproximationModes"] = typeof(ContourApproximationModes),
            ["DetectMode"] = typeof(RetrievalModes),
            ["PRJ_PORALITY"] = typeof(FormulaUtil.PROJECTION_POLARITY),
            ["PRJ_POLARITY"] = typeof(FormulaUtil.PROJECTION_POLARITY),
            ["PRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
            ["VER_PRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
            ["LeftPRJ_PORALITY"] = typeof(FormulaUtil.PROJECTION_POLARITY),
            ["LeftPRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
            ["LeftVER_PRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
            ["RightPRJ_PORALITY"] = typeof(FormulaUtil.PROJECTION_POLARITY),
            ["RightPRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
            ["RightVER_PRJ_DIR"] = typeof(FormulaUtil.PROJECTION_DIR),
            ["AVERAGE_FILTER_TYPE"] = typeof(LineGaugeProperty.AVERAGE_FILTER_TYPES),
            ["MATCH_MODE"] = typeof(TemplateMatchModes),
            ["CONTOUR_RETRIEVAL_MODE"] = typeof(RetrievalModes),
            ["CONTOUR_APPROXIMATION_MODE"] = typeof(ContourApproximationModes),
            ["MEAN_TYPES"] = typeof(MeanType),
            ["MeasurementMode"] = typeof(PinArrayGapMeasurementMode),
            ["EDGE_POLARITY"] = typeof(CircleGaugeEdgePolarity),
            [VisionPipelineOverlayMergeService.RenderPresetParameter] = typeof(VisionPipelineOverlayRenderPreset),
            [VisionPipelineOverlayMergeService.LabelModeParameter] = typeof(VisionPipelineOverlayLabelMode),
            [VisionPipelineFixtureFrameService.ApplyModeParameter] = typeof(VisionPipelineFixtureApplyMode)
        };

        private static readonly HashSet<string> IntegerParameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "MIN_AREA",
            "MAX_AREA",
            "MIN_WIDTH",
            "MAX_WIDTH",
            "MIN_HEIGHT",
            "MAX_HEIGHT",
            "DrawThickness",
            "POINT_RANGE",
            "LeftPOINT_RANGE",
            "RightPOINT_RANGE",
            "EXTEND_FIT_LINE_VALUE",
            "LeftEXTEND_FIT_LINE_VALUE",
            "RightEXTEND_FIT_LINE_VALUE",
            "NUM_MATCH",
            "FIND_ANGLE_MAX",
            "FIND_ANGLE_MIN",
            "COARSE_ANGLE_TOP_K",
            "CANNY_HIGH",
            "CANNY_LOW",
            "CANNY_APERTURE_SIZE",
            "SEARCH_STEP",
            "HYBRID_VERIFY_TOP_N",
            "MAX_TEMPLATE_POINTS",
            "MEAN_MAX",
            "MEAN_MIN",
            "RangeMin",
            "RangeMax",
            "BlockSize",
            "Weight",
            "KernelWidth",
            "KernelHeight",
            "Iterations",
            "MedianKernelSize",
            "Diameter",
            "SigmaColor",
            "SigmaSpace",
            "CannyThresholdLow",
            "CannyThresholdHigh",
            "CannyApertureSize",
            "SobelDegreeX",
            "SobelDegreeY",
            "SobelKernelSize",
            "ScharrDegreeX",
            "ScharrDegreeY",
            "LaplacianKernelSize",
            "MaxPoints",
            VisionPipelineOverlayMergeService.LineWidthParameter,
            VisionPipelineOverlayMergeService.PointSizeParameter,
            VisionPipelineOverlayMergeService.LabelMarginParameter,
            "HueMin",
            "HueMax",
            "SaturationMin",
            "SaturationMax",
            "ValueMin",
            "ValueMax",
            VisionPipelineArithmeticStep.ParameterGray,
            VisionPipelineArithmeticStep.ParameterB,
            VisionPipelineArithmeticStep.ParameterG,
            VisionPipelineArithmeticStep.ParameterR,
            VisionPipelineArithmeticStep.ParameterOffsetX,
            VisionPipelineArithmeticStep.ParameterOffsetY,
            "DifferenceThreshold",
            "MinimumDefectArea",
            "MaximumDefectArea",
            "MorphologyKernel",
            "IgnoreBorder",
            "OrbFeatures",
            "MinimumInliers",
            "DarkThreshold",
            "ForegroundThreshold",
            "MinPinWidth",
            "MaxPinBreakWidth",
            "MinGapWidth",
            "MinComponentArea",
            "MinComponentHeight",
            "SCAN_COUNT",
            VisionPipelineFixtureFrameService.ReferenceImageWidthParameter,
            VisionPipelineFixtureFrameService.ReferenceImageHeightParameter,
            VisionPipelineLineFixtureService.MinimumSupportAParameter,
            VisionPipelineLineFixtureService.MinimumSupportBParameter,
            VisionPipelineMultiMatchMeanService.ReferenceImageWidthParameter,
            VisionPipelineMultiMatchMeanService.ReferenceImageHeightParameter,
            VisionPipelineMultiMatchMeanService.MinimumInstancesParameter,
            VisionPipelineMultiMatchMeanService.MaximumInstancesParameter,
            VisionPipelineMultiMatchMeanService.MinimumPassCountParameter,
            nameof(AffineTransformToolProperty.OutputWidth),
            nameof(AffineTransformToolProperty.OutputHeight)
        };

        private static readonly HashSet<string> DoubleParameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "PIXELPERMM",
            "THRESHOLD",
            "ADAPTIVE_THRESHOLD",
            "EPSILON",
            "CONTRAST",
            "THICKNESS",
            "SAMPLING_STEP",
            "MANUAL_ANGLE_VALUE",
            "LeftPIXELPERMM",
            "LeftCONTRAST",
            "LeftTHICKNESS",
            "LeftSAMPLING_STEP",
            "LeftMANUAL_ANGLE_VALUE",
            "RightPIXELPERMM",
            "RightCONTRAST",
            "RightTHICKNESS",
            "RightSAMPLING_STEP",
            "RightMANUAL_ANGLE_VALUE",
            "SCORE_MIN",
            "UNIQUE_MATCH_MIN_SCORE_MARGIN",
            "MAGNIFIATION",
            "FIND_ANGLE",
            "COARSE_ANGLE_STEP",
            "FIND_SCALE_MIN",
            "FIND_SCALE_MAX",
            "FIND_SCALE_STEP",
            VisionPipelineFixtureFrameService.ReferenceXParameter,
            VisionPipelineFixtureFrameService.ReferenceYParameter,
            VisionPipelineFixtureFrameService.ReferenceAngleParameter,
            VisionPipelineFixtureFrameService.ReferenceScaleParameter,
            VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter,
            VisionPipelineFixtureFrameService.MinimumScaleRatioParameter,
            VisionPipelineFixtureFrameService.MaximumScaleRatioParameter,
            VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter,
            VisionPipelineFixtureFrameService.RuntimeReferenceXParameter,
            VisionPipelineFixtureFrameService.RuntimeReferenceYParameter,
            VisionPipelineFixtureFrameService.RuntimeCurrentXParameter,
            VisionPipelineFixtureFrameService.RuntimeCurrentYParameter,
            VisionPipelineFixtureFrameService.RuntimeAngleDeltaParameter,
            VisionPipelineFixtureFrameService.RuntimeScaleRatioParameter,
            "RANSAC_REPROJ_THRESHOLD",
            "GREEDINESS",
            "HYBRID_VERIFY_IMAGE_WEIGHT",
            "MIN_GRADIENT_MAGNITUDE",
            "AVERAGE_Diff",
            "Threshold",
            "MaxValue",
            "Angle",
            "ScaleXPercent",
            "ScaleYPercent",
            "MatchRatio",
            "RansacThreshold",
            "MinDarkCoverageRatio",
            "MinComponentHeightRatio",
            "EdgeFitEndPercent",
            VisionPipelineGapEdgePairTool.MinimumGapParameter,
            VisionPipelineGapEdgePairTool.MaximumGapParameter,
            VisionPipelineGapEdgePairTool.MaximumAngleParameter,
            VisionPipelineGapEdgePairTool.MaximumParallelDeltaParameter,
            VisionPipelineGapEdgePairTool.MinimumSupportRatioParameter,
            VisionPipelineGapEdgePairTool.MinimumDarkContrastParameter,
            VisionPipelineGapEdgePairTool.MinimumDarkCoverageParameter,
            VisionPipelineGapEdgePairTool.MinimumScoreMarginParameter,
            "CENTER_X",
            "CENTER_Y",
            "RADIUS_MIN",
            "RADIUS_MAX",
            "START_ANGLE_DEG",
            "SWEEP_ANGLE_DEG",
            "MIN_CONTRAST",
            "MIN_SUPPORT_RATIO",
            "MAX_FIT_RESIDUAL_PX",
            VisionPipelineGeometryMeasureService.MaximumParallelAngleDeltaParameter,
            VisionPipelineGeometryMeasureService.MaximumExtensionAParameter,
            VisionPipelineGeometryMeasureService.MaximumExtensionBParameter,
            VisionPipelineLineFixtureService.MaximumFitResidualAParameter,
            VisionPipelineLineFixtureService.MaximumFitResidualBParameter,
            VisionPipelineLineFixtureService.MinimumIncludedAngleParameter,
            VisionPipelineLineFixtureService.MaximumIncludedAngleParameter,
            VisionPipelineMultiMatchMeanService.ReferenceXParameter,
            VisionPipelineMultiMatchMeanService.ReferenceYParameter,
            VisionPipelineMultiMatchMeanService.ReferenceAngleParameter,
            VisionPipelineMultiMatchMeanService.ReferenceScaleParameter,
            VisionPipelineMultiMatchMeanService.RowToleranceParameter,
            VisionPipelineMultiMatchMeanService.MaximumOverlapParameter,
            VisionPipelineMultiMatchMeanService.MinimumMeanParameter,
            VisionPipelineMultiMatchMeanService.MaximumMeanParameter,
            VisionPipelineMultiMatchMeanService.MaximumAngleDeltaParameter,
            VisionPipelineMultiMatchMeanService.MinimumScaleRatioParameter,
            VisionPipelineMultiMatchMeanService.MaximumScaleRatioParameter,
            VisionPipelineMultiMatchMeanService.MinimumValidPixelRatioParameter,
            nameof(AffineTransformToolProperty.SourcePoint1X),
            nameof(AffineTransformToolProperty.SourcePoint1Y),
            nameof(AffineTransformToolProperty.SourcePoint2X),
            nameof(AffineTransformToolProperty.SourcePoint2Y),
            nameof(AffineTransformToolProperty.SourcePoint3X),
            nameof(AffineTransformToolProperty.SourcePoint3Y),
            nameof(AffineTransformToolProperty.DestinationPoint1X),
            nameof(AffineTransformToolProperty.DestinationPoint1Y),
            nameof(AffineTransformToolProperty.DestinationPoint2X),
            nameof(AffineTransformToolProperty.DestinationPoint2Y),
            nameof(AffineTransformToolProperty.DestinationPoint3X),
            nameof(AffineTransformToolProperty.DestinationPoint3Y),
            nameof(AffineTransformToolProperty.BorderValue),
            nameof(AffineTransformToolProperty.MinimumSourceTriangleArea),
            nameof(AffineTransformToolProperty.MinimumDestinationTriangleArea),
            nameof(AffineTransformToolProperty.MinimumValidPixelRatio)
        };

        public static bool TryGetParameterType(string key, out Type type)
        {
            if (ParameterTypes.TryGetValue(key ?? string.Empty, out type))
            {
                return true;
            }

            if (IntegerParameters.Contains(key ?? string.Empty))
            {
                type = typeof(int);
                return true;
            }

            if (DoubleParameters.Contains(key ?? string.Empty))
            {
                type = typeof(double);
                return true;
            }

            if (IsBooleanParameter(key))
            {
                type = typeof(bool);
                return true;
            }

            type = typeof(string);
            return false;
        }

        public static bool TryValidateValue(string key, string value, out string message)
        {
            message = string.Empty;
            if (string.Equals(key, "MeasurementMode", StringComparison.OrdinalIgnoreCase))
            {
                string candidate = value ?? string.Empty;
                if (Enum.TryParse(candidate, true, out PinArrayGapMeasurementMode _)
                    || Enum.TryParse(candidate, true, out GeometryMeasurementMode _))
                {
                    return true;
                }

                message = $"'{key}' expects a PinArrayGap or GeometryMeasure mode.";
                return false;
            }

            if (!TryGetParameterType(key, out Type type))
            {
                return true;
            }

            string text = value ?? string.Empty;
            if (type == typeof(string))
            {
                return true;
            }

            if (type == typeof(bool))
            {
                if (bool.TryParse(text, out _))
                {
                    return true;
                }

                message = $"'{key}' expects True or False.";
                return false;
            }

            if (type == typeof(int))
            {
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                {
                    return true;
                }

                message = $"'{key}' expects an integer value.";
                return false;
            }

            if (type == typeof(double))
            {
                if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                {
                    return true;
                }

                message = $"'{key}' expects a numeric value.";
                return false;
            }

            if (type.IsEnum)
            {
                try
                {
                    Enum.Parse(type, text, ignoreCase: true);
                    return true;
                }
                catch
                {
                    message = $"'{key}' expects one of {string.Join(", ", Enum.GetNames(type))}.";
                    return false;
                }
            }

            return true;
        }

        private static bool IsBooleanParameter(string key)
        {
            return !string.IsNullOrWhiteSpace(key)
                && (key.StartsWith("USE_", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("SHOW_", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("LeftUSE_", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("LeftSHOW_", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("RightUSE_", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("RightSHOW_", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "Invert", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "BurnIn", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "DrawLabels", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, VisionPipelineOverlayMergeService.LabelBackgroundParameter, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "AllowEmpty", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "IgnoreLeftBorderTouching", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, VisionPipelineArithmeticStep.ParameterUseConstantInput, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, VisionPipelineArithmeticStep.ParameterUseColorConstant, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "UseL2Gradient", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, VisionPipelineMultiMatchMeanService.RequireAllParameter, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, VisionPipelineGeometryMeasureService.RequireResultInImageParameter, StringComparison.OrdinalIgnoreCase));
        }
    }
}
