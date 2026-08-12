using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace OpenVisionLab
{
    public partial class OpenVisionStartupLoadingWindow : Window
    {
        private bool canClose;

        public OpenVisionStartupLoadingWindow()
        {
            InitializeComponent();
            titleText.Text = OpenVisionLanguageService.T("Shell.StartupLoading.Title");
            detailText.Text = OpenVisionLanguageService.T("Shell.StartupLoading.Detail");
        }

        public string LoadingTitleForTest => titleText.Text ?? string.Empty;

        public string LoadingDetailForTest => detailText.Text ?? string.Empty;

        public void ShowReady()
        {
            Show();
            Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
        }

        public void Complete()
        {
            if (canClose)
            {
                return;
            }

            canClose = true;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!canClose)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }
    }
}
