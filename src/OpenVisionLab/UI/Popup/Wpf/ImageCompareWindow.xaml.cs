using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed partial class ImageCompareWindow : Window, IDisposable
    {
        private readonly ImageCompareViewModel viewModel;
        private bool disposed;

        public ImageCompareWindow()
        {
            viewModel = new ImageCompareViewModel();
            InitializeComponent();
            DataContext = viewModel;
            Title = viewModel.TitleText;
            Closed += OnClosed;
            StateChanged += OnStateChanged;
        }

        public void LoadImages(params string[] imagePaths)
        {
            viewModel.LoadImages(imagePaths);
        }

        private void LoadImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = ImageCompareViewModel.ImageFilter,
                Multiselect = true,
                InitialDirectory = viewModel.InitialImageDirectory
            };

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            LoadImages(dialog.FileNames);
        }

        private void FitAll_Click(object sender, RoutedEventArgs e)
        {
            viewModel.FitAll();
        }

        private void ToggleSync_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SyncViewEnabled = !viewModel.SyncViewEnabled;
        }

        private void SlotImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ImageCompareSlotViewModel slot)
            {
                viewModel.SelectSlot(slot);
            }
        }

        private void SlotImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not Image image || image.DataContext is not ImageCompareSlotViewModel slot)
            {
                return;
            }

            Point point = e.GetPosition(image);
            if (ImageCompareViewModel.TryMapDisplayedPoint(
                slot.Source?.PixelWidth ?? 0,
                slot.Source?.PixelHeight ?? 0,
                image.ActualWidth,
                image.ActualHeight,
                point.X,
                point.Y,
                out int x,
                out int y))
            {
                viewModel.UpdatePixelStatus(slot, x, y);
            }
            else
            {
                viewModel.ResetStatus();
            }
        }

        private void SlotImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ImageCompareSlotViewModel slot)
            {
                viewModel.ApplyZoom(slot, e.Delta);
                e.Handled = true;
            }
        }

        private void SlotImage_MouseLeave(object sender, MouseEventArgs e)
        {
            viewModel.ResetStatus();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleWindowState();
                return;
            }

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            ToggleWindowState();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToggleWindowState()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            maximizeIcon.Kind = WindowState == WindowState.Maximized
                ? PackIconMaterialKind.WindowRestore
                : PackIconMaterialKind.WindowMaximize;
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            Closed -= OnClosed;
            StateChanged -= OnStateChanged;
            DataContext = null;
            viewModel.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }

    }
}
