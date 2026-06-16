namespace OpenVisionLab.Pipeline.Controls
{
    public enum PipelineFlowStepStatus
    {
        Waiting,
        Running,
        Passed,
        Failed,
        Error,
        Loaded,
        Skipped,
        Canceled,
        Timeout
    }
}
