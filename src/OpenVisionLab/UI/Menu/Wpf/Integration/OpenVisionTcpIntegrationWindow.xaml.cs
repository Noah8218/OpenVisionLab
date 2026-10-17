using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;

namespace OpenVisionLab
{
    public partial class OpenVisionTcpIntegrationWindow : Window
    {
        private readonly OpenVisionTcpIntegrationController controller;
        private string sourceImageDisplayError = string.Empty;

        internal OpenVisionTcpIntegrationWindow(OpenVisionTcpIntegrationController controller)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            DataContext = controller;
            controller.PropertyChanged += OnControllerPropertyChanged;
            Closing += OnClosing;
            Closed += OnClosed;
            RefreshSourceImageViewer();
        }

        internal bool HasSourceImageForSmoke => sourceImageViewer?.HasImage == true;

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(OpenVisionTcpIntegrationController.SelectedSourceImagePath), StringComparison.Ordinal))
            {
                RefreshSourceImageViewer();
            }
            else if (string.IsNullOrEmpty(e.PropertyName) && !sourceImageViewer.HasImage)
            {
                RefreshEmptySourceImageText();
            }
        }

        private void RefreshSourceImageViewer()
        {
            sourceImageDisplayError = string.Empty;
            string path = controller.SelectedSourceImagePath;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                RefreshEmptySourceImageText();
                return;
            }

            try
            {
                using Bitmap image = new Bitmap(path);
                sourceImageViewer.SetLayer(
                    Path.GetFileName(path),
                    image,
                    $"Handoff {controller.SelectedTransaction?.TransactionId:D} · {image.Width}x{image.Height}");
            }
            catch (Exception exception) when (exception is ArgumentException or IOException or InvalidOperationException)
            {
                sourceImageDisplayError = exception.Message;
                RefreshEmptySourceImageText();
            }
        }

        private void RefreshEmptySourceImageText()
        {
            bool english = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English;
            string status = string.IsNullOrEmpty(sourceImageDisplayError)
                ? (english ? "No source image for the selected 2D Handoff." : "선택된 2D Handoff 원본 이미지가 없습니다.")
                : (english ? "Could not display the source image: " : "원본 이미지 표시 실패: ") + sourceImageDisplayError;
            sourceImageViewer.SetLayer(null, null, status);
        }

        private void OnSessionSharedKeyPasswordChanged(object sender, RoutedEventArgs e)
        {
            controller.SetSessionSharedKey(sessionSharedKeyInput.Password);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (controller.CanCloseWindow)
            {
                return;
            }

            e.Cancel = true;
            controller.ReportStopRequiredBeforeClose();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Closing -= OnClosing;
            Closed -= OnClosed;
            controller.PropertyChanged -= OnControllerPropertyChanged;
            sessionSharedKeyInput.PasswordChanged -= OnSessionSharedKeyPasswordChanged;
            sourceImageViewer.Dispose();
            DataContext = null;
            controller.OnWindowClosed(this);
        }
    }
}
