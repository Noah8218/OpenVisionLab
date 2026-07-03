using Lib.OpenCV;
using Lib.OpenCV.Result;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class MatchingToolWpfView : UserControl, ISingleInputVisionToolWpfView, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly VisionToolSingleInputMatchingToolController<MatchingProperty> toolController;

        internal MatchingToolWpfView(VisionToolPropertyGridPresenter<MatchingProperty> presenter)
        {
            OpenVisionToolOpenProfiler.Measure("MatchingInitializeComponent", InitializeComponent);
            toolController = OpenVisionToolOpenProfiler.Measure(
                "MatchingAttachController",
                () => VisionToolSingleInputMatchingToolController<MatchingProperty>.Attach(
                    this,
                    presenter,
                    "VisionMenu.Matching"));
        }

        public event EventHandler SourceLayerChanged
        {
            add { toolController.SourceLayerChanged += value; }
            remove { toolController.SourceLayerChanged -= value; }
        }

        public event EventHandler DestinationLayerChanged
        {
            add { toolController.DestinationLayerChanged += value; }
            remove { toolController.DestinationLayerChanged -= value; }
        }

        public event EventHandler InputPreviewClicked
        {
            add { toolController.InputPreviewClicked += value; }
            remove { toolController.InputPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { toolController.OutputPreviewClicked += value; }
            remove { toolController.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { toolController.CreateOutputLayerRequested += value; }
            remove { toolController.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler RunPreviewRequested
        {
            add { toolController.RunPreviewRequested += value; }
            remove { toolController.RunPreviewRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { toolController.AddPipelineRequested += value; }
            remove { toolController.AddPipelineRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { toolController.LoadPreviewImageRequested += value; }
            remove { toolController.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { toolController.SavePreviewImageRequested += value; }
            remove { toolController.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayer => toolController.SelectedInputLayer;

        public string SelectedOutputLayer => toolController.SelectedOutputLayer;

        public string ResultReviewTextForTest => toolController.ResultReviewText;

        public MatchingProperty CreateProperty()
        {
            return toolController.CreateProperty();
        }

        public void SetTemplatePathForTest(string path)
        {
            toolController.SetTemplatePathForTest(path);
        }

        public void ConfigurePropertyForTest(Action<MatchingProperty> configure)
        {
            toolController.ConfigurePropertyForTest(configure);
        }

        public bool ApplyPresetForTest(string presetId)
        {
            return toolController.ApplyPresetForTest(presetId);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            toolController.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            toolController.SetInputPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolController.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            toolController.SetStatus(status);
        }

        public void SetResultReview(IEnumerable<MatchingResult> results)
        {
            toolController.SetResultReview("Template Match", results);
        }

        public void SetResultReview(IEnumerable<MatchingResult> results, TimeSpan? tactTime)
        {
            toolController.SetResultReview("Template Match", results, tactTime);
        }

        public void DisposeView()
        {
            toolController.Dispose();
        }
    }
}
