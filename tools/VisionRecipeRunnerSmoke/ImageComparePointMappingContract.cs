using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;

internal static class ImageComparePointMappingContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "image_compare_point_mapping_contract_" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);

        List<string> results = new List<string>();
        Check(
            "Centered displayed point maps to image pixels",
            ImageCompareViewModel.TryMapDisplayedPoint(100, 50, 300, 300, 150, 150, out int centerX, out int centerY)
                && centerX == 50
                && centerY == 25,
            results);
        Check(
            "Letterbox area is rejected",
            !ImageCompareViewModel.TryMapDisplayedPoint(100, 50, 300, 300, 150, 10, out _, out _),
            results);
        Check(
            "Image edge is clamped to the last pixel",
            ImageCompareViewModel.TryMapDisplayedPoint(100, 50, 300, 300, 299, 224, out int edgeX, out int edgeY)
                && edgeX == 99
                && edgeY == 49,
            results);
        Check(
            "Invalid image or host dimensions are rejected",
            !ImageCompareViewModel.TryMapDisplayedPoint(0, 50, 300, 300, 150, 150, out _, out _)
                && !ImageCompareViewModel.TryMapDisplayedPoint(100, 50, 0, 300, 150, 150, out _, out _),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "image_compare_point_mapping_contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "IMAGE_COMPARE_POINT_MAPPING_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count);
        return passed ? 0 : 1;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }
}
