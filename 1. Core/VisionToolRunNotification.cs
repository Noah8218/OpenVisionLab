using System;

namespace OpenVisionLab
{
    public enum VisionToolRunStatus
    {
        Started,
        Completed,
        Failed
    }

    public sealed class VisionToolRunEventArgs : EventArgs
    {
        public VisionToolRunStatus Status { get; set; }
        public string ToolName { get; set; } = string.Empty;
        public string SourceLayer { get; set; } = string.Empty;
        public string OutputLayer { get; set; } = string.Empty;
        public double ElapsedMilliseconds { get; set; }
        public int ResultWidth { get; set; }
        public int ResultHeight { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success => Status == VisionToolRunStatus.Completed;
    }
}
