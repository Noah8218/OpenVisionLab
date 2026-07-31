using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeDarkBandGapIntentSkill
    {
        public const string SupportedMeasurementDefinition = "Dark-band upper-to-lower edge distance";
        public const string SupportedUnitMode = "px";
        public const string DefaultRoiText = "100,80,530,230";

        public const int DefaultCannyLow = 10;
        public const int DefaultCannyHigh = 45;
        public const double DefaultMinimumGapPixels = 12D;
        public const double DefaultMaximumGapPixels = 60D;
        public const double DefaultMaximumAngleDegrees = 8D;
        public const double DefaultMaximumParallelDeltaDegrees = 4D;
        public const double DefaultMinimumSupportRatio = 0.26D;
        public const double DefaultMinimumDarkContrast = 8D;
        public const double DefaultMinimumDarkCoverageRatio = 0.25D;
        public const double DefaultMinimumScoreMargin = 0.05D;

        internal static bool TryParseCoarseRoi(
            string text,
            out OpenVisionRecipePinGapIntentSkill.RoiSample roi,
            out string message)
        {
            roi = default;
            if (!OpenVisionRecipePinGapIntentSkill.TryParseRoiSamples(
                    text,
                    out IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rois,
                    out message))
            {
                return false;
            }

            if (rois.Count != 1)
            {
                message = "Exactly one operator-reviewed coarse ROI is required.";
                return false;
            }

            roi = rois[0];
            message = string.Empty;
            return true;
        }

        internal static VisionPipeline CreateMeasurementPipeline(
            OpenVisionRecipePinGapIntentSkill.RoiSample roi)
        {
            if (roi.X < 0 || roi.Y < 0 || roi.Width <= 0 || roi.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roi), "The coarse ROI must have a non-negative origin and positive size.");
            }

            VisionPipeline pipeline = new VisionPipeline
            {
                Name = "Dark_Band_Gap_Measurement"
            };
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = "Detect Dark Band Gap Edges",
                ToolType = "LineDistance",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = "GapMeasured",
                UseAcceptance = false,
                ExpectedSuccess = true
            };

            step.Parameters["Name"] = "DarkBandGapEdgePair";
            step.Parameters["PIXELPERMM"] = "0";
            step.Parameters["USE_ROI"] = "true";
            step.Parameters["CvROI"] = roi.ToText();
            step.Parameters[VisionPipelineGapEdgePairTool.UseParameter] = "true";
            step.Parameters["CANNY_LOW"] = DefaultCannyLow.ToString(CultureInfo.InvariantCulture);
            step.Parameters["CANNY_HIGH"] = DefaultCannyHigh.ToString(CultureInfo.InvariantCulture);
            AddDouble(step, VisionPipelineGapEdgePairTool.MinimumGapParameter, DefaultMinimumGapPixels);
            AddDouble(step, VisionPipelineGapEdgePairTool.MaximumGapParameter, DefaultMaximumGapPixels);
            AddDouble(step, VisionPipelineGapEdgePairTool.MaximumAngleParameter, DefaultMaximumAngleDegrees);
            AddDouble(step, VisionPipelineGapEdgePairTool.MaximumParallelDeltaParameter, DefaultMaximumParallelDeltaDegrees);
            AddDouble(step, VisionPipelineGapEdgePairTool.MinimumSupportRatioParameter, DefaultMinimumSupportRatio);
            AddDouble(step, VisionPipelineGapEdgePairTool.MinimumDarkContrastParameter, DefaultMinimumDarkContrast);
            AddDouble(step, VisionPipelineGapEdgePairTool.MinimumDarkCoverageParameter, DefaultMinimumDarkCoverageRatio);
            AddDouble(step, VisionPipelineGapEdgePairTool.MinimumScoreMarginParameter, DefaultMinimumScoreMargin);
            pipeline.Steps.Add(step);
            return pipeline;
        }

        private static void AddDouble(VisionPipelineStep step, string key, double value)
        {
            step.Parameters[key] = value.ToString("0.######", CultureInfo.InvariantCulture);
        }
    }
}
