using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using System;
using System.Globalization;

namespace OpenVisionLab.ViewModels
{
    internal sealed class EdgeBasedMatchingToolViewModel : ObservableObject, IEdgeBasedMatchingToolViewModel
    {
        private readonly EdgeBasedMatchingProperty property;

        public EdgeBasedMatchingToolViewModel(EdgeBasedMatchingProperty property)
        {
            this.property = property ?? new EdgeBasedMatchingProperty("EdgeBasedMatching");
            ConfigureDefaults();
        }

        public EdgeBasedMatchingProperty CreateProperty()
        {
            Normalize();
            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? "EdgeBasedMatching" : property.NAME;
            property.ReloadTemplateImage();
            return property.DeepCopy();
        }

        public string Summary
        {
            get
            {
                Normalize();
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "Score >= {0:0.###} / Match {1} / {2} / Edge {3}..{4} / Search {5} / Points {6} / {7}",
                    property.SCORE_MIN,
                    property.NUM_MATCH,
                    CreatePoseSummary(),
                    property.CANNY_LOW,
                    property.CANNY_HIGH,
                    CreateSearchSummary(),
                    property.MAX_TEMPLATE_POINTS,
                    VisionToolPropertySummaryViewModel.CreateRoiSummary(property));
            }
        }

        public VisionToolTemplateStatus TemplateStatus
            => VisionToolPropertySummaryViewModel.CreateTemplateStatus(property.PATTERN_PATH, property.ImageTemplate);

        public void ConfigureDefaults()
        {
            VisionToolPropertySummaryViewModel.DisableImagePreprocessDefaults(property, includeCanny: false);
        }

        public void ApplyTemplatePathForTest(string path)
        {
            property.PATTERN_PATH = path ?? string.Empty;
            property.NUM_MATCH = 1;
            property.SCORE_MIN = 0.70D;
            property.CANNY_LOW = 30;
            property.CANNY_HIGH = 90;
            property.SEARCH_STEP = 1;
            property.USE_FIND_ANGLE = false;
            property.USE_FIND_SCALE = false;
            property.USE_SUBPIXEL_REFINE = false;
            property.USE_HYBRID_VERIFY = false;
            ConfigureDefaults();
            property.ReloadTemplateImage();
        }

        public void ReloadTemplateIfPatternChanged(string propertyName)
        {
            if (string.Equals(propertyName, nameof(EdgeBasedMatchingProperty.PATTERN_PATH), StringComparison.Ordinal))
            {
                property.ReloadTemplateImage();
            }
        }

        private void Normalize()
        {
            // Edge matching is sensitive to invalid search/canny values, so normalize at the ViewModel boundary.
            property.SCORE_MIN = VisionToolPropertySummaryViewModel.ClampDouble(property.SCORE_MIN, 0D, 1D);
            property.GREEDINESS = VisionToolPropertySummaryViewModel.ClampDouble(property.GREEDINESS, 0D, 1D);
            property.NUM_MATCH = Math.Max(1, property.NUM_MATCH);
            property.SEARCH_STEP = Math.Max(1, property.SEARCH_STEP);
            property.MAX_TEMPLATE_POINTS = Math.Max(1, property.MAX_TEMPLATE_POINTS);
            property.MIN_GRADIENT_MAGNITUDE = Math.Max(0D, property.MIN_GRADIENT_MAGNITUDE);
            property.FIND_ANGLE = Math.Max(0.001D, property.FIND_ANGLE);
            property.COARSE_ANGLE_STEP = Math.Max(property.FIND_ANGLE, property.COARSE_ANGLE_STEP);
            property.COARSE_ANGLE_TOP_K = Math.Max(1, property.COARSE_ANGLE_TOP_K);
            double scaleMin = VisionToolPropertySummaryViewModel.ClampDouble(property.FIND_SCALE_MIN, 0.1D, 10D);
            double scaleMax = VisionToolPropertySummaryViewModel.ClampDouble(property.FIND_SCALE_MAX, 0.1D, 10D);
            OrderRange(ref scaleMin, ref scaleMax);
            property.FIND_SCALE_MIN = scaleMin;
            property.FIND_SCALE_MAX = scaleMax;
            property.FIND_SCALE_STEP = VisionToolPropertySummaryViewModel.ClampDouble(property.FIND_SCALE_STEP, 0.001D, 10D);
            property.HYBRID_VERIFY_TOP_N = Math.Max(1, property.HYBRID_VERIFY_TOP_N);
            property.HYBRID_VERIFY_IMAGE_WEIGHT = VisionToolPropertySummaryViewModel.ClampDouble(property.HYBRID_VERIFY_IMAGE_WEIGHT, 0D, 1D);
            property.CANNY_APERTURE_SIZE = NormalizeCannyAperture(property.CANNY_APERTURE_SIZE);

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

        private string CreateSearchSummary()
        {
            string search = property.USE_POSITION_REFINE && property.SEARCH_STEP > 1
                ? string.Format(CultureInfo.CurrentCulture, "{0}+refine", property.SEARCH_STEP)
                : property.SEARCH_STEP.ToString(CultureInfo.CurrentCulture);
            search += string.Format(CultureInfo.CurrentCulture, " / Greedy {0:0.###}", property.GREEDINESS);

            if (property.USE_PYRAMID_POSITION_PROPOSAL)
            {
                search += string.Format(
                    CultureInfo.CurrentCulture,
                    " / Pyramid top {0} min {1:0.###}",
                    property.PYRAMID_POSITION_TOP_N,
                    property.PYRAMID_POSITION_MIN_SCORE);
            }

            if (property.USE_HYBRID_VERIFY)
            {
                search += string.Format(
                    CultureInfo.CurrentCulture,
                    " / Hybrid top {0} w {1:0.###}",
                    property.HYBRID_VERIFY_TOP_N,
                    property.HYBRID_VERIFY_IMAGE_WEIGHT);
            }

            return search;
        }

        private string CreatePoseSummary()
        {
            return CreateAngleSummary() + " / " + CreateScaleSummary();
        }

        private string CreateAngleSummary()
        {
            if (!property.USE_FIND_ANGLE)
            {
                return "Angle off";
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "Angle {0:0.###} / {1}..{2} / {3}{4}",
                property.FIND_ANGLE,
                property.FIND_ANGLE_MIN,
                property.FIND_ANGLE_MAX,
                CreateAngleCandidateSummary(),
                CreateAngleSearchWarningSuffix());
        }

        private string CreateAngleCandidateSummary()
        {
            if (ShouldUseCoarseToFineAngleSearch())
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "Coarse {0:0.###} / Top {1} / Est {2}",
                    property.COARSE_ANGLE_STEP,
                    property.COARSE_ANGLE_TOP_K,
                    CalculateCoarseToFineCandidateEstimate());
            }

