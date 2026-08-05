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
    /// Locates the lower-right corner of the largest bright object without fixed image coordinates.
    /// </summary>
    internal sealed class VisionPipelineOuterCornerIntersectionTool : IVisionTool
    {
        private readonly IDictionary<string, string> parameters;

        public VisionPipelineOuterCornerIntersectionTool(string name, IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "OuterCornerIntersection" : name;
            this.parameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (source == null || source.Empty())
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(VisionToolErrorCode.InputImageInvalid, "OuterCornerIntersection input image is empty.", stopwatch.Elapsed);
            }

            int foregroundThreshold = Math.Clamp(GetInt("ForegroundThreshold", 45), 1, 254);
            double minimumArea = Math.Max(1, GetDouble("MinComponentArea", 10000));
            double edgeFitEndPercent = Math.Clamp(GetDouble("EdgeFitEndPercent", 0.80), 0.55, 0.95);
            int sampleStep = Math.Max(1, GetInt("SampleStep", 4));

            using Mat gray = CreateGray(source);
            using Mat mask = new Mat();
            using Mat closed = new Mat();
            using Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(5, 5));
            Cv2.Threshold(gray, mask, foregroundThreshold, 255, ThresholdTypes.Binary);
            Cv2.MorphologyEx(mask, closed, MorphTypes.Close, kernel);
            Cv2.FindContours(closed, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // ponytail: reject frame-touching background before choosing the card; add a taught ROI only if this family needs multiple parts.
            OpenCvSharp.Point[] contour = contours
                .Where(candidate => Cv2.ContourArea(candidate) >= minimumArea)
                .OrderBy(candidate =>
                {
                    Rect candidateBounds = Cv2.BoundingRect(candidate);
                    return candidateBounds.Right >= source.Width || candidateBounds.Bottom >= source.Height ? 1 : 0;
                })
                .ThenByDescending(candidate => Cv2.ContourArea(candidate))
                .FirstOrDefault();
            Rect bounds = contour == null ? default : Cv2.BoundingRect(contour);
            List<OpenCvSharp.Point> bottomPoints = new List<OpenCvSharp.Point>();
            List<OpenCvSharp.Point> rightPoints = new List<OpenCvSharp.Point>();
            FittedLine bottomLine = default;
            FittedLine rightLine = default;
            Point2d intersection = default;
            bool usedHoughFallback = false;
            bool usedProjectionFallback = false;
            bool usedOuterContourFit = false;
            bool brightComponentFits = contour != null && bounds.Right < source.Width && bounds.Bottom < source.Height;
            if (brightComponentFits)
            {
                brightComponentFits = TryFindOuterContourTangentCorner(
                    contour,
                    source,
                    out bottomPoints,
                    out rightPoints,
                    out bottomLine,
                    out rightLine,
                    out intersection);
                usedOuterContourFit = brightComponentFits;
                if (!brightComponentFits)
                {
                    using Mat componentMask = Mat.Zeros(source.Rows, source.Cols, MatType.CV_8UC1);
                    Cv2.DrawContours(componentMask, new[] { contour }, -1, Scalar.White, -1);
                    bottomPoints = FindBottomEdgePoints(componentMask, bounds, edgeFitEndPercent, sampleStep);
                    rightPoints = FindRightEdgePoints(componentMask, bounds, edgeFitEndPercent, sampleStep);
                    brightComponentFits = bottomPoints.Count >= 2
                        && rightPoints.Count >= 2
                        && TryFitLine(bottomPoints, out bottomLine)
                        && TryFitLine(rightPoints, out rightLine)
                        && TryFindIntersection(bottomLine, rightLine, out intersection)
                        && IsInsideImage(intersection, source);
                }
            }

            if (!brightComponentFits)
            {
                OpenCvSharp.Point[] fallbackContour = FindDetachedBrightContour(gray, foregroundThreshold, minimumArea);
                Rect? fallbackBounds = fallbackContour == null ? null : Cv2.BoundingRect(fallbackContour);
                bool houghFound = TryFindHoughCorner(gray, source, fallbackContour, out bottomLine, out rightLine, out intersection, out string houghDiagnostic);
                bool raisedContourFound = false;
                bool projectionFound = false;
                string projectionDiagnostic = string.Empty;
                if (!houghFound)
                {
                    raisedContourFound = fallbackContour != null
                        && TryFindOuterContourTangentCorner(
                            fallbackContour,
                            source,
                            out bottomPoints,
                            out rightPoints,
                            out bottomLine,
                            out rightLine,
                            out intersection);
                    if (raisedContourFound)
                    {
                        contour = fallbackContour;
                        bounds = fallbackBounds!.Value;
                        usedOuterContourFit = true;
                    }
                    else
                    {
                        projectionFound = TryFindProjectionCorner(gray, source, fallbackContour, out bottomLine, out rightLine, out intersection, out projectionDiagnostic);
                    }
                }

                if (!houghFound && !raisedContourFound && !projectionFound)
                {
                    stopwatch.Stop();
                    return VisionToolResult.Failed(
                        VisionToolErrorCode.LineGaugeEdgeNotFound,
                        "OuterCornerIntersection could not isolate a visible lower/right outer corner. " + projectionDiagnostic + " " + houghDiagnostic + " Tune ForegroundThreshold, MinComponentArea, or EdgeFitEndPercent; a corner cropped beyond the image is intentionally not measured.",
                        stopwatch.Elapsed);
                }

                if (!raisedContourFound)
                {
                    usedHoughFallback = houghFound;
                    usedProjectionFallback = projectionFound;
                    bottomPoints = new List<OpenCvSharp.Point> { ToPoint(bottomLine.Start), ToPoint(bottomLine.End) };
                    rightPoints = new List<OpenCvSharp.Point> { ToPoint(rightLine.Start), ToPoint(rightLine.End) };
                    contour = null;
                    bounds = default;
                }
            }

            Mat resultImage = CreateColorImage(source);
            DrawResult(resultImage, contour, bounds, bottomPoints, rightPoints, bottomLine, rightLine, intersection, usedHoughFallback, usedProjectionFallback, usedOuterContourFit);
            Dictionary<string, double> metrics = CreateMetrics(source, resultImage, bounds, bottomPoints, rightPoints, bottomLine, rightLine, intersection, usedOuterContourFit);
            List<VisionToolOverlay> overlays = CreateOverlays(bounds, bottomLine, rightLine, intersection);

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

        private static List<OpenCvSharp.Point> FindBottomEdgePoints(Mat componentMask, Rect bounds, double endPercent, int sampleStep)
        {
            int startX = bounds.X + (int)Math.Round(bounds.Width * 0.05);
            int endX = bounds.X + (int)Math.Round(bounds.Width * endPercent);
            List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();
            for (int x = Math.Max(bounds.X, startX); x < Math.Min(bounds.Right, endX); x += sampleStep)
            {
                for (int y = bounds.Bottom - 1; y >= bounds.Y; y--)
                {
                    if (componentMask.At<byte>(y, x) != 0)
                    {
                        points.Add(new OpenCvSharp.Point(x, y));
                        break;
                    }
                }
            }

            return points;
        }

        private static List<OpenCvSharp.Point> FindRightEdgePoints(Mat componentMask, Rect bounds, double endPercent, int sampleStep)
        {
            int startY = bounds.Y + (int)Math.Round(bounds.Height * 0.05);
            int endY = bounds.Y + (int)Math.Round(bounds.Height * endPercent);
            List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();
            for (int y = Math.Max(bounds.Y, startY); y < Math.Min(bounds.Bottom, endY); y += sampleStep)
            {
                for (int x = bounds.Right - 1; x >= bounds.X; x--)
                {
                    if (componentMask.At<byte>(y, x) != 0)
                    {
                        points.Add(new OpenCvSharp.Point(x, y));
                        break;
                    }
                }
            }

            return points;
        }

        private static bool TryFitLine(IReadOnlyList<OpenCvSharp.Point> points, out FittedLine fitted)
        {
            fitted = default;
            if (points == null || points.Count < 2)
            {
                return false;
            }

            Line2D line = Cv2.FitLine(points.ToArray(), DistanceTypes.L2, 0, 0.01, 0.01);
            double vx = line.Vx;
            double vy = line.Vy;
            if (Math.Abs(vx) < 0.000001 && Math.Abs(vy) < 0.000001)
            {
                return false;
            }

            Point2d origin = new Point2d(line.X1, line.Y1);
            double minimumProjection = points.Min(point => ((point.X - origin.X) * vx) + ((point.Y - origin.Y) * vy));
            double maximumProjection = points.Max(point => ((point.X - origin.X) * vx) + ((point.Y - origin.Y) * vy));
            fitted = new FittedLine(
                new Point2d(origin.X + vx * minimumProjection, origin.Y + vy * minimumProjection),
                new Point2d(origin.X + vx * maximumProjection, origin.Y + vy * maximumProjection));
            return true;
        }

        private static bool TryFindIntersection(FittedLine first, FittedLine second, out Point2d intersection)
        {
            intersection = default;
            double a1 = first.End.Y - first.Start.Y;
            double b1 = first.Start.X - first.End.X;
            double c1 = (a1 * first.Start.X) + (b1 * first.Start.Y);
            double a2 = second.End.Y - second.Start.Y;
            double b2 = second.Start.X - second.End.X;
            double c2 = (a2 * second.Start.X) + (b2 * second.Start.Y);
            double determinant = (a1 * b2) - (a2 * b1);
            if (Math.Abs(determinant) < 0.000001)
            {
                return false;
            }

            intersection = new Point2d(
                ((b2 * c1) - (b1 * c2)) / determinant,
                ((a1 * c2) - (a2 * c1)) / determinant);
            return true;
        }

        private static bool IsInsideImage(Point2d point, Mat source)
        {
            return point.X >= 0 && point.X < source.Width && point.Y >= 0 && point.Y < source.Height;
        }

        private static bool TryFindOuterContourTangentCorner(
            OpenCvSharp.Point[] contour,
            Mat source,
            out List<OpenCvSharp.Point> bottomPoints,
            out List<OpenCvSharp.Point> rightPoints,
            out FittedLine bottomLine,
            out FittedLine rightLine,
            out Point2d intersection)
        {
            bottomPoints = new List<OpenCvSharp.Point>();
            rightPoints = new List<OpenCvSharp.Point>();
            bottomLine = default;
            rightLine = default;
            intersection = default;
            if (contour == null || contour.Length < 4)
            {
                return false;
            }

            Point2f[] vertices = Cv2.MinAreaRect(contour).Points();
            if (vertices == null || vertices.Length != 4)
            {
                return false;
            }

            int cornerIndex = Enumerable.Range(0, vertices.Length)
                .OrderByDescending(index => vertices[index].X + vertices[index].Y)
                .First();
            Point2d corner = new Point2d(vertices[cornerIndex].X, vertices[cornerIndex].Y);
            Point2d center = new Point2d(vertices.Average(vertex => vertex.X), vertices.Average(vertex => vertex.Y));
            Point2d[] adjacent = Enumerable.Range(0, vertices.Length)
                .Where(index => index != cornerIndex)
                .Select(index => new Point2d(vertices[index].X, vertices[index].Y))
                .OrderBy(point => DistanceSquared(point, corner))
                .Take(2)
                .ToArray();
            if (adjacent.Length != 2)
            {
                return false;
            }

            FittedLine first = new FittedLine(adjacent[0], corner);
            FittedLine second = new FittedLine(adjacent[1], corner);
            if (first.Length < source.Width * 0.15D || second.Length < source.Height * 0.15D)
            {
                return false;
            }

            bool firstHorizontal = IsMostlyHorizontal(first.Angle);
            bool secondHorizontal = IsMostlyHorizontal(second.Angle);
            bool firstVertical = IsMostlyVertical(first.Angle);
            bool secondVertical = IsMostlyVertical(second.Angle);
            if ((firstHorizontal && secondVertical) || (secondHorizontal && firstVertical))
            {
                FittedLine bottomReference = firstHorizontal ? first : second;
                FittedLine rightReference = firstVertical ? first : second;
                Point2d bottomNormal = GetOutwardNormal(bottomReference, center, corner);
                Point2d rightNormal = GetOutwardNormal(rightReference, center, corner);
                double bottomMaximum = contour.Max(point => Dot(point, bottomNormal));
                double rightMaximum = contour.Max(point => Dot(point, rightNormal));
                double edgeBand = Math.Max(4D, Math.Min(bottomReference.Length, rightReference.Length) * 0.025D);

                // ponytail: use actual contour support, not the enclosing rectangle vertex; add taught ROI only if one image contains multiple bright cards.
                bottomPoints = contour
                    .Where(point => Dot(point, bottomNormal) >= bottomMaximum - edgeBand
                        && Dot(point, rightNormal) < rightMaximum - edgeBand)
                    .ToList();
                rightPoints = contour
                    .Where(point => Dot(point, rightNormal) >= rightMaximum - edgeBand
                        && Dot(point, bottomNormal) < bottomMaximum - edgeBand)
                    .ToList();
                return bottomPoints.Count >= 8
                    && rightPoints.Count >= 8
                    && TryFitLine(bottomPoints, out bottomLine)
                    && TryFitLine(rightPoints, out rightLine)
                    && IsMostlyHorizontal(bottomLine.Angle)
                    && IsMostlyVertical(rightLine.Angle)
                    && TryFindIntersection(bottomLine, rightLine, out intersection)
                    && IsPlausibleBottomRight(intersection, source);
            }

            return false;
        }

        private static Point2d GetOutwardNormal(FittedLine line, Point2d center, Point2d corner)
        {
            double length = line.Length;
            if (length < 0.000001D)
            {
                return default;
            }

            Point2d normal = new Point2d(-(line.End.Y - line.Start.Y) / length, (line.End.X - line.Start.X) / length);
            Point2d towardCorner = new Point2d(corner.X - center.X, corner.Y - center.Y);
            return (normal.X * towardCorner.X) + (normal.Y * towardCorner.Y) < 0D
                ? new Point2d(-normal.X, -normal.Y)
                : normal;
        }

        private static double Dot(OpenCvSharp.Point point, Point2d axis) => (point.X * axis.X) + (point.Y * axis.Y);

        private static bool IsMostlyHorizontal(double angle)
        {
            double absolute = Math.Abs(angle);
            return absolute <= 15D || absolute >= 165D;
        }

        private static bool IsMostlyVertical(double angle) => Math.Abs(Math.Abs(angle) - 90D) <= 15D;

        private static OpenCvSharp.Point[] FindDetachedBrightContour(Mat gray, int foregroundThreshold, double minimumArea)
        {
            const int thresholdStep = 15;
            for (int threshold = foregroundThreshold; threshold <= 210; threshold += thresholdStep)
            {
                using Mat mask = new Mat();
                using Mat closed = new Mat();
                using Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(5, 5));
                Cv2.Threshold(gray, mask, threshold, 255, ThresholdTypes.Binary);
                Cv2.MorphologyEx(mask, closed, MorphTypes.Close, kernel);
                Cv2.FindContours(closed, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                OpenCvSharp.Point[] candidate = contours
                    .Where(value => Cv2.ContourArea(value) >= minimumArea)
                    .Where(value =>
                    {
                        Rect candidateBounds = Cv2.BoundingRect(value);
                        return candidateBounds.Right < gray.Width && candidateBounds.Bottom < gray.Height;
                    })
                    .OrderByDescending(value => Cv2.ContourArea(value))
                    .FirstOrDefault();
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return null;
        }

        private static bool TryFindProjectionCorner(Mat gray, Mat source, OpenCvSharp.Point[] cardContour, out FittedLine bottomLine, out FittedLine rightLine, out Point2d intersection, out string diagnostic)
        {
            const int edgeContrast = 18;
            const int sampleStep = 4;
            bottomLine = default;
            rightLine = default;
            intersection = default;
            diagnostic = string.Empty;
            List<OpenCvSharp.Point> bottomPoints = new List<OpenCvSharp.Point>();
            List<OpenCvSharp.Point> rightPoints = new List<OpenCvSharp.Point>();

            for (int x = 0; x < source.Width * 0.90D; x += sampleStep)
            {
                for (int y = source.Height - 1; y >= 1; y--)
                {
                    if (gray.At<byte>(y - 1, x) - gray.At<byte>(y, x) >= edgeContrast)
                    {
                        bottomPoints.Add(new OpenCvSharp.Point(x, y));
                        break;
                    }
                }
            }

            for (int y = 0; y < source.Height * 0.90D; y += sampleStep)
            {
                for (int x = source.Width - 1; x >= 1; x--)
                {
                    if (gray.At<byte>(y, x - 1) - gray.At<byte>(y, x) >= edgeContrast)
                    {
                        rightPoints.Add(new OpenCvSharp.Point(x, y));
                        break;
                    }
                }
            }

            bottomPoints = KeepDominantCoordinates(bottomPoints, point => point.Y, 20);
            rightPoints = KeepDominantCoordinates(rightPoints, point => point.X, 28);
            bool fitted = bottomPoints.Count >= 8
                && rightPoints.Count >= 8
                && TryFitLine(bottomPoints, out bottomLine)
                && TryFitLine(rightPoints, out rightLine)
                && TryFindIntersection(bottomLine, rightLine, out intersection);
            diagnostic = fitted
                ? $"Projection candidates: bottom={bottomPoints.Count}, right={rightPoints.Count}, angle={bottomLine.Angle:0.##}/{rightLine.Angle:0.##}, corner={intersection.X:0.##},{intersection.Y:0.##}."
                : $"Projection candidates: bottom={bottomPoints.Count}, right={rightPoints.Count}.";
            return fitted
                && Math.Abs(bottomLine.Angle) <= 10D
                && Math.Abs(Math.Abs(rightLine.Angle) - 90D) <= 12D
                && IsSupportedByCardContour(intersection, cardContour, source)
                && IsPlausibleBottomRight(intersection, source);
        }

        private static bool IsPlausibleBottomRight(Point2d point, Mat source)
        {
            return IsInsideImage(point, source)
                && point.X >= source.Width * 0.45D
                && point.X < source.Width - 3D
                && point.Y >= source.Height * 0.35D
                && point.Y < source.Height - 6D;
        }

        private static List<OpenCvSharp.Point> KeepDominantCoordinates(
            IReadOnlyList<OpenCvSharp.Point> points,
            Func<OpenCvSharp.Point, int> selector,
            int allowedDifference)
        {
            if (points == null || points.Count == 0)
            {
                return new List<OpenCvSharp.Point>();
            }

            int median = points.Select(selector).OrderBy(value => value).ElementAt(points.Count / 2);
            return points.Where(point => Math.Abs(selector(point) - median) <= allowedDifference).ToList();
        }

        private static bool TryFindHoughCorner(Mat gray, Mat source, OpenCvSharp.Point[] cardContour, out FittedLine bottomLine, out FittedLine rightLine, out Point2d intersection, out string diagnostic)
        {
            bottomLine = default;
            rightLine = default;
            intersection = default;
            diagnostic = string.Empty;
            using Mat edges = new Mat();
            Cv2.Canny(gray, edges, 10, 60);
            int minimumLength = Math.Max(55, source.Width / 10);
            LineSegmentPoint[] lines = Cv2.HoughLinesP(edges, 1, Math.PI / 180D, 25, minimumLength, 48);
            List<FittedLine> horizontal = new List<FittedLine>();
            List<FittedLine> vertical = new List<FittedLine>();
            foreach (LineSegmentPoint line in lines ?? Array.Empty<LineSegmentPoint>())
            {
                FittedLine candidate = new FittedLine(line.P1, line.P2);
                double angle = Math.Abs(candidate.Angle);
                double averageX = (candidate.Start.X + candidate.End.X) / 2D;
                double averageY = (candidate.Start.Y + candidate.End.Y) / 2D;
                if ((angle <= 12D || angle >= 168D) && averageY > source.Height * 0.25D && averageY < source.Height - 6)
                {
                    horizontal.Add(candidate);
                }
                else if (Math.Abs(angle - 90D) <= 12D && averageX > source.Width * 0.30D && averageX < source.Width - 6)
                {
                    vertical.Add(candidate);
                }
            }

            double bestScore = double.NegativeInfinity;
            foreach (FittedLine horizontalCandidate in horizontal)
            {
                foreach (FittedLine verticalCandidate in vertical)
                {
                    if (!TryFindIntersection(horizontalCandidate, verticalCandidate, out Point2d candidateIntersection)
                        || !IsSupportedByCardContour(candidateIntersection, cardContour, source)
                        || !IsLowerContourBoundary(horizontalCandidate, cardContour, source)
                        || !IsPlausibleBottomRight(candidateIntersection, source)
                        || DistanceToSegment(candidateIntersection, horizontalCandidate) > 150D
                        || DistanceToSegment(candidateIntersection, verticalCandidate) > 150D)
                    {
                        continue;
                    }

                    double score = candidateIntersection.X / source.Width
                        + candidateIntersection.Y / source.Height
                        + Math.Min(horizontalCandidate.Length, verticalCandidate.Length) / Math.Max(source.Width, source.Height);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bottomLine = horizontalCandidate;
                        rightLine = verticalCandidate;
                        intersection = candidateIntersection;
                    }
                }
            }

            diagnostic = $"Hough candidates: all={lines?.Length ?? 0}, horizontal={horizontal.Count}, vertical={vertical.Count}.";
            return bestScore > double.NegativeInfinity;
        }

        private static bool IsSupportedByCardContour(Point2d intersection, OpenCvSharp.Point[] cardContour, Mat source)
        {
            if (cardContour == null || cardContour.Length == 0)
            {
                return true;
            }

            Rect bounds = Cv2.BoundingRect(cardContour);
            double rightSlack = Math.Max(12D, source.Width * 0.04D);
            double bottomSlack = Math.Max(12D, bounds.Height * 0.12D);
            double boundaryDistance = Math.Abs(Cv2.PointPolygonTest(cardContour, new Point2f((float)intersection.X, (float)intersection.Y), true));
            return intersection.X >= bounds.X + (bounds.Width * 0.70D)
                && intersection.X <= bounds.Right + rightSlack
                && intersection.Y >= bounds.Y + (bounds.Height * 0.45D)
                && intersection.Y <= bounds.Bottom + bottomSlack
                && boundaryDistance <= Math.Max(12D, Math.Min(bounds.Width, bounds.Height) * 0.18D);
        }

        private static bool IsLowerContourBoundary(FittedLine candidate, OpenCvSharp.Point[] cardContour, Mat source)
        {
            if (cardContour == null || cardContour.Length == 0)
            {
                return true;
            }

            const int sampleCount = 5;
            double offset = Math.Max(6D, Math.Min(source.Width, source.Height) * 0.025D);
            int supported = 0;
            int checkedSamples = 0;
            for (int index = 1; index <= sampleCount; index++)
            {
                double factor = index / (double)(sampleCount + 1);
                double x = candidate.Start.X + ((candidate.End.X - candidate.Start.X) * factor);
                double y = candidate.Start.Y + ((candidate.End.Y - candidate.Start.Y) * factor);
                if (x < 0D || x >= source.Width || y - offset < 0D || y + offset >= source.Height)
                {
                    continue;
                }

                checkedSamples++;
                double insideAbove = Cv2.PointPolygonTest(cardContour, new Point2f((float)x, (float)(y - offset)), true);
                double outsideBelow = Cv2.PointPolygonTest(cardContour, new Point2f((float)x, (float)(y + offset)), true);
                if (insideAbove >= 0D && outsideBelow < 0D)
                {
                    supported++;
                }
            }

            return checkedSamples > 0 && supported * 2 >= checkedSamples;
        }

        private static double DistanceToSegment(Point2d point, FittedLine line)
        {
            double dx = line.End.X - line.Start.X;
            double dy = line.End.Y - line.Start.Y;
            double lengthSquared = (dx * dx) + (dy * dy);
            if (lengthSquared < 0.000001D)
            {
                return Math.Sqrt(((point.X - line.Start.X) * (point.X - line.Start.X)) + ((point.Y - line.Start.Y) * (point.Y - line.Start.Y)));
            }

            double factor = Math.Max(0D, Math.Min(1D, (((point.X - line.Start.X) * dx) + ((point.Y - line.Start.Y) * dy)) / lengthSquared));
            double closestX = line.Start.X + factor * dx;
            double closestY = line.Start.Y + factor * dy;
            return Math.Sqrt(((point.X - closestX) * (point.X - closestX)) + ((point.Y - closestY) * (point.Y - closestY)));
        }

        private static void DrawResult(
            Mat image,
            OpenCvSharp.Point[] contour,
            Rect bounds,
            IReadOnlyList<OpenCvSharp.Point> bottomPoints,
            IReadOnlyList<OpenCvSharp.Point> rightPoints,
            FittedLine bottomLine,
            FittedLine rightLine,
            Point2d intersection,
            bool usedHoughFallback,
            bool usedProjectionFallback,
            bool usedOuterContourFit)
        {
            if (contour != null)
            {
                Cv2.DrawContours(image, new[] { contour }, -1, new Scalar(0, 220, 255), 1, LineTypes.AntiAlias);
                Cv2.Rectangle(image, bounds, new Scalar(255, 80, 220), 1, LineTypes.AntiAlias);
            }
            foreach (OpenCvSharp.Point point in bottomPoints)
            {
                Cv2.Circle(image, point, 1, new Scalar(0, 220, 80), -1, LineTypes.AntiAlias);
            }

            foreach (OpenCvSharp.Point point in rightPoints)
            {
                Cv2.Circle(image, point, 1, new Scalar(255, 180, 0), -1, LineTypes.AntiAlias);
            }

            OpenCvSharp.Point corner = ToPoint(intersection);
            Cv2.Line(image, ToPoint(GetFarthestEndpoint(bottomLine, intersection)), corner, new Scalar(0, 0, 255), 3, LineTypes.AntiAlias);
            Cv2.Line(image, ToPoint(GetFarthestEndpoint(rightLine, intersection)), corner, new Scalar(0, 0, 255), 3, LineTypes.AntiAlias);
            Cv2.Circle(image, corner, 7, new Scalar(0, 255, 0), 2, LineTypes.AntiAlias);
            Cv2.Line(image, new OpenCvSharp.Point(corner.X - 11, corner.Y), new OpenCvSharp.Point(corner.X + 11, corner.Y), new Scalar(0, 255, 0), 2, LineTypes.AntiAlias);
            Cv2.Line(image, new OpenCvSharp.Point(corner.X, corner.Y - 11), new OpenCvSharp.Point(corner.X, corner.Y + 11), new Scalar(0, 255, 0), 2, LineTypes.AntiAlias);
            string fitKind = usedOuterContourFit ? "outer" : usedHoughFallback ? "hough" : usedProjectionFallback ? "projection" : "profile";
            string label = $"Virtual corner {intersection.X:0.#}, {intersection.Y:0.#} ({fitKind})";
            Cv2.PutText(image, label, new OpenCvSharp.Point(Math.Max(4, bounds.X), Math.Max(16, bounds.Y + 16)), HersheyFonts.HersheySimplex, 0.45, new Scalar(0, 255, 255), 1, LineTypes.AntiAlias);
        }

        private static Dictionary<string, double> CreateMetrics(
            Mat source,
            Mat resultImage,
            Rect bounds,
            IReadOnlyList<OpenCvSharp.Point> bottomPoints,
            IReadOnlyList<OpenCvSharp.Point> rightPoints,
            FittedLine bottomLine,
            FittedLine rightLine,
            Point2d intersection,
            bool usedOuterContourFit)
        {
            double bottomLength = bottomLine.Length;
            double rightLength = rightLine.Length;
            double bottomAngle = bottomLine.Angle;
            double rightAngle = rightLine.Angle;
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = 1,
                [VisionPipelineKnownMetrics.EdgeCount] = 2,
                [VisionPipelineKnownMetrics.EdgePointCount] = bottomPoints.Count + rightPoints.Count,
                [VisionPipelineKnownMetrics.LineLengthMin] = Math.Min(bottomLength, rightLength),
                [VisionPipelineKnownMetrics.LineLengthMax] = Math.Max(bottomLength, rightLength),
                [VisionPipelineKnownMetrics.LineLengthAvg] = (bottomLength + rightLength) / 2D,
                [VisionPipelineKnownMetrics.LineAngleMin] = Math.Min(bottomAngle, rightAngle),
                [VisionPipelineKnownMetrics.LineAngleMax] = Math.Max(bottomAngle, rightAngle),
                [VisionPipelineKnownMetrics.LineAngleAvg] = (bottomAngle + rightAngle) / 2D,
                ["IntersectionCross"] = 1,
                [VisionPipelineKnownMetrics.CornerOuterContourVerified] = usedOuterContourFit ? 1D : 0D,
                [VisionPipelineKnownMetrics.IntersectionX] = intersection.X,
                [VisionPipelineKnownMetrics.IntersectionY] = intersection.Y,
                [VisionPipelineKnownMetrics.BoundsWidthMin] = bounds.Width,
                [VisionPipelineKnownMetrics.BoundsWidthMax] = bounds.Width,
                [VisionPipelineKnownMetrics.BoundsWidthAvg] = bounds.Width,
                [VisionPipelineKnownMetrics.BoundsHeightMin] = bounds.Height,
                [VisionPipelineKnownMetrics.BoundsHeightMax] = bounds.Height,
                [VisionPipelineKnownMetrics.BoundsHeightAvg] = bounds.Height,
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = resultImage.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = resultImage.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = resultImage.Channels()
            };
        }

        private static List<VisionToolOverlay> CreateOverlays(Rect bounds, FittedLine bottomLine, FittedLine rightLine, Point2d intersection)
        {
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Rectangle,
                    Label = "Bright object",
                    Bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                    Center = new PointF(bounds.X + bounds.Width / 2F, bounds.Y + bounds.Height / 2F)
                });
            }

            overlays.AddRange(new[]
            {
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = "Bottom outer tangent",
                    Start = ToPointF(GetFarthestEndpoint(bottomLine, intersection)),
                    End = ToPointF(intersection),
                    Center = ToPointF(GetMidpoint(GetFarthestEndpoint(bottomLine, intersection), intersection))
                },
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = "Right outer tangent",
                    Start = ToPointF(GetFarthestEndpoint(rightLine, intersection)),
                    End = ToPointF(intersection),
                    Center = ToPointF(GetMidpoint(GetFarthestEndpoint(rightLine, intersection), intersection))
                },
                new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Point,
                    Label = "Virtual bottom-right corner",
                    Center = ToPointF(intersection)
                }
            });
            return overlays;
        }

        private static OpenCvSharp.Point ToPoint(Point2d point) => new OpenCvSharp.Point((int)Math.Round(point.X), (int)Math.Round(point.Y));

        private static PointF ToPointF(Point2d point) => new PointF((float)point.X, (float)point.Y);

        private static Point2d GetFarthestEndpoint(FittedLine line, Point2d point)
        {
            return DistanceSquared(line.Start, point) >= DistanceSquared(line.End, point) ? line.Start : line.End;
        }

        private static Point2d GetMidpoint(Point2d first, Point2d second) => new Point2d((first.X + second.X) / 2D, (first.Y + second.Y) / 2D);

        private static double DistanceSquared(Point2d first, Point2d second)
        {
            double x = first.X - second.X;
            double y = first.Y - second.Y;
            return (x * x) + (y * y);
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

        private int GetInt(string key, int defaultValue) => int.TryParse(GetString(key, string.Empty), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : defaultValue;

        private double GetDouble(string key, double defaultValue) => double.TryParse(GetString(key, string.Empty), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : defaultValue;

        private readonly struct FittedLine
        {
            public FittedLine(Point2d start, Point2d end)
            {
                Start = start;
                End = end;
            }

            public Point2d Start { get; }
            public Point2d End { get; }
            public Point2d Center => new Point2d((Start.X + End.X) / 2D, (Start.Y + End.Y) / 2D);
            public double Length => Math.Sqrt(((End.X - Start.X) * (End.X - Start.X)) + ((End.Y - Start.Y) * (End.Y - Start.Y)));
            public double Angle => Math.Atan2(End.Y - Start.Y, End.X - Start.X) * 180D / Math.PI;
        }
    }
}
