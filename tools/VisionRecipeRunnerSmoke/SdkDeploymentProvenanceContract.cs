#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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
        bool applicationVersionMatches = false;
        object? applicationVersionEvidence = null;
        if (expectedCase == "application-version")
        {
            Assembly application = typeof(AppVersion).Assembly;
            string metadataVersion = application.GetCustomAttributes<AssemblyMetadataAttribute>().Single(attribute => attribute.Key == "OpenVisionApplicationVersion").Value
                ?? throw new InvalidDataException("Application version build metadata is missing.");
            string informationalVersion = application.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0]
                ?? throw new InvalidDataException("Application informational version is missing.");
            string defaultVersion = AppVersion.VERSION;
            using JsonDocument runtimeManifest = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(evidenceRoot, "openvisionlab.runtime.json")));
            string? manifestVersion = runtimeManifest.RootElement.GetProperty("identity").GetProperty("applicationVersion").GetString();
            string? manifestHash = runtimeManifest.RootElement.GetProperty("entryAssembly").GetProperty("sha256").GetString();
            applicationVersionMatches = defaultVersion == metadataVersion && metadataVersion == informationalVersion && metadataVersion == manifestVersion
                && string.Equals(manifestHash, ComputeFileSha256(application.Location), StringComparison.OrdinalIgnoreCase);
            List<object> versionCases = new();
            try
            {
                foreach (string version in new[] { metadataVersion, "contract-runtime-override" })
                {
                    AppVersion.VERSION = version;
                    VisionPipeline pipeline = new() { Name = "Version identity contract" };
                    byte[] originalXml = VisionPipelineExecutionPlan.SerializePipeline(pipeline);
                    VisionPipelineExecutionProvenance identity = VisionPipelineExecutionPlan.CreateIdentityOnly(pipeline);
                    string bundlePath = Path.Combine(evidenceRoot, version == metadataVersion ? "default-version.review.zip" : "setter-override.review.zip");
                    bool exported = OpenVisionRecipeReviewBundleExporter.TryExport(bundlePath, "VersionContract", pipeline.Name, pipeline,
                        Encoding.UTF8.GetString(originalXml), Array.Empty<OpenVisionRecipeReviewReference>(), out string exportMessage);
                    if (!exported) { throw new InvalidDataException(exportMessage); }
                    using ZipArchive archive = ZipFile.OpenRead(bundlePath);
                    using Stream manifestStream = archive.GetEntry(OpenVisionRecipeReviewBundleExporter.ManifestEntryName)!.Open();
                    using JsonDocument bundleManifest = JsonDocument.Parse(manifestStream);
                    string? bundleVersion = bundleManifest.RootElement.GetProperty("applicationVersion").GetString();
                    using Stream xmlStream = archive.GetEntry(OpenVisionRecipeReviewBundleExporter.PipelineEntryName)!.Open();
                    using MemoryStream exportedXml = new(); xmlStream.CopyTo(exportedXml);
                    bool casePassed = AppVersion.VERSION == version && identity.ApplicationIdentity.Split(';').Contains("AppVersion=" + version)
                        && bundleVersion == version && originalXml.SequenceEqual(exportedXml.ToArray())
                        && originalXml.SequenceEqual(VisionPipelineExecutionPlan.SerializePipeline(pipeline));
                    applicationVersionMatches &= casePassed;
                    versionCases.Add(new { Version = version, identity.ApplicationIdentity, BundleVersion = bundleVersion, XmlUnchanged = originalXml.SequenceEqual(exportedXml.ToArray()), Passed = casePassed });
                }
            }
            finally { AppVersion.VERSION = defaultVersion; }
            applicationVersionMatches &= AppVersion.VERSION == defaultVersion;
            applicationVersionEvidence = new { MetadataVersion = metadataVersion, InformationalVersion = informationalVersion, ManifestVersion = manifestVersion, DefaultVersion = defaultVersion, SetterRestored = AppVersion.VERSION == defaultVersion, ProductSHA256 = ComputeFileSha256(application.Location), Cases = versionCases, Scope = "Non-UI build identity, execution-plan provenance and isolated review-export fixtures; actual startup log remains separate" };
        }
        bool passed = expectedCase switch
        {
            "application-version" => applicationVersionMatches,
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
                    applicationVersionEvidence,
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
