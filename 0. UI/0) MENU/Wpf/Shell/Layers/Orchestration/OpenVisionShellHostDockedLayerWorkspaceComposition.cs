using OpenVisionLab.Docking.Controls;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostDockedLayerWorkspaceComposition
    {
        public OpenVisionShellHostDockedLayerWorkspaceComposition(IOpenVisionDockedLayerWorkspace workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }

            Commands = workspace;
            Synchronization = workspace;
            Refresh = workspace;
            Session = workspace;
            Diagnostics = workspace;
        }

        public IOpenVisionDockedLayerWorkspaceCommands Commands { get; }

        public IOpenVisionDockedLayerWorkspaceSynchronization Synchronization { get; }

        public IOpenVisionDockedLayerWorkspaceRefresh Refresh { get; }

        public IOpenVisionDockedLayerWorkspaceSession Session { get; }

        public IOpenVisionDockedLayerWorkspaceDiagnostics Diagnostics { get; }

        public void Attach(OpenVisionShellHostRefreshCoordinator refreshCoordinator, IOpenVisionDockLifecycle lifecycle)
        {
            if (refreshCoordinator == null)
            {
                throw new ArgumentNullException(nameof(refreshCoordinator));
            }

            if (lifecycle == null)
            {
                throw new ArgumentNullException(nameof(lifecycle));
            }

            refreshCoordinator.AttachDockedLayerWorkspace(Refresh);
            Session.AttachDockLifecycle(lifecycle);
        }

        public OpenVisionShellHostDockingTestFacade CreateTestFacade(Action updateLayout)
        {
            return new OpenVisionShellHostDockingTestFacade(Diagnostics, updateLayout);
        }
    }
}
