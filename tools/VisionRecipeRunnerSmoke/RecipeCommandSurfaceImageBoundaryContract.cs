using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

internal static class RecipeCommandSurfaceImageBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "recipe-command-surface-image-boundary_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string imagePath = Path.Combine(evidenceDirectory, "image-fixture.bmp");

        try
        {
            Require(
                string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
                "Image boundary evidence must be on D:.");
            using (Bitmap bitmap = new Bitmap(3, 2, PixelFormat.Format24bppRgb))
            {
                bitmap.SetPixel(0, 0, Color.Red);
                bitmap.SetPixel(1, 0, Color.Green);
                bitmap.SetPixel(2, 0, Color.Blue);
                bitmap.Save(imagePath, ImageFormat.Bmp);
            }

            BitmapSource source = OpenVisionBitmapImagePreviewFactory.CreateFromPath(imagePath);
            Require(source != null && source.IsFrozen, "Path preview was not loaded as a frozen BitmapSource.");
            passed.Add("existing preview factory loads a file-backed BitmapSource with OnLoad lifetime");

            (int width, int height) = OpenVisionBitmapImagePreviewFactory.ReadPixelSize(imagePath);
            Require(width == 3 && height == 2, "Pixel-size reader did not preserve the source dimensions.");
            passed.Add("existing preview factory reads source pixel dimensions");

            string missingPath = Path.Combine(evidenceDirectory, "missing-image.bmp");
            bool missingPreviewFailed = false;
            try
            {
                OpenVisionBitmapImagePreviewFactory.CreateFromPath(missingPath);
            }
            catch (FileNotFoundException)
            {
                missingPreviewFailed = true;
            }

            Require(missingPreviewFailed, "Missing overlay path did not fail through the existing file error contract.");
            passed.Add("missing preview path preserves FileNotFoundException behavior");

            bool missingSizeFailed = false;
            try
            {
                OpenVisionBitmapImagePreviewFactory.ReadPixelSize(missingPath);
            }
            catch (Exception)
            {
                missingSizeFailed = true;
            }

            Require(missingSizeFailed, "Missing pixel-size path did not fail through the existing decode error path.");
            passed.Add("missing pixel-size path preserves failure behavior for callers that map decode errors");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }

        string outputPath = Path.Combine(
            evidenceDirectory,
            "recipe-command-surface-image-boundary-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: PL-0026 RecipeCommandSurface image boundary",
                "EvidenceDirectory: " + evidenceDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            "CONTRACT|recipe-command-surface-image-boundary|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
