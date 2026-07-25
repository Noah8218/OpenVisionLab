using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
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
    public enum CircleGaugeEdgePolarity
    {
        DarkToLight,
        LightToDark,
        Either
    }

    internal sealed class VisionPipelineCircleGaugeTool : IVisionTool
    {
        private readonly VisionPipelineStep step;
        private readonly Dictionary<string, string> parameters;

        public VisionPipelineCircleGaugeTool(VisionPipelineStep step)
        {
            this.step = step ?? throw new ArgumentNullException(nameof(step));
            parameters = new Dictionary<string, string>(step.Parameters ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
            Name = string.IsNullOrWhiteSpace(step.Name) ? "CircleGauge" : step.Name;
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (OpenCvHelper.IsImageEmpty(source))
            {
                return VisionToolResult.Failed(VisionToolErrorCode.InputImageInvalid, "CircleGauge input image is empty.", stopwatch.Elapsed);
            }

            Mat resultImage = CreateColor(source);
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();
            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = 0D
            };

            if (!TryReadConfiguration(source, out Configuration config, out string configurationError))
            {
                return Fail(resultImage, stopwatch, configurationError, metrics, overlays);
            }

            DrawAnnulus(resultImage, config, new Scalar(0, 180, 255));
            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Rectangle,
                Label = "CircleGauge ROI",
                Bounds = new RectangleF(config.Roi.X, config.Roi.Y, config.Roi.Width, config.Roi.Height)
            });

            using Mat gray = CreateGray(source);
            List<RadialSample> samples = CollectSamples(gray, config);
            List<Point2d> points = samples.Where(item => item.Accepted).Select(item => item.Point).ToList();
            DrawSamples(resultImage, samples);

            double supportRatio = points.Count / (double)config.ScanCount;
            double coverage = config.SweepAngleDeg * supportRatio;
            metrics[VisionPipelineKnownMetrics.CircleSupportCount] = points.Count;
            metrics[VisionPipelineKnownMetrics.CircleSupportRatio] = supportRatio;
            metrics[VisionPipelineKnownMetrics.CircleCoverageDeg] = coverage;
            if (points.Count < 3 || supportRatio < config.MinimumSupportRatio)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    $"CircleGauge support ratio {supportRatio:0.###} is below {config.MinimumSupportRatio:0.###} ({points.Count}/{config.ScanCount}).",
                    metrics,
                    overlays);
            }

            if (!TryRobustFit(points, out CircleFit fit, out List<Point2d> inliers))
            {
                return Fail(resultImage, stopwatch, "CircleGauge could not fit a finite circle from the radial support points.", metrics, overlays);
            }

            DrawFitOutliers(resultImage, points, inliers);

            supportRatio = inliers.Count / (double)config.ScanCount;
            coverage = config.SweepAngleDeg * supportRatio;
            metrics[VisionPipelineKnownMetrics.CircleCenterX] = fit.Center.X;
            metrics[VisionPipelineKnownMetrics.CircleCenterY] = fit.Center.Y;
            metrics[VisionPipelineKnownMetrics.CircleRadiusPx] = fit.Radius;
            metrics[VisionPipelineKnownMetrics.CircleDiameterPx] = fit.Radius * 2D;
            metrics[VisionPipelineKnownMetrics.CircleSupportCount] = inliers.Count;
            metrics[VisionPipelineKnownMetrics.CircleSupportRatio] = supportRatio;
            metrics[VisionPipelineKnownMetrics.CircleCoverageDeg] = coverage;
            metrics[VisionPipelineKnownMetrics.CircleFitResidualPx] = fit.Residual;

            DrawFit(resultImage, config, fit);
            VisionToolOverlay pointOverlay = new VisionToolOverlay { Kind = VisionToolOverlayKind.Points, Label = "Circle support" };
            pointOverlay.Points.AddRange(inliers.Select(point => new PointF((float)point.X, (float)point.Y)));
            overlays.Add(pointOverlay);
            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Point,
                Label = "Circle center",
                Center = new PointF((float)fit.Center.X, (float)fit.Center.Y)
            });

            if (fit.Radius < config.MinimumRadius || fit.Radius > config.MaximumRadius)
            {
                return Fail(resultImage, stopwatch, $"CircleGauge fitted radius {fit.Radius:0.###}px is outside {config.MinimumRadius:0.###}..{config.MaximumRadius:0.###}px.", metrics, overlays);
            }
            if (supportRatio < config.MinimumSupportRatio)
            {
                return Fail(resultImage, stopwatch, $"CircleGauge robust-fit support ratio {supportRatio:0.###} is below {config.MinimumSupportRatio:0.###}.", metrics, overlays);
            }
            if (fit.Residual > config.MaximumFitResidual)
            {
                return Fail(resultImage, stopwatch, $"CircleGauge fit residual {fit.Residual:0.###}px exceeds {config.MaximumFitResidual:0.###}px.", metrics, overlays);
            }

            metrics[VisionPipelineKnownMetrics.ResultCount] = 1D;
            string message = $"PASS R={fit.Radius:0.###}px S={supportRatio:0.###} C={coverage:0.#}deg RMS={fit.Residual:0.###}px";
            DrawStatus(resultImage, message, true);
            stopwatch.Stop();
            VisionToolResult result = VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics, overlays);
            result.Message = message;
            VisionPipelineGeometryFeatureStore.Set(result, new[]
            {
                new VisionPipelineGeometryFeatureResult
                {
                    SourceStep = step.Name ?? string.Empty,
                    FeatureName = "Circle",
                    Kind = VisionPipelineGeometryKind.Circle,
                    CoordinateLayer = step.InputLayer ?? string.Empty,
                    ImageWidth = source.Width,
                    ImageHeight = source.Height,
                    CenterX = fit.Center.X,
                    CenterY = fit.Center.Y,
                    X1 = fit.Center.X,
                    Y1 = fit.Center.Y,
                    RadiusPx = fit.Radius,
                    SupportCount = inliers.Count,
                    SupportRatio = supportRatio,
                    CoverageDeg = coverage,
                    FitResidualPx = fit.Residual
                },
                new VisionPipelineGeometryFeatureResult
                {
                    SourceStep = step.Name ?? string.Empty,
                    FeatureName = "Center",
                    Kind = VisionPipelineGeometryKind.Point,
                    CoordinateLayer = step.InputLayer ?? string.Empty,
                    ImageWidth = source.Width,
                    ImageHeight = source.Height,
                    CenterX = fit.Center.X,
                    CenterY = fit.Center.Y,
                    X1 = fit.Center.X,
                    Y1 = fit.Center.Y,
                    SupportCount = inliers.Count,
                    SupportRatio = supportRatio,
                    CoverageDeg = coverage,
                    FitResidualPx = fit.Residual
                }
            });
            return result;
        }

        private bool TryReadConfiguration(Mat source, out Configuration config, out string error)
        {
            if (!GetBool("USE_ROI", false) || !TryGetRect("CvROI", out Rect reviewedRoi))
            {
                config = null;
                error = "CircleGauge requires USE_ROI=true and one valid operator-reviewed CvROI.";
                return false;
            }

            config = new Configuration
            {
                Center = new Point2d(GetDouble("CENTER_X", source.Width / 2D), GetDouble("CENTER_Y", source.Height / 2D)),
                MinimumRadius = GetDouble("RADIUS_MIN", 20D),
                MaximumRadius = GetDouble("RADIUS_MAX", 60D),
                StartAngleDeg = GetDouble("START_ANGLE_DEG", 0D),
                SweepAngleDeg = GetDouble("SWEEP_ANGLE_DEG", 360D),
                ScanCount = GetInt("SCAN_COUNT", 180),
                Polarity = GetEnum("EDGE_POLARITY", CircleGaugeEdgePolarity.Either),
                MinimumContrast = GetDouble("MIN_CONTRAST", 12D),
                MinimumSupportRatio = GetDouble("MIN_SUPPORT_RATIO", 0.60D),
                MaximumFitResidual = GetDouble("MAX_FIT_RESIDUAL_PX", 2D),
                Roi = reviewedRoi
            };

            if (config.Roi.Width <= 0 || config.Roi.Height <= 0 || config.Roi.X < 0 || config.Roi.Y < 0 || config.Roi.Right > source.Width || config.Roi.Bottom > source.Height)
            {
                error = "CircleGauge CvROI must be fully inside the source image.";
                return false;
            }
            if (!config.Roi.Contains(new OpenCvSharp.Point((int)Math.Round(config.Center.X), (int)Math.Round(config.Center.Y))))
            {
                error = "CircleGauge center must be inside the reviewed ROI.";
                return false;
            }
            if (config.MinimumRadius <= 0D || config.MaximumRadius <= config.MinimumRadius)
            {
                error = "CircleGauge requires 0 < RADIUS_MIN < RADIUS_MAX.";
                return false;
            }
            if (config.ScanCount < 3 || config.SweepAngleDeg <= 0D || config.SweepAngleDeg > 360D)
            {
                error = "CircleGauge requires SCAN_COUNT >= 3 and SWEEP_ANGLE_DEG in (0, 360].";
                return false;
            }
            if (config.MinimumContrast < 0D || config.MinimumSupportRatio <= 0D || config.MinimumSupportRatio > 1D || config.MaximumFitResidual < 0D)
            {
                error = "CircleGauge contrast/support/residual gates are invalid.";
                return false;
            }
            error = string.Empty;
            return true;
        }

        private static List<RadialSample> CollectSamples(Mat gray, Configuration config)
        {
            List<RadialSample> samples = new List<RadialSample>(config.ScanCount);
            for (int index = 0; index < config.ScanCount; index++)
            {
                double fraction = config.ScanCount == 1 ? 0D : index / (double)config.ScanCount;
                double angleDeg = config.StartAngleDeg + config.SweepAngleDeg * fraction;
                double angle = angleDeg * Math.PI / 180D;
                Point2d bestPoint = default;
                double bestStrength = double.MinValue;
                bool found = false;
                for (double radius = config.MinimumRadius; radius < config.MaximumRadius; radius += 1D)
                {
                    Point2d p1 = Polar(config.Center, radius, angle);
                    Point2d p2 = Polar(config.Center, radius + 1D, angle);
                    if (!TrySample(gray, config.Roi, p1, out byte before) || !TrySample(gray, config.Roi, p2, out byte after)) continue;
                    double delta = after - before;
                    double strength = config.Polarity == CircleGaugeEdgePolarity.DarkToLight
                        ? delta
                        : config.Polarity == CircleGaugeEdgePolarity.LightToDark
                            ? -delta
                            : Math.Abs(delta);
                    if (strength > bestStrength)
                    {
                        bestStrength = strength;
                        bestPoint = new Point2d((p1.X + p2.X) / 2D, (p1.Y + p2.Y) / 2D);
                        found = true;
                    }
                }
                samples.Add(new RadialSample
                {
                    Point = bestPoint,
                    Strength = found ? bestStrength : 0D,
                    Accepted = found && bestStrength >= config.MinimumContrast
                });
            }
            return samples;
        }

        private static bool TryRobustFit(List<Point2d> points, out CircleFit fit, out List<Point2d> inliers)
        {
            inliers = points?.ToList() ?? new List<Point2d>();
            if (!TryFit(inliers, out fit)) return false;
            double rejection = Math.Max(1.5D, fit.Residual * 2.5D);
            CircleFit initialFit = fit;
            List<Point2d> filtered = inliers.Where(point => Math.Abs(Distance(point, initialFit.Center) - initialFit.Radius) <= rejection).ToList();
            if (filtered.Count >= 3 && filtered.Count < inliers.Count && TryFit(filtered, out CircleFit refined))
            {
                inliers = filtered;
                fit = refined;
            }
            return true;
        }

        private static bool TryFit(IReadOnlyList<Point2d> points, out CircleFit fit)
        {
            fit = default;
            if (points == null || points.Count < 3) return false;
            double[,] matrix = new double[3, 3];
            double[] vector = new double[3];
            foreach (Point2d point in points)
            {
                double z = -(point.X * point.X + point.Y * point.Y);
                double[] row = { point.X, point.Y, 1D };
                for (int r = 0; r < 3; r++)
                {
                    vector[r] += row[r] * z;
                    for (int c = 0; c < 3; c++) matrix[r, c] += row[r] * row[c];
                }
            }
            if (!Solve3x3(matrix, vector, out double[] solution)) return false;
            Point2d center = new Point2d(-solution[0] / 2D, -solution[1] / 2D);
            double radiusSquared = center.X * center.X + center.Y * center.Y - solution[2];
            if (radiusSquared <= 0D || !IsFinite(radiusSquared)) return false;
            double radius = Math.Sqrt(radiusSquared);
            double residual = Math.Sqrt(points.Sum(point => Math.Pow(Distance(point, center) - radius, 2D)) / points.Count);
            if (!IsFinite(center.X) || !IsFinite(center.Y) || !IsFinite(radius) || !IsFinite(residual)) return false;
            fit = new CircleFit { Center = center, Radius = radius, Residual = residual };
            return true;
        }

        private static bool Solve3x3(double[,] matrix, double[] vector, out double[] solution)
        {
            double[,] augmented = new double[3, 4];
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++) augmented[r, c] = matrix[r, c];
                augmented[r, 3] = vector[r];
            }
            for (int pivot = 0; pivot < 3; pivot++)
            {
                int best = pivot;
                for (int row = pivot + 1; row < 3; row++) if (Math.Abs(augmented[row, pivot]) > Math.Abs(augmented[best, pivot])) best = row;
                if (Math.Abs(augmented[best, pivot]) <= 1e-10) { solution = null; return false; }
                if (best != pivot) for (int c = pivot; c < 4; c++) { double swap = augmented[pivot, c]; augmented[pivot, c] = augmented[best, c]; augmented[best, c] = swap; }
                double divisor = augmented[pivot, pivot];
                for (int c = pivot; c < 4; c++) augmented[pivot, c] /= divisor;
                for (int row = 0; row < 3; row++)
                {
                    if (row == pivot) continue;
                    double factor = augmented[row, pivot];
                    for (int c = pivot; c < 4; c++) augmented[row, c] -= factor * augmented[pivot, c];
                }
            }
            solution = new[] { augmented[0, 3], augmented[1, 3], augmented[2, 3] };
            return true;
        }

        private static void DrawAnnulus(Mat image, Configuration config, Scalar color)
        {
            OpenCvSharp.Point center = ToPoint(config.Center);
            Cv2.Ellipse(image, center, new OpenCvSharp.Size((int)Math.Round(config.MinimumRadius), (int)Math.Round(config.MinimumRadius)), 0, config.StartAngleDeg, config.StartAngleDeg + config.SweepAngleDeg, color, 1, LineTypes.AntiAlias);
            Cv2.Ellipse(image, center, new OpenCvSharp.Size((int)Math.Round(config.MaximumRadius), (int)Math.Round(config.MaximumRadius)), 0, config.StartAngleDeg, config.StartAngleDeg + config.SweepAngleDeg, color, 1, LineTypes.AntiAlias);
            Cv2.Rectangle(image, config.Roi, color, 1, LineTypes.AntiAlias);
        }

        private static void DrawSamples(Mat image, IEnumerable<RadialSample> samples)
        {
            foreach (RadialSample sample in samples ?? Enumerable.Empty<RadialSample>())
            {
                if (!IsFinite(sample.Point.X) || !IsFinite(sample.Point.Y)) continue;
                Cv2.Circle(image, ToPoint(sample.Point), sample.Accepted ? 2 : 1, sample.Accepted ? new Scalar(255, 255, 0) : new Scalar(0, 128, 255), -1, LineTypes.AntiAlias);
            }
        }

        private static void DrawFitOutliers(Mat image, IReadOnlyList<Point2d> candidates, IReadOnlyList<Point2d> inliers)
        {
            foreach (Point2d candidate in candidates ?? Array.Empty<Point2d>())
            {
                bool retained = (inliers ?? Array.Empty<Point2d>()).Any(inlier => Distance(candidate, inlier) <= 0.01D);
                if (!retained)
                {
                    Cv2.Circle(image, ToPoint(candidate), 3, new Scalar(0, 0, 255), -1, LineTypes.AntiAlias);
                }
            }
        }

        private static void DrawFit(Mat image, Configuration config, CircleFit fit)
        {
            Cv2.Ellipse(image, ToPoint(fit.Center), new OpenCvSharp.Size((int)Math.Round(fit.Radius), (int)Math.Round(fit.Radius)), 0, config.StartAngleDeg, config.StartAngleDeg + config.SweepAngleDeg, new Scalar(0, 255, 0), 2, LineTypes.AntiAlias);
            Cv2.DrawMarker(image, ToPoint(fit.Center), new Scalar(0, 255, 0), MarkerTypes.Cross, 19, 2);
        }

        private static VisionToolResult Fail(Mat image, Stopwatch stopwatch, string message, IDictionary<string, double> metrics, IEnumerable<VisionToolOverlay> overlays)
        {
            DrawStatus(image, "REJECT: " + message, false);
            stopwatch.Stop();
            VisionToolResult result = new VisionToolResult
            {
                Success = false,
                Message = message ?? string.Empty,
                ResultImage = image,
                Elapsed = stopwatch.Elapsed,
                ErrorCode = VisionToolErrorCode.InvalidParameter,
                ResultStatus = VisionToolResultStatus.InvalidParameter
            };
            foreach (KeyValuePair<string, double> metric in metrics ?? new Dictionary<string, double>()) result.Metrics[metric.Key] = metric.Value;
            result.Overlays.AddRange(overlays ?? Enumerable.Empty<VisionToolOverlay>());
            return result;
        }

        private static Mat CreateGray(Mat source)
        {
            if (source.Channels() == 1) return source.Clone();
            Mat gray = new Mat();
            Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }
        private static Mat CreateColor(Mat source)
        {
            if (source.Channels() != 1) return source.Clone();
            Mat color = new Mat();
            Cv2.CvtColor(source, color, ColorConversionCodes.GRAY2BGR);
            return color;
        }
        private static bool TrySample(Mat image, Rect roi, Point2d point, out byte value)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);
            if (x < roi.X || y < roi.Y || x >= roi.Right || y >= roi.Bottom || x < 0 || y < 0 || x >= image.Width || y >= image.Height)
            {
                value = 0;
                return false;
            }
            value = image.At<byte>(y, x);
            return true;
        }
        private static Point2d Polar(Point2d center, double radius, double angle) => new Point2d(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
        private static double Distance(Point2d a, Point2d b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        private static OpenCvSharp.Point ToPoint(Point2d value) => new OpenCvSharp.Point((int)Math.Round(value.X), (int)Math.Round(value.Y));
        private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
        private static void DrawStatus(Mat image, string text, bool pass)
        {
            if (image == null || image.Empty()) return;
            string bounded = text?.Length > 120 ? text.Substring(0, 120) : text ?? string.Empty;
            double scale = FitStatusScale(bounded, image.Width);
            Cv2.PutText(image, bounded, new OpenCvSharp.Point(12, Math.Max(20, Math.Min(image.Height - 10, 28))), HersheyFonts.HersheySimplex, scale, pass ? new Scalar(0, 220, 0) : new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
        }
        private static double FitStatusScale(string text, int imageWidth)
        {
            const double preferred = 0.55D;
            OpenCvSharp.Size size = Cv2.GetTextSize(text ?? string.Empty, HersheyFonts.HersheySimplex, preferred, 2, out _);
            return size.Width <= Math.Max(1, imageWidth - 24)
                ? preferred
                : Math.Max(0.28D, preferred * (imageWidth - 24D) / Math.Max(1D, size.Width));
        }
        private string GetString(string key) => parameters.TryGetValue(key, out string value) ? value?.Trim() ?? string.Empty : string.Empty;
        private int GetInt(string key, int fallback) => int.TryParse(GetString(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : fallback;
        private double GetDouble(string key, double fallback) => double.TryParse(GetString(key), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : fallback;
        private bool GetBool(string key, bool fallback) => bool.TryParse(GetString(key), out bool value) ? value : fallback;
        private T GetEnum<T>(string key, T fallback) where T : struct => Enum.TryParse(GetString(key), true, out T value) ? value : fallback;
        private bool TryGetRect(string key, out Rect roi)
        {
            roi = default;
            string[] parts = GetString(key).Split(',');
            if (parts.Length != 4) return false;
            int[] values = new int[4];
            for (int i = 0; i < 4; i++) if (!int.TryParse(parts[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out values[i])) return false;
            roi = new Rect(values[0], values[1], values[2], values[3]);
            return roi.Width > 0 && roi.Height > 0;
        }

        private sealed class Configuration
        {
            public Point2d Center { get; set; }
            public double MinimumRadius { get; set; }
            public double MaximumRadius { get; set; }
            public double StartAngleDeg { get; set; }
            public double SweepAngleDeg { get; set; }
            public int ScanCount { get; set; }
            public CircleGaugeEdgePolarity Polarity { get; set; }
            public double MinimumContrast { get; set; }
            public double MinimumSupportRatio { get; set; }
            public double MaximumFitResidual { get; set; }
            public Rect Roi { get; set; }
        }
        private sealed class RadialSample
        {
            public Point2d Point { get; set; }
            public double Strength { get; set; }
            public bool Accepted { get; set; }
        }
        private struct CircleFit
        {
            public Point2d Center { get; set; }
            public double Radius { get; set; }
            public double Residual { get; set; }
        }
    }
}
