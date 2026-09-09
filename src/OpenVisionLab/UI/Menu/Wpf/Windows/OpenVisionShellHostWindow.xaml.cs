using MahApps.Metro.IconPacks;
using OpenVisionLab.Core;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace OpenVisionLab
{
    public partial class OpenVisionShellHostWindow : Window
    {
        private const double ReferenceWidth = 1600D;
        private const double ReferenceHeight = 900D;
        private const double TitleBarHeight = 42D;
        private const double ResizeBorderThickness = 7D;
        private const double MaximumResponsiveScale = 1.5D;
        private readonly ScaleTransform titleBarScale = new();
        private readonly ScaleTransform contentScale = new();
        private HwndSource windowSource;
        private double responsiveScale = 1D;

        public OpenVisionShellHostWindow()
            : this(ApplicationRuntimeContext.CreateDefault())
        {
        }

        public OpenVisionShellHostWindow(ApplicationRuntimeContext runtimeContext)
        {
            InitializeComponent();
            shellTitleBar.TitleText = "OpenVisionLab";
            shellTitleBar.IconKind = PackIconMaterialKind.ImageFilterCenterFocus;
            contentHost.Content = new OpenVisionShellHostView(runtimeContext);
            shellTitleBar.LayoutTransform = titleBarScale;
            contentHost.LayoutTransform = contentScale;
            ApplyResponsiveScale(Width, Height);
        }

        public OpenVisionShellHostView ShellHostForSmoke => contentHost.Content as OpenVisionShellHostView;

        public Task StartupPreparationTask =>
            ShellHostForSmoke?.StartupPreparationTask ?? Task.CompletedTask;

        public double ResponsiveScaleForSmoke => responsiveScale;

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ApplyResponsiveScale(sizeInfo.NewSize.Width, sizeInfo.NewSize.Height);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            windowSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            windowSource?.AddHook(WindowProc);
        }

        protected override void OnClosed(EventArgs e)
        {
            ShellHostForSmoke?.Dispose();
            contentHost.Content = null;
            windowSource?.RemoveHook(WindowProc);
            windowSource = null;
            base.OnClosed(e);
        }

        private void ApplyResponsiveScale(double width, double height)
        {
            double widthScale = width > 0D ? width / ReferenceWidth : 1D;
            double heightScale = height > 0D ? height / ReferenceHeight : 1D;
            double scale = Math.Clamp(Math.Min(widthScale, heightScale), 1D, MaximumResponsiveScale);
            if (Math.Abs(scale - responsiveScale) < 0.001D)
            {
                return;
            }

            responsiveScale = scale;
            titleBarScale.ScaleX = scale;
            titleBarScale.ScaleY = scale;
            contentScale.ScaleX = scale;
            contentScale.ScaleY = scale;
            titleBarRow.Height = new GridLength(TitleBarHeight * scale);
            shellWindowChrome.CaptionHeight = TitleBarHeight * scale;
            shellWindowChrome.ResizeBorderThickness = new Thickness(ResizeBorderThickness * scale);
        }

        private static IntPtr WindowProc(
            IntPtr hwnd,
            int message,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            if (message == OpenVisionWindowWorkArea.GetMinMaxInfoMessage)
            {
                OpenVisionWindowWorkArea.Apply(hwnd, lParam);
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
