using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

internal static class PropertyGridMetadataAdapterContract
{
    private const string ExpectedMetadataTokenHash = "fcd108d34b7a6a24204e3f9dc6ac60378206d7e7e6a09c49faad482713d56c0f";

    private static readonly string[] MetadataDeclarations =
    {
        "internal static class BridgeCategoryOrderMap",
        "internal sealed class BridgePropertyComparer",
        "internal sealed class BridgeCategoryComparer",
        "internal sealed class DynamicPropertyGridTypeDescriptionProvider",
        "internal sealed class DynamicPropertyGridTypeDescriptor",
        "internal sealed class LocalizedPropertyDescriptor",
        "internal static class PropertyGridLocalization"
    };

    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl36_property_grid_metadata_adapter_contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);

        string adapterPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "WpfPropertyGridBridge",
            "WpfPropertyGridAdapter.cs");
        string metadataPath = Path.Combine(
            repositoryRoot,
            "src",
            "Libraries",
            "WpfPropertyGridBridge",
            "PropertyGridMetadataAdapters.cs");
        string adapter = File.ReadAllText(adapterPath);
        string metadata = File.ReadAllText(metadataPath);

        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        Check(
            "metadata responsibility has one concrete owner file",
            MetadataDeclarations.All(declaration => Count(metadata, declaration) == 1)
                && MetadataDeclarations.All(declaration => Count(adapter, declaration) == 0),
            passed,
            failed);
        Check(
            "extracted metadata logic keeps the pre-refactor token contract",
            ComputeTokenHash(metadata) == ExpectedMetadataTokenHash,
            passed,
            failed);
        Check(
            "PropertyGrid still composes the extracted provider and ordering adapters",
            adapter.Contains("new DynamicPropertyGridTypeDescriptionProvider(TypeDescriptor.GetProvider(type))", StringComparison.Ordinal)
                && adapter.Contains("innerPropertyGrid.PropertyComparer = new BridgePropertyComparer(selectedType)", StringComparison.Ordinal)
                && adapter.Contains("innerPropertyGrid.CategoryComparer = new BridgeCategoryComparer(selectedType)", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "provider call path still projects parent metadata through the dynamic descriptor",
            metadata.Contains("return new DynamicPropertyGridTypeDescriptor(parentProvider.GetTypeDescriptor(objectType, instance), objectType, instance);", StringComparison.Ordinal)
                && metadata.Contains("return BuildProperties(base.GetProperties());", StringComparison.Ordinal)
                && metadata.Contains("return BuildProperties(base.GetProperties(attributes));", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "descriptor keeps hidden-property, range-companion, and progressive-viewport policy delegated to PropertyGrid",
            metadata.Contains("PropertyGrid.IsPropertyHidden(objectType, name)", StringComparison.Ordinal)
                && metadata.Contains("PropertyGrid.TryGetProgressivePropertyViewport(instance, out int visiblePropertyCount)", StringComparison.Ordinal)
                && metadata.Contains("PropertyGrid.RegisterHiddenPropertiesForType(selectedType, propertyNames)", StringComparison.Ordinal)
                && metadata.Contains("rangeEditor.MaxPropertyName", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "localization and ordering behavior remain in the generic bridge boundary",
            metadata.Contains("PropertyGridLocalization.TranslateProperty", StringComparison.Ordinal)
                && metadata.Contains("PropertyGridLocalization.TranslateCategory", StringComparison.Ordinal)
                && metadata.Contains("OpenVisionLanguageService.T(key)", StringComparison.Ordinal)
                && metadata.Contains("categoryOrders[attribute.CategoryName] = attribute.Order", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "application tool policy is not reintroduced into the bridge",
            !metadata.Contains("PropertyGridToolPolicy", StringComparison.Ordinal)
                && !metadata.Contains("OpenVisionLab.Common", StringComparison.Ordinal),
            passed,
            failed);

        string outputPath = Path.Combine(evidenceDirectory, "property-grid-metadata-adapter-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-36 PropertyGrid generic metadata adapter owner",
                "RepositoryRoot: " + repositoryRoot,
                "EvidenceDirectory: " + evidenceDirectory,
                "MetadataDeclarations: " + MetadataDeclarations.Length,
                "AdapterLines: " + CountLines(adapter),
                "MetadataLines: " + CountLines(metadata)
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
            "CONTRACT|property-grid-metadata-adapter|passed="
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
                string adapterPath = Path.Combine(
                    current.FullName,
                    "src",
                    "Libraries",
                    "WpfPropertyGridBridge",
                    "WpfPropertyGridAdapter.cs");
                if (File.Exists(adapterPath))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root could not be located.");
    }

    private static int Count(string text, string value)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }

    private static int CountLines(string text)
    {
        return text.Length == 0 ? 0 : text.Count(character => character == '\n') + 1;
    }

    private static string ComputeTokenHash(string metadata)
    {
        int start = metadata.IndexOf("    internal static class BridgeCategoryOrderMap", StringComparison.Ordinal);
        int end = metadata.LastIndexOf('}');
        if (start < 0 || end <= start)
        {
            return string.Empty;
        }

        string canonical = new string(metadata[start..end].Where(character => !char.IsWhiteSpace(character)).ToArray());
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static void Check(
        string name,
        bool condition,
        List<string> passed,
        List<string> failed)
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
