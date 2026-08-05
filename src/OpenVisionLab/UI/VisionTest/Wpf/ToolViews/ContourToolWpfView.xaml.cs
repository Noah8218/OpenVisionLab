using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    public partial class ContourToolWpfView : VisionToolSingleInputPropertyToolViewBase
    {
        private readonly VisionToolPropertyGridPresenter<ContourProperty> presenter;
        private readonly VisionToolSingleInputPropertyToolController<ContourProperty> toolController;
        private readonly VisionToolAreaVerificationGuidePresenter<ContourProperty, ContourResult> verificationGuidePresenter;
        private readonly VisionToolThresholdTeachingPreviewController thresholdTeachingPreviewController;

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
            thresholdTeachingPreviewController = new VisionToolThresholdTeachingPreviewController(
                () => toolController?.ClearResultReview());
            toolController = VisionToolSingleInputPropertyToolController<ContourProperty>.Attach(
                this,
                presenter,
                "VisionMenu.Contour",
                beforeAutoPreview: thresholdTeachingPreviewController.Request,
                // Contour uses the same internal threshold teaching flow as Blob, so PropertyGrid edits should refresh the preview.
                autoPreviewOnPropertyChanged: true,
                refreshVerificationGuide: verificationGuidePresenter.ShowTeachingState,
                presets: VisionToolPresetCatalog.GetPropertyGridPresets<ContourProperty>());
            AttachPropertyToolController(toolController);
        }

        public ContourProperty CreateProperty()
        {
            return toolController.CreateProperty();
        }

        public bool ConsumeThresholdTeachingPreviewRequest()
        {
            return thresholdTeachingPreviewController.ConsumeRequest();
        }

        public void SetResultReview(IEnumerable<ContourResult> results)
        {
            toolController.ShowAreaResultReview(
                verificationGuidePresenter,
                CurrentProperty,
                "Contour",
                VisionToolVerificationText.T("VisionTool.Review.NoContour", "no contour"),
                results,
                item => item.Area,
                item => item.Center.X,
                item => item.Center.Y,
                item => item.Bounding.Width,
                item => item.Bounding.Height);
        }

        private ContourProperty CurrentProperty => presenter.SelectedObject as ContourProperty;
    }
}
