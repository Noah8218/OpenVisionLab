using System;
namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionLayerDockWorkspaceView
    {
        private void HookDockingManagerEvents()
        {
            layerDockingManager.LayoutChanged += OnDockingLayoutChanged;
            layerDockingManager.ContentDocked += OnDockingContentDocked;
            layerDockingManager.ContentFloated += OnDockingContentFloated;
        }

        private void OnDockingLayoutChanged(object sender, EventArgs e)
        {
            DockingLayoutChanged?.Invoke(this, e);
        }

        private void OnDockingContentDocked(object sender, EventArgs e)
        {
            DockingContentDocked?.Invoke(this, e);
        }

        private void OnDockingContentFloated(object sender, EventArgs e)
        {
            DockingContentFloated?.Invoke(this, e);
        }
    }
}
