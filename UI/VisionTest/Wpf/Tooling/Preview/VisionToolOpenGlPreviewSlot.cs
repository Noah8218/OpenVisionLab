namespace OpenVisionLab
{
    public sealed class VisionToolOpenGlPreviewSlot : VisionToolInlinePreviewSlot
    {
        // Compatibility shim: new tool panels should use VisionToolInlinePreviewSlot to avoid OpenGL/HWND confusion.
        public VisionToolOpenGlPreviewSlot()
        {
        }
    }
}