using OpenVisionLab.Pipeline.Controls;
using OpenVisionLab.Vision2D.Pipeline;
using System.Drawing;

namespace OpenVisionLab
{
    // Owns selected-step guide/result text composition without owning View or execution state.
    internal sealed class OpenVisionPipelineReviewGuideResultProjectionOwner
    {
        internal OpenVisionPipelineReviewGuideResultProjection ProjectSelected(
            OpenVisionPipelineReviewGuideResultProjectionRequest request)
        {
            request = request ?? new OpenVisionPipelineReviewGuideResultProjectionRequest();
            OpenVisionPipelineReviewGuideState guide = OpenVisionPipelineReviewGuidePresenter.CreateSelected(
                request.DisplayIndex,
                request.StepCount,
                request.Step,
                request.StatusText,
                request.HasInputImage,
                request.HasOutputImage,
                request.Summary,
                request.ValidationResult,
                request.ExpectedInputLayer,
                request.IsBranch,
                request.InputWillBeProduced,
                request.SamplePairGuide);
            return new OpenVisionPipelineReviewGuideResultProjection(
                guide,
                OpenVisionPipelineReviewResultPresenter.FormatResultSummary(request.Summary),
                OpenVisionPipelineReviewResultPresenter.FormatResultDetails(request.Step, request.Summary),
                OpenVisionPipelineReviewResultPresenter.FormatRunLog(
                    request.Step,
                    request.InputImage,
                    request.OutputImage,
                    request.PreviewMode,
                    request.StatusText,
                    request.ValidationStatusText,
                    request.Summary),
                OpenVisionPipelineReviewResultPresenter.ResolvePairActionText(request.ActivePairCounterpartSample),
                request.ActivePairCounterpartSample?.CanOpen == true,
                OpenVisionPipelineReviewResultPresenter.ResolvePairMetricComparisonText(
                    request.Step,
                    request.Summary,
                    request.ActiveCatalogSample,
                    request.ActivePairCounterpartSample,
                    request.SamplePairGuide));
        }

        internal OpenVisionPipelineReviewGuideState ProjectValidationErrorGuide(
            int displayIndex,
            int stepCount,
            VisionPipelineStep step)
        {
            return OpenVisionPipelineReviewGuidePresenter.CreateValidationError(
                displayIndex,
                stepCount,
                step);
        }

        internal OpenVisionPipelineReviewGuideState ProjectRunningGuide(
            int displayIndex,
            int stepCount,
            VisionPipelineStep step)
        {
            return OpenVisionPipelineReviewGuidePresenter.CreateRunning(
                displayIndex,
                stepCount,
                step);
        }
    }

    internal sealed class OpenVisionPipelineReviewGuideResultProjectionRequest
    {
        internal int DisplayIndex { get; set; }

        internal int StepCount { get; set; }

        internal VisionPipelineStep Step { get; set; }

        internal string StatusText { get; set; }

        internal bool HasInputImage { get; set; }

        internal bool HasOutputImage { get; set; }

        internal VisionPipelineStepResultSummary Summary { get; set; }

        internal VisionPipelineValidationResult ValidationResult { get; set; }

        internal string ExpectedInputLayer { get; set; }

        internal bool IsBranch { get; set; }

        internal bool InputWillBeProduced { get; set; }

        internal OpenVisionWorkspaceSamplePairDecisionGuide SamplePairGuide { get; set; }

        internal VisionPipelineSampleCatalogItem ActiveCatalogSample { get; set; }

        internal VisionPipelineSampleCatalogItem ActivePairCounterpartSample { get; set; }

        internal Bitmap InputImage { get; set; }

        internal Bitmap OutputImage { get; set; }

        internal PipelineFlowPreviewMode PreviewMode { get; set; }

        internal string ValidationStatusText { get; set; }
    }

    internal sealed class OpenVisionPipelineReviewGuideResultProjection
    {
        internal OpenVisionPipelineReviewGuideResultProjection(
            OpenVisionPipelineReviewGuideState guideState,
            string resultSummaryText,
            string resultDetailText,
            string runLogText,
            string pairActionText,
            bool canOpenPairAction,
            string pairMetricText)
        {
            GuideState = guideState;
            ResultSummaryText = resultSummaryText ?? string.Empty;
            ResultDetailText = resultDetailText ?? string.Empty;
            RunLogText = runLogText ?? string.Empty;
            PairActionText = pairActionText ?? string.Empty;
            CanOpenPairAction = canOpenPairAction;
            PairMetricText = pairMetricText ?? string.Empty;
        }

        internal OpenVisionPipelineReviewGuideState GuideState { get; }

        internal string ResultSummaryText { get; }

        internal string ResultDetailText { get; }

        internal string RunLogText { get; }

        internal string PairActionText { get; }

        internal bool CanOpenPairAction { get; }

        internal string PairMetricText { get; }
    }
}
