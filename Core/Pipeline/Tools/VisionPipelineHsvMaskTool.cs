using Lib.OpenCV;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineHsvMaskTool : IVisionTool
    {
        private readonly IDictionary<string, string> parameters;

        public VisionPipelineHsvMaskTool(string name, IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "HsvMask" : name;
            this.parameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (OpenCvHelper.IsImageEmpty(source))
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InputImageInvalid,
                    "HSV input image is empty.",
                    stopwatch.Elapsed);
            }

            try
            {
                using Mat bgr = ToBgr(source);
                using Mat hsv = new Mat();
                Cv2.CvtColor(bgr, hsv, ColorConversionCodes.BGR2HSV);

                Rect roi = ResolveRoi(hsv);
                using Mat mask = new Mat(hsv.Size(), MatType.CV_8UC1, Scalar.Black);

                int denominator;
                if (roi.Width > 0 && roi.Height > 0)
                {
                    using Mat hsvRoi = new Mat(hsv, roi);
                    using Mat maskRoi = CreateMask(hsvRoi);
                    using Mat destination = new Mat(mask, roi);
                    maskRoi.CopyTo(destination);
                    denominator = roi.Width * roi.Height;
                }
                else
                {
                    using Mat fullMask = CreateMask(hsv);
                    fullMask.CopyTo(mask);
                    denominator = hsv.Width * hsv.Height;
                }

                double maskPixelCount = Cv2.CountNonZero(mask);
                double maskPixelRatio = denominator > 0 ? maskPixelCount / denominator : 0d;

                stopwatch.Stop();
                return VisionToolResult.Passed(mask.Clone(), stopwatch.Elapsed, CreateMetrics(source, mask, maskPixelCount, maskPixelRatio));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.OpenCvExecutionFailed,
                    "HSV mask failed. " + ex.Message,
                    stopwatch.Elapsed,
                    ex);
            }
        }

        private Mat CreateMask(Mat hsv)
        {
            int hueMin = Clamp(GetInt("HueMin", GetInt("HUE_MIN", 0)), 0, 179);
            int hueMax = Clamp(GetInt("HueMax", GetInt("HUE_MAX", 179)), 0, 179);
            int saturationMin = Clamp(GetInt("SaturationMin", GetInt("SATURATION_MIN", 0)), 0, 255);
            int saturationMax = Clamp(GetInt("SaturationMax", GetInt("SATURATION_MAX", 255)), 0, 255);
            int valueMin = Clamp(GetInt("ValueMin", GetInt("VALUE_MIN", 0)), 0, 255);
            int valueMax = Clamp(GetInt("ValueMax", GetInt("VALUE_MAX", 255)), 0, 255);

            Mat mask = new Mat();
            if (hueMin <= hueMax)
            {
                Cv2.InRange(
                    hsv,
                    new Scalar(hueMin, saturationMin, valueMin),
                    new Scalar(hueMax, saturationMax, valueMax),
                    mask);
                return mask;
            }

            using Mat highHue = new Mat();
            using Mat lowHue = new Mat();
            Cv2.InRange(
                hsv,
                new Scalar(hueMin, saturationMin, valueMin),
                new Scalar(179, saturationMax, valueMax),
                highHue);
            Cv2.InRange(
                hsv,
                new Scalar(0, saturationMin, valueMin),
                new Scalar(hueMax, saturationMax, valueMax),
                lowHue);
            Cv2.BitwiseOr(highHue, lowHue, mask);
            return mask;
        }

        private static Dictionary<string, double> CreateMetrics(Mat source, Mat result, double maskPixelCount, double maskPixelRatio)
        {
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.MaskPixelCount] = maskPixelCount,
                [VisionPipelineKnownMetrics.MaskPixelRatio] = maskPixelRatio,
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = result.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = result.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = result.Channels()
            };
        }

        private static Mat ToBgr(Mat source)
        {
            Mat bgr = new Mat();
            if (source.Channels() == 1)
            {
                Cv2.CvtColor(source, bgr, ColorConversionCodes.GRAY2BGR);
            }
            else if (source.Channels() == 4)
            {
                Cv2.CvtColor(source, bgr, ColorConversionCodes.BGRA2BGR);
            }
            else
            {
                source.CopyTo(bgr);
            }

            return bgr;
        }

        private Rect ResolveRoi(Mat image)
        {
            if (!GetBool("USE_ROI", false))
            {
                return default;
            }

            string value = GetString("CvROI", string.Empty);
            string[] parts = value.Split(',');
            if (parts.Length != 4)
            {
                return default;
            }

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
            {
                return default;
            }

            int left = Clamp(x, 0, image.Width);
            int top = Clamp(y, 0, image.Height);
            int right = Clamp(x + width, left, image.Width);
            int bottom = Clamp(y + height, top, image.Height);
            return new Rect(left, top, right - left, bottom - top);
        }

        private string GetString(string key, string defaultValue)
        {
            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(item.Value) ? defaultValue : item.Value;
                }
            }

            return defaultValue;
        }

        private int GetInt(string key, int defaultValue)
        {
            return int.TryParse(GetString(key, string.Empty), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : defaultValue;
        }

        private bool GetBool(string key, bool defaultValue)
        {
            return bool.TryParse(GetString(key, string.Empty), out bool value) ? value : defaultValue;
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
