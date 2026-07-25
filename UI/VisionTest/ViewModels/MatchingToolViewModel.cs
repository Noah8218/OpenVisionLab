using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using System;
using System.Globalization;

namespace OpenVisionLab.ViewModels
{
    internal sealed class MatchingToolViewModel : ObservableObject, IMatchingToolViewModel
    {
        private readonly MatchingProperty property;

        public MatchingToolViewModel(MatchingProperty property)
        {
            this.property = property ?? new MatchingProperty("Matching");
            ConfigureDefaults();
        }

        public MatchingProperty CreateProperty()
        {
            Normalize();
            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? "Matching" : property.NAME;
            property.ReloadTemplateImage();
            return property.DeepCopy();
        }

        public string Summary
        {
            get
            {
                Normalize();
                string angle = property.USE_FIND_ANGLE
                    ? string.Format(
                        CultureInfo.CurrentCulture,
                        "Angle {0:0.###} / {1}..{2} / {3}{4}",
                        property.FIND_ANGLE,
                        property.FIND_ANGLE_MIN,
                        property.FIND_ANGLE_MAX,
                        CreateAngleCandidateSummary(),
                        CreateAngleSearchWarningSuffix())
                    : "Angle off";
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "Score >= {0:0.###} / Match {1} / {2} / {3} / {4} / {5} / {6}",
                    property.SCORE_MIN,
                    property.NUM_MATCH,
                    angle,
                    CreateScaleSummary(),
                    CreateSearchSummary(),
                    VisionToolPropertySummaryViewModel.CreateThresholdSummary(property),
                    VisionToolPropertySummaryViewModel.CreateRoiSummary(property));
            }
        }

        public VisionToolTemplateStatus TemplateStatus
            => VisionToolPropertySummaryViewModel.CreateTemplateStatus(property.PATTERN_PATH, property.ImageTemplate);

        public void ConfigureDefaults()
        {
            VisionToolPropertySummaryViewModel.DisableImagePreprocessDefaults(property, includeCanny: true);
        }

        public void ApplyTemplatePathForTest(string path)
        {
            property.PATTERN_PATH = path ?? string.Empty;
            property.NUM_MATCH = 1;
            property.USE_FIND_ANGLE = false;
            property.USE_FIND_SCALE = false;
            property.SCORE_MIN = 0.55D;
            ConfigureDefaults();
            property.ReloadTemplateImage();
        }

        public void ReloadTemplateIfPatternChanged(string propertyName)
        {
            if (string.Equals(propertyName, nameof(MatchingProperty.PATTERN_PATH), StringComparison.Ordinal))
            {
                property.ReloadTemplateImage();
            }
        }

        private void Normalize()
        {
            // Matching range editors can be edited as text; normalize before every preview/pipeline copy.
            property.SCORE_MIN = VisionToolPropertySummaryViewModel.ClampDouble(property.SCORE_MIN, 0D, 1D);
            property.MAGNIFIATION = Math.Max(0.01D, property.MAGNIFIATION);
            property.NUM_MATCH = Math.Max(1, property.NUM_MATCH);
            double scaleMin = VisionToolPropertySummaryViewModel.ClampDouble(property.FIND_SCALE_MIN, 0.1D, 10D);
            double scaleMax = VisionToolPropertySummaryViewModel.ClampDouble(property.FIND_SCALE_MAX, 0.1D, 10D);
            VisionToolPropertySummaryViewModel.OrderRange(ref scaleMin, ref scaleMax);
            property.FIND_SCALE_MIN = scaleMin;
            property.FIND_SCALE_MAX = scaleMax;
            property.FIND_SCALE_STEP = VisionToolPropertySummaryViewModel.ClampDouble(property.FIND_SCALE_STEP, 0.001D, 10D);
            property.PYRAMID_POSITION_TOP_N = Math.Max(1, property.PYRAMID_POSITION_TOP_N);
            property.PYRAMID_POSITION_MIN_SCORE = VisionToolPropertySummaryViewModel.ClampDouble(property.PYRAMID_POSITION_MIN_SCORE, 0D, 1D);
            property.FIND_ANGLE = Math.Max(0.001D, property.FIND_ANGLE);
            property.COARSE_ANGLE_STEP = Math.Max(property.FIND_ANGLE, property.COARSE_ANGLE_STEP);
            property.COARSE_ANGLE_TOP_K = Math.Max(1, property.COARSE_ANGLE_TOP_K);
            int angleMin = property.FIND_ANGLE_MIN;
            int angleMax = property.FIND_ANGLE_MAX;
            VisionToolPropertySummaryViewModel.OrderRange(ref angleMin, ref angleMax);
            property.FIND_ANGLE_MIN = VisionToolPropertySummaryViewModel.ClampInt(angleMin, -180, 180);
            property.FIND_ANGLE_MAX = VisionToolPropertySummaryViewModel.ClampInt(angleMax, -180, 180);

            int cannyLow = VisionToolPropertySummaryViewModel.ClampInt(property.CANNY_LOW, 0, 255);
            int cannyHigh = VisionToolPropertySummaryViewModel.ClampInt(property.CANNY_HIGH, 0, 255);
            VisionToolPropertySummaryViewModel.OrderRange(ref cannyLow, ref cannyHigh);
            property.CANNY_LOW = cannyLow;
            property.CANNY_HIGH = cannyHigh;
        }

