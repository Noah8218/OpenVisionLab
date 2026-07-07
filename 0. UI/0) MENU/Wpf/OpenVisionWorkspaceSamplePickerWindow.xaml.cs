using System.Collections.Generic;
using System.Windows;

namespace OpenVisionLab
{
    internal partial class OpenVisionWorkspaceSamplePickerWindow : Window
    {
        public OpenVisionWorkspaceSamplePickerWindow(OpenVisionWorkspaceSamplePickerViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }

        public OpenVisionWorkspaceSamplePickerViewModel ViewModel { get; }

        public VisionPipelineSampleCatalogItem SelectedSample => ViewModel.SelectedSample;

        public static bool TrySelectSample(
            Window owner,
            IEnumerable<VisionPipelineSampleCatalogItem> samples,
            out VisionPipelineSampleCatalogItem sample)
        {
            return TrySelectSample(owner, samples, null, out sample);
        }

        public static bool TrySelectSample(
            Window owner,
            IEnumerable<VisionPipelineSampleCatalogItem> samples,
            string preferredLearnPathId,
            out VisionPipelineSampleCatalogItem sample)
        {
            OpenVisionWorkspaceSamplePickerViewModel viewModel = new OpenVisionWorkspaceSamplePickerViewModel(samples, preferredLearnPathId);
            if (!viewModel.HasSamples)
            {
                sample = null;
                return false;
            }

            OpenVisionWorkspaceSamplePickerWindow window = new OpenVisionWorkspaceSamplePickerWindow(viewModel);
            if (owner != null)
            {
                window.Owner = owner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            bool accepted = window.ShowDialog() == true && viewModel.CanSelect;
            sample = accepted ? viewModel.SelectedSample : null;
            return accepted;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            AcceptSelectedSample();
        }

        private void OpenGuideAndSelect_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.CanOpenLearnAndSample)
            {
                return;
            }

            ViewModel.OpenLearnDocumentForSelection();
            AcceptSelectedSample();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AcceptSelectedSample()
        {
            if (!ViewModel.CanSelect)
            {
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
