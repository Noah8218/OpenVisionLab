using System;
using System.ComponentModel;
using System.Windows;

namespace OpenVisionLab
{
    public partial class OpenVisionTcpIntegrationWindow : Window
    {
        private readonly OpenVisionTcpIntegrationController controller;

        internal OpenVisionTcpIntegrationWindow(OpenVisionTcpIntegrationController controller)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            DataContext = controller;
            Closing += OnClosing;
            Closed += OnClosed;
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
            sessionSharedKeyInput.PasswordChanged -= OnSessionSharedKeyPasswordChanged;
            DataContext = null;
            controller.OnWindowClosed(this);
        }
    }
}
