using OpenVisionLab.Contracts;
using System;
using System.Globalization;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class VisionToolMatchingVerificationGuidePresenter
    {
        private readonly VisionToolVerificationGuideView guideView;

        public VisionToolMatchingVerificationGuidePresenter(VisionToolVerificationGuideView guideView)
        {
            this.guideView = guideView;
        }

        public void ShowTeachingState(
            VisionToolTemplateStatus templateStatus,
            VisionToolMatchingResultReviewCriteria criteria)
        {
            if (guideView == null)
            {
                return;
            }

            VisionToolTemplateStatus resolvedStatus = templateStatus ?? new VisionToolTemplateStatus(string.Empty, false);
            string headerText = CreateHeader(criteria);
            string stateText = resolvedStatus.IsReady
                ? CreateReadyState(criteria)
                : CreateNotReadyState(criteria);
            string criteriaText = CreateCriteriaText(criteria);
            string nextActionText = resolvedStatus.IsReady
                ? CreateReadyNextAction(criteria)
                : CreateNotReadyNextAction(criteria);

            ApplyText(headerText, stateText, criteriaText, FormatNextAction(nextActionText));
            guideView.StateBrush = ResolveBrush(resolvedStatus.IsReady);
        }

        public void ShowResult(
            bool isSuccess,
            VisionToolMatchingResultReviewCriteria criteria,
            string decision,
            string reason,
            string nextAction)
        {
            if (guideView == null)
            {
                return;
            }

            string headerText = CreateHeader(criteria);
            string stateText = string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.MatchingGuide.DecisionStateFormat", "State: {0}"),
                string.IsNullOrWhiteSpace(decision) ? (isSuccess ? VisionToolVerificationText.PreviewOk : VisionToolVerificationText.PreviewNg) : decision);
            string criteriaText = CreateCriteriaText(criteria);
            string nextActionText = string.IsNullOrWhiteSpace(nextAction)
                ? CreateDefaultNextAction(isSuccess, criteria)
                : nextAction;

            ApplyText(headerText, stateText, criteriaText, FormatNextAction(nextActionText));
            guideView.StateBrush = ResolveBrush(isSuccess);
        }

        private void ApplyText(string headerText, string stateText, string criteriaText, string nextActionText)
        {
            guideView.HeaderText = headerText;
            if (guideView.IsCompactMode)
            {
                guideView.StateText = string.Format(
                    CultureInfo.CurrentCulture,
                    T("VisionTool.MatchingGuide.CompactStateFormat", "{0} / {1}"),
                    headerText,
                    stateText);
                guideView.CriteriaText = criteriaText + " / " + nextActionText;
                guideView.NextActionText = string.Empty;
                return;
            }

            guideView.StateText = stateText;
            guideView.CriteriaText = criteriaText;
            guideView.NextActionText = nextActionText;
        }

        private static string CreateHeader(VisionToolMatchingResultReviewCriteria criteria)
        {
            if (criteria.IsEdgeBasedMatching)
            {
                return T("VisionTool.MatchingGuide.EdgeHeader", "Edge match verification");
            }

            if (criteria.IsFeatureMatching)
            {
                return T("VisionTool.MatchingGuide.FeatureHeader", "Feature match verification");
            }

            return T("VisionTool.MatchingGuide.Header", "Verification flow");
        }

        private static string CreateReadyState(VisionToolMatchingResultReviewCriteria criteria)
        {
            if (criteria.IsEdgeBasedMatching)
            {
                return T("VisionTool.MatchingGuide.EdgeReadyState", "State: template ready / waiting for edge candidate verification");
            }

            if (criteria.IsFeatureMatching)
            {
                return T("VisionTool.MatchingGuide.FeatureReadyState", "State: template ready / waiting for feature candidate verification");
            }

            return T("VisionTool.MatchingGuide.ReadyState", "State: template ready / preview available");
        }

        private static string CreateNotReadyState(VisionToolMatchingResultReviewCriteria criteria)
        {
            if (criteria.IsEdgeBasedMatching)
            {
                return T("VisionTool.MatchingGuide.EdgeNotReadyState", "State: template required / register a pattern path");
            }

            if (criteria.IsFeatureMatching)
            {
                return T("VisionTool.MatchingGuide.FeatureNotReadyState", "State: feature template required / register a pattern path");
            }

            return T("VisionTool.MatchingGuide.NotReadyState", "State: template required / prepare before preview");
        }

        private static string CreateCriteriaText(VisionToolMatchingResultReviewCriteria criteria)
        {
            string criteriaSummary = criteria.CreateSummary();
            if (string.IsNullOrWhiteSpace(criteriaSummary))
            {
                if (criteria.IsEdgeBasedMatching)
                {
                    return T("VisionTool.MatchingGuide.EdgeCriteriaFallback", "Pass criteria: check edge boxes, score, and output layer");
                }

                if (criteria.IsFeatureMatching)
                {
                    return T("VisionTool.MatchingGuide.FeatureCriteriaFallback", "Pass criteria: check feature geometry and output layer");
                }

                return T("VisionTool.MatchingGuide.CriteriaFallback", "Pass criteria: check result position and overlay");
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.MatchingGuide.CriteriaFormat", "Pass criteria: {0}"),
                criteriaSummary);
        }

        private static string CreateReadyNextAction(VisionToolMatchingResultReviewCriteria criteria)
        {
            if (criteria.IsEdgeBasedMatching)
            {
                return T("VisionTool.MatchingGuide.EdgeReadyNext", "Run preview, then check edge boxes and the output layer");
            }

            if (criteria.IsFeatureMatching)
            {
                return T("VisionTool.MatchingGuide.FeatureReadyNext", "Run preview, then check feature boxes and the output layer");
            }

            return T("VisionTool.MatchingGuide.ReadyNext", "Check input/output layers and run preview");
        }

        private static string CreateNotReadyNextAction(VisionToolMatchingResultReviewCriteria criteria)
        {
            return criteria.IsFeatureMatching
                ? T("VisionTool.MatchingGuide.FeatureNotReadyNext", "Register a feature template from the pattern path, then run preview")
                : T("VisionTool.MatchingGuide.NotReadyNext", "Register a template from the pattern path, then run preview");
        }

        private static string FormatNextAction(string nextAction)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T("VisionTool.MatchingGuide.NextActionFormat", "Next: {0}"),
                string.IsNullOrWhiteSpace(nextAction) ? "-" : nextAction);
        }

        private static string CreateDefaultNextAction(
            bool isSuccess,
            VisionToolMatchingResultReviewCriteria criteria)
        {
            if (isSuccess)
            {
                if (criteria.IsEdgeBasedMatching)
                {
                    return T("VisionTool.MatchingGuide.EdgeSuccessNext", "Check edge boxes and the output layer, then add to pipeline");
                }

                if (criteria.IsFeatureMatching)
                {
                    return T("VisionTool.MatchingGuide.FeatureSuccessNext", "Check feature boxes and the output layer, then add to pipeline");
                }

                return T("VisionTool.MatchingGuide.SuccessNext", "Check result boxes and the output layer, then add to pipeline");
            }

            if (criteria.IsEdgeBasedMatching)
            {
                return T("VisionTool.MatchingGuide.EdgeFailureNext", "Adjust template ROI, Canny range, min score, and match count, then run preview again");
            }

            if (criteria.IsFeatureMatching)
            {
                return T("VisionTool.MatchingGuide.FeatureFailureNext", "Adjust feature template, ratio, and RANSAC tolerance, then run preview again");
            }

            return T("VisionTool.MatchingGuide.FailureNext", "Adjust template ROI, min score, and match count, then run preview again");
        }

        private Brush ResolveBrush(bool isSuccess)
        {
            return VisionToolResultReviewPresenter.ResolveStatusBrush(guideView, isSuccess);
        }

        private static string T(string key, string fallbackText)
        {
            return VisionToolVerificationText.T(key, fallbackText);
        }
    }
}
