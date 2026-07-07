using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipePinGapIntentSkill
    {
        public static VisionPipeline CreatePipeline(
            int roiX,
            int roiY,
            int roiWidth,
            int roiHeight,
            double minDistanceMm,
            double maxDistanceMm,
            double maxRangeMm,
            double mmPerPixel)
        {
            string roiText = FormatRoi(roiX, roiY, roiWidth, roiHeight);
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_PinGap_DistanceSkill" };

            VisionPipelineStep distanceStep = CreateStep("01 Pin Gap Distance", "LineDistance", "Main", "PinGap_Distance_Value");
            ApplyLineDistanceParameters(distanceStep, "PinGap_Distance_Value", roiText, mmPerPixel);
            distanceStep.UseAcceptance = true;
            distanceStep.ExpectedSuccess = true;
            distanceStep.MaxElapsedMilliseconds = 500;
            distanceStep.AcceptanceMetricName = VisionPipelineKnownMetrics.DistanceMmAvg;
            distanceStep.UseAcceptanceMetricMinimum = true;
            distanceStep.AcceptanceMetricMinimum = minDistanceMm;
            distanceStep.UseAcceptanceMetricMaximum = true;
            distanceStep.AcceptanceMetricMaximum = maxDistanceMm;
            pipeline.Steps.Add(distanceStep);

            VisionPipelineStep consistencyStep = CreateStep("02 Pin Gap Consistency", "LineDistance", "Main", "PinGap_Distance_Consistency");
            ApplyLineDistanceParameters(consistencyStep, "PinGap_Distance_Consistency", roiText, mmPerPixel);
            consistencyStep.Parameters["ALLOW_BRANCH_INPUT"] = "true";
            consistencyStep.UseAcceptance = true;
            consistencyStep.ExpectedSuccess = true;
            consistencyStep.MaxElapsedMilliseconds = 500;
            consistencyStep.AcceptanceMetricName = VisionPipelineKnownMetrics.DistanceMmRange;
            consistencyStep.UseAcceptanceMetricMaximum = true;
            consistencyStep.AcceptanceMetricMaximum = maxRangeMm;
            pipeline.Steps.Add(consistencyStep);

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
            x = 0;
            y = 0;
            width = 0;
            height = 0;
            message = string.Empty;

            string[] parts = (text ?? string.Empty)
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                message = "ROI must contain four numbers.";
                return false;
            }

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out x)
                || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out y)
                || !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out width)
                || !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out height)
                || width <= 0
                || height <= 0)
            {
                message = "ROI width and height must be positive integers.";
                return false;
            }

            return true;
        }

        public static bool TryParsePositiveDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && value > 0;
        }

        public static bool TryExtractMetricValue(string metricText, string metricName, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(metricText) || string.IsNullOrWhiteSpace(metricName))
            {
                return false;
            }

            string prefix = metricName + "=";
            int index = metricText.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return false;
            }

            int start = index + prefix.Length;
            int end = start;
            while (end < metricText.Length && IsMetricNumberChar(metricText[end]))
            {
                end++;
            }

            return end > start
                && double.TryParse(metricText.Substring(start, end - start), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public static string FormatRoi(int x, int y, int width, int height)
        {
            return x.ToString(CultureInfo.InvariantCulture)
                + ","
                + y.ToString(CultureInfo.InvariantCulture)
                + ","
                + width.ToString(CultureInfo.InvariantCulture)
                + ","
                + height.ToString(CultureInfo.InvariantCulture);
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

        private static void ApplyLineDistanceParameters(
            VisionPipelineStep step,
            string name,
            string roiText,
            double mmPerPixel)
        {
            step.Parameters["Name"] = name;
            step.Parameters["PIXELPERMM"] = mmPerPixel.ToString("0.######", CultureInfo.InvariantCulture);
            step.Parameters["USE_THRESHOLD"] = "false";
            step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            step.Parameters["USE_BITWISENOT"] = "false";
            step.Parameters["USE_ROI"] = "true";
            step.Parameters["CvROI"] = roiText;
            step.Parameters["LeftPRJ_DIR"] = "X_LTOR";
            step.Parameters["RightPRJ_DIR"] = "X_RTOL";
            step.Parameters["PRJ_PORALITY"] = "WTOB";
            step.Parameters["CONTRAST"] = "18";
            step.Parameters["THICKNESS"] = "2";
            step.Parameters["SAMPLING_STEP"] = "16";
            step.Parameters["POINT_RANGE"] = "8";
            step.Parameters["VER_PRJ_DIR"] = "X_RTOL";
            step.Parameters["USE_MANUAL_ANGLE"] = "true";
            step.Parameters["MANUAL_ANGLE_VALUE"] = "89";
            step.Parameters["SHOW_EDGE"] = "true";
            step.Parameters["SHOW_VERTICAL_LINE"] = "true";
        }

        private static bool IsMetricNumberChar(char value)
        {
            return char.IsDigit(value)
                || value == '.'
                || value == '-'
                || value == '+'
                || value == 'e'
                || value == 'E';
        }
    }
}
