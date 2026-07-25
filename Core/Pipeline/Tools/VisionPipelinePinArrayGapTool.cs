using Lib.OpenCV;
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
    internal enum PinArrayGapMeasurementMode
    {
        EdgeGap,
        CenterPitch
    }

    /// <summary>
    /// Measures adjacent edge gaps or center-to-center pitch between vertically continuous dark pins in one taught row ROI.
    /// </summary>
    internal sealed class VisionPipelinePinArrayGapTool : IVisionTool
    {
        private readonly IDictionary<string, string> parameters;

        public VisionPipelinePinArrayGapTool(string name, IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "PinArrayGap" : name;
            this.parameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (source == null || source.Empty())
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(VisionToolErrorCode.InputImageInvalid, "PinArrayGap input image is empty.", stopwatch.Elapsed);
            }

            if (!GetBool("USE_ROI", false) || !TryGetRoi(source, out Rect roi))
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidRoi,
                    "PinArrayGap requires USE_ROI=true and one in-image CvROI covering a single pin row.",
                    stopwatch.Elapsed);
            }

            int darkThreshold = GetInt("DarkThreshold", 128);
            double minimumDarkCoverage = GetDouble("MinDarkCoverageRatio", 0.55);
            int minimumPinWidth = GetInt("MinPinWidth", 5);
            int maximumPinBreakWidth = GetInt("MaxPinBreakWidth", 2);
            int minimumGapWidth = GetInt("MinGapWidth", 3);
            if (!Enum.TryParse(GetString("MeasurementMode", nameof(PinArrayGapMeasurementMode.EdgeGap)), true, out PinArrayGapMeasurementMode measurementMode)
                || !Enum.IsDefined(typeof(PinArrayGapMeasurementMode), measurementMode))
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidParameter,
                    "PinArrayGap MeasurementMode must be EdgeGap or CenterPitch.",
                    stopwatch.Elapsed);
            }

            using Mat gray = CreateGray(source);
            List<PinRun> pins = FindPinRuns(
                gray,
                roi,
                Math.Clamp(darkThreshold, 0, 255),
                Math.Clamp(minimumDarkCoverage, 0.01, 1.0),
                Math.Max(1, minimumPinWidth),
                Math.Max(0, maximumPinBreakWidth));
            List<GapRun> gaps = FindGapRuns(pins, Math.Max(1, minimumGapWidth));
            List<PitchRun> pitches = FindPitchRuns(pins);

            int measurementCount = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? pitches.Count : gaps.Count;
            if (pins.Count < 2 || measurementCount == 0)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.LineGaugeEdgeNotFound,
                    $"PinArrayGap {measurementMode} found {pins.Count} pin run(s) and {measurementCount} valid measurement(s) in CvROI {roi.X},{roi.Y},{roi.Width},{roi.Height}. Tune DarkThreshold, MinDarkCoverageRatio, or the row ROI.",
                    stopwatch.Elapsed);
            }

            Mat resultImage = CreateColorImage(source);
            Dictionary<string, double> metrics = CreateMetrics(source, resultImage, pins, gaps, pitches, measurementMode);
            List<VisionToolOverlay> overlays = CreateOverlays(roi, pins, gaps, pitches, measurementMode);
            DrawResult(resultImage, roi, pins, gaps, pitches, measurementMode);

            stopwatch.Stop();
            return VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics, overlays);
        }

        private static Mat CreateGray(Mat source)
        {
            if (source.Channels() == 1)
            {
                return source.Clone();
            }

            Mat gray = new Mat();
            Cv2.CvtColor(
                source,
                gray,
                source.Channels() == 4 ? ColorConversionCodes.BGRA2GRAY : ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        private static Mat CreateColorImage(Mat source)
        {
            if (source.Channels() == 1)
            {
                Mat color = new Mat();
                Cv2.CvtColor(source, color, ColorConversionCodes.GRAY2BGR);
                return color;
            }

            if (source.Channels() == 4)
            {
                Mat color = new Mat();
                Cv2.CvtColor(source, color, ColorConversionCodes.BGRA2BGR);
                return color;
            }

            return source.Clone();
        }

        private static List<PinRun> FindPinRuns(
            Mat gray,
            Rect roi,
            int darkThreshold,
            double minimumDarkCoverage,
            int minimumPinWidth,
            int maximumPinBreakWidth)
        {
            List<PinRun> rawRuns = new List<PinRun>();
            int start = -1;
            for (int x = roi.X; x < roi.Right; x++)
            {
                int darkPixels = 0;
                for (int y = roi.Y; y < roi.Bottom; y++)
                {
                    if (gray.At<byte>(y, x) <= darkThreshold)
                    {
                        darkPixels++;
                    }
                }

                bool isPinColumn = darkPixels / (double)roi.Height >= minimumDarkCoverage;
                if (isPinColumn && start < 0)
                {
                    start = x;
                }
                else if (!isPinColumn && start >= 0)
                {
                    rawRuns.Add(new PinRun(start, x - 1));
                    start = -1;
                }
            }

            if (start >= 0)
            {
                rawRuns.Add(new PinRun(start, roi.Right - 1));
            }

            List<PinRun> mergedRuns = new List<PinRun>();
            foreach (PinRun run in rawRuns)
            {
                if (mergedRuns.Count > 0 && run.Start - mergedRuns[mergedRuns.Count - 1].End - 1 <= maximumPinBreakWidth)
                {
                    mergedRuns[mergedRuns.Count - 1] = new PinRun(mergedRuns[mergedRuns.Count - 1].Start, run.End);
                }
                else
                {
                    mergedRuns.Add(run);
                }
            }

            return mergedRuns.Where(run => run.Width >= minimumPinWidth).ToList();
        }

        private static List<GapRun> FindGapRuns(IReadOnlyList<PinRun> pins, int minimumGapWidth)
        {
            List<GapRun> gaps = new List<GapRun>();
            for (int index = 1; index < pins.Count; index++)
            {
                int start = pins[index - 1].End + 1;
                int end = pins[index].Start - 1;
                if (end - start + 1 >= minimumGapWidth)
                {
                    gaps.Add(new GapRun(start, end));
                }
            }

            return gaps;
        }

        private static List<PitchRun> FindPitchRuns(IReadOnlyList<PinRun> pins)
        {
            List<PitchRun> pitches = new List<PitchRun>();
            for (int index = 1; index < pins.Count; index++)
            {
                pitches.Add(new PitchRun(pins[index - 1].Center, pins[index].Center));
            }

            return pitches;
        }

        private Dictionary<string, double> CreateMetrics(
            Mat source,
            Mat resultImage,
            IReadOnlyList<PinRun> pins,
            IReadOnlyList<GapRun> gaps,
            IReadOnlyList<PitchRun> pitches,
            PinArrayGapMeasurementMode measurementMode)
        {
            List<double> distances = measurementMode == PinArrayGapMeasurementMode.CenterPitch
                ? pitches.Select(pitch => pitch.Distance).ToList()
                : gaps.Select(gap => (double)gap.Width).ToList();
            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = distances.Count,
                [VisionPipelineKnownMetrics.EdgeCount] = pins.Count,
                [VisionPipelineKnownMetrics.EdgePointCount] = pins.Count * 2,
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = resultImage.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = resultImage.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = resultImage.Channels()
            };

            if (measurementMode == PinArrayGapMeasurementMode.CenterPitch)
            {
                metrics[VisionPipelineKnownMetrics.PitchCount] = distances.Count;
                metrics[VisionPipelineKnownMetrics.PitchPxMin] = distances.Min();
                metrics[VisionPipelineKnownMetrics.PitchPxMax] = distances.Max();
                metrics[VisionPipelineKnownMetrics.PitchPxAvg] = distances.Average();
                metrics[VisionPipelineKnownMetrics.PitchPxRange] = distances.Max() - distances.Min();
                return metrics;
            }

            metrics[VisionPipelineKnownMetrics.DistanceCount] = distances.Count;
            metrics[VisionPipelineKnownMetrics.DistancePxMin] = distances.Min();
            metrics[VisionPipelineKnownMetrics.DistancePxMax] = distances.Max();
            metrics[VisionPipelineKnownMetrics.DistancePxAvg] = distances.Average();
            metrics[VisionPipelineKnownMetrics.DistancePxRange] = distances.Max() - distances.Min();

            double pixelPerMm = GetDouble("PIXELPERMM", 0);
            if (pixelPerMm > 0)
            {
                metrics[VisionPipelineKnownMetrics.DistanceMmMin] = metrics[VisionPipelineKnownMetrics.DistancePxMin] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmMax] = metrics[VisionPipelineKnownMetrics.DistancePxMax] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmAvg] = metrics[VisionPipelineKnownMetrics.DistancePxAvg] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmRange] = metrics[VisionPipelineKnownMetrics.DistancePxRange] * pixelPerMm;
            }

            return metrics;
        }

        private static List<VisionToolOverlay> CreateOverlays(
            Rect roi,
            IReadOnlyList<PinRun> pins,
            IReadOnlyList<GapRun> gaps,
            IReadOnlyList<PitchRun> pitches,
            PinArrayGapMeasurementMode measurementMode)
        {
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>
            {
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Rectangle,
                    Label = "Row ROI | " + BuildRowSummary(pins, gaps, pitches, measurementMode),
                    Bounds = new RectangleF(roi.X, roi.Y, roi.Width, roi.Height),
                    Center = new PointF(roi.X + roi.Width / 2F, roi.Y + roi.Height / 2F)
                }
            };
            foreach (PinRun pin in pins)
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Rectangle,
                    Label = "Pin",
                    Bounds = new RectangleF(pin.Start, roi.Y, pin.Width, roi.Height),
                    Center = new PointF(pin.Start + pin.Width / 2F, roi.Y + roi.Height / 2F)
                });
            }

            float lineY = roi.Y + roi.Height / 2F;
            int measurementCount = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? pitches.Count : gaps.Count;
            for (int index = 0; index < measurementCount; index++)
            {
                float start = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? (float)pitches[index].StartCenter : gaps[index].Start;
                float end = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? (float)pitches[index].EndCenter : gaps[index].End;
                double distance = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? pitches[index].Distance : gaps[index].Width;
                string prefix = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? "P" : "G";
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = string.Format(CultureInfo.InvariantCulture, "{0}{1}: {2:0.###}px", prefix, index + 1, distance),
                    Start = new PointF(start, lineY),
                    End = new PointF(end, lineY),
                    Center = new PointF((start + end) / 2F, lineY)
                });
            }

            return overlays;
        }

        private static void DrawResult(
            Mat resultImage,
            Rect roi,
            IReadOnlyList<PinRun> pins,
            IReadOnlyList<GapRun> gaps,
            IReadOnlyList<PitchRun> pitches,
            PinArrayGapMeasurementMode measurementMode)
        {
            Scalar roiColor = new Scalar(255, 220, 0);
            Scalar pinColor = new Scalar(0, 220, 90);
            Scalar gapColor = new Scalar(0, 0, 255);
            float lineY = roi.Y + roi.Height / 2F;
            Cv2.Rectangle(resultImage, roi, roiColor, 1);
            Cv2.PutText(
                resultImage,
                BuildRowSummary(pins, gaps, pitches, measurementMode),
                new OpenCvSharp.Point(roi.X + 4, Math.Max(12, roi.Y + 14)),
                HersheyFonts.HersheySimplex,
                0.36,
                roiColor,
                1,
                LineTypes.AntiAlias);
            foreach (PinRun pin in pins)
            {
                Cv2.Rectangle(resultImage, new Rect(pin.Start, roi.Y, pin.Width, roi.Height), pinColor, 1);
                if (measurementMode == PinArrayGapMeasurementMode.CenterPitch)
                {
                    OpenCvSharp.Point center = new OpenCvSharp.Point((int)Math.Round(pin.Center), (int)lineY);
                    Cv2.Line(resultImage, new OpenCvSharp.Point(center.X, center.Y - 7), new OpenCvSharp.Point(center.X, center.Y + 7), new Scalar(255, 255, 0), 1, LineTypes.AntiAlias);
                    Cv2.Circle(resultImage, center, 3, new Scalar(255, 255, 0), 1, LineTypes.AntiAlias);
                }
            }

            int measurementCount = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? pitches.Count : gaps.Count;
            for (int index = 0; index < measurementCount; index++)
            {
                double start = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? pitches[index].StartCenter : gaps[index].Start;
                double end = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? pitches[index].EndCenter : gaps[index].End;
                double distance = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? pitches[index].Distance : gaps[index].Width;
                string prefix = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? "P" : "G";
                Cv2.Line(resultImage, new OpenCvSharp.Point((int)Math.Round(start), (int)lineY), new OpenCvSharp.Point((int)Math.Round(end), (int)lineY), gapColor, 2, LineTypes.AntiAlias);
                if (index % 2 == 0 || measurementCount <= 8)
                {
                    Cv2.PutText(
                        resultImage,
                        string.Format(CultureInfo.InvariantCulture, "{0}{1}:{2:0.###}px", prefix, index + 1, distance),
                        new OpenCvSharp.Point((int)Math.Round(start), Math.Max(10, (int)lineY - 4)),
                        HersheyFonts.HersheySimplex,
                        0.32,
                        gapColor,
                        1,
                    LineTypes.AntiAlias);
                }
            }

            if (measurementMode == PinArrayGapMeasurementMode.CenterPitch)
            {
                foreach (PinRun pin in pins)
                {
                    OpenCvSharp.Point center = new OpenCvSharp.Point((int)Math.Round(pin.Center), (int)lineY);
                    Cv2.Circle(resultImage, center, 4, new Scalar(255, 255, 0), -1, LineTypes.AntiAlias);
                    Cv2.Circle(resultImage, center, 4, new Scalar(0, 90, 90), 1, LineTypes.AntiAlias);
                }
            }
        }

        private static string BuildRowSummary(
            IReadOnlyList<PinRun> pins,
            IReadOnlyList<GapRun> gaps,
            IReadOnlyList<PitchRun> pitches,
            PinArrayGapMeasurementMode measurementMode)
        {
            List<double> distances = measurementMode == PinArrayGapMeasurementMode.CenterPitch
                ? pitches.Select(pitch => pitch.Distance).ToList()
                : gaps.Select(gap => (double)gap.Width).ToList();
            string measurementName = measurementMode == PinArrayGapMeasurementMode.CenterPitch ? "pitches" : "gaps";
            return string.Format(
                CultureInfo.InvariantCulture,
                "mode={0} pins={1} {2}={3} min={4:0.###} max={5:0.###} avg={6:0.###} range={7:0.###}px",
                measurementMode,
                pins.Count,
                measurementName,
                distances.Count,
                distances.Min(),
                distances.Max(),
                distances.Average(),
                distances.Max() - distances.Min());
        }

        private bool TryGetRoi(Mat source, out Rect roi)
        {
            roi = default;
            string text = GetString("CvROI", string.Empty);
            string[] values = text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 4
                || !int.TryParse(values[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(values[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(values[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
            {
                return false;
            }

            roi = new Rect(x, y, width, height);
            return width > 0
                && height > 0
                && x >= 0
                && y >= 0
                && roi.Right <= source.Width
                && roi.Bottom <= source.Height;
        }

        private string GetString(string key, string defaultValue)
        {
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                if (string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(pair.Value) ? defaultValue : pair.Value;
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

        private double GetDouble(string key, double defaultValue)
        {
            return double.TryParse(GetString(key, string.Empty), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                ? value
                : defaultValue;
        }

        private bool GetBool(string key, bool defaultValue)
        {
            return bool.TryParse(GetString(key, string.Empty), out bool value) ? value : defaultValue;
        }

        private readonly struct PinRun
        {
            public PinRun(int start, int end)
            {
                Start = start;
                End = end;
            }

            public int Start { get; }
            public int End { get; }
            public int Width => End - Start + 1;
            public double Center => (Start + End) / 2D;
        }

        private readonly struct GapRun
        {
            public GapRun(int start, int end)
            {
                Start = start;
                End = end;
            }

            public int Start { get; }
            public int End { get; }
            public int Width => End - Start + 1;
        }

        private readonly struct PitchRun
        {
            public PitchRun(double startCenter, double endCenter)
            {
                StartCenter = startCenter;
                EndCenter = endCenter;
            }

            public double StartCenter { get; }
            public double EndCenter { get; }
            public double Distance => EndCenter - StartCenter;
        }
    }
}
