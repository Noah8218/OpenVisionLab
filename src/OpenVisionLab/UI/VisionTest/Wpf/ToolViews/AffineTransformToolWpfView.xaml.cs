using System;
using OpenVisionLab.Vision2D.Tool;

namespace OpenVisionLab
{
    public partial class AffineTransformToolWpfView : VisionToolSingleInputPropertyToolViewBase
    {
        private readonly VisionToolSingleInputPropertyToolController<AffineTransformProperty> toolController;

        internal AffineTransformToolWpfView(VisionToolPropertyGridPresenter<AffineTransformProperty> presenter)
        {
            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            OpenVisionToolOpenProfiler.Measure("AffineTransformInitializeComponent", InitializeComponent);
            toolController = VisionToolSingleInputPropertyToolController<AffineTransformProperty>.Attach(
                this,
                presenter,
                "VisionMenu.AffineTransform",
                autoPreviewOnPropertyChanged: false);
            AttachPropertyToolController(toolController);
        }

        public AffineTransformProperty CreateProperty() => toolController.CreateProperty();

        public string ResultReviewTextForTest => toolController.ResultReviewText;

        public void SetResultReview(VisionToolResult result)
        {
            AffineTransformResultReviewPresenter.Show(
                result,
                (summary, isSuccess, items, guidance) =>
                    toolController.ShowResultReview(summary, isSuccess, items, guidance));
        }

        public void ConfigurePropertyForTest(Action<AffineTransformProperty> configure)
        {
            toolController.ConfigurePropertyForTest(configure);
        }
    }
}
