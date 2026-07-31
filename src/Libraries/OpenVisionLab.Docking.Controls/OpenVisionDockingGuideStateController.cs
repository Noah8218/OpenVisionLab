using System;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockingGuideStateController
    {
        private readonly OpenVisionLayerDockWorkspaceView workspaceView;
        private readonly OpenVisionLayerDockingGuidePresenter guidePresenter;

        public OpenVisionDockingGuideStateController(
            OpenVisionLayerDockWorkspaceView workspaceView,
            OpenVisionLayerDockingGuidePresenter guidePresenter)
        {
            this.workspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            this.guidePresenter = guidePresenter ?? throw new ArgumentNullException(nameof(guidePresenter));
        }

        public void SetOverlayVisible(bool visible)
        {
            workspaceView.IsGuideOverlayVisible = visible;
            if (!visible)
            {
                guidePresenter.ResetPaneGuideMargin();
                return;
            }

            SetActiveZone(DockingGuideZone.Center);
        }

        public void SetActiveZone(DockingGuideZone activeZone)
        {
            workspaceView.ActiveGuideZone = activeZone;
        }
    }
}
