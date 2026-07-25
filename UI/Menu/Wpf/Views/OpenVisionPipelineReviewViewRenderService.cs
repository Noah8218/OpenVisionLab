using System;
using System.Drawing;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionPipelineReviewViewRenderService
    {
        internal static Bitmap CreateScaleCalibrationPreview(
            Bitmap sourceImage,
            VisionPipelineGeometryFeatureResult pointA,
            VisionPipelineGeometryFeatureResult pointB,
            out string previewLabel)
        {
            previewLabel = string.Empty;

            if (sourceImage == null)
            {
                return null;
            }

            var drawing = new Bitmap(sourceImage);
            using Graphics graphics = Graphics.FromImage(drawing);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float thickness = Math.Max(2F, Math.Min(drawing.Width, drawing.Height) / 180F);
            using Pen linePen = new Pen(Color.LimeGreen, thickness);
            using Pen pointAPen = new Pen(Color.Gold, thickness);
            using Pen pointBPen = new Pen(Color.DeepSkyBlue, thickness);

            graphics.DrawLine(linePen, (float)pointA.CenterX, (float)pointA.CenterY, (float)pointB.CenterX, (float)pointB.CenterY);
            DrawCross(graphics, pointAPen, pointA.CenterX, pointA.CenterY, thickness, "A");
            DrawCross(graphics, pointBPen, pointB.CenterX, pointB.CenterY, thickness, "B");

            double dx = pointB.CenterX - pointA.CenterX;
            double dy = pointB.CenterY - pointA.CenterY;
            double distancePx = Math.Sqrt(dx * dx + dy * dy);
            previewLabel = string.Concat(distancePx.ToString("0.###", CultureInfo.CurrentCulture), " px");

            using Font font = new Font(
                "Segoe UI",
                Math.Max(9F, thickness * 3F),
                System.Drawing.FontStyle.Bold,
                GraphicsUnit.Pixel);
            using Brush background = new SolidBrush(Color.FromArgb(190, 12, 28, 32));
            using Brush foreground = new SolidBrush(Color.White);
            float labelX = (float)((pointA.CenterX + pointB.CenterX) / 2D);
            float labelY = (float)((pointA.CenterY + pointB.CenterY) / 2D);
            SizeF size = graphics.MeasureString(previewLabel, font);
            graphics.FillRectangle(background, labelX - size.Width / 2F - 3F, labelY - size.Height - 6F, size.Width + 6F, size.Height + 3F);
            graphics.DrawString(previewLabel, font, foreground, labelX - size.Width / 2F, labelY - size.Height - 5F);

            return drawing;
        }

        internal static Bitmap CreateGeometryHighlight(Bitmap sourceImage, VisionPipelineGeometryFeatureResult item)
        {
            if (sourceImage == null || item == null)
            {
                return null;
            }

            var highlighted = new Bitmap(sourceImage);
            using Graphics graphics = Graphics.FromImage(highlighted);
            float thickness = Math.Max(2f, Math.Min(highlighted.Width, highlighted.Height) / 180f);
            using Pen pen = new Pen(Color.LimeGreen, thickness);

            if (item.Kind == VisionPipelineGeometryKind.Segment)
            {
                graphics.DrawLine(pen, (float)item.X1, (float)item.Y1, (float)item.X2, (float)item.Y2);
            }
            else if (item.Kind == VisionPipelineGeometryKind.Circle)
            {
                float radius = (float)Math.Max(1D, item.RadiusPx);
                graphics.DrawEllipse(pen, (float)item.CenterX - radius, (float)item.CenterY - radius, radius * 2f, radius * 2f);
            }

            float cross = Math.Max(7f, thickness * 3f);
            DrawCross(graphics, pen, item.CenterX, item.CenterY, cross, string.Empty);

            return highlighted;
        }

        internal static Bitmap CreateObjectHighlight(Bitmap sourceImage, VisionPipelineObjectResult item)
        {
            if (sourceImage == null || item == null)
            {
                return null;
            }

            var highlighted = new Bitmap(sourceImage);
            using Graphics graphics = Graphics.FromImage(highlighted);
            Color color = item.Accepted ? Color.LimeGreen : Color.OrangeRed;
            float thickness = Math.Max(2f, Math.Min(highlighted.Width, highlighted.Height) / 180f);
            using Pen pen = new Pen(color, thickness);

            int width = Math.Max(1, item.BoundsWidth);
            int height = Math.Max(1, item.BoundsHeight);
            graphics.DrawRectangle(pen, item.BoundsX, item.BoundsY, width, height);

            float cross = Math.Max(5f, thickness * 2f);
            DrawCross(graphics, pen, item.CenterX, item.CenterY, cross, string.Empty);

            return highlighted;
        }

        internal static bool TryParsePositiveDouble(string text, out double value)
        {
            bool parsed = (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                    || double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value > 0D;
            return parsed;
        }

        internal static string ResolveUnitText(VisionScaleCalibrationUnit unit)
        {
            return unit == VisionScaleCalibrationUnit.Micrometer
                ? "¥ìm"
                : unit == VisionScaleCalibrationUnit.Inch ? "inch" : "mm";
        }

        internal static double GeometryHitDistance(VisionPipelineGeometryFeatureResult item, double x, double y)
        {
            if (item == null)
            {
                return double.PositiveInfinity;
            }

            double dx = x - item.CenterX;
            double dy = y - item.CenterY;
            if (item.Kind == VisionPipelineGeometryKind.Point)
            {
                return Math.Sqrt(dx * dx + dy * dy);
            }

            if (item.Kind == VisionPipelineGeometryKind.Circle)
            {
                return Math.Abs(Math.Sqrt(dx * dx + dy * dy) - item.RadiusPx);
            }

            if (item.Kind != VisionPipelineGeometryKind.Segment)
            {
                return double.PositiveInfinity;
            }

            double vx = item.X2 - item.X1;
            double vy = item.Y2 - item.Y1;
            double lengthSquared = vx * vx + vy * vy;
            if (lengthSquared <= 1e-12D)
            {
                return double.PositiveInfinity;
            }

            double fraction = Math.Max(0D, Math.Min(1D, ((x - item.X1) * vx + (y - item.Y1) * vy) / lengthSquared));
            double nearestX = item.X1 + fraction * vx;
            double nearestY = item.Y1 + fraction * vy;
            double nearestDx = x - nearestX;
            double nearestDy = y - nearestY;
            return Math.Sqrt(nearestDx * nearestDx + nearestDy * nearestDy);
        }

        private static void DrawCross(Graphics graphics, Pen pen, double x, double y, float cross, string label)
        {
            DrawCross(graphics, pen, (float)x, (float)y, cross, label);
        }

        private static void DrawCross(Graphics graphics, Pen pen, float x, float y, float cross, string label)
        {
            graphics.DrawLine(pen, x - cross, y, x + cross, y);
            graphics.DrawLine(pen, x, y - cross, x, y + cross);
            if (string.IsNullOrWhiteSpace(label))
            {
                return;
            }

            using Font font = new Font(
                "Segoe UI",
                Math.Max(9F, cross * 2F),
                System.Drawing.FontStyle.Bold,
                GraphicsUnit.Pixel);
            using Brush brush = new SolidBrush(pen.Color);
            graphics.DrawString(label, font, brush, x + cross + 2F, y - cross - 2F);
        }
    }
}
