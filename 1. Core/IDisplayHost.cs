using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab._1._Core
{
    internal interface IDisplayHost
    {
        DockPanel DockPanel { get; }
        string ActiveDocumentTitle { get; }

        void SetOwner(Form form);
        void SetDockPanel(DockPanel dockPanel);
        void InvokeOnUiThread(Action action);
    }
}
