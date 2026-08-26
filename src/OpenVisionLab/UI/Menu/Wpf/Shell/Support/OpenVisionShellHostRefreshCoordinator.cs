using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostRefreshCoordinator
    {
        private IOpenVisionDockedLayerWorkspaceRefresh dockedLayerWorkspace;
        private OpenVisionShellHostLayerRefreshController layerRefreshController;
        private OpenVisionShellHostLayerWorkspacePresenter layerWorkspacePresenter;
        private OpenVisionShellHostLayerCommandSurface layerCommands;
        private OpenVisionShellHostWorkspaceCommandSurface workspaceCommands;

        public void AttachDockedLayerWorkspace(IOpenVisionDockedLayerWorkspaceRefresh workspace)
        {
            dockedLayerWorkspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        }

        public void AttachLayerRefreshController(OpenVisionShellHostLayerRefreshController controller)
        {
            layerRefreshController = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        public void AttachLayerWorkspacePresenter(OpenVisionShellHostLayerWorkspacePresenter presenter)
        {
            layerWorkspacePresenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        public void AttachCommandSurfaces(
            OpenVisionShellHostLayerCommandSurface layerCommandSurface,
            OpenVisionShellHostWorkspaceCommandSurface workspaceCommandSurface)
        {
            layerCommands = layerCommandSurface ?? throw new ArgumentNullException(nameof(layerCommandSurface));
            workspaceCommands = workspaceCommandSurface ?? throw new ArgumentNullException(nameof(workspaceCommandSurface));
        }

        public void RefreshDockedLayerViews()
        {
            dockedLayerWorkspace?.RefreshLayerViewers();
        }

        public void ApplyDockedLayerRefreshResult(OpenVisionDockDocumentRefreshResult result)
        {
            layerWorkspacePresenter?.ApplyDockedLayerRefreshResult(result);
        }

        public void RefreshHostLayerRows()
        {
            layerRefreshController?.RefreshRows();
        }

        public List<string> CreateWorkspaceLayerTitleSnapshot()
        {
            return layerRefreshController?.CreateWorkspaceLayerTitleSnapshot()
                ?? new List<string>();
        }

        public void RefreshHostSelectedLayerDetail(string layerTitle)
        {
            layerRefreshController?.RefreshSelectedLayerDetail(layerTitle);
            RefreshCommandCanExecute();
        }

        public void RefreshLayerActionButtons()
        {
            layerRefreshController?.RefreshActionButtons();
        }

        public void RefreshHostCommandCanExecute()
        {
            RefreshCommandCanExecute();
        }

        public void ApplyWorkspacePointerStatus(OpenVisionZoomableImageStatus status)
        {
            layerRefreshController?.ApplyWorkspacePointerStatus(status);
        }

        private void RefreshCommandCanExecute()
        {
            workspaceCommands?.RefreshCanExecute();
            layerCommands?.RefreshCanExecute();
        }
    }
}
