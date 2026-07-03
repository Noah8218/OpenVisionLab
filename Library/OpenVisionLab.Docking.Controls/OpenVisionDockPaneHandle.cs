namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockPaneHandle
    {
        public static readonly OpenVisionDockPaneHandle Empty = new OpenVisionDockPaneHandle(null);

        private OpenVisionDockPaneHandle(object nativePane)
        {
            NativePane = nativePane;
        }

        public object NativePane { get; }

        public bool HasPane => NativePane != null;

        public static OpenVisionDockPaneHandle FromNative(object nativePane)
        {
            return nativePane == null ? Empty : new OpenVisionDockPaneHandle(nativePane);
        }
    }
}
