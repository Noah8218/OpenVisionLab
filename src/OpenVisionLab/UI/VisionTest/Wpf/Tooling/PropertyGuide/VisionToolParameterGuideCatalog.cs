using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionToolParameterGuideCatalog
    {
        private sealed class Definition
        {
            public Definition(
                string keyBase,
                string[] relatedPropertyNames = null,
                string activeWhenPropertyName = null,
                string[] activeWhenValues = null)
            {
                KeyBase = keyBase ?? string.Empty;
                RelatedPropertyNames = relatedPropertyNames ?? Array.Empty<string>();
                ActiveWhenPropertyName = activeWhenPropertyName ?? string.Empty;
                ActiveWhenValues = activeWhenValues ?? Array.Empty<string>();
            }

            public string KeyBase { get; }
            public string[] RelatedPropertyNames { get; }
            public string ActiveWhenPropertyName { get; }
            public string[] ActiveWhenValues { get; }
        }

        private static readonly IReadOnlyDictionary<string, Definition> CommonDefinitions =
            new Dictionary<string, Definition>(StringComparer.Ordinal)
            {
                ["NAME"] = Guide("Common.Name"),
                ["PIXELPERMM"] = Guide("Common.PixelScale"),
                ["USE_THRESHOLD"] = Guide("Common.UseThreshold", "THRESHOLD_TYPES", "THRESHOLD"),
                ["THRESHOLD_TYPES"] = GuideWhen("Common.ThresholdType", "USE_THRESHOLD", "USE_THRESHOLD", "THRESHOLD"),
                ["THRESHOLD"] = GuideWhen("Common.ThresholdValue", "USE_THRESHOLD", "USE_THRESHOLD", "THRESHOLD_TYPES"),
                ["USE_BITWISENOT"] = Guide("Common.InvertResult", "USE_THRESHOLD", "USE_ADAPTIVE_THRESHOLD"),
                ["USE_ADAPTIVE_THRESHOLD"] = Guide(
                    "Common.UseAdaptiveThreshold",
                    "ADAPTIVE_THRESHOLD_ALGORITHM",
                    "BlockSize",
                    "Weight"),
                ["ADAPTIVE_THRESHOLD"] = GuideWhen(
                    "Common.AdaptiveThresholdValue",
                    "USE_ADAPTIVE_THRESHOLD",
                    "USE_ADAPTIVE_THRESHOLD",
                    "BlockSize",
                    "Weight"),
                ["ADAPTIVE_THRESHOLD_TYPES"] = GuideWhen(
                    "Common.AdaptiveResultType",
                    "USE_ADAPTIVE_THRESHOLD",
                    "USE_ADAPTIVE_THRESHOLD"),
                ["ADAPTIVE_THRESHOLD_ALGORITHM"] = GuideWhen(
                    "Common.AdaptiveAlgorithm",
                    "USE_ADAPTIVE_THRESHOLD",
                    "USE_ADAPTIVE_THRESHOLD",
                    "BlockSize",
                    "Weight"),
                ["BlockSize"] = GuideWhen(
                    "Common.AdaptiveBlockSize",
                    "USE_ADAPTIVE_THRESHOLD",
                    "USE_ADAPTIVE_THRESHOLD",
                    "Weight"),
                ["Weight"] = GuideWhen(
                    "Common.AdaptiveWeight",
                    "USE_ADAPTIVE_THRESHOLD",
                    "USE_ADAPTIVE_THRESHOLD",
                    "BlockSize"),
                ["USE_ROI"] = Guide("Common.UseRoi", "CvROI", "USE_MULTI_ROI"),
                ["CvROI"] = GuideWhen("Common.Roi", "USE_ROI", "USE_ROI"),
                ["USE_MULTI_ROI"] = GuideWhen("Common.UseMultiRoi", "USE_ROI", "USE_ROI", "CvROIS"),
                ["CvROIS"] = GuideWhen("Common.MultiRoi", "USE_MULTI_ROI", "USE_MULTI_ROI"),
                ["USE_MASKING"] = Guide("Common.UseMasking", "CvMASKS"),
                ["CvMASKS"] = GuideWhen("Common.MaskingRegions", "USE_MASKING", "USE_MASKING")
            };

        private static readonly IReadOnlyDictionary<string, Definition> Definitions =
            new Dictionary<string, Definition>(StringComparer.Ordinal)
            {
                ["Blob.MIN_AREA"] = Guide("Object.MinArea", "MAX_AREA"),
                ["Blob.MAX_AREA"] = Guide("Object.MaxArea", "MIN_AREA"),
                ["Blob.MIN_WIDTH"] = Guide("Object.MinWidth", "MAX_WIDTH"),
                ["Blob.MAX_WIDTH"] = Guide("Object.MaxWidth", "MIN_WIDTH"),
                ["Blob.MIN_HEIGHT"] = Guide("Object.MinHeight", "MAX_HEIGHT"),
                ["Blob.MAX_HEIGHT"] = Guide("Object.MaxHeight", "MIN_HEIGHT"),

                ["Contour.USE_APPROXPOLYDP"] = Guide("Contour.UseApproxPoly", "EPSILON"),
                ["Contour.DrawMode"] = Guide("Contour.DrawMode", "DrawColor", "DrawThickness"),
                ["Contour.ApproximationModes"] = Guide("Contour.ApproximationMode", "DetectMode"),
                ["Contour.DetectMode"] = Guide("Contour.RetrievalMode", "ApproximationModes"),
                ["Contour.EPSILON"] = GuideWhen(
                    "Contour.Epsilon",
                    "USE_APPROXPOLYDP",
                    "USE_APPROXPOLYDP"),
                ["Contour.MIN_AREA"] = Guide("Object.MinArea", "MAX_AREA"),
                ["Contour.MAX_AREA"] = Guide("Object.MaxArea", "MIN_AREA"),
                ["Contour.MIN_WIDTH"] = Guide("Object.MinWidth", "MAX_WIDTH"),
                ["Contour.MAX_WIDTH"] = Guide("Object.MaxWidth", "MIN_WIDTH"),
                ["Contour.MIN_HEIGHT"] = Guide("Object.MinHeight", "MAX_HEIGHT"),
                ["Contour.MAX_HEIGHT"] = Guide("Object.MaxHeight", "MIN_HEIGHT"),
                ["Contour.DrawColor"] = Guide("Contour.DrawColor", "DrawMode"),
                ["Contour.DrawThickness"] = Guide("Contour.DrawThickness", "DrawMode"),

                ["Threshold.Mode"] = Guide("Threshold.Mode"),
                ["Threshold.Threshold"] = GuideWhenValue(
                    "Threshold.Value",
                    "Mode",
                    new[] { "Threshold" },
                    "Mode",
                    "ThresholdType",
                    "MaxValue"),
                ["Threshold.MaxValue"] = Guide("Threshold.MaxValue", "Mode"),
                ["Threshold.ThresholdType"] = GuideWhenValue(
                    "Threshold.ResultType",
                    "Mode",
                    new[] { "Threshold" },
                    "Mode",
                    "Threshold"),
                ["Threshold.RangeMin"] = GuideWhenValue(
                    "Threshold.RangeMin",
                    "Mode",
                    new[] { "Range" },
                    "Mode",
                    "RangeMax",
                    "Invert"),
                ["Threshold.RangeMax"] = GuideWhenValue(
                    "Threshold.RangeMax",
                    "Mode",
                    new[] { "Range" },
                    "Mode",
                    "RangeMin",
                    "Invert"),
                ["Threshold.Invert"] = GuideWhenValue(
                    "Threshold.RangeInvert",
                    "Mode",
                    new[] { "Range" },
                    "Mode",
                    "RangeMin",
                    "RangeMax"),
                ["Threshold.AdaptiveType"] = GuideWhenValue(
                    "Threshold.AdaptiveMethod",
                    "Mode",
                    new[] { "Adaptive" },
                    "Mode",
                    "BlockSize",
                    "Weight"),
                ["Threshold.AdaptiveThresholdType"] = GuideWhenValue(
                    "Threshold.AdaptiveResultType",
                    "Mode",
                    new[] { "Adaptive" },
                    "Mode"),
                ["Threshold.BlockSize"] = GuideWhenValue(
                    "Threshold.BlockSize",
                    "Mode",
                    new[] { "Adaptive" },
                    "Mode",
                    "Weight"),
                ["Threshold.Weight"] = GuideWhenValue(
                    "Threshold.Weight",
                    "Mode",
                    new[] { "Adaptive" },
                    "Mode",
                    "BlockSize"),

                ["Morphology.Operator"] = Guide("Morphology.Operator", "Shape", "KernelWidth", "Iterations"),
                ["Morphology.Shape"] = Guide("Morphology.Shape", "Operator", "KernelWidth", "KernelHeight"),
                ["Morphology.KernelWidth"] = Guide("Morphology.KernelWidth", "KernelHeight", "Operator"),
                ["Morphology.KernelHeight"] = Guide("Morphology.KernelHeight", "KernelWidth", "Operator"),
                ["Morphology.Iterations"] = Guide("Morphology.Iterations", "Operator", "KernelWidth"),

                ["Filter.FilterType"] = Guide("Filter.Type"),
                ["Filter.KernelWidth"] = GuideWhenValue(
                    "Filter.KernelWidth",
                    "FilterType",
                    new[] { "Blur", "GaussianBlur", "BoxFilter" },
                    "FilterType",
                    "KernelHeight"),
                ["Filter.KernelHeight"] = GuideWhenValue(
                    "Filter.KernelHeight",
                    "FilterType",
                    new[] { "Blur", "GaussianBlur", "BoxFilter" },
                    "FilterType",
                    "KernelWidth"),
                ["Filter.MedianKernelSize"] = GuideWhenValue(
                    "Filter.MedianKernel",
                    "FilterType",
                    new[] { "MedianBlur" },
                    "FilterType"),
                ["Filter.Diameter"] = GuideWhenValue(
                    "Filter.Diameter",
                    "FilterType",
                    new[] { "BilateralFilter" },
                    "FilterType",
                    "SigmaColor",
                    "SigmaSpace"),
                ["Filter.SigmaColor"] = GuideWhenValue(
                    "Filter.SigmaColor",
                    "FilterType",
                    new[] { "BilateralFilter" },
                    "FilterType",
                    "Diameter",
                    "SigmaSpace"),
                ["Filter.SigmaSpace"] = GuideWhenValue(
                    "Filter.SigmaSpace",
                    "FilterType",
                    new[] { "BilateralFilter" },
                    "FilterType",
                    "Diameter",
                    "SigmaColor"),
                ["Filter.BorderType"] = Guide("Filter.BorderType", "FilterType"),

                ["EdgeDetection.EdgeType"] = Guide("EdgeDetection.Type"),
                ["EdgeDetection.CannyThresholdLow"] = GuideWhenValue(
                    "EdgeDetection.CannyLow",
                    "EdgeType",
                    new[] { "Canny" },
                    "EdgeType",
                    "CannyThresholdHigh",
                    "CannyApertureSize"),
                ["EdgeDetection.CannyThresholdHigh"] = GuideWhenValue(
                    "EdgeDetection.CannyHigh",
                    "EdgeType",
                    new[] { "Canny" },
                    "EdgeType",
                    "CannyThresholdLow",
                    "CannyApertureSize"),
                ["EdgeDetection.CannyApertureSize"] = GuideWhenValue(
                    "EdgeDetection.CannyAperture",
                    "EdgeType",
                    new[] { "Canny" },
                    "EdgeType",
                    "CannyThresholdLow",
                    "CannyThresholdHigh"),
                ["EdgeDetection.UseL2Gradient"] = GuideWhenValue(
                    "EdgeDetection.UseL2",
                    "EdgeType",
                    new[] { "Canny" },
                    "EdgeType"),
                ["EdgeDetection.SobelDegreeX"] = GuideWhenValue(
                    "EdgeDetection.SobelX",
                    "EdgeType",
                    new[] { "Sobel" },
                    "EdgeType",
                    "SobelDegreeY",
                    "SobelKernelSize"),
                ["EdgeDetection.SobelDegreeY"] = GuideWhenValue(
                    "EdgeDetection.SobelY",
                    "EdgeType",
                    new[] { "Sobel" },
                    "EdgeType",
                    "SobelDegreeX",
                    "SobelKernelSize"),
                ["EdgeDetection.SobelKernelSize"] = GuideWhenValue(
                    "EdgeDetection.SobelKernel",
                    "EdgeType",
                    new[] { "Sobel" },
                    "EdgeType",
                    "SobelDegreeX",
                    "SobelDegreeY"),
                ["EdgeDetection.ScharrDegreeX"] = GuideWhenValue(
                    "EdgeDetection.ScharrX",
                    "EdgeType",
                    new[] { "Scharr" },
                    "EdgeType",
                    "ScharrDegreeY"),
                ["EdgeDetection.ScharrDegreeY"] = GuideWhenValue(
                    "EdgeDetection.ScharrY",
                    "EdgeType",
                    new[] { "Scharr" },
                    "EdgeType",
                    "ScharrDegreeX"),
                ["EdgeDetection.LaplacianKernelSize"] = GuideWhenValue(
                    "EdgeDetection.LaplacianKernel",
                    "EdgeType",
                    new[] { "Laplacian" },
                    "EdgeType"),

                ["RotateScale.Angle"] = Guide(
                    "RotateScale.Angle",
                    "ScaleXPercent",
                    "ScaleYPercent",
                    "Interpolation",
                    "BorderType"),
                ["RotateScale.ScaleXPercent"] = Guide(
                    "RotateScale.ScaleX",
                    "ScaleYPercent",
                    "Interpolation"),
                ["RotateScale.ScaleYPercent"] = Guide(
                    "RotateScale.ScaleY",
                    "ScaleXPercent",
                    "Interpolation"),
                ["RotateScale.Interpolation"] = Guide(
                    "RotateScale.Interpolation",
                    "ScaleXPercent",
                    "ScaleYPercent"),
                ["RotateScale.BorderType"] = Guide(
                    "RotateScale.Border",
                    "Angle"),

                ["Mean.MEAN_TYPES"] = Guide(
                    "Mean.Type",
                    "MEAN_MIN",
                    "MEAN_MAX",
                    "CvROI",
                    "USE_ROI"),
                ["Mean.MEAN_MIN"] = Guide(
                    "Mean.Minimum",
                    "MEAN_TYPES",
                    "MEAN_MAX",
                    "CvROI",
                    "USE_ROI"),
                ["Mean.MEAN_MAX"] = Guide(
                    "Mean.Maximum",
                    "MEAN_TYPES",
                    "MEAN_MIN",
                    "CvROI",
                    "USE_ROI"),

                ["FeatureMatching.PATTERN_PATH"] = Guide(
                    "FeatureMatching.Pattern",
                    "SCORE_MIN",
                    "RANSAC_REPROJ_THRESHOLD",
                    "USE_ROI",
                    "CvROI"),
                ["FeatureMatching.SCORE_MIN"] = Guide(
                    "FeatureMatching.Ratio",
                    "PATTERN_PATH",
                    "RANSAC_REPROJ_THRESHOLD",
                    "USE_ROI",
                    "CvROI"),
                ["FeatureMatching.RANSAC_REPROJ_THRESHOLD"] = Guide(
                    "FeatureMatching.Ransac",
                    "SCORE_MIN",
                    "PATTERN_PATH",
                    "USE_ROI",
                    "CvROI"),

                ["Matching.PATTERN_PATH"] = Guide("Matching.Pattern", "SCORE_MIN", "NUM_MATCH"),
                ["Matching.AUTO_PREVIEW"] = Guide("Common.AutoPreview"),
                ["Matching.MATCH_MODE"] = Guide("Matching.Mode", "SCORE_MIN"),
                ["Matching.SCORE_MIN"] = Guide("Matching.ScoreMin", "NUM_MATCH", "MATCH_MODE"),
                ["Matching.NUM_MATCH"] = Guide("Matching.MatchCount", "SCORE_MIN"),
                ["Matching.MAGNIFIATION"] = Guide(
                    "Matching.Magnification",
                    "SCORE_MIN",
                    "USE_FIND_SCALE"),
                ["Matching.USE_FIND_ANGLE"] = Guide("Matching.UseAngle", "FIND_ANGLE_MIN", "FIND_ANGLE", "FIND_ANGLE_MAX"),
                ["Matching.FIND_ANGLE_MIN"] = GuideWhen("Matching.AngleRange", "USE_FIND_ANGLE", "USE_FIND_ANGLE", "FIND_ANGLE"),
                ["Matching.FIND_ANGLE_MAX"] = GuideWhen("Matching.AngleRange", "USE_FIND_ANGLE", "USE_FIND_ANGLE", "FIND_ANGLE"),
                ["Matching.FIND_ANGLE"] = GuideWhen("Matching.AngleStep", "USE_FIND_ANGLE", "FIND_ANGLE_MIN", "FIND_ANGLE_MAX"),
                ["Matching.USE_COARSE_TO_FINE_ANGLE_SEARCH"] = GuideWhen(
                    "Matching.UseCoarseAngle",
                    "USE_FIND_ANGLE",
                    "USE_FIND_ANGLE",
                    "FIND_ANGLE",
                    "COARSE_ANGLE_STEP",
                    "COARSE_ANGLE_TOP_K"),
                ["Matching.COARSE_ANGLE_STEP"] = GuideWhen(
                    "Matching.CoarseAngleStep",
                    "USE_COARSE_TO_FINE_ANGLE_SEARCH",
                    "USE_FIND_ANGLE",
                    "FIND_ANGLE",
                    "COARSE_ANGLE_TOP_K"),
                ["Matching.COARSE_ANGLE_TOP_K"] = GuideWhen(
                    "Matching.CoarseAngleTopK",
                    "USE_COARSE_TO_FINE_ANGLE_SEARCH",
                    "USE_FIND_ANGLE",
                    "FIND_ANGLE",
                    "COARSE_ANGLE_STEP"),
                ["Matching.USE_FIND_SCALE"] = Guide("Matching.UseScale", "FIND_SCALE_MIN", "FIND_SCALE_MAX", "FIND_SCALE_STEP"),
                ["Matching.FIND_SCALE_MIN"] = GuideWhen("Matching.ScaleRange", "USE_FIND_SCALE", "USE_FIND_SCALE", "FIND_SCALE_STEP"),
                ["Matching.FIND_SCALE_MAX"] = GuideWhen("Matching.ScaleRange", "USE_FIND_SCALE", "USE_FIND_SCALE", "FIND_SCALE_STEP"),
                ["Matching.FIND_SCALE_STEP"] = GuideWhen("Matching.ScaleStep", "USE_FIND_SCALE", "FIND_SCALE_MIN", "FIND_SCALE_MAX"),
                ["Matching.USE_PYRAMID_POSITION_PROPOSAL"] = Guide(
                    "Matching.UsePyramidPositionProposal",
                    "USE_FIND_ANGLE",
                    "USE_FIND_SCALE",
                    "PYRAMID_POSITION_TOP_N",
                    "PYRAMID_POSITION_MIN_SCORE"),
                ["Matching.PYRAMID_POSITION_TOP_N"] = GuideWhen(
                    "Matching.PyramidPositionTopN",
                    "USE_PYRAMID_POSITION_PROPOSAL",
                    "USE_FIND_ANGLE",
                    "USE_FIND_SCALE",
                    "PYRAMID_POSITION_MIN_SCORE"),
                ["Matching.PYRAMID_POSITION_MIN_SCORE"] = GuideWhen(
                    "Matching.PyramidPositionMinScore",
                    "USE_PYRAMID_POSITION_PROPOSAL",
                    "USE_FIND_ANGLE",
                    "USE_FIND_SCALE",
                    "PYRAMID_POSITION_TOP_N",
                    "SCORE_MIN"),
                ["Matching.USE_CANNY"] = Guide("Matching.UseCanny", "CANNY_LOW", "CANNY_HIGH"),
                ["Matching.CANNY_LOW"] = GuideWhen("Matching.CannyRange", "USE_CANNY", "USE_CANNY", "CANNY_HIGH"),
                ["Matching.CANNY_HIGH"] = GuideWhen("Matching.CannyRange", "USE_CANNY", "USE_CANNY", "CANNY_LOW"),
                ["Matching.USE_PADDING_COLOR_WHITE"] = GuideWhen(
                    "Matching.UseWhitePadding",
                    "USE_FIND_ANGLE",
                    "USE_FIND_ANGLE",
                    "FIND_ANGLE",
                    "PATTERN_PATH"),
                ["Matching.USE_ROI"] = Guide("Common.UseRoi", "CvROI"),
                ["Matching.CvROI"] = GuideWhen("Common.Roi", "USE_ROI", "USE_ROI"),
                ["Matching.PIXELPERMM"] = Guide("Common.PixelScale"),

                ["EdgeBasedMatching.ShowAdvancedSettings"] = Guide(
                    "EdgeMatching.ShowAdvancedSettings",
                    "PATTERN_PATH",
                    "SCORE_MIN"),
                ["EdgeBasedMatching.PATTERN_PATH"] = Guide("EdgeMatching.Pattern", "SCORE_MIN", "CANNY_LOW"),
                ["EdgeBasedMatching.SCORE_MIN"] = Guide("EdgeMatching.ScoreMin", "UNIQUE_MATCH_MIN_SCORE_MARGIN"),
                ["EdgeBasedMatching.NUM_MATCH"] = Guide("Matching.MatchCount", "SCORE_MIN"),
                ["EdgeBasedMatching.USE_UNIQUE_MATCH_VALIDATION"] = Guide(
                    "EdgeMatching.UseUnique",
                    "UNIQUE_MATCH_MIN_SCORE_MARGIN",
                    "NUM_MATCH"),
                ["EdgeBasedMatching.UNIQUE_MATCH_MIN_SCORE_MARGIN"] = GuideWhen(
                    "EdgeMatching.UniqueMargin",
                    "USE_UNIQUE_MATCH_VALIDATION",
                    "USE_UNIQUE_MATCH_VALIDATION",
                    "SCORE_MIN"),
                ["EdgeBasedMatching.ALLOW_GLOBAL_POLARITY_REVERSAL"] = Guide(
                    "EdgeMatching.PolarityReversal",
                    "SCORE_MIN"),
                ["EdgeBasedMatching.USE_DRAW_IMAGE"] = Guide(
                    "EdgeMatching.DrawResult",
                    "SCORE_MIN"),
                ["EdgeBasedMatching.AUTO_MPOINT_USE_ANALYSIS_ROI"] = Guide(
                    "EdgeMatching.AutoMPointUseRoi",
                    "AUTO_MPOINT_ANALYSIS_ROI"),
                ["EdgeBasedMatching.AUTO_MPOINT_ANALYSIS_ROI"] = GuideWhen(
                    "EdgeMatching.AutoMPointRoi",
                    "AUTO_MPOINT_USE_ANALYSIS_ROI",
                    "AUTO_MPOINT_USE_ANALYSIS_ROI"),
                ["EdgeBasedMatching.AUTO_MPOINT_PATTERN_WIDTH"] = Guide(
                    "EdgeMatching.AutoMPointPatternWidth",
                    "AUTO_MPOINT_PATTERN_HEIGHT",
                    "AUTO_MPOINT_STRIDE"),
                ["EdgeBasedMatching.AUTO_MPOINT_PATTERN_HEIGHT"] = Guide(
                    "EdgeMatching.AutoMPointPatternHeight",
                    "AUTO_MPOINT_PATTERN_WIDTH",
                    "AUTO_MPOINT_STRIDE"),
                ["EdgeBasedMatching.AUTO_MPOINT_STRIDE"] = Guide(
                    "EdgeMatching.AutoMPointStride",
                    "AUTO_MPOINT_PATTERN_WIDTH",
                    "AUTO_MPOINT_PATTERN_HEIGHT"),
                ["EdgeBasedMatching.AUTO_MPOINT_MAX_RESULTS"] = Guide(
                    "EdgeMatching.AutoMPointMaxResults",
                    "AUTO_MPOINT_MIN_FEATURE_QUALITY",
                    "AUTO_MPOINT_MIN_UNIQUENESS"),
                ["EdgeBasedMatching.AUTO_MPOINT_MIN_FEATURE_QUALITY"] = Guide(
                    "EdgeMatching.AutoMPointFeatureQuality",
                    "AUTO_MPOINT_MIN_UNIQUENESS"),
                ["EdgeBasedMatching.AUTO_MPOINT_MIN_UNIQUENESS"] = Guide(
                    "EdgeMatching.AutoMPointUniqueness",
                    "AUTO_MPOINT_MIN_FEATURE_QUALITY",
                    "UNIQUE_MATCH_MIN_SCORE_MARGIN"),
                ["EdgeBasedMatching.AUTO_MPOINT_MAX_POSITION_ERROR"] = Guide(
                    "EdgeMatching.AutoMPointPositionError",
                    "AUTO_MPOINT_MIN_UNIQUENESS"),
                ["EdgeBasedMatching.AUTO_MPOINT_MIN_REPRESENTATIVE_IMAGES"] = Guide(
                    "EdgeMatching.AutoMPointRepresentativeCount",
                    "AUTO_MPOINT_MIN_REPRESENTATIVE_SUCCESS_RATE"),
                ["EdgeBasedMatching.AUTO_MPOINT_MIN_REPRESENTATIVE_SUCCESS_RATE"] = Guide(
                    "EdgeMatching.AutoMPointRepresentativeRate",
                    "AUTO_MPOINT_MIN_REPRESENTATIVE_IMAGES"),
                ["EdgeBasedMatching.CANNY_LOW"] = Guide("EdgeMatching.CannyRange", "CANNY_HIGH", "MIN_GRADIENT_MAGNITUDE"),
                ["EdgeBasedMatching.CANNY_HIGH"] = Guide("EdgeMatching.CannyRange", "CANNY_LOW", "MIN_GRADIENT_MAGNITUDE"),
                ["EdgeBasedMatching.CANNY_APERTURE_SIZE"] = Guide("EdgeMatching.CannyAperture", "CANNY_LOW", "CANNY_HIGH"),
                ["EdgeBasedMatching.USE_L2_GRADIENT"] = Guide(
                    "EdgeMatching.UseL2Gradient",
                    "CANNY_APERTURE_SIZE"),
                ["EdgeBasedMatching.CONTOUR_RETRIEVAL_MODE"] = Guide(
                    "EdgeMatching.ContourRetrieval",
                    "CONTOUR_APPROXIMATION_MODE",
                    "MAX_TEMPLATE_POINTS"),
                ["EdgeBasedMatching.CONTOUR_APPROXIMATION_MODE"] = Guide(
                    "EdgeMatching.ContourApproximation",
                    "CONTOUR_RETRIEVAL_MODE",
                    "MAX_TEMPLATE_POINTS"),
                ["EdgeBasedMatching.MAX_TEMPLATE_POINTS"] = Guide("EdgeMatching.MaxTemplatePoints", "SEARCH_STEP"),
                ["EdgeBasedMatching.MIN_GRADIENT_MAGNITUDE"] = Guide("EdgeMatching.MinGradient", "CANNY_LOW", "CANNY_HIGH"),
                ["EdgeBasedMatching.USE_FIND_ANGLE"] = Guide("Matching.UseAngle", "FIND_ANGLE_MIN", "FIND_ANGLE", "FIND_ANGLE_MAX"),
                ["EdgeBasedMatching.FIND_ANGLE_MIN"] = GuideWhen("Matching.AngleRange", "USE_FIND_ANGLE", "USE_FIND_ANGLE", "FIND_ANGLE"),
                ["EdgeBasedMatching.FIND_ANGLE_MAX"] = GuideWhen("Matching.AngleRange", "USE_FIND_ANGLE", "USE_FIND_ANGLE", "FIND_ANGLE"),
                ["EdgeBasedMatching.FIND_ANGLE"] = GuideWhen("Matching.AngleStep", "USE_FIND_ANGLE", "FIND_ANGLE_MIN", "FIND_ANGLE_MAX"),
                ["EdgeBasedMatching.USE_COARSE_TO_FINE_ANGLE_SEARCH"] = GuideWhen(
                    "EdgeMatching.UseCoarseAngle",
                    "USE_FIND_ANGLE",
                    "FIND_ANGLE",
                    "COARSE_ANGLE_STEP",
                    "COARSE_ANGLE_TOP_K"),
                ["EdgeBasedMatching.COARSE_ANGLE_STEP"] = GuideWhen(
                    "EdgeMatching.CoarseAngleStep",
                    "USE_COARSE_TO_FINE_ANGLE_SEARCH",
                    "FIND_ANGLE",
                    "COARSE_ANGLE_TOP_K"),
                ["EdgeBasedMatching.COARSE_ANGLE_TOP_K"] = GuideWhen(
                    "EdgeMatching.CoarseAngleTopK",
                    "USE_COARSE_TO_FINE_ANGLE_SEARCH",
                    "COARSE_ANGLE_STEP",
                    "FIND_ANGLE"),
                ["EdgeBasedMatching.USE_FIND_SCALE"] = Guide("Matching.UseScale", "FIND_SCALE_MIN", "FIND_SCALE_MAX", "FIND_SCALE_STEP"),
                ["EdgeBasedMatching.FIND_SCALE_MIN"] = GuideWhen("Matching.ScaleRange", "USE_FIND_SCALE", "USE_FIND_SCALE", "FIND_SCALE_STEP"),
                ["EdgeBasedMatching.FIND_SCALE_MAX"] = GuideWhen("Matching.ScaleRange", "USE_FIND_SCALE", "USE_FIND_SCALE", "FIND_SCALE_STEP"),
                ["EdgeBasedMatching.FIND_SCALE_STEP"] = GuideWhen("Matching.ScaleStep", "USE_FIND_SCALE", "FIND_SCALE_MIN", "FIND_SCALE_MAX"),
                ["EdgeBasedMatching.SEARCH_STEP"] = Guide("EdgeMatching.SearchStep", "USE_POSITION_REFINE", "GREEDINESS"),
                ["EdgeBasedMatching.USE_POSITION_REFINE"] = Guide(
                    "EdgeMatching.PositionRefine",
                    "SEARCH_STEP",
                    "USE_SUBPIXEL_REFINE"),
                ["EdgeBasedMatching.USE_SUBPIXEL_REFINE"] = Guide(
                    "EdgeMatching.SubpixelRefine",
                    "USE_POSITION_REFINE",
                    "SEARCH_STEP"),
                ["EdgeBasedMatching.GREEDINESS"] = Guide("EdgeMatching.Greediness", "SEARCH_STEP", "SCORE_MIN"),
                ["EdgeBasedMatching.USE_PYRAMID_POSITION_PROPOSAL"] = Guide(
                    "EdgeMatching.UsePyramid",
                    "PYRAMID_POSITION_TOP_N",
                    "PYRAMID_POSITION_MIN_SCORE",
                    "USE_FIND_SCALE"),
                ["EdgeBasedMatching.PYRAMID_POSITION_TOP_N"] = GuideWhen(
                    "EdgeMatching.PyramidTopN",
                    "USE_PYRAMID_POSITION_PROPOSAL",
                    "PYRAMID_POSITION_MIN_SCORE"),
                ["EdgeBasedMatching.PYRAMID_POSITION_MIN_SCORE"] = GuideWhen(
                    "EdgeMatching.PyramidMinScore",
                    "USE_PYRAMID_POSITION_PROPOSAL",
                    "PYRAMID_POSITION_TOP_N",
                    "SCORE_MIN"),
                ["EdgeBasedMatching.USE_HYBRID_VERIFY"] = Guide(
                    "EdgeMatching.UseHybrid",
                    "HYBRID_VERIFY_TOP_N",
                    "HYBRID_VERIFY_IMAGE_WEIGHT"),
                ["EdgeBasedMatching.HYBRID_VERIFY_TOP_N"] = GuideWhen(
                    "EdgeMatching.HybridTopN",
                    "USE_HYBRID_VERIFY",
                    "HYBRID_VERIFY_IMAGE_WEIGHT"),
                ["EdgeBasedMatching.HYBRID_VERIFY_IMAGE_WEIGHT"] = GuideWhen(
                    "EdgeMatching.HybridImageWeight",
                    "USE_HYBRID_VERIFY",
                    "HYBRID_VERIFY_TOP_N",
                    "SCORE_MIN"),
                ["EdgeBasedMatching.USE_ROI"] = Guide("Common.UseRoi", "CvROI"),
                ["EdgeBasedMatching.CvROI"] = GuideWhen("Common.Roi", "USE_ROI", "USE_ROI"),
                ["EdgeBasedMatching.PIXELPERMM"] = Guide("Common.PixelScale"),

                ["LineGauge.PIXELPERMM"] = Guide("Common.PixelScale"),
                ["LineGauge.USE_ROI"] = Guide("Common.UseRoi", "CvROI"),
                ["LineGauge.CvROI"] = GuideWhen("Common.Roi", "USE_ROI", "USE_ROI"),
                ["LineGauge.PRJ_PORALITY"] = Guide("Line.Polarity", "PRJ_DIR", "CONTRAST"),
                ["LineGauge.PRJ_DIR"] = Guide("Line.ProjectionDirection", "PRJ_PORALITY", "CONTRAST"),
                ["LineGauge.CONTRAST"] = Guide("Line.Contrast", "PRJ_PORALITY", "THICKNESS"),
                ["LineGauge.THICKNESS"] = Guide("Line.Thickness", "CONTRAST", "SAMPLING_STEP"),
                ["LineGauge.SAMPLING_STEP"] = Guide("Line.SamplingStep", "CONTRAST", "POINT_RANGE"),
                ["LineGauge.VER_PRJ_DIR"] = Guide("Line.ScanDirection", "POINT_RANGE", "USE_MANUAL_ANGLE"),
                ["LineGauge.POINT_RANGE"] = Guide("Line.ScanInterval", "VER_PRJ_DIR", "SAMPLING_STEP"),
                ["LineGauge.USE_MANUAL_ANGLE"] = Guide("Line.UseManualAngle", "MANUAL_ANGLE_VALUE"),
                ["LineGauge.MANUAL_ANGLE_VALUE"] = GuideWhen("Line.ManualAngle", "USE_MANUAL_ANGLE", "USE_MANUAL_ANGLE"),
                ["LineGauge.USE_EXTEND_FIT_LINE"] = Guide("Line.ExtendFitLine", "EXTEND_FIT_LINE_VALUE"),
                ["LineGauge.EXTEND_FIT_LINE_VALUE"] = GuideWhen("Line.ExtendLength", "USE_EXTEND_FIT_LINE", "USE_EXTEND_FIT_LINE"),
                ["LineGauge.USE_AVERAGE_FILTER"] = Guide("Line.UseAverageFilter", "AVERAGE_Diff", "AVERAGE_FILTER_TYPE"),
                ["LineGauge.AVERAGE_Diff"] = GuideWhen("Line.AverageDifference", "USE_AVERAGE_FILTER", "AVERAGE_FILTER_TYPE"),
                ["LineGauge.AVERAGE_FILTER_TYPE"] = GuideWhen("Line.AverageFilterType", "USE_AVERAGE_FILTER", "AVERAGE_Diff"),
                ["LineGauge.SHOW_VERTICAL_LINE"] = Guide("Line.ShowScanLines", "POINT_RANGE", "VER_PRJ_DIR"),
                ["LineGauge.SHOW_EDGE"] = Guide("Line.ShowEdges"),
                ["LineGauge.SHOW_CONTOUR"] = Guide("Line.ShowContour"),
                ["LineGauge.SHOW_FITLINE"] = Guide("Line.ShowFitLine", "USE_EXTEND_FIT_LINE", "EXTEND_FIT_LINE_VALUE"),

                ["AffineTransform.SourcePoint1X"] = GuideWhenValue(
                    "Affine.SourcePointX",
                    "UseDetectedSourcePoints",
                    new[] { "False" },
                    "SourcePoint1Y",
                    "SourcePoint2X",
                    "SourcePoint2Y",
                    "SourcePoint3X",
                    "SourcePoint3Y",
                    "MinimumSourceTriangleArea"),
                ["AffineTransform.SourcePoint1Y"] = GuideWhenValue(
                    "Affine.SourcePointY",
                    "UseDetectedSourcePoints",
                    new[] { "False" },
                    "SourcePoint1X",
                    "SourcePoint2X",
                    "SourcePoint2Y",
                    "SourcePoint3X",
                    "SourcePoint3Y",
                    "MinimumSourceTriangleArea"),
                ["AffineTransform.SourcePoint2X"] = GuideWhenValue(
                    "Affine.SourcePointX",
                    "UseDetectedSourcePoints",
                    new[] { "False" },
                    "SourcePoint2Y",
                    "SourcePoint1X",
                    "SourcePoint1Y",
                    "SourcePoint3X",
                    "SourcePoint3Y",
                    "MinimumSourceTriangleArea"),
                ["AffineTransform.SourcePoint2Y"] = GuideWhenValue(
                    "Affine.SourcePointY",
                    "UseDetectedSourcePoints",
                    new[] { "False" },
                    "SourcePoint2X",
                    "SourcePoint1X",
                    "SourcePoint1Y",
                    "SourcePoint3X",
                    "SourcePoint3Y",
                    "MinimumSourceTriangleArea"),
                ["AffineTransform.SourcePoint3X"] = GuideWhenValue(
                    "Affine.SourcePointX",
                    "UseDetectedSourcePoints",
                    new[] { "False" },
                    "SourcePoint3Y",
                    "SourcePoint1X",
                    "SourcePoint1Y",
                    "SourcePoint2X",
                    "SourcePoint2Y",
                    "MinimumSourceTriangleArea"),
                ["AffineTransform.SourcePoint3Y"] = GuideWhenValue(
                    "Affine.SourcePointY",
                    "UseDetectedSourcePoints",
                    new[] { "False" },
                    "SourcePoint3X",
                    "SourcePoint1X",
                    "SourcePoint1Y",
                    "SourcePoint2X",
                    "SourcePoint2Y",
                    "MinimumSourceTriangleArea"),
                ["AffineTransform.DestinationPoint1X"] = Guide(
                    "Affine.DestinationPointX",
                    "DestinationPoint1Y",
                    "DestinationPoint2X",
                    "DestinationPoint2Y",
                    "DestinationPoint3X",
                    "DestinationPoint3Y",
                    "MinimumDestinationTriangleArea"),
                ["AffineTransform.DestinationPoint1Y"] = Guide(
                    "Affine.DestinationPointY",
                    "DestinationPoint1X",
                    "DestinationPoint2X",
                    "DestinationPoint2Y",
                    "DestinationPoint3X",
                    "DestinationPoint3Y",
                    "MinimumDestinationTriangleArea"),
                ["AffineTransform.DestinationPoint2X"] = Guide(
                    "Affine.DestinationPointX",
                    "DestinationPoint2Y",
                    "DestinationPoint1X",
                    "DestinationPoint1Y",
                    "DestinationPoint3X",
                    "DestinationPoint3Y",
                    "MinimumDestinationTriangleArea"),
                ["AffineTransform.DestinationPoint2Y"] = Guide(
                    "Affine.DestinationPointY",
                    "DestinationPoint2X",
                    "DestinationPoint1X",
                    "DestinationPoint1Y",
                    "DestinationPoint3X",
                    "DestinationPoint3Y",
                    "MinimumDestinationTriangleArea"),
                ["AffineTransform.DestinationPoint3X"] = Guide(
                    "Affine.DestinationPointX",
                    "DestinationPoint3Y",
                    "DestinationPoint1X",
                    "DestinationPoint1Y",
                    "DestinationPoint2X",
                    "DestinationPoint2Y",
                    "MinimumDestinationTriangleArea"),
                ["AffineTransform.DestinationPoint3Y"] = Guide(
                    "Affine.DestinationPointY",
                    "DestinationPoint3X",
                    "DestinationPoint1X",
                    "DestinationPoint1Y",
                    "DestinationPoint2X",
                    "DestinationPoint2Y",
                    "MinimumDestinationTriangleArea"),
                ["AffineTransform.ShowAdvancedSettings"] = Guide(
                    "Affine.ShowAdvancedSettings",
                    "OutputWidth",
                    "Interpolation",
                    "MinimumValidPixelRatio"),
                ["AffineTransform.OutputWidth"] = Guide(
                    "Affine.OutputWidth",
                    "OutputHeight",
                    "MinimumValidPixelRatio"),
                ["AffineTransform.OutputHeight"] = Guide(
                    "Affine.OutputHeight",
                    "OutputWidth",
                    "MinimumValidPixelRatio"),
                ["AffineTransform.Interpolation"] = Guide(
                    "Affine.Interpolation",
                    "BorderType",
                    "BorderValue"),
                ["AffineTransform.BorderType"] = Guide(
                    "Affine.BorderType",
                    "BorderValue",
                    "Interpolation"),
                ["AffineTransform.BorderValue"] = GuideWhenValue(
                    "Affine.BorderValue",
                    "BorderType",
                    new[] { "Constant" },
                    "BorderType",
                    "Interpolation"),
                ["AffineTransform.MinimumSourceTriangleArea"] = Guide(
                    "Affine.MinimumSourceArea",
                    "SourcePoint1X",
                    "SourcePoint1Y",
                    "SourcePoint2X",
                    "SourcePoint2Y",
                    "SourcePoint3X",
                    "SourcePoint3Y"),
                ["AffineTransform.MinimumDestinationTriangleArea"] = Guide(
                    "Affine.MinimumDestinationArea",
                    "DestinationPoint1X",
                    "DestinationPoint1Y",
                    "DestinationPoint2X",
                    "DestinationPoint2Y",
                    "DestinationPoint3X",
                    "DestinationPoint3Y"),
                ["AffineTransform.MinimumValidPixelRatio"] = Guide(
                    "Affine.MinimumValidPixelRatio",
                    "OutputWidth",
                    "OutputHeight",
                    "BorderType")
            };

        public static VisionToolParameterGuideContent Resolve(object selectedObject, string propertyName)
        {
            if (selectedObject == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(selectedObject)[propertyName];
            if (descriptor == null)
            {
                return null;
            }

            string family = ResolveFamily(selectedObject.GetType());
            Definitions.TryGetValue(family + "." + propertyName, out Definition definition);
            if (definition == null)
            {
                CommonDefinitions.TryGetValue(propertyName, out definition);
            }
            string displayName = ResolvePropertyText(
                selectedObject.GetType(),
                descriptor,
                "DisplayName",
                descriptor.DisplayName);
            string description = ResolvePropertyText(
                selectedObject.GetType(),
                descriptor,
                "Description",
                descriptor.Description);
            object value = descriptor.GetValue(selectedObject);
            string unit = ResolveUnit(propertyName);
            string identity = propertyName
                + " = "
                + FormatValue(value)
                + (string.IsNullOrWhiteSpace(unit) ? string.Empty : " " + unit);
            bool detailed = definition != null && HasGuideText(definition.KeyBase + ".Summary");
            string summary = detailed
                ? ResolveGuideField(
                    definition,
                    "Summary",
                    FirstNonEmpty(description, T("VisionTool.ParameterGuide.FallbackSummary")))
                : FirstNonEmpty(
                    description,
                    T("VisionTool.ParameterGuide.FallbackSummary"));
            string impact = detailed
                ? ResolveGuideField(
                    definition,
                    "Impact",
                    ResolveFallbackImpact(descriptor.PropertyType))
                : ResolveFallbackImpact(descriptor.PropertyType);
            string bestWhen = detailed
                ? ResolveGuideField(
                    definition,
                    "BestWhen",
                    T("VisionTool.ParameterGuide.FallbackBestWhen"))
                : T("VisionTool.ParameterGuide.FallbackBestWhen");
            string risk = detailed
                ? ResolveGuideField(
                    definition,
                    "Risk",
                    ResolveFallbackRisk(family))
                : ResolveFallbackRisk(family);
            string check = detailed
                ? ResolveGuideField(
                    definition,
                    "Check",
                    ResolveFallbackCheck(family))
                : ResolveFallbackCheck(family);
            string applicability = ResolveApplicability(selectedObject, definition);

            return new VisionToolParameterGuideContent
            {
                PropertyName = propertyName,
                Title = displayName,
                Identity = identity,
                Coverage = detailed
                    ? T("VisionTool.ParameterGuide.VerifiedCoverage")
                    : T("VisionTool.ParameterGuide.BasicCoverage"),
                Applicability = applicability,
                Summary = summary,
                Impact = impact,
                BestWhen = bestWhen,
                Risk = risk,
                CheckAfterPreview = check,
                RelatedPropertyNames = definition?.RelatedPropertyNames ?? Array.Empty<string>()
            };
        }

        private static Definition Guide(string keySuffix, params string[] relatedPropertyNames)
        {
            return new Definition(
                "VisionTool.ParameterGuide." + keySuffix,
                relatedPropertyNames);
        }

        private static Definition GuideWhen(
            string keySuffix,
            string activeWhenPropertyName,
            params string[] relatedPropertyNames)
        {
            return new Definition(
                "VisionTool.ParameterGuide." + keySuffix,
                relatedPropertyNames,
                activeWhenPropertyName);
        }

        private static Definition GuideWhenValue(
            string keySuffix,
            string activeWhenPropertyName,
            string[] activeWhenValues,
            params string[] relatedPropertyNames)
        {
            return new Definition(
                "VisionTool.ParameterGuide." + keySuffix,
                relatedPropertyNames,
                activeWhenPropertyName,
                activeWhenValues);
        }

        private static string ResolveFamily(Type type)
        {
            if (type != null && typeof(EdgeBasedMatchingProperty).IsAssignableFrom(type))
            {
                return "EdgeBasedMatching";
            }

            string name = type?.Name ?? string.Empty;
            return name switch
            {
                "MatchingProperty" => "Matching",
                "EdgeBasedMatchingProperty" => "EdgeBasedMatching",
                "LineGaugeProperty" => "LineGauge",
                "PipelineAffineTransformToolProperty" => "AffineTransform",
                "ThresholdToolProperty" => "Threshold",
                "MorphologyToolProperty" => "Morphology",
                "FilterToolProperty" => "Filter",
                "EdgeDetectionToolProperty" => "EdgeDetection",
                "RotateScaleToolProperty" => "RotateScale",
                _ => name.EndsWith("Property", StringComparison.Ordinal)
                    ? name.Substring(0, name.Length - "Property".Length)
                    : name
            };
        }

        private static string ResolvePropertyText(
            Type type,
            PropertyDescriptor descriptor,
            string field,
            string fallback)
        {
            string typeKey = "PropertyGrid.Type."
                + (type?.Name ?? string.Empty)
                + "."
                + descriptor.Name
                + "."
                + field;
            string value = T(typeKey);
            if (!string.Equals(value, typeKey, StringComparison.Ordinal))
            {
                return value;
            }

            string sharedKey = "PropertyGrid.Property." + descriptor.Name + "." + field;
            value = T(sharedKey);
            return string.Equals(value, sharedKey, StringComparison.Ordinal)
                ? fallback ?? descriptor.Name
                : value;
        }

        private static string ResolveApplicability(object selectedObject, Definition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.ActiveWhenPropertyName))
            {
                return string.Empty;
            }

            PropertyDescriptor activeDescriptor =
                TypeDescriptor.GetProperties(selectedObject)[definition.ActiveWhenPropertyName];
            if (activeDescriptor == null)
            {
                return string.Empty;
            }
            object activeValue = activeDescriptor?.GetValue(selectedObject);
            bool active = definition.ActiveWhenValues.Length > 0
                ? definition.ActiveWhenValues.Any(expected => string.Equals(
                    Convert.ToString(activeValue, CultureInfo.InvariantCulture),
                    expected,
                    StringComparison.OrdinalIgnoreCase))
                : activeValue is bool value && value;
            if (active)
            {
                return string.Empty;
            }

            string activePropertyName = activeDescriptor == null
                ? definition.ActiveWhenPropertyName
                : ResolvePropertyText(
                    selectedObject.GetType(),
                    activeDescriptor,
                    "DisplayName",
                    definition.ActiveWhenPropertyName);
            if (definition.ActiveWhenValues.Length > 0)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    T("VisionTool.ParameterGuide.InactiveValueFormat"),
                    activePropertyName,
                    string.Join(" / ", definition.ActiveWhenValues));
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.ParameterGuide.InactiveFormat"),
                activePropertyName);
        }

        private static string ResolveUnit(string propertyName)
        {
            if (string.Equals(propertyName, "PIXELPERMM", StringComparison.Ordinal))
            {
                return "mm/px";
            }

            if (string.Equals(
                propertyName,
                "RANSAC_REPROJ_THRESHOLD",
                StringComparison.Ordinal))
            {
                return "px";
            }

            if (propertyName.IndexOf("AREA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "px\u00B2";
            }

            if (propertyName.IndexOf("ANGLE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "deg";
            }

            if (propertyName.EndsWith("Percent", StringComparison.OrdinalIgnoreCase))
            {
                return "%";
            }

            if (propertyName.IndexOf("SCORE", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("GREEDINESS", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("RATIO", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "0..1";
            }

            if (propertyName.IndexOf("ROI", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("STEP", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("WIDTH", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("HEIGHT", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("KERNEL", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(propertyName, "BlockSize", StringComparison.Ordinal)
                || string.Equals(propertyName, "Diameter", StringComparison.Ordinal)
                || string.Equals(propertyName, "SigmaSpace", StringComparison.Ordinal))
            {
                return "px";
            }

            if (propertyName.IndexOf("CANNY", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("THRESHOLD", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(propertyName, "MEAN_MIN", StringComparison.Ordinal)
                || string.Equals(propertyName, "MEAN_MAX", StringComparison.Ordinal)
                || string.Equals(propertyName, "MaxValue", StringComparison.Ordinal)
                || string.Equals(propertyName, "RangeMin", StringComparison.Ordinal)
                || string.Equals(propertyName, "RangeMax", StringComparison.Ordinal)
                || string.Equals(propertyName, "Weight", StringComparison.Ordinal)
                || string.Equals(propertyName, "SigmaColor", StringComparison.Ordinal))
            {
                return "GV";
            }

            if (string.Equals(propertyName, "CONTRAST", StringComparison.Ordinal))
            {
                return "\u0394GV";
            }

            return string.Empty;
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return "-";
            }

            if (value is bool boolean)
            {
                return boolean
                    ? T("VisionTool.ParameterGuide.ValueOn")
                    : T("VisionTool.ParameterGuide.ValueOff");
            }

            if (value is double floating)
            {
                return floating.ToString("0.###", CultureInfo.CurrentCulture);
            }

            if (value is float single)
            {
                return single.ToString("0.###", CultureInfo.CurrentCulture);
            }

            if (value is System.Collections.ICollection collection)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    T("VisionTool.ParameterGuide.ItemCountFormat"),
                    collection.Count);
            }

            return Convert.ToString(value, CultureInfo.CurrentCulture) ?? "-";
        }

        private static string ResolveFallbackImpact(Type propertyType)
        {
            if (propertyType == typeof(bool))
            {
                return T("VisionTool.ParameterGuide.FallbackBooleanImpact");
            }

            if (propertyType?.IsEnum == true)
            {
                return T("VisionTool.ParameterGuide.FallbackEnumImpact");
            }

            return T("VisionTool.ParameterGuide.FallbackValueImpact");
        }

        private static string ResolveFallbackRisk(string family)
        {
            return family switch
            {
                "Matching" or "EdgeBasedMatching" =>
                    T("VisionTool.ParameterGuide.FallbackMatchingRisk"),
                "LineGauge" =>
                    T("VisionTool.ParameterGuide.FallbackLineRisk"),
                _ =>
                    T("VisionTool.ParameterGuide.FallbackRisk")
            };
        }

        private static string ResolveFallbackCheck(string family)
        {
            return family switch
            {
                "Matching" =>
                    T("VisionTool.ParameterGuide.FallbackMatchingCheck"),
                "EdgeBasedMatching" =>
                    T("VisionTool.ParameterGuide.FallbackEdgeMatchingCheck"),
                "LineGauge" =>
                    T("VisionTool.ParameterGuide.FallbackLineCheck"),
                _ =>
                    T("VisionTool.ParameterGuide.FallbackCheck")
            };
        }

        private static bool HasGuideText(string key)
        {
            string value = T(key);
            return !string.IsNullOrWhiteSpace(value)
                && !string.Equals(value, key, StringComparison.Ordinal);
        }

        private static string ResolveGuideField(
            Definition definition,
            string suffix,
            string fallback)
        {
            if (definition == null)
            {
                return fallback ?? string.Empty;
            }

            string key = definition.KeyBase + "." + suffix;
            string value = T(key);
            return string.IsNullOrWhiteSpace(value)
                || string.Equals(value, key, StringComparison.Ordinal)
                    ? fallback ?? string.Empty
                    : value;
        }

        private static string T(string key)
        {
            return OpenVisionLanguageService.T(key);
        }

        private static string FirstNonEmpty(params string[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }
    }
}
