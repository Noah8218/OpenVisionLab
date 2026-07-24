using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineNormalizeImageTool : IVisionTool
    {
        private readonly IDictionary<string, string> parameters;

        public VisionPipelineNormalizeImageTool(string name, IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "NormalizeImage" : name;
            this.parameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (source == null || source.Empty())
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InputImageInvalid,
                    "NormalizeImage input image is empty.",
                    stopwatch.Elapsed);
            }

            if (!TryGetRequiredDouble(VisionPipelineFixtureFrameService.RuntimeReferenceXParameter, out double referenceX)
                || !TryGetRequiredDouble(VisionPipelineFixtureFrameService.RuntimeReferenceYParameter, out double referenceY)
                || !TryGetRequiredDouble(VisionPipelineFixtureFrameService.RuntimeCurrentXParameter, out double currentX)
                || !TryGetRequiredDouble(VisionPipelineFixtureFrameService.RuntimeCurrentYParameter, out double currentY)
                || !TryGetRequiredDouble(VisionPipelineFixtureFrameService.RuntimeAngleDeltaParameter, out double angleDelta)
                || !TryGetRequiredDouble(VisionPipelineFixtureFrameService.RuntimeScaleRatioParameter, out double scaleRatio)
                || scaleRatio <= 0d)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidParameter,
                    "NormalizeImage requires a valid runtime fixture center, angle delta, and positive scale ratio.",
                    stopwatch.Elapsed);
            }

            int referenceWidth = GetInt(VisionPipelineFixtureFrameService.ReferenceImageWidthParameter, 0);
            int referenceHeight = GetInt(VisionPipelineFixtureFrameService.ReferenceImageHeightParameter, 0);
            if (referenceWidth <= 0 || referenceHeight <= 0)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidParameter,
                    "NormalizeImage requires positive taught reference image dimensions.",
                    stopwatch.Elapsed);
            }

            if (source.Width != referenceWidth || source.Height != referenceHeight)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidParameter,
                    $"NormalizeImage source size {source.Width}x{source.Height} does not match the taught reference size {referenceWidth}x{referenceHeight}.",
                    stopwatch.Elapsed);
            }

            double minimumValidPixelRatio = GetDouble(
                VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter,
                VisionPipelineFixtureFrameService.DefaultMinimumValidPixelRatio);
            if (!IsFinite(minimumValidPixelRatio)
                || minimumValidPixelRatio <= 0d
                || minimumValidPixelRatio > 1d)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidParameter,
                    $"NormalizeImage requires 0 < {VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter} <= 1.",
                    stopwatch.Elapsed);
            }

            try
            {
                double correctionAngle = -angleDelta;
                double correctionScale = 1d / scaleRatio;
                using Mat matrix = Cv2.GetRotationMatrix2D(
                    new Point2f((float)currentX, (float)currentY),
                    correctionAngle,
                    correctionScale);
                matrix.Set(0, 2, matrix.At<double>(0, 2) + referenceX - currentX);
                matrix.Set(1, 2, matrix.At<double>(1, 2) + referenceY - currentY);

                InterpolationFlags interpolation = GetEnum("Interpolation", InterpolationFlags.Linear);
                BorderTypes borderType = GetEnum("BorderType", BorderTypes.Constant);
                Mat resultImage = new Mat();
                Cv2.WarpAffine(
                    source,
                    resultImage,
                    matrix,
                    new OpenCvSharp.Size(referenceWidth, referenceHeight),
                    interpolation,
                    borderType,
                    Scalar.All(0));

                using Mat sourceMask = new Mat(source.Size(), MatType.CV_8UC1, Scalar.White);
                using Mat validMask = new Mat();
                Cv2.WarpAffine(
                    sourceMask,
                    validMask,
                    matrix,
                    new OpenCvSharp.Size(referenceWidth, referenceHeight),
                    InterpolationFlags.Nearest,
                    BorderTypes.Constant,
                    Scalar.Black);

                double validPixelRatio = Cv2.CountNonZero(validMask) / (double)(referenceWidth * referenceHeight);
                if (!IsFinite(validPixelRatio) || validPixelRatio < minimumValidPixelRatio)
                {
                    resultImage.Dispose();
                    stopwatch.Stop();
                    return VisionToolResult.Failed(
                        VisionToolErrorCode.InvalidParameter,
                        $"NormalizeImage valid-pixel ratio {validPixelRatio:0.###} is below the required {minimumValidPixelRatio:0.###}.",
                        stopwatch.Elapsed);
                }

                Rect validBounds = ResolveValidBounds(validMask, referenceWidth, referenceHeight);
                Dictionary<string, double> metrics = CreateMetrics(
                    source,
                    resultImage,
                    referenceWidth,
                    referenceHeight,
                    validPixelRatio,
                    currentX,
                    currentY,
                    correctionAngle,
                    correctionScale);
                List<VisionToolOverlay> overlays = CreateOverlays(
                    validBounds,
                    referenceX,
                    referenceY,
                    referenceWidth,
                    referenceHeight,
                    validPixelRatio);

                stopwatch.Stop();
                return VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics, overlays);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.OpenCvExecutionFailed,
                    "NormalizeImage inverse similarity transform failed. " + ex.Message,
                    stopwatch.Elapsed,
                    ex);
            }
        }

        private static Dictionary<string, double> CreateMetrics(
            Mat source,
            Mat result,
            int referenceWidth,
            int referenceHeight,
            double validPixelRatio,
            double currentX,
            double currentY,
            double correctionAngle,
            double correctionScale)
        {
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = result.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = result.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = result.Channels(),
                [VisionPipelineKnownMetrics.FixtureReferenceImageWidth] = referenceWidth,
                [VisionPipelineKnownMetrics.FixtureReferenceImageHeight] = referenceHeight,
                [VisionPipelineKnownMetrics.FixtureNormalizedImageWidth] = result.Width,
                [VisionPipelineKnownMetrics.FixtureNormalizedImageHeight] = result.Height,
                [VisionPipelineKnownMetrics.FixtureValidPixelRatio] = validPixelRatio,
                [VisionPipelineKnownMetrics.FixtureAppliedCenterX] = currentX,
                [VisionPipelineKnownMetrics.FixtureAppliedCenterY] = currentY,
                [VisionPipelineKnownMetrics.FixtureAppliedAngle] = correctionAngle,
                [VisionPipelineKnownMetrics.FixtureAppliedScaleRatio] = correctionScale
            };
        }

        private static List<VisionToolOverlay> CreateOverlays(
            Rect validBounds,
            double referenceX,
            double referenceY,
            int width,
            int height,
            double validPixelRatio)
        {
            float centerX = (float)Math.Clamp(referenceX, 0d, Math.Max(0d, width - 1d));
            float centerY = (float)Math.Clamp(referenceY, 0d, Math.Max(0d, height - 1d));
            float axisHalfLength = Math.Max(12f, Math.Min(width, height) * 0.06f);
            return new List<VisionToolOverlay>
            {
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Rectangle,
                    Label = $"Valid normalized pixels {validPixelRatio:P1}",
                    Bounds = new RectangleF(validBounds.X, validBounds.Y, validBounds.Width, validBounds.Height),
                    Center = new PointF(
                        validBounds.X + validBounds.Width / 2f,
                        validBounds.Y + validBounds.Height / 2f)
                },
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = "Reference X axis",
                    Start = new PointF(Math.Max(0f, centerX - axisHalfLength), centerY),
                    End = new PointF(Math.Min(width - 1f, centerX + axisHalfLength), centerY)
                },
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = "Reference Y axis",
                    Start = new PointF(centerX, Math.Max(0f, centerY - axisHalfLength)),
                    End = new PointF(centerX, Math.Min(height - 1f, centerY + axisHalfLength))
                },
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Point,
                    Label = "Reference center",
                    Center = new PointF(centerX, centerY)
                }
            };
        }

        private static Rect ResolveValidBounds(Mat validMask, int width, int height)
        {
            Cv2.FindContours(
                validMask,
                out OpenCvSharp.Point[][] contours,
                out _,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                return new Rect(0, 0, width, height);
            }

            Rect first = Cv2.BoundingRect(contours[0]);
            int left = first.X;
            int top = first.Y;
            int right = first.Right;
            int bottom = first.Bottom;
            foreach (Rect bounds in contours.Skip(1).Select(Cv2.BoundingRect))
            {
                left = Math.Min(left, bounds.X);
                top = Math.Min(top, bounds.Y);
                right = Math.Max(right, bounds.Right);
                bottom = Math.Max(bottom, bounds.Bottom);
            }

            return new Rect(left, top, right - left, bottom - top);
        }

        private bool TryGetRequiredDouble(string key, out double value)
        {
            value = GetDouble(key, double.NaN);
            return IsFinite(value);
        }

        private string GetString(string key, string defaultValue = "")
        {
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (string.Equals(parameter.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(parameter.Value) ? defaultValue : parameter.Value.Trim();
                }
            }

            return defaultValue;
        }

        private int GetInt(string key, int defaultValue)
        {
            return int.TryParse(GetString(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : defaultValue;
        }

        private double GetDouble(string key, double defaultValue)
        {
            return double.TryParse(GetString(key), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                ? value
                : defaultValue;
        }

        private TEnum GetEnum<TEnum>(string key, TEnum defaultValue) where TEnum : struct
        {
            return Enum.TryParse(GetString(key), true, out TEnum value) ? value : defaultValue;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
