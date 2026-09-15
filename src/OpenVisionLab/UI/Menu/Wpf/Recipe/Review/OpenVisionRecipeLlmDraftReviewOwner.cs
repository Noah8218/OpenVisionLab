using OpenVisionLab.Vision2D.Pipeline;
using System;

namespace OpenVisionLab
{
    // Owns the persisted active-Pipeline lookup used by LLM draft review.
    // It returns read-only comparison text and does not change Recipe state.
    internal sealed class OpenVisionRecipeLlmDraftReviewOwner
    {
        internal OpenVisionRecipeLlmDraftReview Build(
            string recipeName,
            VisionPipeline draftPipeline)
        {
            string normalizedRecipeName = string.IsNullOrWhiteSpace(recipeName)
                ? "Default"
                : recipeName.Trim();
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                normalizedRecipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            VisionPipeline activePipeline = VisionPipelineStorage.Load(
                normalizedRecipeName,
                activePipelineName);
            return new OpenVisionRecipeLlmDraftReview(
                OpenVisionRecipePipelineComparisonPresenter.BuildDraftImportReview(
                    activePipeline,
                    draftPipeline),
                OpenVisionRecipePipelineComparisonPresenter.BuildDraftDiffReview(
                    activePipeline,
                    draftPipeline));
        }
    }

    internal sealed class OpenVisionRecipeLlmDraftReview
    {
        internal OpenVisionRecipeLlmDraftReview(
            string importReviewText,
            string diffReviewText)
        {
            ImportReviewText = importReviewText ?? string.Empty;
            DiffReviewText = diffReviewText ?? string.Empty;
        }

        internal string ImportReviewText { get; }

        internal string DiffReviewText { get; }
    }
}
