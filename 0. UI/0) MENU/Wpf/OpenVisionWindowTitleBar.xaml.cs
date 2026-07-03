using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    public partial class OpenVisionWindowTitleBar : UserControl
    {
        public OpenVisionWindowTitleBar()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            ApplyLocalizedChromeText();
        }

        public event EventHandler DockRequested = delegate { };

        public string TitleText
        {
            get => txtTitle?.Text ?? string.Empty;
            set
            {
                string title = string.IsNullOrWhiteSpace(value) ? "OpenVisionLab" : value;
                txtTitle.Text = title;
                Window owner = Window.GetWindow(this);
                if (owner != null)
                {
                    owner.Title = title;
                }
            }
        }

        public PackIconMaterialKind IconKind
        {
            get => titleIcon.Kind;
            set => titleIcon.Kind = value;
        }

        public string MinimizeToolTipText => btnMinimize.ToolTip?.ToString() ?? string.Empty;

        public string DockToolTipText => btnDock.ToolTip?.ToString() ?? string.Empty;

        public bool IsDockButtonVisible
        {
            get => btnDock?.Visibility == Visibility.Visible;
            set => btnDock.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public string MaximizeRestoreToolTipText => btnMaximize.ToolTip?.ToString() ?? string.Empty;

        public string CloseToolTipText => btnClose.ToolTip?.ToString() ?? string.Empty;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window owner = Window.GetWindow(this);
            if (owner != null && !string.IsNullOrWhiteSpace(TitleText))
            {
                owner.Title = TitleText;
            }

            ApplyLocalizedChromeText();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalizedChromeText();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window owner = Window.GetWindow(this);
            if (owner == null)
            {
                return;
            }

            if (e.ClickCount == 2)
            {
                ToggleWindowState(owner);
                return;
            }

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                owner.DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Window owner = Window.GetWindow(this);
            if (owner != null)
            {
                owner.WindowState = WindowState.Minimized;
            }
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            Window owner = Window.GetWindow(this);
            if (owner != null)
            {
                ToggleWindowState(owner);
            }
        }

        private void Dock_Click(object sender, RoutedEventArgs e)
        {
            DockRequested(this, EventArgs.Empty);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void ToggleWindowState(Window owner)
        {
            owner.WindowState = owner.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
            ApplyLocalizedChromeText();
        }

        public void ApplyLocalizedChromeText()
        {
            string dockText = OpenVisionLanguageService.T("Common.DockRight");
            btnDock.ToolTip = string.Equals(dockText, "Common.DockRight", StringComparison.Ordinal) ? "우측 고정" : dockText;
            btnMinimize.ToolTip = OpenVisionLanguageService.T("Common.Minimize");
            Window owner = Window.GetWindow(this);
            btnMaximize.ToolTip = owner?.WindowState == WindowState.Maximized
                ? OpenVisionLanguageService.T("Common.Restore")
                : OpenVisionLanguageService.T("Common.Maximize");
            btnClose.ToolTip = OpenVisionLanguageService.T("Common.Close");
            maximizeIcon.Kind = owner?.WindowState == WindowState.Maximized
                ? PackIconMaterialKind.WindowRestore
                : PackIconMaterialKind.WindowMaximize;
        }
    }
}
