using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerWorkspaceState
    {
        public static readonly OpenVisionDockedLayerWorkspaceState Empty =
            new OpenVisionDockedLayerWorkspaceState(
                Enumerable.Empty<OpenVisionDockedLayerDocumentState>(),
                0,
                string.Empty,
                0);

        public OpenVisionDockedLayerWorkspaceState(
            IEnumerable<OpenVisionDockedLayerDocumentState> documents,
            int paneCount,
            string rootOrientationName,
            int nestedLayoutPanelCount)
        {
            Documents = (documents ?? Enumerable.Empty<OpenVisionDockedLayerDocumentState>()).ToList();
            PaneCount = paneCount;
            RootOrientationName = rootOrientationName ?? string.Empty;
            NestedLayoutPanelCount = nestedLayoutPanelCount;
        }

        public IReadOnlyList<OpenVisionDockedLayerDocumentState> Documents { get; }

        public int PaneCount { get; }

        public string RootOrientationName { get; }

        public int NestedLayoutPanelCount { get; }
    }
}
