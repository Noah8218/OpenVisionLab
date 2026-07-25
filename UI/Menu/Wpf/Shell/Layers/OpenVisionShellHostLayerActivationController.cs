using OpenVisionLab.Core;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerActivationController
    {
        private readonly IDisplayManager displayManager;
        private readonly Action<string> refreshSelectedLayerDetail;
        private readonly Action refreshRows;

        public OpenVisionShellHostLayerActivationController(
            IDisplayManager displayManager,
            Action<string> refreshSelectedLayerDetail,
            Action refreshRows)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.refreshSelectedLayerDetail = refreshSelectedLayerDetail ?? throw new ArgumentNullException(nameof(refreshSelectedLayerDetail));
            this.refreshRows = refreshRows ?? throw new ArgumentNullException(nameof(refreshRows));
        }

        public bool Activate(string layerTitle)
        {
            int layerIndex = displayManager.FindIndex(layerTitle);
            if (layerIndex < 0)
            {
                return false;
            }

            displayManager.SelectedItem = layerTitle;
            displayManager.ActivateLayer(layerIndex);
            refreshSelectedLayerDetail(layerTitle);
            refreshRows();
            return true;
        }
    }
}
