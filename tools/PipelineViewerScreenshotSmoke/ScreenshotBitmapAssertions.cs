using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

internal static class ScreenshotBitmapAssertions
{
    internal static void AssertBitmapPresent(Bitmap bitmap, string name)
    {
        if (bitmap == null || bitmap.Width <= 0 || bitmap.Height <= 0)
        {
            throw new InvalidOperationException(name + " was not available.");
        }
    }

    internal static void SaveDiagnosticBitmap(string outputPath, string fileName, Bitmap bitmap)
    {
        AssertBitmapPresent(bitmap, fileName);
        string? parentDirectory = Path.GetDirectoryName(outputPath);
        string outputName = Path.GetFileNameWithoutExtension(outputPath);
        string diagnosticsDirectory = Path.Combine(
            string.IsNullOrWhiteSpace(parentDirectory) ? "." : parentDirectory,
            string.IsNullOrWhiteSpace(outputName) ? "diagnostics" : outputName + ".diagnostics");
        Directory.CreateDirectory(diagnosticsDirectory);
        bitmap.Save(Path.Combine(diagnosticsDirectory, fileName), ImageFormat.Png);
    }

    internal static void AssertBitmapVisiblyDifferent(Bitmap expectedSource, Bitmap actualOutput, string name)
    {
        AssertBitmapPresent(expectedSource, name + " source");
        AssertBitmapPresent(actualOutput, name + " output");
        int width = Math.Min(expectedSource.Width, actualOutput.Width);
        int height = Math.Min(expectedSource.Height, actualOutput.Height);
        int sampled = 0;
        int changed = 0;
        long totalDelta = 0;

        for (int y = 0; y < height; y += 4)
        {
            for (int x = 0; x < width; x += 4)
            {
                Color left = expectedSource.GetPixel(x, y);
                Color right = actualOutput.GetPixel(x, y);
                int delta = Math.Abs(left.R - right.R) + Math.Abs(left.G - right.G) + Math.Abs(left.B - right.B);
                sampled++;
                totalDelta += delta;
                if (delta >= 60)
                {
                    changed++;
                }
            }
        }

        double changedRatio = sampled <= 0 ? 0D : changed / (double)sampled;
        double averageDelta = sampled <= 0 ? 0D : totalDelta / (double)sampled;
        if (changedRatio < 0.04D || averageDelta < 18D)
        {
            throw new InvalidOperationException(
                name + " did not change enough to be teachable. "
                + $"ChangedRatio={changedRatio:0.000}, AverageDelta={averageDelta:0.0}, Sampled={sampled}");
        }
    }

    internal static void AssertBitmapPreviewOverlayDifferentFromMain(Bitmap expectedSource, Bitmap actualOutput, string name)
    {
        AssertBitmapPresent(expectedSource, name + " source");
        AssertBitmapPresent(actualOutput, name + " output");
        int width = Math.Min(expectedSource.Width, actualOutput.Width);
        int height = Math.Min(expectedSource.Height, actualOutput.Height);
        int sampled = 0;
        int changed = 0;
        long totalDelta = 0;

        for (int y = 0; y < height; y += 4)
        {
            for (int x = 0; x < width; x += 4)
            {
                Color left = expectedSource.GetPixel(x, y);
                Color right = actualOutput.GetPixel(x, y);
                int delta = Math.Abs(left.R - right.R) + Math.Abs(left.G - right.G) + Math.Abs(left.B - right.B);
                sampled++;
                totalDelta += delta;
                if (delta >= 60)
                {
                    changed++;
                }
            }
        }

        double changedRatio = sampled <= 0 ? 0D : changed / (double)sampled;
        double averageDelta = sampled <= 0 ? 0D : totalDelta / (double)sampled;
        if (changedRatio < 0.008D || averageDelta < 6D)
        {
            throw new InvalidOperationException(
                name + " did not show enough preview overlay/change. "
                + $"ChangedRatio={changedRatio:0.000}, AverageDelta={averageDelta:0.0}, Sampled={sampled}");
        }
    }

