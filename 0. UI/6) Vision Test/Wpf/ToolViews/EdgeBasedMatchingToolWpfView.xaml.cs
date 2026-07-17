using Lib.OpenCV;
using Lib.OpenCV.Result;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    public partial class EdgeBasedMatchingToolWpfView : VisionToolSingleInputPropertyToolViewBase
    {
        private readonly VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty> toolController;

        internal EdgeBasedMatchingToolWpfView(VisionToolPropertyGridPresenter<EdgeBasedMatchingProperty> presenter)
        {
            OpenVisionToolOpenProfiler.Measure("EdgeBasedMatchingInitializeComponent", InitializeComponent);
            toolController = OpenVisionToolOpenProfiler.Measure(
                "EdgeBasedMatchingAttachController",
                () => VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty>.Attach(
                    this,
                    presenter,
                    "VisionMenu.EdgeBasedMatching",
                    "Edge Match"));
            AttachPropertyToolController(toolController);
        }

        public string ResultReviewTextForTest => toolController.ResultReviewText;

        public EdgeBasedMatchingProperty CreateProperty()
        {
            return toolController.CreateProperty();
        }

        public void SetTemplatePathForTest(string path)
        {
            toolController.SetTemplatePathForTest(path);
        }

        public void ConfigurePropertyForTest(Action<EdgeBasedMatchingProperty> configure)
        {
            toolController.ConfigurePropertyForTest(configure);
        }

        public void SetResultReview(IEnumerable<MatchingResult> results)
        {
            toolController.SetResultReview(results);
        }

        public void SetResultReview(IEnumerable<MatchingResult> results, TimeSpan? tactTime)
        {
            toolController.SetResultReview(results, tactTime);
        }
    }
}
