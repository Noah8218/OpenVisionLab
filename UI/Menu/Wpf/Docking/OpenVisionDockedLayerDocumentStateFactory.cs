using OpenVisionLab.Docking.Controls;
using System.IO;

namespace OpenVisionLab
{
    internal static class OpenVisionDockedLayerDocumentStateFactory
    {
        public static IOpenVisionDockDocumentState Create()
        {
            string directory = AppPathService.EnsureDirectory("CONFIG", "UI");
            return new OpenVisionDockDocumentStateController(
                new OpenVisionDockDocumentStateStore(
                    Path.Combine(directory, "LayerDocking.layers"),
                    Path.Combine(directory, "LayerDocking.layout")));
        }
    }
}
