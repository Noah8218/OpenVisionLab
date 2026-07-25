using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolLayerStateChangedEventArgs : EventArgs
    {
        public OpenVisionNativeToolLayerStateChangedEventArgs(bool showOutputWorkspacePreview)
        {
            ShowOutputWorkspacePreview = showOutputWorkspacePreview;
        }

        public bool ShowOutputWorkspacePreview { get; }
    }
}
