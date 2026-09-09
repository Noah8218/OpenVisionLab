using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class NamespaceProjectBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl21-namespace-project-boundary-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Namespace boundary evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        string storagePath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "Property", "ParameterPropertyStorage.cs");
        string propertyPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "Property", "ParameterProperty.cs");
        string compatibilityPath = Path.Combine(repositoryRoot, "tools", "RecipeXmlCompatibilityCheck", "Program.cs");
        string storage = File.ReadAllText(storagePath);
        string property = File.ReadAllText(propertyPath);
        string compatibility = File.ReadAllText(compatibilityPath);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        Check(
            "storage owner follows the folder namespace",
            storage.Contains("namespace OpenVisionLab.Property", StringComparison.Ordinal)
                && !storage.Contains("namespace OpenVisionLab\r\n", StringComparison.Ordinal)
                && !storage.Contains("namespace OpenVisionLab\n", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "storage owner remains internal and static",
            storage.Contains("internal static class ParameterPropertyStorage", StringComparison.Ordinal)
                && !storage.Contains("public static class ParameterPropertyStorage", StringComparison.Ordinal)
                && !storage.Contains("public class ParameterPropertyStorage", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "legacy ParameterProperty type remains in its public namespace",
            property.Contains("namespace OpenVisionLab", StringComparison.Ordinal)
                && property.Contains("[System.Xml.Serialization.XmlRoot(\"CPropertyParam\")]", StringComparison.Ordinal)
                && property.Contains("using OpenVisionLab.Property;", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "legacy model delegates persistence to the moved owner",
            property.Contains("ParameterPropertyStorage.Load(this, strName)", StringComparison.Ordinal)
                && property.Contains("ParameterPropertyStorage.Save(this, strName)", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Recipe XML compatibility map keeps the legacy qualified type",
            compatibility.Contains("new XmlRootCheck(\"OpenVisionLab.ParameterProperty\", \"CPropertyParam\"", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "moved owner has no public reflection or serializer type contract",
            !storage.Contains("Type.GetType", StringComparison.Ordinal)
                && !storage.Contains("AssemblyQualifiedName", StringComparison.Ordinal)
                && !storage.Contains("XmlSerializer", StringComparison.Ordinal)
                && !storage.Contains("XmlRoot", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "moved owner is not referenced by XAML",
            !Directory.EnumerateFiles(Path.Combine(repositoryRoot, "src"), "*.xaml", SearchOption.AllDirectories)
                .Any(path => File.ReadAllText(path).Contains("ParameterPropertyStorage", StringComparison.Ordinal)),
            passed,
            failed);

        List<(string Path, string Text)> sourceFiles = Directory
            .EnumerateFiles(repositoryRoot, "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains("\\bin\\", StringComparison.OrdinalIgnoreCase)
                && !path.Contains("\\obj\\", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(Path.GetFileName(path), "NamespaceProjectBoundaryContract.cs", StringComparison.OrdinalIgnoreCase))
            .Select(path => (path, File.ReadAllText(path)))
            .ToList();
        List<string> referencingFiles = sourceFiles
            .Where(item => item.Text.Contains("ParameterPropertyStorage", StringComparison.Ordinal))
            .Select(item => Path.GetRelativePath(repositoryRoot, item.Path))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
        Check(
            "storage owner has exactly one production caller",
            referencingFiles.Count == 2
                && referencingFiles.Any(path => path.Equals(Path.Combine("src", "OpenVisionLab", "Property", "ParameterPropertyStorage.cs"), StringComparison.OrdinalIgnoreCase))
                && referencingFiles.Any(path => path.Equals(Path.Combine("src", "OpenVisionLab", "Property", "ParameterProperty.cs"), StringComparison.OrdinalIgnoreCase)),
            passed,
            failed);

        string templateExtractionPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "Property", "TemplateImageExtraction.cs");
        string legacyTemplateExtractionPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "Common", "TemplateImageExtraction.cs");
        string templateExtraction = File.Exists(templateExtractionPath) ? File.ReadAllText(templateExtractionPath) : string.Empty;
        Check(
            "template extraction owner follows the property boundary",
            File.Exists(templateExtractionPath)
                && !File.Exists(legacyTemplateExtractionPath)
                && templateExtraction.Contains("namespace OpenVisionLab.Property", StringComparison.Ordinal)
                && templateExtraction.Contains("internal static class TemplateImageExtraction", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "template extraction owner has no public serialization contract",
            !templateExtraction.Contains("public static class TemplateImageExtraction", StringComparison.Ordinal)
                && !templateExtraction.Contains("Type.GetType", StringComparison.Ordinal)
                && !templateExtraction.Contains("XmlSerializer", StringComparison.Ordinal)
                && !templateExtraction.Contains("XmlRoot", StringComparison.Ordinal),
            passed,
            failed);

        List<string> templateExtractionReferences = sourceFiles
            .Where(item => item.Text.Contains("TemplateImageExtraction", StringComparison.Ordinal))
            .Select(item => Path.GetRelativePath(repositoryRoot, item.Path))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
        Check(
            "template extraction references remain limited to its property workflow",
            templateExtractionReferences.Count == 5
                && templateExtractionReferences.Any(path => path.Equals(Path.Combine("src", "OpenVisionLab", "Common", "PropertyGridImageEditorService.cs"), StringComparison.OrdinalIgnoreCase))
                && templateExtractionReferences.Any(path => path.Equals(Path.Combine("src", "OpenVisionLab", "Property", "TemplateImageExtraction.cs"), StringComparison.OrdinalIgnoreCase))
                && templateExtractionReferences.Any(path => path.Equals(Path.Combine("src", "OpenVisionLab", "UI", "Popup", "Wpf", "OpenGlTemplateEditorWindow.xaml.cs"), StringComparison.OrdinalIgnoreCase))
                && templateExtractionReferences.Any(path => path.Equals(Path.Combine("tools", "LocatorRelativeBlobSkillSmoke", "Program.cs"), StringComparison.OrdinalIgnoreCase))
                && templateExtractionReferences.Any(path => path.Equals(Path.Combine("tools", "PipelineViewerScreenshotSmoke", "Program.cs"), StringComparison.OrdinalIgnoreCase)),
            passed,
            failed);
        Check(
            "template extraction owner is not referenced by XAML",
            !Directory.EnumerateFiles(Path.Combine(repositoryRoot, "src"), "*.xaml", SearchOption.AllDirectories)
                .Any(path => File.ReadAllText(path).Contains("TemplateImageExtraction", StringComparison.Ordinal)),
            passed,
            failed);

        string outputPath = Path.Combine(evidenceDirectory, "namespace-project-boundary-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-21 compatibility-safe namespace/project boundary",
                "RepositoryRoot: " + repositoryRoot,
                "EvidenceDirectory: " + evidenceDirectory,
                "StorageReferences: " + string.Join(", ", referencingFiles),
                "TemplateExtractionReferences: " + string.Join(", ", templateExtractionReferences)
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
            "CONTRACT|namespace-project-boundary|passed="
            + passed.Count
            + "|failed="
            + failed.Count);
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static string ResolveRepositoryRoot()
    {
        foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            DirectoryInfo? current = new DirectoryInfo(Path.GetFullPath(start));
            while (current != null)
            {
                string candidate = Path.Combine(current.FullName, "src", "OpenVisionLab", "Property", "ParameterPropertyStorage.cs");
                if (File.Exists(candidate))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root could not be located.");
    }

    private static void Check(string name, bool condition, List<string> passed, List<string> failed)
    {
        if (condition)
        {
            passed.Add(name);
        }
        else
        {
            failed.Add(name);
        }
    }
}
