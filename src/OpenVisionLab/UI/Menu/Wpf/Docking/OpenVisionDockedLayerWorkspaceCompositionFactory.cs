using OpenVisionLab.Docking.Controls;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionDockedLayerWorkspaceCompositionFactory
    {
        public static OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState> Create(
            OpenVisionDockedLayerWorkspaceRuntimeOptions options,
            OpenVisionDockedLayerContentComposition content)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState>.Create(
                new OpenVisionDockWorkspaceCompositionOptions<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState>(
                    options.WorkspaceView,
                    content.DocumentState,
                    content.ContentSource,
                    content.DocumentContentPredicate,
                    OpenVisionDockedLayerDocumentProjection.CreateDocumentState,
                    OpenVisionDockedLayerDocumentProjection.CreateWorkspaceState,
                    options.CanOpenLayer,
                    options.IsLoadedProvider,
                    options.ApplyRefreshResult,
                    options.RefreshLayerActions));
        }
    }
}
