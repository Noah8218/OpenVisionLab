using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Automation;

namespace OpenVisionLab
{
    public partial class OpenVisionFloatingToolWindow : Window
    {
        public OpenVisionFloatingToolWindow()
        {
            InitializeComponent();
            Closed += OnClosed;
            StateChanged += OnStateChanged;
            windowTitleBar.DockRequested += WindowTitleBar_DockRequested;
            ApplyLocalizedChromeText();
        }

        public event EventHandler DockRequested = delegate { };

        public OpenVisionFloatingToolWindow(string title, FrameworkElement content)
            : this()
        {
            SetTitle(title);
            SetHostedContent(content);
        }

        public void SetTitle(string title)
        {
            string text = string.IsNullOrWhiteSpace(title) ? "OpenVisionLab Tool" : title;
            windowTitleBar.TitleText = text;
            Title = text;
            AutomationProperties.SetName(this, text);
        }

        public void SetTitleIcon(PackIconMaterialKind kind)
        {
            windowTitleBar.IconKind = kind;
        }

        public void SetHostedContent(FrameworkElement content)
        {
            contentHost.Content = content;
        }

        public FrameworkElement HostedContent => contentHost.Content as FrameworkElement;

        public void ClearHostedContent(bool disposeContent = false)
        {
            if (disposeContent && contentHost.Content is IDisposable disposable)
            {
                disposable.Dispose();
            }

            contentHost.Content = null;
        }

        public void BringAboveOwnerAirspace()
        {
            if (!IsVisible)
            {
                return;
            }

            // WinForms/OpenGL child HWNDs in the owner can otherwise stay above owned WPF tool windows.
            Topmost = true;
            Topmost = false;
            Activate();
        }

        public bool IsDockButtonVisible
        {
            get => windowTitleBar.IsDockButtonVisible;
            set => windowTitleBar.IsDockButtonVisible = value;
        }

        public string DockToolTipText => windowTitleBar.DockToolTipText;

        public string MinimizeToolTipText => windowTitleBar.MinimizeToolTipText;

        public string MaximizeRestoreToolTipText => windowTitleBar.MaximizeRestoreToolTipText;

        public string CloseToolTipText => windowTitleBar.CloseToolTipText;

        private void OnStateChanged(object sender, EventArgs e)
        {
            ApplyLocalizedChromeText();
        }

        internal void RequestDockForTest()
        {
            DockRequested(this, EventArgs.Empty);
        }

        private void WindowTitleBar_DockRequested(object sender, EventArgs e)
        {
            DockRequested(this, EventArgs.Empty);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Closed -= OnClosed;
            StateChanged -= OnStateChanged;
            windowTitleBar.DockRequested -= WindowTitleBar_DockRequested;
        }

        private void ApplyLocalizedChromeText()
        {
            windowTitleBar.ApplyLocalizedChromeText();
        }
    }
}
