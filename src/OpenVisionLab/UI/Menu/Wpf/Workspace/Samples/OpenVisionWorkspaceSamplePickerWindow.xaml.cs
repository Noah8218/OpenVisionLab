using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace OpenVisionLab
{
    internal partial class OpenVisionWorkspaceSamplePickerWindow : Window
    {
        private HwndSource windowSource;

        public OpenVisionWorkspaceSamplePickerWindow(OpenVisionWorkspaceSamplePickerViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.SelectionAccepted += OnSelectionAccepted;
            samplePickerTitleBar.TitleText = ViewModel.DialogTitleText;
            samplePickerTitleBar.IconKind = PackIconMaterialKind.ImageMultipleOutline;
        }

        public OpenVisionWorkspaceSamplePickerViewModel ViewModel { get; }

        public VisionPipelineSampleCatalogItem SelectedSample => ViewModel.SelectedSample;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            windowSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            windowSource?.AddHook(WindowProc);
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel.SelectionAccepted -= OnSelectionAccepted;
            windowSource?.RemoveHook(WindowProc);
            windowSource = null;
            base.OnClosed(e);
        }

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

        private void OnSelectionAccepted(object sender, EventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static IntPtr WindowProc(
            IntPtr hwnd,
            int message,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            if (message == OpenVisionWindowWorkArea.GetMinMaxInfoMessage)
            {
                OpenVisionWindowWorkArea.Apply(hwnd, lParam);
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
