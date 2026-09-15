using System;
using System.Collections.Generic;
using System.IO;

internal static class TemplateEditorImageBoundaryContract
{
    internal static int Run(string requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string templateViewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Popup",
            "Wpf",
            "OpenGlTemplateEditorWindow.xaml.cs");
        string roiViewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Popup",
            "Wpf",
            "RoiEditorWindow.xaml.cs");
        string factoryPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Viewer",
            "OpenVisionBitmapImagePreviewFactory.cs");

        string templateViewSource = File.ReadAllText(templateViewPath);
        string roiViewSource = File.ReadAllText(roiViewPath);
        string factorySource = File.ReadAllText(factoryPath);
        List<string> results = new();

        Check(
            "Template editor routes pattern decoding through the existing image owner",
            templateViewSource.Contains("OpenVisionBitmapImagePreviewFactory.LoadBitmap", StringComparison.Ordinal)
                && !templateViewSource.Contains("File.Exists(imagePath)", StringComparison.Ordinal)
                && !templateViewSource.Contains("new Bitmap(imagePath)", StringComparison.Ordinal),
            results);
        Check(
            "ROI editor routes pattern decoding through the existing image owner",
            roiViewSource.Contains("OpenVisionBitmapImagePreviewFactory.LoadBitmap", StringComparison.Ordinal)
                && !roiViewSource.Contains("File.Exists(imagePath)", StringComparison.Ordinal)
                && !roiViewSource.Contains("new Bitmap(imagePath)", StringComparison.Ordinal),
            results);
        Check(
            "The existing factory remains the file-backed Bitmap owner",
            factorySource.Contains("public static Bitmap LoadBitmap", StringComparison.Ordinal)
                && factorySource.Contains("FileStream", StringComparison.Ordinal)
                && factorySource.Contains("Image.FromStream", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "template-editor-image-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "TEMPLATE_EDITOR_IMAGE_BOUNDARY_CONTRACT="
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
