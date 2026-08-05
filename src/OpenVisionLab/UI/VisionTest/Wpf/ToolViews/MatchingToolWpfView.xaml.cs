using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Result;
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

        internal void ApplySampleProperty(MatchingProperty source)
        {
            if (source == null)
            {
                return;
            }

            string resolvedTemplatePath = VisionPipelineAppToolFactory.ResolveTemplatePath(source.PATTERN_PATH);
            toolController.SetTemplatePathForTest(resolvedTemplatePath);
            toolController.ConfigurePropertyForTest(target =>
            {
                target.PIXELPERMM = source.PIXELPERMM;
                target.USE_THRESHOLD = source.USE_THRESHOLD;
                target.USE_BITWISENOT = source.USE_BITWISENOT;
                target.THRESHOLD_TYPES = source.THRESHOLD_TYPES;
                target.THRESHOLD = source.THRESHOLD;
                target.USE_ADAPTIVE_THRESHOLD = source.USE_ADAPTIVE_THRESHOLD;
                target.ADAPTIVE_THRESHOLD = source.ADAPTIVE_THRESHOLD;
                target.ADAPTIVE_THRESHOLD_TYPES = source.ADAPTIVE_THRESHOLD_TYPES;
                target.ADAPTIVE_THRESHOLD_ALGORITHM = source.ADAPTIVE_THRESHOLD_ALGORITHM;
                target.BlockSize = source.BlockSize;
                target.Weight = source.Weight;
                target.USE_ROI = source.USE_ROI;
                target.CvROI = source.CvROI;
                target.USE_MULTI_ROI = source.USE_MULTI_ROI;
                target.CvROIS = source.CvROIS == null
                    ? new List<OpenCvSharp.Rect>()
                    : new List<OpenCvSharp.Rect>(source.CvROIS);
                target.USE_MASKING = source.USE_MASKING;
                target.CvMASKS = source.CvMASKS == null
                    ? new List<OpenCvSharp.Rect>()
                    : new List<OpenCvSharp.Rect>(source.CvMASKS);
                target.AUTO_PREVIEW = false;
                target.MATCH_MODE = source.MATCH_MODE;
                target.SCORE_MIN = source.SCORE_MIN;
                target.NUM_MATCH = source.NUM_MATCH;
                target.MAGNIFIATION = source.MAGNIFIATION;
                target.USE_FIND_ANGLE = source.USE_FIND_ANGLE;
                target.FIND_ANGLE = source.FIND_ANGLE;
                target.FIND_ANGLE_MIN = source.FIND_ANGLE_MIN;
                target.FIND_ANGLE_MAX = source.FIND_ANGLE_MAX;
                target.USE_COARSE_TO_FINE_ANGLE_SEARCH = source.USE_COARSE_TO_FINE_ANGLE_SEARCH;
                target.COARSE_ANGLE_STEP = source.COARSE_ANGLE_STEP;
                target.COARSE_ANGLE_TOP_K = source.COARSE_ANGLE_TOP_K;
                target.USE_FIND_SCALE = source.USE_FIND_SCALE;
                target.FIND_SCALE_MIN = source.FIND_SCALE_MIN;
                target.FIND_SCALE_MAX = source.FIND_SCALE_MAX;
                target.FIND_SCALE_STEP = source.FIND_SCALE_STEP;
                target.USE_PYRAMID_POSITION_PROPOSAL = source.USE_PYRAMID_POSITION_PROPOSAL;
                target.PYRAMID_POSITION_TOP_N = source.PYRAMID_POSITION_TOP_N;
                target.PYRAMID_POSITION_MIN_SCORE = source.PYRAMID_POSITION_MIN_SCORE;
                target.USE_CANNY = source.USE_CANNY;
                target.CANNY_LOW = source.CANNY_LOW;
                target.CANNY_HIGH = source.CANNY_HIGH;
                target.USE_PADDING_COLOR_WHITE = source.USE_PADDING_COLOR_WHITE;
            });
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
