using OpenVisionLab.Core;
using System;
using System.Drawing;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolLayerViewController
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionNativeLayerRouteController layerRouteController;
        private readonly OpenVisionNativePreviewLayerPublisher previewLayerPublisher;

        public OpenVisionNativeToolLayerViewController(
            IDisplayManager displayManager,
            OpenVisionNativeLayerRouteController layerRouteController,
            OpenVisionNativePreviewLayerPublisher previewLayerPublisher)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.layerRouteController = layerRouteController ?? throw new ArgumentNullException(nameof(layerRouteController));
            this.previewLayerPublisher = previewLayerPublisher ?? throw new ArgumentNullException(nameof(previewLayerPublisher));
        }

        public void RefreshSingleLayerState(ISingleInputVisionToolWpfView view, string selectedOutputLayer = null)
        {
            if (view == null)
            {
                return;
            }

            string outputLayer = string.IsNullOrWhiteSpace(selectedOutputLayer)
                ? layerRouteController.ResolveOutputLayer()
                : selectedOutputLayer;
            string inputLayer = layerRouteController.ResolveInputLayerForOutput(outputLayer);
            view.SetLayerList(layerRouteController.GetWorkspaceLayerNames(inputLayer, outputLayer), inputLayer, outputLayer);
            RefreshSinglePreviews(view);
        }

        public void RefreshSinglePreviews(ISingleInputVisionToolWpfView view)
        {
            if (view == null)
            {
                return;
            }

            view.SetInputPreview(displayManager.GetLayerImage(layerRouteController.ResolveInputLayer()));
            view.SetOutputPreview(displayManager.GetLayerImage(layerRouteController.ResolveOutputLayer()));
        }

        public void RefreshArithmeticLayerState(IArithmeticVisionToolWpfView view, string selectedOutputLayer = null)
        {
            if (view == null)
            {
                return;
            }

            string outputLayer = string.IsNullOrWhiteSpace(selectedOutputLayer)
                ? layerRouteController.ResolveArithmeticOutputLayer()
                : selectedOutputLayer;
            string inputA = layerRouteController.ResolveArithmeticInputLayerAForOutput(outputLayer);
            string inputB = layerRouteController.ResolveArithmeticInputLayerBForOutput(outputLayer);
            view.SetLayerList(layerRouteController.GetWorkspaceLayerNames(inputA, inputB, outputLayer), inputA, inputB, outputLayer);
            RefreshArithmeticPreviews(view);
        }

        public void RefreshArithmeticPreviews(IArithmeticVisionToolWpfView view)
        {
            if (view == null)
            {
                return;
            }

            view.SetInputAPreview(displayManager.GetLayerImage(layerRouteController.ResolveArithmeticInputLayerA()));
            view.SetInputBPreview(displayManager.GetLayerImage(layerRouteController.ResolveArithmeticInputLayerB()));
            view.SetOutputPreview(displayManager.GetLayerImage(layerRouteController.ResolveArithmeticOutputLayer()));
        }

        public void ActivateLayer(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return;
            }

            displayManager.SelectedItem = layerName;
            displayManager.ActivateLayer(layerName);
        }

        public bool ActivateLayerIfPresent(string layerName)
        {
            int index = displayManager.FindIndex(layerName);
            if (index < 0)
            {
                return false;
            }

            displayManager.SelectedItem = layerName;
            displayManager.ActivateLayer(index);
            return true;
        }

        public void EnsureOutputLayerFromInput(string inputLayer, string outputLayer)
        {
            Bitmap source = displayManager.GetLayerImage(inputLayer);
            int width = Math.Max(1, source?.Width ?? 512);
            int height = Math.Max(1, source?.Height ?? 384);
            previewLayerPublisher.EnsureOutputLayer(outputLayer, width, height);
            previewLayerPublisher.RestoreDisplayActivation(inputLayer);
        }
    }
}
