using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

internal static class ImageCompareResourceContract
{
    public static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "image_compare_resource_contract_" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);

        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        string pngPath = Path.Combine(evidenceDirectory, "compare-resource.png");
        string bmpPath = Path.Combine(evidenceDirectory, "compare-resource.bmp");
        string missingPath = Path.Combine(evidenceDirectory, "missing-image.png");

        try
        {
            CreateImage(pngPath, ImageFormat.Png, 11, 7);
            CreateImage(bmpPath, ImageFormat.Bmp, 13, 5);

            ImageCompareImageResource resource = ImageCompareImageResource.Load(pngPath);
            try
            {
                Require(resource.Bitmap.Width == 11 && resource.Bitmap.Height == 7, "PNG resource dimensions changed.");
                Require(resource.Source.PixelWidth == 11 && resource.Source.PixelHeight == 7, "PNG WPF source dimensions changed.");
                Require(resource.Source.IsFrozen, "PNG WPF source was not frozen.");
                Require(resource.FormatText.StartsWith("PNG ", StringComparison.Ordinal), "PNG format metadata was not resolved by the resource owner.");
                observations.Add("resource load: decoded Bitmap, frozen BitmapSource, and PNG metadata");
            }
            finally
            {
                resource.Dispose();
                resource.Dispose();
            }

            ImageCompareSlotViewModel slot = new ImageCompareSlotViewModel(0);
            slot.Load(pngPath);
            Bitmap previousBitmap = slot.Bitmap;
            BitmapSource previousSource = slot.Source;
            Require(slot.IsLoaded, "Slot did not expose a loaded PNG resource.");
            Require(previousSource != null && previousSource.IsFrozen, "Slot did not expose the frozen resource source.");

            slot.Load(bmpPath);
            Require(!CanReadBitmap(previousBitmap), "Replacing a slot left the previous Bitmap undisposed.");
            Require(slot.IsLoaded && slot.Width == 13 && slot.Height == 5, "Slot replacement did not expose the new BMP resource.");
            Require(slot.HeaderText.Contains("BMP ", StringComparison.Ordinal), "Slot did not project the resource format metadata.");
            observations.Add("slot replacement: previous Bitmap disposed before new resource projection");

            slot.Load(missingPath);
            Require(slot.Bitmap == null && slot.Source == null && !slot.IsLoaded, "Invalid path did not clear the slot resource.");
            slot.Dispose();
            slot.Dispose();
            observations.Add("invalid path and repeated Dispose: slot is empty and idempotent");
        }
        catch (Exception exception)
        {
            failures.Add(exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(evidenceDirectory, "image_compare_resource_contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: ImageCompare image file decoding, WPF source ownership, replacement disposal, and invalid-path reset",
                "EvidenceDirectory: " + evidenceDirectory,
                "ResourceOwner: ImageCompareImageResource",
                "SlotOwner: ImageCompareSlotViewModel binding state and resource projection"
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Image Compare resource contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Image Compare resource contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static void CreateImage(string path, ImageFormat format, int width, int height)
    {
        using Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bitmap.SetPixel(x, y, Color.FromArgb(30 + x, 60 + y, 90));
            }
        }

        bitmap.Save(path, format);
    }

    private static bool CanReadBitmap(Bitmap bitmap)
    {
        try
        {
            _ = bitmap.GetPixel(0, 0);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
