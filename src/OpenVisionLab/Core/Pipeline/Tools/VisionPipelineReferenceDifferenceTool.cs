using Lib.OpenCV;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineReferenceDifferenceTool : IVisionTool
    {
        private readonly IDictionary<string, string> parameters;

        public VisionPipelineReferenceDifferenceTool(string name, IDictionary<string, string> parameters)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "ReferenceDifference" : name;
            this.parameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (OpenCvHelper.IsImageEmpty(source))
            {
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InputImageInvalid,
                    "ReferenceDifference input image is empty.",
                    stopwatch.Elapsed);
            }

            string[] referencePaths = Enumerable.Range(1, 4)
                .Select(index => GetString("ReferencePath" + index.ToString(CultureInfo.InvariantCulture), string.Empty))
                .Concat(GetString("ReferencePaths", string.Empty)
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(path => path.Trim())
                .Where(path => path.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (referencePaths.Length == 0 || referencePaths.Any(path => !File.Exists(path)))
            {
                string missing = referencePaths.FirstOrDefault(path => !File.Exists(path)) ?? "ReferencePath1";
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidParameter,
                    "ReferenceDifference reference image was not found: " + missing,
                    stopwatch.Elapsed);
            }

            try
            {
                using Mat sourceGray = ToGray(source);
                using ORB orb = ORB.Create(Math.Max(300, GetInt("OrbFeatures", 1600)));
                using Mat sourceDescriptors = new Mat();
                orb.DetectAndCompute(sourceGray, null, out KeyPoint[] sourceKeyPoints, sourceDescriptors);
                if (sourceDescriptors.Empty() || sourceKeyPoints.Length < 8)
                {
                    return VisionToolResult.Failed(
                        VisionToolErrorCode.OpenCvExecutionFailed,
                        "ReferenceDifference could not find enough source features for registration.",
                        stopwatch.Elapsed);
                }

                RegistrationCandidate best = null;
                try
                {
                    for (int index = 0; index < referencePaths.Length; index++)
                    {
                        using Mat reference = Cv2.ImRead(referencePaths[index], ImreadModes.Grayscale);
                        RegistrationCandidate candidate = TryRegister(
                            orb,
                            reference,
                            sourceGray,
                            sourceKeyPoints,
                            sourceDescriptors,
                            index);
                        if (candidate == null)
                        {
                            continue;
                        }

                        if (best == null
                            || candidate.DifferenceMean < best.DifferenceMean
                            || (Math.Abs(candidate.DifferenceMean - best.DifferenceMean) < 0.0001
                                && candidate.Inliers > best.Inliers))
                        {
                            best?.Dispose();
                            best = candidate;
                        }
                        else
                        {
                            candidate.Dispose();
                        }
                    }

                    if (best == null)
                    {
                        return VisionToolResult.Failed(
                            VisionToolErrorCode.OpenCvExecutionFailed,
                            "ReferenceDifference could not register any approved reference.",
                            stopwatch.Elapsed);
                    }

                    using Mat difference = best.Difference.Clone();
                    Cv2.GaussianBlur(difference, difference, new Size(3, 3), 0);
                    using Mat binary = new Mat();
                    Cv2.Threshold(
                        difference,
                        binary,
                        Clamp(GetInt("DifferenceThreshold", 35), 0, 255),
                        255,
                        ThresholdTypes.Binary);
                    Cv2.BitwiseAnd(binary, best.ValidMask, binary);

                    int morphologyKernel = NormalizeOddKernel(GetInt("MorphologyKernel", 3), 1, 31);
                    if (morphologyKernel > 1)
                    {
                        using Mat kernel = Cv2.GetStructuringElement(
                            MorphShapes.Ellipse,
                            new Size(morphologyKernel, morphologyKernel));
                        Cv2.MorphologyEx(binary, binary, MorphTypes.Open, kernel);
                    }

                    Cv2.FindContours(
                        binary,
                        out Point[][] contours,
                        out _,
                        RetrievalModes.External,
                        ContourApproximationModes.ApproxSimple);
                    double minimumArea = Math.Max(1, GetDouble("MinimumDefectArea", 80));
                    double maximumArea = Math.Max(minimumArea, GetDouble("MaximumDefectArea", 20000));
                    List<DetectedRegion> regions = contours
                        .Select(contour => new DetectedRegion(contour, Cv2.ContourArea(contour), Cv2.BoundingRect(contour)))
                        .Where(region => region.Area >= minimumArea && region.Area <= maximumArea)
                        .OrderByDescending(region => region.Area)
                        .ToList();

                    Mat resultImage = ToColor(source);
                    for (int index = 0; index < regions.Count; index++)
                    {
                        DetectedRegion region = regions[index];
                        Cv2.Rectangle(resultImage, region.Bounds, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
                        Cv2.PutText(
                            resultImage,
                            "D" + (index + 1).ToString(CultureInfo.InvariantCulture),
                            new Point(region.Bounds.X, Math.Max(14, region.Bounds.Y - 4)),
                            HersheyFonts.HersheySimplex,
                            0.45,
                            new Scalar(0, 255, 255),
                            1,
                            LineTypes.AntiAlias);
                    }

                    stopwatch.Stop();
                    Dictionary<string, double> metrics = CreateMetrics(
                        source,
                        resultImage,
                        best,
                        binary,
                        regions);
                    return VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics);
                }
                finally
                {
                    best?.Dispose();
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.OpenCvExecutionFailed,
                    "ReferenceDifference failed. " + ex.GetBaseException().Message,
                    stopwatch.Elapsed,
                    ex);
            }
        }

        private RegistrationCandidate TryRegister(
            ORB orb,
            Mat reference,
            Mat source,
            KeyPoint[] sourceKeyPoints,
            Mat sourceDescriptors,
            int referenceIndex)
        {
            if (reference == null || reference.Empty())
            {
                return null;
            }

            using Mat referenceDescriptors = new Mat();
            orb.DetectAndCompute(reference, null, out KeyPoint[] referenceKeyPoints, referenceDescriptors);
            if (referenceDescriptors.Empty() || referenceKeyPoints.Length < 8)
            {
                return null;
            }

            using BFMatcher matcher = new BFMatcher(NormTypes.Hamming, false);
            DMatch[][] pairs = matcher.KnnMatch(referenceDescriptors, sourceDescriptors, 2);
            double ratio = Clamp(GetDouble("MatchRatio", 0.75), 0.4, 0.95);
            List<DMatch> goodMatches = pairs
                .Where(pair => pair != null && pair.Length >= 2 && pair[0].Distance < pair[1].Distance * ratio)
                .Select(pair => pair[0])
                .ToList();
            int minimumInliers = Math.Max(6, GetInt("MinimumInliers", 12));
            if (goodMatches.Count < minimumInliers)
            {
                return null;
            }

            Point2d[] referencePoints = goodMatches
                .Select(match => new Point2d(referenceKeyPoints[match.QueryIdx].Pt.X, referenceKeyPoints[match.QueryIdx].Pt.Y))
                .ToArray();
            Point2d[] sourcePoints = goodMatches
                .Select(match => new Point2d(sourceKeyPoints[match.TrainIdx].Pt.X, sourceKeyPoints[match.TrainIdx].Pt.Y))
                .ToArray();
            using Mat inlierMask = new Mat();
            using Mat homography = Cv2.FindHomography(
                referencePoints,
                sourcePoints,
                HomographyMethods.Ransac,
                Math.Max(0.5, GetDouble("RansacThreshold", 3.0)),
                inlierMask);
            if (homography.Empty())
            {
                return null;
            }

            int inliers = Cv2.CountNonZero(inlierMask);
            if (inliers < minimumInliers)
            {
                return null;
            }

            Mat aligned = new Mat();
            Cv2.WarpPerspective(
                reference,
                aligned,
                homography,
                source.Size(),
                InterpolationFlags.Linear,
                BorderTypes.Reflect101);
            Mat validMask = new Mat(reference.Size(), MatType.CV_8UC1, Scalar.All(255));
            Mat alignedValidMask = new Mat();
            Cv2.WarpPerspective(
                validMask,
                alignedValidMask,
                homography,
                source.Size(),
                InterpolationFlags.Nearest,
                BorderTypes.Constant,
                Scalar.Black);
            validMask.Dispose();

            int ignoreBorder = Clamp(GetInt("IgnoreBorder", 8), 0, 64);
            if (ignoreBorder > 0)
            {
                using Mat borderKernel = Cv2.GetStructuringElement(
                    MorphShapes.Rect,
                    new Size(ignoreBorder * 2 + 1, ignoreBorder * 2 + 1));
                Cv2.Erode(alignedValidMask, alignedValidMask, borderKernel);
            }

            double validPixels = Cv2.CountNonZero(alignedValidMask);
            if (validPixels < source.Width * source.Height * 0.45)
            {
                aligned.Dispose();
                alignedValidMask.Dispose();
                return null;
            }

            Cv2.MeanStdDev(source, out Scalar sourceMean, out Scalar sourceStd, alignedValidMask);
            Cv2.MeanStdDev(aligned, out Scalar referenceMean, out Scalar referenceStd, alignedValidMask);
            double alpha = referenceStd.Val0 > 0.001 ? sourceStd.Val0 / referenceStd.Val0 : 1.0;
            alpha = Clamp(alpha, 0.6, 1.6);
            double beta = sourceMean.Val0 - referenceMean.Val0 * alpha;
            Mat normalized = new Mat();
            aligned.ConvertTo(normalized, MatType.CV_8UC1, alpha, beta);
            aligned.Dispose();

            Mat difference = new Mat();
            Cv2.Absdiff(source, normalized, difference);
            normalized.Dispose();
            double differenceMean = Cv2.Mean(difference, alignedValidMask).Val0;
            return new RegistrationCandidate(
                referenceIndex,
                inliers,
                goodMatches.Count,
                validPixels / (source.Width * source.Height),
                differenceMean,
                difference,
                alignedValidMask);
        }

        private static Dictionary<string, double> CreateMetrics(
            Mat source,
            Mat resultImage,
            RegistrationCandidate registration,
            Mat binary,
            IReadOnlyList<DetectedRegion> regions)
        {
            double validPixelCount = Math.Max(1, Cv2.CountNonZero(registration.ValidMask));
            double differencePixels = Cv2.CountNonZero(binary);
            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = regions.Count,
                [VisionPipelineKnownMetrics.AreaMin] = regions.Count == 0 ? 0 : regions.Min(region => region.Area),
                [VisionPipelineKnownMetrics.AreaMax] = regions.Count == 0 ? 0 : regions.Max(region => region.Area),
                [VisionPipelineKnownMetrics.AreaAvg] = regions.Count == 0 ? 0 : regions.Average(region => region.Area),
                [VisionPipelineKnownMetrics.BoundsWidthMax] = regions.Count == 0 ? 0 : regions.Max(region => region.Bounds.Width),
                [VisionPipelineKnownMetrics.BoundsHeightMax] = regions.Count == 0 ? 0 : regions.Max(region => region.Bounds.Height),
                [VisionPipelineKnownMetrics.DifferencePixelCount] = differencePixels,
                [VisionPipelineKnownMetrics.DifferencePixelRatio] = differencePixels / validPixelCount,
                [VisionPipelineKnownMetrics.DifferenceMean] = registration.DifferenceMean,
                [VisionPipelineKnownMetrics.RegistrationInliers] = registration.Inliers,
                [VisionPipelineKnownMetrics.RegistrationInlierRatio] = registration.MatchCount <= 0 ? 0 : (double)registration.Inliers / registration.MatchCount,
                [VisionPipelineKnownMetrics.RegistrationScore] = Math.Max(0, 100.0 - registration.DifferenceMean),
                [VisionPipelineKnownMetrics.ReferenceIndex] = registration.ReferenceIndex,
                [VisionPipelineKnownMetrics.ValidPixelRatio] = registration.ValidPixelRatio,
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = resultImage.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = resultImage.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = resultImage.Channels()
            };
            return metrics;
        }

        private static Mat ToGray(Mat source)
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

        private static Mat ToColor(Mat source)
        {
            if (source.Channels() == 3)
            {
                return source.Clone();
            }

            Mat color = new Mat();
            Cv2.CvtColor(
                source,
                color,
                source.Channels() == 4 ? ColorConversionCodes.BGRA2BGR : ColorConversionCodes.GRAY2BGR);
            return color;
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

        private double GetDouble(string key, double defaultValue)
        {
            return double.TryParse(GetString(key, string.Empty), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                ? value
                : defaultValue;
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

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private sealed class DetectedRegion
        {
            public DetectedRegion(Point[] contour, double area, Rect bounds)
            {
                Contour = contour;
                Area = area;
                Bounds = bounds;
            }

            public Point[] Contour { get; }

            public double Area { get; }

            public Rect Bounds { get; }
        }

        private sealed class RegistrationCandidate : IDisposable
        {
            public RegistrationCandidate(
                int referenceIndex,
                int inliers,
                int matchCount,
                double validPixelRatio,
                double differenceMean,
                Mat difference,
                Mat validMask)
            {
                ReferenceIndex = referenceIndex;
                Inliers = inliers;
                MatchCount = matchCount;
                ValidPixelRatio = validPixelRatio;
                DifferenceMean = differenceMean;
                Difference = difference;
                ValidMask = validMask;
            }

            public int ReferenceIndex { get; }

            public int Inliers { get; }

            public int MatchCount { get; }

            public double ValidPixelRatio { get; }

            public double DifferenceMean { get; }

            public Mat Difference { get; }

            public Mat ValidMask { get; }

            public void Dispose()
            {
                Difference?.Dispose();
                ValidMask?.Dispose();
            }
        }
    }
}
