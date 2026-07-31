using Lib.OpenCV.Pipeline;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewExecutionResult
    {
        public OpenVisionPipelineReviewExecutionResult(int stepResultCount)
        {
            StepResultCount = Math.Max(0, stepResultCount);
        }

        public int StepResultCount { get; }
    }

    internal sealed class OpenVisionPipelineReviewStepUpdatedEventArgs : EventArgs
    {
        public OpenVisionPipelineReviewStepUpdatedEventArgs(
            VisionPipelineStep step,
            VisionPipelineStepResultSummary summary)
        {
            Step = step;
            Summary = summary;
        }

        public VisionPipelineStep Step { get; }

        public VisionPipelineStepResultSummary Summary { get; }
    }
}
