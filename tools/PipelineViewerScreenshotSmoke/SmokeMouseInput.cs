#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

internal static class SmokeMouseInput
{
    private const uint MouseEventLeftDown = 0x0002;
    private const uint MouseEventLeftUp = 0x0004;

    internal static void ReleaseLeftButton()
    {
        mouse_event(MouseEventLeftUp, 0U, 0U, 0U, UIntPtr.Zero);
    }

    internal static void PressLeftButton()
    {
        mouse_event(MouseEventLeftDown, 0U, 0U, 0U, UIntPtr.Zero);
    }

    internal static void DragOnBackgroundThread(Point sourceScreenPoint, Point targetScreenPoint, Action pump)
    {
        RunOnBackgroundThread(
            () => SendMouseDrag(sourceScreenPoint, targetScreenPoint),
            pump,
            "OpenVisionDockingMouseDragSmoke",
            "Mouse drag input thread did not finish within the expected time.",
            "Mouse drag input failed.");
    }

    internal static void DragViaPointOnBackgroundThread(
        Point sourceScreenPoint,
        Point viaScreenPoint,
        Point targetScreenPoint,
        Action pump)
    {
        RunOnBackgroundThread(
            () => SendMouseDragThroughPoints(sourceScreenPoint, viaScreenPoint, targetScreenPoint),
            pump,
            "OpenVisionHostTabMouseDragSmoke",
            "Host tab mouse drag input thread did not finish within the expected time.",
            "Host tab mouse drag input failed.");
    }

    internal static void SetCursorPosOrThrow(int x, int y)
    {
        if (!SetCursorPos(x, y))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "SetCursorPos failed.");
        }
    }

    internal static int RoundToScreenPixel(double value)
    {
        return (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }

    private static void RunOnBackgroundThread(
        Action input,
        Action pump,
        string threadName,
        string timeoutMessage,
        string failureMessage)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (pump == null)
        {
            throw new ArgumentNullException(nameof(pump));
        }

        Exception? inputException = null;
        Thread inputThread = new Thread(() =>
        {
            try
            {
                input();
            }
            catch (Exception ex)
            {
                inputException = ex;
            }
        });
        inputThread.IsBackground = true;
        inputThread.Name = threadName;
        inputThread.Start();

        DateTime deadline = DateTime.UtcNow.AddSeconds(8D);
        while (inputThread.IsAlive && DateTime.UtcNow < deadline)
        {
            pump();
        }

        if (inputThread.IsAlive)
        {
            throw new TimeoutException(timeoutMessage);
        }

        inputThread.Join();
        if (inputException != null)
        {
            throw new InvalidOperationException(failureMessage, inputException);
        }
    }

    private static void SendMouseDrag(Point sourceScreenPoint, Point targetScreenPoint)
    {
        int sourceX = RoundToScreenPixel(sourceScreenPoint.X);
        int sourceY = RoundToScreenPixel(sourceScreenPoint.Y);
        int targetX = RoundToScreenPixel(targetScreenPoint.X);
        int targetY = RoundToScreenPixel(targetScreenPoint.Y);

        ReleaseLeftButton();
        Thread.Sleep(80);
        SetCursorPosOrThrow(sourceX, sourceY);
        Thread.Sleep(160);
        PressLeftButton();
        Thread.Sleep(120);

        const int steps = 34;
        for (int step = 1; step <= steps; step++)
        {
            double ratio = step / (double)steps;
            int x = RoundToScreenPixel(sourceX + ((targetX - sourceX) * ratio));
            int y = RoundToScreenPixel(sourceY + ((targetY - sourceY) * ratio));
            SetCursorPosOrThrow(x, y);
            Thread.Sleep(18);
        }

        Thread.Sleep(180);
        ReleaseLeftButton();
    }

    private static void SendMouseDragThroughPoints(params Point[] screenPoints)
    {
        if (screenPoints == null || screenPoints.Length < 2)
        {
            throw new ArgumentException("At least two screen points are required.", nameof(screenPoints));
        }

        int sourceX = RoundToScreenPixel(screenPoints[0].X);
        int sourceY = RoundToScreenPixel(screenPoints[0].Y);

        ReleaseLeftButton();
        Thread.Sleep(80);
        SetCursorPosOrThrow(sourceX, sourceY);
        Thread.Sleep(180);
        PressLeftButton();
        Thread.Sleep(120);

        for (int segment = 1; segment < screenPoints.Length; segment++)
        {
            int fromX = RoundToScreenPixel(screenPoints[segment - 1].X);
            int fromY = RoundToScreenPixel(screenPoints[segment - 1].Y);
            int toX = RoundToScreenPixel(screenPoints[segment].X);
            int toY = RoundToScreenPixel(screenPoints[segment].Y);
            int steps = segment == 1 ? 10 : 34;

            for (int step = 1; step <= steps; step++)
            {
                double ratio = step / (double)steps;
                int x = RoundToScreenPixel(fromX + ((toX - fromX) * ratio));
                int y = RoundToScreenPixel(fromY + ((toY - fromY) * ratio));
                SetCursorPosOrThrow(x, y);
                Thread.Sleep(segment == 1 ? 24 : 18);
            }
        }

        Thread.Sleep(180);
        ReleaseLeftButton();
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
