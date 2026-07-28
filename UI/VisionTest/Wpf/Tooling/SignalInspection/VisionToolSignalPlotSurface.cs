using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class VisionToolSignalPlotSurface : FrameworkElement
    {
        private static readonly Thickness PlotMargin = new Thickness(46, 12, 14, 30);
        private VisionToolSignalEvidence evidence;
        private IReadOnlyList<VisionToolSignalMarker> advisoryMarkers =
            Array.Empty<VisionToolSignalMarker>();
        private double domainMin;
        private double domainMax = 1;
        private double viewMin;
        private double viewMax = 1;
        private Point? cursorPosition;
        private Point panStart;
        private double panStartMin;
        private double panStartMax;
        private bool isPanning;
        private bool panMoved;
        private double? selectionX;
        private VisionToolSignalMarker draggedMarker;
        private double draggedMarkerX;

        public VisionToolSignalPlotSurface()
        {
            Focusable = true;
            Cursor = Cursors.Cross;
            SnapsToDevicePixels = true;
        }

        public event EventHandler<string> CursorValueChanged = delegate { };

        public event EventHandler<VisionToolSignalMarkerValueChangedEventArgs> MarkerValueChangeRequested = delegate { };

        public event EventHandler<VisionToolSignalSampleSelectedEventArgs> SampleSelectionRequested = delegate { };

        internal int SeriesCount => evidence?.Series.Count ?? 0;

        internal int AdvisoryMarkerCount => advisoryMarkers.Count;

        internal double? SelectionX => selectionX;

        internal void SelectSampleForTest(double x)
        {
            SampleSelectionRequested(this, new VisionToolSignalSampleSelectedEventArgs(x));
        }

        public void SetEvidence(VisionToolSignalEvidence value)
        {
            evidence = value;
            advisoryMarkers = Array.Empty<VisionToolSignalMarker>();
            draggedMarker = null;
            selectionX = null;
            if (evidence == null)
            {
                domainMin = 0;
                domainMax = 1;
            }
            else
            {
                domainMin = evidence.Series.Min(series => series.XStart);
                domainMax = evidence.Series.Max(series => series.XEnd);
                if (domainMax <= domainMin)
                {
                    domainMax = domainMin + 1;
                }
            }

            ResetView();
        }

        public void SetAdvisoryMarkers(IEnumerable<VisionToolSignalMarker> markers)
        {
            advisoryMarkers = (markers ?? Array.Empty<VisionToolSignalMarker>())
                .Where(marker => marker != null)
                .ToArray();
            InvalidateVisual();
        }

        public void SetSelectionX(double? value)
        {
            selectionX = value.HasValue && double.IsFinite(value.Value)
                ? Math.Clamp(value.Value, domainMin, domainMax)
                : null;
            InvalidateVisual();
        }

        public void ResetView()
        {
            viewMin = domainMin;
            viewMax = domainMax;
            cursorPosition = null;
            CursorValueChanged(this, string.Empty);
            InvalidateVisual();
        }

        internal bool ExerciseNavigationForTest()
        {
            if (evidence == null)
            {
                return false;
            }

            double domainSpan = domainMax - domainMin;
            double nextSpan = domainSpan * 0.5d;
            double centeredMin = domainMin + ((domainSpan - nextSpan) * 0.5d);
            SetView(centeredMin, centeredMin + nextSpan);
            bool zoomed = (viewMax - viewMin) < domainSpan;
            double beforePan = viewMin;
            SetView(viewMin + (nextSpan * 0.1d), viewMax + (nextSpan * 0.1d));
            bool panned = viewMin > beforePan;
            ResetView();
            return zoomed && panned;
        }

        internal void CommitMarkerForTest(string markerId, double value)
        {
            VisionToolSignalMarker marker = evidence?.Markers
                .FirstOrDefault(item => item.IsEditable && string.Equals(item.Id, markerId, StringComparison.Ordinal));
            if (marker == null)
            {
                throw new InvalidOperationException("An editable signal marker was not found: " + markerId);
            }

            MarkerValueChangeRequested(
                this,
                new VisionToolSignalMarkerValueChangedEventArgs(marker.Id, SnapAndClamp(marker, value)));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Rect plot = GetPlotRect();
            drawingContext.DrawRoundedRectangle(Brushes.White, new Pen(new SolidColorBrush(Color.FromRgb(191, 208, 216)), 1), new Rect(0, 0, ActualWidth, ActualHeight), 3, 3);
            if (evidence == null || plot.Width <= 1 || plot.Height <= 1)
            {
                return;
            }

            GetVisibleYRange(out double yMin, out double yMax);
            DrawGridAndAxes(drawingContext, plot, yMin, yMax);
            drawingContext.PushClip(new RectangleGeometry(plot));
            foreach (VisionToolSignalSeries series in evidence.Series)
            {
                DrawSeries(drawingContext, plot, yMin, yMax, series);
            }

            DrawMarkers(drawingContext, plot);
            DrawSelection(drawingContext, plot);
            DrawCursor(drawingContext, plot);
            drawingContext.Pop();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (evidence == null)
            {
                return;
            }

            Rect plot = GetPlotRect();
            Point position = e.GetPosition(this);
            if (!plot.Contains(position))
            {
                return;
            }

            double currentSpan = viewMax - viewMin;
            double minimumSpan = Math.Max((domainMax - domainMin) / 128d, 1d);
            double scale = e.Delta > 0 ? 0.8d : 1.25d;
            double nextSpan = Math.Clamp(currentSpan * scale, minimumSpan, domainMax - domainMin);
            double anchor = PixelToX(position.X, plot);
            double anchorRatio = (anchor - viewMin) / currentSpan;
            double nextMin = anchor - (nextSpan * anchorRatio);
            SetView(nextMin, nextMin + nextSpan);
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (evidence == null || !GetPlotRect().Contains(e.GetPosition(this)))
            {
                return;
            }

            Focus();
            VisionToolSignalMarker marker = FindEditableMarker(e.GetPosition(this), GetPlotRect());
            if (marker != null)
            {
                draggedMarker = marker;
                draggedMarkerX = marker.X;
                Cursor = Cursors.SizeWE;
                CaptureMouse();
                e.Handled = true;
                return;
            }

            isPanning = true;
            panMoved = false;
            panStart = e.GetPosition(this);
            panStartMin = viewMin;
            panStartMax = viewMax;
            CaptureMouse();
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (evidence == null)
            {
                return;
            }

            Rect plot = GetPlotRect();
            Point position = e.GetPosition(this);
            if (draggedMarker != null && e.LeftButton == MouseButtonState.Pressed)
            {
                draggedMarkerX = SnapAndClamp(draggedMarker, PixelToX(position.X, plot));
                cursorPosition = new Point(
                    Math.Clamp(position.X, plot.Left, plot.Right),
                    Math.Clamp(position.Y, plot.Top, plot.Bottom));
                PublishCursorValue(plot);
                InvalidateVisual();
                e.Handled = true;
                return;
            }

            if (isPanning && e.LeftButton == MouseButtonState.Pressed)
            {
                if (Math.Abs(position.X - panStart.X) >= 4D
                    || Math.Abs(position.Y - panStart.Y) >= 4D)
                {
                    panMoved = true;
                }

                double delta = -(position.X - panStart.X) * (panStartMax - panStartMin) / Math.Max(1d, plot.Width);
                SetView(panStartMin + delta, panStartMax + delta);
                e.Handled = true;
                return;
            }

            cursorPosition = plot.Contains(position) ? position : null;
            Cursor = FindEditableMarker(position, plot) == null ? Cursors.Cross : Cursors.SizeWE;
            PublishCursorValue(plot);
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (draggedMarker != null)
            {
                VisionToolSignalMarker completedMarker = draggedMarker;
                double completedValue = draggedMarkerX;
                draggedMarker = null;
                ReleaseMouseCapture();
                Cursor = Cursors.Cross;
                MarkerValueChangeRequested(
                    this,
                    new VisionToolSignalMarkerValueChangedEventArgs(completedMarker.Id, completedValue));
                e.Handled = true;
                return;
            }

            if (!isPanning)
            {
                return;
            }

            bool shouldSelect = !panMoved;
            double selectedX = PixelToX(
                Math.Clamp(e.GetPosition(this).X, GetPlotRect().Left, GetPlotRect().Right),
                GetPlotRect());
            isPanning = false;
            panMoved = false;
            ReleaseMouseCapture();
            if (shouldSelect)
            {
                SampleSelectionRequested(
                    this,
                    new VisionToolSignalSampleSelectedEventArgs(selectedX));
            }
            e.Handled = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!isPanning && draggedMarker == null)
            {
                cursorPosition = null;
                Cursor = Cursors.Cross;
                CursorValueChanged(this, string.Empty);
                InvalidateVisual();
            }

            base.OnMouseLeave(e);
        }

        private Rect GetPlotRect()
        {
            return new Rect(
                PlotMargin.Left,
                PlotMargin.Top,
                Math.Max(0, ActualWidth - PlotMargin.Left - PlotMargin.Right),
                Math.Max(0, ActualHeight - PlotMargin.Top - PlotMargin.Bottom));
        }

        private void SetView(double requestedMin, double requestedMax)
        {
            double domainSpan = domainMax - domainMin;
            double requestedSpan = Math.Min(domainSpan, requestedMax - requestedMin);
            if (requestedMin < domainMin)
            {
                requestedMin = domainMin;
                requestedMax = domainMin + requestedSpan;
            }
            else if (requestedMax > domainMax)
            {
                requestedMax = domainMax;
                requestedMin = domainMax - requestedSpan;
            }

            viewMin = requestedMin;
            viewMax = requestedMax;
            InvalidateVisual();
        }

        private void GetVisibleYRange(out double minimum, out double maximum)
        {
            minimum = double.PositiveInfinity;
            maximum = double.NegativeInfinity;
            foreach (VisionToolSignalSeries series in evidence.Series)
            {
                for (int index = 0; index < series.Values.Count; index++)
                {
                    double x = series.XStart + (index * series.XStep);
                    if (x >= viewMin && x <= viewMax)
                    {
                        minimum = Math.Min(minimum, series.Values[index]);
                        maximum = Math.Max(maximum, series.Values[index]);
                    }
                }
            }

            if (!double.IsFinite(minimum) || !double.IsFinite(maximum))
            {
                minimum = 0;
                maximum = 1;
                return;
            }

            minimum = Math.Min(minimum, 0);
            maximum = Math.Max(maximum, 0);
            double span = maximum - minimum;
            if (span <= 0)
            {
                maximum = minimum + 1;
                return;
            }

            if (minimum < 0)
            {
                minimum -= span * 0.08d;
            }

            if (maximum > 0)
            {
                maximum += span * 0.08d;
            }
        }

        private void DrawGridAndAxes(DrawingContext drawingContext, Rect plot, double yMin, double yMax)
        {
            Pen gridPen = new Pen(new SolidColorBrush(Color.FromRgb(226, 232, 238)), 1);
            Pen axisPen = new Pen(new SolidColorBrush(Color.FromRgb(101, 116, 135)), 1);
            Brush labelBrush = new SolidColorBrush(Color.FromRgb(101, 116, 135));
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            for (int index = 0; index <= 4; index++)
            {
                double ratio = index / 4d;
                double x = plot.Left + (plot.Width * ratio);
                double y = plot.Bottom - (plot.Height * ratio);
                drawingContext.DrawLine(gridPen, new Point(x, plot.Top), new Point(x, plot.Bottom));
                drawingContext.DrawLine(gridPen, new Point(plot.Left, y), new Point(plot.Right, y));

                FormattedText xLabel = CreateText(
                    (viewMin + ((viewMax - viewMin) * ratio)).ToString("0.#", CultureInfo.CurrentCulture),
                    labelBrush,
                    10,
                    pixelsPerDip);
                drawingContext.DrawText(xLabel, new Point(x - (xLabel.Width / 2), plot.Bottom + 4));

                FormattedText yLabel = CreateText(
                    (yMin + ((yMax - yMin) * ratio)).ToString("0.##", CultureInfo.CurrentCulture),
                    labelBrush,
                    10,
                    pixelsPerDip);
                drawingContext.DrawText(yLabel, new Point(Math.Max(1, plot.Left - yLabel.Width - 6), y - (yLabel.Height / 2)));
            }

            drawingContext.DrawLine(axisPen, plot.BottomLeft, plot.BottomRight);
            drawingContext.DrawLine(axisPen, plot.TopLeft, plot.BottomLeft);
            if (yMin < 0 && yMax > 0)
            {
                double zeroRatio = (0 - yMin) / (yMax - yMin);
                double zeroY = plot.Bottom - (zeroRatio * plot.Height);
                drawingContext.DrawLine(axisPen, new Point(plot.Left, zeroY), new Point(plot.Right, zeroY));
            }
        }

        private void DrawSeries(
            DrawingContext drawingContext,
            Rect plot,
            double yMin,
            double yMax,
            VisionToolSignalSeries series)
        {
            StreamGeometry geometry = new StreamGeometry();
            bool hasPoint = false;
            using (StreamGeometryContext context = geometry.Open())
            {
                for (int index = 0; index < series.Values.Count; index++)
                {
                    double xValue = series.XStart + (index * series.XStep);
                    if (xValue < viewMin || xValue > viewMax)
                    {
                        continue;
                    }

                    Point point = new Point(
                        XToPixel(xValue, plot),
                        plot.Bottom - (Math.Clamp((series.Values[index] - yMin) / (yMax - yMin), 0, 1) * plot.Height));
                    if (!hasPoint)
                    {
                        context.BeginFigure(point, false, false);
                        hasPoint = true;
                    }
                    else
                    {
                        context.LineTo(point, true, false);
                    }
                }
            }

            if (!hasPoint)
            {
                return;
            }

            geometry.Freeze();
            Color color = (Color)ColorConverter.ConvertFromString(series.ColorHex);
            Pen seriesPen = new Pen(new SolidColorBrush(color), 1.6);
            seriesPen.Freeze();
            drawingContext.DrawGeometry(null, seriesPen, geometry);
        }

        private void DrawMarkers(DrawingContext drawingContext, Rect plot)
        {
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            foreach (VisionToolSignalMarker marker in evidence.Markers.Concat(advisoryMarkers))
            {
                double markerX = ReferenceEquals(marker, draggedMarker) ? draggedMarkerX : marker.X;
                if (markerX < viewMin || markerX > viewMax)
                {
                    continue;
                }

                Color color = (Color)ColorConverter.ConvertFromString(marker.ColorHex);
                Pen markerPen = new Pen(new SolidColorBrush(color), marker.IsEditable ? 2 : 1.4)
                {
                    DashStyle = marker.IsEditable ? DashStyles.Solid : DashStyles.Dash
                };
                double pixelX = XToPixel(markerX, plot);
                drawingContext.DrawLine(markerPen, new Point(pixelX, plot.Top), new Point(pixelX, plot.Bottom));

                string label = string.Format(CultureInfo.CurrentCulture, "{0} {1:0.#}", marker.Name, markerX);
                FormattedText markerText = CreateText(label, new SolidColorBrush(color), 10, pixelsPerDip);
                double textX = Math.Clamp(pixelX + 3, plot.Left + 2, plot.Right - markerText.Width - 3);
                Rect labelBackground = new Rect(
                    textX - 2,
                    plot.Top + 2,
                    markerText.Width + 4,
                    markerText.Height + 2);
                drawingContext.DrawRoundedRectangle(
                    new SolidColorBrush(Color.FromArgb(225, 255, 255, 255)),
                    null,
                    labelBackground,
                    2,
                    2);
                drawingContext.DrawText(markerText, new Point(textX, plot.Top + 3));
            }
        }

        private void DrawCursor(DrawingContext drawingContext, Rect plot)
        {
            if (!cursorPosition.HasValue || !plot.Contains(cursorPosition.Value))
            {
                return;
            }

            Pen cursorPen = new Pen(new SolidColorBrush(Color.FromArgb(180, 36, 48, 64)), 1)
            {
                DashStyle = DashStyles.Dash
            };
            double x = cursorPosition.Value.X;
            drawingContext.DrawLine(cursorPen, new Point(x, plot.Top), new Point(x, plot.Bottom));
        }

        private void DrawSelection(DrawingContext drawingContext, Rect plot)
        {
            if (!selectionX.HasValue
                || selectionX.Value < viewMin
                || selectionX.Value > viewMax)
            {
                return;
            }

            Pen selectionPen = new Pen(
                new SolidColorBrush(Color.FromRgb(192, 57, 43)),
                2);
            double x = XToPixel(selectionX.Value, plot);
            drawingContext.DrawLine(selectionPen, new Point(x, plot.Top), new Point(x, plot.Bottom));
        }

        private void PublishCursorValue(Rect plot)
        {
            if (!cursorPosition.HasValue || evidence == null)
            {
                CursorValueChanged(this, string.Empty);
                return;
            }

            double x = PixelToX(cursorPosition.Value.X, plot);
            string[] values = evidence.Series
                .Select(series =>
                {
                    int index = (int)Math.Round((x - series.XStart) / series.XStep);
                    index = Math.Clamp(index, 0, series.Values.Count - 1);
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "{0} {1:0.###}",
                        series.Name,
                        series.Values[index]);
                })
                .ToArray();
            CursorValueChanged(
                this,
                string.Format(CultureInfo.CurrentCulture, "{0} {1:0.#}  |  {2}", evidence.XAxisLabel, x, string.Join("  |  ", values)));
        }

        private double XToPixel(double x, Rect plot)
        {
            return plot.Left + (((x - viewMin) / (viewMax - viewMin)) * plot.Width);
        }

        private double PixelToX(double pixel, Rect plot)
        {
            return viewMin + (((pixel - plot.Left) / plot.Width) * (viewMax - viewMin));
        }

        private VisionToolSignalMarker FindEditableMarker(Point position, Rect plot)
        {
            if (evidence == null || !plot.Contains(position))
            {
                return null;
            }

            return evidence.Markers
                .Where(marker => marker.IsEditable && marker.X >= viewMin && marker.X <= viewMax)
                .OrderBy(marker => Math.Abs(XToPixel(marker.X, plot) - position.X))
                .FirstOrDefault(marker => Math.Abs(XToPixel(marker.X, plot) - position.X) <= 7d);
        }

        private double SnapAndClamp(VisionToolSignalMarker marker, double value)
        {
            double snapped = Math.Round(value / marker.SnapStep) * marker.SnapStep;
            return Math.Clamp(snapped, domainMin, domainMax);
        }

        private static FormattedText CreateText(string value, Brush brush, double size, double pixelsPerDip)
        {
            return new FormattedText(
                value,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                size,
                brush,
                pixelsPerDip);
        }
    }
}
