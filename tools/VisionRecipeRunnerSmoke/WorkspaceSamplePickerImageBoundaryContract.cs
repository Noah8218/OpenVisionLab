using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class WorkspaceSamplePickerImageBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            requestedEvidenceDirectory
                ?? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "sample-picker-image-boundary-20260913"));
        Directory.CreateDirectory(evidenceDirectory);

        string viewModelPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Workspace",
            "Samples",
            "OpenVisionWorkspaceSamplePickerViewModel.cs");
        string factoryPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Viewer",
            "OpenVisionBitmapImagePreviewFactory.cs");
        string viewModelSource = File.ReadAllText(viewModelPath);
        string factorySource = File.ReadAllText(factoryPath);
        List<string> results = new List<string>();

        Check(
            "Sample Picker ViewModel delegates preview loading to the existing factory",
            viewModelSource.Contains("OpenVisionBitmapImagePreviewFactory.TryCreateFromPath", StringComparison.Ordinal)
                && !viewModelSource.Contains("File.Exists", StringComparison.Ordinal)
                && !viewModelSource.Contains("new BitmapImage", StringComparison.Ordinal)
                && !viewModelSource.Contains("BitmapCacheOption.OnLoad", StringComparison.Ordinal)
                && !viewModelSource.Contains("DecodePixelWidth", StringComparison.Ordinal),
            results);
        Check(
            "Existing factory owns optional path-preview decode and frozen OnLoad lifetime",
            factorySource.Contains("TryCreateFromPath", StringComparison.Ordinal)
                && factorySource.Contains("decodePixelWidth", StringComparison.Ordinal)
                && factorySource.Contains("BitmapCacheOption.OnLoad", StringComparison.Ordinal)
                && factorySource.Contains("image.Freeze();", StringComparison.Ordinal),
            results);
        Check(
            "SelectedImageSource binding facade remains unchanged",
            viewModelSource.Contains("public ImageSource SelectedImageSource => LoadImageSource(SelectedSample?.ImageFullPath);", StringComparison.Ordinal)
                && viewModelSource.Contains("private static ImageSource LoadImageSource(string path)", StringComparison.Ordinal),
            results);

        string imagePath = Path.Combine(evidenceDirectory, "sample-preview.bmp");
        using (Bitmap bitmap = new Bitmap(8, 4, PixelFormat.Format24bppRgb))
        {
            bitmap.SetPixel(0, 0, Color.Red);
            bitmap.Save(imagePath, ImageFormat.Bmp);
        }

        System.Windows.Media.Imaging.BitmapSource source =
            OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(imagePath, 420);
        Check("Valid sample preview is loaded and frozen", source != null && source.IsFrozen, results);

        string missingPath = Path.Combine(evidenceDirectory, "missing-preview.bmp");
        Check(
            "Missing sample preview preserves null behavior",
            OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(missingPath, 420) == null,
            results);

        string corruptPath = Path.Combine(evidenceDirectory, "corrupt-preview.bmp");
        File.WriteAllText(corruptPath, "not-an-image", System.Text.Encoding.UTF8);
        Check(
            "Corrupt sample preview preserves null behavior",
            OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(corruptPath, 420) == null,
            results);

        string outputPath = Path.Combine(evidenceDirectory, "workspace-sample-picker-image-boundary-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: PL-0030 Sample Picker image preview boundary",
                "EvidenceDirectory: " + evidenceDirectory
            }.Concat(results));

        foreach (string result in results)
        {
            Console.WriteLine(result);
        }

        bool passed = results.All(result => result.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "WORKSPACE_SAMPLE_PICKER_IMAGE_BOUNDARY_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count.ToString(CultureInfo.InvariantCulture));
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
