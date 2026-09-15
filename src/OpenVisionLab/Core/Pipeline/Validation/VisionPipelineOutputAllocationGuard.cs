using OpenCvSharp;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineOutputAllocationPreflightResult
    {
        public bool Success { get; set; }
        public VisionToolErrorCode ErrorCode { get; set; } = VisionToolErrorCode.None;
        public string Message { get; set; } = string.Empty;
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; }
        public int ElementBytes { get; set; }
        public long EstimatedBytes { get; set; }
    }

    internal static class VisionPipelineOutputAllocationGuard
    {
        internal const int MaximumAffineDimension = 32768;

        public static VisionPipelineOutputAllocationPreflightResult Validate(
            VisionPipelineStep step,
            Mat input)
        {
            if (step == null)
            {
                return Failure(VisionToolErrorCode.InvalidParameter, "Pipeline step is missing.");
            }

            if (input == null || input.Empty())
            {
                return Failure(
                    VisionToolErrorCode.InputLayerMissing,
                    $"Input layer '{step.InputLayer ?? "-"}' has no image.");
            }

            string toolType = VisionPipelineNormalizer.NormalizeToolType(step.ToolType);
            if (toolType != "rotatescale"
                && toolType != "rotateandscale"
                && toolType != "affine"
                && toolType != "affinematrix"
                && toolType != "affinetransform")
            {
                return Success(input.Width, input.Height, input.ElemSize());
            }

            if (!TryResolveOutputDimensions(step, input, toolType, out int outputWidth, out int outputHeight, out VisionToolErrorCode errorCode, out string errorMessage))
            {
                return Failure(errorCode, errorMessage);
            }

            int elementBytes = input.ElemSize();
            if (elementBytes <= 0)
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"{step.Name ?? "Step"} input element size is invalid; native allocation was skipped.");
            }

            if (!TryCalculateBytes(outputWidth, outputHeight, elementBytes, out long estimatedBytes))
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"{step.Name ?? "Step"} output allocation estimate {outputWidth}x{outputHeight}x{elementBytes} bytes cannot be represented safely; native allocation was skipped.");
            }

            return Success(outputWidth, outputHeight, elementBytes, estimatedBytes);
        }

        private static bool TryResolveOutputDimensions(
            VisionPipelineStep step,
            Mat input,
            string toolType,
            out int outputWidth,
            out int outputHeight,
            out VisionToolErrorCode errorCode,
            out string errorMessage)
        {
            outputWidth = 0;
            outputHeight = 0;
            errorCode = VisionToolErrorCode.InvalidParameter;
            errorMessage = string.Empty;

            if (toolType == "affine" || toolType == "affinematrix" || toolType == "affinetransform")
            {
                if (!TryReadInt(step.Parameters, "OutputWidth", 0, out int configuredWidth)
                    || !TryReadInt(step.Parameters, "OutputHeight", 0, out int configuredHeight))
                {
                    errorCode = VisionToolErrorCode.AffineInvalidOutputSize;
                    errorMessage = $"{step.Name ?? "Step"} Affine output width and height must be integers; native allocation was skipped.";
                    return false;
                }

                if (!TryResolveAffineDimension(configuredWidth, input.Width, out outputWidth)
                    || !TryResolveAffineDimension(configuredHeight, input.Height, out outputHeight))
                {
                    errorCode = VisionToolErrorCode.AffineInvalidOutputSize;
                    errorMessage = $"{step.Name ?? "Step"} Affine output dimensions must be 0 or 1..{MaximumAffineDimension}; native allocation was skipped.";
                    return false;
                }

                return true;
            }

            if (!TryReadDouble(step.Parameters, "ScaleXPercent", 100D, out double scaleXPercent)
                || !TryReadDouble(step.Parameters, "ScaleYPercent", 100D, out double scaleYPercent)
                || !IsFinitePositive(scaleXPercent)
                || !IsFinitePositive(scaleYPercent))
            {
                errorCode = VisionToolErrorCode.RotateScaleInvalidScale;
                errorMessage = $"{step.Name ?? "Step"} RotateScale scale must be finite and greater than 0; native allocation was skipped.";
                return false;
            }

            double scaleX = scaleXPercent / 100D;
            double scaleY = scaleYPercent / 100D;
            if (!IsFinitePositive(scaleX)
                || !IsFinitePositive(scaleY)
                || !TryScaleDimension(input.Width, scaleX, out outputWidth)
                || !TryScaleDimension(input.Height, scaleY, out outputHeight))
            {
                errorCode = VisionToolErrorCode.RotateScaleInvalidScale;
                errorMessage = $"{step.Name ?? "Step"} RotateScale scale produces an output dimension outside the native integer range; native allocation was skipped.";
                return false;
            }

            return true;
        }

        private static bool TryResolveAffineDimension(int configured, int inputDimension, out int resolved)
        {
            resolved = configured == 0 ? inputDimension : configured;
            return resolved > 0 && resolved <= MaximumAffineDimension;
        }

        private static bool TryScaleDimension(int inputDimension, double scale, out int resolved)
        {
            resolved = 0;
            double scaled = inputDimension * scale;
            if (!IsFinitePositive(scaled) || scaled > int.MaxValue)
            {
                return false;
            }

            double rounded = Math.Round(scaled, MidpointRounding.ToEven);
            if (rounded > int.MaxValue)
            {
                return false;
            }

            resolved = Math.Max(1, (int)rounded);
            return true;
        }

        private static bool TryCalculateBytes(
            int width,
            int height,
            int elementBytes,
            out long bytes)
        {
            bytes = 0L;
            try
            {
                bytes = checked(checked((long)width * height) * elementBytes);
                return bytes > 0L;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        private static bool TryReadInt(
            IDictionary<string, string> parameters,
            string key,
            int defaultValue,
            out int value)
        {
            value = defaultValue;
            if (!TryGetParameter(parameters, key, out string text))
            {
                return true;
            }

            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryReadDouble(
            IDictionary<string, string> parameters,
            string key,
            double defaultValue,
            out double value)
        {
            value = defaultValue;
            if (!TryGetParameter(parameters, key, out string text))
            {
                return true;
            }

            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryGetParameter(
            IDictionary<string, string> parameters,
            string key,
            out string value)
        {
            value = null;
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = item.Value;
                    return true;
                }
            }

            return false;
        }

        private static bool IsFinitePositive(double value)
        {
            return value > 0D && !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static VisionPipelineOutputAllocationPreflightResult Success(
            int outputWidth,
            int outputHeight,
            int elementBytes,
            long estimatedBytes = 0L)
        {
            return new VisionPipelineOutputAllocationPreflightResult
            {
                Success = true,
                OutputWidth = outputWidth,
                OutputHeight = outputHeight,
                ElementBytes = elementBytes,
                EstimatedBytes = estimatedBytes
            };
        }

        private static VisionPipelineOutputAllocationPreflightResult Failure(
            VisionToolErrorCode errorCode,
            string message)
        {
            return new VisionPipelineOutputAllocationPreflightResult
            {
                Success = false,
                ErrorCode = errorCode,
                Message = message ?? string.Empty
            };
        }
    }
}
