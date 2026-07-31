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

        private void OnClosed(object sender, EventArgs e)
        {
            Closed -= OnClosed;
            controller.Dispose();
        }
    }
}
