using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab._1._Core
{
    public interface IDisplayHostBinder
    {
        void SetForm(Form form);
        void SetDockPanel(DockPanel dockPanel);
    }
}
