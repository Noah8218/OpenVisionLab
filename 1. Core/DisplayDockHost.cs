using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab._1._Core
{
    internal sealed class DisplayDockHost : IDisplayHost
    {
        private Form owner;

        public DockPanel DockPanel { get; private set; }

        public string ActiveDocumentTitle => DockPanel?.ActiveDocument?.DockHandler?.TabText;

        public void SetOwner(Form form)
        {
            owner = form;
        }

        public void SetDockPanel(DockPanel dockPanel)
        {
            DockPanel = dockPanel;
        }

        public void InvokeOnUiThread(Action action)
        {
            if (action == null) return;

            if (owner == null || owner.IsDisposed)
            {
                action();
                return;
            }

            if (owner.InvokeRequired)
            {
                owner.Invoke((MethodInvoker)delegate { action(); });
                return;
            }

            action();
        }
    }
}
