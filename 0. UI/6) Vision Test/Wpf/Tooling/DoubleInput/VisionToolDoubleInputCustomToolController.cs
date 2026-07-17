using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class VisionToolDoubleInputCustomToolController : IDisposable
    {
        private readonly VisionToolDoubleInputToolEventHub eventHub;
        private readonly Action applyToolLocalization;
        private readonly VisionToolLanguageChangeController languageChangeController;
        private readonly VisionToolDoubleInputCustomToolRuntime toolRuntime;
        private bool disposed;

        private VisionToolDoubleInputCustomToolController(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Func<bool> useOffsetMode,
            Action refreshViewState,
            Action clearResultReview,
            Action applyToolLocalization)
        {
            eventHub = new VisionToolDoubleInputToolEventHub(owner);
            this.applyToolLocalization = applyToolLocalization;

            toolRuntime = VisionToolDoubleInputCustomToolRuntime.Attach(
                owner,
                titleLocalizationKey,
                parameterContent,
                useOffsetMode,
                eventHub.RaiseInputALayerChanged,
                eventHub.RaiseInputBLayerChanged,
                eventHub.RaiseOutputLayerChanged,
                eventHub.RaiseInputAPreviewClicked,
                eventHub.RaiseInputBPreviewClicked,
                eventHub.RaiseOutputPreviewClicked,
                eventHub.RaiseCreateOutputLayerRequested,
                eventHub.RaiseRunPreviewRequested,
                eventHub.RaiseRunOffsetRequested,
                eventHub.RaiseAddPipelineRequested,
                eventHub.RaiseLoadPreviewImageRequested,
                eventHub.RaiseSavePreviewImageRequested,
                refreshViewState,
                clearResultReview);

            languageChangeController = VisionToolLanguageChangeController.Attach(RefreshLocalization);
        }

        public event EventHandler InputALayerChanged
        {
            add { eventHub.InputALayerChanged += value; }
            remove { eventHub.InputALayerChanged -= value; }
        }

        public event EventHandler InputBLayerChanged
        {
            add { eventHub.InputBLayerChanged += value; }
            remove { eventHub.InputBLayerChanged -= value; }
        }

        public event EventHandler OutputLayerChanged
        {
            add { eventHub.OutputLayerChanged += value; }
            remove { eventHub.OutputLayerChanged -= value; }
        }

        public event EventHandler InputAPreviewClicked
        {
            add { eventHub.InputAPreviewClicked += value; }
            remove { eventHub.InputAPreviewClicked -= value; }
        }

        public event EventHandler InputBPreviewClicked
        {
            add { eventHub.InputBPreviewClicked += value; }
            remove { eventHub.InputBPreviewClicked -= value; }
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

        public event EventHandler RunOffsetRequested
        {
            add { eventHub.RunOffsetRequested += value; }
            remove { eventHub.RunOffsetRequested -= value; }
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

        public string SelectedInputLayerA => toolRuntime.SelectedInputLayerA;

        public string SelectedInputLayerB => toolRuntime.SelectedInputLayerB;

        public string SelectedOutputLayer => toolRuntime.SelectedOutputLayer;

        public static VisionToolDoubleInputCustomToolController Attach(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Func<bool> useOffsetMode,
            Action refreshViewState = null,
            Action clearResultReview = null,
            Action applyToolLocalization = null)
        {
            return new VisionToolDoubleInputCustomToolController(
                owner,
                titleLocalizationKey,
                parameterContent,
                useOffsetMode,
                refreshViewState,
                clearResultReview,
                applyToolLocalization);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayerA, string selectedInputLayerB, string selectedOutputLayer)
        {
            toolRuntime.SetLayerList(layerNames, selectedInputLayerA, selectedInputLayerB, selectedOutputLayer);
        }

        public void SetInputAPreview(Bitmap image)
        {
            toolRuntime.SetInputAPreview(image);
        }

        public void SetInputBPreview(Bitmap image)
        {
            toolRuntime.SetInputBPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolRuntime.SetOutputPreview(image);
        }

        public void RequestRunPreview()
        {
            eventHub.RaiseRunPreviewRequested();
        }

        public void RequestRunOffset()
        {
            eventHub.RaiseRunOffsetRequested();
        }

        public void SetStatus(string status)
        {
            toolRuntime.SetStatus(status);
        }

        public void SetSummaryText(string text)
        {
            toolRuntime.SetSummaryText(text);
        }

        public void SetRunOffsetText(string text)
        {
            toolRuntime.SetRunOffsetText(text);
        }

        public void SetInputBPreviewVisible(bool visible)
        {
            toolRuntime.SetInputBPreviewVisible(visible);
        }

        public void SetOffsetActionsVisible(bool useOffsetMode)
        {
            toolRuntime.SetOffsetActionsVisible(useOffsetMode);
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
