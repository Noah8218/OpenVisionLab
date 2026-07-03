using System;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockWorkspaceLayoutController
    {
        private readonly OpenVisionLayerDockWorkspaceView workspaceView;
        private readonly IOpenVisionDockDocumentWorkspace workspace;
        private readonly OpenVisionDockWorkspaceStateSaveScheduler stateSaveScheduler;
        private readonly Func<bool> canNormalizeLayout;
        private bool normalizingLayout;

        public OpenVisionDockWorkspaceLayoutController(
            OpenVisionLayerDockWorkspaceView workspaceView,
            IOpenVisionDockDocumentWorkspace workspace,
            OpenVisionDockWorkspaceStateSaveScheduler stateSaveScheduler,
            Func<bool> canNormalizeLayout)
        {
            this.workspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.stateSaveScheduler = stateSaveScheduler ?? throw new ArgumentNullException(nameof(stateSaveScheduler));
            this.canNormalizeLayout = canNormalizeLayout ?? throw new ArgumentNullException(nameof(canNormalizeLayout));
        }

        public void HandleLayoutChanged(object sender, EventArgs e)
        {
            NormalizeComparisonLayout();
            stateSaveScheduler.Queue();
        }

        public void HandleDockingStateChanged(object sender, EventArgs e)
        {
            stateSaveScheduler.Queue();
        }

        public void RefreshLayout()
        {
            NormalizeComparisonLayout();
            workspaceView.UpdateDockLayout();
        }

        private void NormalizeComparisonLayout()
        {
            if (normalizingLayout || !canNormalizeLayout())
            {
                return;
            }

            normalizingLayout = true;
            try
            {
                if (workspace.NormalizeComparisonPaneSizes())
                {
                    workspaceView.UpdateDockLayout();
                }
            }
            finally
            {
                normalizingLayout = false;
            }
        }
    }
}
