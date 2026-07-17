using System;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerSelectionController
    {
        private readonly ListBox layerRowsList;
        private readonly OpenVisionShellHostLayerListPresenter layerListPresenter;
        private readonly OpenVisionShellHostLayerActivationController activationController;

        public OpenVisionShellHostLayerSelectionController(
            ListBox layerRowsList,
            OpenVisionShellHostLayerListPresenter layerListPresenter,
            OpenVisionShellHostLayerActivationController activationController)
        {
            this.layerRowsList = layerRowsList ?? throw new ArgumentNullException(nameof(layerRowsList));
            this.layerListPresenter = layerListPresenter ?? throw new ArgumentNullException(nameof(layerListPresenter));
            this.activationController = activationController ?? throw new ArgumentNullException(nameof(activationController));
        }

        public void HandleSelectionChanged()
        {
            if (layerListPresenter.IsSynchronizingSelection)
            {
                return;
            }

            if (!layerListPresenter.TryGetLayerTitle(layerRowsList.SelectedIndex, out string layerTitle)
                || string.IsNullOrWhiteSpace(layerTitle))
            {
                return;
            }

            activationController.Activate(layerTitle);
        }

        public string GetSelectedLayerTitle()
        {
            return layerListPresenter.GetSelectedLayerTitle(layerRowsList.SelectedIndex);
        }
    }
}
