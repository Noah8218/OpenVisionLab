using Lib.OpenCV.Result;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class ContourToolWpfView : UserControl, ISingleInputVisionToolWpfView, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly VisionToolPropertyGridPresenter<ContourProperty> presenter;
        private readonly VisionToolSingleInputPropertyToolController<ContourProperty> toolController;
        private readonly VisionToolAreaVerificationGuidePresenter<ContourProperty, ContourResult> verificationGuidePresenter;
        private bool thresholdTeachingPreviewRequested;

        internal ContourToolWpfView(VisionToolPropertyGridPresenter<ContourProperty> presenter)
        {
            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            InitializeComponent();
            this.presenter = presenter;
            VisionToolVerificationGuideView verificationGuide = toolShell.ToolContent as VisionToolVerificationGuideView
                ?? throw new InvalidOperationException("Contour tool shell must provide a verification guide view.");
            verificationGuidePresenter = new VisionToolAreaVerificationGuidePresenter<ContourProperty, ContourResult>(
                verificationGuide,
                toolShell.ResultGuidanceText,
                "Contour",
                VisionToolAreaVerificationCriteriaText.CreateContour,
                item => item.Area,
                item => item.Bounding.Width,
                item => item.Bounding.Height);
            toolController = VisionToolSingleInputPropertyToolController<ContourProperty>.Attach(
                this,
                presenter,
                "VisionMenu.Contour",
                beforeAutoPreview: RequestThresholdTeachingPreview,
                // Contour uses the same internal threshold teaching flow as Blob, so PropertyGrid edits should refresh the preview.
                autoPreviewOnPropertyChanged: true,
                refreshVerificationGuide: verificationGuidePresenter.ShowTeachingState,
                presets: VisionToolPresetCatalog.GetPropertyGridPresets<ContourProperty>());
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

        public ContourProperty CreateProperty()
        {
            return toolController.CreateProperty();
        }

        public bool ConsumeThresholdTeachingPreviewRequest()
        {
            bool requested = thresholdTeachingPreviewRequested;
            thresholdTeachingPreviewRequested = false;
            return requested;
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

        public void SetResultReview(IEnumerable<ContourResult> results)
        {
            List<ContourResult> resultList = results?.Where(item => item != null).ToList() ?? new List<ContourResult>();
            verificationGuidePresenter.ShowResult(resultList, CurrentProperty);
            toolController.ShowAreaResultReview(
                "Contour",
                VisionToolVerificationText.T("VisionTool.Review.NoContour", "no contour"),
                resultList,
                item => item.Area,
                item => item.Center.X,
                item => item.Center.Y,
                item => item.Bounding.Width,
                item => item.Bounding.Height);
        }

        public void DisposeView()
        {
            toolController.Dispose();
        }

        private void RequestThresholdTeachingPreview()
        {
            thresholdTeachingPreviewRequested = true;
        }

        private ContourProperty CurrentProperty => presenter.SelectedObject as ContourProperty;
    }
}
