using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class LineToolReviewController
    {
        private readonly VisionToolSingleInputSpecialPropertyToolController toolController;
        private readonly LineToolInteractionController interactionController;
        private readonly LineToolResultReviewPresenter resultReviewPresenter;
        private readonly LineToolVerificationGuidePresenter verificationGuidePresenter;
        private readonly LineToolTextPresenter textPresenter;

        public LineToolReviewController(
            VisionToolSingleInputSpecialPropertyToolController toolController,
            LineToolInteractionController interactionController,
            LineToolResultReviewPresenter resultReviewPresenter,
            LineToolVerificationGuidePresenter verificationGuidePresenter,
            LineToolTextPresenter textPresenter)
        {
            this.toolController = toolController ?? throw new ArgumentNullException(nameof(toolController));
            this.interactionController = interactionController ?? throw new ArgumentNullException(nameof(interactionController));
            this.resultReviewPresenter = resultReviewPresenter ?? throw new ArgumentNullException(nameof(resultReviewPresenter));
            this.verificationGuidePresenter = verificationGuidePresenter ?? throw new ArgumentNullException(nameof(verificationGuidePresenter));
            this.textPresenter = textPresenter ?? throw new ArgumentNullException(nameof(textPresenter));
        }

        public void ShowLineResult(IEnumerable<LineGaugeResult> results)
        {
            List<LineGaugeResult> resultList = results?.Where(item => item != null).ToList() ?? new List<LineGaugeResult>();
            resultReviewPresenter.Show(resultList);
            verificationGuidePresenter.ShowLineResult(
                resultList,
                interactionController.SelectedPurpose,
                interactionController.SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void ShowDistanceResult(VisionToolResult result)
        {
            resultReviewPresenter.ShowDistance(result);
            verificationGuidePresenter.ShowDistanceResult(
                result,
                interactionController.SelectedPurpose,
                interactionController.SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void ShowIntersectionResult(LineGaugeTool lineA, LineGaugeTool lineB, OpenCvSharp.Point intersectionPoint)
        {
            resultReviewPresenter.ShowIntersection(lineA, lineB, intersectionPoint);
            verificationGuidePresenter.ShowIntersectionResult(
                true,
                interactionController.SelectedPurpose,
                interactionController.SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void ShowIntersectionResult(VisionToolResult result)
        {
            resultReviewPresenter.ShowIntersection(result);
            bool crosses = result?.Success == true
                && result.Metrics != null
                && result.Metrics.TryGetValue("IntersectionCross", out double crossValue)
                && crossValue >= 0.5D;
            verificationGuidePresenter.ShowIntersectionResult(
                crosses,
                interactionController.SelectedPurpose,
                interactionController.SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void RefreshTeachingSummary()
        {
            if (!string.IsNullOrWhiteSpace(toolController.ResultReviewText.Text))
            {
                return;
            }

            textPresenter.RefreshSummary(
                interactionController.SelectedPurpose,
                interactionController.IsLineBSelected,
                interactionController.SelectedLineName);
            verificationGuidePresenter.ShowTeachingState(
                interactionController.SelectedPurpose,
                interactionController.SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void ClearResultReview()
        {
            toolController.ClearResultReview();
            verificationGuidePresenter.ShowTeachingState(
                interactionController.SelectedPurpose,
                interactionController.SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }
    }
}
