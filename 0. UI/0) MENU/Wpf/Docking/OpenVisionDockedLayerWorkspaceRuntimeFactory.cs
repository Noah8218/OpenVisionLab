using OpenVisionLab.Docking.Controls;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionDockedLayerWorkspaceRuntimeFactory
    {
        public static OpenVisionShellHostDockedLayerWorkspaceComposition CreateComposition(
            OpenVisionDockedLayerWorkspaceRuntimeOptions options)
        {
            return new OpenVisionShellHostDockedLayerWorkspaceComposition(Create(options));
        }

        public static IOpenVisionDockedLayerWorkspace Create(OpenVisionDockedLayerWorkspaceRuntimeOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            OpenVisionDockedLayerContentComposition content = OpenVisionDockedLayerContentComposition.Create(options);
            OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState> workspaceComposition =
                OpenVisionDockedLayerWorkspaceCompositionFactory.Create(options, content);
            OpenVisionShellHostDockedLayerOrchestrator orchestrator = new OpenVisionShellHostDockedLayerOrchestrator(workspaceComposition);

            return new OpenVisionDockedLayerWorkspaceRuntime(options.WorkspaceView, content.ViewModel, orchestrator);
        }
    }
}
