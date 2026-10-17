using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    draftPipeline),
                (draftPipeline?.Steps ?? new List<VisionPipelineStep>()).Select((step, index) => new KeyValuePair<OpenVisionRecipePipelineStepPreview, string>(
                    new OpenVisionRecipePipelineStepPreview(index + 1, step, null),
                    OpenVisionRecipePipelineComparisonPresenter.FormatDetailedStepDiff(index < (activePipeline?.Steps?.Count ?? 0) ? activePipeline.Steps[index] : null, step, showUnchanged: true))).ToList());
        }
    }

    internal sealed class OpenVisionRecipeLlmDraftReview
    {
        internal OpenVisionRecipeLlmDraftReview(
            string importReviewText,
            string diffReviewText,
            IReadOnlyList<KeyValuePair<OpenVisionRecipePipelineStepPreview, string>> steps = null)
        {
            ImportReviewText = importReviewText ?? string.Empty;
            DiffReviewText = diffReviewText ?? string.Empty;
            Steps = steps ?? Array.Empty<KeyValuePair<OpenVisionRecipePipelineStepPreview, string>>();
        }

        internal IReadOnlyList<KeyValuePair<OpenVisionRecipePipelineStepPreview, string>> Steps { get; }

        internal string ImportReviewText { get; }

        internal string DiffReviewText { get; }
    }
}
