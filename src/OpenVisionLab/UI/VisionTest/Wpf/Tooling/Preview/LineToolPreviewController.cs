using OpenVisionLab.Vision2D.Tool;
using System;
using System.Drawing;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class LineToolPreviewController : IDisposable
    {
        private readonly LineToolPresenter presenter;
        private readonly VisionToolSingleInputSpecialPropertyToolController toolController;
        private readonly LineToolInteractionController interactionController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private bool autoPreviewShouldShowThresholdTeachingImage;
        private bool thresholdTeachingPreviewRequested;

        public LineToolPreviewController(
            FrameworkElement owner,
            LineToolPresenter presenter,
            VisionToolSingleInputSpecialPropertyToolController toolController,
            LineToolInteractionController interactionController)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.toolController = toolController ?? throw new ArgumentNullException(nameof(toolController));
            this.interactionController = interactionController ?? throw new ArgumentNullException(nameof(interactionController));
            previewScheduler = new VisionToolDebouncedPreviewScheduler(owner, RunAutoPreview, 120);
        }

        public bool ConsumeThresholdTeachingPreviewRequest()
        {
            bool requested = thresholdTeachingPreviewRequested;
            thresholdTeachingPreviewRequested = false;
            return requested;
        }

        public void SetInputPreview(Bitmap image)
        {
            toolController.SetInputPreview(image, UpdateInputRoiOverlay);
        }

        public void UpdateInputRoiOverlay()
        {
            VisionToolInlinePreviewSlot inputPreview = toolController.InputPreview;
            if (inputPreview == null)
            {
                return;
            }

            if (!inputPreview.HasImage)
            {
                inputPreview.ClearRoiOverlays();
                return;
            }

            inputPreview.SetLineRoiOverlays(
                presenter.LineAProperty.CvROI,
                presenter.LineBProperty.CvROI,
                interactionController.IsLineBSelected);
        }

        public void ScheduleAutoPreview()
        {
            autoPreviewShouldShowThresholdTeachingImage = ShouldShowThresholdTeachingPreview();
            previewScheduler.Schedule();
        }

        public void CancelAutoPreview()
        {
            previewScheduler.Cancel();
        }

        public void Dispose()
        {
            previewScheduler.Dispose();
        }

        private void RunAutoPreview()
        {
            thresholdTeachingPreviewRequested = autoPreviewShouldShowThresholdTeachingImage;
            autoPreviewShouldShowThresholdTeachingImage = false;
            toolController.RequestRunPreview();
        }

        private bool ShouldShowThresholdTeachingPreview()
        {
            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            return property != null && (property.USE_THRESHOLD || property.USE_ADAPTIVE_THRESHOLD);
        }
    }
}
