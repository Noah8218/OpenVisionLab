using System;
using System.Windows;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionLayerDockingGuidePresenter
    {
        private readonly OpenVisionLayerDockWorkspaceView dockWorkspaceView;
        private readonly IOpenVisionDockPaneProvider paneProvider;

        public OpenVisionLayerDockingGuidePresenter(
            OpenVisionLayerDockWorkspaceView dockWorkspaceView,
            IOpenVisionDockPaneProvider paneProvider)
        {
            this.dockWorkspaceView = dockWorkspaceView ?? throw new ArgumentNullException(nameof(dockWorkspaceView));
            this.paneProvider = paneProvider ?? throw new ArgumentNullException(nameof(paneProvider));
        }

        public OpenVisionDockPaneHandle ResolveTargetPane(Point point)
        {
            return dockWorkspaceView.ResolveTargetPane(
                point,
                paneProvider.EnumeratePaneHandles(),
                paneProvider.GetPrimaryPaneHandle());
        }

        public DockingGuideZone ResolveZone(Point point, OpenVisionDockPaneHandle targetPane)
        {
            return dockWorkspaceView.ResolveGuideZone(point, targetPane);
        }

        public void PositionPaneGuideOverlay(OpenVisionDockPaneHandle targetPane)
        {
            dockWorkspaceView.PositionPaneGuideOverlay(targetPane);
        }

        public void ResetPaneGuideMargin()
        {
            dockWorkspaceView.ResetPaneGuideMargin();
        }
    }
}
