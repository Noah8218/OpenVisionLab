using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputSpecialPropertyToolController : IVisionToolSingleInputPropertyToolController
    {
        private readonly VisionToolSingleInputToolEventHub eventHub;
        private VisionToolLanguageChangeController languageChangeController;
        private readonly VisionToolSingleInputSpecialPropertyToolRuntime toolRuntime;
        private bool disposed;

        private VisionToolSingleInputSpecialPropertyToolController(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement toolContent,
            Action clearResultReview)
        {
            eventHub = new VisionToolSingleInputToolEventHub(owner);
            toolRuntime = VisionToolSingleInputSpecialPropertyToolRuntime.Attach(
                owner,
                titleLocalizationKey,
                toolContent,
                eventHub.RaiseSourceLayerChanged,
                eventHub.RaiseDestinationLayerChanged,
                eventHub.RaiseInputPreviewClicked,
                eventHub.RaiseOutputPreviewClicked,
                eventHub.RaiseCreateOutputLayerRequested,
                eventHub.RaiseRunPreviewRequested,
                eventHub.RaiseAddPipelineRequested,
                eventHub.RaiseLoadPreviewImageRequested,
                eventHub.RaiseSavePreviewImageRequested,
                clearResultReview);

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

        public Border PropertyGridHost => toolRuntime.PropertyGridHost;

        public TextBlock ResultReviewText => toolRuntime.ResultReviewText;

        public TextBlock ResultGuidanceText => toolRuntime.ResultGuidanceText;

        public TextBlock SummaryText => toolRuntime.SummaryText;

        public Panel ResultReviewChips => toolRuntime.ResultReviewChips;

        public VisionToolInlinePreviewSlot InputPreview => toolRuntime.InputPreview;

        public static VisionToolSingleInputSpecialPropertyToolController Attach(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement toolContent,
            Action clearResultReview = null)
        {
            return new VisionToolSingleInputSpecialPropertyToolController(
                owner,
                titleLocalizationKey,
                toolContent,
                clearResultReview);
        }

        public VisionToolPresetButtonPresenter<TProperty> AttachPresetPresenter<TProperty>(
            IReadOnlyList<VisionToolPreset<TProperty>> presets,
            Action<VisionToolPreset<TProperty>> applyPreset)
        {
            return toolRuntime.AttachPresetPresenter(presets, applyPreset);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            toolRuntime.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            toolRuntime.SetInputPreview(image);
        }

        public void SetInputPreview(Bitmap image, Action afterRefresh)
        {
            if (afterRefresh == null)
            {
                toolRuntime.SetInputPreview(image);
                return;
            }

            toolRuntime.SetInputPreview(image, afterRefresh);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolRuntime.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            toolRuntime.SetStatus(status);
        }

        public void SetSummaryText(string summary)
        {
            toolRuntime.SetSummaryText(summary);
        }

        public void ApplyLocalization()
        {
            toolRuntime.ApplyLocalization();
        }

        public void ClearResultReview()
        {
            toolRuntime.ClearResultReview();
        }

        public void RequestRunPreview()
        {
            eventHub.RaiseRunPreviewRequested();
        }

        public void AttachLanguageChange(Action refreshLocalization)
        {
            if (refreshLocalization == null || languageChangeController != null)
            {
                return;
            }

            languageChangeController = VisionToolLanguageChangeController.Attach(refreshLocalization);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            languageChangeController?.Dispose();
            toolRuntime.Dispose();
        }
    }
}
