using MahApps.Metro.IconPacks;
using OpenVisionLab._1._Core;
using System.Windows;

namespace OpenVisionLab
{
    public partial class OpenVisionShellHostWindow : Window
    {
        public OpenVisionShellHostWindow()
            : this(ApplicationRuntimeContext.CreateDefault())
        {
        }

        public OpenVisionShellHostWindow(ApplicationRuntimeContext runtimeContext)
        {
            InitializeComponent();
            shellTitleBar.TitleText = "OpenVisionLab";
            shellTitleBar.IconKind = PackIconMaterialKind.ImageFilterCenterFocus;
            contentHost.Content = new OpenVisionShellHostView(runtimeContext);
        }

        public OpenVisionShellHostView ShellHostForSmoke => contentHost.Content as OpenVisionShellHostView;
    }
}
