using Lib.OpenCV.Result;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputMatchingToolController<TProperty> : IVisionToolSingleInputPropertyToolController
    {
        private readonly VisionToolSingleInputToolEventHub eventHub;
        private readonly VisionToolLanguageChangeController languageChangeController;
        private readonly VisionToolSingleInputMatchingToolRuntime<TProperty> toolRuntime;
        private readonly string resultReviewTitle;

        private VisionToolSingleInputMatchingToolController(
            FrameworkElement owner,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            string titleLocalizationKey,
            string resultReviewTitle)
        {
            eventHub = OpenVisionToolOpenProfiler.Measure("CreateMatchingEventHub", () => new VisionToolSingleInputToolEventHub(owner));
            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            this.resultReviewTitle = string.IsNullOrWhiteSpace(resultReviewTitle) ? "Match" : resultReviewTitle.Trim();

            // Matching views share the same shell wiring; only the property type and review title differ.
            toolRuntime = OpenVisionToolOpenProfiler.Measure(
                "AttachSingleInputMatchingRuntime",
                () => VisionToolSingleInputMatchingToolRuntime<TProperty>.Attach(
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
                    eventHub.RaiseSavePreviewImageRequested));

            languageChangeController = OpenVisionToolOpenProfiler.Measure("AttachMatchingLanguageController", () => VisionToolLanguageChangeController.Attach(RefreshLocalization));
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

        public static VisionToolSingleInputMatchingToolController<TProperty> Attach(
            FrameworkElement owner,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            string titleLocalizationKey,
            string resultReviewTitle)
        {
            return new VisionToolSingleInputMatchingToolController<TProperty>(owner, presenter, titleLocalizationKey, resultReviewTitle);
        }

        public TProperty CreateProperty()
        {
            return toolRuntime.CreateProperty();
        }

        public void SetTemplatePathForTest(string path)
        {
            toolRuntime.SetTemplatePathForTest(path);
        }

        public void ConfigurePropertyForTest(Action<TProperty> configure)
        {
            toolRuntime.ConfigurePropertyForTest(configure);
        }

        public bool ApplyPresetForTest(string presetId)
        {
            return toolRuntime.ApplyPresetForTest(presetId);
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

        public void SetResultReview(string title, IEnumerable<MatchingResult> results, TimeSpan? tactTime = null)
        {
            toolRuntime.SetResultReview(title, results, tactTime);
        }

        public void SetResultReview(IEnumerable<MatchingResult> results, TimeSpan? tactTime = null)
        {
            toolRuntime.SetResultReview(resultReviewTitle, results, tactTime);
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
