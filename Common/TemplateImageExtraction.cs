using OpenCvSharp;
using System;

namespace OpenVisionLab
{
    internal static class TemplateImageExtraction
    {
        public static Mat Extract(Mat sourceImage, Rect selectedRegion, double rotationDegrees)
        {
            Rect templateRegion = ClampToMatBounds(sourceImage, selectedRegion);
            if (!IsValidRoi(templateRegion)) { return new Mat(); }

            double normalizedRotationDegrees = NormalizeRotationDegrees(rotationDegrees);
            if (Math.Abs(normalizedRotationDegrees) < 0.0001D)
            {
                return sourceImage.SubMat(templateRegion).Clone();
            }

            Point2f[] sourcePoints = GetRotatedRegionSourcePoints(templateRegion, normalizedRotationDegrees);
            Point2f[] destinationPoints =
            {
                new Point2f(0, 0),
                new Point2f(templateRegion.Width - 1, 0),
                new Point2f(0, templateRegion.Height - 1)
            };

            using Mat transform = Cv2.GetAffineTransform(sourcePoints, destinationPoints);
            Mat extracted = new Mat();
            Cv2.WarpAffine(
                sourceImage,
                extracted,
                transform,
                new Size(templateRegion.Width, templateRegion.Height),
                InterpolationFlags.Linear,
                BorderTypes.Replicate);
            return extracted;
        }

        public static double NormalizeRotationDegrees(double rotationDegrees)
        {
            if (double.IsNaN(rotationDegrees) || double.IsInfinity(rotationDegrees))
            {
                return 0D;
            }

            double normalized = rotationDegrees % 360D;
            if (normalized > 180D)
            {
                normalized -= 360D;
            }
            else if (normalized <= -180D)
            {
                normalized += 360D;
            }

            return Math.Round(normalized, 3);
        }

        private static Point2f[] GetRotatedRegionSourcePoints(Rect rect, double rotationDegrees)
        {
            double centerX = rect.X + rect.Width / 2D;
            double centerY = rect.Y + rect.Height / 2D;
            double radians = rotationDegrees * Math.PI / 180D;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            Point2f Transform(double localX, double localY)
            {
                double dx = localX - rect.Width / 2D;
                double dy = localY - rect.Height / 2D;
                return new Point2f(
                    (float)(centerX + (dx * cos) - (dy * sin)),
                    (float)(centerY + (dx * sin) + (dy * cos)));
            }

            return new[]
            {
                Transform(0D, 0D),
                Transform(rect.Width - 1D, 0D),
                Transform(0D, rect.Height - 1D)
            };
        }

        private static Rect ClampToMatBounds(Mat sourceImage, Rect roi)
        {
            if (sourceImage == null || sourceImage.Empty() || !IsValidRoi(roi)) { return new Rect(); }

            int x = Math.Max(0, roi.X);
            int y = Math.Max(0, roi.Y);
            int right = Math.Min(sourceImage.Width, roi.X + roi.Width);
            int bottom = Math.Min(sourceImage.Height, roi.Y + roi.Height);
            int width = Math.Max(0, right - x);
            int height = Math.Max(0, bottom - y);

            return width > 0 && height > 0 ? new Rect(x, y, width, height) : new Rect();
        }

        private static bool IsValidRoi(Rect roi)
        {
            return roi.Width > 0 && roi.Height > 0;
        }
    }
}
