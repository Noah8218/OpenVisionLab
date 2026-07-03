using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class LineToolVerificationGuidePresenter
    {
        private readonly TextBlock guideText;
        private readonly TextBlock resultGuidanceText;

        public LineToolVerificationGuidePresenter(
            TextBlock guideText,
            TextBlock resultGuidanceText)
        {
            this.guideText = guideText ?? throw new ArgumentNullException(nameof(guideText));
            this.resultGuidanceText = resultGuidanceText;
        }

        public void ShowTeachingState(LineToolPurpose purpose, string lineName, LineGaugeProperty property)
        {
            ApplyGuide(
                isSuccess: false,
                stateText: VisionToolVerificationText.PreviewNotRun,
                criteriaText: CreateCriteriaText(purpose, lineName, property),
                nextActionText: VisionToolVerificationText.RunPreview);
        }

        public void ShowLineResult(
            IEnumerable<LineGaugeResult> results,
            LineToolPurpose purpose,
            string lineName,
            LineGaugeProperty property)
        {
            ShowResult(
                LineToolResultExplanation.CreateLineResult(results, purpose),
                purpose,
                lineName,
                property);
        }

        public void ShowDistanceResult(VisionToolResult result, LineToolPurpose purpose, string lineName, LineGaugeProperty property)
        {
            ShowResult(
                LineToolResultExplanation.CreateDistanceResult(result),
                purpose,
                lineName,
                property);
        }

        public void ShowIntersectionResult(bool crosses, LineToolPurpose purpose, string lineName, LineGaugeProperty property)
        {
            ShowResult(
                LineToolResultExplanation.CreateIntersectionResult(crosses),
                purpose,
                lineName,
                property);
        }

        private void ShowResult(
            LineToolResultExplanation explanation,
            LineToolPurpose purpose,
            string lineName,
            LineGaugeProperty property)
        {
            string criteriaText = CreateCriteriaText(purpose, lineName, property);
            ApplyGuide(
                explanation.IsSuccess,
                explanation.Decision,
                criteriaText,
                explanation.GuideNextAction);
            ApplyResultGuidance(
                explanation.IsSuccess,
                VisionToolVerificationText.FormatResultGuidance(
                    explanation.Decision,
                    criteriaText,
                    explanation.Reason,
                    explanation.ResultNextAction));
        }

        private void ApplyGuide(bool isSuccess, string stateText, string criteriaText, string nextActionText)
        {
            string guide = VisionToolVerificationText.FormatCompactGuide(
                VisionToolVerificationText.FormatVerificationHeader("Line"),
                stateText,
                criteriaText,
                nextActionText);
            guideText.Text = guide;
            guideText.ToolTip = guide;
            guideText.Foreground = VisionToolResultReviewPresenter.ResolveStatusBrush(guideText, isSuccess);
        }

        private void ApplyResultGuidance(bool isSuccess, string guidance)
        {
            if (resultGuidanceText == null)
            {
                return;
            }

            resultGuidanceText.Text = guidance;
            resultGuidanceText.ToolTip = guidance;
            resultGuidanceText.Foreground = VisionToolResultReviewPresenter.ResolveStatusBrush(guideText, isSuccess);
        }

        private static string CreateCriteriaText(LineToolPurpose purpose, string lineName, LineGaugeProperty property)
        {
            LineGaugeProperty resolvedProperty = property ?? new LineGaugeProperty();
            string roiText = resolvedProperty.USE_ROI ? VisionToolVerificationText.RoiOn : VisionToolVerificationText.FullImage;
            return VisionToolVerificationText.FormatLineCriteria(
                VisionToolVerificationText.CreateLinePurposeText(purpose.ToString()),
                string.IsNullOrWhiteSpace(lineName) ? "Line" : lineName,
                resolvedProperty.CONTRAST,
                resolvedProperty.SAMPLING_STEP,
                roiText);
        }
    }
}
