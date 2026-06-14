using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public enum PipelineOverlayLabelMode
    {
        None = 0,
        Number = 1,
        Details = 2
    }

    public sealed class PipelineOverlayImageCanvas : Control
    {
        private Bitmap image;
        private List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();
        private float zoom = 1F;
        private Point pan = Point.Empty;
        private bool dragging;
        private Point lastMouse;
        private bool ownsImage;

        public PipelineOverlayImageCanvas()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseEnter += (sender, e) => Focus();
            SetContent(null, null);
        }

        public event EventHandler ViewChanged;

        public event Action<int> OverlaySelected;

        public bool ShowBoxes { get; set; } = true;

        public PipelineOverlayLabelMode LabelMode { get; set; } = PipelineOverlayLabelMode.Number;

        public int PointLimit { get; set; } = 300;

        public Color OverlayColor { get; set; } = Color.FromArgb(0, 210, 120);

        public float StrokeWidth { get; set; } = 2F;

        public int SelectedOverlayIndex { get; set; } = -1;

        public float Zoom => zoom;

        public void SetContent(Bitmap image, List<VisionToolOverlay> overlays)
        {
            if (ownsImage)
            {
                this.image?.Dispose();
            }

            if (image == null)
            {
                this.image = new Bitmap(16, 16);
                ownsImage = true;
            }
            else
            {
                this.image = image;
                ownsImage = false;
            }

            this.overlays = overlays ?? new List<VisionToolOverlay>();
            SelectedOverlayIndex = -1;
            Invalidate();
        }

        public void FitToWindow()
        {
            if (image.Width <= 0 || image.Height <= 0 || Width <= 0 || Height <= 0)
            {
                return;
            }

            float sx = (Width - 24F) / image.Width;
            float sy = (Height - 24F) / image.Height;
            zoom = Math.Max(0.05F, Math.Min(sx, sy));
            CenterImage();
            Invalidate();
            ViewChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetZoom(float value)
        {
            zoom = Math.Max(0.05F, Math.Min(20F, value));
            CenterImage();
            Invalidate();
            ViewChanged?.Invoke(this, EventArgs.Empty);
        }

        public void FocusOverlay(int overlayIndex)
        {
            if (overlayIndex < 0 || overlayIndex >= overlays.Count)
            {
                return;
            }

            VisionToolOverlay overlay = overlays[overlayIndex];
            RectangleF target = ResolveOverlayBounds(overlay);
            if (target.Width <= 0 || target.Height <= 0 || Width <= 0 || Height <= 0)
            {
                SelectedOverlayIndex = overlayIndex;
                Invalidate();
                return;
            }

            float sx = (Width - 80F) / target.Width;
            float sy = (Height - 80F) / target.Height;
            zoom = Math.Max(0.2F, Math.Min(8F, Math.Min(sx, sy)));
            pan = new Point(
                (int)(Width / 2F - (target.X + target.Width / 2F) * zoom),
                (int)(Height / 2F - (target.Y + target.Height / 2F) * zoom));
            SelectedOverlayIndex = overlayIndex;
            Invalidate();
            ViewChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && ownsImage)
            {
                image?.Dispose();
                image = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            e.Graphics.TranslateTransform(pan.X, pan.Y);
            e.Graphics.ScaleTransform(zoom, zoom);
            e.Graphics.DrawImage(image, 0, 0, image.Width, image.Height);
            DrawOverlays(e.Graphics);
        }

        private void DrawOverlays(Graphics graphics)
        {
            float penWidth = Math.Max(StrokeWidth / zoom, 1F / zoom);
            using (Pen boxPen = new Pen(OverlayColor, penWidth))
            using (Pen selectedPen = new Pen(Color.FromArgb(255, 255, 224, 72), penWidth * 1.8F))
            using (Pen centerPen = new Pen(Color.FromArgb(245, OverlayColor), penWidth))
            using (Brush pointBrush = new SolidBrush(Color.FromArgb(210, OverlayColor)))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush textBackBrush = new SolidBrush(Color.FromArgb(205, OverlayColor.R / 2, OverlayColor.G / 2, OverlayColor.B / 2)))
            using (Font font = new Font("Segoe UI", Math.Max(7F, 10F / zoom), FontStyle.Bold, GraphicsUnit.Pixel))
            {
                for (int i = 0; i < overlays.Count; i++)
                {
                    VisionToolOverlay overlay = overlays[i];
                    Pen activeBoxPen = i == SelectedOverlayIndex ? selectedPen : boxPen;
                    switch (overlay.Kind)
                    {
                        case VisionToolOverlayKind.Rectangle:
                            if (ShowBoxes)
                            {
                                graphics.DrawRectangle(activeBoxPen, overlay.Bounds.X, overlay.Bounds.Y, overlay.Bounds.Width, overlay.Bounds.Height);
                                DrawCenterMarker(graphics, overlay.Center, centerPen);
                            }

                            DrawLabel(graphics, overlay.Label, overlay.Bounds.Location, textBrush, textBackBrush, font);
                            break;
                        case VisionToolOverlayKind.Point:
                            DrawCenterMarker(graphics, overlay.Center, centerPen);
                            DrawLabel(graphics, overlay.Label, overlay.Center, textBrush, textBackBrush, font);
                            break;
                        case VisionToolOverlayKind.Points:
                            DrawPoints(graphics, overlay.Points, pointBrush);
                            break;
                        case VisionToolOverlayKind.Line:
                            graphics.DrawLine(activeBoxPen, overlay.Start, overlay.End);
                            DrawLabel(graphics, overlay.Label, overlay.Center, textBrush, textBackBrush, font);
                            break;
                    }
                }
            }
        }

        private void DrawPoints(Graphics graphics, IEnumerable<PointF> points, Brush brush)
        {
            int count = 0;
            float radius = Math.Max(1.5F / zoom, 0.75F);
            foreach (PointF point in points ?? Enumerable.Empty<PointF>())
            {
                if (count++ >= PointLimit)
                {
                    break;
                }

                graphics.FillEllipse(brush, point.X - radius, point.Y - radius, radius * 2F, radius * 2F);
            }
        }

        private void DrawLabel(Graphics graphics, string label, PointF location, Brush textBrush, Brush backBrush, Font font)
        {
            string text = FormatOverlayLabel(label);
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            SizeF size = graphics.MeasureString(text, font);
            RectangleF back = new RectangleF(
                location.X,
                Math.Max(0, location.Y - size.Height - 3F / zoom),
                size.Width + 4F / zoom,
                size.Height + 2F / zoom);
            graphics.FillRectangle(backBrush, back);
            graphics.DrawString(text, font, textBrush, back.X + 2F / zoom, back.Y + 1F / zoom);
        }

        private string FormatOverlayLabel(string label)
        {
            if (LabelMode == PipelineOverlayLabelMode.None || string.IsNullOrWhiteSpace(label))
            {
                return string.Empty;
            }

            if (LabelMode == PipelineOverlayLabelMode.Details)
            {
                return label;
            }

            string trimmed = label.Trim();
            int separator = trimmed.IndexOf(' ');
            return separator <= 0 ? trimmed : trimmed.Substring(0, separator);
        }

        private void DrawCenterMarker(Graphics graphics, PointF center, Pen pen)
        {
            float radius = Math.Max(4F / zoom, 1.5F);
            graphics.DrawLine(pen, center.X - radius, center.Y, center.X + radius, center.Y);
            graphics.DrawLine(pen, center.X, center.Y - radius, center.X, center.Y + radius);
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            float previous = zoom;
            float factor = e.Delta > 0 ? 1.15F : 1F / 1.15F;
            zoom = Math.Max(0.05F, Math.Min(20F, zoom * factor));
            if (Math.Abs(previous - zoom) < 0.0001F)
            {
                return;
            }

            pan = new Point(
                (int)(e.X - (e.X - pan.X) * (zoom / previous)),
                (int)(e.Y - (e.Y - pan.Y) * (zoom / previous)));
            Invalidate();
            ViewChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            int overlayIndex = HitTestOverlay(e.Location);
            if (overlayIndex >= 0)
            {
                SelectedOverlayIndex = overlayIndex;
                OverlaySelected?.Invoke(overlayIndex);
                Invalidate();
                return;
            }

            dragging = true;
            lastMouse = e.Location;
            Cursor = Cursors.Hand;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging)
            {
                return;
            }

            pan.Offset(e.X - lastMouse.X, e.Y - lastMouse.Y);
            lastMouse = e.Location;
            Invalidate();
            ViewChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            Cursor = Cursors.Default;
        }

        private void CenterImage()
        {
            pan = new Point(
                (int)((Width - image.Width * zoom) / 2F),
                (int)((Height - image.Height * zoom) / 2F));
        }

        private int HitTestOverlay(Point screenPoint)
        {
            PointF imagePoint = new PointF(
                (screenPoint.X - pan.X) / zoom,
                (screenPoint.Y - pan.Y) / zoom);

            for (int i = overlays.Count - 1; i >= 0; i--)
            {
                RectangleF bounds = ResolveOverlayBounds(overlays[i]);
                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    continue;
                }

                RectangleF expanded = bounds;
                float padding = Math.Max(4F / zoom, 3F);
                expanded.Inflate(padding, padding);
                if (expanded.Contains(imagePoint))
                {
                    return i;
                }
            }

            return -1;
        }

        private static RectangleF ResolveOverlayBounds(VisionToolOverlay overlay)
        {
            if (overlay == null)
            {
                return RectangleF.Empty;
            }

            if (overlay.Kind == VisionToolOverlayKind.Line)
            {
                float x = Math.Min(overlay.Start.X, overlay.End.X);
                float y = Math.Min(overlay.Start.Y, overlay.End.Y);
                return new RectangleF(
                    x,
                    y,
                    Math.Abs(overlay.Start.X - overlay.End.X),
                    Math.Abs(overlay.Start.Y - overlay.End.Y));
            }

            if (overlay.Bounds.Width > 0 || overlay.Bounds.Height > 0)
            {
                return overlay.Bounds;
            }

            return new RectangleF(overlay.Center.X - 4F, overlay.Center.Y - 4F, 8F, 8F);
        }
    }
}
