using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    public sealed class VisionPipelineExecutionProvenance
    {
        public int SchemaVersion { get; set; } = 1;
        public string OriginalPipelineSnapshotFile { get; set; } = string.Empty;
        public string EffectivePipelineSnapshotFile { get; set; } = string.Empty;
        public string OriginalPipelineSha256 { get; set; } = string.Empty;
        public string EffectivePipelineSha256 { get; set; } = string.Empty;
        public string ApplicationIdentity { get; set; } = string.Empty;
        public string VisionSdkIdentity { get; set; } = string.Empty;
        public string VisionSdkManifestIdentity { get; set; } = string.Empty;
        public string VisionSdkManifestSha256 { get; set; } = string.Empty;
        public List<VisionPipelineNormalizationChangeEvidence> NormalizationChanges { get; set; } = new List<VisionPipelineNormalizationChangeEvidence>();
    }

    public sealed class VisionPipelineNormalizationChangeEvidence
    {
        public int StepIndex { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string OriginalValue { get; set; } = string.Empty;
        public string EffectiveValue { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    internal sealed class VisionPipelineExecutionPlan
    {
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private VisionPipelineExecutionPlan(
            VisionPipeline effectivePipeline,
            byte[] originalPipelineBytes,
            byte[] effectivePipelineBytes,
            VisionPipelineExecutionProvenance provenance,
            IReadOnlyList<VisionPipelineNormalizationChange> normalizationChanges)
        {
            EffectivePipeline = effectivePipeline;
            OriginalPipelineXmlBytes = originalPipelineBytes ?? Array.Empty<byte>();
            EffectivePipelineXmlBytes = effectivePipelineBytes ?? Array.Empty<byte>();
            Provenance = provenance ?? new VisionPipelineExecutionProvenance();
            NormalizationChanges = normalizationChanges ?? Array.Empty<VisionPipelineNormalizationChange>();
        }

        public VisionPipeline EffectivePipeline { get; }

        public byte[] OriginalPipelineXmlBytes { get; }

        public byte[] EffectivePipelineXmlBytes { get; }

        public VisionPipelineExecutionProvenance Provenance { get; }

        public IReadOnlyList<VisionPipelineNormalizationChange> NormalizationChanges { get; }

        public static VisionPipelineExecutionPlan Create(
            VisionPipeline originalPipeline,
            string originalXmlText = null,
            byte[] originalXmlBytes = null)
        {
            if (originalPipeline == null)
            {
                throw new ArgumentNullException(nameof(originalPipeline));
            }

            byte[] originalBytes = originalXmlBytes
                ?? (originalXmlText == null
                    ? SerializePipeline(originalPipeline)
                    : Utf8NoBom.GetBytes(originalXmlText));
            string sourceXml = originalXmlText ?? Utf8NoBom.GetString(originalBytes);
            if (!SerializeHelper.TryLoadFromXmlText(sourceXml, out VisionPipeline effectivePipeline, out string loadError)
                || effectivePipeline == null)
            {
                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(loadError)
                        ? "Pipeline execution copy could not be created."
                        : "Pipeline execution copy could not be created: " + loadError);
            }

            IReadOnlyList<VisionPipelineNormalizationChange> changes =
                VisionPipelineNormalizer.NormalizeForRun(effectivePipeline);
            byte[] effectiveBytes = SerializePipeline(effectivePipeline);
            VisionPipelineExecutionProvenance provenance = CreateProvenance(
                originalBytes,
                effectiveBytes,
                changes);
            return new VisionPipelineExecutionPlan(
                effectivePipeline,
                originalBytes,
                effectiveBytes,
                provenance,
                changes);
        }

        internal static VisionPipelineExecutionProvenance CreateIdentityOnly(VisionPipeline pipeline)
        {
            byte[] bytes = SerializePipeline(pipeline ?? new VisionPipeline());
            return CreateProvenance(bytes, bytes, Array.Empty<VisionPipelineNormalizationChange>());
        }

        internal static VisionPipelineExecutionProvenance CopyForStorage(
            VisionPipelineExecutionProvenance provenance,
            string originalSnapshotFile,
            string effectiveSnapshotFile)
        {
            VisionPipelineExecutionProvenance source = provenance ?? new VisionPipelineExecutionProvenance();
            return new VisionPipelineExecutionProvenance
            {
                SchemaVersion = source.SchemaVersion <= 0 ? 1 : source.SchemaVersion,
                OriginalPipelineSnapshotFile = originalSnapshotFile ?? string.Empty,
                EffectivePipelineSnapshotFile = effectiveSnapshotFile ?? string.Empty,
                OriginalPipelineSha256 = source.OriginalPipelineSha256 ?? string.Empty,
                EffectivePipelineSha256 = source.EffectivePipelineSha256 ?? string.Empty,
                ApplicationIdentity = source.ApplicationIdentity ?? string.Empty,
                VisionSdkIdentity = source.VisionSdkIdentity ?? string.Empty,
                VisionSdkManifestIdentity = source.VisionSdkManifestIdentity ?? string.Empty,
                VisionSdkManifestSha256 = source.VisionSdkManifestSha256 ?? string.Empty,
                NormalizationChanges = (source.NormalizationChanges ?? new List<VisionPipelineNormalizationChangeEvidence>())
                    .Where(change => change != null)
                    .Select(change => new VisionPipelineNormalizationChangeEvidence
                    {
                        StepIndex = change.StepIndex,
                        StepName = change.StepName ?? string.Empty,
                        Kind = change.Kind ?? string.Empty,
                        PropertyName = change.PropertyName ?? string.Empty,
                        OriginalValue = change.OriginalValue ?? string.Empty,
                        EffectiveValue = change.EffectiveValue ?? string.Empty,
                        Message = change.Message ?? string.Empty
                    })
                    .ToList()
            };
        }

        internal static byte[] SerializePipeline(VisionPipeline pipeline)
        {
            if (pipeline == null)
            {
                pipeline = new VisionPipeline();
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineOnAttributes = true,
                Encoding = Utf8NoBom
            };
            using MemoryStream stream = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VisionPipeline));
                serializer.Serialize(writer, pipeline);
            }

            return stream.ToArray();
        }

        internal static void SaveSnapshot(string path, byte[] bytes)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Snapshot path is required.", nameof(path));
            }

            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(path, bytes ?? Array.Empty<byte>());
        }

        private static VisionPipelineExecutionProvenance CreateProvenance(
            byte[] originalBytes,
            byte[] effectiveBytes,
            IReadOnlyList<VisionPipelineNormalizationChange> changes)
        {
            (string sdkIdentity, string manifestIdentity, string manifestSha256) = ResolveVisionSdkIdentity();
            return new VisionPipelineExecutionProvenance
            {
                OriginalPipelineSha256 = ComputeSha256(originalBytes),
                EffectivePipelineSha256 = ComputeSha256(effectiveBytes),
                ApplicationIdentity = ResolveApplicationIdentity(),
                VisionSdkIdentity = sdkIdentity,
                VisionSdkManifestIdentity = manifestIdentity,
                VisionSdkManifestSha256 = manifestSha256,
                NormalizationChanges = (changes ?? Array.Empty<VisionPipelineNormalizationChange>())
                    .Where(change => change != null)
                    .SelectMany(change => (change.Properties ?? new List<VisionPipelineNormalizationPropertyChange>())
                        .Select(property => new VisionPipelineNormalizationChangeEvidence
                        {
                            StepIndex = change.StepIndex,
                            StepName = change.Step?.Name ?? string.Empty,
                            Kind = change.Kind ?? string.Empty,
                            PropertyName = property?.PropertyName ?? string.Empty,
                            OriginalValue = property?.OriginalValue ?? string.Empty,
                            EffectiveValue = property?.EffectiveValue ?? string.Empty,
                            Message = change.Message ?? string.Empty
                        }))
                    .ToList()
            };
        }

        private static string ComputeSha256(byte[] bytes)
        {
            return Convert.ToHexString(SHA256.HashData(bytes ?? Array.Empty<byte>()));
        }

        private static string ResolveApplicationIdentity()
        {
            Assembly assembly = typeof(AppVersion).Assembly;
            AssemblyName name = assembly.GetName();
            string informationalVersion = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
            return string.Join(
                ";",
                $"{name.Name}",
                $"AppVersion={AppVersion.VERSION}",
                $"AssemblyVersion={name.Version}",
                $"InformationalVersion={informationalVersion ?? string.Empty}");
        }

        private static (string SdkIdentity, string ManifestIdentity, string ManifestSha256) ResolveVisionSdkIdentity()
        {
            Assembly sdkAssembly = typeof(VisionPipeline).Assembly;
            string sdkIdentity = $"{sdkAssembly.GetName().Name};AssemblyVersion={sdkAssembly.GetName().Version}";
            string manifestPath = FindRepositoryFile(
                Path.Combine("dll", "OpenVisionLab-Vision-SDK", "sdk-manifest.json"));
            if (string.IsNullOrWhiteSpace(manifestPath) || !File.Exists(manifestPath))
            {
                return (sdkIdentity + ";Manifest=unavailable", "unavailable", string.Empty);
            }

            byte[] manifestBytes = File.ReadAllBytes(manifestPath);
            string manifestSha256 = ComputeSha256(manifestBytes);
            string version = string.Empty;
            string commit = string.Empty;
            try
            {
                using JsonDocument document = JsonDocument.Parse(manifestBytes);
                if (document.RootElement.TryGetProperty("sdk", out JsonElement sdk)
                    && sdk.ValueKind == JsonValueKind.Object)
                {
                    version = GetJsonString(sdk, "version");
                    commit = GetJsonString(sdk, "commit");
                }
            }
            catch (JsonException)
            {
                version = string.Empty;
                commit = string.Empty;
            }

            string manifestIdentity = $"sdk-manifest.json;SHA256={manifestSha256}";
            string sdkDetails = string.IsNullOrWhiteSpace(version)
                ? string.Empty
                : $";ManifestVersion={version};ManifestCommit={commit}";
            return (sdkIdentity + sdkDetails, manifestIdentity, manifestSha256);
        }

        private static string GetJsonString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out JsonElement property)
                && property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
        }

        private static string FindRepositoryFile(string relativePath)
        {
            DirectoryInfo current = new DirectoryInfo(AppContext.BaseDirectory);
            for (int depth = 0; current != null && depth < 10; depth++, current = current.Parent)
            {
                string candidate = Path.Combine(current.FullName, relativePath);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }
    }
}
