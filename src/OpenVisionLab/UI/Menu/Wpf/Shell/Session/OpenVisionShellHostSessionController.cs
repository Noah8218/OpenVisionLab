using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostSessionController
    {
        private readonly OpenVisionShellHostSessionState state;
        private readonly IOpenVisionDockedLayerWorkspaceSession dockedLayerWorkspace;
        private readonly OpenVisionShellHostToolPrewarmController toolPrewarmController;
        private readonly OpenVisionShellHostLifecycleController lifecycle;
        private readonly OpenVisionShellPreviewViewModel viewModel;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionZoomableImageController workspaceFallbackZoomController;
        private readonly OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController;
        private readonly Action<object> setDataContext;
        private readonly Action refreshHostLayerRows;
        private readonly Action closeLayerViewerWindows;

        public OpenVisionShellHostSessionController(
            OpenVisionShellHostSessionState state,
            IOpenVisionDockedLayerWorkspaceSession dockedLayerWorkspace,
            OpenVisionShellHostToolPrewarmController toolPrewarmController,
            OpenVisionShellHostLifecycleController lifecycle,
            OpenVisionShellPreviewViewModel viewModel,
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            OpenVisionZoomableImageController workspaceFallbackZoomController,
            OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController,
            Action<object> setDataContext,
            Action refreshHostLayerRows,
            Action closeLayerViewerWindows)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dockedLayerWorkspace = dockedLayerWorkspace ?? throw new ArgumentNullException(nameof(dockedLayerWorkspace));
            this.toolPrewarmController = toolPrewarmController ?? throw new ArgumentNullException(nameof(toolPrewarmController));
            this.lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.workspaceFallbackZoomController = workspaceFallbackZoomController ?? throw new ArgumentNullException(nameof(workspaceFallbackZoomController));
            this.toolWindowLifecycleController = toolWindowLifecycleController ?? throw new ArgumentNullException(nameof(toolWindowLifecycleController));
            this.setDataContext = setDataContext ?? throw new ArgumentNullException(nameof(setDataContext));
            this.refreshHostLayerRows = refreshHostLayerRows ?? throw new ArgumentNullException(nameof(refreshHostLayerRows));
            this.closeLayerViewerWindows = closeLayerViewerWindows ?? throw new ArgumentNullException(nameof(closeLayerViewerWindows));
        }

        public Task StartupPreparationTask { get; private set; } = Task.CompletedTask;

        public void OnLoaded()
        {
            if (state.Loaded)
            {
                return;
            }

            state.Loaded = true;
            refreshHostLayerRows();
            dockedLayerWorkspace.EnsureWorkspaceStateLoaded();
            StartupPreparationTask = toolPrewarmController.ScheduleStartupWork();
        }

        public void OnWorkspaceCanvasLoaded(Dispatcher dispatcher)
        {
            dispatcher?.BeginInvoke(new Action(workspacePreviewController.RefreshCanvas), DispatcherPriority.ContextIdle);
        }

        public bool DisposeSession()
        {
            if (state.Disposed)
            {
                return false;
            }

            state.Disposed = true;
            state.Loaded = false;
            toolPrewarmController.Cancel();
            lifecycle.Dispose();
            dockedLayerWorkspace.StopPendingWorkspaceSave();
            toolPrewarmController.Dispose();
            setDataContext(null);
            viewModel.Dispose();
            dockedLayerWorkspace.SaveLayerWorkspaceState(preservePendingPersistedState: true);
            closeLayerViewerWindows();
            toolWindowLifecycleController.CloseActiveDocument();
            workspacePreviewController.Dispose();
            workspaceFallbackZoomController.Dispose();
            return true;
        }
    }
}
