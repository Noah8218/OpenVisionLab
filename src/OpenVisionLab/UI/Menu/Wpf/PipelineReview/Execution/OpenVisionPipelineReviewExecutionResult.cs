using OpenVisionLab.Vision2D.Pipeline;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewExecutionResult
    {
        public OpenVisionPipelineReviewExecutionResult(int stepResultCount, bool wasSuperseded = false)
        {
            StepResultCount = Math.Max(0, stepResultCount);
            WasSuperseded = wasSuperseded;
        }

        public int StepResultCount { get; }

        public bool WasSuperseded { get; }
    }

    internal sealed class OpenVisionPipelineReviewStepUpdatedEventArgs : EventArgs
    {
        public OpenVisionPipelineReviewStepUpdatedEventArgs(
            VisionPipelineStep step,
            VisionPipelineStepResultSummary summary,
            long inputRevision,
            long recipeRevision)
        {
            Step = step;
            Summary = summary;
            InputRevision = inputRevision;
            RecipeRevision = recipeRevision;
        }

        public VisionPipelineStep Step { get; }

        public VisionPipelineStepResultSummary Summary { get; }

        public long InputRevision { get; }

        public long RecipeRevision { get; }
    }
}
