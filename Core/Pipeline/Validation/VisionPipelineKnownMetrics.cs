using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineMetricDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    internal sealed class VisionPipelineAcceptancePreset
    {
        public string Name { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
        public string[] ToolTypes { get; set; } = Array.Empty<string>();
        public bool UseMinimum { get; set; }
        public double Minimum { get; set; }
        public bool UseMaximum { get; set; }
        public double Maximum { get; set; }
        public double MaxElapsedMilliseconds { get; set; }
    }

    internal static class VisionPipelineKnownMetrics
    {
        public const string ResultCount = "ResultCount";
        public const string AreaMin = "AreaMin";
        public const string AreaMax = "AreaMax";
        public const string AreaAvg = "AreaAvg";
        public const string ScoreMin = "ScoreMin";
        public const string ScoreMax = "ScoreMax";
        public const string ScoreAvg = "ScoreAvg";
        public const string ScoreMargin = "ScoreMargin";
        public const string UniqueMatchState = "UniqueMatch.State";
        public const string UniqueMatchPlausibleAlternativeCount = "UniqueMatch.PlausibleAlternativeCount";
        public const string UniqueMatchScoreMargin = "UniqueMatch.ScoreMargin";
        public const string AngleMin = "AngleMin";
        public const string AngleMax = "AngleMax";
        public const string AngleAvg = "AngleAvg";
        public const string MeanValueMin = "MeanValueMin";
        public const string MeanValueMax = "MeanValueMax";
        public const string MeanValueAvg = "MeanValueAvg";
        public const string MaskPixelCount = "MaskPixelCount";
        public const string MaskPixelRatio = "MaskPixelRatio";
        public const string EdgeCount = "EdgeCount";
        public const string EdgePointCount = "EdgePointCount";
        public const string LineLengthMin = "LineLengthMin";
        public const string LineLengthMax = "LineLengthMax";
        public const string LineLengthAvg = "LineLengthAvg";
        public const string LineLengthMmMin = "LineLengthMmMin";
        public const string LineLengthMmMax = "LineLengthMmMax";
        public const string LineLengthMmAvg = "LineLengthMmAvg";
        public const string LineAngleMin = "LineAngleMin";
        public const string LineAngleMax = "LineAngleMax";
        public const string LineAngleAvg = "LineAngleAvg";
        public const string IntersectionX = "IntersectionX";
        public const string IntersectionY = "IntersectionY";
        public const string CornerOuterContourVerified = "CornerOuterContourVerified";
        public const string DistanceCount = "DistanceCount";
        public const string DistancePxMin = "DistancePxMin";
        public const string DistancePxMax = "DistancePxMax";
        public const string DistancePxAvg = "DistancePxAvg";
        public const string DistancePxRange = "DistancePxRange";
        public const string DistanceMmMin = "DistanceMmMin";
        public const string DistanceMmMax = "DistanceMmMax";
        public const string DistanceMmAvg = "DistanceMmAvg";
        public const string DistanceMmRange = "DistanceMmRange";
        public const string PitchCount = "PitchCount";
        public const string PitchPxMin = "PitchPxMin";
        public const string PitchPxMax = "PitchPxMax";
        public const string PitchPxAvg = "PitchPxAvg";
        public const string PitchPxRange = "PitchPxRange";
        public const string GapCandidateLineCount = "GapCandidateLineCount";
        public const string GapCandidatePairCount = "GapCandidatePairCount";
        public const string GapOverlapPairCount = "GapOverlapPairCount";
        public const string GapSeparationPairCount = "GapSeparationPairCount";
        public const string GapParallelPairCount = "GapParallelPairCount";
        public const string GapContrastPairCount = "GapContrastPairCount";
        public const string GapBestDarkContrast = "GapBestDarkContrast";
        public const string GapBestDarkCoverageRatio = "GapBestDarkCoverageRatio";
        public const string GapSelectedAngleDeltaDeg = "GapSelectedAngleDeltaDeg";
        public const string GapSelectedSupportRatio = "GapSelectedSupportRatio";
        public const string GapDarkContrast = "GapDarkContrast";
        public const string GapDarkCoverageRatio = "GapDarkCoverageRatio";
        public const string GapBandMeanGray = "GapBandMeanGray";
        public const string GapScoreMargin = "GapScoreMargin";
        public const string GapUpperSupportPointCount = "GapUpperSupportPointCount";
        public const string GapLowerSupportPointCount = "GapLowerSupportPointCount";
        public const string CurveOuterArcLengthPx = "CurveOuterArcLengthPx";
        public const string CurveInnerArcLengthPx = "CurveInnerArcLengthPx";
        public const string CurveCenterArcLengthPx = "CurveCenterArcLengthPx";
        public const string CurveOuterArcLengthMm = "CurveOuterArcLengthMm";
        public const string CurveInnerArcLengthMm = "CurveInnerArcLengthMm";
        public const string CurveCenterArcLengthMm = "CurveCenterArcLengthMm";
        public const string CurveProfileRowCount = "CurveProfileRowCount";
        public const string MergeOverlayCount = "MergeOverlayCount";
        public const string MergeSourceCount = "MergeSourceCount";
        public const string BoundsWidthMin = "BoundsWidthMin";
        public const string BoundsWidthMax = "BoundsWidthMax";
        public const string BoundsWidthAvg = "BoundsWidthAvg";
        public const string BoundsWidthMmMin = "BoundsWidthMmMin";
        public const string BoundsWidthMmMax = "BoundsWidthMmMax";
        public const string BoundsWidthMmAvg = "BoundsWidthMmAvg";
        public const string BoundsHeightMin = "BoundsHeightMin";
        public const string BoundsHeightMax = "BoundsHeightMax";
        public const string BoundsHeightAvg = "BoundsHeightAvg";
        public const string BoundsHeightMmMin = "BoundsHeightMmMin";
        public const string BoundsHeightMmMax = "BoundsHeightMmMax";
        public const string BoundsHeightMmAvg = "BoundsHeightMmAvg";
        public const string SourceImageWidth = "SourceImageWidth";
        public const string SourceImageHeight = "SourceImageHeight";
        public const string SourceImageChannels = "SourceImageChannels";
        public const string ResultImageWidth = "ResultImageWidth";
        public const string ResultImageHeight = "ResultImageHeight";
        public const string ResultImageChannels = "ResultImageChannels";
        public const string FixtureCenterX = "FixtureCenterX";
        public const string FixtureCenterY = "FixtureCenterY";
        public const string FixtureAngle = "FixtureAngle";
        public const string FixtureScale = "FixtureScale";
        public const string FixtureOffsetX = "FixtureOffsetX";
        public const string FixtureOffsetY = "FixtureOffsetY";
        public const string FixtureAngleDelta = "FixtureAngleDelta";
        public const string FixtureScaleRatio = "FixtureScaleRatio";
        public const string FixtureReferenceImageWidth = "FixtureReferenceImageWidth";
        public const string FixtureReferenceImageHeight = "FixtureReferenceImageHeight";
        public const string FixtureNormalizedImageWidth = "FixtureNormalizedImageWidth";
        public const string FixtureNormalizedImageHeight = "FixtureNormalizedImageHeight";
        public const string FixtureValidPixelRatio = "FixtureValidPixelRatio";
        public const string FixtureAppliedCenterX = "FixtureAppliedCenterX";
        public const string FixtureAppliedCenterY = "FixtureAppliedCenterY";
        public const string FixtureAppliedAngle = "FixtureAppliedAngle";
        public const string FixtureAppliedScaleRatio = "FixtureAppliedScaleRatio";
        public const string FixtureEffectiveRoiX = "FixtureEffectiveRoiX";
        public const string FixtureEffectiveRoiY = "FixtureEffectiveRoiY";
        public const string FixtureLineASupportCount = "FixtureLineASupportCount";
        public const string FixtureLineBSupportCount = "FixtureLineBSupportCount";
        public const string FixtureLineAFitResidualPx = "FixtureLineAFitResidualPx";
        public const string FixtureLineBFitResidualPx = "FixtureLineBFitResidualPx";
        public const string FixtureIncludedAngleDeg = "FixtureIncludedAngleDeg";
        public const string AffineM11 = "AffineM11";
        public const string AffineM12 = "AffineM12";
        public const string AffineM13 = "AffineM13";
        public const string AffineM21 = "AffineM21";
        public const string AffineM22 = "AffineM22";
        public const string AffineM23 = "AffineM23";
        public const string AffineDeterminant = "AffineDeterminant";
        public const string AffineScaleX = "AffineScaleX";
        public const string AffineScaleY = "AffineScaleY";
        public const string AffineRotationDeg = "AffineRotationDeg";
        public const string AffineShearCosine = "AffineShearCosine";
        public const string AffineTranslationX = "AffineTranslationX";
        public const string AffineTranslationY = "AffineTranslationY";
        public const string AffineSourceTriangleArea = "AffineSourceTriangleArea";
        public const string AffineDestinationTriangleArea = "AffineDestinationTriangleArea";
        public const string AffineValidPixelRatio = "AffineValidPixelRatio";
        public const string AffineDetectedSourcePointCount = "AffineDetectedSourcePointCount";
        public const string AffineSourcePoint1X = "AffineSourcePoint1X";
        public const string AffineSourcePoint1Y = "AffineSourcePoint1Y";
        public const string AffineSourcePoint2X = "AffineSourcePoint2X";
        public const string AffineSourcePoint2Y = "AffineSourcePoint2Y";
        public const string AffineSourcePoint3X = "AffineSourcePoint3X";
        public const string AffineSourcePoint3Y = "AffineSourcePoint3Y";
        public const string DifferencePixelCount = "DifferencePixelCount";
        public const string DifferencePixelRatio = "DifferencePixelRatio";
        public const string DifferenceMean = "DifferenceMean";
        public const string RegistrationInliers = "RegistrationInliers";
        public const string RegistrationInlierRatio = "RegistrationInlierRatio";
        public const string RegistrationScore = "RegistrationScore";
        public const string ReferenceIndex = "ReferenceIndex";
        public const string ValidPixelRatio = "ValidPixelRatio";
        public const string GeometryDistancePx = "GeometryDistancePx";
        public const string GeometryDistanceMm = "GeometryDistanceMm";
        public const string GeometryAngleDeg = "GeometryAngleDeg";
        public const string GeometrySignedClearancePx = "GeometrySignedClearancePx";
        public const string GeometrySignedClearanceMm = "GeometrySignedClearanceMm";
        public const string GeometryParallelDeltaDeg = "GeometryParallelDeltaDeg";
        public const string GeometryExtensionAPx = "GeometryExtensionAPx";
        public const string GeometryExtensionBPx = "GeometryExtensionBPx";
        public const string CircleCenterX = "CircleCenterX";
        public const string CircleCenterY = "CircleCenterY";
        public const string CircleRadiusPx = "CircleRadiusPx";
        public const string CircleDiameterPx = "CircleDiameterPx";
        public const string CircleRadiusMm = "CircleRadiusMm";
        public const string CircleDiameterMm = "CircleDiameterMm";
        public const string CircleSupportCount = "CircleSupportCount";
        public const string CircleSupportRatio = "CircleSupportRatio";
        public const string CircleCoverageDeg = "CircleCoverageDeg";
        public const string CircleFitResidualPx = "CircleFitResidualPx";

        private static readonly VisionPipelineMetricDefinition[] MetricDefinitions =
        {
            new VisionPipelineMetricDefinition { Name = ResultCount, DisplayName = "Result Count", Description = "Number of result items detected by the tool." },
            new VisionPipelineMetricDefinition { Name = AffineM11, DisplayName = "Affine M11", Description = "First row, first column of the authoritative 2 x 3 affine matrix." },
            new VisionPipelineMetricDefinition { Name = AffineM12, DisplayName = "Affine M12", Description = "First row, second column of the authoritative 2 x 3 affine matrix." },
            new VisionPipelineMetricDefinition { Name = AffineM13, DisplayName = "Affine M13", Description = "Horizontal translation coefficient of the authoritative affine matrix." },
            new VisionPipelineMetricDefinition { Name = AffineM21, DisplayName = "Affine M21", Description = "Second row, first column of the authoritative 2 x 3 affine matrix." },
            new VisionPipelineMetricDefinition { Name = AffineM22, DisplayName = "Affine M22", Description = "Second row, second column of the authoritative 2 x 3 affine matrix." },
            new VisionPipelineMetricDefinition { Name = AffineM23, DisplayName = "Affine M23", Description = "Vertical translation coefficient of the authoritative affine matrix." },
            new VisionPipelineMetricDefinition { Name = AffineDeterminant, DisplayName = "Affine Determinant", Description = "Signed area-scale determinant of the affine linear component." },
            new VisionPipelineMetricDefinition { Name = AffineScaleX, DisplayName = "Affine Scale X", Description = "Review scale of the first affine basis vector." },
            new VisionPipelineMetricDefinition { Name = AffineScaleY, DisplayName = "Affine Scale Y", Description = "Review scale of the second affine basis vector." },
            new VisionPipelineMetricDefinition { Name = AffineRotationDeg, DisplayName = "Affine Rotation (deg)", Description = "Review rotation derived from the first affine basis vector." },
            new VisionPipelineMetricDefinition { Name = AffineShearCosine, DisplayName = "Affine Shear Cosine", Description = "Cosine between transformed basis vectors; zero indicates orthogonal basis vectors." },
            new VisionPipelineMetricDefinition { Name = AffineTranslationX, DisplayName = "Affine Translation X", Description = "Horizontal affine translation in output pixels." },
            new VisionPipelineMetricDefinition { Name = AffineTranslationY, DisplayName = "Affine Translation Y", Description = "Vertical affine translation in output pixels." },
            new VisionPipelineMetricDefinition { Name = AffineSourceTriangleArea, DisplayName = "Affine Source Triangle Area", Description = "Pixel area spanned by the three taught source points." },
            new VisionPipelineMetricDefinition { Name = AffineDestinationTriangleArea, DisplayName = "Affine Destination Triangle Area", Description = "Pixel area spanned by the three taught destination points." },
            new VisionPipelineMetricDefinition { Name = AffineValidPixelRatio, DisplayName = "Affine Valid Pixel Ratio", Description = "Fraction of the output canvas covered by transformed source pixels." },
            new VisionPipelineMetricDefinition { Name = AffineDetectedSourcePointCount, DisplayName = "Affine Detected Source Point Count", Description = "Number of earlier accepted typed Point features resolved for this Affine run." },
            new VisionPipelineMetricDefinition { Name = AffineSourcePoint1X, DisplayName = "Affine Source Point 1 X", Description = "Runtime X coordinate resolved from source Point feature 1." },
            new VisionPipelineMetricDefinition { Name = AffineSourcePoint1Y, DisplayName = "Affine Source Point 1 Y", Description = "Runtime Y coordinate resolved from source Point feature 1." },
            new VisionPipelineMetricDefinition { Name = AffineSourcePoint2X, DisplayName = "Affine Source Point 2 X", Description = "Runtime X coordinate resolved from source Point feature 2." },
            new VisionPipelineMetricDefinition { Name = AffineSourcePoint2Y, DisplayName = "Affine Source Point 2 Y", Description = "Runtime Y coordinate resolved from source Point feature 2." },
            new VisionPipelineMetricDefinition { Name = AffineSourcePoint3X, DisplayName = "Affine Source Point 3 X", Description = "Runtime X coordinate resolved from source Point feature 3." },
            new VisionPipelineMetricDefinition { Name = AffineSourcePoint3Y, DisplayName = "Affine Source Point 3 Y", Description = "Runtime Y coordinate resolved from source Point feature 3." },
            new VisionPipelineMetricDefinition { Name = GeometryDistancePx, DisplayName = "Geometry Distance (px)", Description = "Pixel distance produced by the selected typed feature relationship." },
            new VisionPipelineMetricDefinition { Name = GeometryDistanceMm, DisplayName = "Geometry Distance (mm)", Description = "Typed geometry distance converted by the positive legacy PIXELPERMM value, whose runtime semantics are mm per pixel." },
            new VisionPipelineMetricDefinition { Name = GeometryAngleDeg, DisplayName = "Geometry Angle (deg)", Description = "Smaller undirected angle between two source segments." },
            new VisionPipelineMetricDefinition { Name = GeometrySignedClearancePx, DisplayName = "Signed Circle Clearance (px)", Description = "Finite-segment center distance minus fitted circle radius; negative values indicate overlap." },
            new VisionPipelineMetricDefinition { Name = GeometrySignedClearanceMm, DisplayName = "Signed Circle Clearance (mm)", Description = "Signed circle clearance converted by the positive legacy PIXELPERMM value, whose runtime semantics are mm per pixel." },
            new VisionPipelineMetricDefinition { Name = GeometryParallelDeltaDeg, DisplayName = "Parallel Delta (deg)", Description = "Smaller undirected angle delta used by the LineLineDistance parallel gate." },
            new VisionPipelineMetricDefinition { Name = GeometryExtensionAPx, DisplayName = "Intersection Extension A (px)", Description = "Required extension beyond source segment A to reach the fitted-line intersection." },
            new VisionPipelineMetricDefinition { Name = GeometryExtensionBPx, DisplayName = "Intersection Extension B (px)", Description = "Required extension beyond source segment B to reach the fitted-line intersection." },
            new VisionPipelineMetricDefinition { Name = CircleCenterX, DisplayName = "Circle Center X", Description = "Fitted circle center X in the producer coordinate layer." },
            new VisionPipelineMetricDefinition { Name = CircleCenterY, DisplayName = "Circle Center Y", Description = "Fitted circle center Y in the producer coordinate layer." },
            new VisionPipelineMetricDefinition { Name = CircleRadiusPx, DisplayName = "Circle Radius (px)", Description = "Robust radial-caliper fitted radius in pixels." },
            new VisionPipelineMetricDefinition { Name = CircleDiameterPx, DisplayName = "Circle Diameter (px)", Description = "Twice the fitted circle radius in pixels." },
            new VisionPipelineMetricDefinition { Name = CircleRadiusMm, DisplayName = "Circle Radius (mm)", Description = "Fitted circle radius converted by the positive legacy PIXELPERMM value, whose runtime semantics are mm per pixel." },
            new VisionPipelineMetricDefinition { Name = CircleDiameterMm, DisplayName = "Circle Diameter (mm)", Description = "Fitted circle diameter converted by the positive legacy PIXELPERMM value, whose runtime semantics are mm per pixel." },
            new VisionPipelineMetricDefinition { Name = CircleSupportCount, DisplayName = "Circle Support Count", Description = "Accepted radial edge samples used by the fitted circle." },
            new VisionPipelineMetricDefinition { Name = CircleSupportRatio, DisplayName = "Circle Support Ratio", Description = "Accepted radial samples divided by requested scans." },
            new VisionPipelineMetricDefinition { Name = CircleCoverageDeg, DisplayName = "Circle Coverage (deg)", Description = "Configured angular sweep multiplied by accepted radial-sample ratio." },
            new VisionPipelineMetricDefinition { Name = CircleFitResidualPx, DisplayName = "Circle Fit Residual (px)", Description = "RMS radial residual of accepted support points." },
            new VisionPipelineMetricDefinition { Name = AreaMin, DisplayName = "Area Min", Description = "Minimum detected area." },
            new VisionPipelineMetricDefinition { Name = AreaMax, DisplayName = "Area Max", Description = "Maximum detected area." },
            new VisionPipelineMetricDefinition { Name = AreaAvg, DisplayName = "Area Avg", Description = "Average detected area." },
            new VisionPipelineMetricDefinition { Name = ScoreMin, DisplayName = "Score Min", Description = "Minimum matching score." },
            new VisionPipelineMetricDefinition { Name = ScoreMax, DisplayName = "Score Max", Description = "Maximum matching score." },
            new VisionPipelineMetricDefinition { Name = ScoreAvg, DisplayName = "Score Avg", Description = "Average matching score." },
            new VisionPipelineMetricDefinition { Name = ScoreMargin, DisplayName = "Best/Second Score Margin", Description = "Best score minus second-best score in percentage points when Matching requests exactly two candidates; a missing second candidate contributes zero." },
            new VisionPipelineMetricDefinition { Name = UniqueMatchState, DisplayName = "Unique Match State", Description = "Unique-match runtime state: 1 NoMatch, 2 Success, 3 Ambiguous." },
            new VisionPipelineMetricDefinition { Name = UniqueMatchPlausibleAlternativeCount, DisplayName = "Unique Alternatives", Description = "Spatially distinct alternatives above SCORE_MIN whose selected-minus-alternative score margin violates the unique-match gate." },
            new VisionPipelineMetricDefinition { Name = UniqueMatchScoreMargin, DisplayName = "Unique Score Margin", Description = "Normalized 0..1 selected score minus strongest spatially distinct alternative score." },
            new VisionPipelineMetricDefinition { Name = AngleMin, DisplayName = "Angle Min", Description = "Minimum result angle." },
            new VisionPipelineMetricDefinition { Name = AngleMax, DisplayName = "Angle Max", Description = "Maximum result angle." },
            new VisionPipelineMetricDefinition { Name = AngleAvg, DisplayName = "Angle Avg", Description = "Average result angle." },
            new VisionPipelineMetricDefinition { Name = FixtureCenterX, DisplayName = "Fixture Center X", Description = "Current X coordinate published by the fixture producer." },
            new VisionPipelineMetricDefinition { Name = FixtureCenterY, DisplayName = "Fixture Center Y", Description = "Current Y coordinate published by the fixture producer." },
            new VisionPipelineMetricDefinition { Name = FixtureAngle, DisplayName = "Fixture Angle", Description = "Current OpenCV-convention angle published by the fixture producer." },
            new VisionPipelineMetricDefinition { Name = FixtureScale, DisplayName = "Fixture Scale", Description = "Current uniform scale published by the fixture producer." },
            new VisionPipelineMetricDefinition { Name = FixtureOffsetX, DisplayName = "Fixture Offset X", Description = "Current fixture X minus its taught reference X." },
            new VisionPipelineMetricDefinition { Name = FixtureOffsetY, DisplayName = "Fixture Offset Y", Description = "Current fixture Y minus its taught reference Y." },
            new VisionPipelineMetricDefinition { Name = FixtureAngleDelta, DisplayName = "Fixture Angle Delta", Description = "Normalized current fixture angle minus its taught reference angle." },
            new VisionPipelineMetricDefinition { Name = FixtureScaleRatio, DisplayName = "Fixture Scale Ratio", Description = "Current fixture scale divided by its taught reference scale." },
            new VisionPipelineMetricDefinition { Name = FixtureReferenceImageWidth, DisplayName = "Fixture Reference Width", Description = "Reviewed reference image width used by fixture normalization." },
            new VisionPipelineMetricDefinition { Name = FixtureReferenceImageHeight, DisplayName = "Fixture Reference Height", Description = "Reviewed reference image height used by fixture normalization." },
            new VisionPipelineMetricDefinition { Name = FixtureNormalizedImageWidth, DisplayName = "Normalized Width", Description = "Output width of the inverse-similarity normalized image." },
            new VisionPipelineMetricDefinition { Name = FixtureNormalizedImageHeight, DisplayName = "Normalized Height", Description = "Output height of the inverse-similarity normalized image." },
            new VisionPipelineMetricDefinition { Name = FixtureValidPixelRatio, DisplayName = "Normalized Valid Pixel Ratio", Description = "Fraction of the normalized canvas covered by transformed source pixels." },
            new VisionPipelineMetricDefinition { Name = FixtureAppliedCenterX, DisplayName = "Applied Current Center X", Description = "Current fixture center X used by inverse-similarity normalization." },
            new VisionPipelineMetricDefinition { Name = FixtureAppliedCenterY, DisplayName = "Applied Current Center Y", Description = "Current fixture center Y used by inverse-similarity normalization." },
            new VisionPipelineMetricDefinition { Name = FixtureAppliedAngle, DisplayName = "Applied Correction Angle", Description = "Inverse angle correction applied to produce the normalized image." },
            new VisionPipelineMetricDefinition { Name = FixtureAppliedScaleRatio, DisplayName = "Applied Correction Scale", Description = "Inverse uniform scale applied to produce the normalized image." },
            new VisionPipelineMetricDefinition { Name = FixtureEffectiveRoiX, DisplayName = "Effective ROI X", Description = "Runtime ROI X after fixture translation. The saved CvROI is unchanged." },
            new VisionPipelineMetricDefinition { Name = FixtureEffectiveRoiY, DisplayName = "Effective ROI Y", Description = "Runtime ROI Y after fixture translation. The saved CvROI is unchanged." },
            new VisionPipelineMetricDefinition { Name = FixtureLineASupportCount, DisplayName = "Fixture Datum A Support", Description = "Support count retained by the earlier Line A result used to publish a line fixture." },
            new VisionPipelineMetricDefinition { Name = FixtureLineBSupportCount, DisplayName = "Fixture Datum B Support", Description = "Support count retained by the earlier Line B result used to publish a line fixture." },
            new VisionPipelineMetricDefinition { Name = FixtureLineAFitResidualPx, DisplayName = "Fixture Datum A Residual", Description = "Fit residual in pixels retained by the earlier Line A result used to publish a line fixture." },
            new VisionPipelineMetricDefinition { Name = FixtureLineBFitResidualPx, DisplayName = "Fixture Datum B Residual", Description = "Fit residual in pixels retained by the earlier Line B result used to publish a line fixture." },
            new VisionPipelineMetricDefinition { Name = FixtureIncludedAngleDeg, DisplayName = "Fixture Included Angle", Description = "Undirected included angle in degrees between the two datum segments." },
            new VisionPipelineMetricDefinition { Name = DifferencePixelCount, DisplayName = "Difference Pixel Count", Description = "Pixels above the reference-difference threshold inside the valid registered region." },
            new VisionPipelineMetricDefinition { Name = DifferencePixelRatio, DisplayName = "Difference Pixel Ratio", Description = "Difference pixels divided by the valid registered comparison pixels." },
            new VisionPipelineMetricDefinition { Name = DifferenceMean, DisplayName = "Difference Mean", Description = "Mean absolute grayscale difference after registration and brightness normalization." },
            new VisionPipelineMetricDefinition { Name = RegistrationInliers, DisplayName = "Registration Inliers", Description = "RANSAC inlier matches used by the selected reference registration." },
            new VisionPipelineMetricDefinition { Name = RegistrationInlierRatio, DisplayName = "Registration Inlier Ratio", Description = "RANSAC registration inliers divided by ratio-test matches." },
            new VisionPipelineMetricDefinition { Name = RegistrationScore, DisplayName = "Registration Score", Description = "Review score derived from the registered mean difference; higher is closer." },
            new VisionPipelineMetricDefinition { Name = ReferenceIndex, DisplayName = "Reference Index", Description = "Zero-based index of the selected reference path." },
            new VisionPipelineMetricDefinition { Name = ValidPixelRatio, DisplayName = "Valid Pixel Ratio", Description = "Fraction of source pixels covered by the registered reference after border exclusion." },
            new VisionPipelineMetricDefinition { Name = MeanValueMin, DisplayName = "Mean Min", Description = "Minimum mean value." },
            new VisionPipelineMetricDefinition { Name = MeanValueMax, DisplayName = "Mean Max", Description = "Maximum mean value." },
            new VisionPipelineMetricDefinition { Name = MeanValueAvg, DisplayName = "Mean Avg", Description = "Average mean value." },
            new VisionPipelineMetricDefinition { Name = MaskPixelCount, DisplayName = "Mask Pixel Count", Description = "Number of pixels selected by a mask-producing tool." },
            new VisionPipelineMetricDefinition { Name = MaskPixelRatio, DisplayName = "Mask Pixel Ratio", Description = "Selected mask pixels divided by the inspected image or ROI area." },
            new VisionPipelineMetricDefinition { Name = EdgeCount, DisplayName = "Edge Count", Description = "Number of edge groups." },
            new VisionPipelineMetricDefinition { Name = EdgePointCount, DisplayName = "Edge Point Count", Description = "Total number of edge points." },
            new VisionPipelineMetricDefinition { Name = LineLengthMin, DisplayName = "Line Length Min", Description = "Minimum fitted line overlay length." },
            new VisionPipelineMetricDefinition { Name = LineLengthMax, DisplayName = "Line Length Max", Description = "Maximum fitted line overlay length." },
            new VisionPipelineMetricDefinition { Name = LineLengthAvg, DisplayName = "Line Length Avg", Description = "Average fitted line overlay length." },
            new VisionPipelineMetricDefinition { Name = LineLengthMmMin, DisplayName = "Line Length Min (mm)", Description = "Minimum fitted line overlay length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = LineLengthMmMax, DisplayName = "Line Length Max (mm)", Description = "Maximum fitted line overlay length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = LineLengthMmAvg, DisplayName = "Line Length Avg (mm)", Description = "Average fitted line overlay length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = LineAngleMin, DisplayName = "Line Angle Min", Description = "Minimum fitted line overlay angle in degrees." },
            new VisionPipelineMetricDefinition { Name = LineAngleMax, DisplayName = "Line Angle Max", Description = "Maximum fitted line overlay angle in degrees." },
            new VisionPipelineMetricDefinition { Name = LineAngleAvg, DisplayName = "Line Angle Avg", Description = "Average fitted line overlay angle in degrees." },
            new VisionPipelineMetricDefinition { Name = IntersectionX, DisplayName = "Intersection X", Description = "Horizontal image coordinate of the fitted line intersection." },
            new VisionPipelineMetricDefinition { Name = IntersectionY, DisplayName = "Intersection Y", Description = "Vertical image coordinate of the fitted line intersection." },
            new VisionPipelineMetricDefinition { Name = CornerOuterContourVerified, DisplayName = "Corner Outer Contour Verified", Description = "1 when the corner comes from fitted lower/right support points on the selected outer contour; it does not prove agreement with an operator target. 0 when profile or edge fallback was required." },
            new VisionPipelineMetricDefinition { Name = DistanceCount, DisplayName = "Distance Count", Description = "Number of valid distance lines between paired edges." },
            new VisionPipelineMetricDefinition { Name = DistancePxMin, DisplayName = "Distance Min (px)", Description = "Minimum distance between paired edge points in pixels." },
            new VisionPipelineMetricDefinition { Name = DistancePxMax, DisplayName = "Distance Max (px)", Description = "Maximum distance between paired edge points in pixels." },
            new VisionPipelineMetricDefinition { Name = DistancePxAvg, DisplayName = "Distance Avg (px)", Description = "Average distance between paired edge points in pixels." },
            new VisionPipelineMetricDefinition { Name = DistancePxRange, DisplayName = "Distance Range (px)", Description = "Spread between maximum and minimum paired edge distances in pixels." },
            new VisionPipelineMetricDefinition { Name = DistanceMmMin, DisplayName = "Distance Min (mm)", Description = "Minimum distance between paired edge points converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = DistanceMmMax, DisplayName = "Distance Max (mm)", Description = "Maximum distance between paired edge points converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = DistanceMmAvg, DisplayName = "Distance Avg (mm)", Description = "Average distance between paired edge points converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = DistanceMmRange, DisplayName = "Distance Range (mm)", Description = "Spread between maximum and minimum paired edge distances converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = PitchCount, DisplayName = "Pitch Count", Description = "Number of adjacent center-to-center pin pitch measurements." },
            new VisionPipelineMetricDefinition { Name = PitchPxMin, DisplayName = "Pitch Min (px)", Description = "Minimum adjacent pin center-to-center pitch in pixels." },
            new VisionPipelineMetricDefinition { Name = PitchPxMax, DisplayName = "Pitch Max (px)", Description = "Maximum adjacent pin center-to-center pitch in pixels." },
            new VisionPipelineMetricDefinition { Name = PitchPxAvg, DisplayName = "Pitch Avg (px)", Description = "Average adjacent pin center-to-center pitch in pixels." },
            new VisionPipelineMetricDefinition { Name = PitchPxRange, DisplayName = "Pitch Range (px)", Description = "Spread between maximum and minimum adjacent pin center-to-center pitch in pixels." },
            new VisionPipelineMetricDefinition { Name = GapCandidateLineCount, DisplayName = "Gap Candidate Lines", Description = "Near-horizontal edge lines retained inside the reviewed Gap ROI before pairing." },
            new VisionPipelineMetricDefinition { Name = GapCandidatePairCount, DisplayName = "Gap Candidate Pairs", Description = "Parallel dark-band edge pairs that passed separation, support, and contrast gates." },
            new VisionPipelineMetricDefinition { Name = GapOverlapPairCount, DisplayName = "Gap Overlap Pairs", Description = "Candidate pairs whose shared horizontal support passed the reviewed minimum ratio." },
            new VisionPipelineMetricDefinition { Name = GapSeparationPairCount, DisplayName = "Gap Separation Pairs", Description = "Overlap-supported pairs whose measured separation remained inside the configured Gap range." },
            new VisionPipelineMetricDefinition { Name = GapParallelPairCount, DisplayName = "Gap Parallel Pairs", Description = "Separation-supported pairs whose angle delta passed the parallel-line gate." },
            new VisionPipelineMetricDefinition { Name = GapContrastPairCount, DisplayName = "Gap Contrast Pairs", Description = "Parallel pairs whose between-edge band was darker than its surroundings by the configured contrast." },
            new VisionPipelineMetricDefinition { Name = GapBestDarkContrast, DisplayName = "Gap Best Dark Contrast", Description = "Highest surrounding-minus-band gray-value contrast among geometry-supported pairs, including rejected pairs." },
            new VisionPipelineMetricDefinition { Name = GapBestDarkCoverageRatio, DisplayName = "Gap Best Dark Coverage", Description = "Highest fraction of sampled columns with sufficient local dark-band contrast among geometry-supported pairs." },
            new VisionPipelineMetricDefinition { Name = GapSelectedAngleDeltaDeg, DisplayName = "Gap Parallel Delta (deg)", Description = "Absolute angle difference between the selected upper and lower Gap edges." },
            new VisionPipelineMetricDefinition { Name = GapSelectedSupportRatio, DisplayName = "Gap Support Ratio", Description = "Selected edge-pair horizontal overlap divided by the reviewed ROI width." },
            new VisionPipelineMetricDefinition { Name = GapDarkContrast, DisplayName = "Gap Dark Contrast", Description = "Mean surrounding brightness minus mean brightness between the selected Gap edges." },
            new VisionPipelineMetricDefinition { Name = GapDarkCoverageRatio, DisplayName = "Gap Dark Coverage", Description = "Fraction of selected-pair sample columns whose local dark-band contrast passed the configured gate." },
            new VisionPipelineMetricDefinition { Name = GapBandMeanGray, DisplayName = "Gap Band Mean Gray", Description = "Mean gray value between the selected edges; lower values provide evidence that the pair encloses the intended dark band." },
            new VisionPipelineMetricDefinition { Name = GapScoreMargin, DisplayName = "Gap Score Margin", Description = "Selected edge-pair score minus the next candidate score; small values are rejected as ambiguous." },
            new VisionPipelineMetricDefinition { Name = GapUpperSupportPointCount, DisplayName = "Gap Upper Support Points", Description = "Canny edge points supporting the selected upper fitted line." },
            new VisionPipelineMetricDefinition { Name = GapLowerSupportPointCount, DisplayName = "Gap Lower Support Points", Description = "Canny edge points supporting the selected lower fitted line." },
            new VisionPipelineMetricDefinition { Name = CurveOuterArcLengthPx, DisplayName = "Curve Outer Arc (px)", Description = "Arc length of the detected dark-band outer edge in pixels." },
            new VisionPipelineMetricDefinition { Name = CurveInnerArcLengthPx, DisplayName = "Curve Inner Arc (px)", Description = "Arc length of the detected dark-band inner edge in pixels." },
            new VisionPipelineMetricDefinition { Name = CurveCenterArcLengthPx, DisplayName = "Curve Center Arc (px)", Description = "Arc length of the detected dark-band center path in pixels." },
            new VisionPipelineMetricDefinition { Name = CurveOuterArcLengthMm, DisplayName = "Curve Outer Arc (mm)", Description = "Outer edge arc length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = CurveInnerArcLengthMm, DisplayName = "Curve Inner Arc (mm)", Description = "Inner edge arc length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = CurveCenterArcLengthMm, DisplayName = "Curve Center Arc (mm)", Description = "Center path arc length converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = CurveProfileRowCount, DisplayName = "Curve Profile Rows", Description = "Number of image rows contributing to the detected curve profile." },
            new VisionPipelineMetricDefinition { Name = MergeOverlayCount, DisplayName = "Merge Overlay Count", Description = "Number of overlays collected into the merged result." },
            new VisionPipelineMetricDefinition { Name = MergeSourceCount, DisplayName = "Merge Source Count", Description = "Number of previous steps that contributed overlays." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMin, DisplayName = "Bounds Width Min", Description = "Minimum rectangle overlay width." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMax, DisplayName = "Bounds Width Max", Description = "Maximum rectangle overlay width." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthAvg, DisplayName = "Bounds Width Avg", Description = "Average rectangle overlay width." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMmMin, DisplayName = "Bounds Width Min (mm)", Description = "Minimum rectangle overlay width converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMmMax, DisplayName = "Bounds Width Max (mm)", Description = "Maximum rectangle overlay width converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsWidthMmAvg, DisplayName = "Bounds Width Avg (mm)", Description = "Average rectangle overlay width converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMin, DisplayName = "Bounds Height Min", Description = "Minimum rectangle overlay height." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMax, DisplayName = "Bounds Height Max", Description = "Maximum rectangle overlay height." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightAvg, DisplayName = "Bounds Height Avg", Description = "Average rectangle overlay height." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMmMin, DisplayName = "Bounds Height Min (mm)", Description = "Minimum rectangle overlay height converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMmMax, DisplayName = "Bounds Height Max (mm)", Description = "Maximum rectangle overlay height converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = BoundsHeightMmAvg, DisplayName = "Bounds Height Avg (mm)", Description = "Average rectangle overlay height converted by PIXELPERMM." },
            new VisionPipelineMetricDefinition { Name = SourceImageWidth, DisplayName = "Source Width", Description = "Input image width used by the tool." },
            new VisionPipelineMetricDefinition { Name = SourceImageHeight, DisplayName = "Source Height", Description = "Input image height used by the tool." },
            new VisionPipelineMetricDefinition { Name = SourceImageChannels, DisplayName = "Source Channels", Description = "Input image channel count used by the tool." },
            new VisionPipelineMetricDefinition { Name = ResultImageWidth, DisplayName = "Result Width", Description = "Result image width returned by the tool." },
            new VisionPipelineMetricDefinition { Name = ResultImageHeight, DisplayName = "Result Height", Description = "Result image height returned by the tool." },
            new VisionPipelineMetricDefinition { Name = ResultImageChannels, DisplayName = "Result Channels", Description = "Result image channel count returned by the tool." }
        };

        private static readonly string[] ImageMetricNames =
        {
            SourceImageWidth,
            SourceImageHeight,
            SourceImageChannels,
            ResultImageWidth,
            ResultImageHeight,
            ResultImageChannels
        };

        private static readonly string[] RectangleOverlayMetricNames =
        {
            BoundsWidthMin,
            BoundsWidthMax,
            BoundsWidthAvg,
            BoundsWidthMmMin,
            BoundsWidthMmMax,
            BoundsWidthMmAvg,
            BoundsHeightMin,
            BoundsHeightMax,
            BoundsHeightAvg,
            BoundsHeightMmMin,
            BoundsHeightMmMax,
            BoundsHeightMmAvg
        };

        private static readonly string[] LineOverlayMetricNames =
        {
            LineLengthMin,
            LineLengthMax,
            LineLengthAvg,
            LineLengthMmMin,
            LineLengthMmMax,
            LineLengthMmAvg,
            LineAngleMin,
            LineAngleMax,
            LineAngleAvg
        };

        private static readonly string[] DistanceMetricNames =
        {
            DistanceCount,
            DistancePxMin,
            DistancePxMax,
            DistancePxAvg,
            DistancePxRange,
            DistanceMmMin,
            DistanceMmMax,
            DistanceMmAvg,
            DistanceMmRange
        };

        private static readonly string[] PinPitchMetricNames =
        {
            PitchCount,
            PitchPxMin,
            PitchPxMax,
            PitchPxAvg,
            PitchPxRange
        };

        private static readonly string[] GapEdgePairMetricNames =
        {
            GapCandidateLineCount,
            GapCandidatePairCount,
            GapOverlapPairCount,
            GapSeparationPairCount,
            GapParallelPairCount,
            GapContrastPairCount,
            GapBestDarkContrast,
            GapBestDarkCoverageRatio,
            GapSelectedAngleDeltaDeg,
            GapSelectedSupportRatio,
            GapDarkContrast,
            GapDarkCoverageRatio,
            GapBandMeanGray,
            GapScoreMargin,
            GapUpperSupportPointCount,
            GapLowerSupportPointCount
        };

        private static readonly string[] CurveBandMetricNames =
        {
            CurveOuterArcLengthPx,
            CurveInnerArcLengthPx,
            CurveCenterArcLengthPx,
            CurveOuterArcLengthMm,
            CurveInnerArcLengthMm,
            CurveCenterArcLengthMm,
            CurveProfileRowCount
        };

        private static readonly Dictionary<string, string[]> ToolMetricNames = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["blob"] = WithImageAndRectangleMetrics(ResultCount, AreaMin, AreaMax, AreaAvg, AngleMin, AngleMax, AngleAvg),
            ["contour"] = WithImageAndRectangleMetrics(ResultCount, AreaMin, AreaMax, AreaAvg, AngleMin, AngleMax, AngleAvg),
            ["corner"] = WithImageAndRectangleMetrics(ResultCount, AreaMin, AreaMax, AreaAvg),
            ["matching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, ScoreMargin, AngleMin, AngleMax, AngleAvg, FixtureCenterX, FixtureCenterY, FixtureAngle, FixtureScale, FixtureOffsetX, FixtureOffsetY, FixtureAngleDelta, FixtureScaleRatio, FixtureReferenceImageWidth, FixtureReferenceImageHeight),
            ["templatematching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, ScoreMargin, AngleMin, AngleMax, AngleAvg, FixtureCenterX, FixtureCenterY, FixtureAngle, FixtureScale, FixtureOffsetX, FixtureOffsetY, FixtureAngleDelta, FixtureScaleRatio, FixtureReferenceImageWidth, FixtureReferenceImageHeight),
            ["edgebasedmatching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg, UniqueMatchState, UniqueMatchPlausibleAlternativeCount, UniqueMatchScoreMargin),
            ["edgebasedtemplatematching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg, UniqueMatchState, UniqueMatchPlausibleAlternativeCount, UniqueMatchScoreMargin),
            ["edgetemplatematching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg, UniqueMatchState, UniqueMatchPlausibleAlternativeCount, UniqueMatchScoreMargin),
            ["feature"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["featurematching"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["sift"] = WithImageAndRectangleMetrics(ResultCount, ScoreMin, ScoreMax, ScoreAvg, AngleMin, AngleMax, AngleAvg),
            ["mean"] = WithImageAndRectangleMetrics(ResultCount, MeanValueMin, MeanValueMax, MeanValueAvg),
            ["hsv"] = WithImageMetrics(MaskPixelCount, MaskPixelRatio),
            ["hsvmask"] = WithImageMetrics(MaskPixelCount, MaskPixelRatio),
            ["colorhsv"] = WithImageMetrics(MaskPixelCount, MaskPixelRatio),
            ["colormask"] = WithImageMetrics(MaskPixelCount, MaskPixelRatio),
            ["line"] = WithImageAndLineMetrics(ResultCount, EdgeCount, EdgePointCount),
            ["linegauge"] = WithImageAndLineMetrics(ResultCount, EdgeCount, EdgePointCount),
            ["linedistance"] = WithImageAndLineMetrics(new[] { ResultCount, EdgeCount, EdgePointCount }.Concat(DistanceMetricNames).Concat(GapEdgePairMetricNames).ToArray()),
            ["linedistancegauge"] = WithImageAndLineMetrics(new[] { ResultCount, EdgeCount, EdgePointCount }.Concat(DistanceMetricNames).Concat(GapEdgePairMetricNames).ToArray()),
            ["pinarraygap"] = WithImageAndLineMetrics(new[] { ResultCount, EdgeCount, EdgePointCount }.Concat(DistanceMetricNames).Concat(PinPitchMetricNames).ToArray()),
            ["adjacentpingap"] = WithImageAndLineMetrics(new[] { ResultCount, EdgeCount, EdgePointCount }.Concat(DistanceMetricNames).Concat(PinPitchMetricNames).ToArray()),
            ["curvebandprofile"] = WithImageAndRectangleMetrics(new[] { ResultCount, EdgeCount, EdgePointCount }.Concat(DistanceMetricNames).Concat(CurveBandMetricNames).ToArray()),
            ["darkbandcurve"] = WithImageAndRectangleMetrics(new[] { ResultCount, EdgeCount, EdgePointCount }.Concat(DistanceMetricNames).Concat(CurveBandMetricNames).ToArray()),
            ["outercornerintersection"] = WithImageAndRectangleMetrics(new[] { ResultCount, EdgeCount, EdgePointCount, IntersectionX, IntersectionY, CornerOuterContourVerified }.Concat(LineOverlayMetricNames).ToArray()),
            ["brightobjectcorner"] = WithImageAndRectangleMetrics(new[] { ResultCount, EdgeCount, EdgePointCount, IntersectionX, IntersectionY, CornerOuterContourVerified }.Concat(LineOverlayMetricNames).ToArray()),
            ["lineintersection"] = WithImageAndLineMetrics(new[] { ResultCount, EdgeCount, EdgePointCount, IntersectionX, IntersectionY }.Concat(LineOverlayMetricNames).ToArray()),
            ["lineintersectiongauge"] = WithImageAndLineMetrics(new[] { ResultCount, EdgeCount, EdgePointCount, IntersectionX, IntersectionY }.Concat(LineOverlayMetricNames).ToArray()),
            ["geometrymeasure"] = WithImageAndLineMetrics(ResultCount, GeometryDistancePx, GeometryDistanceMm, GeometryAngleDeg, GeometrySignedClearancePx, GeometrySignedClearanceMm, GeometryParallelDeltaDeg, GeometryExtensionAPx, GeometryExtensionBPx, IntersectionX, IntersectionY),
            ["geometricmeasurement"] = WithImageAndLineMetrics(ResultCount, GeometryDistancePx, GeometryDistanceMm, GeometryAngleDeg, GeometrySignedClearancePx, GeometrySignedClearanceMm, GeometryParallelDeltaDeg, GeometryExtensionAPx, GeometryExtensionBPx, IntersectionX, IntersectionY),
            ["linefixture"] = WithImageAndLineMetrics(ResultCount, FixtureCenterX, FixtureCenterY, FixtureAngle, FixtureScale, FixtureOffsetX, FixtureOffsetY, FixtureAngleDelta, FixtureScaleRatio, FixtureReferenceImageWidth, FixtureReferenceImageHeight, FixtureLineASupportCount, FixtureLineBSupportCount, FixtureLineAFitResidualPx, FixtureLineBFitResidualPx, FixtureIncludedAngleDeg, GeometryExtensionAPx, GeometryExtensionBPx),
            ["dualedgefixture"] = WithImageAndLineMetrics(ResultCount, FixtureCenterX, FixtureCenterY, FixtureAngle, FixtureScale, FixtureOffsetX, FixtureOffsetY, FixtureAngleDelta, FixtureScaleRatio, FixtureReferenceImageWidth, FixtureReferenceImageHeight, FixtureLineASupportCount, FixtureLineBSupportCount, FixtureLineAFitResidualPx, FixtureLineBFitResidualPx, FixtureIncludedAngleDeg, GeometryExtensionAPx, GeometryExtensionBPx),
            ["circlegauge"] = WithImageMetrics(ResultCount, CircleCenterX, CircleCenterY, CircleRadiusPx, CircleDiameterPx, CircleRadiusMm, CircleDiameterMm, CircleSupportCount, CircleSupportRatio, CircleCoverageDeg, CircleFitResidualPx),
            ["threshold"] = ImageMetricNames,
            ["morphology"] = ImageMetricNames,
            ["filter"] = ImageMetricNames,
            ["edgedetection"] = ImageMetricNames,
            ["edge"] = ImageMetricNames,
            ["rotatescale"] = WithImageMetrics(FixtureReferenceImageWidth, FixtureReferenceImageHeight, FixtureNormalizedImageWidth, FixtureNormalizedImageHeight, FixtureValidPixelRatio, FixtureAppliedCenterX, FixtureAppliedCenterY, FixtureAppliedAngle, FixtureAppliedScaleRatio, FixtureCenterX, FixtureCenterY, FixtureAngle, FixtureScale, FixtureOffsetX, FixtureOffsetY, FixtureAngleDelta, FixtureScaleRatio),
            ["rotateandscale"] = WithImageMetrics(FixtureReferenceImageWidth, FixtureReferenceImageHeight, FixtureNormalizedImageWidth, FixtureNormalizedImageHeight, FixtureValidPixelRatio, FixtureAppliedCenterX, FixtureAppliedCenterY, FixtureAppliedAngle, FixtureAppliedScaleRatio, FixtureCenterX, FixtureCenterY, FixtureAngle, FixtureScale, FixtureOffsetX, FixtureOffsetY, FixtureAngleDelta, FixtureScaleRatio),
            ["affine"] = WithImageAndLineMetrics(AffineM11, AffineM12, AffineM13, AffineM21, AffineM22, AffineM23, AffineDeterminant, AffineScaleX, AffineScaleY, AffineRotationDeg, AffineShearCosine, AffineTranslationX, AffineTranslationY, AffineSourceTriangleArea, AffineDestinationTriangleArea, AffineValidPixelRatio, AffineDetectedSourcePointCount, AffineSourcePoint1X, AffineSourcePoint1Y, AffineSourcePoint2X, AffineSourcePoint2Y, AffineSourcePoint3X, AffineSourcePoint3Y),
            ["affinematrix"] = WithImageAndLineMetrics(AffineM11, AffineM12, AffineM13, AffineM21, AffineM22, AffineM23, AffineDeterminant, AffineScaleX, AffineScaleY, AffineRotationDeg, AffineShearCosine, AffineTranslationX, AffineTranslationY, AffineSourceTriangleArea, AffineDestinationTriangleArea, AffineValidPixelRatio, AffineDetectedSourcePointCount, AffineSourcePoint1X, AffineSourcePoint1Y, AffineSourcePoint2X, AffineSourcePoint2Y, AffineSourcePoint3X, AffineSourcePoint3Y),
            ["affinetransform"] = WithImageAndLineMetrics(AffineM11, AffineM12, AffineM13, AffineM21, AffineM22, AffineM23, AffineDeterminant, AffineScaleX, AffineScaleY, AffineRotationDeg, AffineShearCosine, AffineTranslationX, AffineTranslationY, AffineSourceTriangleArea, AffineDestinationTriangleArea, AffineValidPixelRatio, AffineDetectedSourcePointCount, AffineSourcePoint1X, AffineSourcePoint1Y, AffineSourcePoint2X, AffineSourcePoint2Y, AffineSourcePoint3X, AffineSourcePoint3Y),
            ["overlaymerge"] = WithImageAndRectangleMetrics(ResultCount, MergeOverlayCount, MergeSourceCount),
            ["resultmerge"] = WithImageAndRectangleMetrics(ResultCount, MergeOverlayCount, MergeSourceCount),
            ["mergeresult"] = WithImageAndRectangleMetrics(ResultCount, MergeOverlayCount, MergeSourceCount),
            ["referencedifference"] = WithImageAndRectangleMetrics(
                ResultCount,
                AreaMin,
                AreaMax,
                AreaAvg,
                DifferencePixelCount,
                DifferencePixelRatio,
                DifferenceMean,
                RegistrationInliers,
                RegistrationInlierRatio,
                RegistrationScore,
                ReferenceIndex,
                ValidPixelRatio)
        };

        private static readonly VisionPipelineAcceptancePreset[] Presets =
        {
            new VisionPipelineAcceptancePreset { Name = "Fast Step <= 100 ms", MaxElapsedMilliseconds = 100 },
            new VisionPipelineAcceptancePreset { Name = "Detect Count >= 1", MetricName = ResultCount, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "edgebasedmatching", "edgebasedtemplatematching", "edgetemplatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 1 },
            new VisionPipelineAcceptancePreset { Name = "Detect Count = 0", MetricName = ResultCount, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "edgebasedmatching", "edgebasedtemplatematching", "edgetemplatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 0, UseMaximum = true, Maximum = 0 },
            new VisionPipelineAcceptancePreset { Name = "Reference Defect Count = 0", MetricName = ResultCount, ToolTypes = new[] { "referencedifference" }, UseMinimum = true, Minimum = 0, UseMaximum = true, Maximum = 0 },
            new VisionPipelineAcceptancePreset { Name = "Text/Symbol Count 35..80", MetricName = ResultCount, ToolTypes = new[] { "contour", "blob" }, UseMinimum = true, Minimum = 35, UseMaximum = true, Maximum = 80, MaxElapsedMilliseconds = 1000 },
            new VisionPipelineAcceptancePreset { Name = "Area Avg 150..600", MetricName = AreaAvg, ToolTypes = new[] { "blob", "contour", "corner" }, UseMinimum = true, Minimum = 150, UseMaximum = true, Maximum = 600 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width <= 20 px", MetricName = BoundsWidthMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "edgebasedmatching", "edgebasedtemplatematching", "edgetemplatematching", "feature", "featurematching", "sift" }, UseMaximum = true, Maximum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width >= 20 px", MetricName = BoundsWidthMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "edgebasedmatching", "edgebasedtemplatematching", "edgetemplatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height <= 20 px", MetricName = BoundsHeightMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "edgebasedmatching", "edgebasedtemplatematching", "edgetemplatematching", "feature", "featurematching", "sift" }, UseMaximum = true, Maximum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height >= 20 px", MetricName = BoundsHeightMax, ToolTypes = new[] { "blob", "contour", "corner", "matching", "templatematching", "edgebasedmatching", "edgebasedtemplatematching", "edgetemplatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 20 },
            new VisionPipelineAcceptancePreset { Name = "Best Score >= 80", MetricName = ScoreMax, ToolTypes = new[] { "matching", "templatematching", "edgebasedmatching", "edgebasedtemplatematching", "edgetemplatematching", "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 80 },
            new VisionPipelineAcceptancePreset { Name = "Best/Second Score Margin >= 10", MetricName = ScoreMargin, ToolTypes = new[] { "matching", "templatematching" }, UseMinimum = true, Minimum = 10 },
            new VisionPipelineAcceptancePreset { Name = "Best Score >= 60", MetricName = ScoreMax, ToolTypes = new[] { "feature", "featurematching", "sift" }, UseMinimum = true, Minimum = 60 },
            new VisionPipelineAcceptancePreset { Name = "Mean <= 180", MetricName = MeanValueAvg, ToolTypes = new[] { "mean" }, UseMaximum = true, Maximum = 180 },
            new VisionPipelineAcceptancePreset { Name = "Mask Ratio 0.10..0.90", MetricName = MaskPixelRatio, ToolTypes = new[] { "hsv", "hsvmask", "colorhsv", "colormask" }, UseMinimum = true, Minimum = 0.10, UseMaximum = true, Maximum = 0.90 },
            new VisionPipelineAcceptancePreset { Name = "Line Edge Count >= 1", MetricName = EdgeCount, ToolTypes = new[] { "line", "linegauge" }, UseMinimum = true, Minimum = 1 },
            new VisionPipelineAcceptancePreset { Name = "Fitted Line Length >= 100 px", MetricName = LineLengthMax, ToolTypes = new[] { "line", "linegauge" }, UseMinimum = true, Minimum = 100 },
            new VisionPipelineAcceptancePreset { Name = "Fitted Line Length >= 3 mm", MetricName = LineLengthMmMax, ToolTypes = new[] { "line", "linegauge" }, UseMinimum = true, Minimum = 3 },
            new VisionPipelineAcceptancePreset { Name = "Edge Distance 0.30..0.50 mm", MetricName = DistanceMmAvg, ToolTypes = new[] { "linedistance", "linedistancegauge" }, UseMinimum = true, Minimum = 0.30, UseMaximum = true, Maximum = 0.50 },
            new VisionPipelineAcceptancePreset { Name = "Edge Distance Spread <= 0.06 mm", MetricName = DistanceMmRange, ToolTypes = new[] { "linedistance", "linedistancegauge" }, UseMaximum = true, Maximum = 0.06 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width <= 0.12 mm", MetricName = BoundsWidthMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMaximum = true, Maximum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Width >= 0.12 mm", MetricName = BoundsWidthMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMinimum = true, Minimum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height <= 0.12 mm", MetricName = BoundsHeightMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMaximum = true, Maximum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Max Bounds Height >= 0.12 mm", MetricName = BoundsHeightMmMax, ToolTypes = new[] { "blob", "contour", "corner" }, UseMinimum = true, Minimum = 0.12 },
            new VisionPipelineAcceptancePreset { Name = "Merged Overlay Count >= 1", MetricName = MergeOverlayCount, ToolTypes = new[] { "overlaymerge", "resultmerge", "mergeresult" }, UseMinimum = true, Minimum = 1 }
        };

        public static IReadOnlyList<string> GetMetricNames()
        {
            return MetricDefinitions.Select(metric => metric.Name).ToArray();
        }

        private static string[] WithImageMetrics(params string[] metricNames)
        {
            return (metricNames ?? Array.Empty<string>())
                .Concat(ImageMetricNames)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string[] WithImageAndRectangleMetrics(params string[] metricNames)
        {
            return (metricNames ?? Array.Empty<string>())
                .Concat(RectangleOverlayMetricNames)
                .Concat(ImageMetricNames)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string[] WithImageAndLineMetrics(params string[] metricNames)
        {
            return (metricNames ?? Array.Empty<string>())
                .Concat(LineOverlayMetricNames)
                .Concat(ImageMetricNames)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static IReadOnlyList<VisionPipelineMetricDefinition> GetMetricDefinitions()
        {
            return MetricDefinitions;
        }

        public static string GetDisplayName(string metricName)
        {
            if (string.IsNullOrWhiteSpace(metricName))
            {
                return string.Empty;
            }

            VisionPipelineMetricDefinition definition = MetricDefinitions.FirstOrDefault(metric =>
                string.Equals(metric.Name, metricName, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(definition?.DisplayName)
                ? metricName
                : definition.DisplayName;
        }

        public static IReadOnlyList<string> GetMetricNamesForTool(string toolType)
        {
            string normalized = NormalizeToolType(toolType);
            return ToolMetricNames.TryGetValue(normalized, out string[] metricNames)
                ? metricNames
                : GetMetricNames();
        }

        public static bool IsKnownMetric(string metricName)
        {
            return MetricDefinitions.Any(metric =>
                string.Equals(metric.Name, metricName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsMetricRecommendedForTool(string toolType, string metricName)
        {
            if (string.IsNullOrWhiteSpace(metricName))
            {
                return true;
            }

            string normalized = NormalizeToolType(toolType);
            if (!ToolMetricNames.TryGetValue(normalized, out string[] metricNames))
            {
                return true;
            }

            return metricNames.Any(metric =>
                string.Equals(metric, metricName, StringComparison.OrdinalIgnoreCase));
        }

        public static string FormatMetricListForTool(string toolType)
        {
            IReadOnlyList<string> metricNames = GetMetricNamesForTool(toolType);
            return metricNames.Count == 0 ? "(none)" : string.Join(", ", metricNames);
        }

        public static IReadOnlyList<VisionPipelineAcceptancePreset> GetPresets()
        {
            return Presets;
        }

        public static IReadOnlyList<VisionPipelineAcceptancePreset> GetPresetsForTool(string toolType)
        {
            string normalized = NormalizeToolType(toolType);
            return Presets
                .Where(preset => AppliesToTool(preset, normalized))
                .ToArray();
        }

        public static void ApplyPreset(VisionPipelineStep step, VisionPipelineAcceptancePreset preset)
        {
            if (step == null || preset == null)
            {
                return;
            }

            step.UseAcceptance = true;
            step.ExpectedSuccess = true;
            step.RequiredMessageText = string.Empty;
            step.AcceptanceMetricName = preset.MetricName;
            step.UseAcceptanceMetricMinimum = preset.UseMinimum;
            step.AcceptanceMetricMinimum = preset.Minimum;
            step.UseAcceptanceMetricMaximum = preset.UseMaximum;
            step.AcceptanceMetricMaximum = preset.Maximum;
            step.MaxElapsedMilliseconds = preset.MaxElapsedMilliseconds;
        }

        public static void ClearAcceptance(VisionPipelineStep step)
        {
            if (step == null)
            {
                return;
            }

            step.UseAcceptance = false;
            step.ExpectedSuccess = true;
            step.MaxElapsedMilliseconds = 0;
            step.RequiredMessageText = string.Empty;
            step.AcceptanceMetricName = string.Empty;
            step.UseAcceptanceMetricMinimum = false;
            step.AcceptanceMetricMinimum = 0;
            step.UseAcceptanceMetricMaximum = false;
            step.AcceptanceMetricMaximum = 0;
        }

        public static string FormatMetrics(IDictionary<string, double> metrics)
        {
            if (metrics == null || metrics.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(
                ", ",
                OrderMetrics(metrics)
                    .Select(metric => $"{metric.Key}={metric.Value:0.###}"));
        }

        public static IEnumerable<KeyValuePair<string, double>> OrderMetrics(IDictionary<string, double> metrics)
        {
            if (metrics == null)
            {
                return Enumerable.Empty<KeyValuePair<string, double>>();
            }

            Dictionary<string, int> orderMap = MetricDefinitions
                .Select((metric, index) => new { metric.Name, Index = index })
                .ToDictionary(metric => metric.Name, metric => metric.Index, StringComparer.OrdinalIgnoreCase);

            return metrics
                .Where(metric => !string.IsNullOrWhiteSpace(metric.Key))
                .OrderBy(metric => orderMap.TryGetValue(metric.Key, out int index) ? index : int.MaxValue)
                .ThenBy(metric => metric.Key);
        }

        private static bool AppliesToTool(VisionPipelineAcceptancePreset preset, string normalizedToolType)
        {
            if (preset == null)
            {
                return false;
            }

            if (preset.ToolTypes == null || preset.ToolTypes.Length == 0)
            {
                return true;
            }

            return preset.ToolTypes.Any(toolType =>
                string.Equals(NormalizeToolType(toolType), normalizedToolType, StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeToolType(string toolType)
        {
            return (toolType ?? string.Empty)
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .Trim()
                .ToLowerInvariant();
        }
    }
}
