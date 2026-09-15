using System;
using System.Collections.Generic;
using System.IO;
using OpenVisionLab.ImageCanvas;

internal static class RoiImageCanvasPathPolicyContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "roi-image-canvas-path-policy-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewModelPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "OpenVisionLab.ImageCanvas",
            "ViewModel",
            "RoiImageCanvasViewModel.cs");
        string policyPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "OpenVisionLab.ImageCanvas",
            "Util",
            "ImageCanvasDirectoryPolicy.cs");
        string viewModelSource = File.ReadAllText(viewModelPath);
        string policySource = File.ReadAllText(policyPath);
        List<string> results = new();

        Check(
            "Existing directory policy owns image-name and save-name policy",
            policySource.Contains("ResolveImageName", StringComparison.Ordinal)
                && policySource.Contains("Path.GetFileNameWithoutExtension", StringComparison.Ordinal)
                && policySource.Contains("CreateDefaultSaveFileName", StringComparison.Ordinal)
                && policySource.Contains("Path.GetInvalidFileNameChars", StringComparison.Ordinal),
            results);
        Check(
            "ViewModel delegates both path-policy call paths",
            Count(viewModelSource, "ImageCanvasDirectoryPolicy.ResolveImageName(fileName)") == 2
                && viewModelSource.Contains(
                    "ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName(_currentImageName)",
                    StringComparison.Ordinal),
            results);
        Check(
            "ViewModel no longer imports or calls System.IO path APIs",
            !viewModelSource.Contains("using System.IO;", StringComparison.Ordinal)
                && !viewModelSource.Contains("System.IO.Path", StringComparison.Ordinal)
                && !viewModelSource.Contains("Path.GetFileNameWithoutExtension", StringComparison.Ordinal)
                && !viewModelSource.Contains("Path.GetInvalidFileNameChars", StringComparison.Ordinal)
                && !viewModelSource.Contains("private string CreateDefaultSaveFileName", StringComparison.Ordinal),
            results);
        Check(
            "Null and whitespace image names preserve the existing default",
            ImageCanvasDirectoryPolicy.ResolveImageName(null) == "Image"
                && ImageCanvasDirectoryPolicy.ResolveImageName("   ") == "Image"
                && ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName(null) == "Image.png",
            results);
        Check(
            "Image path extraction preserves the existing filename stem",
            ImageCanvasDirectoryPolicy.ResolveImageName("C:\\Images\\Inspection.001.png") == "Inspection.001"
                && ImageCanvasDirectoryPolicy.ResolveImageName("C:\\Images\\Inspection") == "Inspection",
            results);
        Check(
            "Save filename sanitization preserves the existing PNG contract",
            ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName("Inspection:01*") == "Inspection_01_.png"
                && ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName("Inspection") == "Inspection.png",
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "roi-image-canvas-path-policy-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "ROI_IMAGE_CANVAS_PATH_POLICY_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count);
        return passed ? 0 : 1;
    }

    private static int Count(string value, string token)
    {
        int count = 0;
        int index = 0;
        while ((index = value.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += token.Length;
        }

        return count;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }

    private static string ResolveRepositoryRoot()
    {
        DirectoryInfo? current = new DirectoryInfo(Environment.CurrentDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        current = new DirectoryInfo(AppContext.BaseDirectory);
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
