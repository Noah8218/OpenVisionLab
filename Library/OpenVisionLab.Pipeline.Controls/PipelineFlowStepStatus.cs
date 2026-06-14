namespace OpenVisionLab.Pipeline.Controls
{
    public enum PipelineFlowStepStatus
    {
        Waiting,
        Running,
        Passed,
        Failed,
        Loaded,
        Skipped,
        Canceled,
        Timeout
    }
}
