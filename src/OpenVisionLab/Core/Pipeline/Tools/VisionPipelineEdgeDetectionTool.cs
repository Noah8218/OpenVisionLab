using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineEdgeDetectionTool : IVisionTool
    {
        private readonly IDictionary<string, string> parameters;

        public VisionPipelineEdgeDetectionTool(string name, IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "EdgeDetection" : name;
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
                    "EdgeDetection input image is empty.",
                    stopwatch.Elapsed);
            }

            try
            {
                using Mat gray = ToGray(source);
                Rect roi = ResolveRoi(gray);
                using Mat work = roi.Width > 0 && roi.Height > 0
                    ? new Mat(gray, roi).Clone()
                    : gray.Clone();
                using Mat edge = ExecuteEdge(work);

                Mat result = new Mat(gray.Size(), MatType.CV_8UC1, Scalar.Black);
                if (roi.Width > 0 && roi.Height > 0)
                {
                    edge.CopyTo(new Mat(result, roi));
                }
                else
                {
                    edge.CopyTo(result);
                }

                stopwatch.Stop();
                return VisionToolResult.Passed(result, stopwatch.Elapsed, CreateMetrics(source, result));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.OpenCvExecutionFailed,
                    "EdgeDetection failed. " + ex.Message,
                    stopwatch.Elapsed,
                    ex);
            }
        }

        private Mat ExecuteEdge(Mat gray)
        {
            string edgeType = GetString("EdgeType", "Canny").Replace(" ", string.Empty);
            if (string.Equals(edgeType, "Sobel", StringComparison.OrdinalIgnoreCase))
            {
                return ExecuteSobel(gray);
            }

            if (string.Equals(edgeType, "Scharr", StringComparison.OrdinalIgnoreCase))
            {
                return ExecuteScharr(gray);
            }

            if (string.Equals(edgeType, "Laplacian", StringComparison.OrdinalIgnoreCase))
            {
                return ExecuteLaplacian(gray);
            }

            Mat edge = new Mat();
            Cv2.Canny(
                gray,
                edge,
                Clamp(GetInt("CannyThresholdLow", GetInt("CANNY_LOW", 100)), 0, 255),
                Clamp(GetInt("CannyThresholdHigh", GetInt("CANNY_HIGH", 200)), 0, 255),
                NormalizeOddKernel(GetInt("CannyApertureSize", GetInt("CANNY_APERTURE_SIZE", 3)), 3, 7),
                GetBool("UseL2Gradient", GetBool("USE_L2_GRADIENT", true)));
            return edge;
        }

        private Mat ExecuteSobel(Mat gray)
        {
            Mat gradient = new Mat();
            Cv2.Sobel(
                gray,
                gradient,
                MatType.CV_16S,
                Clamp(GetInt("SobelDegreeX", 1), 0, 9),
                Clamp(GetInt("SobelDegreeY", 0), 0, 9),
                NormalizeOddKernel(GetInt("SobelKernelSize", 3), 1, 31));

            Mat edge = new Mat();
            Cv2.ConvertScaleAbs(gradient, edge);
            gradient.Dispose();
            return edge;
        }

        private Mat ExecuteScharr(Mat gray)
        {
            Mat gradient = new Mat();
            Cv2.Scharr(
                gray,
                gradient,
                MatType.CV_16S,
                Clamp(GetInt("ScharrDegreeX", 1), 0, 1),
                Clamp(GetInt("ScharrDegreeY", 0), 0, 1));

            Mat edge = new Mat();
            Cv2.ConvertScaleAbs(gradient, edge);
            gradient.Dispose();
            return edge;
        }

        private Mat ExecuteLaplacian(Mat gray)
        {
            Mat gradient = new Mat();
            Cv2.Laplacian(
                gray,
                gradient,
                MatType.CV_16S,
                NormalizeOddKernel(GetInt("LaplacianKernelSize", 3), 1, 31));

            Mat edge = new Mat();
            Cv2.ConvertScaleAbs(gradient, edge);
            gradient.Dispose();
            return edge;
        }

        private static Dictionary<string, double> CreateMetrics(Mat source, Mat result)
        {
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.EdgePointCount] = Cv2.CountNonZero(result),
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = result.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = result.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = result.Channels()
            };
        }

        private static Mat ToGray(Mat source)
        {
            Mat gray = new Mat();
            if (source.Channels() == 1)
            {
                source.CopyTo(gray);
            }
            else if (source.Channels() == 4)
            {
                Cv2.CvtColor(source, gray, ColorConversionCodes.BGRA2GRAY);
            }
            else
            {
                Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            }

            return gray;
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

        private static int NormalizeOddKernel(int value, int min, int max)
        {
            int result = Clamp(value, min, max);
            return result % 2 == 0 ? Math.Max(min, result - 1) : result;
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
