using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeEdgeBasedMatchingIntentSkill
    {
        public static VisionPipeline CreatePipeline(
            string templatePath,
            double scoreMinimum,
            int searchCount,
            int cannyLow,
            int cannyHigh,
            double acceptanceScoreMinimum)
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_EdgeBasedMatchingTarget_Skill" };
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = "01 Edge Shape Match",
                ToolType = "EdgeBasedMatching",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = "EdgeBased_Preview",
                UseAcceptance = true,
                ExpectedSuccess = true,
                MaxElapsedMilliseconds = 3000,
                AcceptanceMetricName = VisionPipelineKnownMetrics.ScoreMax,
                UseAcceptanceMetricMinimum = true,
                AcceptanceMetricMinimum = acceptanceScoreMinimum,
                UseAcceptanceMetricMaximum = true,
                AcceptanceMetricMaximum = 100
            };

            string normalizedTemplatePath = (templatePath ?? string.Empty).Trim();
            step.Parameters["Name"] = "Edge_Shape_Match";
            step.Parameters["TemplatePath"] = normalizedTemplatePath;
            step.Parameters["PATTERN_PATH"] = normalizedTemplatePath;
            step.Parameters["SCORE_MIN"] = scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture);
            step.Parameters["NUM_MATCH"] = searchCount.ToString(CultureInfo.InvariantCulture);
            step.Parameters["CANNY_LOW"] = cannyLow.ToString(CultureInfo.InvariantCulture);
            step.Parameters["CANNY_HIGH"] = cannyHigh.ToString(CultureInfo.InvariantCulture);
            step.Parameters["CANNY_APERTURE_SIZE"] = "3";
            step.Parameters["USE_L2_GRADIENT"] = "true";
            step.Parameters["CONTOUR_RETRIEVAL_MODE"] = "External";
            step.Parameters["CONTOUR_APPROXIMATION_MODE"] = "ApproxNone";
            step.Parameters["GREEDINESS"] = "0.90";
            step.Parameters["SEARCH_STEP"] = "1";
            step.Parameters["MAX_TEMPLATE_POINTS"] = "260";
            step.Parameters["MIN_GRADIENT_MAGNITUDE"] = "1";
            step.Parameters["USE_DRAW_IMAGE"] = "true";
            step.Parameters["USE_FIND_ANGLE"] = "false";
            step.Parameters["USE_POSITION_REFINE"] = "true";
            step.Parameters["USE_HYBRID_VERIFY"] = "false";
            step.Parameters["USE_THRESHOLD"] = "false";
            step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            step.Parameters["USE_ROI"] = "false";
            pipeline.Steps.Add(step);
            return pipeline;
        }

        public static bool TryParseScore(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value >= 0D
                && value <= 1D;
        }

        public static bool TryParsePositiveInt(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(text, out value);
        }

        public static bool TryParseByte(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParseByte(text, out value);
        }

        public static bool TryParseAcceptanceScoreMinimum(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value > 0D
                && value <= 100D;
        }
    }
}
