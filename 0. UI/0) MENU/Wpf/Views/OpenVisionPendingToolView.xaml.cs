using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class OpenVisionPendingToolView : UserControl
    {
        public OpenVisionPendingToolView(OpenVisionPendingToolViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = viewModel;
        }

        public OpenVisionPendingToolViewModel ViewModel { get; }
    }
}
