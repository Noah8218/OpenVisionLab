using Lib.OpenCV.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal readonly struct VisionToolMatchingResultExplanation
    {
        private VisionToolMatchingResultExplanation(
            bool isSuccess,
            string decision,
            string reason,
            string nextAction)
        {
            IsSuccess = isSuccess;
            Decision = decision ?? string.Empty;
            Reason = reason ?? string.Empty;
            NextAction = nextAction ?? string.Empty;
        }

        public bool IsSuccess { get; }
        public string Decision { get; }
        public string Reason { get; }
        public string NextAction { get; }

        public static VisionToolMatchingResultExplanation Create(
            IReadOnlyCollection<MatchingResult> matches,
            MatchingResult best,
            bool scorePassed,
            bool countPassed,
            VisionToolMatchingResultReviewCriteria criteria)
        {
            int count = matches?.Count ?? 0;
            bool isSuccess = scorePassed && countPassed;
            string decision = isSuccess ? VisionToolVerificationText.PreviewOk : VisionToolVerificationText.PreviewNg;
            string reason = isSuccess
                ? CreateSuccessReason(count, best, criteria)
                : CreateFailureReason(count, best.Score, scorePassed, countPassed, criteria);
            string nextAction = isSuccess
                ? CreateSuccessNextAction(criteria)
                : CreateFailureNextAction(criteria);

            return new VisionToolMatchingResultExplanation(isSuccess, decision, reason, nextAction);
        }

        public static VisionToolMatchingResultExplanation CreateEmpty(VisionToolMatchingResultReviewCriteria criteria)
        {
            string reason = criteria.IsEdgeBasedMatching
                ? T("VisionTool.MatchingReview.EdgeNoCandidateReason", "No edge candidate passed the current Canny/score criteria.")
                : criteria.IsFeatureMatching
                    ? T("VisionTool.MatchingReview.FeatureNoCandidateReason", "No feature candidate passed the current Ratio/RANSAC criteria.")
                    : T("VisionTool.MatchingReview.NoCandidateReason", "No candidate passed the current criteria.");

            string detail = criteria.IsEdgeBasedMatching
                ? T("VisionTool.MatchingReview.EdgeNoCandidateCause", "Cause candidates: Canny range, edge contrast, template ROI, min score, or match count.")
                : criteria.IsFeatureMatching
                    ? T("VisionTool.MatchingReview.FeatureNoCandidateCause", "Cause candidates: feature texture, template ROI, Ratio, RANSAC tolerance, or image blur.")
                    : T("VisionTool.MatchingReview.NoCandidateCause", "Cause candidates: template too large, ROI mismatch, low contrast, or min score too strict.");

            return new VisionToolMatchingResultExplanation(
                false,
                VisionToolVerificationText.PreviewNg,
                CombineReason(reason, detail),
                CreateFailureNextAction(criteria));
        }

        private static string CreateSuccessReason(
            int count,
            MatchingResult best,
            VisionToolMatchingResultReviewCriteria criteria)
        {
            List<string> parts = new List<string>();

            if (criteria.MinimumScore.HasValue)
            {
                string format = criteria.IsEdgeBasedMatching
                    ? T("VisionTool.MatchingReview.EdgeScorePassFormat", "Best edge score {0:0.###} >= {1:0.###}.")
                    : criteria.IsFeatureMatching
                        ? T("VisionTool.MatchingReview.FeatureScorePassFormat", "Feature score {0:0.###} passed.")
                        : T("VisionTool.MatchingReview.ScorePassFormat", "Best score {0:0.###} >= {1:0.###}.");
                parts.Add(string.Format(CultureInfo.CurrentCulture, format, best.Score, criteria.MinimumScore.Value));
            }
            else
            {
                parts.Add(criteria.IsFeatureMatching
                    ? T("VisionTool.MatchingReview.FeatureSuccessReason", "Feature geometry passed the Ratio/RANSAC criteria.")
                    : criteria.IsEdgeBasedMatching
                        ? T("VisionTool.MatchingReview.EdgeSuccessReason", "Edge score and count criteria passed.")
                        : T("VisionTool.MatchingReview.SuccessReason", "Score and count criteria passed."));
            }

            if (criteria.RequestedCount.HasValue)
            {
                string countFormat = T("VisionTool.MatchingReview.CountPassFormat", "Detected {0}/{1} matches.");
                parts.Add(string.Format(CultureInfo.CurrentCulture, countFormat, count, criteria.RequestedCount.Value));
            }

            if (criteria.UsesAngleSearch)
            {
                string angleFormat = T("VisionTool.MatchingReview.AngleResultFormat", "Angle search result {0:0.###} deg.");
                parts.Add(string.Format(CultureInfo.CurrentCulture, angleFormat, best.Angle));
            }

            if (criteria.UsesScaleSearch && best.Scale > 0D)
            {
                string scaleFormat = T("VisionTool.MatchingReview.ScaleResultFormat", "Scale search result {0:0.###}.");
                parts.Add(string.Format(CultureInfo.CurrentCulture, scaleFormat, best.Scale));
            }

            if (criteria.RequestedCount.HasValue && count > 1)
            {
                parts.Add(T(
                    "VisionTool.MatchingReview.MultipleCandidateRisk",
                    "Multiple candidates passed; check repeated patterns and narrow the ROI if false positives appear."));
            }

            if (criteria.UsesPyramidProposal)
            {
                parts.Add(T(
                    "VisionTool.MatchingReview.PyramidReviewHint",
                    "Pyramid proposal is a speed aid; verify final boxes on the original image."));
            }

            return string.Join(" / ", parts.Where(item => !string.IsNullOrWhiteSpace(item)));
        }

        private static string CreateFailureReason(
            int count,
            double score,
            bool scorePassed,
            bool countPassed,
            VisionToolMatchingResultReviewCriteria criteria)
        {
            if (!scorePassed && criteria.MinimumScore.HasValue)
            {
                string format = criteria.IsEdgeBasedMatching
                    ? T("VisionTool.MatchingReview.EdgeScoreFailReasonFormat", "Best edge score {0:0.###} is below the criteria {1:0.###}.")
                    : T("VisionTool.MatchingReview.ScoreFailReasonFormat", "Best score {0:0.###} is below the criteria {1:0.###}.");
                string detail = criteria.IsEdgeBasedMatching
                    ? T("VisionTool.MatchingReview.EdgeScoreFailCause", "Cause candidates: edge contrast is weak, Canny range is too narrow, or ROI contains distractors.")
                    : T("VisionTool.MatchingReview.ScoreFailCause", "Cause candidates: template is too large, lighting changed, ROI is too wide, or min score is too strict.");
                return CombineReason(string.Format(CultureInfo.CurrentCulture, format, score, criteria.MinimumScore.Value), detail);
            }

            if (!countPassed && criteria.RequestedCount.HasValue)
            {
                string format = criteria.IsEdgeBasedMatching
                    ? T("VisionTool.MatchingReview.EdgeCountFailReasonFormat", "Edge candidate count {0} is below the required {1}.")
                    : T("VisionTool.MatchingReview.CountFailReasonFormat", "Detected count {0} is below the required {1}.");
                string detail = criteria.IsEdgeBasedMatching
                    ? T("VisionTool.MatchingReview.EdgeCountFailCause", "Cause candidates: search ROI is too small, search step is too coarse, or edge model points are insufficient.")
                    : T("VisionTool.MatchingReview.CountFailCause", "Cause candidates: ROI is too small, min score is high, or template contrast is insufficient.");
                return CombineReason(string.Format(CultureInfo.CurrentCulture, format, count, criteria.RequestedCount.Value), detail);
            }

            return criteria.IsFeatureMatching
                ? T("VisionTool.MatchingReview.FeatureFailureReason", "Feature matching failed; check Ratio, RANSAC tolerance, template texture, and image blur.")
                : criteria.IsEdgeBasedMatching
                    ? T("VisionTool.MatchingReview.EdgeFailureReason", "Edge matching failed; check Canny range, edge contrast, ROI, and min score.")
                    : T("VisionTool.MatchingReview.FailureReason", "Matching failed; check template ROI, min score, contrast, and match count.");
        }

        private static string CreateSuccessNextAction(VisionToolMatchingResultReviewCriteria criteria)
        {
            if (criteria.IsEdgeBasedMatching)
            {
                return T("VisionTool.MatchingReview.EdgeSuccessNext", "Check edge boxes and the output layer, then add to pipeline");
            }

            if (criteria.IsFeatureMatching)
            {
                return T("VisionTool.MatchingReview.FeatureSuccessNext", "Check feature boxes and the output layer, then add to pipeline");
            }

            return T("VisionTool.MatchingReview.SuccessNext", "Check result boxes and the output layer, then add to pipeline");
        }

        private static string CreateFailureNextAction(VisionToolMatchingResultReviewCriteria criteria)
        {
            if (criteria.IsEdgeBasedMatching)
            {
                return T("VisionTool.MatchingReview.EdgeFailureNext", "Adjust template ROI, Canny range, min score, and match count, then run preview again");
            }

            if (criteria.IsFeatureMatching)
            {
                return T("VisionTool.MatchingReview.FeatureFailureNext", "Adjust feature template, Ratio, and RANSAC tolerance, then run preview again");
            }

            return T("VisionTool.MatchingReview.FailureNext", "Adjust template ROI, min score, and match count, then run preview again");
        }

        private static string CombineReason(string reason, string detail)
        {
            if (string.IsNullOrWhiteSpace(detail))
            {
                return reason ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return detail;
            }

            return reason + " / " + detail;
        }

        private static string T(string key, string fallbackText)
        {
            return VisionToolVerificationText.T(key, fallbackText);
        }
    }
}
