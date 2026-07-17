using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeMeanIntentSkill
    {
        private static readonly IReadOnlyList<string> SupportedMeanTypes =
            Array.AsReadOnly(Enum.GetNames(typeof(MeanType)));

        public static IReadOnlyList<string> MeanTypeOptions => SupportedMeanTypes;

        public static VisionPipeline CreatePipeline(
            bool useRoi,
            int roiX,
            int roiY,
            int roiWidth,
            int roiHeight,
            MeanType meanType,
            int minimum,
            int maximum)
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_MeanBrightnessDrift_Skill" };
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = "01 Mean Brightness Drift",
                ToolType = "Mean",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = "Mean_Brightness_Result",
                UseAcceptance = true,
                ExpectedSuccess = true,
                MaxElapsedMilliseconds = 300,
                AcceptanceMetricName = VisionPipelineKnownMetrics.MeanValueAvg,
                UseAcceptanceMetricMinimum = true,
                AcceptanceMetricMinimum = minimum,
                UseAcceptanceMetricMaximum = true,
                AcceptanceMetricMaximum = maximum
            };

            step.Parameters["Name"] = "Mean_Brightness_Drift";
            step.Parameters["MEAN_TYPES"] = meanType.ToString();
            step.Parameters["MEAN_MIN"] = minimum.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MEAN_MAX"] = maximum.ToString(CultureInfo.InvariantCulture);
            step.Parameters["USE_THRESHOLD"] = "false";
            step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            step.Parameters["USE_BITWISENOT"] = "false";
            step.Parameters["USE_ROI"] = useRoi ? "true" : "false";
            step.Parameters["USE_MULTI_ROI"] = "false";
            if (useRoi)
            {
                step.Parameters["CvROI"] = OpenVisionRecipeBlobCountIntentSkill.FormatRoi(roiX, roiY, roiWidth, roiHeight);
            }

            pipeline.Steps.Add(step);
            return pipeline;
        }

        public static bool TryParseOptionalRoi(
            string text,
            out bool useRoi,
            out int x,
            out int y,
            out int width,
            out int height,
            out string message)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                useRoi = false;
                x = 0;
                y = 0;
                width = 0;
                height = 0;
                message = string.Empty;
                return true;
            }

            useRoi = true;
            return OpenVisionRecipeBlobCountIntentSkill.TryParseRoi(text, out x, out y, out width, out height, out message);
        }

        public static bool TryParseMeanType(string text, out MeanType value)
        {
            return Enum.TryParse(text, true, out value)
                && Enum.IsDefined(typeof(MeanType), value);
        }

        public static bool TryParseByte(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParseByte(text, out value);
        }
    }
}
