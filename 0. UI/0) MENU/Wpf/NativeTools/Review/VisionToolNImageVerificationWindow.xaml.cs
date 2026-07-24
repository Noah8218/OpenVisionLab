using System;
using System.Windows;

namespace OpenVisionLab
{
    public partial class VisionToolNImageVerificationWindow : Window
    {
        private readonly VisionToolNImageVerificationController controller;

        internal VisionToolNImageVerificationWindow(VisionToolNImageVerificationController controller)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            DataContext = this.controller;
            Closed += OnClosed;
        }

        private void AddFilesButton_Click(object sender, RoutedEventArgs e)
        {
            controller.AddFiles(this);
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            controller.AddFolder(this);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            controller.ClearImages();
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            await controller.RunAsync();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            controller.Stop();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            controller.ExportHtml(this);
        }

        private void PromoteLocatorValidationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!controller.CanPromoteLocatorValidation)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show(
                this,
                controller.PromotionConfirmationText,
                controller.PromoteLocatorValidationText,
                MessageBoxButton.YesNo,
                MessageBoxImage.Information,
                MessageBoxResult.No);
            if (confirmation == MessageBoxResult.Yes)
            {
                controller.PromoteLocatorValidation();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Closed -= OnClosed;
            controller.Dispose();
        }
    }
}
