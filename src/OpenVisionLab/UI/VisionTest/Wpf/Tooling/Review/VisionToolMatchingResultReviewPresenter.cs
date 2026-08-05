using OpenVisionLab.Vision2D.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolMatchingResultReviewPresenter
    {
        private readonly FrameworkElement owner;
        private readonly TextBlock resultReviewText;
        private readonly TextBlock resultGuidanceText;
        private readonly Panel resultReviewChips;
        private readonly VisionToolMatchingVerificationGuidePresenter verificationGuidePresenter;

        public VisionToolMatchingResultReviewPresenter(
            FrameworkElement owner,
            TextBlock resultReviewText,
            TextBlock resultGuidanceText,
            Panel resultReviewChips,
            VisionToolMatchingVerificationGuidePresenter verificationGuidePresenter = null)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.resultReviewText = resultReviewText ?? throw new ArgumentNullException(nameof(resultReviewText));
            this.resultGuidanceText = resultGuidanceText;
            this.resultReviewChips = resultReviewChips ?? throw new ArgumentNullException(nameof(resultReviewChips));
            this.verificationGuidePresenter = verificationGuidePresenter;
        }

        public void Show(
            string title,
            IEnumerable<MatchingResult> results,
            TimeSpan? tactTime = null,
            VisionToolMatchingResultReviewCriteria criteria = default)
        {
            string resolvedTitle = string.IsNullOrWhiteSpace(title) ? "Match" : title.Trim();
            List<MatchingResult> matches = results?.ToList() ?? new List<MatchingResult>();
            if (matches.Count == 0)
            {
                ShowEmptyResult(resolvedTitle, tactTime, criteria);
                return;
            }

            MatchingResult best = matches.OrderByDescending(item => item.Score).First();
            bool scorePassed = !criteria.MinimumScore.HasValue || best.Score >= criteria.MinimumScore.Value;
            bool countPassed = !criteria.RequestedCount.HasValue || matches.Count >= criteria.RequestedCount.Value;
            bool isSuccess = scorePassed && countPassed;
            double centerX = best.Center.X;
            double centerY = best.Center.Y;
            string tactSuffix = FormatTactSuffix(tactTime);
            string scaleSuffix = FormatScaleSuffix(best.Scale);
            string summary = VisionToolVerificationText.FormatMatchingSummary(
                resolvedTitle,
                matches.Count,
                best.Score,
                centerX,
                centerY,
                best.Bounding.Width,
                best.Bounding.Height,
                best.Angle,
                scaleSuffix,
                tactSuffix);

            List<VisionToolResultReviewItem> items = new List<VisionToolResultReviewItem>
            {
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.DecisionLabel, isSuccess ? VisionToolVerificationText.PreviewOk : VisionToolVerificationText.PreviewNg)
            };
            AddCriteriaItem(items, criteria);
            items.AddRange(new[]
            {
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CountLabel, matches.Count),
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.ScoreLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.000}", best.Score)),
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CenterLabel, VisionToolResultReviewPresenter.FormatPoint(centerX, centerY)),
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.BoxLabel, VisionToolResultReviewPresenter.FormatSize(best.Bounding.Width, best.Bounding.Height)),
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.AngleLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.###}", best.Angle))
            });
            if (ShouldShowScale(best.Scale))
            {
                items.Add(VisionToolResultReviewPresenter.Item(VisionToolVerificationText.ScaleLabel, string.Format(CultureInfo.CurrentCulture, "{0:0.###}", best.Scale)));
            }

            AddTactItem(items, tactTime);

            VisionToolResultReviewPresenter.Show(
                owner,
                resultReviewText,
                resultReviewChips,
                summary,
                isSuccess,
                items);

            VisionToolMatchingResultExplanation explanation = VisionToolMatchingResultExplanation.Create(
                matches,
                best,
                scorePassed,
                countPassed,
                criteria);
            ApplyGuidance(
                explanation.IsSuccess,
                criteria,
                explanation.Decision,
                explanation.Reason,
                explanation.NextAction);
            verificationGuidePresenter?.ShowResult(
                explanation.IsSuccess,
                criteria,
                explanation.Decision,
                explanation.Reason,
                explanation.NextAction);
        }

        public void Clear()
        {
            VisionToolResultReviewPresenter.Clear(owner, resultReviewText, resultReviewChips);
            if (resultGuidanceText != null)
            {
                resultGuidanceText.Text = VisionToolVerificationText.PreviewNotRunCurrentRoute;
                resultGuidanceText.ToolTip = resultGuidanceText.Text;
            }
        }

        private void ShowEmptyResult(
            string resolvedTitle,
            TimeSpan? tactTime,
            VisionToolMatchingResultReviewCriteria criteria)
        {
            List<VisionToolResultReviewItem> emptyItems = new List<VisionToolResultReviewItem>
            {
                VisionToolResultReviewPresenter.Item(VisionToolVerificationText.DecisionLabel, VisionToolVerificationText.PreviewNg)
            };
            AddCriteriaItem(emptyItems, criteria);
            emptyItems.Add(VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CountLabel, 0));
            emptyItems.Add(VisionToolResultReviewPresenter.Item(VisionToolVerificationText.StateLabel, VisionToolVerificationText.NoMatch));
            AddTactItem(emptyItems, tactTime);

            VisionToolResultReviewPresenter.Show(
                owner,
                resultReviewText,
                resultReviewChips,
                VisionToolVerificationText.FormatMatchingEmptySummary(resolvedTitle),
                false,
                emptyItems);

            VisionToolMatchingResultExplanation explanation = VisionToolMatchingResultExplanation.CreateEmpty(criteria);
            ApplyGuidance(
                false,
                criteria,
                explanation.Decision,
                explanation.Reason,
                explanation.NextAction);
            verificationGuidePresenter?.ShowResult(
                false,
                criteria,
                explanation.Decision,
                explanation.Reason,
                explanation.NextAction);
        }

        private void ApplyGuidance(
            bool isSuccess,
            VisionToolMatchingResultReviewCriteria criteria,
            string decision,
            string reason,
            string nextAction)
        {
            if (resultGuidanceText == null)
            {
                return;
            }

            string criteriaText = criteria.CreateSummary();
            string guidance = string.IsNullOrWhiteSpace(criteriaText)
                ? decision + " / " + reason + " / " + VisionToolVerificationText.FormatNextAction(nextAction)
                : VisionToolVerificationText.FormatResultGuidance(decision, criteriaText, reason, nextAction);
            resultGuidanceText.Text = guidance;
            resultGuidanceText.ToolTip = guidance;
            resultGuidanceText.Foreground = VisionToolResultReviewPresenter.ResolveStatusBrush(owner, isSuccess);
        }

        private static void AddTactItem(ICollection<VisionToolResultReviewItem> items, TimeSpan? tactTime)
        {
            if (!tactTime.HasValue || tactTime.Value < TimeSpan.Zero)
            {
                return;
            }

            items.Add(VisionToolResultReviewPresenter.Item(VisionToolVerificationText.TactLabel, FormatTact(tactTime.Value)));
        }

        private static void AddCriteriaItem(
            ICollection<VisionToolResultReviewItem> items,
            VisionToolMatchingResultReviewCriteria criteria)
        {
            string criteriaText = criteria.CreateCompactSummary();
            if (!string.IsNullOrWhiteSpace(criteriaText))
            {
                items.Add(VisionToolResultReviewPresenter.Item(VisionToolVerificationText.CriteriaLabel, criteriaText));
            }
        }

        private static string FormatTactSuffix(TimeSpan? tactTime)
        {
            return tactTime.HasValue && tactTime.Value >= TimeSpan.Zero
                ? " / " + VisionToolVerificationText.TactLabel + " " + FormatTact(tactTime.Value)
                : string.Empty;
        }

        private static string FormatScaleSuffix(double scale)
        {
            return ShouldShowScale(scale)
                ? string.Format(CultureInfo.CurrentCulture, " / {0} {1:0.###}", VisionToolVerificationText.ScaleLabel, scale)
                : string.Empty;
        }

        private static bool ShouldShowScale(double scale)
        {
            return scale > 0D && Math.Abs(scale - 1D) > 0.000001D;
        }

        private static string FormatTact(TimeSpan tactTime)
        {
            return tactTime.TotalMilliseconds < 1000D
                ? string.Format(CultureInfo.CurrentCulture, "{0:0.0} ms", tactTime.TotalMilliseconds)
                : string.Format(CultureInfo.CurrentCulture, "{0:0.000} s", tactTime.TotalSeconds);
        }

    }

    internal enum VisionToolMatchingVerificationKind
    {
        TemplateMatching,
        EdgeBasedMatching,
        FeatureMatching
    }

    internal readonly struct VisionToolMatchingResultReviewCriteria
    {
        public VisionToolMatchingResultReviewCriteria(
            double? minimumScore,
            int? requestedCount,
            bool usesAngleSearch,
            bool usesScaleSearch,
            bool usesPyramidProposal,
            string thresholdLabel)
        {
            MinimumScore = minimumScore;
            RequestedCount = requestedCount;
            UsesAngleSearch = usesAngleSearch;
            UsesScaleSearch = usesScaleSearch;
            UsesPyramidProposal = usesPyramidProposal;
            ThresholdLabel = ResolveThresholdLabel(thresholdLabel);
            VerificationKind = VisionToolMatchingVerificationKind.TemplateMatching;
            EdgeRangeText = string.Empty;
            SearchText = string.Empty;
            MaxTemplatePoints = null;
            AdditionalCriteriaText = string.Empty;
        }

        public VisionToolMatchingResultReviewCriteria(
            double? minimumScore,
            int? requestedCount,
            bool usesAngleSearch,
            bool usesScaleSearch,
            bool usesPyramidProposal,
            string thresholdLabel,
            VisionToolMatchingVerificationKind verificationKind,
            string edgeRangeText,
            string searchText,
            int? maxTemplatePoints,
            string additionalCriteriaText = null)
            : this(
                minimumScore,
                requestedCount,
                usesAngleSearch,
                usesScaleSearch,
                usesPyramidProposal,
                thresholdLabel)
        {
            VerificationKind = verificationKind;
            EdgeRangeText = edgeRangeText ?? string.Empty;
            SearchText = searchText ?? string.Empty;
            MaxTemplatePoints = maxTemplatePoints;
            AdditionalCriteriaText = additionalCriteriaText ?? string.Empty;
        }

        public double? MinimumScore { get; }
        public int? RequestedCount { get; }
        public bool UsesAngleSearch { get; }
        public bool UsesScaleSearch { get; }
        public bool UsesPyramidProposal { get; }
        public string ThresholdLabel { get; }
        public VisionToolMatchingVerificationKind VerificationKind { get; }
        public string EdgeRangeText { get; }
        public string SearchText { get; }
        public int? MaxTemplatePoints { get; }
        public string AdditionalCriteriaText { get; }
        public bool IsEdgeBasedMatching => VerificationKind == VisionToolMatchingVerificationKind.EdgeBasedMatching;
        public bool IsFeatureMatching => VerificationKind == VisionToolMatchingVerificationKind.FeatureMatching;

        private static string ResolveThresholdLabel(string thresholdLabel)
        {
            string resolved = string.IsNullOrWhiteSpace(thresholdLabel) ? "Score >=" : thresholdLabel.Trim();
            return string.Equals(resolved, "Score >=", StringComparison.OrdinalIgnoreCase)
                ? VisionToolVerificationText.T("VisionTool.Review.ScoreThresholdLabel", "Score >=")
                : resolved;
        }

        public string CreateSummary()
        {
            if (!MinimumScore.HasValue
                && !RequestedCount.HasValue
                && !UsesAngleSearch
                && !UsesScaleSearch
                && !UsesPyramidProposal
                && string.IsNullOrWhiteSpace(EdgeRangeText)
                && string.IsNullOrWhiteSpace(SearchText)
                && !MaxTemplatePoints.HasValue
                && string.IsNullOrWhiteSpace(AdditionalCriteriaText))
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            if (MinimumScore.HasValue)
            {
                parts.Add(string.Format(CultureInfo.CurrentCulture, "{0} {1:0.###}", ThresholdLabel, MinimumScore.Value));
            }

            if (RequestedCount.HasValue)
            {
                parts.Add(VisionToolVerificationText.FormatMatchCountCriteria(RequestedCount.Value));
            }

            if (MinimumScore.HasValue || RequestedCount.HasValue || UsesAngleSearch)
            {
                parts.Add(UsesAngleSearch ? VisionToolVerificationText.AngleOn : VisionToolVerificationText.AngleOff);
            }

            if (MinimumScore.HasValue || RequestedCount.HasValue || UsesScaleSearch)
            {
                parts.Add(UsesScaleSearch ? VisionToolVerificationText.ScaleOn : VisionToolVerificationText.ScaleOff);
            }

            if (UsesPyramidProposal)
            {
                parts.Add(VisionToolVerificationText.PyramidOn);
            }

            if (!string.IsNullOrWhiteSpace(EdgeRangeText))
            {
                parts.Add(VisionToolVerificationText.FormatCannyCriteria(EdgeRangeText));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                parts.Add(VisionToolVerificationText.FormatSearchCriteria(SearchText));
            }

            if (MaxTemplatePoints.HasValue)
            {
                parts.Add(VisionToolVerificationText.FormatPointsCriteria(MaxTemplatePoints.Value));
            }

            if (!string.IsNullOrWhiteSpace(AdditionalCriteriaText))
            {
                parts.Add(AdditionalCriteriaText);
            }

            return string.Join(" / ", parts);
        }

        public string CreateCompactSummary()
        {
            List<string> parts = new List<string>();
            if (MinimumScore.HasValue)
            {
                parts.Add(string.Format(CultureInfo.CurrentCulture, "{0} {1:0.###}", ThresholdLabel, MinimumScore.Value));
            }

            if (RequestedCount.HasValue)
            {
                parts.Add(VisionToolVerificationText.FormatMatchCountCriteria(RequestedCount.Value));
            }

            if (!string.IsNullOrWhiteSpace(AdditionalCriteriaText))
            {
                parts.Add(AdditionalCriteriaText);
            }

            return string.Join(" / ", parts);
        }
    }
}
