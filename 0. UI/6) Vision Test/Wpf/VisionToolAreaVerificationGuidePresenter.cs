using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolAreaVerificationGuidePresenter<TProperty, TResult>
        where TResult : class
    {
        private readonly VisionToolVerificationGuideView guideView;
        private readonly TextBlock resultGuidanceText;
        private readonly string toolName;
        private readonly Func<TProperty, string> createCriteriaText;
        private readonly Func<TResult, double> getArea;
        private readonly Func<TResult, double> getBoxWidth;
        private readonly Func<TResult, double> getBoxHeight;

        public VisionToolAreaVerificationGuidePresenter(
            VisionToolVerificationGuideView guideView,
            TextBlock resultGuidanceText,
            string toolName,
            Func<TProperty, string> createCriteriaText,
            Func<TResult, double> getArea,
            Func<TResult, double> getBoxWidth = null,
            Func<TResult, double> getBoxHeight = null)
        {
            this.guideView = guideView ?? throw new ArgumentNullException(nameof(guideView));
            this.resultGuidanceText = resultGuidanceText;
            this.toolName = string.IsNullOrWhiteSpace(toolName) ? "Area" : toolName.Trim();
            this.createCriteriaText = createCriteriaText ?? throw new ArgumentNullException(nameof(createCriteriaText));
            this.getArea = getArea ?? throw new ArgumentNullException(nameof(getArea));
            this.getBoxWidth = getBoxWidth;
            this.getBoxHeight = getBoxHeight;
        }

        public void ShowTeachingState(TProperty property)
        {
            ApplyGuide(
                isSuccess: false,
                stateText: VisionToolVerificationText.FormatTeachingState(toolName) + " / " + VisionToolVerificationText.PreviewNotRun,
                criteriaText: CreateCriteriaText(property),
                nextActionText: VisionToolVerificationText.RunPreview);
        }

        public void ShowResult(IEnumerable<TResult> results, TProperty property)
        {
            List<TResult> items = results?.Where(item => item != null).ToList() ?? new List<TResult>();
            VisionToolAreaResultExplanation explanation = VisionToolAreaResultExplanation.Create(
                toolName,
                items,
                getArea,
                getBoxWidth,
                getBoxHeight);
            string criteriaText = CreateCriteriaText(property);

            ApplyGuide(
                explanation.IsSuccess,
                VisionToolVerificationText.FormatResultState(toolName, explanation.Decision),
                criteriaText,
                explanation.NextAction);
            ApplyResultGuidance(
                explanation.IsSuccess,
                VisionToolVerificationText.FormatResultGuidance(
                    explanation.Decision,
                    criteriaText,
                    explanation.Reason,
                    explanation.NextAction));
        }

        private string CreateCriteriaText(TProperty property)
        {
            return createCriteriaText(property) ?? string.Empty;
        }

        private void ApplyGuide(
            bool isSuccess,
            string stateText,
            string criteriaText,
            string nextActionText)
        {
            string headerText = VisionToolVerificationText.FormatVerificationHeader(toolName);
            guideView.HeaderText = headerText;
            if (guideView.IsCompactMode)
            {
                guideView.StateText = headerText + " / " + stateText;
                guideView.CriteriaText = criteriaText + " / " + VisionToolVerificationText.FormatNextAction(nextActionText);
                guideView.NextActionText = string.Empty;
            }
            else
            {
                guideView.StateText = stateText;
                guideView.CriteriaText = criteriaText;
                guideView.NextActionText = nextActionText;
            }

            guideView.StateBrush = VisionToolResultReviewPresenter.ResolveStatusBrush(guideView, isSuccess);
        }

        private void ApplyResultGuidance(bool isSuccess, string guidance)
        {
            if (resultGuidanceText == null)
            {
                return;
            }

            resultGuidanceText.Text = guidance;
            resultGuidanceText.ToolTip = guidance;
            resultGuidanceText.Foreground = VisionToolResultReviewPresenter.ResolveStatusBrush(guideView, isSuccess);
        }
    }
}
