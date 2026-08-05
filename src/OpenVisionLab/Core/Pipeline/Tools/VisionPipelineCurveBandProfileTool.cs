using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    /// <summary>
    /// Finds one vertically continuous dark band in a taught ROI and measures its width and curved center path.
    /// </summary>
    internal sealed class VisionPipelineCurveBandProfileTool : IVisionTool
    {
        private readonly IDictionary<string, string> parameters;

        public VisionPipelineCurveBandProfileTool(string name, IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "CurveBandProfile" : name;
            this.parameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (source == null || source.Empty())
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(VisionToolErrorCode.InputImageInvalid, "CurveBandProfile input image is empty.", stopwatch.Elapsed);
            }

            if (!GetBool("USE_ROI", false) || !TryGetRoi(source, out Rect roi))
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidRoi,
                    "CurveBandProfile requires USE_ROI=true and one in-image CvROI containing the full curved dark band.",
                    stopwatch.Elapsed);
            }

            int darkThreshold = Math.Clamp(GetInt("DarkThreshold", 128), 0, 255);
            double minimumArea = Math.Max(1, GetDouble("MinComponentArea", 300));
            int minimumHeight = Math.Max(1, GetInt("MinComponentHeight", 1));
            double minimumHeightRatio = Math.Clamp(GetDouble("MinComponentHeightRatio", 0.60), 0.01, 1.0);
            bool ignoreLeftBorderTouching = GetBool("IgnoreLeftBorderTouching", true);

            using Mat gray = CreateGray(source);
            using Mat roiGray = new Mat(gray, roi);
            using Mat darkMask = new Mat();
            Cv2.Threshold(roiGray, darkMask, darkThreshold, 255, ThresholdTypes.BinaryInv);
            Cv2.FindContours(
                darkMask,
                out OpenCvSharp.Point[][] contours,
                out _,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            int requiredHeight = Math.Max(minimumHeight, (int)Math.Ceiling(roi.Height * minimumHeightRatio));
            List<CurveComponent> components = contours
                .Select(contour => new CurveComponent(contour, Cv2.ContourArea(contour), Cv2.BoundingRect(contour)))
                .ToList();
            CurveComponent component = components
                .Where(candidate => candidate.Area >= minimumArea
                    && candidate.Bounds.Height >= requiredHeight
                    && (!ignoreLeftBorderTouching || candidate.Bounds.X > 0))
                .OrderBy(candidate => candidate.Bounds.X)
                .ThenByDescending(candidate => candidate.Area)
                .FirstOrDefault();

            if (component == null)
            {
                string candidates = string.Join(
                    "; ",
                    components
                        .OrderByDescending(candidate => candidate.Area)
                        .Take(3)
                        .Select(candidate => $"area={candidate.Area:0.#},bounds={candidate.Bounds.X},{candidate.Bounds.Y},{candidate.Bounds.Width},{candidate.Bounds.Height}"));
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.LineGaugeEdgeNotFound,
                    $"CurveBandProfile found no eligible dark component with area >= {minimumArea:0.###} and height >= {requiredHeight} in CvROI {roi.X},{roi.Y},{roi.Width},{roi.Height}. Candidates: {candidates}. Tune DarkThreshold, component limits, left-border rule, or ROI.",
                    stopwatch.Elapsed);
            }

            using Mat componentMask = Mat.Zeros(roi.Height, roi.Width, MatType.CV_8UC1);
            Cv2.DrawContours(componentMask, new[] { component.Contour }, -1, Scalar.White, -1);
            List<BandRow> profile = CreateProfile(componentMask, roi);
            if (profile.Count < requiredHeight)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.LineGaugeEdgeNotFound,
                    $"CurveBandProfile retained only {profile.Count} profile row(s); {requiredHeight} are required. Check the dark-band component and taught ROI.",
                    stopwatch.Elapsed);
            }

            Mat resultImage = CreateColorImage(source);
            Dictionary<string, double> metrics = CreateMetrics(source, resultImage, component.Bounds, profile, roi);
            List<VisionToolOverlay> overlays = CreateOverlays(component.Bounds, profile, roi, metrics);
            DrawResult(resultImage, component.Bounds, profile, roi, metrics);

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
            Cv2.CvtColor(source, gray, source.Channels() == 4 ? ColorConversionCodes.BGRA2GRAY : ColorConversionCodes.BGR2GRAY);
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

        private static List<BandRow> CreateProfile(Mat componentMask, Rect roi)
        {
            List<BandRow> rows = new List<BandRow>();
            for (int localY = 0; localY < componentMask.Rows; localY++)
            {
                int first = -1;
                int last = -1;
                for (int localX = 0; localX < componentMask.Cols; localX++)
                {
                    if (componentMask.At<byte>(localY, localX) == 0)
                    {
                        continue;
                    }

                    if (first < 0)
                    {
                        first = localX;
                    }

                    last = localX;
                }

                if (first >= 0 && last >= first)
                {
                    rows.Add(new BandRow(roi.X + first, roi.X + last, roi.Y + localY));
                }
            }

            return rows;
        }

        private Dictionary<string, double> CreateMetrics(
            Mat source,
            Mat resultImage,
            Rect localBounds,
            IReadOnlyList<BandRow> profile,
            Rect roi)
        {
            List<double> widths = profile.Select(row => (double)row.Width).ToList();
            List<PointF> outerPoints = profile.Select(row => new PointF(row.Left, row.Y)).ToList();
            List<PointF> innerPoints = profile.Select(row => new PointF(row.Right, row.Y)).ToList();
            List<PointF> centerPoints = profile.Select(row => new PointF((row.Left + row.Right) / 2F, row.Y)).ToList();
            double outerArcLength = GetPolylineLength(outerPoints);
            double innerArcLength = GetPolylineLength(innerPoints);
            double centerArcLength = GetPolylineLength(centerPoints);

            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = 1,
                [VisionPipelineKnownMetrics.DistanceCount] = widths.Count,
                [VisionPipelineKnownMetrics.DistancePxMin] = widths.Min(),
                [VisionPipelineKnownMetrics.DistancePxMax] = widths.Max(),
                [VisionPipelineKnownMetrics.DistancePxAvg] = widths.Average(),
                [VisionPipelineKnownMetrics.DistancePxRange] = widths.Max() - widths.Min(),
                [VisionPipelineKnownMetrics.CurveOuterArcLengthPx] = outerArcLength,
                [VisionPipelineKnownMetrics.CurveInnerArcLengthPx] = innerArcLength,
                [VisionPipelineKnownMetrics.CurveCenterArcLengthPx] = centerArcLength,
                [VisionPipelineKnownMetrics.CurveProfileRowCount] = profile.Count,
                [VisionPipelineKnownMetrics.EdgeCount] = 2,
                [VisionPipelineKnownMetrics.EdgePointCount] = profile.Count * 2,
                [VisionPipelineKnownMetrics.BoundsWidthMin] = localBounds.Width,
                [VisionPipelineKnownMetrics.BoundsWidthMax] = localBounds.Width,
                [VisionPipelineKnownMetrics.BoundsWidthAvg] = localBounds.Width,
                [VisionPipelineKnownMetrics.BoundsHeightMin] = localBounds.Height,
                [VisionPipelineKnownMetrics.BoundsHeightMax] = localBounds.Height,
                [VisionPipelineKnownMetrics.BoundsHeightAvg] = localBounds.Height,
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = resultImage.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = resultImage.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = resultImage.Channels()
            };

            double pixelPerMm = GetDouble("PIXELPERMM", 0);
            if (pixelPerMm > 0)
            {
                metrics[VisionPipelineKnownMetrics.DistanceMmMin] = metrics[VisionPipelineKnownMetrics.DistancePxMin] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmMax] = metrics[VisionPipelineKnownMetrics.DistancePxMax] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmAvg] = metrics[VisionPipelineKnownMetrics.DistancePxAvg] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmRange] = metrics[VisionPipelineKnownMetrics.DistancePxRange] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.CurveOuterArcLengthMm] = outerArcLength * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.CurveInnerArcLengthMm] = innerArcLength * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.CurveCenterArcLengthMm] = centerArcLength * pixelPerMm;
            }

            return metrics;
        }

        private static List<VisionToolOverlay> CreateOverlays(
            Rect localBounds,
            IReadOnlyList<BandRow> profile,
            Rect roi,
            IReadOnlyDictionary<string, double> metrics)
        {
            Rect bounds = new Rect(roi.X + localBounds.X, roi.Y + localBounds.Y, localBounds.Width, localBounds.Height);
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>
            {
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Rectangle,
                    Label = "Curve band",
                    Bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                    Center = new PointF(bounds.X + bounds.Width / 2F, bounds.Y + bounds.Height / 2F)
                }
            };

            AddPointsOverlay(overlays, "Outer curve", profile.Select(row => new PointF(row.Left, row.Y)));
            AddPointsOverlay(overlays, "Inner curve", profile.Select(row => new PointF(row.Right, row.Y)));
            int stride = Math.Max(1, profile.Count / 16);
            for (int index = 0; index < profile.Count; index += stride)
            {
                BandRow row = profile[index];
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = index == 0 ? $"Width avg {metrics[VisionPipelineKnownMetrics.DistancePxAvg]:0.##} px" : string.Empty,
                    Start = new PointF(row.Left, row.Y),
                    End = new PointF(row.Right, row.Y),
                    Center = new PointF((row.Left + row.Right) / 2F, row.Y)
                });
            }

            return overlays;
        }

        private static void AddPointsOverlay(List<VisionToolOverlay> overlays, string label, IEnumerable<PointF> points)
        {
            VisionToolOverlay overlay = new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Points,
                Label = label
            };
            overlay.Points.AddRange(points);
            overlays.Add(overlay);
        }

        private static void DrawResult(
            Mat resultImage,
            Rect localBounds,
            IReadOnlyList<BandRow> profile,
            Rect roi,
            IReadOnlyDictionary<string, double> metrics)
        {
            Rect bounds = new Rect(roi.X + localBounds.X, roi.Y + localBounds.Y, localBounds.Width, localBounds.Height);
            Cv2.Rectangle(resultImage, roi, new Scalar(220, 80, 255), 1, LineTypes.AntiAlias);
            Cv2.Rectangle(resultImage, bounds, new Scalar(0, 220, 255), 2, LineTypes.AntiAlias);
            for (int index = 1; index < profile.Count; index++)
            {
                BandRow previous = profile[index - 1];
                BandRow current = profile[index];
                Cv2.Line(resultImage, new OpenCvSharp.Point(previous.Left, previous.Y), new OpenCvSharp.Point(current.Left, current.Y), new Scalar(0, 220, 80), 2, LineTypes.AntiAlias);
                Cv2.Line(resultImage, new OpenCvSharp.Point(previous.Right, previous.Y), new OpenCvSharp.Point(current.Right, current.Y), new Scalar(0, 120, 255), 2, LineTypes.AntiAlias);
            }

            int stride = Math.Max(1, profile.Count / 16);
            for (int index = 0; index < profile.Count; index += stride)
            {
                BandRow row = profile[index];
                Cv2.Line(resultImage, new OpenCvSharp.Point(row.Left, row.Y), new OpenCvSharp.Point(row.Right, row.Y), new Scalar(0, 0, 255), 1, LineTypes.AntiAlias);
            }

            string label = $"Arc {metrics[VisionPipelineKnownMetrics.CurveCenterArcLengthPx]:0.##} px | width {metrics[VisionPipelineKnownMetrics.DistancePxAvg]:0.##} px";
            Cv2.PutText(resultImage, label, new OpenCvSharp.Point(bounds.X, Math.Max(14, bounds.Y - 5)), HersheyFonts.HersheySimplex, 0.42, new Scalar(0, 255, 255), 1, LineTypes.AntiAlias);
        }

        private static double GetPolylineLength(IReadOnlyList<PointF> points)
        {
            double length = 0;
            for (int index = 1; index < points.Count; index++)
            {
                double dx = points[index].X - points[index - 1].X;
                double dy = points[index].Y - points[index - 1].Y;
                length += Math.Sqrt(dx * dx + dy * dy);
            }

            return length;
        }

        private bool TryGetRoi(Mat source, out Rect roi)
        {
            roi = default;
            string[] values = GetString("CvROI", string.Empty)
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 4
                || !int.TryParse(values[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(values[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(values[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
            {
                return false;
            }

            roi = new Rect(x, y, width, height);
            return width > 0 && height > 0 && x >= 0 && y >= 0 && roi.Right <= source.Width && roi.Bottom <= source.Height;
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

        private sealed class CurveComponent
        {
            public CurveComponent(OpenCvSharp.Point[] contour, double area, Rect bounds)
            {
                Contour = contour;
                Area = area;
                Bounds = bounds;
            }

            public OpenCvSharp.Point[] Contour { get; }
            public double Area { get; }
            public Rect Bounds { get; }
        }

        private readonly struct BandRow
        {
            public BandRow(int left, int right, int y)
            {
                Left = left;
                Right = right;
                Y = y;
            }

            public int Left { get; }
            public int Right { get; }
            public int Y { get; }
            public int Width => Right - Left + 1;
        }
    }
}
