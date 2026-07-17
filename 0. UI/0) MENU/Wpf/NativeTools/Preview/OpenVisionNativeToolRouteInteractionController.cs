using System;

namespace OpenVisionLab
{
    // Layer route side effects stay here so every native tool follows the same input/output selection rules.
    internal sealed class OpenVisionNativeToolRouteInteractionController
    {
        private readonly OpenVisionNativeLayerRouteController layerRouteController;
        private readonly OpenVisionNativeToolLayerViewController layerViewController;
        private readonly Action layerStateChanged;
        private readonly Action<string> setStatus;

        public OpenVisionNativeToolRouteInteractionController(
            OpenVisionNativeLayerRouteController layerRouteController,
            OpenVisionNativeToolLayerViewController layerViewController,
            Action layerStateChanged,
            Action<string> setStatus)
        {
            this.layerRouteController = layerRouteController ?? throw new ArgumentNullException(nameof(layerRouteController));
            this.layerViewController = layerViewController ?? throw new ArgumentNullException(nameof(layerViewController));
            this.layerStateChanged = layerStateChanged ?? throw new ArgumentNullException(nameof(layerStateChanged));
            this.setStatus = setStatus ?? throw new ArgumentNullException(nameof(setStatus));
        }

        public void RefreshSingleLayerState(ISingleInputVisionToolWpfView view)
        {
            layerViewController.RefreshSingleLayerState(view);
            layerStateChanged();
        }

        public void RefreshArithmeticLayerState(IArithmeticVisionToolWpfView view)
        {
            layerViewController.RefreshArithmeticLayerState(view);
            layerStateChanged();
        }

        public void RefreshSinglePreviews(ISingleInputVisionToolWpfView view)
        {
            layerViewController.RefreshSinglePreviews(view);
        }

        public void RefreshArithmeticPreviews(IArithmeticVisionToolWpfView view)
        {
            layerViewController.RefreshArithmeticPreviews(view);
        }

        public void HandleSingleInputLayerChanged(ISingleInputVisionToolWpfView view)
        {
            if (view == null)
            {
                return;
            }

            layerRouteController.TryAcceptInputLayer(view.SelectedInputLayer, layerRouteController.ResolveOutputLayer());
            layerViewController.ActivateLayer(layerRouteController.ResolveInputLayer());
            RefreshSinglePreviews(view);
            layerStateChanged();
        }

        public void HandleSingleOutputLayerChanged(ISingleInputVisionToolWpfView view)
        {
            if (view == null)
            {
                return;
            }

            layerRouteController.AcceptOutputLayer(view.SelectedOutputLayer);
            RefreshSinglePreviews(view);
            layerStateChanged();
        }

        public void HandleSingleInputPreviewClicked()
        {
            layerViewController.ActivateLayer(layerRouteController.ResolveInputLayer());
            layerStateChanged();
        }

        public void HandleSingleOutputPreviewClicked()
        {
            if (layerViewController.ActivateLayerIfPresent(layerRouteController.ResolveOutputLayer()))
            {
                layerStateChanged();
            }
        }

        public void HandleSingleCreateOutputLayerRequested(ISingleInputVisionToolWpfView view)
        {
            string inputLayer = layerRouteController.ResolveInputLayer();
            string outputLayer = layerRouteController.SelectNextOutputLayerName();
            // The create button means "prepare another result layer"; preview keeps using the selected output.
            layerViewController.EnsureOutputLayerFromInput(inputLayer, outputLayer);
            layerViewController.RefreshSingleLayerState(view, outputLayer);
            layerStateChanged();
            setStatus("Output layer ready / " + outputLayer);
        }

        public void HandleArithmeticInputALayerChanged(IArithmeticVisionToolWpfView view)
        {
            if (view == null)
            {
                return;
            }

            layerRouteController.TryAcceptArithmeticInputLayerA(view.SelectedInputLayerA, layerRouteController.ResolveArithmeticOutputLayer());
            layerViewController.ActivateLayer(layerRouteController.ResolveArithmeticInputLayerA());
            RefreshArithmeticPreviews(view);
            layerStateChanged();
        }

        public void HandleArithmeticInputBLayerChanged(IArithmeticVisionToolWpfView view)
        {
            if (view == null)
            {
                return;
            }

            layerRouteController.TryAcceptArithmeticInputLayerB(view.SelectedInputLayerB, layerRouteController.ResolveArithmeticOutputLayer());
            layerViewController.ActivateLayer(layerRouteController.ResolveArithmeticInputLayerB());
            RefreshArithmeticPreviews(view);
            layerStateChanged();
        }

        public void HandleArithmeticOutputLayerChanged(IArithmeticVisionToolWpfView view)
        {
            if (view == null)
            {
                return;
            }

            layerRouteController.AcceptArithmeticOutputLayer(view.SelectedOutputLayer);
            RefreshArithmeticPreviews(view);
            layerStateChanged();
        }

        public void HandleArithmeticInputAPreviewClicked()
        {
            layerViewController.ActivateLayer(layerRouteController.ResolveArithmeticInputLayerA());
            layerStateChanged();
        }

        public void HandleArithmeticInputBPreviewClicked()
        {
            layerViewController.ActivateLayer(layerRouteController.ResolveArithmeticInputLayerB());
            layerStateChanged();
        }

        public void HandleArithmeticOutputPreviewClicked()
        {
            if (layerViewController.ActivateLayerIfPresent(layerRouteController.ResolveArithmeticOutputLayer()))
            {
                layerStateChanged();
            }
        }

        public void HandleArithmeticCreateOutputLayerRequested(IArithmeticVisionToolWpfView view)
        {
            string inputLayer = layerRouteController.ResolveArithmeticInputLayerA();
            string outputLayer = layerRouteController.SelectNextArithmeticOutputLayerName();
            // The create button means "prepare another result layer"; preview keeps using the selected output.
            layerViewController.EnsureOutputLayerFromInput(inputLayer, outputLayer);
            layerViewController.RefreshArithmeticLayerState(view, outputLayer);
            layerStateChanged();
            setStatus("Output layer ready / " + outputLayer);
        }
    }
}
