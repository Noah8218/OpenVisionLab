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
            if (ViewModel.IsImageOnly) { Width = 820; Height = 620; MinWidth = 680; MinHeight = 560; }
            ViewModel.SelectionAccepted += OnSelectionAccepted;
            samplePickerTitleBar.TitleText = ViewModel.DialogTitleText;
            samplePickerTitleBar.IconKind = PackIconMaterialKind.ImageMultipleOutline;
        }

        public OpenVisionWorkspaceSamplePickerViewModel ViewModel { get; }

        public VisionPipelineSampleCatalogItem SelectedSample => ViewModel.SelectedSample;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (Owner != null) { SetBinding(OpenVisionWorkbenchThemeBehavior.ThemeNameProperty, new System.Windows.Data.Binding { Source = Owner, Path = new PropertyPath("(0)", OpenVisionWorkbenchThemeBehavior.ThemeNameProperty) }); }
            windowSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            windowSource?.AddHook(WindowProc);
        }

        protected override void OnClosed(EventArgs e)
        {
            System.Windows.Data.BindingOperations.ClearBinding(this, OpenVisionWorkbenchThemeBehavior.ThemeNameProperty);
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
            out VisionPipelineSampleCatalogItem sample,
            bool imageOnly = false)
        {
            OpenVisionWorkspaceSamplePickerViewModel viewModel = new OpenVisionWorkspaceSamplePickerViewModel(samples, preferredLearnPathId, imageOnly);
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

        public static bool TrySelectImages(Window owner, IEnumerable<VisionPipelineSampleCatalogItem> samples, out IReadOnlyList<VisionPipelineSampleCatalogItem> selection, out bool useCatalogLabels)
        {
            var viewModel = new OpenVisionWorkspaceSamplePickerViewModel(samples, null, true);
            var window = new OpenVisionWorkspaceSamplePickerWindow(viewModel);
            if (owner != null) { window.Owner = owner; }
            bool accepted = viewModel.HasSamples && window.ShowDialog() == true && viewModel.CanSelect;
            selection = accepted ? viewModel.SelectedImageSamples : Array.Empty<VisionPipelineSampleCatalogItem>();
            useCatalogLabels = accepted && viewModel.UseCatalogLabels;
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
