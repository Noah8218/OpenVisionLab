using OpenVisionLab.Core;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolDocumentFactory
    {
        // Thin compatibility wrapper; OpenVisionNativeToolRegistry is the single native-tool registration point.
        public static bool TryCreate(VISION_MENU menu, IDisplayManager displayManager, out OpenVisionNativeToolDocument document, bool forWorkbench = false)
        {
            return OpenVisionNativeToolRegistry.TryCreateDocument(menu, displayManager, out document, forWorkbench);
        }
    }
}
