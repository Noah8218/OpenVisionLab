using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeContourCountIntentSkill
    {
        public static VisionPipeline CreatePipeline(
            int roiX,
            int roiY,
            int roiWidth,
            int roiHeight,
            int threshold,
            int minCount,
            int maxCount,
            int minArea,
            int maxArea)
        {
            string roiText = FormatRoi(roiX, roiY, roiWidth, roiHeight);
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_ContourCountSize_Skill" };

            VisionPipelineStep thresholdStep = CreateStep("01 Contour Binary", "Threshold", "Main", "ContourCount_Binary");
            thresholdStep.Parameters["Mode"] = "Threshold";
            thresholdStep.Parameters["Threshold"] = threshold.ToString(CultureInfo.InvariantCulture);
            thresholdStep.Parameters["MaxValue"] = "255";
            thresholdStep.Parameters["ThresholdType"] = "Binary";
            pipeline.Steps.Add(thresholdStep);

            VisionPipelineStep countStep = CreateStep("02 Contour Count", "Contour", "ContourCount_Binary", "ContourCount_Result");
            ApplyContourParameters(countStep, "Contour_Count", roiText, minArea, maxArea);
            countStep.UseAcceptance = true;
            countStep.ExpectedSuccess = true;
            countStep.MaxElapsedMilliseconds = 1000;
            countStep.AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount;
            countStep.UseAcceptanceMetricMinimum = true;
            countStep.AcceptanceMetricMinimum = minCount;
            countStep.UseAcceptanceMetricMaximum = true;
            countStep.AcceptanceMetricMaximum = maxCount;
            pipeline.Steps.Add(countStep);

            VisionPipelineStep sizeStep = CreateStep("03 Contour Size Guard", "Contour", "ContourCount_Binary", "ContourSize_Result");
            ApplyContourParameters(sizeStep, "Contour_Size", roiText, minArea, maxArea);
            sizeStep.Parameters["ALLOW_BRANCH_INPUT"] = "true";
            sizeStep.UseAcceptance = true;
            sizeStep.ExpectedSuccess = true;
            sizeStep.MaxElapsedMilliseconds = 1000;
            sizeStep.AcceptanceMetricName = VisionPipelineKnownMetrics.AreaMax;
            sizeStep.UseAcceptanceMetricMaximum = true;
            sizeStep.AcceptanceMetricMaximum = maxArea;
            pipeline.Steps.Add(sizeStep);

            VisionPipelineStep reviewStep = CreateStep("04 Contour Review Overlay", "OverlayMerge", "Main", "ContourCount_Review");
            reviewStep.Parameters["SourceLayers"] = "ContourSize_Result";
            reviewStep.Parameters["BurnIn"] = "true";
            reviewStep.Parameters["DrawLabels"] = "true";
            reviewStep.Parameters["AllowEmpty"] = "false";
            pipeline.Steps.Add(reviewStep);

            return pipeline;
        }

        public static bool TryParseRoi(
            string text,
            out int x,
            out int y,
            out int width,
            out int height,
            out string message)
        {
            return OpenVisionRecipePinGapIntentSkill.TryParseRoi(text, out x, out y, out width, out height, out message);
        }

        public static bool TryParseByte(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParseByte(text, out value);
        }

        public static bool TryParseNonNegativeInt(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(text, out value);
        }

        public static bool TryParsePositiveInt(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(text, out value);
        }

        public static bool TryExtractMetricValue(string metricText, string metricName, out double value)
        {
            return OpenVisionRecipePinGapIntentSkill.TryExtractMetricValue(metricText, metricName, out value);
        }

        public static string FormatRoi(int x, int y, int width, int height)
        {
            return OpenVisionRecipePinGapIntentSkill.FormatRoi(x, y, width, height);
        }

        private static VisionPipelineStep CreateStep(string name, string toolType, string inputLayer, string outputLayer)
        {
            return new VisionPipelineStep
            {
                Name = name,
                ToolType = toolType,
                Enabled = true,
                InputLayer = inputLayer,
                OutputLayer = outputLayer
            };
        }

        private static void ApplyContourParameters(
            VisionPipelineStep step,
            string name,
            string roiText,
            int minArea,
            int maxArea)
        {
            step.Parameters["Name"] = name;
            step.Parameters["PIXELPERMM"] = "1";
            step.Parameters["USE_THRESHOLD"] = "false";
            step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            step.Parameters["USE_BITWISENOT"] = "false";
            step.Parameters["USE_ROI"] = "true";
            step.Parameters["USE_MULTI_ROI"] = "false";
            step.Parameters["USE_DRAW_IMAGE"] = "false";
            step.Parameters["CvROI"] = roiText;
            step.Parameters["ApproximationModes"] = "ApproxSimple";
            step.Parameters["DetectMode"] = "External";
            step.Parameters["MIN_AREA"] = minArea.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MAX_AREA"] = maxArea.ToString(CultureInfo.InvariantCulture);
            step.Parameters["ClrGridHtml"] = "#00ff00";
            step.Parameters["DrawThickness"] = "2";
        }
    }
}
