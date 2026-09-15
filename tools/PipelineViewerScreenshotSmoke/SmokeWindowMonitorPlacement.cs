#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

internal static class SmokeWindowMonitorPlacement
{
    internal static string PlaceOnLeftmostMonitor(Window window)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        List<MonitorInfo> monitors = new List<MonitorInfo>();
        MonitorEnumCallback callback = (IntPtr monitor, IntPtr _, ref NativeRect __, IntPtr ___) =>
        {
            MonitorInfo info = MonitorInfo.Create();
            if (GetMonitorInfo(monitor, ref info))
            {
                monitors.Add(info);
            }

            return true;
        };

        if (!EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero) || monitors.Count == 0)
        {
            throw new InvalidOperationException("No display monitor was available for the EXE capture.");
        }

        MonitorInfo selected = monitors
            .OrderBy(info => info.Monitor.Left)
            .ThenBy(info => info.Monitor.Top)
            .First();
        IntPtr handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero || !GetWindowRect(handle, out NativeRect initialWindow))
        {
            throw new InvalidOperationException("The EXE window rectangle was unavailable before monitor placement.");
        }

        int width = initialWindow.Right - initialWindow.Left;
        int height = initialWindow.Bottom - initialWindow.Top;
        (int left, int top) = CalculateCenteredPosition(
            selected.WorkArea.Left,
            selected.WorkArea.Top,
            selected.WorkArea.Right,
            selected.WorkArea.Bottom,
            width,
            height);
        const uint noSize = 0x0001;
        const uint noZOrder = 0x0004;
        if (!SetWindowPos(handle, IntPtr.Zero, left, top, 0, 0, noSize | noZOrder)
            || !GetWindowRect(handle, out NativeRect actualWindow))
        {
            throw new InvalidOperationException("The EXE window could not be placed on the leftmost monitor.");
        }

        bool intersects = Intersects(actualWindow, selected.Monitor);
        if (!intersects)
        {
            throw new InvalidOperationException(
                "The EXE window did not intersect the selected leftmost monitor. "
                + $"Window={actualWindow}; Monitor={selected.Monitor}");
        }

        return "CaptureMonitor: " + selected.DeviceName
            + "; Bounds=" + selected.Monitor
            + "; WorkArea=" + selected.WorkArea
            + "; Window=" + actualWindow
            + "; Intersects=true";
    }

    internal static (int Left, int Top) CalculateCenteredPosition(
        int workAreaLeft,
        int workAreaTop,
        int workAreaRight,
        int workAreaBottom,
        int windowWidth,
        int windowHeight)
    {
        int left = workAreaLeft + Math.Max(0, (workAreaRight - workAreaLeft - windowWidth) / 2);
        int top = workAreaTop + Math.Max(0, (workAreaBottom - workAreaTop - windowHeight) / 2);
        return (left, top);
    }

    internal static bool Intersects(NativeRect window, NativeRect monitor)
    {
        return window.Left < monitor.Right
            && window.Right > monitor.Left
            && window.Top < monitor.Bottom
            && window.Bottom > monitor.Top;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(
        IntPtr hdc,
        IntPtr clip,
        MonitorEnumCallback callback,
        IntPtr data);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo monitorInfo);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr window, out NativeRect rect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr window,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    private delegate bool MonitorEnumCallback(
        IntPtr monitor,
        IntPtr hdc,
        ref NativeRect rect,
        IntPtr data);

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;

        public override readonly string ToString()
        {
            return $"{Left},{Top},{Right - Left},{Bottom - Top}";
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfo
    {
        internal int Size;
        internal NativeRect Monitor;
        internal NativeRect WorkArea;
        internal uint Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string DeviceName;

        internal static MonitorInfo Create()
        {
            return new MonitorInfo
            {
                Size = Marshal.SizeOf<MonitorInfo>(),
                DeviceName = string.Empty
            };
        }
    }
}
