using System;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockWorkspaceLifecycleBinder
    {
        private readonly OpenVisionLayerDockWorkspaceView workspaceView;
        private readonly EventHandler layoutChangedHandler;
        private readonly EventHandler dockingStateChangedHandler;

        public OpenVisionDockWorkspaceLifecycleBinder(
            OpenVisionLayerDockWorkspaceView workspaceView,
            EventHandler layoutChangedHandler,
            EventHandler dockingStateChangedHandler)
        {
            this.workspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            this.layoutChangedHandler = layoutChangedHandler ?? throw new ArgumentNullException(nameof(layoutChangedHandler));
            this.dockingStateChangedHandler = dockingStateChangedHandler ?? throw new ArgumentNullException(nameof(dockingStateChangedHandler));
        }

        public void Attach(IOpenVisionDockLifecycle lifecycle)
        {
            if (lifecycle == null)
            {
                throw new ArgumentNullException(nameof(lifecycle));
            }

            lifecycle.Track(() => workspaceView.DockingLayoutChanged += layoutChangedHandler, () => workspaceView.DockingLayoutChanged -= layoutChangedHandler);
            lifecycle.Track(() => workspaceView.DockingContentDocked += dockingStateChangedHandler, () => workspaceView.DockingContentDocked -= dockingStateChangedHandler);
            lifecycle.Track(() => workspaceView.DockingContentFloated += dockingStateChangedHandler, () => workspaceView.DockingContentFloated -= dockingStateChangedHandler);
        }
    }
}
