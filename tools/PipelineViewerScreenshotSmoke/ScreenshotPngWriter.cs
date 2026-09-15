using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrawingSize = System.Drawing.Size;
using Graphics = System.Drawing.Graphics;
using WpfPoint = System.Windows.Point;
using WpfSize = System.Windows.Size;

internal static class ScreenshotPngWriter
{
    internal static void WriteDpiAwareWindowPng(Window window, string outputPath)
    {
        double scaleX = 1D;
        double scaleY = 1D;
        PresentationSource source = PresentationSource.FromVisual(window);
        if (source?.CompositionTarget != null)
        {
            Matrix transform = source.CompositionTarget.TransformToDevice;
            scaleX = transform.M11;
            scaleY = transform.M22;
        }

        int pixelWidth = Math.Max(1, (int)Math.Ceiling(window.ActualWidth * scaleX));
        int pixelHeight = Math.Max(1, (int)Math.Ceiling(window.ActualHeight * scaleY));
        RenderTargetBitmap renderTarget = new RenderTargetBitmap(
            pixelWidth,
            pixelHeight,
            96D * scaleX,
            96D * scaleY,
            PixelFormats.Pbgra32);
        renderTarget.Render(window);

        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderTarget));
        using (FileStream stream = File.Create(outputPath))
        {
            encoder.Save(stream);
        }
    }

    internal static void WriteScreenPng(Window window, string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        window.UpdateLayout();
        WpfPoint topLeft = window.PointToScreen(new WpfPoint(0D, 0D));
        int width = Math.Max(1, (int)Math.Round(window.ActualWidth));
        int height = Math.Max(1, (int)Math.Round(window.ActualHeight));
        using Bitmap bitmap = new(width, height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(
            Math.Max(0, (int)Math.Round(topLeft.X)),
            Math.Max(0, (int)Math.Round(topLeft.Y)),
            0,
            0,
            new DrawingSize(width, height));
        bitmap.Save(outputPath, ImageFormat.Png);
    }

    internal static void WriteWindowScreenPng(Window window, string outputPath)
    {
        System.Windows.Point topLeft = window.PointToScreen(new WpfPoint(0D, 0D));
        System.Windows.Point bottomRight = window.PointToScreen(new WpfPoint(window.ActualWidth, window.ActualHeight));
        int pixelWidth = Math.Max(1, (int)Math.Ceiling(bottomRight.X - topLeft.X));
        int pixelHeight = Math.Max(1, (int)Math.Ceiling(bottomRight.Y - topLeft.Y));
        using (Bitmap bitmap = new Bitmap(pixelWidth, pixelHeight))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(
                (int)Math.Floor(topLeft.X),
                (int)Math.Floor(topLeft.Y),
                0,
                0,
                new DrawingSize(pixelWidth, pixelHeight));
            bitmap.Save(outputPath, ImageFormat.Png);
        }
    }

    internal static void WriteWindowsScreenPng(IEnumerable<Window> windows, string outputPath)
    {
        List<Window> visibleWindows = windows
            .Where(window => window?.IsVisible == true)
            .Distinct()
            .ToList();
        if (visibleWindows.Count == 0)
        {
            throw new InvalidOperationException("No visible windows were supplied for screen capture.");
        }

        Rect union = Rect.Empty;
        foreach (Window window in visibleWindows)
        {
            WpfPoint topLeft = window.PointToScreen(new WpfPoint(0D, 0D));
            WpfPoint bottomRight = window.PointToScreen(new WpfPoint(window.ActualWidth, window.ActualHeight));
            Rect bounds = new Rect(topLeft, bottomRight);
            union = union.IsEmpty ? bounds : Rect.Union(union, bounds);
        }

        int pixelWidth = Math.Max(1, (int)Math.Ceiling(union.Width));
        int pixelHeight = Math.Max(1, (int)Math.Ceiling(union.Height));
        using (Bitmap bitmap = new Bitmap(pixelWidth, pixelHeight))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(
                (int)Math.Floor(union.X),
                (int)Math.Floor(union.Y),
                0,
                0,
                new DrawingSize(pixelWidth, pixelHeight));
            bitmap.Save(outputPath, ImageFormat.Png);
        }
    }

    internal static void WriteElementPng(FrameworkElement element, string outputPath, int width, int height)
    {
        width = Math.Max(1, width);
        height = Math.Max(1, height);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        element.Measure(new WpfSize(width, height));
        element.Arrange(new Rect(0, 0, width, height));
        element.UpdateLayout();

        RenderTargetBitmap bitmap = new(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(element);
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using FileStream stream = File.Create(outputPath);
        encoder.Save(stream);
    }

    internal static void WriteVisibleElementPng(FrameworkElement element, string outputPath)
    {
        int width = Math.Max(1, (int)Math.Round(element.ActualWidth));
        int height = Math.Max(1, (int)Math.Round(element.ActualHeight));
        RenderTargetBitmap bitmap = new(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(element);
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using FileStream stream = File.Create(outputPath);
        encoder.Save(stream);
    }
}
