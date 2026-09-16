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
        private const string VisionSdkManifestFileName = "sdk-manifest.json";
        private const string CleanRuntimeManifestFileName = "clean_runtime_manifest.json";
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
            if (originalXmlBytes != null && originalXmlText == null)
            {
                VisionPipelineXmlSchemaPolicy.ThrowIfExecutionBlocked(originalBytes);
            }
            else if (originalXmlText != null)
            {
                VisionPipelineXmlSchemaPolicy.ThrowIfExecutionBlocked(originalXmlText);
            }

            VisionPipeline effectivePipeline;
            string loadError = string.Empty;
            bool loaded;
            if (originalXmlBytes != null && originalXmlText == null)
            {
                loaded = SerializeHelper.TryLoadFromXmlBytes(
                    originalBytes,
                    out effectivePipeline,
                    out Exception byteLoadException);
                loadError = byteLoadException?.Message;
            }
            else
            {
                loaded = SerializeHelper.TryLoadFromXmlText(
                    originalXmlText ?? Utf8NoBom.GetString(originalBytes),
                    out effectivePipeline,
                    out loadError);
            }
            if (!loaded || effectivePipeline == null)
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
            string manifestPath = FindDeploymentFile(VisionSdkManifestFileName);
            bool deploymentManifest = !string.IsNullOrWhiteSpace(manifestPath);
            if (!deploymentManifest)
            {
                manifestPath = FindRepositoryFile(
                    Path.Combine("dll", "OpenVisionLab-Vision-SDK", VisionSdkManifestFileName));
            }
            if (string.IsNullOrWhiteSpace(manifestPath) || !File.Exists(manifestPath))
            {
                return (sdkIdentity + ";Manifest=unavailable", "unavailable", string.Empty);
            }

            try
            {
                byte[] manifestBytes = File.ReadAllBytes(manifestPath);
                string manifestSha256 = ComputeSha256(manifestBytes);
                bool manifestMatches = TryValidateVisionSdkManifest(
                    manifestPath,
                    manifestBytes,
                    sdkAssembly,
                    deploymentManifest,
                    manifestSha256,
                    ResolveEmbeddedVisionSdkManifestSha256(),
                    out string version,
                    out string commit);
                string status = manifestMatches ? "match" : "mismatch";
                string manifestIdentity = $"{VisionSdkManifestFileName};SHA256={manifestSha256};Status={status}";
                string sdkDetails = string.IsNullOrWhiteSpace(version)
                    ? string.Empty
                    : $";ManifestVersion={version};ManifestCommit={commit}";
                return (
                    sdkIdentity + sdkDetails + $";ManifestStatus={status}",
                    manifestIdentity,
                    manifestSha256);
            }
            catch (IOException)
            {
                return (
                    sdkIdentity + ";ManifestStatus=mismatch",
                    $"{VisionSdkManifestFileName};Status=mismatch",
                    string.Empty);
            }
            catch (UnauthorizedAccessException)
            {
                return (
                    sdkIdentity + ";ManifestStatus=mismatch",
                    $"{VisionSdkManifestFileName};Status=mismatch",
                    string.Empty);
            }
        }

        private static string GetJsonString(JsonElement element, string propertyName)
        {
            return TryGetJsonProperty(element, propertyName, out JsonElement property)
                && property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
        }

        private static bool TryValidateVisionSdkManifest(
            string manifestPath,
            byte[] manifestBytes,
            Assembly sdkAssembly,
            bool deploymentManifest,
            string manifestSha256,
            string embeddedManifestSha256,
            out string version,
            out string commit)
        {
            version = string.Empty;
            commit = string.Empty;
            try
            {
                using JsonDocument document = ParseJsonDocument(manifestBytes);
                if (!TryGetJsonProperty(document.RootElement, "sdk", out JsonElement sdk)
                    || sdk.ValueKind != JsonValueKind.Object)
                {
                    return false;
                }

                version = GetJsonString(sdk, "version");
                commit = GetJsonString(sdk, "commit");
                if (!string.IsNullOrWhiteSpace(embeddedManifestSha256)
                    && !string.Equals(
                        embeddedManifestSha256,
                        manifestSha256,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(commit)
                    || !TryGetJsonProperty(document.RootElement, "files", out JsonElement files)
                    || files.ValueKind != JsonValueKind.Array)
                {
                    return false;
                }

                string manifestDirectory = Path.GetDirectoryName(manifestPath) ?? string.Empty;
                string loadedAssemblyPath = sdkAssembly.Location;
                if (string.IsNullOrWhiteSpace(manifestDirectory)
                    || string.IsNullOrWhiteSpace(loadedAssemblyPath)
                    || !File.Exists(loadedAssemblyPath))
                {
                    return false;
                }

                FileInfo loadedAssembly = new FileInfo(loadedAssemblyPath);
                string loadedAssemblyHash = ComputeFileSha256(loadedAssemblyPath);
                bool loadedAssemblyMatched = false;
                foreach (JsonElement file in files.EnumerateArray())
                {
                    if (file.ValueKind != JsonValueKind.Object
                        || !TryGetJsonProperty(file, "path", out JsonElement pathProperty)
                        || pathProperty.ValueKind != JsonValueKind.String
                        || !TryGetJsonProperty(file, "length", out JsonElement lengthProperty)
                        || !lengthProperty.TryGetInt64(out long expectedLength)
                        || !TryGetJsonProperty(file, "sha256", out JsonElement hashProperty)
                        || hashProperty.ValueKind != JsonValueKind.String)
                    {
                        return false;
                    }

                    string relativePath = pathProperty.GetString() ?? string.Empty;
                    string expectedHash = hashProperty.GetString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(relativePath)
                        || Path.IsPathRooted(relativePath)
                        || expectedLength < 0
                        || expectedHash.Length != 64)
                    {
                        return false;
                    }

                    string filePath = Path.GetFullPath(Path.Combine(manifestDirectory, relativePath));
                    if (!IsContainedPath(manifestDirectory, filePath) || !File.Exists(filePath))
                    {
                        return false;
                    }

                    FileInfo actualFile = new FileInfo(filePath);
                    string actualHash = ComputeFileSha256(filePath);
                    if (actualFile.Length != expectedLength
                        || !string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    if (string.Equals(
                            Path.GetFileName(filePath),
                            Path.GetFileName(loadedAssemblyPath),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        loadedAssemblyMatched = loadedAssembly.Length == expectedLength
                            && string.Equals(
                                loadedAssemblyHash,
                                expectedHash,
                                StringComparison.OrdinalIgnoreCase);
                    }
                }

                if (!loadedAssemblyMatched)
                {
                    return false;
                }

                if (deploymentManifest)
                {
                    string runtimeManifestPath = Path.Combine(
                        AppContext.BaseDirectory,
                        CleanRuntimeManifestFileName);
                    if (File.Exists(runtimeManifestPath)
                        && !TryValidateCleanRuntimeManifest(
                            runtimeManifestPath,
                            manifestSha256,
                            version,
                            commit))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static bool TryValidateCleanRuntimeManifest(
            string runtimeManifestPath,
            string sdkManifestSha256,
            string version,
            string commit)
        {
            try
            {
                using JsonDocument document = ParseJsonDocument(File.ReadAllBytes(runtimeManifestPath));
                bool hasManifestAnchor = false;
                if (TryGetJsonProperty(
                        document.RootElement,
                        "VisionSdkManifestSha256",
                        out JsonElement manifestHashProperty)
                    && manifestHashProperty.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(manifestHashProperty.GetString()))
                {
                    hasManifestAnchor = true;
                    if (!string.Equals(
                            manifestHashProperty.GetString(),
                            sdkManifestSha256,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                if (TryGetJsonProperty(document.RootElement, "VisionSdk", out JsonElement runtimeSdk)
                    && runtimeSdk.ValueKind == JsonValueKind.Object)
                {
                    string runtimeVersion = GetJsonString(runtimeSdk, "version");
                    string runtimeCommit = GetJsonString(runtimeSdk, "commit");
                    if ((!string.IsNullOrWhiteSpace(runtimeVersion)
                            && !string.Equals(runtimeVersion, version, StringComparison.Ordinal))
                        || (!string.IsNullOrWhiteSpace(runtimeCommit)
                            && !string.Equals(runtimeCommit, commit, StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                }

                if (TryGetJsonProperty(document.RootElement, "Files", out JsonElement files)
                    && files.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement file in files.EnumerateArray())
                    {
                        string path = GetJsonString(file, "Path");
                        if (!string.Equals(
                                Path.GetFileName(path),
                                VisionSdkManifestFileName,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        hasManifestAnchor = true;
                        if (!TryGetJsonProperty(file, "Length", out JsonElement lengthProperty)
                            || !lengthProperty.TryGetInt64(out long length)
                            || !TryGetJsonProperty(file, "SHA256", out JsonElement hashProperty)
                            || hashProperty.ValueKind != JsonValueKind.String
                            || length != new FileInfo(Path.Combine(AppContext.BaseDirectory, VisionSdkManifestFileName)).Length
                            || !string.Equals(
                                hashProperty.GetString(),
                                sdkManifestSha256,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }

                return !hasManifestAnchor || !string.IsNullOrWhiteSpace(sdkManifestSha256);
            }
            catch (JsonException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static bool TryGetJsonProperty(
            JsonElement element,
            string propertyName,
            out JsonElement property)
        {
            if (element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty(propertyName, out property))
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty candidate in element.EnumerateObject())
                {
                    if (string.Equals(candidate.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        property = candidate.Value;
                        return true;
                    }
                }
            }

            property = default;
            return false;
        }

        private static JsonDocument ParseJsonDocument(byte[] bytes)
        {
            return JsonDocument.Parse(Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF'));
        }

        private static string ComputeFileSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            return Convert.ToHexString(SHA256.HashData(stream));
        }

        private static string ResolveEmbeddedVisionSdkManifestSha256()
        {
            return typeof(AppVersion).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(attribute => string.Equals(
                    attribute.Key,
                    "OpenVisionVisionSdkManifestSha256",
                    StringComparison.Ordinal))?
                .Value ?? string.Empty;
        }

        private static bool IsContainedPath(string root, string path)
        {
            string fullRoot = Path.GetFullPath(root).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
            string fullPath = Path.GetFullPath(path).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
            return string.Equals(fullRoot, fullPath, StringComparison.OrdinalIgnoreCase)
                || fullPath.StartsWith(
                    fullRoot + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string FindDeploymentFile(string fileName)
        {
            string candidate = Path.Combine(AppContext.BaseDirectory, fileName);
            return File.Exists(candidate) ? candidate : string.Empty;
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
