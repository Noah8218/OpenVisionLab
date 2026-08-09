using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class OpenVisionFloatingToolDockRequestedEventArgs : EventArgs
    {
        public OpenVisionFloatingToolDockRequestedEventArgs(FrameworkElement content, string title, double floatingWidth, double floatingHeight)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Title = string.IsNullOrWhiteSpace(title) ? "OpenVisionLab Tool" : title;
            FloatingWidth = floatingWidth;
            FloatingHeight = floatingHeight;
        }

        public FrameworkElement Content { get; }

        public string Title { get; }

        public double FloatingWidth { get; }

        public double FloatingHeight { get; }
    }

    internal sealed class OpenVisionFloatingToolWindowHost
    {
        private const uint MonitorDefaultToNearest = 0x00000002;
        private const double LargeToolSavedBoundsWidthThreshold = 1100D;
        private const double LargeToolSavedBoundsHeightThreshold = 820D;
        private readonly OpenVisionFloatingToolWindowPlacementStore placementStore = new OpenVisionFloatingToolWindowPlacementStore();
        private bool closingToolWindow;
        private bool suppressPlacementSave;

        public event EventHandler ClosedByUser = delegate { };

        public event EventHandler<OpenVisionFloatingToolDockRequestedEventArgs> DockRequested = delegate { };

        public OpenVisionFloatingToolWindow ActiveWindow { get; private set; }

        public void Prepare(Window owner)
        {
            if (ActiveWindow != null)
            {
                return;
            }

            ActiveWindow = new OpenVisionFloatingToolWindow
            {
                Width = 920,
                Height = 660,
                MinWidth = 760,
                MinHeight = 520,
                WindowStartupLocation = WindowStartupLocation.Manual,
                ResizeMode = ResizeMode.CanResize,
                ShowActivated = false,
                ShowInTaskbar = false,
                Opacity = 0,
                Left = -32000,
                Top = -32000
            };

            ActiveWindow.IsDockButtonVisible = true;
            if (owner != null)
            {
                ActiveWindow.Owner = owner;
            }

            AttachWindowEvents(ActiveWindow);
            // Pay the first WPF Window handle/show cost before the operator opens a tool.
            suppressPlacementSave = true;
            try
            {
                ActiveWindow.Show();
                ActiveWindow.Hide();
            }
            finally
            {
                suppressPlacementSave = false;
            }

            ActiveWindow.Opacity = 1;
            ActiveWindow.ShowActivated = true;
        }

        public bool Show(FrameworkElement content, string title, double width, double height, Window owner)
        {
            if (ActiveWindow != null)
            {
                ActiveWindow.IsDockButtonVisible = true;
                OpenVisionToolOpenProfiler.Measure("ReuseToolWindowSetTitle", () => ActiveWindow.SetTitle(title));
                OpenVisionToolOpenProfiler.Measure("ReuseToolWindowSize", () =>
                {
                    ActiveWindow.Width = width;
                    ActiveWindow.Height = height;
                });
                OpenVisionToolOpenProfiler.Measure("ReuseToolWindowSetContent", () => ActiveWindow.SetHostedContent(content));
                if (!ActiveWindow.IsVisible)
                {
                    OpenVisionToolOpenProfiler.Measure("ReuseToolWindowPosition", () => PositionWindowForShow(ActiveWindow, owner, placementStore));
                    OpenVisionToolOpenProfiler.Measure("ReuseToolWindowShow", ActiveWindow.Show);
                }

                OpenVisionToolOpenProfiler.Measure("ReuseToolWindowBringAboveAirspace", ActiveWindow.BringAboveOwnerAirspace);
                return true;
            }

            ActiveWindow = OpenVisionToolOpenProfiler.Measure(
                "CreateFloatingToolWindow",
                () => new OpenVisionFloatingToolWindow(title, content)
                {
                    Width = width,
                    Height = height,
                    MinWidth = 760,
                    MinHeight = 520,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.CanResize
                });

            ActiveWindow.IsDockButtonVisible = true;
            if (owner != null)
            {
                OpenVisionToolOpenProfiler.Measure("SetFloatingToolWindowOwner", () => ActiveWindow.Owner = owner);
            }
            else
            {
                OpenVisionToolOpenProfiler.Measure("SetFloatingToolWindowStartupLocation", () => ActiveWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen);
            }

            AttachWindowEvents(ActiveWindow);
            OpenVisionToolOpenProfiler.Measure("PositionFloatingToolWindow", () => PositionWindowForShow(ActiveWindow, owner, placementStore));
            OpenVisionToolOpenProfiler.Measure("ShowFloatingToolWindow", ActiveWindow.Show);
            OpenVisionToolOpenProfiler.Measure("BringFloatingToolWindowAboveAirspace", ActiveWindow.BringAboveOwnerAirspace);
            return false;
        }

        private static void PositionWindowForShow(
            OpenVisionFloatingToolWindow window,
            Window owner,
            OpenVisionFloatingToolWindowPlacementStore placementStore)
        {
            if (window == null)
            {
                return;
            }

            window.WindowStartupLocation = WindowStartupLocation.Manual;
            Rect workArea = ResolveWorkArea(owner);
            if (placementStore != null && placementStore.TryLoad(out Rect savedBounds))
            {
                ApplyBounds(
                    window,
                    ClampToWorkArea(
                        savedBounds,
                        workArea,
                        ResolveSavedBoundsMinimumWidth(window),
                        ResolveSavedBoundsMinimumHeight(window)));
                return;
            }

            Rect preferredBounds = CreateRightSideBounds(window, owner, workArea);
            ApplyBounds(window, ClampToWorkArea(preferredBounds, workArea, window.MinWidth, window.MinHeight));
        }

        public bool CloseSilently()
        {
            if (ActiveWindow == null)
            {
                return false;
            }

            OpenVisionFloatingToolWindow window = ActiveWindow;
            ActiveWindow = null;
            closingToolWindow = true;
            try
            {
                placementStore.Save(window);
                DetachWindowEvents(window);
                window.ClearHostedContent();
                window.Close();
            }
            finally
            {
                closingToolWindow = false;
            }

            return true;
        }

        public bool HideForReuse()
        {
            if (ActiveWindow == null)
            {
                return false;
            }

            placementStore.Save(ActiveWindow);
            ActiveWindow.Hide();
            return true;
        }

        public void BringActiveWindowAboveOwnerAirspace()
        {
            // Owner OpenGL HWNDs can reclaim z-order after layer activation; keep the tool window clickable.
            ActiveWindow?.BringAboveOwnerAirspace();
        }

        public bool CloseByUser()
        {
            if (ActiveWindow == null)
            {
                return false;
            }

            OpenVisionFloatingToolWindow window = ActiveWindow;
            ActiveWindow = null;
            placementStore.Save(window);
            DetachWindowEvents(window);
            window.ClearHostedContent();
            window.Close();
            ClosedByUser(this, EventArgs.Empty);
            return true;
        }

        public bool RequestDockForTest()
        {
            if (ActiveWindow == null || ActiveWindow.HostedContent == null)
            {
                return false;
            }

            ActiveWindow.RequestDockForTest();
            return true;
        }

        private void ActiveWindow_Closed(object sender, EventArgs e)
        {
            if (closingToolWindow)
            {
                return;
            }

            if (ActiveWindow != null)
            {
                placementStore.Save(ActiveWindow);
                DetachWindowEvents(ActiveWindow);
                ActiveWindow.ClearHostedContent();
                ActiveWindow = null;
            }

            ClosedByUser(this, EventArgs.Empty);
        }

        private void ActiveWindow_DockRequested(object sender, EventArgs e)
        {
            if (ActiveWindow == null)
            {
                return;
            }

            OpenVisionFloatingToolWindow window = ActiveWindow;
            FrameworkElement content = window.HostedContent;
            if (content == null)
            {
                return;
            }

            ActiveWindow = null;
            closingToolWindow = true;
            try
            {
                placementStore.Save(window);
                DetachWindowEvents(window);
                window.ClearHostedContent();
                window.Close();
            }
            finally
            {
                closingToolWindow = false;
            }

            DockRequested(
                this,
                new OpenVisionFloatingToolDockRequestedEventArgs(content, window.Title, window.Width, window.Height));
        }

        private void AttachWindowEvents(OpenVisionFloatingToolWindow window)
        {
            if (window == null)
            {
                return;
            }

            window.Closed += ActiveWindow_Closed;
            window.DockRequested += ActiveWindow_DockRequested;
            AttachPlacementTracking(window);
        }

        private void DetachWindowEvents(OpenVisionFloatingToolWindow window)
        {
            if (window == null)
            {
                return;
            }

            window.Closed -= ActiveWindow_Closed;
            window.DockRequested -= ActiveWindow_DockRequested;
            DetachPlacementTracking(window);
        }

        private void AttachPlacementTracking(OpenVisionFloatingToolWindow window)
        {
            if (window == null)
            {
                return;
            }

            window.LocationChanged += ToolWindow_PlacementChanged;
            window.SizeChanged += ToolWindow_PlacementChanged;
        }

        private void DetachPlacementTracking(OpenVisionFloatingToolWindow window)
        {
            if (window == null)
            {
                return;
            }

            window.LocationChanged -= ToolWindow_PlacementChanged;
            window.SizeChanged -= ToolWindow_PlacementChanged;
        }

        private void ToolWindow_PlacementChanged(object sender, EventArgs e)
        {
            if (sender is Window window && window.IsVisible)
            {
                SavePlacement(window);
            }
        }

        private void SavePlacement(Window window)
        {
            if (suppressPlacementSave)
            {
                return;
            }

            placementStore.Save(window);
        }

        private static Rect ResolveWorkArea(Window owner)
        {
            if (owner != null)
            {
                IntPtr handle = new WindowInteropHelper(owner).Handle;
                IntPtr monitor = handle == IntPtr.Zero
                    ? IntPtr.Zero
                    : MonitorFromWindow(handle, MonitorDefaultToNearest);
                if (monitor != IntPtr.Zero)
                {
                    MonitorInfo monitorInfo = new MonitorInfo
                    {
                        Size = Marshal.SizeOf<MonitorInfo>()
                    };
                    if (GetMonitorInfo(monitor, ref monitorInfo))
                    {
                        HwndSource source = HwndSource.FromHwnd(handle);
                        Matrix fromDevice = source?.CompositionTarget?.TransformFromDevice ?? Matrix.Identity;
                        Point topLeft = fromDevice.Transform(
                            new Point(monitorInfo.WorkArea.Left, monitorInfo.WorkArea.Top));
                        Point bottomRight = fromDevice.Transform(
                            new Point(monitorInfo.WorkArea.Right, monitorInfo.WorkArea.Bottom));
                        Rect ownerMonitorWorkArea = new Rect(topLeft, bottomRight);
                        if (ownerMonitorWorkArea.Width > 0D && ownerMonitorWorkArea.Height > 0D)
                        {
                            return ownerMonitorWorkArea;
                        }
                    }
                }
            }

            return SystemParameters.WorkArea;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo monitorInfo);

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

        private static Rect CreateRightSideBounds(OpenVisionFloatingToolWindow window, Window owner, Rect workArea)
        {
            double width = Math.Min(Math.Max(window.Width, window.MinWidth), workArea.Width);
            double height = Math.Min(Math.Max(window.Height, window.MinHeight), workArea.Height);

            if (owner != null && owner.IsVisible)
            {
                double ownerWidth = owner.ActualWidth > 0 ? owner.ActualWidth : owner.Width;
                double ownerHeight = owner.ActualHeight > 0 ? owner.ActualHeight : owner.Height;
                double gap = 12D;
                double rightOutside = owner.Left + ownerWidth + gap;
                if (rightOutside + width <= workArea.Right)
                {
                    return new Rect(rightOutside, owner.Top + 24D, width, height);
                }

                double leftOutside = owner.Left - width - gap;
                if (leftOutside >= workArea.Left)
                {
                    return new Rect(leftOutside, owner.Top + 24D, width, height);
                }

                double insideRight = owner.Left + ownerWidth - width - 16D;
                double top = owner.Top + Math.Min(84D, Math.Max(16D, ownerHeight * 0.08D));
                return new Rect(insideRight, top, width, height);
            }

            return new Rect(workArea.Right - width - 16D, workArea.Top + 24D, width, height);
        }

        private static double ResolveSavedBoundsMinimumWidth(OpenVisionFloatingToolWindow window)
        {
            if (window == null)
            {
                return 0D;
            }

            return window.Width >= LargeToolSavedBoundsWidthThreshold
                ? window.Width
                : window.MinWidth;
        }

        private static double ResolveSavedBoundsMinimumHeight(OpenVisionFloatingToolWindow window)
        {
            if (window == null)
            {
                return 0D;
            }

            return window.Height >= LargeToolSavedBoundsHeightThreshold
                ? window.Height
                : window.MinHeight;
        }

        private static Rect ClampToWorkArea(Rect bounds, Rect workArea, double minWidth, double minHeight)
        {
            double width = Math.Min(Math.Max(bounds.Width, minWidth), workArea.Width);
            double height = Math.Min(Math.Max(bounds.Height, minHeight), workArea.Height);
            double left = Math.Min(Math.Max(bounds.Left, workArea.Left), Math.Max(workArea.Left, workArea.Right - width));
            double top = Math.Min(Math.Max(bounds.Top, workArea.Top), Math.Max(workArea.Top, workArea.Bottom - height));
            return new Rect(left, top, width, height);
        }

        private static void ApplyBounds(OpenVisionFloatingToolWindow window, Rect bounds)
        {
            window.Width = bounds.Width;
            window.Height = bounds.Height;
            window.Left = bounds.Left;
            window.Top = bounds.Top;
        }
    }
}
