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
        private readonly ElementHost logHost;

        public FormLogViewer()
        {
            Text = "Log";
            TabText = "Log";
            MinimumSize = new Size(420, 520);
            BackColor = Color.FromArgb(18, 22, 29);
            CloseButton = false;
            CloseButtonVisible = false;
            HideOnClose = true;

            logHost = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = new LogPanelView()
            };

            Controls.Add(logHost);
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
