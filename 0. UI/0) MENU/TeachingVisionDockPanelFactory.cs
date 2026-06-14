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

            ApplyWorkbenchSkin(dockPanel, font);

            hostControl.Controls.Add(dockPanel);
            return dockPanel;
        }

        private static void ApplyWorkbenchSkin(DockPanel dockPanel, Font font)
        {
            Font tabFont = new Font(font.FontFamily, 9F, FontStyle.Regular, GraphicsUnit.Point);
            Font activeTabFont = new Font(font.FontFamily, 9F, FontStyle.Bold, GraphicsUnit.Point);

            dockPanel.BackColor = Color.FromArgb(11, 15, 21);
            dockPanel.Theme.Skin.DockPaneStripSkin.TextFont = tabFont;
            dockPanel.Theme.Skin.AutoHideStripSkin.TextFont = tabFont;

            dockPanel.Theme.Skin.AutoHideStripSkin.DockStripGradient.StartColor = Color.FromArgb(31, 38, 55);
            dockPanel.Theme.Skin.AutoHideStripSkin.DockStripGradient.EndColor = Color.FromArgb(31, 38, 55);
            dockPanel.Theme.Skin.AutoHideStripSkin.TabGradient.StartColor = Color.FromArgb(36, 45, 64);
            dockPanel.Theme.Skin.AutoHideStripSkin.TabGradient.EndColor = Color.FromArgb(36, 45, 64);
            dockPanel.Theme.Skin.AutoHideStripSkin.TabGradient.TextColor = Color.FromArgb(224, 234, 248);

            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.StartColor = Color.FromArgb(13, 18, 25);
            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.EndColor = Color.FromArgb(13, 18, 25);
            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.StartColor = Color.FromArgb(22, 130, 192);
            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.EndColor = Color.FromArgb(22, 130, 192);
            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.TextColor = Color.White;
            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.StartColor = Color.FromArgb(32, 39, 56);
            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.EndColor = Color.FromArgb(32, 39, 56);
            dockPanel.Theme.Skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.TextColor = Color.FromArgb(202, 216, 235);

            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.StartColor = Color.FromArgb(18, 24, 34);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.EndColor = Color.FromArgb(18, 24, 34);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor = Color.FromArgb(46, 67, 101);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor = Color.FromArgb(46, 67, 101);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor = Color.FromArgb(246, 251, 255);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor = Color.FromArgb(32, 39, 56);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor = Color.FromArgb(32, 39, 56);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor = Color.FromArgb(189, 202, 220);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.StartColor = Color.FromArgb(42, 73, 107);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.EndColor = Color.FromArgb(42, 73, 107);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.TextColor = Color.White;
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.StartColor = Color.FromArgb(31, 38, 55);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.EndColor = Color.FromArgb(31, 38, 55);
            dockPanel.Theme.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.TextColor = Color.FromArgb(202, 216, 235);
        }
    }
}
