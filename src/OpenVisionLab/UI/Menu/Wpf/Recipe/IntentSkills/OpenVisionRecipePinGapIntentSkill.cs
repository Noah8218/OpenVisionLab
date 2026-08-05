using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipePinGapIntentSkill
    {
        private const int DefaultReferenceWidth = 768;
        private const int DefaultReferenceHeight = 576;
        private const int MinScaledRoiSize = 8;

        public static readonly IReadOnlyList<RoiSample> DefaultRoiSamples = new[]
        {
            new RoiSample(42, 150, 80, 80),
            new RoiSample(151, 150, 80, 80),
            new RoiSample(424, 150, 80, 80),
            new RoiSample(478, 150, 80, 80)
        };

        public static string DefaultRoiSamplesText => FormatRoiSamples(DefaultRoiSamples);

        public static IReadOnlyList<RoiSample> CreateScaledRoiSamples(int imageWidth, int imageHeight)
        {
            if (imageWidth <= 0 || imageHeight <= 0)
            {
                return DefaultRoiSamples;
            }

            double scaleX = imageWidth / (double)DefaultReferenceWidth;
            double scaleY = imageHeight / (double)DefaultReferenceHeight;
            return DefaultRoiSamples
                .Select(sample => ScaleAndClampRoi(sample, scaleX, scaleY, imageWidth, imageHeight))
                .ToArray();
        }

        public static VisionPipeline CreatePipeline(
            IReadOnlyList<RoiSample> roiSamples,
            double minDistanceMm,
            double maxDistanceMm,
            double maxRangeMm,
            double mmPerPixel)
        {
            return CreatePipeline(
                roiSamples,
                minDistanceMm,
                maxDistanceMm,
                maxRangeMm,
                mmPerPixel,
                VisionPipelineKnownMetrics.DistanceMmAvg,
                VisionPipelineKnownMetrics.DistanceMmRange);
        }

        public static VisionPipeline CreatePixelPipeline(
            IReadOnlyList<RoiSample> roiSamples,
            double minDistancePx,
            double maxDistancePx,
            double maxRangePx)
        {
            return CreatePipeline(
                roiSamples,
                minDistancePx,
                maxDistancePx,
                maxRangePx,
                0,
                VisionPipelineKnownMetrics.DistancePxAvg,
                VisionPipelineKnownMetrics.DistancePxRange);
        }

        private static VisionPipeline CreatePipeline(
            IReadOnlyList<RoiSample> roiSamples,
            double minimumDistance,
            double maximumDistance,
            double maximumRange,
            double mmPerPixel,
            string averageMetricName,
            string rangeMetricName)
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_PinGap_DistanceSkill" };
            string[] sampleNames = { "LeftA", "LeftB", "Center", "Right" };
            string[] reviewLayers = new string[roiSamples.Count];

            for (int index = 0; index < roiSamples.Count; index++)
            {
                string sampleName = index < sampleNames.Length
                    ? sampleNames[index]
                    : "Sample" + (index + 1).ToString(CultureInfo.InvariantCulture);
                string roiText = roiSamples[index].ToText();
                int avgStepNumber = index * 2 + 1;
                int rangeStepNumber = avgStepNumber + 1;
                string avgLayer = "PinArray_" + sampleName + "_Avg";
                string rangeLayer = "PinArray_" + sampleName + "_Range";

                VisionPipelineStep distanceStep = CreateStep(
                    FormatStepName(avgStepNumber, "Pin Array " + sampleName + " Avg"),
                    "LineDistance",
                    "Main",
                    avgLayer);
                ApplyLineDistanceParameters(distanceStep, avgLayer, roiText, mmPerPixel);
                if (index > 0)
                {
                    distanceStep.Parameters["ALLOW_BRANCH_INPUT"] = "true";
                }

                distanceStep.UseAcceptance = true;
                distanceStep.ExpectedSuccess = true;
                distanceStep.MaxElapsedMilliseconds = 500;
                distanceStep.AcceptanceMetricName = averageMetricName;
                distanceStep.UseAcceptanceMetricMinimum = true;
                distanceStep.AcceptanceMetricMinimum = minimumDistance;
                distanceStep.UseAcceptanceMetricMaximum = true;
                distanceStep.AcceptanceMetricMaximum = maximumDistance;
                pipeline.Steps.Add(distanceStep);

                VisionPipelineStep consistencyStep = CreateStep(
                    FormatStepName(rangeStepNumber, "Pin Array " + sampleName + " Range"),
                    "LineDistance",
                    "Main",
                    rangeLayer);
                ApplyLineDistanceParameters(consistencyStep, rangeLayer, roiText, mmPerPixel);
                consistencyStep.Parameters["ALLOW_BRANCH_INPUT"] = "true";
                consistencyStep.UseAcceptance = true;
                consistencyStep.ExpectedSuccess = true;
                consistencyStep.MaxElapsedMilliseconds = 500;
                consistencyStep.AcceptanceMetricName = rangeMetricName;
                consistencyStep.UseAcceptanceMetricMaximum = true;
                consistencyStep.AcceptanceMetricMaximum = maximumRange;
                pipeline.Steps.Add(consistencyStep);
                reviewLayers[index] = rangeLayer;
            }

            VisionPipelineStep reviewStep = CreateStep(
                FormatStepName(roiSamples.Count * 2 + 1, "Pin Array Review Overlay"),
                "OverlayMerge",
                "Main",
                "PinArray_Review");
            reviewStep.Parameters["SourceLayers"] = string.Join(";", reviewLayers);
            reviewStep.Parameters["BurnIn"] = "true";
            reviewStep.Parameters["DrawLabels"] = "true";
            reviewStep.Parameters["AllowEmpty"] = "false";
            pipeline.Steps.Add(reviewStep);

            return pipeline;
        }

        public static bool TryParseRoiSamples(
            string text,
            out IReadOnlyList<RoiSample> samples,
            out string message)
        {
            samples = Array.Empty<RoiSample>();
            message = string.Empty;

            if (TryParseRoi(text, out int x, out int y, out int width, out int height, out _))
            {
                samples = new[] { new RoiSample(x, y, width, height) };
                return true;
            }

            string[] groups = (text ?? string.Empty)
                .Replace("\r\n", ";", StringComparison.Ordinal)
                .Replace('\n', ';')
                .Replace('|', ';')
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (groups.Length == 0)
            {
                message = "ROI samples must contain at least one x,y,w,h group.";
                return false;
            }

            RoiSample[] parsed = new RoiSample[groups.Length];
            for (int index = 0; index < groups.Length; index++)
            {
                if (!TryParseRoi(groups[index], out x, out y, out width, out height, out message))
                {
                    message = "ROI sample " + (index + 1).ToString(CultureInfo.InvariantCulture) + ": " + message;
                    return false;
                }

                parsed[index] = new RoiSample(x, y, width, height);
            }

            samples = parsed;
            return true;
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

        public static string FormatRoiSamples(IEnumerable<RoiSample> samples)
        {
            return string.Join(";", (samples ?? Array.Empty<RoiSample>()).Select(sample => sample.ToText()));
        }

        private static RoiSample ScaleAndClampRoi(
            RoiSample sample,
            double scaleX,
            double scaleY,
            int imageWidth,
            int imageHeight)
        {
            int width = ClampSize((int)Math.Round(sample.Width * scaleX), imageWidth);
            int height = ClampSize((int)Math.Round(sample.Height * scaleY), imageHeight);
            int x = Clamp((int)Math.Round(sample.X * scaleX), 0, Math.Max(0, imageWidth - width));
            int y = Clamp((int)Math.Round(sample.Y * scaleY), 0, Math.Max(0, imageHeight - height));
            return new RoiSample(x, y, width, height);
        }

        private static int ClampSize(int value, int limit)
        {
            if (limit <= 0)
            {
                return 1;
            }

            int minimum = Math.Min(MinScaledRoiSize, limit);
            return Clamp(Math.Max(value, minimum), 1, limit);
        }

        private static int Clamp(int value, int minimum, int maximum)
        {
            if (maximum < minimum)
            {
                return minimum;
            }

            return Math.Min(Math.Max(value, minimum), maximum);
        }

        private static string FormatStepName(int stepNumber, string text)
        {
            return stepNumber.ToString("00", CultureInfo.InvariantCulture) + " " + text;
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

        public readonly struct RoiSample
        {
            public RoiSample(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public int X { get; }

            public int Y { get; }

            public int Width { get; }

            public int Height { get; }

            public string ToText()
            {
                return FormatRoi(X, Y, Width, Height);
            }
        }
    }
}
