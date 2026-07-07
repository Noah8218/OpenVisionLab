using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeBlobCountIntentSkill
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
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_BlobCount_Skill" };

            VisionPipelineStep thresholdStep = CreateStep("01 Blob Count Binary", "Threshold", "Main", "BlobCount_Binary");
            thresholdStep.Parameters["Mode"] = "Threshold";
            thresholdStep.Parameters["Threshold"] = threshold.ToString(CultureInfo.InvariantCulture);
            thresholdStep.Parameters["MaxValue"] = "255";
            thresholdStep.Parameters["ThresholdType"] = "Binary";
            pipeline.Steps.Add(thresholdStep);

            VisionPipelineStep blobStep = CreateStep("02 Blob Count Inspect", "Blob", "BlobCount_Binary", "BlobCount_Result");
            blobStep.Parameters["Name"] = "Blob_Count";
            blobStep.Parameters["PIXELPERMM"] = "1";
            blobStep.Parameters["USE_THRESHOLD"] = "false";
            blobStep.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            blobStep.Parameters["USE_BITWISENOT"] = "false";
            blobStep.Parameters["USE_ROI"] = "true";
            blobStep.Parameters["USE_MULTI_ROI"] = "false";
            blobStep.Parameters["CvROI"] = roiText;
            blobStep.Parameters["MIN_AREA"] = minArea.ToString(CultureInfo.InvariantCulture);
            blobStep.Parameters["MAX_AREA"] = maxArea.ToString(CultureInfo.InvariantCulture);
            blobStep.UseAcceptance = true;
            blobStep.ExpectedSuccess = true;
            blobStep.MaxElapsedMilliseconds = 1000;
            blobStep.AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount;
            blobStep.UseAcceptanceMetricMinimum = true;
            blobStep.AcceptanceMetricMinimum = minCount;
            blobStep.UseAcceptanceMetricMaximum = true;
            blobStep.AcceptanceMetricMaximum = maxCount;
            pipeline.Steps.Add(blobStep);

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
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                && value >= 0
                && value <= 255;
        }

        public static bool TryParseNonNegativeInt(string text, out int value)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                && value >= 0;
        }

        public static bool TryParsePositiveInt(string text, out int value)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                && value > 0;
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
    }
}
