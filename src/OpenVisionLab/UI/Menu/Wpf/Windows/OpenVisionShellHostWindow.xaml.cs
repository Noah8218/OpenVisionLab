using MahApps.Metro.IconPacks;
using OpenVisionLab.Core;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace OpenVisionLab
{
    public partial class OpenVisionShellHostWindow : Window
    {
        private const int WmGetMinMaxInfo = 0x0024;
        private const uint MonitorDefaultToNearest = 0x00000002;
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
            if (message == WmGetMinMaxInfo)
            {
                ApplyMonitorWorkArea(hwnd, lParam);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private static void ApplyMonitorWorkArea(IntPtr hwnd, IntPtr lParam)
        {
            IntPtr monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);
            if (monitor == IntPtr.Zero)
            {
                return;
            }

            MonitorInfo monitorInfo = new MonitorInfo
            {
                Size = Marshal.SizeOf<MonitorInfo>()
            };
            if (!GetMonitorInfo(monitor, ref monitorInfo))
            {
                return;
            }

            MinMaxInfo minMaxInfo = Marshal.PtrToStructure<MinMaxInfo>(lParam);
            minMaxInfo.MaxPosition.X = monitorInfo.WorkArea.Left - monitorInfo.MonitorArea.Left;
            minMaxInfo.MaxPosition.Y = monitorInfo.WorkArea.Top - monitorInfo.MonitorArea.Top;
            minMaxInfo.MaxSize.X = monitorInfo.WorkArea.Right - monitorInfo.WorkArea.Left;
            minMaxInfo.MaxSize.Y = monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top;
            Marshal.StructureToPtr(minMaxInfo, lParam, false);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo monitorInfo);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MinMaxInfo
        {
            public NativePoint Reserved;
            public NativePoint MaxSize;
            public NativePoint MaxPosition;
            public NativePoint MinTrackSize;
            public NativePoint MaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfo
        {
            public int Size;
            public NativeRect MonitorArea;
            public NativeRect WorkArea;
            public uint Flags;
        }
    }
}