            return string.Format(CultureInfo.CurrentCulture, "Candidates {0}", CalculateAngleCandidateCount());
        }

        private int CalculateAngleCandidateCount()
        {
            if (!property.USE_FIND_ANGLE)
            {
                return 1;
            }

            int start = (int)Math.Ceiling(property.FIND_ANGLE_MIN / property.FIND_ANGLE);
            int end = (int)Math.Floor(property.FIND_ANGLE_MAX / property.FIND_ANGLE);
            return Math.Max(1, end - start + 1);
        }

        private string CreateAngleSearchWarningSuffix()
        {
            int candidateCount = ShouldUseCoarseToFineAngleSearch()
                ? CalculateCoarseToFineCandidateEstimate()
                : CalculateAngleCandidateCount();
            if (candidateCount >= 181)
            {
                return " / Very slow";
            }

            if (candidateCount >= 61)
            {
                return " / Slow";
            }

            return string.Empty;
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

            int start = (int)Math.Ceiling(property.FIND_SCALE_MIN / property.FIND_SCALE_STEP);
            int end = (int)Math.Floor(property.FIND_SCALE_MAX / property.FIND_SCALE_STEP);
            int count = Math.Max(1, end - start + 1);
            bool includesOne = property.FIND_SCALE_MIN <= 1D && property.FIND_SCALE_MAX >= 1D;
            bool gridContainsOne = includesOne
                && Math.Abs(Math.Round(1D / property.FIND_SCALE_STEP) * property.FIND_SCALE_STEP - 1D) < 0.000001D;
            return includesOne && !gridContainsOne ? count + 1 : count;
        }

        private int CalculateCoarseToFineCandidateEstimate()
        {
            int fullCount = CalculateAngleCandidateCount();
            int coarseCount = CalculateAngleCandidateCount(property.COARSE_ANGLE_STEP);
            int finePerCandidate = Math.Max(1, (int)Math.Ceiling(property.COARSE_ANGLE_STEP / property.FIND_ANGLE) + 1);
            return Math.Min(fullCount, coarseCount + (finePerCandidate * Math.Max(1, property.COARSE_ANGLE_TOP_K)));
        }

        private int CalculateAngleCandidateCount(double angleStep)
        {
            int start = (int)Math.Ceiling(property.FIND_ANGLE_MIN / angleStep);
            int end = (int)Math.Floor(property.FIND_ANGLE_MAX / angleStep);
            return Math.Max(1, end - start + 1);
        }

        private bool ShouldUseCoarseToFineAngleSearch()
        {
            return property.USE_FIND_ANGLE
                && property.USE_COARSE_TO_FINE_ANGLE_SEARCH
                && property.COARSE_ANGLE_STEP > property.FIND_ANGLE;
        }

        private static int NormalizeCannyAperture(int value)
        {
            if (value <= 3)
            {
                return 3;
            }

            return value <= 5 ? 5 : 7;
        }

        private static void OrderRange(ref double min, ref double max)
        {
            if (min <= max)
            {
                return;
            }

            double temp = min;
            min = max;
            max = temp;
        }
    }
}
