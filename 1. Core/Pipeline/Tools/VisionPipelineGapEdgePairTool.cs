using Lib.OpenCV;
using Lib.OpenCV.Tool;
using Lib.Line;
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
    /// Opt-in LineDistance mode for measuring the vertical thickness of one long dark band.
    /// It deliberately fails closed when the two supporting edges are weak or ambiguous.
    /// </summary>
    internal sealed class VisionPipelineGapEdgePairTool : IVisionTool
    {
        public const string UseParameter = "USE_GAP_EDGE_PAIR";
        public const string MinimumGapParameter = "GAP_MIN_PX";
        public const string MaximumGapParameter = "GAP_MAX_PX";
        public const string MaximumAngleParameter = "GAP_MAX_ANGLE_DEG";
        public const string MaximumParallelDeltaParameter = "GAP_MAX_PARALLEL_DELTA_DEG";
        public const string MinimumSupportRatioParameter = "GAP_MIN_SUPPORT_RATIO";
        public const string MinimumDarkContrastParameter = "GAP_MIN_DARK_CONTRAST";
        public const string MinimumDarkCoverageParameter = "GAP_MIN_DARK_COVERAGE_RATIO";
        public const string MinimumScoreMarginParameter = "GAP_MIN_SCORE_MARGIN";

        private readonly LineGaugeProperty property;
        private readonly Dictionary<string, string> parameters;

        public VisionPipelineGapEdgePairTool(
            string name,
            LineGaugeProperty property,
            IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "GapEdgePair" : name;
            this.property = property ?? throw new ArgumentNullException(nameof(property));
            this.parameters = new Dictionary<string, string>(parameters ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (OpenCvHelper.IsImageEmpty(source))
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(VisionToolErrorCode.InputImageInvalid, "Gap edge-pair input image is empty.", stopwatch.Elapsed);
            }

            Rect roi = property.CvROI;
            if (!property.USE_ROI
                || roi.Width <= 0
                || roi.Height <= 0
                || roi.X < 0
                || roi.Y < 0
                || roi.Right > source.Width
                || roi.Bottom > source.Height)
            {
                Mat invalidImage = CreateResultImage(source);
                DrawStatus(invalidImage, "REJECT: reviewed ROI is required", false);
                return Fail(
                    invalidImage,
                    stopwatch,
                    "Gap edge-pair mode requires one reviewed ROI fully inside the source image.",
                    new Dictionary<string, double>(),
                    CreateRoiOverlays(roi));
            }

            double minimumGap = GetDouble(MinimumGapParameter, 12D);
            double maximumGap = GetDouble(MaximumGapParameter, 60D);
            double maximumAngle = GetDouble(MaximumAngleParameter, 8D);
            double maximumParallelDelta = GetDouble(MaximumParallelDeltaParameter, 4D);
            double minimumSupportRatio = GetDouble(MinimumSupportRatioParameter, 0.26D);
            double minimumDarkContrast = GetDouble(MinimumDarkContrastParameter, 8D);
            double minimumDarkCoverage = GetDouble(MinimumDarkCoverageParameter, 0.25D);
            double minimumScoreMargin = GetDouble(MinimumScoreMarginParameter, 0.05D);
            int cannyLow = GetInt("CANNY_LOW", 10);
            int cannyHigh = GetInt("CANNY_HIGH", 45);

            using Mat gray = CreateGray(source);
            using Mat crop = new Mat(gray, roi);
            using Mat blurred = new Mat();
            using Mat edges = new Mat();
            Cv2.GaussianBlur(crop, blurred, new OpenCvSharp.Size(5, 5), 0);
            Cv2.Canny(blurred, edges, cannyLow, cannyHigh, 3, true);

            List<GapLineCandidate> candidates = DetectCandidates(
                edges,
                roi,
                maximumAngle);
            GapPairDiagnostics pairDiagnostics = new GapPairDiagnostics();
            List<GapPairCandidate> pairs = CreatePairs(
                crop,
                blurred,
                candidates,
                roi,
                minimumGap,
                maximumGap,
                maximumParallelDelta,
                minimumSupportRatio,
                minimumDarkContrast,
                minimumDarkCoverage,
                pairDiagnostics);

            Mat resultImage = CreateResultImage(source);
            DrawRoi(resultImage, roi);
            DrawCandidateLines(resultImage, candidates);

            Dictionary<string, double> metrics = CreateCandidateMetrics(source, resultImage, candidates, pairs, pairDiagnostics);
            List<VisionToolOverlay> overlays = CreateRoiOverlays(roi);
            AddCandidateOverlays(overlays, candidates);

            if (pairs.Count == 0)
            {
                DrawStatus(resultImage, "REJECT: no supported dark edge pair", false);
                return Fail(
                    resultImage,
                    stopwatch,
                    "Gap edge-pair found no upper edge with a supported nearest lower dark-band boundary.",
                    metrics,
                    overlays);
            }

            GapPairCandidate selected = pairs[0];
            GapPairCandidate distinctCompetitor = pairs.Skip(1).FirstOrDefault(pair => !AreEquivalentPairs(selected, pair));
            double scoreMargin = distinctCompetitor != null ? selected.Score - distinctCompetitor.Score : 99D;
            List<LineSegment2D> distanceLines = CreateDistanceLines(selected);
            AddSelectedMetrics(metrics, selected, distanceLines, scoreMargin, resultImage);
            AddSelectedOverlays(overlays, selected, distanceLines);
            DrawSelectedPair(resultImage, selected, distanceLines);

            if (scoreMargin < minimumScoreMargin)
            {
                DrawStatus(resultImage, $"REJECT: ambiguous pair margin {scoreMargin:0.###}", false);
                return Fail(
                    resultImage,
                    stopwatch,
                    $"Gap edge-pair is ambiguous. Score margin {scoreMargin.ToString("0.###", CultureInfo.InvariantCulture)} is below {minimumScoreMargin.ToString("0.###", CultureInfo.InvariantCulture)}.",
                    metrics,
                    overlays);
            }

            DrawStatus(resultImage, $"PASS Gap {metrics[VisionPipelineKnownMetrics.DistancePxAvg]:0.##} px", true);
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
            Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        private static Mat CreateResultImage(Mat source)
        {
            if (source.Channels() == 1)
            {
                Mat color = new Mat();
                Cv2.CvtColor(source, color, ColorConversionCodes.GRAY2BGR);
                return color;
            }

            return source.Clone();
        }

        private static List<GapLineCandidate> DetectCandidates(
            Mat edges,
            Rect roi,
            double maximumAngle)
        {
            double minimumLineLength = Math.Max(20D, roi.Width * 0.20D);
            LineSegmentPoint[] rawLines = Cv2.HoughLinesP(
                edges,
                1,
                Math.PI / 720D,
                25,
                minimumLineLength,
                20D);

            List<GapLineCandidate> rawCandidates = new List<GapLineCandidate>();
            foreach (LineSegmentPoint raw in rawLines ?? Array.Empty<LineSegmentPoint>())
            {
                Point2d localStart = new Point2d(raw.P1.X, raw.P1.Y);
                Point2d localEnd = new Point2d(raw.P2.X, raw.P2.Y);
                double angle = NormalizeHorizontalAngle(Math.Atan2(localEnd.Y - localStart.Y, localEnd.X - localStart.X) * 180D / Math.PI);
                if (Math.Abs(angle) > maximumAngle)
                {
                    continue;
                }

                if (localStart.X > localEnd.X)
                {
                    Point2d swap = localStart;
                    localStart = localEnd;
                    localEnd = swap;
                }

                GapLineCandidate candidate = CreateRefinedCandidate(edges, roi, localStart, localEnd, angle);
                if (candidate != null && candidate.Length >= minimumLineLength)
                {
                    rawCandidates.Add(candidate);
                }
            }

            List<GapLineCandidate> deduplicated = new List<GapLineCandidate>();
            foreach (GapLineCandidate candidate in rawCandidates.OrderByDescending(item => item.Length))
            {
                double centerX = roi.X + roi.Width / 2D;
                int duplicateIndex = deduplicated.FindIndex(existing =>
                    Math.Abs(existing.YAt(centerX) - candidate.YAt(centerX)) < 3D
                    && AngleDelta(existing.AngleDegrees, candidate.AngleDegrees) < 1.5D);
                if (duplicateIndex >= 0)
                {
                    deduplicated[duplicateIndex] = MergeCandidates(deduplicated[duplicateIndex], candidate);
                }
                else
                {
                    deduplicated.Add(candidate);
                }

                if (deduplicated.Count >= 30)
                {
                    break;
                }
            }

            return deduplicated;
        }

        private static GapLineCandidate MergeCandidates(GapLineCandidate first, GapLineCandidate second)
        {
            List<OpenCvSharp.Point> support = first.SupportPoints
                .Concat(second.SupportPoints)
                .Distinct()
                .ToList();
            double minimumX = Math.Min(first.MinimumX, second.MinimumX);
            double maximumX = Math.Max(first.MaximumX, second.MaximumX);
            double angle = first.Length >= second.Length ? first.AngleDegrees : second.AngleDegrees;
            Point2d start = first.Length >= second.Length ? first.Start : second.Start;
            Point2d end = first.Length >= second.Length ? first.End : second.End;

            if (support.Count >= 8)
            {
                Line2D fitted = Cv2.FitLine(support.ToArray(), DistanceTypes.L2, 0, 0.01, 0.01);
                if (Math.Abs(fitted.Vx) > 0.000001D)
                {
                    start = new Point2d(minimumX, fitted.Y1 + ((minimumX - fitted.X1) * fitted.Vy / fitted.Vx));
                    end = new Point2d(maximumX, fitted.Y1 + ((maximumX - fitted.X1) * fitted.Vy / fitted.Vx));
                    angle = NormalizeHorizontalAngle(Math.Atan2(fitted.Vy, fitted.Vx) * 180D / Math.PI);
                }
            }

            return new GapLineCandidate(start, end, angle, support);
        }

        private static GapLineCandidate CreateRefinedCandidate(
            Mat edges,
            Rect roi,
            Point2d localStart,
            Point2d localEnd,
            double fallbackAngle)
        {
            double dx = localEnd.X - localStart.X;
            if (Math.Abs(dx) < 0.000001D)
            {
                return null;
            }

            List<OpenCvSharp.Point> support = new List<OpenCvSharp.Point>();
            int startX = Math.Max(0, (int)Math.Floor(localStart.X));
            int endX = Math.Min(edges.Width - 1, (int)Math.Ceiling(localEnd.X));
            for (int x = startX; x <= endX; x++)
            {
                double expectedY = localStart.Y + ((x - localStart.X) * (localEnd.Y - localStart.Y) / dx);
                int firstY = Math.Max(0, (int)Math.Floor(expectedY - 2.5D));
                int lastY = Math.Min(edges.Height - 1, (int)Math.Ceiling(expectedY + 2.5D));
                for (int y = firstY; y <= lastY; y++)
                {
                    if (edges.At<byte>(y, x) > 0)
                    {
                        support.Add(new OpenCvSharp.Point(x + roi.X, y + roi.Y));
                    }
                }
            }

            Point2d globalStart = new Point2d(localStart.X + roi.X, localStart.Y + roi.Y);
            Point2d globalEnd = new Point2d(localEnd.X + roi.X, localEnd.Y + roi.Y);
            double angle = fallbackAngle;
            if (support.Count >= 8)
            {
                Line2D fitted = Cv2.FitLine(support.ToArray(), DistanceTypes.L2, 0, 0.01, 0.01);
                if (Math.Abs(fitted.Vx) > 0.000001D)
                {
                    double globalX1 = localStart.X + roi.X;
                    double globalX2 = localEnd.X + roi.X;
                    globalStart = new Point2d(globalX1, fitted.Y1 + ((globalX1 - fitted.X1) * fitted.Vy / fitted.Vx));
                    globalEnd = new Point2d(globalX2, fitted.Y1 + ((globalX2 - fitted.X1) * fitted.Vy / fitted.Vx));
                    angle = NormalizeHorizontalAngle(Math.Atan2(fitted.Vy, fitted.Vx) * 180D / Math.PI);
                }
            }

            return new GapLineCandidate(globalStart, globalEnd, angle, support);
        }

        private static List<GapPairCandidate> CreatePairs(
            Mat grayCrop,
            Mat blurredCrop,
            IReadOnlyList<GapLineCandidate> candidates,
            Rect roi,
            double minimumGap,
            double maximumGap,
            double maximumParallelDelta,
            double minimumSupportRatio,
            double minimumDarkContrast,
            double minimumDarkCoverage,
            GapPairDiagnostics diagnostics)
        {
            List<GapPairCandidate> pairs = new List<GapPairCandidate>();
            foreach (GapLineCandidate upper in candidates)
            {
                if (!TryTraceNearestLowerBoundary(
                        blurredCrop,
                        roi,
                        upper,
                        minimumGap,
                        maximumGap,
                        minimumDarkContrast,
                        minimumSupportRatio,
                        out GapLineCandidate lower))
                {
                    continue;
                }

                double overlapStart = Math.Max(upper.MinimumX, lower.MinimumX);
                double overlapEnd = Math.Min(upper.MaximumX, lower.MaximumX);
                double overlap = overlapEnd - overlapStart;
                double supportRatio = overlap / roi.Width;
                if (supportRatio < minimumSupportRatio)
                {
                    continue;
                }

                diagnostics.OverlapPairCount++;

                double[] gaps = Enumerable.Range(0, 5)
                    .Select(index => overlapStart + ((index + 1D) * overlap / 6D))
                    .Select(x => lower.YAt(x) - upper.YAt(x))
                    .ToArray();
                double averageGap = gaps.Average();
                if (gaps.Any(gap => gap < minimumGap || gap > maximumGap))
                {
                    continue;
                }

                diagnostics.SeparationPairCount++;

                double angleDelta = AngleDelta(upper.AngleDegrees, lower.AngleDegrees);
                if (angleDelta > maximumParallelDelta)
                {
                    continue;
                }

                diagnostics.ParallelPairCount++;

                GapDarkStatistics dark = CalculateDarkStatistics(
                    grayCrop,
                    roi,
                    upper,
                    lower,
                    overlapStart,
                    overlapEnd,
                    averageGap,
                    minimumDarkContrast);
                diagnostics.BestDarkContrast = Math.Max(diagnostics.BestDarkContrast, dark.QualifiedMeanContrast);
                diagnostics.BestDarkCoverageRatio = Math.Max(diagnostics.BestDarkCoverageRatio, dark.CoverageRatio);
                if (dark.CoverageRatio < minimumDarkCoverage)
                {
                    continue;
                }

                diagnostics.ContrastPairCount++;

                double lineSupport = Math.Min(upper.Length, lower.Length) / roi.Width;
                double score = (2D * supportRatio)
                    + lineSupport
                    + Math.Min(2D, dark.QualifiedMeanContrast / 64D)
                    + dark.CoverageRatio
                    + Math.Max(0D, (255D - dark.MeanBandGray) / 16D)
                    - (angleDelta / Math.Max(0.1D, maximumParallelDelta));
                pairs.Add(new GapPairCandidate(
                    upper,
                    lower,
                    overlapStart,
                    overlapEnd,
                    supportRatio,
                    averageGap,
                    angleDelta,
                    dark.QualifiedMeanContrast,
                    dark.CoverageRatio,
                    dark.MeanBandGray,
                    score));
            }

            return pairs.OrderByDescending(pair => pair.Score).ToList();
        }

        private static bool TryTraceNearestLowerBoundary(
            Mat blurredCrop,
            Rect roi,
            GapLineCandidate upper,
            double minimumGap,
            double maximumGap,
            double minimumDarkContrast,
            double minimumSupportRatio,
            out GapLineCandidate lower)
        {
            lower = null;
            List<OpenCvSharp.Point> traced = new List<OpenCvSharp.Point>();
            int firstX = Math.Max(roi.X, (int)Math.Ceiling(upper.MinimumX));
            int lastX = Math.Min(roi.Right - 1, (int)Math.Floor(upper.MaximumX));

            for (int globalX = firstX; globalX <= lastX; globalX++)
            {
                int localX = globalX - roi.X;
                int upperY = (int)Math.Round(upper.YAt(globalX)) - roi.Y;
                int aboveStart = upperY - 10;
                int aboveEnd = upperY - 3;
                int coreStart = upperY + 4;
                int coreEnd = upperY + Math.Max(6, (int)Math.Floor(minimumGap * 0.75D));
                if (aboveStart < 0 || coreEnd >= blurredCrop.Height)
                {
                    continue;
                }

                double aboveMedian = MedianGray(blurredCrop, localX, aboveStart, aboveEnd);
                double coreMedian = MedianGray(blurredCrop, localX, coreStart, coreEnd);
                if (aboveMedian - coreMedian < minimumDarkContrast)
                {
                    continue;
                }

                double riseThreshold = coreMedian + Math.Max(
                    minimumDarkContrast * 2D,
                    (aboveMedian - coreMedian) * 0.12D);
                int searchStart = Math.Max(coreEnd + 1, (int)Math.Ceiling(upper.YAt(globalX) + minimumGap) - roi.Y);
                int searchEnd = Math.Min(
                    blurredCrop.Height - 3,
                    (int)Math.Floor(upper.YAt(globalX) + maximumGap) - roi.Y);
                for (int y = searchStart; y <= searchEnd; y++)
                {
                    int sustainedBrightCount = 0;
                    for (int offset = 0; offset < 3; offset++)
                    {
                        if (blurredCrop.At<byte>(y + offset, localX) >= riseThreshold)
                        {
                            sustainedBrightCount++;
                        }
                    }

                    if (sustainedBrightCount >= 2)
                    {
                        traced.Add(new OpenCvSharp.Point(globalX, y + roi.Y));
                        break;
                    }
                }
            }

            if (traced.Count < 8 || traced.Count / (double)roi.Width < minimumSupportRatio)
            {
                return false;
            }

            Line2D initial = Cv2.FitLine(traced.ToArray(), DistanceTypes.L1, 0, 0.01, 0.01);
            if (Math.Abs(initial.Vx) < 0.000001D)
            {
                return false;
            }

            List<OpenCvSharp.Point> inliers = traced
                .Where(point => Math.Abs(point.Y - (initial.Y1 + ((point.X - initial.X1) * initial.Vy / initial.Vx))) <= 3D)
                .ToList();
            if (inliers.Count < 8 || inliers.Count / (double)roi.Width < minimumSupportRatio)
            {
                return false;
            }

            int minimumX = inliers.Min(point => point.X);
            int maximumX = inliers.Max(point => point.X);
            if ((maximumX - minimumX) / (double)roi.Width < minimumSupportRatio)
            {
                return false;
            }

            Line2D fitted = Cv2.FitLine(inliers.ToArray(), DistanceTypes.L2, 0, 0.01, 0.01);
            if (Math.Abs(fitted.Vx) < 0.000001D)
            {
                return false;
            }

            Point2d start = new Point2d(
                minimumX,
                fitted.Y1 + ((minimumX - fitted.X1) * fitted.Vy / fitted.Vx));
            Point2d end = new Point2d(
                maximumX,
                fitted.Y1 + ((maximumX - fitted.X1) * fitted.Vy / fitted.Vx));
            double angle = NormalizeHorizontalAngle(Math.Atan2(fitted.Vy, fitted.Vx) * 180D / Math.PI);
            lower = new GapLineCandidate(start, end, angle, inliers);
            return true;
        }

        private static double MedianGray(Mat image, int x, int firstY, int lastY)
        {
            List<byte> values = new List<byte>();
            for (int y = firstY; y <= lastY; y++)
            {
                values.Add(image.At<byte>(y, x));
            }

            values.Sort();
            int middle = values.Count / 2;
            return values.Count % 2 == 0
                ? (values[middle - 1] + values[middle]) / 2D
                : values[middle];
        }

        private static GapDarkStatistics CalculateDarkStatistics(
            Mat grayCrop,
            Rect roi,
            GapLineCandidate upper,
            GapLineCandidate lower,
            double overlapStart,
            double overlapEnd,
            double averageGap,
            double minimumDarkContrast)
        {
            List<double> localContrasts = new List<double>();
            List<double> localBandMeans = new List<double>();
            int xStep = Math.Max(1, (int)Math.Floor((overlapEnd - overlapStart) / 32D));
            int outsideDepth = Math.Max(3, (int)Math.Round(Math.Min(averageGap / 2D, 16D)));

            for (int globalX = (int)Math.Ceiling(overlapStart); globalX <= (int)Math.Floor(overlapEnd); globalX += xStep)
            {
                double insideSum = 0D;
                int insideCount = 0;
                double outsideSum = 0D;
                int outsideCount = 0;
                int localX = globalX - roi.X;
                int upperY = (int)Math.Round(upper.YAt(globalX)) - roi.Y;
                int lowerY = (int)Math.Round(lower.YAt(globalX)) - roi.Y;
                for (int y = upperY + 2; y <= lowerY - 2; y += 2)
                {
                    if (localX >= 0 && localX < grayCrop.Width && y >= 0 && y < grayCrop.Height)
                    {
                        insideSum += grayCrop.At<byte>(y, localX);
                        insideCount++;
                    }
                }

                for (int offset = 2; offset <= outsideDepth; offset += 2)
                {
                    int above = upperY - offset;
                    int below = lowerY + offset;
                    if (localX >= 0 && localX < grayCrop.Width && above >= 0 && above < grayCrop.Height)
                    {
                        outsideSum += grayCrop.At<byte>(above, localX);
                        outsideCount++;
                    }

                    if (localX >= 0 && localX < grayCrop.Width && below >= 0 && below < grayCrop.Height)
                    {
                        outsideSum += grayCrop.At<byte>(below, localX);
                        outsideCount++;
                    }
                }

                if (insideCount > 0 && outsideCount > 0)
                {
                    localBandMeans.Add(insideSum / insideCount);
                    localContrasts.Add((outsideSum / outsideCount) - (insideSum / insideCount));
                }
            }

            if (localContrasts.Count == 0)
            {
                return new GapDarkStatistics(0D, 0D, 255D);
            }

            List<double> qualified = localContrasts.Where(value => value >= minimumDarkContrast).ToList();
            return new GapDarkStatistics(
                qualified.Count == 0 ? 0D : qualified.Average(),
                (double)qualified.Count / localContrasts.Count,
                localBandMeans.Average());
        }

        private static List<LineSegment2D> CreateDistanceLines(GapPairCandidate pair)
        {
            List<LineSegment2D> lines = new List<LineSegment2D>();
            for (int index = 0; index < 5; index++)
            {
                double x = pair.OverlapStart + ((index + 1D) * (pair.OverlapEnd - pair.OverlapStart) / 6D);
                OpenCvSharp.Point top = new OpenCvSharp.Point((int)Math.Round(x), (int)Math.Round(pair.Upper.YAt(x)));
                OpenCvSharp.Point bottom = new OpenCvSharp.Point((int)Math.Round(x), (int)Math.Round(pair.Lower.YAt(x)));
                if (bottom.Y > top.Y)
                {
                    lines.Add(new LineSegment2D(top, bottom));
                }
            }

            return lines;
        }

        private Dictionary<string, double> CreateCandidateMetrics(
            Mat source,
            Mat resultImage,
            IReadOnlyCollection<GapLineCandidate> candidates,
            IReadOnlyCollection<GapPairCandidate> pairs,
            GapPairDiagnostics diagnostics)
        {
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = 0D,
                [VisionPipelineKnownMetrics.EdgeCount] = candidates.Count,
                [VisionPipelineKnownMetrics.EdgePointCount] = candidates.Sum(candidate => candidate.SupportPoints.Count),
                [VisionPipelineKnownMetrics.GapCandidateLineCount] = candidates.Count,
                [VisionPipelineKnownMetrics.GapCandidatePairCount] = pairs.Count,
                [VisionPipelineKnownMetrics.GapOverlapPairCount] = diagnostics.OverlapPairCount,
                [VisionPipelineKnownMetrics.GapSeparationPairCount] = diagnostics.SeparationPairCount,
                [VisionPipelineKnownMetrics.GapParallelPairCount] = diagnostics.ParallelPairCount,
                [VisionPipelineKnownMetrics.GapContrastPairCount] = diagnostics.ContrastPairCount,
                [VisionPipelineKnownMetrics.GapBestDarkContrast] = diagnostics.BestDarkContrast,
                [VisionPipelineKnownMetrics.GapBestDarkCoverageRatio] = diagnostics.BestDarkCoverageRatio,
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = resultImage.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = resultImage.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = resultImage.Channels()
            };
        }

        private void AddSelectedMetrics(
            IDictionary<string, double> metrics,
            GapPairCandidate pair,
            IReadOnlyList<LineSegment2D> distanceLines,
            double scoreMargin,
            Mat resultImage)
        {
            List<double> distances = distanceLines.Select(line => line.Distance()).ToList();
            metrics[VisionPipelineKnownMetrics.ResultCount] = distanceLines.Count;
            metrics[VisionPipelineKnownMetrics.DistanceCount] = distanceLines.Count;
            metrics[VisionPipelineKnownMetrics.DistancePxMin] = distances.Min();
            metrics[VisionPipelineKnownMetrics.DistancePxMax] = distances.Max();
            metrics[VisionPipelineKnownMetrics.DistancePxAvg] = distances.Average();
            metrics[VisionPipelineKnownMetrics.DistancePxRange] = distances.Max() - distances.Min();
            metrics[VisionPipelineKnownMetrics.GapSelectedAngleDeltaDeg] = pair.AngleDelta;
            metrics[VisionPipelineKnownMetrics.GapSelectedSupportRatio] = pair.SupportRatio;
            metrics[VisionPipelineKnownMetrics.GapDarkContrast] = pair.DarkContrast;
            metrics[VisionPipelineKnownMetrics.GapDarkCoverageRatio] = pair.DarkCoverageRatio;
            metrics[VisionPipelineKnownMetrics.GapBandMeanGray] = pair.MeanBandGray;
            metrics[VisionPipelineKnownMetrics.GapScoreMargin] = scoreMargin;
            metrics[VisionPipelineKnownMetrics.GapUpperSupportPointCount] = pair.Upper.SupportPoints.Count;
            metrics[VisionPipelineKnownMetrics.GapLowerSupportPointCount] = pair.Lower.SupportPoints.Count;
            metrics[VisionPipelineKnownMetrics.LineLengthMin] = Math.Min(pair.Upper.Length, pair.Lower.Length);
            metrics[VisionPipelineKnownMetrics.LineLengthMax] = Math.Max(pair.Upper.Length, pair.Lower.Length);
            metrics[VisionPipelineKnownMetrics.LineLengthAvg] = (pair.Upper.Length + pair.Lower.Length) / 2D;
            metrics[VisionPipelineKnownMetrics.LineAngleMin] = Math.Min(pair.Upper.AngleDegrees, pair.Lower.AngleDegrees);
            metrics[VisionPipelineKnownMetrics.LineAngleMax] = Math.Max(pair.Upper.AngleDegrees, pair.Lower.AngleDegrees);
            metrics[VisionPipelineKnownMetrics.LineAngleAvg] = (pair.Upper.AngleDegrees + pair.Lower.AngleDegrees) / 2D;

            double pixelPerMm = property.PIXELPERMM;
            if (pixelPerMm > 0D)
            {
                metrics[VisionPipelineKnownMetrics.DistanceMmMin] = metrics[VisionPipelineKnownMetrics.DistancePxMin] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmMax] = metrics[VisionPipelineKnownMetrics.DistancePxMax] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmAvg] = metrics[VisionPipelineKnownMetrics.DistancePxAvg] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmRange] = metrics[VisionPipelineKnownMetrics.DistancePxRange] * pixelPerMm;
            }
        }

        private static List<VisionToolOverlay> CreateRoiOverlays(Rect roi)
        {
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();
            if (roi.Width > 0 && roi.Height > 0)
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Rectangle,
                    Label = "Gap search ROI",
                    Bounds = new RectangleF(roi.X, roi.Y, roi.Width, roi.Height)
                });
            }

            return overlays;
        }

        private static void AddCandidateOverlays(List<VisionToolOverlay> overlays, IEnumerable<GapLineCandidate> candidates)
        {
            int index = 1;
            foreach (GapLineCandidate candidate in candidates)
            {
                overlays.Add(CreateLineOverlay(candidate, $"Candidate edge {index++}"));
            }
        }

        private static void AddSelectedOverlays(
            List<VisionToolOverlay> overlays,
            GapPairCandidate pair,
            IEnumerable<LineSegment2D> distances)
        {
            overlays.Add(CreateLineOverlay(pair.Upper, "Selected upper gap edge"));
            overlays.Add(CreateLineOverlay(pair.Lower, "Selected lower gap edge"));

            VisionToolOverlay upperPoints = new VisionToolOverlay { Kind = VisionToolOverlayKind.Points, Label = "Upper edge support points" };
            upperPoints.Points.AddRange(pair.Upper.SupportPoints.Select(point => new PointF(point.X, point.Y)));
            overlays.Add(upperPoints);
            VisionToolOverlay lowerPoints = new VisionToolOverlay { Kind = VisionToolOverlayKind.Points, Label = "Lower edge support points" };
            lowerPoints.Points.AddRange(pair.Lower.SupportPoints.Select(point => new PointF(point.X, point.Y)));
            overlays.Add(lowerPoints);

            int index = 1;
            foreach (LineSegment2D distance in distances)
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = $"Gap D{index++}",
                    Start = new PointF(distance.Start.X, distance.Start.Y),
                    End = new PointF(distance.End.X, distance.End.Y),
                    Center = new PointF((distance.Start.X + distance.End.X) / 2F, (distance.Start.Y + distance.End.Y) / 2F)
                });
            }
        }

        private static VisionToolOverlay CreateLineOverlay(GapLineCandidate candidate, string label)
        {
            return new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Line,
                Label = label,
                Start = new PointF((float)candidate.Start.X, (float)candidate.Start.Y),
                End = new PointF((float)candidate.End.X, (float)candidate.End.Y),
                Center = new PointF((float)((candidate.Start.X + candidate.End.X) / 2D), (float)((candidate.Start.Y + candidate.End.Y) / 2D))
            };
        }

        private static void DrawRoi(Mat image, Rect roi)
        {
            Cv2.Rectangle(image, roi, new Scalar(0, 255, 0), 1, LineTypes.AntiAlias);
        }

        private static void DrawCandidateLines(Mat image, IEnumerable<GapLineCandidate> candidates)
        {
            foreach (GapLineCandidate candidate in candidates)
            {
                Cv2.Line(image, ToPoint(candidate.Start), ToPoint(candidate.End), new Scalar(0, 215, 255), 1, LineTypes.AntiAlias);
            }
        }

        private static void DrawSelectedPair(Mat image, GapPairCandidate pair, IEnumerable<LineSegment2D> distances)
        {
            Cv2.Line(image, ToPoint(pair.Upper.Start), ToPoint(pair.Upper.End), new Scalar(255, 0, 0), 3, LineTypes.AntiAlias);
            Cv2.Line(image, ToPoint(pair.Lower.Start), ToPoint(pair.Lower.End), new Scalar(255, 0, 255), 3, LineTypes.AntiAlias);
            foreach (LineSegment2D distance in distances)
            {
                Cv2.Line(image, distance.Start, distance.End, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            }
        }

        private static void DrawStatus(Mat image, string text, bool passed)
        {
            Scalar color = passed ? new Scalar(0, 200, 0) : new Scalar(0, 0, 255);
            Cv2.PutText(image, text, new OpenCvSharp.Point(8, 24), HersheyFonts.HersheySimplex, 0.55, color, 2, LineTypes.AntiAlias);
        }

        private static OpenCvSharp.Point ToPoint(Point2d point)
        {
            return new OpenCvSharp.Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }

        private static double NormalizeHorizontalAngle(double angle)
        {
            while (angle > 90D)
            {
                angle -= 180D;
            }

            while (angle < -90D)
            {
                angle += 180D;
            }

            return angle;
        }

        private static double AngleDelta(double first, double second)
        {
            return Math.Abs(NormalizeHorizontalAngle(first - second));
        }

        private static bool AreEquivalentPairs(GapPairCandidate first, GapPairCandidate second)
        {
            double overlapStart = Math.Max(first.OverlapStart, second.OverlapStart);
            double overlapEnd = Math.Min(first.OverlapEnd, second.OverlapEnd);
            double x = overlapEnd > overlapStart
                ? (overlapStart + overlapEnd) / 2D
                : (first.OverlapStart + first.OverlapEnd) / 2D;
            return Math.Abs(first.Upper.YAt(x) - second.Upper.YAt(x)) < 5D
                && Math.Abs(first.Lower.YAt(x) - second.Lower.YAt(x)) < 5D
                && AngleDelta(first.Upper.AngleDegrees, second.Upper.AngleDegrees) < 2D
                && AngleDelta(first.Lower.AngleDegrees, second.Lower.AngleDegrees) < 2D;
        }

        private VisionToolResult Fail(
            Mat resultImage,
            Stopwatch stopwatch,
            string message,
            IDictionary<string, double> metrics,
            IEnumerable<VisionToolOverlay> overlays)
        {
            stopwatch.Stop();
            VisionToolResult failed = VisionToolResult.Failed(VisionToolErrorCode.LineGaugeEdgeNotFound, message, stopwatch.Elapsed);
            failed.ResultImage = resultImage;
            foreach (KeyValuePair<string, double> metric in metrics ?? new Dictionary<string, double>())
            {
                failed.Metrics[metric.Key] = metric.Value;
            }

            foreach (VisionToolOverlay overlay in overlays ?? Enumerable.Empty<VisionToolOverlay>())
            {
                failed.Overlays.Add(overlay);
            }

            return failed;
        }

        private string GetString(string key, string defaultValue)
        {
            return parameters.TryGetValue(key, out string value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : defaultValue;
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

        private sealed class GapLineCandidate
        {
            public GapLineCandidate(Point2d start, Point2d end, double angleDegrees, List<OpenCvSharp.Point> supportPoints)
            {
                Start = start;
                End = end;
                AngleDegrees = angleDegrees;
                SupportPoints = supportPoints ?? new List<OpenCvSharp.Point>();
            }

            public Point2d Start { get; }
            public Point2d End { get; }
            public double AngleDegrees { get; }
            public List<OpenCvSharp.Point> SupportPoints { get; }
            public double MinimumX => Math.Min(Start.X, End.X);
            public double MaximumX => Math.Max(Start.X, End.X);
            public double Length => Math.Sqrt(((End.X - Start.X) * (End.X - Start.X)) + ((End.Y - Start.Y) * (End.Y - Start.Y)));

            public double YAt(double x)
            {
                double dx = End.X - Start.X;
                return Math.Abs(dx) < 0.000001D ? (Start.Y + End.Y) / 2D : Start.Y + ((x - Start.X) * (End.Y - Start.Y) / dx);
            }
        }

        private sealed class GapPairCandidate
        {
            public GapPairCandidate(
                GapLineCandidate upper,
                GapLineCandidate lower,
                double overlapStart,
                double overlapEnd,
                double supportRatio,
                double averageGap,
                double angleDelta,
                double darkContrast,
                double darkCoverageRatio,
                double meanBandGray,
                double score)
            {
                Upper = upper;
                Lower = lower;
                OverlapStart = overlapStart;
                OverlapEnd = overlapEnd;
                SupportRatio = supportRatio;
                AverageGap = averageGap;
                AngleDelta = angleDelta;
                DarkContrast = darkContrast;
                DarkCoverageRatio = darkCoverageRatio;
                MeanBandGray = meanBandGray;
                Score = score;
            }

            public GapLineCandidate Upper { get; }
            public GapLineCandidate Lower { get; }
            public double OverlapStart { get; }
            public double OverlapEnd { get; }
            public double SupportRatio { get; }
            public double AverageGap { get; }
            public double AngleDelta { get; }
            public double DarkContrast { get; }
            public double DarkCoverageRatio { get; }
            public double MeanBandGray { get; }
            public double Score { get; }
        }

        private sealed class GapPairDiagnostics
        {
            public int OverlapPairCount { get; set; }
            public int SeparationPairCount { get; set; }
            public int ParallelPairCount { get; set; }
            public int ContrastPairCount { get; set; }
            public double BestDarkContrast { get; set; }
            public double BestDarkCoverageRatio { get; set; }
        }

        private readonly struct GapDarkStatistics
        {
            public GapDarkStatistics(double qualifiedMeanContrast, double coverageRatio, double meanBandGray)
            {
                QualifiedMeanContrast = qualifiedMeanContrast;
                CoverageRatio = coverageRatio;
                MeanBandGray = meanBandGray;
            }

            public double QualifiedMeanContrast { get; }
            public double CoverageRatio { get; }
            public double MeanBandGray { get; }
        }
    }
}
