using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

internal static class RecipeRunEvidenceImageBoundaryContract
{
    internal static int Run(string requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "Review",
            "OpenVisionRecipeRunEvidenceViewerView.xaml.cs");
        string factoryPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Viewer",
            "OpenVisionBitmapImagePreviewFactory.cs");
        string viewSource = File.ReadAllText(viewPath);
        string factorySource = File.ReadAllText(factoryPath);
        string imagePath = Path.Combine(evidenceDirectory, "evidence.png");
        List<string> results = new();

        Check(
            "Evidence View routes decoding through the existing preview factory",
            viewSource.Contains("OpenVisionBitmapImagePreviewFactory.LoadBitmap", StringComparison.Ordinal)
                && !viewSource.Contains("FileStream", StringComparison.Ordinal)
                && !viewSource.Contains("Image.FromStream", StringComparison.Ordinal),
            results);
        Check(
            "Preview factory owns the file-backed Bitmap decode",
            factorySource.Contains("public static Bitmap LoadBitmap", StringComparison.Ordinal)
                && factorySource.Contains("FileStream", StringComparison.Ordinal)
                && factorySource.Contains("Image.FromStream", StringComparison.Ordinal),
            results);

        using (Bitmap source = new Bitmap(7, 5))
        {
            source.Save(imagePath, ImageFormat.Png);
        }

        using (Bitmap loaded = OpenVisionBitmapImagePreviewFactory.LoadBitmap(imagePath, "contract"))
        {
            Check("Valid evidence image preserves pixel dimensions", loaded.Width == 7 && loaded.Height == 5, results);
        }

        bool missingFileWrapped = false;
        try
        {
            using Bitmap ignored = OpenVisionBitmapImagePreviewFactory.LoadBitmap(
                Path.Combine(evidenceDirectory, "missing.png"),
                "contract");
        }
        catch (InvalidOperationException exception)
        {
            missingFileWrapped = exception.Message.Contains("contract image could not be loaded", StringComparison.Ordinal);
        }

        Check("Missing evidence image keeps the role-specific failure contract", missingFileWrapped, results);
        File.WriteAllLines(Path.Combine(evidenceDirectory, "recipe-run-evidence-image-boundary-contract.txt"), results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "RECIPE_RUN_EVIDENCE_IMAGE_BOUNDARY_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count);
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
