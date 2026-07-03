using Lib.OpenCV.Blob;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class BlobToolWpfView : UserControl, ISingleInputVisionToolWpfView, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly VisionToolPropertyGridPresenter<BlobProperty> presenter;
        private readonly VisionToolSingleInputPropertyToolController<BlobProperty> toolController;
        private readonly VisionToolAreaVerificationGuidePresenter<BlobProperty, BlobResult> verificationGuidePresenter;
        private bool thresholdTeachingPreviewRequested;

        internal BlobToolWpfView(VisionToolPropertyGridPresenter<BlobProperty> presenter)
        {
            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            OpenVisionToolOpenProfiler.Measure("BlobInitializeComponent", InitializeComponent);
            this.presenter = presenter;
            VisionToolVerificationGuideView verificationGuide = toolShell.ToolContent as VisionToolVerificationGuideView
                ?? throw new InvalidOperationException("Blob tool shell must provide a verification guide view.");
            verificationGuidePresenter = new VisionToolAreaVerificationGuidePresenter<BlobProperty, BlobResult>(
                verificationGuide,
                toolShell.ResultGuidanceText,
                "Blob",
                VisionToolAreaVerificationCriteriaText.CreateBlob,
                item => item.Area,
                item => item.Bounding.Width,
                item => item.Bounding.Height);
            toolController = OpenVisionToolOpenProfiler.Measure(
                "BlobAttachController",
                () => VisionToolSingleInputPropertyToolController<BlobProperty>.Attach(
                    this,
                    presenter,
                    "VisionMenu.Blob",
                    beforeAutoPreview: RequestThresholdTeachingPreview,
                    // Blob auto-preview is a teaching view: threshold image first, detection result only after Run.
                    autoPreviewOnPropertyChanged: true,
                    refreshVerificationGuide: verificationGuidePresenter.ShowTeachingState,
                    presets: VisionToolPresetCatalog.GetPropertyGridPresets<BlobProperty>()));
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

        public BlobProperty CreateProperty()
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

        public void SetResultReview(IEnumerable<BlobResult> results)
        {
            List<BlobResult> resultList = results?.Where(item => item != null).ToList() ?? new List<BlobResult>();
            verificationGuidePresenter.ShowResult(resultList, CurrentProperty);
            toolController.ShowAreaResultReview(
                "Blob",
                VisionToolVerificationText.T("VisionTool.Review.NoBlob", "no blob"),
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
            toolController.ClearResultReview();
            thresholdTeachingPreviewRequested = true;
        }

        private BlobProperty CurrentProperty => presenter.SelectedObject as BlobProperty;
    }
}
