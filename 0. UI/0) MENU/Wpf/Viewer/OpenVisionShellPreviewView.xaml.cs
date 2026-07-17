using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class OpenVisionShellPreviewView : UserControl
    {
        public const string ShellMode = "WpfShellPreview";

        public OpenVisionShellPreviewView()
        {
            InitializeComponent();
            DataContext = OpenVisionShellPreviewViewModel.CreatePreview();
            Unloaded += OpenVisionShellPreviewView_Unloaded;
        }

        private void OpenVisionShellPreviewView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is System.IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
