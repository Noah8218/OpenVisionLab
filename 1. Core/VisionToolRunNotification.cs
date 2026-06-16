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
        public int OverlayCount { get; set; }
        public int MetricCount { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorName { get; set; } = string.Empty;
        public string ResultStatus { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success => Status == VisionToolRunStatus.Completed;
        public bool HasToolError => ErrorCode != 0;
    }
}
