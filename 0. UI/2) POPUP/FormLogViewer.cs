using OpenVisionLab.Logging.Controls.View;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab
{
    public sealed class FormLogViewer : DockContent
    {
        private const string DockTitle = "실행 로그";
        private readonly ElementHost logHost;

        public FormLogViewer()
        {
            Text = DockTitle;
            TabText = DockTitle;
            MinimumSize = new Size(420, 90);
            BackColor = Color.FromArgb(18, 22, 29);
            CloseButton = false;
            CloseButtonVisible = false;
            HideOnClose = true;
            ToolTipText = DockTitle;

            logHost = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = new LogPanelView()
            };

            Controls.Add(logHost);
        }

        public void SetDockTitleVisible(bool visible)
        {
            Text = DockTitle;
            TabText = DockTitle;
            ToolTipText = DockTitle;
        }

        protected override string GetPersistString()
        {
            return typeof(FormLogViewer).FullName;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                logHost?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
