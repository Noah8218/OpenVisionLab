using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostBusyPresenter
    {
        private readonly Border overlay;
        private readonly TextBlock titleText;
        private readonly TextBlock detailText;

        public OpenVisionShellHostBusyPresenter(
            Border overlay,
            TextBlock titleText,
            TextBlock detailText)
        {
            this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
            this.titleText = titleText ?? throw new ArgumentNullException(nameof(titleText));
            this.detailText = detailText ?? throw new ArgumentNullException(nameof(detailText));
        }

        public bool IsVisible => overlay.Visibility == Visibility.Visible;

        public string Title => titleText.Text ?? string.Empty;

        public void ShowPipelineLoading()
        {
            titleText.Text = OpenVisionLanguageService.T("Shell.PipelineLoading.Title");
            detailText.Text = OpenVisionLanguageService.T("Shell.PipelineLoading.Detail");
            overlay.Visibility = Visibility.Visible;
            overlay.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
        }

        public void Hide()
        {
            overlay.Visibility = Visibility.Collapsed;
        }
    }
}
