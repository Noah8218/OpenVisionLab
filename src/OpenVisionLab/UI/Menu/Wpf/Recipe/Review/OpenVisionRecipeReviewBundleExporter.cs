using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeReviewReference
    {
        public OpenVisionRecipeReviewReference(string kind, string name, string path, string sourceKind)
        {
            Kind = kind ?? string.Empty;
            Name = name ?? string.Empty;
            Path = path ?? string.Empty;
            SourceKind = sourceKind ?? string.Empty;
        }

        public string Kind { get; }

        public string Name { get; }

        public string Path { get; }

        public string SourceKind { get; }
    }

    internal static class OpenVisionRecipeReviewBundleExporter
    {
        internal const string ManifestEntryName = "review-manifest.json";
        internal const string PipelineEntryName = "pipeline.xml";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        internal static bool TryExport(
            string packagePath,
            string recipeName,
            string pipelineName,
            VisionPipeline pipeline,
            string pipelineXml,
            IEnumerable<OpenVisionRecipeReviewReference> references,
            out string message)
        {
            if (string.IsNullOrWhiteSpace(packagePath))
            {
                message = "Review bundle path is empty.";
                return false;
            }

            if (pipeline == null || string.IsNullOrWhiteSpace(pipelineXml))
            {
                message = "Pipeline XML is empty.";
                return false;
            }

            string fullPath;
            string temporaryPath = string.Empty;
            try
            {
                fullPath = Path.GetFullPath(packagePath);
                string directory = Path.GetDirectoryName(fullPath);
                if (string.IsNullOrWhiteSpace(directory))
                {
                    message = "Review bundle directory is empty.";
                    return false;
                }

                Directory.CreateDirectory(directory);
                temporaryPath = Path.Combine(
                    directory,
                    "." + Path.GetFileName(fullPath) + "." + Guid.NewGuid().ToString("N") + ".tmp");

                byte[] pipelineBytes = new UTF8Encoding(false).GetBytes(pipelineXml);
                VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
                List<VisionPipelineStep> steps = (pipeline.Steps ?? new List<VisionPipelineStep>())
                    .Where(step => step != null)
                    .ToList();
                List<ReviewFileRecord> dependencies = BuildDependencyRecords(steps);
                List<ReviewFileRecord> referenceFiles = (references ?? Array.Empty<OpenVisionRecipeReviewReference>())
                    .Where(reference => reference != null && !string.IsNullOrWhiteSpace(reference.Path))
                    .GroupBy(reference => reference.Kind + "\n" + reference.Path, StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.First())
                    .Select(reference => BuildFileRecord(
                        reference.Kind,
                        reference.Name,
                        null,
                        null,
                        reference.Path,
                        reference.SourceKind))
                    .ToList();

                var stepSummaries = steps.Select((step, index) => new
                {
                    index = index + 1,
                    step.Name,
                    step.ToolType,
                    step.Enabled,
                    step.InputLayer,
                    step.OutputLayer,
                    acceptance = new
                    {
                        enabled = step.UseAcceptance,
                        step.ExpectedSuccess,
                        step.MaxElapsedMilliseconds,
                        metricName = step.AcceptanceMetricName,
                        minimum = step.UseAcceptanceMetricMinimum ? step.AcceptanceMetricMinimum : (double?)null,
                        maximum = step.UseAcceptanceMetricMaximum ? step.AcceptanceMetricMaximum : (double?)null
                    }
                }).ToList();
                var acceptanceMetrics = steps
                    .Select((step, index) => new { step, index })
                    .Where(item => item.step.UseAcceptance)
                    .Select(item => new
                    {
                        stepIndex = item.index + 1,
                        stepName = item.step.Name,
                        metricName = item.step.AcceptanceMetricName,
                        expectedSuccess = item.step.ExpectedSuccess,
                        minimum = item.step.UseAcceptanceMetricMinimum ? item.step.AcceptanceMetricMinimum : (double?)null,
                        maximum = item.step.UseAcceptanceMetricMaximum ? item.step.AcceptanceMetricMaximum : (double?)null,
                        item.step.MaxElapsedMilliseconds
                    })
                    .ToList();
                var toolTypes = steps
                    .GroupBy(step => string.IsNullOrWhiteSpace(step.ToolType) ? "-" : step.ToolType.Trim(), StringComparer.OrdinalIgnoreCase)
                    .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(group => new
                    {
                        toolType = group.Key,
                        count = group.Count(),
                        enabledCount = group.Count(step => step.Enabled)
                    })
                    .ToList();

                var manifest = new
                {
                    format = "OpenVisionLab.RecipeReviewBundle",
                    schemaVersion = 1,
                    generatedUtc = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                    applicationVersion = AppVersion.VERSION,
                    pipelineXmlSchema = "VisionPipeline",
                    recipeName = recipeName ?? string.Empty,
                    pipelineName = pipelineName ?? pipeline.Name ?? string.Empty,
                    packagePolicy = new
                    {
                        referencedFilesCopied = false,
                        privateLocalAssetsCopied = false,
                        importExecuted = false,
                        previewOrRunExecuted = false
                    },
                    pipelineXml = new
                    {
                        entry = PipelineEntryName,
                        sizeBytes = pipelineBytes.LongLength,
                        sha256 = ComputeSha256(pipelineBytes)
                    },
                    validation = new
                    {
                        status = validation.Success ? "OK" : "NG",
                        errors = validation.Errors,
                        warnings = validation.Warnings
                    },
                    summary = new
                    {
                        stepCount = steps.Count,
                        enabledStepCount = steps.Count(step => step.Enabled),
                        dependencyCount = dependencies.Count,
                        missingDependencyCount = dependencies.Count(item => !item.Exists),
                        referenceCount = referenceFiles.Count
                    },
                    toolTypes,
                    steps = stepSummaries,
                    acceptanceMetrics,
                    dependencies,
                    references = referenceFiles
                };

                byte[] manifestBytes = new UTF8Encoding(false).GetBytes(JsonSerializer.Serialize(manifest, JsonOptions));
                using (FileStream stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: false))
                {
                    WriteEntry(archive, PipelineEntryName, pipelineBytes);
                    WriteEntry(archive, ManifestEntryName, manifestBytes);
                }

                File.Move(temporaryPath, fullPath, overwrite: true);
                temporaryPath = string.Empty;
                message = fullPath;
                return true;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().Message;
                return false;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(temporaryPath) && File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static List<ReviewFileRecord> BuildDependencyRecords(IEnumerable<VisionPipelineStep> steps)
        {
            List<ReviewFileRecord> records = new List<ReviewFileRecord>();
            foreach (VisionPipelineStep step in steps)
            {
                foreach (KeyValuePair<string, string> parameter in step.Parameters ?? new Dictionary<string, string>())
                {
                    if (!OpenVisionRecipeDependencyReviewService.LooksLikeDependencyPath(parameter.Key, parameter.Value))
                    {
                        continue;
                    }

                    records.Add(BuildFileRecord(
                        "PipelineDependency",
                        step.Name + "." + parameter.Key,
                        step.Name,
                        parameter.Key,
                        parameter.Value,
                        "PipelineParameter"));
                }
            }

            return records;
        }

        private static ReviewFileRecord BuildFileRecord(
            string kind,
            string name,
            string stepName,
            string parameterName,
            string sourcePath,
            string sourceKind)
        {
            string candidate = (sourcePath ?? string.Empty).Trim().Trim('"');
            ReviewFileRecord record = new ReviewFileRecord
            {
                Kind = kind,
                Name = name,
                StepName = stepName,
                ParameterName = parameterName,
                SourceKind = sourceKind,
                SourcePath = candidate,
                PathKind = Path.IsPathRooted(candidate) ? "Absolute" : "Relative",
                PackageState = "ReferencedOnly"
            };

            try
            {
                record.ResolvedPath = OpenVisionRecipeDependencyReviewService.ResolveDependencySourcePath(candidate);
                record.Exists = !string.IsNullOrWhiteSpace(record.ResolvedPath) && File.Exists(record.ResolvedPath);
                if (record.Exists)
                {
                    FileInfo file = new FileInfo(record.ResolvedPath);
                    record.SizeBytes = file.Length;
                    record.Sha256 = ComputeFileSha256(record.ResolvedPath);
                }
            }
            catch (Exception ex)
            {
                record.ReadError = ex.GetBaseException().Message;
            }

            return record;
        }

        private static void WriteEntry(ZipArchive archive, string name, byte[] content)
        {
            ZipArchiveEntry entry = archive.CreateEntry(name, CompressionLevel.Optimal);
            using Stream stream = entry.Open();
            stream.Write(content, 0, content.Length);
        }

        private static string ComputeSha256(byte[] content)
        {
            return Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
        }

        private static string ComputeFileSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using SHA256 sha256 = SHA256.Create();
            return Convert.ToHexString(sha256.ComputeHash(stream)).ToLowerInvariant();
        }

        private sealed class ReviewFileRecord
        {
            public string Kind { get; set; }
            public string Name { get; set; }
            public string StepName { get; set; }
            public string ParameterName { get; set; }
            public string SourceKind { get; set; }
            public string SourcePath { get; set; }
            public string PathKind { get; set; }
            public string ResolvedPath { get; set; }
            public bool Exists { get; set; }
            public long? SizeBytes { get; set; }
            public string Sha256 { get; set; }
            public string ReadError { get; set; }
            public string PackageState { get; set; }
        }
    }
}