        private string CreateScaleSummary()
        {
            if (!property.USE_FIND_SCALE)
            {
                return "Scale off";
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "Scale {0:0.###}..{1:0.###} / Step {2:0.###} / Candidates {3}",
                property.FIND_SCALE_MIN,
                property.FIND_SCALE_MAX,
                property.FIND_SCALE_STEP,
                CalculateScaleCandidateCount());
        }

        private int CalculateScaleCandidateCount()
        {
            if (!property.USE_FIND_SCALE)
            {
                return 1;
            }

            int start = (int)Math.Ceiling((property.FIND_SCALE_MIN - 0.000000001D) / property.FIND_SCALE_STEP);
            int end = (int)Math.Floor((property.FIND_SCALE_MAX + 0.000000001D) / property.FIND_SCALE_STEP);
            int count = Math.Max(0, end - start + 1);
            bool includesOne = property.FIND_SCALE_MIN <= 1D && property.FIND_SCALE_MAX >= 1D;
            bool oneAlreadyOnStep = includesOne
                && Math.Abs(Math.Round(1D / property.FIND_SCALE_STEP) * property.FIND_SCALE_STEP - 1D) < 0.000001D;
            if (includesOne && !oneAlreadyOnStep)
            {
                count++;
            }

            return Math.Max(1, count);
        }

        private string CreateSearchSummary()
        {
            if (!property.USE_PYRAMID_POSITION_PROPOSAL)
            {
                return "Pyramid off";
            }

            string suffix = property.USE_FIND_ANGLE ? " / Angle fallback" : string.Empty;
            return string.Format(
                CultureInfo.CurrentCulture,
                "Pyramid Top {0} / Min {1:0.###}{2}",
                property.PYRAMID_POSITION_TOP_N,
                property.PYRAMID_POSITION_MIN_SCORE,
                suffix);
        }

        private string CreateAngleCandidateSummary()
        {
            int fullCount = CalculateAngleCandidateCount(property.FIND_ANGLE);
            if (!property.USE_COARSE_TO_FINE_ANGLE_SEARCH || property.COARSE_ANGLE_STEP <= property.FIND_ANGLE)
            {
                return string.Format(CultureInfo.CurrentCulture, "Candidates {0}", fullCount);
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "Coarse step {0:0.###} x{1} / Candidates ~{2}/{3}",
                property.COARSE_ANGLE_STEP,
                property.COARSE_ANGLE_TOP_K,
                CalculateCoarseAngleCandidateEstimate(fullCount),
                fullCount);
        }

        private int CalculateEffectiveAngleCandidateCount()
        {
            if (!property.USE_COARSE_TO_FINE_ANGLE_SEARCH || property.COARSE_ANGLE_STEP <= property.FIND_ANGLE)
            {
                return CalculateAngleCandidateCount(property.FIND_ANGLE);
            }

            int fullCount = CalculateAngleCandidateCount(property.FIND_ANGLE);
            return CalculateCoarseAngleCandidateEstimate(fullCount);
        }

        private int CalculateCoarseAngleCandidateEstimate(int fullCount)
        {
            int coarseCount = CalculateAngleCandidateCount(property.COARSE_ANGLE_STEP);
            int fineWindowCount = Math.Max(1, (int)Math.Floor((property.COARSE_ANGLE_STEP * 2D) / property.FIND_ANGLE) + 1);
            return Math.Min(fullCount, coarseCount + fineWindowCount * Math.Max(1, property.COARSE_ANGLE_TOP_K));
        }

        private int CalculateAngleCandidateCount(double angleStep)
        {
            if (!property.USE_FIND_ANGLE)
            {
                return 1;
            }

            // Keep this aligned with Lib.noah MatchingTool's angle iterators so the UI shows real execution cost.
            int start = (int)Math.Ceiling(property.FIND_ANGLE_MIN / angleStep);
            int end = (int)Math.Floor(property.FIND_ANGLE_MAX / angleStep);
            return Math.Max(1, end - start + 1);
        }

        private string CreateAngleSearchWarningSuffix()
        {
            int candidateCount = CalculateEffectiveAngleCandidateCount();
            if (candidateCount >= 1500)
            {
                return " / Very slow";
            }

            if (candidateCount >= 500)
            {
                return " / Slow";
            }

            return string.Empty;
        }
    }
}
