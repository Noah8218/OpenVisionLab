#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;

internal static class SdkDeploymentProvenanceContract
{
    private const string ExpectedSdkVersion = "3.0.0";
    private const string ExpectedSdkCommit = "f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b";

    public static int Run(string evidenceDirectory, string expectedCase)
    {
        string evidenceRoot = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(evidenceRoot);
        VisionPipelineExecutionProvenance provenance = VisionPipelineExecutionPlan.CreateIdentityOnly(
            new VisionPipeline { Name = "2D-046 SDK deployment provenance" });
        Assembly loadedAssembly = typeof(VisionPipeline).Assembly;
        string loadedAssemblyPath = loadedAssembly.Location;
        string sidecarPath = Path.Combine(AppContext.BaseDirectory, "sdk-manifest.json");
        string packageManifestPath = Path.Combine(AppContext.BaseDirectory, "clean_runtime_manifest.json");
        string loadedAssemblyHash = File.Exists(loadedAssemblyPath)
            ? ComputeFileSha256(loadedAssemblyPath)
            : string.Empty;
        string sidecarHash = File.Exists(sidecarPath)
            ? ComputeFileSha256(sidecarPath)
            : string.Empty;
        string? observedVersion = ReadSdkField(sidecarPath, "version");
        string? observedCommit = ReadSdkField(sidecarPath, "commit");
        bool passed = expectedCase switch
        {
            "match" => provenance.VisionSdkIdentity.Contains("ManifestStatus=match", StringComparison.Ordinal)
                && provenance.VisionSdkIdentity.Contains("ManifestVersion=" + ExpectedSdkVersion, StringComparison.Ordinal)
                && provenance.VisionSdkIdentity.Contains("ManifestCommit=" + ExpectedSdkCommit, StringComparison.Ordinal)
                && provenance.VisionSdkManifestIdentity.Contains("Status=match", StringComparison.Ordinal),
            "missing" => provenance.VisionSdkIdentity.Contains("Manifest=unavailable", StringComparison.Ordinal)
                && provenance.VisionSdkManifestIdentity == "unavailable",
            "mismatch" => provenance.VisionSdkIdentity.Contains("ManifestStatus=mismatch", StringComparison.Ordinal)
                && !provenance.VisionSdkIdentity.Contains("ManifestStatus=match", StringComparison.Ordinal),
            _ => false
        };

        string reportPath = Path.Combine(evidenceRoot, "sdk-deployment-provenance-contract.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    expectedCase,
                    baseDirectory = AppContext.BaseDirectory,
                    loadedAssembly = new
                    {
                        name = loadedAssembly.GetName().Name,
                        version = loadedAssembly.GetName().Version?.ToString(),
                        path = loadedAssemblyPath,
                        length = File.Exists(loadedAssemblyPath) ? new FileInfo(loadedAssemblyPath).Length : 0,
                        sha256 = loadedAssemblyHash
                    },
                    sdkManifest = new
                    {
                        path = File.Exists(sidecarPath) ? sidecarPath : string.Empty,
                        length = File.Exists(sidecarPath) ? new FileInfo(sidecarPath).Length : 0,
                        sha256 = sidecarHash,
                        version = observedVersion,
                        commit = observedCommit
                    },
                    cleanRuntimeManifest = new
                    {
                        path = File.Exists(packageManifestPath) ? packageManifestPath : string.Empty,
                        present = File.Exists(packageManifestPath)
                    },
                    provenance = new
                    {
                        provenance.VisionSdkIdentity,
                        provenance.VisionSdkManifestIdentity,
                        provenance.VisionSdkManifestSha256
                    },
                    passed,
                    noAssemblyInternalsInferred = true
                },
                new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine(
            $"SDK deployment provenance {expectedCase}: {(passed ? "PASS" : "FAIL")}. Evidence={reportPath}");
        return passed ? 0 : 1;
    }

    private static string? ReadSdkField(string path, string field)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(path));
            if (!document.RootElement.TryGetProperty("sdk", out JsonElement sdk)
                || !sdk.TryGetProperty(field, out JsonElement value)
                || value.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return value.GetString();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string ComputeFileSha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
}
