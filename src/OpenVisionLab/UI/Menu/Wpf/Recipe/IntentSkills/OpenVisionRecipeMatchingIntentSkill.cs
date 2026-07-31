using Lib.OpenCV.Pipeline;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeMatchingIntentSkill
    {
        public static VisionPipeline CreatePipeline(
            string templatePath,
            int roiX,
            int roiY,
            int roiWidth,
            int roiHeight,
            double scoreMinimum,
            int expectedCount)
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_MatchingTargetPresence_Skill" };
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = "01 Template Target Presence",
                ToolType = "Matching",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = "Matching_Target_Result",
                UseAcceptance = true,
                ExpectedSuccess = true,
                MaxElapsedMilliseconds = 1000,
                AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount,
                UseAcceptanceMetricMinimum = true,
                AcceptanceMetricMinimum = expectedCount,
                UseAcceptanceMetricMaximum = true,
                AcceptanceMetricMaximum = expectedCount
            };

            string normalizedTemplatePath = (templatePath ?? string.Empty).Trim();
            step.Parameters["Name"] = "Matching_Target_Presence";
            step.Parameters["TemplatePath"] = normalizedTemplatePath;
            step.Parameters["PATTERN_PATH"] = normalizedTemplatePath;
            step.Parameters["MATCH_MODE"] = "CCoeffNormed";
            step.Parameters["SCORE_MIN"] = scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture);
            step.Parameters["MAGNIFIATION"] = "1";
            step.Parameters["NUM_MATCH"] = expectedCount.ToString(CultureInfo.InvariantCulture);
            step.Parameters["USE_FIND_ANGLE"] = "false";
            step.Parameters["USE_CANNY"] = "false";
            step.Parameters["USE_THRESHOLD"] = "false";
            step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            step.Parameters["USE_ROI"] = "true";
            step.Parameters["USE_MULTI_ROI"] = "false";
            step.Parameters["CvROI"] = OpenVisionRecipeBlobCountIntentSkill.FormatRoi(roiX, roiY, roiWidth, roiHeight);
            pipeline.Steps.Add(step);
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
            return OpenVisionRecipeBlobCountIntentSkill.TryParseRoi(text, out x, out y, out width, out height, out message);
        }

        public static bool TryParseScore(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && value >= 0D
                && value <= 1D;
        }

        public static bool TryParsePositiveInt(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(text, out value);
        }
    }
}
