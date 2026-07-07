using Lib.OpenCV;
using Lib.OpenCV.Result;
using OpenVisionLab.Contracts;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    public partial class MatchingToolWpfView : VisionToolSingleInputPropertyToolViewBase
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
                    "VisionMenu.Matching",
                    "Template Match"));
            AttachPropertyToolController(toolController);
        }

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
