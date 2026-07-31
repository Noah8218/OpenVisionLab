using System;

namespace OpenVisionLab.Pipeline.Controls
{
    public sealed class PipelineFlowStepSelectedEventArgs : EventArgs
    {
        public PipelineFlowStepSelectedEventArgs(int index)
            : this(index, PipelineFlowPreviewMode.Overlay)
        {
        }

        public PipelineFlowStepSelectedEventArgs(int index, PipelineFlowPreviewMode mode)
        {
            Index = index;
            Mode = mode;
        }

        public int Index { get; }

        public PipelineFlowPreviewMode Mode { get; }
    }
}
