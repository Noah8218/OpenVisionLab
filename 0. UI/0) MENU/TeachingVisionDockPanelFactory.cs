using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OpenVisionLab
{
    internal static class TeachingVisionDockPanelFactory
    {
        public static DockPanel Create(Control hostControl, Font font)
        {
            DockPanel dockPanel = new DockPanel
            {
                Theme = new VS2015DarkTheme(),
                Dock = DockStyle.Fill,
                DocumentStyle = DocumentStyle.DockingWindow
            };

            dockPanel.Theme.Skin.DockPaneStripSkin.TextFont = font;
            dockPanel.Theme.Skin.AutoHideStripSkin.TextFont = font;

            hostControl.Controls.Add(dockPanel);
            return dockPanel;
        }
    }
}
