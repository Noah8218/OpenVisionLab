using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeFeatureMatchingIntentSkill
    {
        public static VisionPipeline CreatePipeline(
            string templatePath,
            double scoreMinimum,
            double ransacReprojectionThreshold,
            double acceptanceScoreMinimum)
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_FeatureMatchingTarget_Skill" };
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = "01 Feature Target Match",
                ToolType = "FeatureMatching",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = "Feature_Preview",
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
            step.Parameters["Name"] = "Feature_Target_Match";
            step.Parameters["TemplatePath"] = normalizedTemplatePath;
            step.Parameters["PATTERN_PATH"] = normalizedTemplatePath;
            step.Parameters["SCORE_MIN"] = scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture);
            step.Parameters["RANSAC_REPROJ_THRESHOLD"] = ransacReprojectionThreshold.ToString("0.###", CultureInfo.InvariantCulture);
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

        public static bool TryParsePositiveDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value > 0D;
        }

        public static bool TryParseAcceptanceScoreMinimum(string text, out double value)
        {
            return TryParsePositiveDouble(text, out value) && value <= 100D;
        }
    }
}
