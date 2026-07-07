using Lib.OpenCV;
using Lib.OpenCV.Result;
using OpenVisionLab.Contracts;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    public partial class FeatureMatchingToolWpfView : VisionToolSingleInputPropertyToolViewBase
    {
        private readonly VisionToolSingleInputMatchingToolController<FeatureMatchingProperty> toolController;

        internal FeatureMatchingToolWpfView(VisionToolPropertyGridPresenter<FeatureMatchingProperty> presenter)
        {
            OpenVisionToolOpenProfiler.Measure("FeatureMatchingInitializeComponent", InitializeComponent);
            toolController = OpenVisionToolOpenProfiler.Measure(
                "FeatureMatchingAttachController",
                () => VisionToolSingleInputMatchingToolController<FeatureMatchingProperty>.Attach(
                    this,
                    presenter,
                    "VisionMenu.FeatureMatching",
                    "Feature Match"));
            AttachPropertyToolController(toolController);
        }

        public string ResultReviewTextForTest => toolController.ResultReviewText;

        public FeatureMatchingProperty CreateProperty()
        {
            return toolController.CreateProperty();
        }

        public void SetTemplatePathForTest(string path)
        {
            toolController.SetTemplatePathForTest(path);
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
