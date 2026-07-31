using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Data;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputCustomToolController : IDisposable
    {
        private readonly VisionToolSingleInputToolEventHub eventHub;
        private readonly Action applyToolLocalization;
        private readonly VisionToolLanguageChangeController languageChangeController;
        private readonly VisionToolSingleInputCustomToolRuntime toolRuntime;
        private bool disposed;

        private VisionToolSingleInputCustomToolController(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Action refreshViewState,
            Action clearResultReview,
            Action applyToolLocalization)
        {
            eventHub = new VisionToolSingleInputToolEventHub(owner);
            this.applyToolLocalization = applyToolLocalization;

            // Custom tools keep their own parameter controls; this controller owns the repeated shell/event wiring.
            toolRuntime = VisionToolSingleInputCustomToolRuntime.Attach(
                owner,
                titleLocalizationKey,
                parameterContent,
                eventHub.RaiseSourceLayerChanged,
                eventHub.RaiseDestinationLayerChanged,
                eventHub.RaiseInputPreviewClicked,
                eventHub.RaiseOutputPreviewClicked,
                eventHub.RaiseCreateOutputLayerRequested,
                eventHub.RaiseRunPreviewRequested,
                eventHub.RaiseAddPipelineRequested,
                eventHub.RaiseLoadPreviewImageRequested,
                eventHub.RaiseSavePreviewImageRequested,
                refreshViewState,
                clearResultReview);

            languageChangeController = VisionToolLanguageChangeController.Attach(RefreshLocalization);
        }

        public event EventHandler SourceLayerChanged
        {
            add { eventHub.SourceLayerChanged += value; }
            remove { eventHub.SourceLayerChanged -= value; }
        }

        public event EventHandler DestinationLayerChanged
        {
            add { eventHub.DestinationLayerChanged += value; }
            remove { eventHub.DestinationLayerChanged -= value; }
        }

        public event EventHandler InputPreviewClicked
        {
            add { eventHub.InputPreviewClicked += value; }
            remove { eventHub.InputPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { eventHub.OutputPreviewClicked += value; }
            remove { eventHub.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { eventHub.CreateOutputLayerRequested += value; }
            remove { eventHub.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler RunPreviewRequested
        {
            add { eventHub.RunPreviewRequested += value; }
            remove { eventHub.RunPreviewRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { eventHub.AddPipelineRequested += value; }
            remove { eventHub.AddPipelineRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { eventHub.LoadPreviewImageRequested += value; }
            remove { eventHub.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { eventHub.SavePreviewImageRequested += value; }
            remove { eventHub.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayer => toolRuntime.SelectedInputLayer;

        public string SelectedOutputLayer => toolRuntime.SelectedOutputLayer;

        public string ResultReviewText => toolRuntime.ResultReviewText;

        public static VisionToolSingleInputCustomToolController Attach(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Action refreshViewState = null,
            Action clearResultReview = null,
            Action applyToolLocalization = null)
        {
            return new VisionToolSingleInputCustomToolController(
                owner,
                titleLocalizationKey,
                parameterContent,
                refreshViewState,
                clearResultReview,
                applyToolLocalization);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            toolRuntime.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            toolRuntime.SetInputPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolRuntime.SetOutputPreview(image);
        }

        public void RequestRunPreview()
        {
            toolRuntime.RequestRunPreview();
        }

        public void SetStatus(string status)
        {
            toolRuntime.SetStatus(status);
        }

        public void SetTitleText(string text)
        {
            toolRuntime.SetTitleText(text);
        }

        public void SetTitleIconKind(PackIconMaterialKind iconKind)
        {
            toolRuntime.SetTitleIconKind(iconKind);
        }

        public void SetAddPipelineVisible(bool visible)
        {
            toolRuntime.SetAddPipelineVisible(visible);
        }

        public void BindSummary(BindingBase binding)
        {
            toolRuntime.BindSummary(binding);
        }

        public void RefreshSummaryBinding()
        {
            toolRuntime.RefreshSummaryBinding();
        }

        public void SetSummaryText(string text)
        {
            toolRuntime.SetSummaryText(text);
        }

        public void ShowResultReview(
            string summary,
            bool isSuccess,
            IEnumerable<VisionToolResultReviewItem> items,
            string guidance)
        {
            toolRuntime.ShowResultReview(summary, isSuccess, items, guidance);
        }

        public void ClearResultReview()
        {
            toolRuntime.ClearResultReview();
        }

        public void ApplyLocalization()
        {
            toolRuntime.ApplyLocalization();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            languageChangeController.Dispose();
            toolRuntime.Dispose();
        }

        private void RefreshLocalization()
        {
            applyToolLocalization?.Invoke();
        }

    }
}
