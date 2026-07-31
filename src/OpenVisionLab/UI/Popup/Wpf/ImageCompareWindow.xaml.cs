using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    public sealed partial class ImageCompareWindow : Window, IDisposable
    {
        private static string lastImageCompareDirectory;
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
            RememberImageDirectory(imagePaths);
        }

        private void LoadImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = ImageCompareViewModel.ImageFilter,
                Multiselect = true,
                InitialDirectory = ResolveInitialImageDirectory()
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
            if (TryMapImagePoint(image, slot, point, out int x, out int y))
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

        private static bool TryMapImagePoint(Image image, ImageCompareSlotViewModel slot, Point point, out int x, out int y)
        {
            x = -1;
            y = -1;
            if (slot?.Source == null || slot.Source.PixelWidth <= 0 || slot.Source.PixelHeight <= 0)
            {
                return false;
            }

            double hostWidth = image.ActualWidth;
            double hostHeight = image.ActualHeight;
            if (hostWidth <= 0 || hostHeight <= 0)
            {
                return false;
            }

            BitmapSource source = slot.Source;
            double scale = Math.Min(hostWidth / source.PixelWidth, hostHeight / source.PixelHeight);
            if (scale <= 0 || double.IsInfinity(scale) || double.IsNaN(scale))
            {
                return false;
            }

            double displayWidth = source.PixelWidth * scale;
            double displayHeight = source.PixelHeight * scale;
            double offsetX = (hostWidth - displayWidth) / 2.0;
            double offsetY = (hostHeight - displayHeight) / 2.0;
            double imageX = (point.X - offsetX) / scale;
            double imageY = (point.Y - offsetY) / scale;

            if (imageX < 0 || imageY < 0 || imageX >= source.PixelWidth || imageY >= source.PixelHeight)
            {
                return false;
            }

            x = Math.Max(0, Math.Min(source.PixelWidth - 1, (int)Math.Floor(imageX)));
            y = Math.Max(0, Math.Min(source.PixelHeight - 1, (int)Math.Floor(imageY)));
            return true;
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

        private static string ResolveInitialImageDirectory()
        {
            if (!string.IsNullOrWhiteSpace(lastImageCompareDirectory) && Directory.Exists(lastImageCompareDirectory))
            {
                return lastImageCompareDirectory;
            }

            string persistedDirectory = LoadPersistedImageDirectory();
            if (!string.IsNullOrWhiteSpace(persistedDirectory) && Directory.Exists(persistedDirectory))
            {
                lastImageCompareDirectory = persistedDirectory;
                return persistedDirectory;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        private static void RememberImageDirectory(string[] imagePaths)
        {
            string firstPath = imagePaths == null ? string.Empty : Array.Find(imagePaths, File.Exists);
            if (string.IsNullOrWhiteSpace(firstPath))
            {
                return;
            }

            string directory = Path.GetDirectoryName(firstPath);
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return;
            }

            lastImageCompareDirectory = directory;
            SavePersistedImageDirectory(directory);
        }

        private static string LoadPersistedImageDirectory()
        {
            string path = GetPersistedImageDirectoryPath();
            try
            {
                return File.Exists(path) ? File.ReadAllText(path).Trim() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void SavePersistedImageDirectory(string directory)
        {
            try
            {
                string path = GetPersistedImageDirectoryPath();
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, directory ?? string.Empty);
            }
            catch
            {
            }
        }

        private static string GetPersistedImageDirectoryPath()
        {
            string configuredRoot = Environment.GetEnvironmentVariable(
                "OPENVISIONLAB_DATA_ROOT");
            string dataRoot = !string.IsNullOrWhiteSpace(configuredRoot)
                && Path.IsPathRooted(
                    Environment.ExpandEnvironmentVariables(
                        configuredRoot.Trim().Trim('"')))
                ? Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(
                        configuredRoot.Trim().Trim('"')))
                : Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "OpenVisionLab");
            return Path.Combine(
                dataRoot,
                "CONFIG",
                "image_compare_last_directory.txt");
        }
    }
}
