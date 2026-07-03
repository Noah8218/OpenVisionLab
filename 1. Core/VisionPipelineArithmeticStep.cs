using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class VisionPipelineArithmeticStep
    {
        public const string ToolType = "Arithmetic";
        public const string ParameterMode = "ArithmeticMode";
        public const string ParameterOperation = "ArithmeticOperation";
        public const string ParameterInputLayerB = "InputLayerB";
        public const string ParameterUseConstantInput = "UseConstantInput";
        public const string ParameterUseColorConstant = "UseColorConstant";
        public const string ParameterGray = "Gray";
        public const string ParameterB = "B";
        public const string ParameterG = "G";
        public const string ParameterR = "R";
        public const string ParameterOffsetX = "OffsetX";
        public const string ParameterOffsetY = "OffsetY";

        public const string ModeOperation = "Operation";
        public const string ModeOffset = "Offset";

        public static bool IsArithmetic(VisionPipelineStep step)
        {
            return string.Equals(
                VisionPipelineNormalizer.NormalizeToolType(step?.ToolType),
                "arithmetic",
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool UsesConstantInput(VisionPipelineStep step)
        {
            return GetBool(step?.Parameters, ParameterUseConstantInput, false);
        }

        public static bool RequiresInputLayerB(VisionPipelineStep step)
        {
            if (!IsArithmetic(step) || IsOffsetMode(step) || UsesConstantInput(step))
            {
                return false;
            }

            string operation = GetString(step?.Parameters, ParameterOperation, "Bitwise_AND");
            return !string.Equals(operation, "Bitwise_NOT", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(operation, "ABS", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsOffsetMode(VisionPipelineStep step)
        {
            return string.Equals(
                GetString(step?.Parameters, ParameterMode, ModeOperation),
                ModeOffset,
                StringComparison.OrdinalIgnoreCase);
        }

        public static string GetInputLayerB(VisionPipelineStep step)
        {
            return GetString(step?.Parameters, ParameterInputLayerB, string.Empty);
        }

        public static VisionToolResult Execute(VisionPipelineStep step, Mat inputA, VisionPipelineContext context)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                if (inputA == null || inputA.Empty())
                {
                    return VisionToolResult.Failed(
                        VisionToolErrorCode.InputLayerMissing,
                        $"Input layer '{step?.InputLayer ?? "-"}' has no image.",
                        stopwatch.Elapsed);
                }

                if (IsOffsetMode(step))
                {
                    return ExecuteOffset(step, inputA, stopwatch);
                }

                Mat grayA = ToGray(inputA, out bool disposeGrayA);
                Mat inputB = null;
                Mat grayB = null;
                bool disposeGrayB = false;
                try
                {
                    inputB = CreateInputB(step, grayA, context, stopwatch, out VisionToolResult inputFailure);
                    if (inputFailure != null)
                    {
                        return inputFailure;
                    }

                    grayB = inputB == null ? null : ToGray(inputB, out disposeGrayB);
                    if (RequiresInputLayerB(step) && (grayB == null || grayB.Empty()))
                    {
                        return VisionToolResult.Failed(
                            VisionToolErrorCode.InputLayerMissing,
                            $"Input B layer '{GetInputLayerB(step)}' has no image.",
                            stopwatch.Elapsed);
                    }

                    if (grayB != null
                        && !grayB.Empty()
                        && (grayA.Width != grayB.Width || grayA.Height != grayB.Height))
                    {
                        return VisionToolResult.Failed(
                            VisionToolErrorCode.InvalidParameter,
                            $"Arithmetic input sizes must match. A={grayA.Width}x{grayA.Height}, B={grayB.Width}x{grayB.Height}.",
                            stopwatch.Elapsed);
                    }

                    Mat result = new Mat();
                    string operation = GetString(step?.Parameters, ParameterOperation, "Bitwise_AND");
                    switch (operation.ToUpperInvariant())
                    {
                        case "BITWISE_AND":
                            Cv2.BitwiseAnd(grayA, grayB, result);
                            break;
                        case "BITWISE_OR":
                            Cv2.BitwiseOr(grayA, grayB, result);
                            break;
                        case "BITWISE_XOR":
                            Cv2.BitwiseXor(grayA, grayB, result);
                            break;
                        case "BITWISE_NOT":
                            Cv2.BitwiseNot(grayA, result);
                            break;
                        case "ADD":
                            Cv2.Add(grayA, grayB, result);
                            break;
                        case "SUBTRACT":
                            Cv2.Subtract(grayA, grayB, result);
                            break;
                        case "MULTIPLY":
                            Cv2.Multiply(grayA, grayB, result);
                            break;
                        case "DIVIDE":
                            Cv2.Divide(grayA, grayB, result);
                            break;
                        case "MAX":
                            Cv2.Max(grayA, grayB, result);
                            break;
                        case "MIN":
                            Cv2.Min(grayA, grayB, result);
                            break;
                        case "ABS":
                            grayA.CopyTo(result);
                            break;
                        case "ABSDIFF":
                            Cv2.Absdiff(grayA, grayB, result);
                            break;
                        default:
                            result.Dispose();
                            return VisionToolResult.Failed(
                                VisionToolErrorCode.InvalidParameter,
                                $"Unsupported arithmetic operation '{operation}'.",
                                stopwatch.Elapsed);
                    }

                    return VisionToolResult.Passed(
                        result,
                        stopwatch.Elapsed,
                        new Dictionary<string, double>
                        {
                            ["OutputWidth"] = result.Width,
                            ["OutputHeight"] = result.Height
                        });
                }
                finally
                {
                    if (disposeGrayB)
                    {
                        grayB?.Dispose();
                    }

                    inputB?.Dispose();

                    if (disposeGrayA)
                    {
                        grayA?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                return VisionToolResult.Failed(
                    VisionToolErrorCode.OpenCvExecutionFailed,
                    ex.GetBaseException().Message,
                    stopwatch.Elapsed,
                    ex);
            }
        }

        private static VisionToolResult ExecuteOffset(VisionPipelineStep step, Mat inputA, Stopwatch stopwatch)
        {
            Mat grayA = ToGray(inputA, out bool disposeGrayA);
            try
            {
                int offsetX = GetInt(step?.Parameters, ParameterOffsetX, 1);
                int offsetY = GetInt(step?.Parameters, ParameterOffsetY, 1);
                if (Math.Abs(offsetX) >= grayA.Width || Math.Abs(offsetY) >= grayA.Height)
                {
                    return VisionToolResult.Failed(
                        VisionToolErrorCode.InvalidParameter,
                        $"Offset must be smaller than the input size. Offset=({offsetX},{offsetY}), Image={grayA.Width}x{grayA.Height}.",
                        stopwatch.Elapsed);
                }

                Mat result = Mat.Zeros(grayA.Size(), grayA.Type());
                int sourceX = offsetX <= 0 ? 0 : offsetX;
                int sourceY = offsetY <= 0 ? 0 : offsetY;
                int targetX = offsetX <= 0 ? -offsetX : 0;
                int targetY = offsetY <= 0 ? -offsetY : 0;
                int copyWidth = grayA.Width - Math.Abs(offsetX);
                int copyHeight = grayA.Height - Math.Abs(offsetY);

                using Mat source = grayA.SubMat(new Rect(sourceX, sourceY, copyWidth, copyHeight));
                using Mat target = result.SubMat(new Rect(targetX, targetY, copyWidth, copyHeight));
                source.CopyTo(target);
                return VisionToolResult.Passed(result, stopwatch.Elapsed);
            }
            finally
            {
                if (disposeGrayA)
                {
                    grayA?.Dispose();
                }
            }
        }

        private static Mat CreateInputB(
            VisionPipelineStep step,
            Mat inputA,
            VisionPipelineContext context,
            Stopwatch stopwatch,
            out VisionToolResult failure)
        {
            failure = null;
            if (!RequiresInputLayerB(step))
            {
                if (!UsesConstantInput(step))
                {
                    return null;
                }
            }

            if (UsesConstantInput(step))
            {
                int value = GetBool(step?.Parameters, ParameterUseColorConstant, false)
                    ? (GetInt(step?.Parameters, ParameterB, 1)
                        + GetInt(step?.Parameters, ParameterG, 1)
                        + GetInt(step?.Parameters, ParameterR, 1)) / 3
                    : GetInt(step?.Parameters, ParameterGray, 1);
                value = Math.Max(0, Math.Min(255, value));
                return new Mat(inputA.Size(), inputA.Type(), Scalar.All(value));
            }

            string inputLayerB = GetInputLayerB(step);
            if (string.IsNullOrWhiteSpace(inputLayerB))
            {
                failure = VisionToolResult.Failed(
                    VisionToolErrorCode.InputLayerMissing,
                    $"{step?.Name ?? ToolType} InputLayerB is required.",
                    stopwatch.Elapsed);
                return null;
            }

            Mat layer = context.GetLayer(inputLayerB);
            if (layer == null || layer.Empty())
            {
                layer?.Dispose();
                failure = VisionToolResult.Failed(
                    VisionToolErrorCode.InputLayerMissing,
                    $"Input B layer '{inputLayerB}' has no image.",
                    stopwatch.Elapsed);
                return null;
            }

            return layer;
        }

        private static Mat ToGray(Mat image, out bool ownsResult)
        {
            ownsResult = false;
            if (image == null)
            {
                return null;
            }

            if (image.Channels() == 1)
            {
                return image;
            }

            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            ownsResult = true;
            return gray;
        }

        private static string GetString(IDictionary<string, string> parameters, string key, string defaultValue)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(item.Value) ? defaultValue : item.Value;
                }
            }

            return defaultValue;
        }

        private static int GetInt(IDictionary<string, string> parameters, string key, int defaultValue)
        {
            string value = GetString(parameters, key, string.Empty);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : defaultValue;
        }

        private static bool GetBool(IDictionary<string, string> parameters, string key, bool defaultValue)
        {
            string value = GetString(parameters, key, string.Empty);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }
    }
}
