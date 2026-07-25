using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipePinArrayGapIntentSkill
    {
        public const string SupportedMeasurementDefinition = "Adjacent edge-to-edge clearance";
        public const string SupportedPinPolarity = "Dark";
        public const string SupportedUnitMode = "px";

        public const int DefaultDarkThreshold = 128;
        public const double DefaultMinimumDarkCoverageRatio = 0.55;
        public const int DefaultMinimumPinWidth = 5;
        public const int DefaultMaximumPinBreakWidth = 2;
        public const int DefaultMinimumGapWidth = 3;

        public static bool TryParseRowRois(
            string text,
            out IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
            out string message)
        {
            if (!OpenVisionRecipePinGapIntentSkill.TryParseRoiSamples(text, out rowRois, out message))
            {
                return false;
            }

            for (int index = 0; index < rowRois.Count; index++)
            {
                OpenVisionRecipePinGapIntentSkill.RoiSample roi = rowRois[index];
                if (roi.X < 0 || roi.Y < 0)
                {
                    message = "Row ROI "
                        + (index + 1).ToString(CultureInfo.InvariantCulture)
                        + ": x and y must be non-negative integers.";
                    rowRois = Array.Empty<OpenVisionRecipePinGapIntentSkill.RoiSample>();
                    return false;
                }
            }

            return true;
        }

        public static bool TryValidateV1Inputs(
            string measurementDefinition,
            string pinPolarity,
            string unitMode,
            IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
            int sourceWidth,
            int sourceHeight,
            int darkThreshold,
            double minimumDarkCoverageRatio,
            int minimumPinWidth,
            int maximumPinBreakWidth,
            int minimumGapWidth,
            out string message)
        {
            if (!EqualsSupported(measurementDefinition, SupportedMeasurementDefinition))
            {
                message = "WAIT - unsupported: v1 measures adjacent edge-to-edge clearance, not center pitch.";
                return false;
            }

            if (!EqualsSupported(pinPolarity, SupportedPinPolarity))
            {
                message = "WAIT - unsupported: v1 detects dark pins only.";
                return false;
            }

            if (!EqualsSupported(unitMode, SupportedUnitMode))
            {
                message = "WAIT - calibration required: v1 judgement uses px only.";
                return false;
            }

            if (!TryValidateDetectionValues(
                    darkThreshold,
                    minimumDarkCoverageRatio,
                    minimumPinWidth,
                    maximumPinBreakWidth,
                    minimumGapWidth,
                    out message))
            {
                return false;
            }

            if (sourceWidth <= 0 || sourceHeight <= 0)
            {
                message = "A source image with positive width and height is required.";
                return false;
            }

            if (rowRois == null || rowRois.Count == 0)
            {
                message = "At least one reviewed single-row ROI is required.";
                return false;
            }

            for (int index = 0; index < rowRois.Count; index++)
            {
                OpenVisionRecipePinGapIntentSkill.RoiSample roi = rowRois[index];
                if (!IsInsideSource(roi, sourceWidth, sourceHeight))
                {
                    message = "Row ROI "
                        + (index + 1).ToString(CultureInfo.InvariantCulture)
                        + " must be positive and fully inside the source image.";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        public static VisionPipeline CreateMeasurementPipeline(
            IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
            int darkThreshold,
            double minimumDarkCoverageRatio,
            int minimumPinWidth,
            int maximumPinBreakWidth,
            int minimumGapWidth)
        {
            ValidateBuildInputs(
                rowRois,
                darkThreshold,
                minimumDarkCoverageRatio,
                minimumPinWidth,
                maximumPinBreakWidth,
                minimumGapWidth);

            return CreatePipeline(
                rowRois,
                darkThreshold,
                minimumDarkCoverageRatio,
                minimumPinWidth,
                maximumPinBreakWidth,
                minimumGapWidth,
                judged: false,
                maximumDistancePxRange: 0D);
        }

        public static VisionPipeline CreateJudgedPipeline(
            IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
            int darkThreshold,
            double minimumDarkCoverageRatio,
            int minimumPinWidth,
            int maximumPinBreakWidth,
            int minimumGapWidth,
            double maximumDistancePxRange)
        {
            ValidateBuildInputs(
                rowRois,
                darkThreshold,
                minimumDarkCoverageRatio,
                minimumPinWidth,
                maximumPinBreakWidth,
                minimumGapWidth);
            if (!IsFinitePositive(maximumDistancePxRange))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumDistancePxRange),
                    "The judged DistancePxRange maximum must be a positive finite value.");
            }

            return CreatePipeline(
                rowRois,
                darkThreshold,
                minimumDarkCoverageRatio,
                minimumPinWidth,
                maximumPinBreakWidth,
                minimumGapWidth,
                judged: true,
                maximumDistancePxRange);
        }

        private static VisionPipeline CreatePipeline(
            IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
            int darkThreshold,
            double minimumDarkCoverageRatio,
            int minimumPinWidth,
            int maximumPinBreakWidth,
            int minimumGapWidth,
            bool judged,
            double maximumDistancePxRange)
        {
            VisionPipeline pipeline = new VisionPipeline
            {
                Name = judged
                    ? "Pin_Row_EdgeGap_Consistency"
                    : "Pin_Row_EdgeGap_Measurement"
            };

            for (int index = 0; index < rowRois.Count; index++)
            {
                int rowNumber = index + 1;
                string rowText = rowNumber.ToString("00", CultureInfo.InvariantCulture);
                string suffix = judged ? "Range" : "Measure";
                string outputLayer = "Pin_Row_" + rowText + "_EdgeGap_" + suffix;
                VisionPipelineStep step = new VisionPipelineStep
                {
                    Name = rowText + " Row " + rowNumber.ToString(CultureInfo.InvariantCulture) + " Edge Gap " + suffix,
                    ToolType = "PinArrayGap",
                    Enabled = true,
                    InputLayer = "Main",
                    OutputLayer = outputLayer
                };

                if (index > 0)
                {
                    step.Parameters["ALLOW_BRANCH_INPUT"] = "true";
                }

                ApplyDetectionParameters(
                    step,
                    outputLayer,
                    rowRois[index],
                    darkThreshold,
                    minimumDarkCoverageRatio,
                    minimumPinWidth,
                    maximumPinBreakWidth,
                    minimumGapWidth);

                if (judged)
                {
                    step.UseAcceptance = true;
                    step.ExpectedSuccess = true;
                    step.MaxElapsedMilliseconds = 200;
                    step.AcceptanceMetricName = VisionPipelineKnownMetrics.DistancePxRange;
                    step.UseAcceptanceMetricMaximum = true;
                    step.AcceptanceMetricMaximum = maximumDistancePxRange;
                }

                pipeline.Steps.Add(step);
            }

            return pipeline;
        }

        private static void ApplyDetectionParameters(
            VisionPipelineStep step,
            string name,
            OpenVisionRecipePinGapIntentSkill.RoiSample roi,
            int darkThreshold,
            double minimumDarkCoverageRatio,
            int minimumPinWidth,
            int maximumPinBreakWidth,
            int minimumGapWidth)
        {
            step.Parameters["Name"] = name;
            step.Parameters["USE_ROI"] = "true";
            step.Parameters["CvROI"] = roi.ToText();
            step.Parameters["DarkThreshold"] = darkThreshold.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MinDarkCoverageRatio"] = minimumDarkCoverageRatio.ToString("0.######", CultureInfo.InvariantCulture);
            step.Parameters["MinPinWidth"] = minimumPinWidth.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MaxPinBreakWidth"] = maximumPinBreakWidth.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MinGapWidth"] = minimumGapWidth.ToString(CultureInfo.InvariantCulture);
        }

        private static void ValidateBuildInputs(
            IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
            int darkThreshold,
            double minimumDarkCoverageRatio,
            int minimumPinWidth,
            int maximumPinBreakWidth,
            int minimumGapWidth)
        {
            if (rowRois == null || rowRois.Count == 0)
            {
                throw new ArgumentException("At least one reviewed single-row ROI is required.", nameof(rowRois));
            }

            for (int index = 0; index < rowRois.Count; index++)
            {
                OpenVisionRecipePinGapIntentSkill.RoiSample roi = rowRois[index];
                if (roi.X < 0 || roi.Y < 0 || roi.Width <= 0 || roi.Height <= 0)
                {
                    throw new ArgumentException(
                        "Every row ROI must have a non-negative origin and positive size.",
                        nameof(rowRois));
                }
            }

            if (!TryValidateDetectionValues(
                    darkThreshold,
                    minimumDarkCoverageRatio,
                    minimumPinWidth,
                    maximumPinBreakWidth,
                    minimumGapWidth,
                    out string message))
            {
                throw new ArgumentOutOfRangeException(nameof(darkThreshold), message);
            }
        }

        private static bool TryValidateDetectionValues(
            int darkThreshold,
            double minimumDarkCoverageRatio,
            int minimumPinWidth,
            int maximumPinBreakWidth,
            int minimumGapWidth,
            out string message)
        {
            if (darkThreshold < 0 || darkThreshold > 255)
            {
                message = "DarkThreshold must be between 0 and 255.";
                return false;
            }

            if (double.IsNaN(minimumDarkCoverageRatio)
                || double.IsInfinity(minimumDarkCoverageRatio)
                || minimumDarkCoverageRatio <= 0D
                || minimumDarkCoverageRatio > 1D)
            {
                message = "MinDarkCoverageRatio must be greater than 0 and at most 1.";
                return false;
            }

            if (minimumPinWidth <= 0)
            {
                message = "MinPinWidth must be a positive integer.";
                return false;
            }

            if (maximumPinBreakWidth < 0)
            {
                message = "MaxPinBreakWidth must be a non-negative integer.";
                return false;
            }

            if (minimumGapWidth <= 0)
            {
                message = "MinGapWidth must be a positive integer.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private static bool IsInsideSource(
            OpenVisionRecipePinGapIntentSkill.RoiSample roi,
            int sourceWidth,
            int sourceHeight)
        {
            return roi.X >= 0
                && roi.Y >= 0
                && roi.Width > 0
                && roi.Height > 0
                && roi.X < sourceWidth
                && roi.Y < sourceHeight
                && roi.Width <= sourceWidth - roi.X
                && roi.Height <= sourceHeight - roi.Y;
        }

        private static bool EqualsSupported(string value, string supportedValue)
        {
            return string.Equals(
                (value ?? string.Empty).Trim(),
                supportedValue,
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFinitePositive(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value) && value > 0D;
        }
    }
}
