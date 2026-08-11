using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Blob;
using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolPreviewOverlayRenderer
    {
        public static Mat CreateThresholdTeachingPreviewImage(Mat source, OpenCvPropertyBase property)
        {
            return CreateInternalThresholdPreviewImage(source, property);
        }

        public static Mat CreateBlobResultPreviewImage(Mat source, IEnumerable<BlobResult> blobs)
        {
            Mat visual = CreateColorCanvas(source);
            if (visual == null || visual.Empty())
            {
                return visual;
            }

            if (visual.Channels() == 1)
            {
                Mat colorImage = new Mat();
                Cv2.CvtColor(visual, colorImage, ColorConversionCodes.GRAY2BGR);
                visual.Dispose();
                visual = colorImage;
            }

            foreach (BlobResult blob in blobs ?? Enumerable.Empty<BlobResult>())
            {
                Rectangle bounds = blob.Bounding;
                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    continue;
                }

                OpenCvSharp.Rect box = ClampRect(
                    new OpenCvSharp.Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                    visual.Width,
                    visual.Height);
                if (box.Width <= 0 || box.Height <= 0)
                {
                    continue;
                }

                Cv2.Rectangle(visual, box, new Scalar(0, 230, 255), 2, LineTypes.AntiAlias);
                Cv2.DrawMarker(
                    visual,
                    new OpenCvSharp.Point((int)Math.Round(blob.Center.X), (int)Math.Round(blob.Center.Y)),
                    new Scalar(0, 230, 255),
                    MarkerTypes.Cross,
                    14,
                    2,
                    LineTypes.AntiAlias);
            }

            return visual;
        }

        public static Mat CreateContourResultPreviewImage(Mat source, OpenCvPropertyBase property, IEnumerable<ContourResult> contours)
        {
            Mat visual = CreateColorCanvas(source);
            if (visual == null || visual.Empty())
            {
                return visual;
            }

            if (visual.Channels() == 1)
            {
                Mat colorImage = new Mat();
                Cv2.CvtColor(visual, colorImage, ColorConversionCodes.GRAY2BGR);
                visual.Dispose();
                visual = colorImage;
            }

            ContourProperty contourProperty = property as ContourProperty;
            Scalar drawColor = ToBgrScalar(contourProperty?.DrawColor ?? Color.FromArgb(255, 180, 0));
            int thickness = Math.Max(1, contourProperty?.DrawThickness ?? 2);
            foreach (ContourResult contour in contours ?? Enumerable.Empty<ContourResult>())
            {
                DrawContourGeometry(visual, contour, contourProperty, drawColor, thickness);
            }

            return visual;
        }

        public static void DrawContourOverlay(VisionToolResult result, IEnumerable<ContourResult> contours, ContourProperty property)
        {
            if (result?.ResultImage == null || result.ResultImage.Empty())
            {
                return;
            }

            Mat resultImage = result.ResultImage;
            if (resultImage.Channels() == 1)
            {
                Mat colorImage = new Mat();
                Cv2.CvtColor(resultImage, colorImage, ColorConversionCodes.GRAY2BGR);
                result.ResultImage = colorImage;
                resultImage.Dispose();
                resultImage = colorImage;
            }

            Scalar drawColor = ToBgrScalar(property?.DrawColor ?? Color.FromArgb(255, 180, 0));
            int thickness = Math.Max(1, property?.DrawThickness ?? 1);
            foreach (ContourResult contour in contours ?? Enumerable.Empty<ContourResult>())
            {
                DrawContourGeometry(resultImage, contour, property, drawColor, thickness);
            }
        }

        private static void DrawContourGeometry(Mat target, ContourResult contour, ContourProperty property, Scalar drawColor, int thickness)
        {
            if (target == null || target.Empty() || contour == null)
            {
                return;
            }

            // Contour is an edge/shape tool: draw the actual contour points by default.
            // Bounding rectangles remain available only through the explicit DrawMode option.
            if ((property?.DrawMode ?? ContourDrawMode.Outline) == ContourDrawMode.BoundingBox)
            {
                DrawContourBoundingBox(target, contour, drawColor, thickness);
                return;
            }

            OpenCvSharp.Point[] points = contour.Contours;
            if (points == null || points.Length < 2)
            {
                return;
            }

            Cv2.Polylines(target, new[] { points }, true, drawColor, thickness, LineTypes.AntiAlias);
            DrawContourCenterMarker(target, contour, drawColor);
        }

        private static void DrawContourBoundingBox(Mat target, ContourResult contour, Scalar drawColor, int thickness)
        {
            Rectangle bounds = contour.Bounding;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            OpenCvSharp.Rect box = ClampRect(
                new OpenCvSharp.Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                target.Width,
                target.Height);
            if (box.Width <= 0 || box.Height <= 0)
            {
                return;
            }

            Cv2.Rectangle(target, box, drawColor, thickness, LineTypes.AntiAlias);
            DrawContourCenterMarker(target, contour, drawColor);
        }

        private static void DrawContourCenterMarker(Mat target, ContourResult contour, Scalar drawColor)
        {
            Cv2.DrawMarker(
                target,
                new OpenCvSharp.Point((int)Math.Round(contour.Center.X), (int)Math.Round(contour.Center.Y)),
                drawColor,
                MarkerTypes.Cross,
                14,
                2,
                LineTypes.AntiAlias);
        }

        public static Mat CreateLineGaugePreviewImage(Mat source, LineGaugeTool tool)
        {
            Mat canvas = CreateColorCanvas(source);
            if (canvas == null || canvas.Empty() || tool == null)
            {
                return canvas;
            }

            DrawLineGaugeRois(canvas, tool, new Scalar(255, 180, 0));
            foreach (LineGaugeResult result in tool.resultList ?? Enumerable.Empty<LineGaugeResult>())
            {
                foreach (OpenCvSharp.Point point in result?.edgeList ?? Enumerable.Empty<OpenCvSharp.Point>())
                {
                    Cv2.Circle(canvas, point, 2, new Scalar(0, 220, 80), -1, LineTypes.AntiAlias);
                }

                if (result?.FitLine != null)
                {
                    Cv2.Line(canvas, result.FitLine.Start, result.FitLine.End, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
                }
            }

            return canvas;
        }

        public static void DrawLineSignalDiagnostic(Mat target, OpenVisionNativeLineSignalProfile profile)
        {
            if (target == null || target.Empty() || profile == null)
            {
                return;
            }

            Scalar scanColor = new Scalar(255, 210, 0);
            Scalar selectedColor = new Scalar(40, 40, 255);
            Scalar alternativeColor = new Scalar(0, 150, 255);
            Cv2.Line(target, profile.ScanStart, profile.ScanEnd, scanColor, 1, LineTypes.AntiAlias);
            Cv2.DrawMarker(
                target,
                profile.SelectedPoint,
                selectedColor,
                MarkerTypes.Cross,
                16,
                2,
                LineTypes.AntiAlias);
            foreach (OpenVisionNativeLineSignalAlternative alternative in profile.Alternatives.Take(4))
            {
                Cv2.DrawMarker(
                    target,
                    alternative.ImagePoint,
                    alternativeColor,
                    MarkerTypes.TiltedCross,
                    10,
                    1,
                    LineTypes.AntiAlias);
            }

            OpenCvSharp.Point labelPoint = new OpenCvSharp.Point(
                Math.Clamp(profile.ScanStart.X + 4, 0, Math.Max(0, target.Width - 1)),
                Math.Clamp(profile.ScanStart.Y - 5, 12, Math.Max(12, target.Height - 1)));
            Cv2.PutText(
                target,
                "Profile " + profile.LineName,
                labelPoint,
                HersheyFonts.HersheySimplex,
                0.38,
                scanColor,
                1,
                LineTypes.AntiAlias);
        }

        public static Mat CreateAffineTransformPreviewImage(Mat transformedImage, IEnumerable<VisionToolOverlay> overlays)
        {
            Mat canvas = CreateColorCanvas(transformedImage);
            foreach (VisionToolOverlay overlay in overlays ?? Enumerable.Empty<VisionToolOverlay>())
            {
                if (overlay == null)
                {
                    continue;
                }

                if (overlay.Kind == VisionToolOverlayKind.Point)
                {
                    Cv2.DrawMarker(
                        canvas,
                        new OpenCvSharp.Point((int)Math.Round(overlay.Center.X), (int)Math.Round(overlay.Center.Y)),
                        new Scalar(0, 230, 80),
                        MarkerTypes.Cross,
                        16,
                        2,
                        LineTypes.AntiAlias);
                    continue;
                }

                if (overlay.Kind != VisionToolOverlayKind.Line)
                {
                    continue;
                }

                bool isDestination = (overlay.Label ?? string.Empty).IndexOf("Destination", StringComparison.OrdinalIgnoreCase) >= 0;
                Scalar color = isDestination ? new Scalar(0, 220, 255) : new Scalar(255, 190, 0);
                Cv2.Line(
                    canvas,
                    new OpenCvSharp.Point((int)Math.Round(overlay.Start.X), (int)Math.Round(overlay.Start.Y)),
                    new OpenCvSharp.Point((int)Math.Round(overlay.End.X), (int)Math.Round(overlay.End.Y)),
                    color,
                    2,
                    LineTypes.AntiAlias);
            }

            return canvas;
        }

        public static Mat CreateMatchingOverlayImage(
            Mat resultImage,
            Mat source,
            IEnumerable<MatchingResult> results,
            bool drawResultBoxes = true)
        {
            bool useResultImage = !OpenCvHelper.IsImageEmpty(resultImage) && !ReferenceEquals(resultImage, source);
            Mat canvas = useResultImage
                ? resultImage
                : source.Clone();

            if (canvas.Channels() == 1)
            {
                Mat colorImage = new Mat();
                Cv2.CvtColor(canvas, colorImage, ColorConversionCodes.GRAY2BGR);
                if (!useResultImage)
                {
                    canvas.Dispose();
                }

                canvas = colorImage;
            }
            else if (canvas.Channels() == 4)
            {
                Mat colorImage = new Mat();
                Cv2.CvtColor(canvas, colorImage, ColorConversionCodes.BGRA2BGR);
                if (!useResultImage)
                {
                    canvas.Dispose();
                }

                canvas = colorImage;
            }

            if (!drawResultBoxes)
            {
                return canvas;
            }

            int index = 0;
            foreach (MatchingResult match in results ?? Enumerable.Empty<MatchingResult>())
            {
                if (match.Bounding.Width <= 0 || match.Bounding.Height <= 0)
                {
                    continue;
                }

                index++;
                OpenCvSharp.Rect box = new OpenCvSharp.Rect(
                    Math.Max(0, (int)Math.Round(match.Bounding.X)),
                    Math.Max(0, (int)Math.Round(match.Bounding.Y)),
                    Math.Max(1, (int)Math.Round(match.Bounding.Width)),
                    Math.Max(1, (int)Math.Round(match.Bounding.Height)));
                box = ClampRect(box, canvas.Width, canvas.Height);
                if (box.Width <= 0 || box.Height <= 0)
                {
                    continue;
                }

                Scalar color = index == 1 ? new Scalar(0, 230, 255) : new Scalar(60, 200, 120);
                DrawMatchingBox(canvas, match, box, color);
                Cv2.DrawMarker(
                    canvas,
                    new OpenCvSharp.Point((int)Math.Round(match.Center.X), (int)Math.Round(match.Center.Y)),
                    color,
                    MarkerTypes.Cross,
                    16,
                    2,
                    LineTypes.AntiAlias);

                string label = string.Format(CultureInfo.InvariantCulture, "#{0} {1:0.000}", index, match.Score);
                int labelY = Math.Max(16, box.Y - 6);
                Cv2.PutText(
                    canvas,
                    label,
                    new OpenCvSharp.Point(box.X, labelY),
                    HersheyFonts.HersheySimplex,
                    0.45,
                    color,
                    1,
                    LineTypes.AntiAlias);
            }

            return canvas;
        }

        private static void DrawMatchingBox(Mat canvas, MatchingResult match, OpenCvSharp.Rect fallbackBox, Scalar color)
        {
            if (!IsFinite(match.Angle) || Math.Abs(match.Angle) < 0.001D)
            {
                Cv2.Rectangle(canvas, fallbackBox, color, 2, LineTypes.AntiAlias);
                return;
            }

            OpenCvSharp.Point[] points = CreateRotatedMatchBoxPoints(match)
                .Select(ToPoint)
                .ToArray();
            if (points.Length != 4)
            {
                Cv2.Rectangle(canvas, fallbackBox, color, 2, LineTypes.AntiAlias);
                return;
            }

            for (int i = 0; i < points.Length; i++)
            {
                Cv2.Line(canvas, points[i], points[(i + 1) % points.Length], color, 2, LineTypes.AntiAlias);
            }
        }

        private static Point2f[] CreateRotatedMatchBoxPoints(MatchingResult match)
        {
            float width = Math.Max(1F, match.Bounding.Width);
            float height = Math.Max(1F, match.Bounding.Height);
            float centerX = IsFinite(match.Center.X) ? match.Center.X : match.Bounding.X + width / 2F;
            float centerY = IsFinite(match.Center.Y) ? match.Center.Y : match.Bounding.Y + height / 2F;
            float halfWidth = width / 2F;
            float halfHeight = height / 2F;

            // MatchingResult.Angle is produced by the OpenCV matching tool. Use the same image-coordinate
            // rotation convention so the taught ROI follows the searched template angle instead of an axis-aligned box.
            double radians = match.Angle * Math.PI / 180D;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            Point2f Transform(float localX, float localY)
            {
                return new Point2f(
                    (float)(centerX + localX * cos + localY * sin),
                    (float)(centerY - localX * sin + localY * cos));
            }

            return new[]
            {
                Transform(-halfWidth, -halfHeight),
                Transform(halfWidth, -halfHeight),
                Transform(halfWidth, halfHeight),
                Transform(-halfWidth, halfHeight)
            };
        }

        private static OpenCvSharp.Point ToPoint(Point2f point)
        {
            return new OpenCvSharp.Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static Mat CreateInternalThresholdPreviewImage(Mat source, OpenCvPropertyBase property)
        {
            if (source == null || source.Empty())
            {
                return new Mat();
            }

            if (property == null || (!property.USE_THRESHOLD && !property.USE_ADAPTIVE_THRESHOLD))
            {
                return source.Clone();
            }

            using Mat gray = CreateGrayImage(source);
            Mat thresholded = new Mat();
            if (property.USE_ADAPTIVE_THRESHOLD)
            {
                int blockSize = NormalizeAdaptiveBlockSize(property.BlockSize);
                double maxValue = Clamp(property.ADAPTIVE_THRESHOLD <= 0D ? 255D : property.ADAPTIVE_THRESHOLD, 1D, 255D);
                Cv2.AdaptiveThreshold(
                    gray,
                    thresholded,
                    maxValue,
                    property.ADAPTIVE_THRESHOLD_ALGORITHM,
                    property.ADAPTIVE_THRESHOLD_TYPES,
                    blockSize,
                    property.Weight);
            }
            else
            {
                Cv2.Threshold(
                    gray,
                    thresholded,
                    Clamp(property.THRESHOLD, 0D, 255D),
                    255D,
                    property.THRESHOLD_TYPES);
            }

            if (property.USE_BITWISENOT)
            {
                Cv2.BitwiseNot(thresholded, thresholded);
            }

            return thresholded;
        }

        private static Mat CreateGrayImage(Mat source)
        {
            if (source.Channels() == 1)
            {
                return source.Clone();
            }

            Mat gray = new Mat();
            if (source.Channels() == 4)
            {
                Cv2.CvtColor(source, gray, ColorConversionCodes.BGRA2GRAY);
            }
            else
            {
                Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            }

            return gray;
        }

        private static int NormalizeAdaptiveBlockSize(int value)
        {
            int normalized = Math.Max(3, value);
            if (normalized % 2 == 0)
            {
                normalized++;
            }

            return normalized;
        }

        private static double Clamp(double value, double minimum, double maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private static Scalar ToBgrScalar(Color color)
        {
            return new Scalar(color.B, color.G, color.R);
        }

        private static Mat CreateColorCanvas(Mat source)
        {
            if (source == null || source.Empty())
            {
                return new Mat();
            }

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

        private static void DrawLineGaugeRois(Mat image, LineGaugeTool tool, Scalar color)
        {
            var property = tool?.property;
            if (property == null || !property.USE_ROI)
            {
                return;
            }

            IEnumerable<OpenCvSharp.Rect> rois = property.USE_MULTI_ROI
                ? property.CvROIS ?? Enumerable.Empty<OpenCvSharp.Rect>()
                : new[] { property.CvROI };

            foreach (OpenCvSharp.Rect roi in rois)
            {
                OpenCvSharp.Rect clamped = ClampRect(roi, image.Width, image.Height);
                if (clamped.Width > 0 && clamped.Height > 0)
                {
                    Cv2.Rectangle(image, clamped, color, 1, LineTypes.AntiAlias);
                }
            }
        }

        private static OpenCvSharp.Rect ClampRect(OpenCvSharp.Rect rect, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return new OpenCvSharp.Rect();
            }

            int x = Math.Min(Math.Max(rect.X, 0), width - 1);
            int y = Math.Min(Math.Max(rect.Y, 0), height - 1);
            int right = Math.Min(Math.Max(rect.X + rect.Width, x + 1), width);
            int bottom = Math.Min(Math.Max(rect.Y + rect.Height, y + 1), height);
            return new OpenCvSharp.Rect(x, y, Math.Max(0, right - x), Math.Max(0, bottom - y));
        }
    }
}
