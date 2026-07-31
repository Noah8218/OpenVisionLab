using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OpenVisionLab
{
    internal static class OpenVisionPipelineReviewCircleEvidenceRenderService
    {
        public static Bitmap CreateSampleHighlight(
            Bitmap sourceImage,
            VisionPipelineCircleEvidence circle,
            VisionPipelineCircleSampleEvidence sample)
        {
            if (sourceImage == null || circle == null || sample == null)
            {
                return null;
            }

            var highlighted = new Bitmap(sourceImage);
            using Graphics graphics = Graphics.FromImage(highlighted);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float thickness = Math.Max(2F, Math.Min(highlighted.Width, highlighted.Height) / 180F);
            using Pen scanPen = new Pen(Color.Gold, thickness);
            graphics.DrawLine(
                scanPen,
                (float)sample.ScanStartX,
                (float)sample.ScanStartY,
                (float)sample.ScanEndX,
                (float)sample.ScanEndY);

            if (circle.HasFit)
            {
                using Pen fitPen = new Pen(Color.LimeGreen, Math.Max(1F, thickness * 0.75F))
                {
                    DashStyle = DashStyle.Dash
                };
                float radius = (float)Math.Max(1D, circle.FitRadiusPx);
                graphics.DrawEllipse(
                    fitPen,
                    (float)circle.FitCenterX - radius,
                    (float)circle.FitCenterY - radius,
                    radius * 2F,
                    radius * 2F);
            }

            Color pointColor = sample.FitInlier
                ? Color.LimeGreen
                : sample.ContrastAccepted ? Color.Red : Color.Orange;
            using Pen pointPen = new Pen(pointColor, thickness);
            if (sample.HasEdgePoint)
            {
                DrawCross(
                    graphics,
                    pointPen,
                    sample.EdgeX,
                    sample.EdgeY,
                    Math.Max(6F, thickness * 3F));
            }

            string label = sample.HasFitResidual
                ? $"#{sample.Number} {sample.StateText} / residual {sample.FitResidualPx:+0.###;-0.###;0} px"
                : $"#{sample.Number} {sample.StateText}";
            using Font font = new Font(
                "Segoe UI",
                Math.Max(9F, thickness * 3F),
                FontStyle.Bold,
                GraphicsUnit.Pixel);
            using Brush background = new SolidBrush(Color.FromArgb(205, 12, 28, 32));
            using Brush foreground = new SolidBrush(Color.White);
            SizeF size = graphics.MeasureString(label, font);
            float labelX = 8F;
            float labelY = Math.Max(8F, highlighted.Height - size.Height - 12F);
            graphics.FillRectangle(
                background,
                labelX - 3F,
                labelY - 2F,
                Math.Min(highlighted.Width - 10F, size.Width + 6F),
                size.Height + 4F);
            graphics.DrawString(label, font, foreground, labelX, labelY);
            return highlighted;
        }

        private static void DrawCross(Graphics graphics, Pen pen, double x, double y, float size)
        {
            float px = (float)x;
            float py = (float)y;
            graphics.DrawLine(pen, px - size, py, px + size, py);
            graphics.DrawLine(pen, px, py - size, px, py + size);
        }
    }
}
