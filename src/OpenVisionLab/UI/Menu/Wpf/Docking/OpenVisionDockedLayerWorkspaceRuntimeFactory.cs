using OpenVisionLab.Docking.Controls;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionDockedLayerWorkspaceRuntimeFactory
    {
        public static ShellDockedLayerWorkspaceComposition CreateComposition(
            OpenVisionDockedLayerWorkspaceRuntimeOptions options)
        {
            return new ShellDockedLayerWorkspaceComposition(Create(options));
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

            return new OpenVisionDockedLayerWorkspaceRuntime(
                options.WorkspaceView,
                content.ViewModel,
                orchestrator,
                options.ActivateLayer);
        }
    }
}
