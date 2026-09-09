using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrawingSize = System.Drawing.Size;
using Graphics = System.Drawing.Graphics;
using WpfPoint = System.Windows.Point;
using WpfSize = System.Windows.Size;

internal static class ScreenshotPngWriter
{
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
