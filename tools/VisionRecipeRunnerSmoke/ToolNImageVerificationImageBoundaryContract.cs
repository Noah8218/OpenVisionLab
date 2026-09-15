using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

internal static class ToolNImageVerificationImageBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            requestedEvidenceDirectory
                ?? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "tool-n-image-verification-image-boundary-20260913"));
        Directory.CreateDirectory(evidenceDirectory);

        string controllerPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "NativeTools",
            "Review",
            "VisionToolNImageVerificationController.cs");
        string factoryPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Viewer",
            "OpenVisionBitmapImagePreviewFactory.cs");
        string controllerSource = File.ReadAllText(controllerPath);
        string factorySource = File.ReadAllText(factoryPath);
        List<string> results = new List<string>();

        Check(
            "N-image controller delegates selected evidence decoding to the existing factory",
            controllerSource.Contains("OpenVisionBitmapImagePreviewFactory.TryCreateFromPath", StringComparison.Ordinal)
                && !controllerSource.Contains("new FileStream", StringComparison.Ordinal)
                && !controllerSource.Contains("BitmapCacheOption.OnLoad", StringComparison.Ordinal)
                && !controllerSource.Contains("image.StreamSource", StringComparison.Ordinal),
            results);
        Check(
            "Existing factory owns BitmapImage path decode and frozen OnLoad lifetime",
            factorySource.Contains("public static BitmapImage TryCreateFromPath", StringComparison.Ordinal)
                && factorySource.Contains("BitmapCacheOption.OnLoad", StringComparison.Ordinal)
                && factorySource.Contains("image.Freeze();", StringComparison.Ordinal),
            results);
        Check(
            "N-image selected preview binding types remain BitmapImage",
            controllerSource.Contains("public BitmapImage SelectedSourceImage", StringComparison.Ordinal)
                && controllerSource.Contains("public BitmapImage SelectedDrawingImage", StringComparison.Ordinal),
            results);

        string imagePath = Path.Combine(evidenceDirectory, "verification-preview.bmp");
        using (Bitmap bitmap = new Bitmap(9, 6, PixelFormat.Format24bppRgb))
        {
            bitmap.SetPixel(0, 0, Color.LimeGreen);
            bitmap.Save(imagePath, ImageFormat.Bmp);
        }

        System.Windows.Media.Imaging.BitmapImage source =
            OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(imagePath, decodePixelWidth: 0);
        Check(
            "Valid N-image preview preserves dimensions and frozen lifetime",
            source != null && source.PixelWidth == 9 && source.PixelHeight == 6 && source.IsFrozen,
            results);

        Check(
            "Missing N-image preview preserves null behavior",
            OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(
                Path.Combine(evidenceDirectory, "missing-preview.bmp"),
                decodePixelWidth: 0) == null,
            results);

        string corruptPath = Path.Combine(evidenceDirectory, "corrupt-preview.bmp");
        File.WriteAllText(corruptPath, "not-an-image", System.Text.Encoding.UTF8);
        Check(
            "Corrupt N-image preview preserves null behavior",
            OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(corruptPath, decodePixelWidth: 0) == null,
            results);

        string outputPath = Path.Combine(
            evidenceDirectory,
            "tool-n-image-verification-image-boundary-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: PL-0031 N-image verification image boundary",
                "EvidenceDirectory: " + evidenceDirectory
            }.Concat(results));

        foreach (string result in results)
        {
            Console.WriteLine(result);
        }

        bool passed = results.All(result => result.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "TOOL_N_IMAGE_VERIFICATION_IMAGE_BOUNDARY_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count);
        Console.WriteLine(outputPath);
        return passed ? 0 : 1;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }

    private static string ResolveRepositoryRoot()
    {
        DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }
}
