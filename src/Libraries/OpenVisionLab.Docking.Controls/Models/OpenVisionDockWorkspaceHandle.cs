namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockWorkspaceHandle
    {
        public static readonly OpenVisionDockWorkspaceHandle Empty = new OpenVisionDockWorkspaceHandle(null, null);

        private OpenVisionDockWorkspaceHandle(object nativeWorkspace, object nativePrimaryPane)
        {
            NativeWorkspace = nativeWorkspace;
            NativePrimaryPane = nativePrimaryPane;
        }

        public object NativeWorkspace { get; }

        public object NativePrimaryPane { get; }

        public static OpenVisionDockWorkspaceHandle FromNative(object nativeWorkspace, object nativePrimaryPane)
        {
            return nativeWorkspace == null && nativePrimaryPane == null
                ? Empty
                : new OpenVisionDockWorkspaceHandle(nativeWorkspace, nativePrimaryPane);
        }
    }
}
