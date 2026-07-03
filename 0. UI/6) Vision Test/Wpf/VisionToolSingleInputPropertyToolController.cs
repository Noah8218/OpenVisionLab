using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputPropertyToolController<TProperty> : IDisposable
    {
        private readonly VisionToolSingleInputToolEventHub eventHub;
        private readonly VisionToolLanguageChangeController languageChangeController;
        private readonly VisionToolSingleInputPropertyToolRuntime<TProperty> toolRuntime;

        private VisionToolSingleInputPropertyToolController(
            FrameworkElement owner,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            string titleLocalizationKey,
            Action beforeAutoPreview,
            bool autoPreviewOnPropertyChanged,
            Action<TProperty> refreshVerificationGuide,
            IReadOnlyList<VisionToolPreset<TProperty>> presets)
        {
            eventHub = OpenVisionToolOpenProfiler.Measure("CreateSingleInputEventHub", () => new VisionToolSingleInputToolEventHub(owner));
            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            toolRuntime = OpenVisionToolOpenProfiler.Measure(
                "AttachSingleInputPropertyRuntime",
                () => VisionToolSingleInputPropertyToolRuntime<TProperty>.Attach(
                    owner,
                    presenter,
                    titleLocalizationKey,
                    eventHub.RaiseSourceLayerChanged,
                    eventHub.RaiseDestinationLayerChanged,
                    eventHub.RaiseInputPreviewClicked,
                    eventHub.RaiseOutputPreviewClicked,
                    eventHub.RaiseCreateOutputLayerRequested,
                    eventHub.RaiseRunPreviewRequested,
                    eventHub.RaiseAddPipelineRequested,
                    eventHub.RaiseLoadPreviewImageRequested,
                    eventHub.RaiseSavePreviewImageRequested,
                    beforeAutoPreview: beforeAutoPreview,
                    autoPreviewOnPropertyChanged: autoPreviewOnPropertyChanged,
                    refreshVerificationGuide: refreshVerificationGuide,
                    presets: presets));

            languageChangeController = OpenVisionToolOpenProfiler.Measure("AttachLanguageController", () => VisionToolLanguageChangeController.Attach(RefreshLocalization));
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

        public static VisionToolSingleInputPropertyToolController<TProperty> Attach(
            FrameworkElement owner,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            string titleLocalizationKey,
            Action beforeAutoPreview = null,
            bool autoPreviewOnPropertyChanged = false,
            Action<TProperty> refreshVerificationGuide = null,
            IReadOnlyList<VisionToolPreset<TProperty>> presets = null)
        {
            return new VisionToolSingleInputPropertyToolController<TProperty>(
                owner,
                presenter,
                titleLocalizationKey,
                beforeAutoPreview,
                autoPreviewOnPropertyChanged,
                refreshVerificationGuide,
                presets);
        }

        public TProperty CreateProperty()
        {
            return toolRuntime.CreateProperty();
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

        public void SetStatus(string status)
        {
            toolRuntime.SetStatus(status);
        }

        public void ClearResultReview()
        {
            toolRuntime.ClearResultReview();
        }

        public void ShowAreaResultReview<TResult>(
            string title,
            string emptyState,
            IEnumerable<TResult> results,
            Func<TResult, double> getArea,
            Func<TResult, double> getCenterX,
            Func<TResult, double> getCenterY,
            Func<TResult, double> getBoxWidth,
            Func<TResult, double> getBoxHeight)
            where TResult : class
        {
            VisionToolAreaResultReviewPresenter.Show(
                toolRuntime.ShowResultReview,
                title,
                emptyState,
                results,
                getArea,
                getCenterX,
                getCenterY,
                getBoxWidth,
                getBoxHeight);
        }

        public void Dispose()
        {
            languageChangeController.Dispose();
            toolRuntime.Dispose();
        }

        private void RefreshLocalization()
        {
            toolRuntime.ApplyLocalization();
            toolRuntime.RefreshSelectedObject();
            toolRuntime.UpdateSummary();
        }

    }
}
