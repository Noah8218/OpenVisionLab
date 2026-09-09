using System;
using System.Collections.Generic;
using OpenVisionLab;

namespace OpenVisionLab.Core.Integration;

/// <summary>
/// Keeps the Integration metric unit contract explicit. Metric names are
/// matched as complete names; a suffix such as "Mm" or "Ratio" is never
/// interpreted heuristically.
/// </summary>
internal static class TwoDIntegrationMetricUnits
{
    internal const string Millimeter = "mm";
    internal const string Pixel = "px";
    internal const string PixelArea = "px²";
    internal const string Degree = "deg";
    internal const string Millisecond = "ms";
    internal const string Fraction = "fraction";
    internal const string Count = "count";
    internal const string Score = "score";
    internal const string Unitless = "unitless";
    internal const string Unknown = "unknown";

    private static readonly IReadOnlyDictionary<string, string> Units =
        CreateUnits();

    internal static string Resolve(string metricName) =>
        metricName is not null && Units.TryGetValue(metricName, out string unit)
            ? unit
            : Unknown;

    internal static bool IsKnown(string metricName) =>
        metricName is not null && Units.ContainsKey(metricName);

    private static IReadOnlyDictionary<string, string> CreateUnits()
    {
        var units = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        Add(units, Millimeter,
            VisionPipelineKnownMetrics.LineLengthMmMin,
            VisionPipelineKnownMetrics.LineLengthMmMax,
            VisionPipelineKnownMetrics.LineLengthMmAvg,
            VisionPipelineKnownMetrics.DistanceMmMin,
            VisionPipelineKnownMetrics.DistanceMmMax,
            VisionPipelineKnownMetrics.DistanceMmAvg,
            VisionPipelineKnownMetrics.DistanceMmRange,
            VisionPipelineKnownMetrics.CurveOuterArcLengthMm,
            VisionPipelineKnownMetrics.CurveInnerArcLengthMm,
            VisionPipelineKnownMetrics.CurveCenterArcLengthMm,
            VisionPipelineKnownMetrics.BoundsWidthMmMin,
            VisionPipelineKnownMetrics.BoundsWidthMmMax,
            VisionPipelineKnownMetrics.BoundsWidthMmAvg,
            VisionPipelineKnownMetrics.BoundsHeightMmMin,
            VisionPipelineKnownMetrics.BoundsHeightMmMax,
            VisionPipelineKnownMetrics.BoundsHeightMmAvg,
            VisionPipelineKnownMetrics.GeometryDistanceMm,
            VisionPipelineKnownMetrics.GeometrySignedClearanceMm,
            VisionPipelineKnownMetrics.CircleRadiusMm,
            VisionPipelineKnownMetrics.CircleDiameterMm);

        Add(units, Pixel,
            VisionPipelineKnownMetrics.IntersectionX,
            VisionPipelineKnownMetrics.IntersectionY,
            VisionPipelineKnownMetrics.LineLengthMin,
            VisionPipelineKnownMetrics.LineLengthMax,
            VisionPipelineKnownMetrics.LineLengthAvg,
            VisionPipelineKnownMetrics.DistancePxMin,
            VisionPipelineKnownMetrics.DistancePxMax,
            VisionPipelineKnownMetrics.DistancePxAvg,
            VisionPipelineKnownMetrics.DistancePxRange,
            VisionPipelineKnownMetrics.PitchPxMin,
            VisionPipelineKnownMetrics.PitchPxMax,
            VisionPipelineKnownMetrics.PitchPxAvg,
            VisionPipelineKnownMetrics.PitchPxRange,
            VisionPipelineKnownMetrics.CurveOuterArcLengthPx,
            VisionPipelineKnownMetrics.CurveInnerArcLengthPx,
            VisionPipelineKnownMetrics.CurveCenterArcLengthPx,
            VisionPipelineKnownMetrics.BoundsWidthMin,
            VisionPipelineKnownMetrics.BoundsWidthMax,
            VisionPipelineKnownMetrics.BoundsWidthAvg,
            VisionPipelineKnownMetrics.BoundsHeightMin,
            VisionPipelineKnownMetrics.BoundsHeightMax,
            VisionPipelineKnownMetrics.BoundsHeightAvg,
            VisionPipelineKnownMetrics.SourceImageWidth,
            VisionPipelineKnownMetrics.SourceImageHeight,
            VisionPipelineKnownMetrics.ResultImageWidth,
            VisionPipelineKnownMetrics.ResultImageHeight,
            VisionPipelineKnownMetrics.FixtureCenterX,
            VisionPipelineKnownMetrics.FixtureCenterY,
            VisionPipelineKnownMetrics.FixtureOffsetX,
            VisionPipelineKnownMetrics.FixtureOffsetY,
            VisionPipelineKnownMetrics.FixtureReferenceImageWidth,
            VisionPipelineKnownMetrics.FixtureReferenceImageHeight,
            VisionPipelineKnownMetrics.FixtureNormalizedImageWidth,
            VisionPipelineKnownMetrics.FixtureNormalizedImageHeight,
            VisionPipelineKnownMetrics.FixtureAppliedCenterX,
            VisionPipelineKnownMetrics.FixtureAppliedCenterY,
            VisionPipelineKnownMetrics.FixtureEffectiveRoiX,
            VisionPipelineKnownMetrics.FixtureEffectiveRoiY,
            VisionPipelineKnownMetrics.FixtureLineAFitResidualPx,
            VisionPipelineKnownMetrics.FixtureLineBFitResidualPx,
            VisionPipelineKnownMetrics.AffineM13,
            VisionPipelineKnownMetrics.AffineM23,
            VisionPipelineKnownMetrics.AffineTranslationX,
            VisionPipelineKnownMetrics.AffineTranslationY,
            VisionPipelineKnownMetrics.AffineSourcePoint1X,
            VisionPipelineKnownMetrics.AffineSourcePoint1Y,
            VisionPipelineKnownMetrics.AffineSourcePoint2X,
            VisionPipelineKnownMetrics.AffineSourcePoint2Y,
            VisionPipelineKnownMetrics.AffineSourcePoint3X,
            VisionPipelineKnownMetrics.AffineSourcePoint3Y,
            VisionPipelineKnownMetrics.GeometryDistancePx,
            VisionPipelineKnownMetrics.GeometrySignedClearancePx,
            VisionPipelineKnownMetrics.GeometryExtensionAPx,
            VisionPipelineKnownMetrics.GeometryExtensionBPx,
            VisionPipelineKnownMetrics.CircleCenterX,
            VisionPipelineKnownMetrics.CircleCenterY,
            VisionPipelineKnownMetrics.CircleRadiusPx,
            VisionPipelineKnownMetrics.CircleDiameterPx,
            VisionPipelineKnownMetrics.CircleFitResidualPx);

        Add(units, PixelArea,
            VisionPipelineKnownMetrics.AreaMin,
            VisionPipelineKnownMetrics.AreaMax,
            VisionPipelineKnownMetrics.AreaAvg,
            VisionPipelineKnownMetrics.AffineSourceTriangleArea,
            VisionPipelineKnownMetrics.AffineDestinationTriangleArea);

        Add(units, Degree,
            VisionPipelineKnownMetrics.AngleMin,
            VisionPipelineKnownMetrics.AngleMax,
            VisionPipelineKnownMetrics.AngleAvg,
            VisionPipelineKnownMetrics.LineAngleMin,
            VisionPipelineKnownMetrics.LineAngleMax,
            VisionPipelineKnownMetrics.LineAngleAvg,
            VisionPipelineKnownMetrics.FixtureAngle,
            VisionPipelineKnownMetrics.FixtureAngleDelta,
            VisionPipelineKnownMetrics.FixtureAppliedAngle,
            VisionPipelineKnownMetrics.FixtureIncludedAngleDeg,
            VisionPipelineKnownMetrics.GapSelectedAngleDeltaDeg,
            VisionPipelineKnownMetrics.AffineRotationDeg,
            VisionPipelineKnownMetrics.GeometryAngleDeg,
            VisionPipelineKnownMetrics.GeometryParallelDeltaDeg,
            VisionPipelineKnownMetrics.CircleCoverageDeg);

        Add(units, Fraction,
            VisionPipelineKnownMetrics.MaskPixelRatio,
            VisionPipelineKnownMetrics.UniqueMatchScoreMargin,
            VisionPipelineKnownMetrics.GapBestDarkCoverageRatio,
            VisionPipelineKnownMetrics.GapSelectedSupportRatio,
            VisionPipelineKnownMetrics.GapDarkCoverageRatio,
            VisionPipelineKnownMetrics.FixtureValidPixelRatio,
            VisionPipelineKnownMetrics.AffineValidPixelRatio,
            VisionPipelineKnownMetrics.DifferencePixelRatio,
            VisionPipelineKnownMetrics.RegistrationInlierRatio,
            VisionPipelineKnownMetrics.ValidPixelRatio,
            VisionPipelineKnownMetrics.CircleSupportRatio,
            VisionPipelineMultiMatchMeanService.InstanceValidPixelRatioMinMetric);

        Add(units, Score,
            VisionPipelineKnownMetrics.ScoreMin,
            VisionPipelineKnownMetrics.ScoreMax,
            VisionPipelineKnownMetrics.ScoreAvg,
            VisionPipelineKnownMetrics.ScoreMargin,
            VisionPipelineKnownMetrics.GapScoreMargin,
            VisionPipelineKnownMetrics.RegistrationScore,
            VisionPipelineMultiMatchMeanService.InstanceScoreMinMetric,
            VisionPipelineMultiMatchMeanService.InstanceScoreMaxMetric);

        Add(units, Count,
            VisionPipelineKnownMetrics.ResultCount,
            VisionPipelineKnownMetrics.MaskPixelCount,
            VisionPipelineKnownMetrics.EdgeCount,
            VisionPipelineKnownMetrics.EdgePointCount,
            VisionPipelineKnownMetrics.DistanceCount,
            VisionPipelineKnownMetrics.PitchCount,
            VisionPipelineKnownMetrics.GapCandidateLineCount,
            VisionPipelineKnownMetrics.GapCandidatePairCount,
            VisionPipelineKnownMetrics.GapOverlapPairCount,
            VisionPipelineKnownMetrics.GapSeparationPairCount,
            VisionPipelineKnownMetrics.GapParallelPairCount,
            VisionPipelineKnownMetrics.GapContrastPairCount,
            VisionPipelineKnownMetrics.GapUpperSupportPointCount,
            VisionPipelineKnownMetrics.GapLowerSupportPointCount,
            VisionPipelineKnownMetrics.CurveProfileRowCount,
            VisionPipelineKnownMetrics.MergeOverlayCount,
            VisionPipelineKnownMetrics.MergeSourceCount,
            VisionPipelineKnownMetrics.SourceImageChannels,
            VisionPipelineKnownMetrics.ResultImageChannels,
            VisionPipelineKnownMetrics.FixtureLineASupportCount,
            VisionPipelineKnownMetrics.FixtureLineBSupportCount,
            VisionPipelineKnownMetrics.AffineDetectedSourcePointCount,
            VisionPipelineKnownMetrics.DifferencePixelCount,
            VisionPipelineKnownMetrics.RegistrationInliers,
            VisionPipelineKnownMetrics.CircleSupportCount,
            VisionPipelineKnownMetrics.UniqueMatchPlausibleAlternativeCount,
            VisionPipelineMultiMatchMeanService.InstanceCountMetric,
            VisionPipelineMultiMatchMeanService.InstancePassCountMetric,
            VisionPipelineMultiMatchMeanService.InstanceFailCountMetric);

        Add(units, Unitless,
            VisionPipelineKnownMetrics.UniqueMatchState,
            VisionPipelineKnownMetrics.MeanValueMin,
            VisionPipelineKnownMetrics.MeanValueMax,
            VisionPipelineKnownMetrics.MeanValueAvg,
            VisionPipelineKnownMetrics.CornerOuterContourVerified,
            VisionPipelineKnownMetrics.GapBestDarkContrast,
            VisionPipelineKnownMetrics.GapDarkContrast,
            VisionPipelineKnownMetrics.GapBandMeanGray,
            VisionPipelineKnownMetrics.FixtureScale,
            VisionPipelineKnownMetrics.FixtureScaleRatio,
            VisionPipelineKnownMetrics.FixtureAppliedScaleRatio,
            VisionPipelineKnownMetrics.AffineM11,
            VisionPipelineKnownMetrics.AffineM12,
            VisionPipelineKnownMetrics.AffineM21,
            VisionPipelineKnownMetrics.AffineM22,
            VisionPipelineKnownMetrics.AffineDeterminant,
            VisionPipelineKnownMetrics.AffineScaleX,
            VisionPipelineKnownMetrics.AffineScaleY,
            VisionPipelineKnownMetrics.AffineShearCosine,
            VisionPipelineKnownMetrics.DifferenceMean,
            VisionPipelineKnownMetrics.ReferenceIndex,
            VisionPipelineMultiMatchMeanService.InstanceAggregatePassedMetric,
            VisionPipelineMultiMatchMeanService.InstanceMeanMinMetric,
            VisionPipelineMultiMatchMeanService.InstanceMeanMaxMetric,
            VisionPipelineMultiMatchMeanService.InstanceMeanAvgMetric);

        return units;
    }

    private static void Add(
        IDictionary<string, string> units,
        string unit,
        params string[] metricNames)
    {
        foreach (string metricName in metricNames)
        {
            if (!units.TryAdd(metricName, unit))
            {
                throw new InvalidOperationException(
                    $"The Integration metric unit map contains a duplicate: {metricName}.");
            }
        }
    }
}
