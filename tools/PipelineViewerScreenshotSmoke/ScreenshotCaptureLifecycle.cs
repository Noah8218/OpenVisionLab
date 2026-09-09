using System;
using System.Linq;
using System.Windows;

internal static class ScreenshotCaptureLifecycle
{
    internal static CaptureResult CaptureWindowWithContent(
        FrameworkElement content,
        string outputPath,
        int width,
        int height,
        Action verify,
        bool captureFloatingToolWindow,
        Action<string>? verifyCapture,
        int initialPumpCount,
        bool captureScreen,
        Action<int> pump,
        Action<string, DependencyObject?[]> writeOpenGlDiagnostics)
    {
        Window window = new()
        {
            Content = content,
            Width = width,
            Height = height,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            Topmost = true
        };

        DateTime started = DateTime.UtcNow;
        window.Show();
        window.Activate();
        try
        {
            pump(initialPumpCount);
            verify();
            pump(12);
            Window captureWindow = captureFloatingToolWindow
                ? Application.Current.Windows
                    .OfType<Window>()
                    .LastOrDefault(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                    ?? window
                : window;
            if (captureScreen)
            {
                ScreenshotPngWriter.WriteScreenPng(captureWindow, outputPath);
            }
            else
            {
                ScreenshotPngWriter.WriteElementPng(captureWindow, outputPath, (int)captureWindow.ActualWidth, (int)captureWindow.ActualHeight);
            }

            writeOpenGlDiagnostics(outputPath, new DependencyObject?[] { captureWindow, content });
            verifyCapture?.Invoke(outputPath);
            return new CaptureResult(
                Math.Max(1, (int)Math.Round(captureWindow.ActualWidth)),
                Math.Max(1, (int)Math.Round(captureWindow.ActualHeight)),
                (DateTime.UtcNow - started).TotalMilliseconds);
        }
        finally
        {
            foreach (Window owned in Application.Current.Windows.OfType<Window>().Where(item => !ReferenceEquals(item, window)).ToArray())
            {
                owned.Close();
            }

            if (content is IDisposable disposable)
            {
                disposable.Dispose();
            }

            window.Close();
        }
    }

    internal static CaptureResult CaptureStandaloneWindow(
        Window window,
        string outputPath,
        int width,
        int height,
        Action verify,
        Action<int> pump,
        Action<string, DependencyObject?[]> writeOpenGlDiagnostics)
    {
        DateTime started = DateTime.UtcNow;
        window.Width = width;
        window.Height = height;
        window.Show();
        try
        {
            pump(20);
            verify();
            pump(12);
            ScreenshotPngWriter.WriteElementPng(window, outputPath, Math.Max(1, (int)Math.Round(window.ActualWidth)), Math.Max(1, (int)Math.Round(window.ActualHeight)));
            writeOpenGlDiagnostics(outputPath, new DependencyObject?[] { window, window.Content as DependencyObject });
            return new CaptureResult(
                Math.Max(1, (int)Math.Round(window.ActualWidth)),
                Math.Max(1, (int)Math.Round(window.ActualHeight)),
                (DateTime.UtcNow - started).TotalMilliseconds);
        }
        finally
        {
            window.Close();
        }
    }

    internal static CaptureResult CaptureElement(FrameworkElement element, string outputPath, int width, int height)
    {
        DateTime started = DateTime.UtcNow;
        ScreenshotPngWriter.WriteElementPng(element, outputPath, width, height);
        return new CaptureResult(width, height, (DateTime.UtcNow - started).TotalMilliseconds);
    }
}
