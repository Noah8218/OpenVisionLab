using OpenVisionLab.Vision2D.Blob;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    public partial class BlobToolWpfView : VisionToolSingleInputPropertyToolViewBase
    {
        private readonly VisionToolPropertyGridPresenter<BlobProperty> presenter;
        private readonly VisionToolSingleInputPropertyToolController<BlobProperty> toolController;
        private readonly VisionToolAreaVerificationGuidePresenter<BlobProperty, BlobResult> verificationGuidePresenter;
        private readonly VisionToolThresholdTeachingPreviewController thresholdTeachingPreviewController;

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
            thresholdTeachingPreviewController = new VisionToolThresholdTeachingPreviewController(
                () => toolController?.ClearResultReview());
            toolController = OpenVisionToolOpenProfiler.Measure(
                "BlobAttachController",
                () => VisionToolSingleInputPropertyToolController<BlobProperty>.Attach(
                    this,
                    presenter,
                    "VisionMenu.Blob",
                    beforeAutoPreview: thresholdTeachingPreviewController.Request,
                    // Blob auto-preview is a teaching view: threshold image first, detection result only after Run.
                    autoPreviewOnPropertyChanged: true,
                    refreshVerificationGuide: verificationGuidePresenter.ShowTeachingState,
                    presets: VisionToolPresetCatalog.GetPropertyGridPresets<BlobProperty>()));
            AttachPropertyToolController(toolController);
        }

        public BlobProperty CreateProperty()
        {
            return toolController.CreateProperty();
        }

        public bool ConsumeThresholdTeachingPreviewRequest()
        {
            return thresholdTeachingPreviewController.ConsumeRequest();
        }

        public void SetResultReview(IEnumerable<BlobResult> results)
        {
            toolController.ShowAreaResultReview(
                verificationGuidePresenter,
                CurrentProperty,
                "Blob",
                VisionToolVerificationText.T("VisionTool.Review.NoBlob", "no blob"),
                results,
                item => item.Area,
                item => item.Center.X,
                item => item.Center.Y,
                item => item.Bounding.Width,
                item => item.Bounding.Height);
        }

        private BlobProperty CurrentProperty => presenter.SelectedObject as BlobProperty;
    }
}
