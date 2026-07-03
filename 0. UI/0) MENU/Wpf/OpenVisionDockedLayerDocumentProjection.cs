using OpenVisionLab.Docking.Controls;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal static class OpenVisionDockedLayerDocumentProjection
    {
        public static OpenVisionDockedLayerDocumentState CreateDocumentState(OpenVisionDockDocumentState document)
        {
            IOpenVisionDockedLayerViewer viewer = document.Content as IOpenVisionDockedLayerViewer;
            return new OpenVisionDockedLayerDocumentState(
                document.ContentId,
                document.CanFloat,
                viewer?.TextureTileCount ?? 0,
                viewer?.IsCompactSizeReady == true,
                viewer?.IsCompactChrome == true);
        }

        public static OpenVisionDockedLayerWorkspaceState CreateWorkspaceState(
            IReadOnlyList<OpenVisionDockedLayerDocumentState> documents,
            int paneCount,
            string rootOrientationName,
            int nestedLayoutPanelCount)
        {
            return new OpenVisionDockedLayerWorkspaceState(
                documents,
                paneCount,
                rootOrientationName,
                nestedLayoutPanelCount);
        }
    }
}
