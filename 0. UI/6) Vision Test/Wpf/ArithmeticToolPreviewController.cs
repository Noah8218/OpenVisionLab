using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class ArithmeticToolPreviewController : IDisposable
    {
        private readonly VisionToolDoubleInputCustomToolController toolController;
        private readonly Func<bool> useOffsetMode;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;

        public ArithmeticToolPreviewController(
            FrameworkElement owner,
            VisionToolDoubleInputCustomToolController toolController,
            Func<bool> useOffsetMode)
        {
            this.toolController = toolController ?? throw new ArgumentNullException(nameof(toolController));
            this.useOffsetMode = useOffsetMode ?? throw new ArgumentNullException(nameof(useOffsetMode));
            previewScheduler = new VisionToolDebouncedPreviewScheduler(owner, RequestPreviewForCurrentMode, 120);
        }

        public void ScheduleAutoPreview()
        {
            previewScheduler.Schedule();
        }

        public void Dispose()
        {
            previewScheduler.Dispose();
        }

        private void RequestPreviewForCurrentMode()
        {
            if (useOffsetMode())
            {
                toolController.RequestRunOffset();
                return;
            }

            toolController.RequestRunPreview();
        }
    }
}