    internal static void AssertBitmapRetainsSourceBackground(Bitmap expectedSource, Bitmap actualOutput, string name)
    {
        AssertBitmapPresent(expectedSource, name + " source");
        AssertBitmapPresent(actualOutput, name + " output");
        int width = Math.Min(expectedSource.Width, actualOutput.Width);
        int height = Math.Min(expectedSource.Height, actualOutput.Height);
        int sampled = 0;
        int changed = 0;
        long totalDelta = 0;

        for (int y = 0; y < height; y += 4)
        {
            for (int x = 0; x < width; x += 4)
            {
                Color left = expectedSource.GetPixel(x, y);
                Color right = actualOutput.GetPixel(x, y);
                int delta = Math.Abs(left.R - right.R) + Math.Abs(left.G - right.G) + Math.Abs(left.B - right.B);
                sampled++;
                totalDelta += delta;
                if (delta >= 60)
                {
                    changed++;
                }
            }
        }

        double changedRatio = sampled <= 0 ? 0D : changed / (double)sampled;
        double averageDelta = sampled <= 0 ? 0D : totalDelta / (double)sampled;
        if (changedRatio > 0.18D || averageDelta > 42D)
        {
            throw new InvalidOperationException(
                name + " replaced too much of the source with a processed image. "
                + $"ChangedRatio={changedRatio:0.000}, AverageDelta={averageDelta:0.0}, Sampled={sampled}");
        }
    }

    internal static void AssertBitmapBinaryLike(Bitmap bitmap, string name)
    {
        AssertBitmapPresent(bitmap, name);
        int sampled = 0;
        int dark = 0;
        int light = 0;
        int middle = 0;

        for (int y = 0; y < bitmap.Height; y += 4)
        {
            for (int x = 0; x < bitmap.Width; x += 4)
            {
                Color color = bitmap.GetPixel(x, y);
                int value = (color.R + color.G + color.B) / 3;
                sampled++;
                if (value <= 35)
                {
                    dark++;
                }
                else if (value >= 220)
                {
                    light++;
                }
                else
                {
                    middle++;
                }
            }
        }

        double binaryRatio = sampled <= 0 ? 0D : (dark + light) / (double)sampled;
        double darkRatio = sampled <= 0 ? 0D : dark / (double)sampled;
        double lightRatio = sampled <= 0 ? 0D : light / (double)sampled;
        if (binaryRatio < 0.82D || darkRatio < 0.03D || lightRatio < 0.03D)
        {
            throw new InvalidOperationException(
                name + " is not binary-like enough for Blob teaching. "
                + $"BinaryRatio={binaryRatio:0.000}, DarkRatio={darkRatio:0.000}, LightRatio={lightRatio:0.000}, Middle={middle}, Sampled={sampled}");
        }
    }

    internal static void AssertBitmapMostlyGrayscale(Bitmap bitmap, string name)
    {
        AssertBitmapPresent(bitmap, name);
        int sampled = 0;
        int colored = 0;
        for (int y = 0; y < bitmap.Height; y += 2)
        {
            for (int x = 0; x < bitmap.Width; x += 2)
            {
                Color color = bitmap.GetPixel(x, y);
                int max = Math.Max(color.R, Math.Max(color.G, color.B));
                int min = Math.Min(color.R, Math.Min(color.G, color.B));
                sampled++;
                if (max - min > 20)
                {
                    colored++;
                }
            }
        }

        double coloredRatio = sampled <= 0 ? 0D : colored / (double)sampled;
        if (coloredRatio > 0.001D)
        {
            throw new InvalidOperationException(
                name + " contains colored overlay pixels before Run. "
                + $"ColoredRatio={coloredRatio:0.0000}, Colored={colored}, Sampled={sampled}");
        }
    }

    internal static void AssertBitmapContainsColorNear(Bitmap bitmap, Color expected, int tolerance, string name)
    {
        AssertBitmapPresent(bitmap, name);
        int sampled = 0;
        int matched = 0;
        int step = Math.Max(1, Math.Min(bitmap.Width, bitmap.Height) / 240);

        for (int y = 0; y < bitmap.Height; y += step)
        {
            for (int x = 0; x < bitmap.Width; x += step)
            {
                Color color = bitmap.GetPixel(x, y);
                sampled++;
                if (Math.Abs(color.R - expected.R) <= tolerance
                    && Math.Abs(color.G - expected.G) <= tolerance
                    && Math.Abs(color.B - expected.B) <= tolerance)
                {
                    matched++;
                }
            }
        }

        double matchedRatio = sampled <= 0 ? 0D : matched / (double)sampled;
        if (matched < 8 || matchedRatio < 0.00008D)
        {
            throw new InvalidOperationException(
                name + " did not contain enough expected draw-color pixels. "
                + $"Matched={matched}, Ratio={matchedRatio:0.000000}, Sampled={sampled}, "
                + $"Expected=R{expected.R} G{expected.G} B{expected.B}");
        }
    }
}