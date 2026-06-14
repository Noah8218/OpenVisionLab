using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineImageViewer : Form
    {
        private readonly Bitmap sourceImage;
        private readonly List<VisionToolOverlay> overlays;
        private readonly VisionPipelineStepResultSummary summary;
        private PipelineOverlayImageCanvas canvas;
        private ComboBox cbLabelMode;
        private ComboBox cbOverlayTone;
        private CheckBox chkBoxes;
        private NumericUpDown nudPointLimit;
        private NumericUpDown nudStrokeWidth;
        private DataGridView overlayGrid;
        private TextBox tbOverlayDetail;
        private Label statusLabel;
        private bool isSelectingOverlay;

        public FormVisionPipelineImageViewer()
            : this(
                  "Designer Preview",
                  new Bitmap(320, 240),
                  Enumerable.Empty<VisionToolOverlay>(),
                  null,
                  FormVision_Pipeline.OverlayLabelMode.Number,
                  300)
        {
        }

        public FormVisionPipelineImageViewer(
            string title,
            Bitmap image,
            IEnumerable<VisionToolOverlay> overlays,
            VisionPipelineStepResultSummary summary,
            FormVision_Pipeline.OverlayLabelMode labelMode,
            int pointLimit,
            int initialOverlayIndex = -1)
        {
            sourceImage = image == null ? new Bitmap(16, 16) : new Bitmap(image);
            this.overlays = (overlays ?? Enumerable.Empty<VisionToolOverlay>())
                .Where(item => item != null)
                .Select(CloneOverlay)
                .ToList();
            this.summary = summary;

            InitializeComponent();
            canvas.SetContent(sourceImage, this.overlays);
            Text = string.IsNullOrWhiteSpace(title) ? "Pipeline Preview" : $"Pipeline Preview - {title}";
            cbLabelMode.SelectedIndex = Math.Max(0, Math.Min(2, (int)labelMode));
            nudPointLimit.Value = Math.Max(0, Math.Min(5000, pointLimit));
            OnOptionChanged(this, EventArgs.Empty);
            PopulateOverlayGrid();

            Shown += (sender, e) =>
            {
                canvas.FitToWindow();
                if (initialOverlayIndex >= 0)
                {
                    SelectOverlay(initialOverlayIndex, true);
                }

                UpdateStatus();
            };
            UpdateStatus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                sourceImage?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnOptionChanged(object sender, EventArgs e)
        {
            canvas.ShowBoxes = chkBoxes.Checked;
            canvas.LabelMode = GetCanvasLabelMode();
            canvas.PointLimit = (int)nudPointLimit.Value;
            canvas.OverlayColor = ResolveOverlayColor();
            canvas.StrokeWidth = (float)nudStrokeWidth.Value;
            canvas.Invalidate();
        }

        private FormVision_Pipeline.OverlayLabelMode GetLabelMode()
        {
            switch (cbLabelMode.SelectedIndex)
            {
                case 0:
                    return FormVision_Pipeline.OverlayLabelMode.None;
                case 2:
                    return FormVision_Pipeline.OverlayLabelMode.Details;
                default:
                    return FormVision_Pipeline.OverlayLabelMode.Number;
            }
        }

        private PipelineOverlayLabelMode GetCanvasLabelMode()
        {
            switch (cbLabelMode.SelectedIndex)
            {
                case 0:
                    return PipelineOverlayLabelMode.None;
                case 2:
                    return PipelineOverlayLabelMode.Details;
                default:
                    return PipelineOverlayLabelMode.Number;
            }
        }

        private void OnFitClicked(object sender, EventArgs e)
        {
            canvas.FitToWindow();
            canvas.Focus();
        }

        private void OnActualClicked(object sender, EventArgs e)
        {
            canvas.SetZoom(1F);
            canvas.Focus();
        }

        private void OnDoubleClicked(object sender, EventArgs e)
        {
            canvas.SetZoom(2F);
            canvas.Focus();
        }

        private void OnViewerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F)
            {
                canvas.FitToWindow();
            }
            else if (e.KeyCode == Keys.D1)
            {
                canvas.SetZoom(1F);
            }
            else if (e.KeyCode == Keys.Up)
            {
                SelectRelativeOverlay(-1);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                SelectRelativeOverlay(1);
                e.Handled = true;
            }
        }

        private void SelectRelativeOverlay(int delta)
        {
            if (overlays.Count == 0)
            {
                return;
            }

            int currentIndex = canvas.SelectedOverlayIndex < 0 ? 0 : canvas.SelectedOverlayIndex;
            int nextIndex = Math.Max(0, Math.Min(overlays.Count - 1, currentIndex + delta));
            SelectOverlay(nextIndex, true);
        }

        private void UpdateStatus()
        {
            string metrics = summary == null
                ? string.Empty
                : $"Status={summary.Status} | {summary.MetricsText}";
            string selectedText = canvas.SelectedOverlayIndex >= 0
                ? $" | Selected {canvas.SelectedOverlayIndex + 1}/{overlays.Count}"
                : string.Empty;
            statusLabel.Text = string.Format(
                CultureInfo.InvariantCulture,
                "Image {0} x {1} | Zoom {2:0}% | Overlays {3}{4}{5}",
                sourceImage.Width,
                sourceImage.Height,
                canvas.Zoom * 100F,
                overlays.Count,
                selectedText,
                string.IsNullOrWhiteSpace(metrics) ? string.Empty : " | " + metrics);
        }

        private void PopulateOverlayGrid()
        {
            overlayGrid.Rows.Clear();
            for (int i = 0; i < overlays.Count; i++)
            {
                VisionToolOverlay overlay = overlays[i];
                int rowIndex = overlayGrid.Rows.Add(
                    (i + 1).ToString(CultureInfo.InvariantCulture),
                    overlay.Kind.ToString(),
                    FormatOverlayArea(overlay),
                    FormatOverlayCenter(overlay));
                overlayGrid.Rows[rowIndex].Tag = i;
            }

            if (overlayGrid.Rows.Count > 0)
            {
                overlayGrid.Rows[0].Selected = true;
                overlayGrid.CurrentCell = overlayGrid.Rows[0].Cells[0];
                SelectOverlay(0, false);
            }
            else
            {
                tbOverlayDetail.Text = "No overlay data.";
            }
        }

        private static string FormatOverlayArea(VisionToolOverlay overlay)
        {
            if (overlay == null || overlay.Bounds.Width <= 0 || overlay.Bounds.Height <= 0)
            {
                return "-";
            }

            return (overlay.Bounds.Width * overlay.Bounds.Height).ToString("0.#", CultureInfo.InvariantCulture);
        }

        private static string FormatOverlayCenter(VisionToolOverlay overlay)
        {
            if (overlay == null)
            {
                return "-";
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:0},{1:0}", overlay.Center.X, overlay.Center.Y);
        }

        private void OnOverlayGridSelectionChanged(object sender, EventArgs e)
        {
            if (isSelectingOverlay || overlayGrid.CurrentRow?.Tag == null)
            {
                return;
            }

            SelectOverlay((int)overlayGrid.CurrentRow.Tag, true);
        }

        private void OnCanvasOverlaySelected(int overlayIndex)
        {
            SelectOverlay(overlayIndex, false);
        }

        private void OnCanvasViewChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void SelectOverlay(int overlayIndex, bool focusCanvas)
        {
            if (overlayIndex < 0 || overlayIndex >= overlays.Count)
            {
                return;
            }

            isSelectingOverlay = true;
            try
            {
                foreach (DataGridViewRow row in overlayGrid.Rows)
                {
                    if (row.Tag is int index && index == overlayIndex)
                    {
                        row.Selected = true;
                        overlayGrid.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }
            finally
            {
                isSelectingOverlay = false;
            }

            VisionToolOverlay overlay = overlays[overlayIndex];
            canvas.SelectedOverlayIndex = overlayIndex;
            tbOverlayDetail.Text = FormatOverlayDetail(overlayIndex, overlay);
            if (focusCanvas)
            {
                canvas.FocusOverlay(overlayIndex);
            }
            else
            {
                canvas.Invalidate();
            }

            UpdateStatus();
        }

        private static string FormatOverlayDetail(int overlayIndex, VisionToolOverlay overlay)
        {
            if (overlay == null)
            {
                return string.Empty;
            }

            RectangleF bounds = overlay.Bounds;
            return string.Format(
                CultureInfo.InvariantCulture,
                "No        : {0}\r\nKind      : {1}\r\nLabel     : {2}\r\nX/Y       : {3:0.#}, {4:0.#}\r\nW/H       : {5:0.#}, {6:0.#}\r\nCenter    : {7:0.#}, {8:0.#}\r\nAngle     : {9:0.###}\r\nPoints    : {10}",
                overlayIndex + 1,
                overlay.Kind,
                string.IsNullOrWhiteSpace(overlay.Label) ? "-" : overlay.Label,
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height,
                overlay.Center.X,
                overlay.Center.Y,
                overlay.Angle,
                overlay.Points?.Count ?? 0);
        }

        private Color ResolveOverlayColor()
        {
            string text = cbOverlayTone?.SelectedItem?.ToString() ?? "Green";
            switch (text)
            {
                case "Cyan":
                    return Color.FromArgb(20, 185, 235);
                case "Orange":
                    return Color.FromArgb(238, 145, 32);
                case "Magenta":
                    return Color.FromArgb(214, 86, 214);
                default:
                    return Color.FromArgb(0, 210, 120);
            }
        }

        private static VisionToolOverlay CloneOverlay(VisionToolOverlay overlay)
        {
            VisionToolOverlay clone = new VisionToolOverlay
            {
                Kind = overlay.Kind,
                Label = overlay.Label,
                Bounds = overlay.Bounds,
                Center = overlay.Center,
                Start = overlay.Start,
                End = overlay.End,
                Angle = overlay.Angle
            };
            clone.Points.AddRange(overlay.Points);
            return clone;
        }
    }
}
