using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab
{
    internal sealed class WorkbenchDockPaneCaptionFactory : DockPanelExtender.IDockPaneCaptionFactory
    {
        private readonly Func<bool> isLogCaptionVisible;

        public WorkbenchDockPaneCaptionFactory(Func<bool> isLogCaptionVisible)
        {
            this.isLogCaptionVisible = isLogCaptionVisible ?? (() => true);
        }

        public DockPaneCaptionBase CreateDockPaneCaption(DockPane pane)
        {
            return new WorkbenchDockPaneCaption(pane, isLogCaptionVisible);
        }
    }

    internal sealed class WorkbenchDockPaneCaption : DockPaneCaptionBase
    {
        private readonly Func<bool> isLogCaptionVisible;

        public WorkbenchDockPaneCaption(DockPane pane, Func<bool> isLogCaptionVisible)
            : base(pane)
        {
            this.isLogCaptionVisible = isLogCaptionVisible ?? (() => true);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        protected override int MeasureHeight()
        {
            return ShouldHideCaption() ? 0 : 26;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ShouldHideCaption())
            {
                return;
            }

            base.OnPaint(e);

            Rectangle bounds = ClientRectangle;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            bool active = DockPane != null && DockPane.IsActivePane;
            Color backColor = active
                ? Color.FromArgb(46, 67, 101)
                : Color.FromArgb(32, 39, 56);
            Color accentColor = active
                ? Color.FromArgb(116, 188, 232)
                : Color.FromArgb(72, 86, 112);
            Color borderColor = active
                ? Color.FromArgb(82, 118, 162)
                : Color.FromArgb(45, 55, 75);
            Color textColor = active
                ? Color.FromArgb(246, 251, 255)
                : Color.FromArgb(189, 202, 220);

            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, bounds);
            }

            using (SolidBrush accentBrush = new SolidBrush(accentColor))
            {
                e.Graphics.FillRectangle(accentBrush, bounds.Left, bounds.Top, 4, bounds.Height);
            }

            using (Pen pen = new Pen(borderColor))
            {
                e.Graphics.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            Rectangle textBounds = new Rectangle(bounds.Left + 12, bounds.Top, Math.Max(0, bounds.Width - 24), bounds.Height);
            string text = DockPane?.CaptionText ?? string.Empty;
            TextRenderer.DrawText(
                e.Graphics,
                text,
                Font,
                textBounds,
                textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private bool ShouldHideCaption()
        {
            return IsLogPane() && !isLogCaptionVisible();
        }

        private bool IsLogPane()
        {
            if (DockPane == null)
            {
                return false;
            }

            foreach (IDockContent content in DockPane.Contents)
            {
                if (content is FormLogViewer)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
