using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineRunReportImageRenderer
    {
        private const int MaxPointCount = 500;

        public static Bitmap Render(Bitmap source, VisionPipelineStepResult stepResult, int index)
        {
            if (source == null)
            {
                return null;
            }

            Bitmap preview = new Bitmap(source);
            VisionPipelineStepResultSummary summary = VisionPipelineResultSummaryService.CreateStepSummary(index, stepResult);
            Color overlayColor = ResolveOverlayColor(summary.Status);
            Color textBackColor = ResolveOverlayTextBackColor(summary.Status);

            using (Graphics graphics = Graphics.FromImage(preview))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                DrawOverlays(graphics, stepResult?.ToolResult?.Overlays, preview.Size, overlayColor, textBackColor);
                DrawStepBadge(graphics, stepResult?.Step, summary, preview.Size, textBackColor);
            }

            return preview;
        }

        private static void DrawOverlays(Graphics graphics, IEnumerable<VisionToolOverlay> overlays, Size imageSize, Color overlayColor, Color textBackColor)
        {
            using (Pen boxPen = new Pen(overlayColor, 2F))
            using (Pen centerPen = new Pen(Color.FromArgb(245, overlayColor), 2F))
            using (Brush pointBrush = new SolidBrush(Color.FromArgb(210, overlayColor)))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush textBackBrush = new SolidBrush(textBackColor))
            using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                foreach (VisionToolOverlay overlay in overlays ?? Enumerable.Empty<VisionToolOverlay>())
                {
                    if (overlay == null)
                    {
                        continue;
                    }

                    switch (overlay.Kind)
                    {
                        case VisionToolOverlayKind.Rectangle:
                            DrawRectangleOverlay(graphics, overlay, imageSize, boxPen, centerPen, textBrush, textBackBrush, font);
                            break;
                        case VisionToolOverlayKind.Point:
                            DrawCenterMarker(graphics, overlay.Center, imageSize, centerPen, pointBrush);
                            DrawOverlayLabel(graphics, overlay.Label, overlay.Center, imageSize, textBrush, textBackBrush, font);
                            break;
                        case VisionToolOverlayKind.Points:
                            DrawPointOverlay(graphics, overlay, imageSize, pointBrush);
                            break;
                        case VisionToolOverlayKind.Line:
                            DrawLineOverlay(graphics, overlay, imageSize, boxPen, centerPen, textBrush, textBackBrush, font);
                            break;
                    }
                }
            }
        }

        private static void DrawRectangleOverlay(
            Graphics graphics,
            VisionToolOverlay overlay,
            Size imageSize,
            Pen boxPen,
            Pen centerPen,
            Brush textBrush,
            Brush textBackBrush,
            Font font)
        {
            RectangleF bounds = ClampRectangle(overlay.Bounds, imageSize);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            graphics.DrawRectangle(boxPen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            DrawCenterMarker(graphics, overlay.Center, imageSize, centerPen, null);
            DrawOverlayLabel(graphics, overlay.Label, new PointF(bounds.X, bounds.Y), imageSize, textBrush, textBackBrush, font);
        }

        private static void DrawLineOverlay(
            Graphics graphics,
            VisionToolOverlay overlay,
            Size imageSize,
            Pen linePen,
            Pen centerPen,
            Brush textBrush,
            Brush textBackBrush,
            Font font)
        {
            PointF start = ClampPoint(overlay.Start, imageSize);
            PointF end = ClampPoint(overlay.End, imageSize);
            if (Math.Abs(start.X - end.X) < 0.000001 && Math.Abs(start.Y - end.Y) < 0.000001)
            {
                return;
            }

            graphics.DrawLine(linePen, start, end);
            PointF center = new PointF((start.X + end.X) / 2F, (start.Y + end.Y) / 2F);
            DrawCenterMarker(graphics, center, imageSize, centerPen, null);
            DrawOverlayLabel(graphics, overlay.Label, center, imageSize, textBrush, textBackBrush, font);
        }

        private static void DrawPointOverlay(Graphics graphics, VisionToolOverlay overlay, Size imageSize, Brush pointBrush)
        {
            int count = 0;
            foreach (PointF point in overlay.Points)
            {
                if (count++ >= MaxPointCount)
                {
                    break;
                }

                PointF clamped = ClampPoint(point, imageSize);
                graphics.FillEllipse(pointBrush, clamped.X - 1.5F, clamped.Y - 1.5F, 3F, 3F);
            }
        }

        private static void DrawStepBadge(Graphics graphics, VisionPipelineStep step, VisionPipelineStepResultSummary summary, Size imageSize, Color backColor)
        {
            string text = BuildStepBadgeText(step, summary);
            if (string.IsNullOrWhiteSpace(text) || imageSize.Width <= 12 || imageSize.Height <= 12)
            {
                return;
            }

            text = TruncateText(text, 120);
            using (Font font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush backBrush = new SolidBrush(backColor))
            using (StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            })
            {
                SizeF textSize = graphics.MeasureString(text, font);
                float width = Math.Min(Math.Max(96F, textSize.Width + 16F), Math.Max(12F, imageSize.Width - 12F));
                float height = Math.Min(Math.Max(22F, textSize.Height + 8F), Math.Max(12F, imageSize.Height - 12F));
                RectangleF bounds = new RectangleF(6F, 6F, width, height);
                graphics.FillRectangle(backBrush, bounds);
                graphics.DrawRectangle(Pens.White, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                graphics.DrawString(text, font, textBrush, new RectangleF(bounds.X + 8F, bounds.Y + 2F, bounds.Width - 12F, bounds.Height - 4F), format);
            }
        }

        private static string BuildStepBadgeText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (summary == null || string.IsNullOrWhiteSpace(summary.Status))
            {
                return string.Empty;
            }

            List<string> parts = new List<string> { summary.Status };
            string metricText = BuildMetricText(step, summary);
            if (!string.IsNullOrWhiteSpace(metricText))
            {
                parts.Add(metricText);
            }

            if (IsFailureStatus(summary.Status) && !string.IsNullOrWhiteSpace(summary.Message))
            {
                parts.Add(TruncateText(summary.Message, 72));
            }

            return string.Join(" | ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
        }

        private static string BuildMetricText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            string metricName = ResolveMetricName(step, summary);
            if (string.IsNullOrWhiteSpace(metricName))
            {
                return string.Empty;
            }

            double metricValue = 0d;
            bool hasValue = summary?.Metrics != null && summary.Metrics.TryGetValue(metricName, out metricValue);
            string valueText = hasValue ? metricValue.ToString("0.###", CultureInfo.InvariantCulture) : "N/A";
            return $"{metricName}={valueText}{BuildAcceptanceCriteriaText(step, metricName)}";
        }

        private static string ResolveMetricName(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (!string.IsNullOrWhiteSpace(step?.AcceptanceMetricName))
            {
                return step.AcceptanceMetricName;
            }

            if (summary?.Metrics == null || summary.Metrics.Count == 0)
            {
                return string.Empty;
            }

            foreach (string metricName in VisionPipelineKnownMetrics.GetMetricNamesForTool(step?.ToolType))
            {
                if (summary.Metrics.ContainsKey(metricName))
                {
                    return metricName;
                }
            }

            return VisionPipelineKnownMetrics.OrderMetrics(summary.Metrics).FirstOrDefault().Key ?? string.Empty;
        }

        private static string BuildAcceptanceCriteriaText(VisionPipelineStep step, string metricName)
        {
            if (step == null
                || !step.UseAcceptance
                || string.IsNullOrWhiteSpace(metricName)
                || !string.Equals(step.AcceptanceMetricName, metricName, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            bool hasMinimum = step.UseAcceptanceMetricMinimum;
            bool hasMaximum = step.UseAcceptanceMetricMaximum;
            if (hasMinimum && hasMaximum)
            {
                if (Math.Abs(step.AcceptanceMetricMinimum - step.AcceptanceMetricMaximum) < 0.000001)
                {
                    return $" = {step.AcceptanceMetricMinimum:0.###}";
                }

                return $" [{step.AcceptanceMetricMinimum:0.###}..{step.AcceptanceMetricMaximum:0.###}]";
            }

            if (hasMinimum)
            {
                return $" >= {step.AcceptanceMetricMinimum:0.###}";
            }

            if (hasMaximum)
            {
                return $" <= {step.AcceptanceMetricMaximum:0.###}";
            }

            return string.Empty;
        }

        private static void DrawCenterMarker(Graphics graphics, PointF center, Size imageSize, Pen pen, Brush brush)
        {
            PointF point = ClampPoint(center, imageSize);
            const float radius = 4F;
            graphics.DrawLine(pen, point.X - radius, point.Y, point.X + radius, point.Y);
            graphics.DrawLine(pen, point.X, point.Y - radius, point.X, point.Y + radius);
            if (brush != null)
            {
                graphics.FillEllipse(brush, point.X - 2F, point.Y - 2F, 4F, 4F);
            }
        }

        private static void DrawOverlayLabel(Graphics graphics, string label, PointF anchor, Size imageSize, Brush textBrush, Brush textBackBrush, Font font)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return;
            }

            PointF point = ClampPoint(anchor, imageSize);
            SizeF textSize = graphics.MeasureString(label, font);
            float x = Math.Min(Math.Max(point.X, 0), Math.Max(0, imageSize.Width - textSize.Width - 4));
            float y = Math.Max(0, point.Y - textSize.Height - 4);
            RectangleF background = new RectangleF(x, y, textSize.Width + 4, textSize.Height + 2);
            graphics.FillRectangle(textBackBrush, background);
            graphics.DrawString(label, font, textBrush, x + 2, y + 1);
        }

        private static RectangleF ClampRectangle(RectangleF rectangle, Size imageSize)
        {
            float x = Math.Max(0, Math.Min(rectangle.X, imageSize.Width));
            float y = Math.Max(0, Math.Min(rectangle.Y, imageSize.Height));
            float right = Math.Max(0, Math.Min(rectangle.Right, imageSize.Width));
            float bottom = Math.Max(0, Math.Min(rectangle.Bottom, imageSize.Height));
            return new RectangleF(x, y, Math.Max(0, right - x), Math.Max(0, bottom - y));
        }

        private static PointF ClampPoint(PointF point, Size imageSize)
        {
            return new PointF(
                Math.Max(0, Math.Min(point.X, imageSize.Width)),
                Math.Max(0, Math.Min(point.Y, imageSize.Height)));
        }

        private static Color ResolveOverlayColor(string status)
        {
            switch ((status ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "OK":
                    return Color.FromArgb(0, 210, 120);
                case "NG":
                case "TIMEOUT":
                    return Color.FromArgb(235, 60, 60);
                case "RUN":
                    return Color.FromArgb(245, 160, 30);
                case "CANCEL":
                case "SKIP":
                    return Color.FromArgb(145, 150, 158);
                default:
                    return Color.FromArgb(60, 140, 220);
            }
        }

        private static Color ResolveOverlayTextBackColor(string status)
        {
            Color color = ResolveOverlayColor(status);
            return Color.FromArgb(190, Math.Max(0, color.R - 35), Math.Max(0, color.G - 35), Math.Max(0, color.B - 35));
        }

        private static bool IsFailureStatus(string status)
        {
            string normalized = (status ?? string.Empty).Trim().ToUpperInvariant();
            return normalized == "NG" || normalized == "TIMEOUT" || normalized == "CANCEL";
        }

        private static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text) || maxLength <= 0)
            {
                return string.Empty;
            }

            return text.Length <= maxLength
                ? text
                : text.Substring(0, Math.Max(0, maxLength - 3)) + "...";
        }
    }
}
