using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    internal sealed class VisionToolNImageVerificationProgress
    {
        public bool IsStarting { get; init; }
        public string ImagePath { get; init; } = string.Empty;
        public int CompletedCount { get; init; }
        public int TotalCount { get; init; }
        public VisionToolNImageVerificationRow Row { get; init; }
    }

    internal sealed class VisionToolNImageVerificationSession
    {
        public string InputIdentity { get; init; } = string.Empty;
        public IReadOnlyDictionary<string, string> AdditionalInputPaths { get; init; } = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> AdditionalInputSha256 { get; init; } = new Dictionary<string, string>();
        public int PrefixCount { get; init; }
        public int PlannedCount { get; init; }
        public string EvidenceDirectory { get; init; } = string.Empty;
        public string ToolName { get; init; } = string.Empty;
        public string PipelineName { get; init; } = string.Empty;
        public string PipelineXml { get; init; } = string.Empty;
        public string StepDefinitionSha256 { get; init; } = string.Empty;
        public string BatchSummaryPath { get; init; } = string.Empty;
        public bool HasAcceptance { get; init; }
        public bool WasCancelled { get; init; }
        public DateTime StartedAt { get; init; }
        public DateTime FinishedAt { get; init; }
        public IReadOnlyList<VisionToolNImageVerificationRow> Rows { get; init; } =
            Array.Empty<VisionToolNImageVerificationRow>();
    }

    internal sealed class VisionToolNImageVerificationRow
    {
        public int Index { get; init; }
        public string ImagePath { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string FileName => string.IsNullOrEmpty(DisplayName) ? Path.GetFileName(ImagePath) : DisplayName;
        public string Status { get; init; } = string.Empty;
        public bool Success { get; init; }
        public double TotalMilliseconds { get; init; }
        public string Message { get; init; } = string.Empty;
        public VisionPipelineStepResultSummary TargetSummary { get; init; }
        public string MetricText { get; init; } = string.Empty;
        public string RunReportPath { get; init; } = string.Empty;
        public string SourceSnapshotPath { get; init; } = string.Empty;
        public string DrawingPath { get; init; } = string.Empty;
        public string SourceSha256 { get; init; } = string.Empty;
        public string FailedStep { get; init; } = string.Empty;
        public string ReviewReasonText { get; set; } = string.Empty;
        public bool IsCompleted { get; init; }
        public bool IsNg => IsCompleted && string.Equals(Status, "NG", StringComparison.OrdinalIgnoreCase);
        public bool IsError => IsCompleted && string.Equals(Status, "ERROR", StringComparison.OrdinalIgnoreCase);
        public bool IsUngated => IsCompleted && string.Equals(Status, "RUN OK", StringComparison.OrdinalIgnoreCase);
        public string ReviewDetailText => string.Join(" · ", new[] { FailedStep, ReviewReasonText, Message }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
        public bool HasDrawing => !string.IsNullOrWhiteSpace(DrawingPath) && File.Exists(DrawingPath);
    }

    internal static class VisionToolNImageVerificationService
    {
        public const int MaximumImageCount = 5000;

        public static async Task<VisionToolNImageVerificationSession> RunAsync(
            string toolName,
            string recipeName,
            Func<VisionPipelineStep> createStep,
            bool normalizeInputToGray,
            IReadOnlyList<string> imagePaths,
            IProgress<VisionToolNImageVerificationProgress> progress,
            CancellationToken cancellationToken,
            VisionPipeline executionPipeline = null, string evidenceDirectory = null,
            IReadOnlyDictionary<string, (string Path, string Name)> imageIdentities = null,
            IReadOnlyDictionary<string, string> inputSnapshots = null, string inputIdentity = null)
        {
            if (createStep == null)
            {
                throw new ArgumentNullException(nameof(createStep));
            }

            List<string> paths = NormalizeImagePaths(imagePaths, includeMissing: executionPipeline != null);
            if (paths.Count == 0)
            {
                throw new InvalidOperationException("검증할 이미지가 없습니다.");
            }

            bool mainTrial = executionPipeline != null;
            VisionPipeline pipeline = mainTrial ? VisionPipelineStepEditService.Copy(executionPipeline) : new VisionPipeline();
            VisionPipelineStep step = mainTrial ? pipeline.Steps.LastOrDefault() : createStep();
            if (step == null) { throw new InvalidOperationException("Current test step is unavailable."); }
            string resolvedToolName = NormalizeName(toolName, step.ToolType, "Tool");
            string pipelineName = "NImage_" + SanitizeName(resolvedToolName);
            pipeline.Name = pipelineName;
            if (!mainTrial)
            {
                step.Name = NormalizeName(step.Name, resolvedToolName, "Tool");
                step.InputLayer = "Main";
                step.OutputLayer = "NImageResult";
                pipeline.Steps.Add(step);
            }
            int prefixCount = pipeline.Steps.Count - 1;
            string pipelineXml = SerializePipeline(pipeline);
            string definitionSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(pipelineXml)));
            string resolvedRecipeName = string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName.Trim();
            var inputHashes = (inputSnapshots ?? new Dictionary<string, string>()).ToDictionary(input => input.Key, input => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(input.Value))), StringComparer.OrdinalIgnoreCase);
            DateTime startedAt = DateTime.Now;
            List<VisionToolNImageVerificationRow> rows = new List<VisionToolNImageVerificationRow>(paths.Count);
            bool wasCancelled = false;

            for (int index = 0; index < paths.Count; index++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    wasCancelled = true;
                    break;
                }

                string path = paths[index];
                if (mainTrial)
                {
                    progress?.Report(new VisionToolNImageVerificationProgress { IsStarting = true, ImagePath = path, CompletedCount = index, TotalCount = paths.Count });
                }
                VisionPipelineSampleCatalogItem sample = new VisionPipelineSampleCatalogItem
                {
                    SampleName = $"{index + 1:0000} {Path.GetFileName(path)}",
                    ImageFullPath = path,
                    PairGroup = "ToolView N-image verification",
                    PairRole = "UNLABELED",
                    Notes = "Execution-only result. Operator drawing review required."
                };
                VisionPipelineSampleCheckResult check =
                    await VisionPipelineSampleCheckService.RunSampleCheckWithReportSafeAsync(
                        sample,
                        pipelineXml,
                        resolvedRecipeName,
                        mainTrial && prefixCount > 0 ? false : normalizeInputToGray,
                        mainTrial ? CancellationToken.None : cancellationToken,
                        evidenceDirectory == null ? null : Path.Combine(evidenceDirectory, "reports"), inputSnapshots);
                VisionToolNImageVerificationRow row = CreateRow(index + 1, path, check, step, prefixCount,
                    imageIdentities?.TryGetValue(path, out var identity) == true ? identity : (path, string.Empty));
                rows.Add(row);
                progress?.Report(new VisionToolNImageVerificationProgress
                {
                    CompletedCount = rows.Count,
                    TotalCount = paths.Count,
                    Row = row
                });
            }

            DateTime finishedAt = DateTime.Now;
            string summaryPath = rows.Count == 0
                ? string.Empty
                : SaveBatchSummary(
                    resolvedRecipeName,
                    pipeline,
                    resolvedToolName,
                    definitionSha256,
                    rows,
                    startedAt,
                    finishedAt,
                    wasCancelled,
                    mainTrial && prefixCount > 0 ? false : normalizeInputToGray,
                    step.UseAcceptance, evidenceDirectory);
            ApplyReviewReasons(summaryPath, rows);
            return new VisionToolNImageVerificationSession
            {
                InputIdentity = inputIdentity ?? string.Empty,
                AdditionalInputPaths = new Dictionary<string, string>(inputSnapshots ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase),
                AdditionalInputSha256 = inputHashes,
                PrefixCount = prefixCount,
                PlannedCount = paths.Count,
                EvidenceDirectory = evidenceDirectory ?? string.Empty,
                ToolName = resolvedToolName,
                PipelineName = pipelineName,
                PipelineXml = pipelineXml,
                StepDefinitionSha256 = definitionSha256,
                BatchSummaryPath = summaryPath,
                HasAcceptance = step.UseAcceptance,
                WasCancelled = wasCancelled,
                StartedAt = startedAt,
                FinishedAt = finishedAt,
                Rows = rows
            };
        }

        internal static List<string> NormalizeImagePaths(IEnumerable<string> paths, bool includeMissing = false)
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<string> normalized = new List<string>();
            foreach (string candidate in paths ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                string path;
                try
                {
                    path = Path.GetFullPath(candidate.Trim());
                }
                catch (Exception ex) when (
                    ex is ArgumentException
                    || ex is NotSupportedException
                    || ex is PathTooLongException)
                {
                    continue;
                }

                if ((!includeMissing && !File.Exists(path))
                    || !IsSupportedImage(path)
                    || !seen.Add(path))
                {
                    continue;
                }

                normalized.Add(path);
                if (normalized.Count > MaximumImageCount)
                {
                    throw new InvalidOperationException(
                        "검증 이미지 제한을 초과했습니다: " + MaximumImageCount.ToString(CultureInfo.InvariantCulture));
                }
            }

            return normalized;
        }

        private static VisionToolNImageVerificationRow CreateRow(
            int index,
            string imagePath,
            VisionPipelineSampleCheckResult check,
            VisionPipelineStep target, int prefixCount, (string Path, string Name) identity)
        {
            bool hasAcceptance = target.UseAcceptance;
            VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(check?.RunReportPath);
            VisionPipelineStepRunReport targetReport = report?.Steps?.LastOrDefault(item => item.Name == target.Name);
            VisionPipelineStepResultSummary targetSummary = targetReport == null ? null : new VisionPipelineStepResultSummary
            {
                ToolType = target.ToolType,
                Executed = targetReport.Executed,
                Skipped = targetReport.Skipped,
                Success = targetReport.ToolSuccess && (!targetReport.AcceptanceEvaluated || targetReport.AcceptancePassed),
                IsAcceptanceNg = targetReport.ToolSuccess && targetReport.AcceptanceEvaluated && !targetReport.AcceptancePassed
            };
            if (targetSummary != null)
            {
                foreach (var metric in targetReport.Metrics ?? new List<VisionPipelineMetricRunReport>()) { targetSummary.Metrics[metric.Name] = metric.Value; }
                targetSummary.Parameters = new Dictionary<string, string>(target.Parameters, StringComparer.OrdinalIgnoreCase);
            }
            string reportDirectory = string.IsNullOrWhiteSpace(check?.RunReportPath)
                ? string.Empty
                : Path.GetDirectoryName(check.RunReportPath) ?? string.Empty;
            string sourcePath = ResolveExistingPath(reportDirectory, report?.SourceImageFile);
            VisionPipelineStepRunReport evidenceStep = report?.Steps?
                .LastOrDefault(item =>
                    !string.IsNullOrWhiteSpace(item?.OverlayImageFile)
                    || !string.IsNullOrWhiteSpace(item?.ResultImageFile))
                ?? report?.Steps?.LastOrDefault();
            string drawingPath = ResolveExistingPath(
                reportDirectory,
                !string.IsNullOrWhiteSpace(evidenceStep?.OverlayImageFile)
                    ? evidenceStep.OverlayImageFile
                    : evidenceStep?.ResultImageFile);
            bool prefixFailed = report?.Steps?.Take(prefixCount).Any(item => item.Executed && !item.ToolSuccess) == true;
            string metricText = prefixCount == 0 ? BuildMetricText(report) : string.Join("; ", (report?.Steps?.LastOrDefault()?.Metrics ?? new List<VisionPipelineMetricRunReport>())
                .Select(metric => metric.Name + "=" + metric.Value.ToString("0.###", CultureInfo.InvariantCulture)));
            return new VisionToolNImageVerificationRow
            {
                Index = index,
                ImagePath = identity.Path,
                DisplayName = identity.Name,
                Status = ResolveStatus(check, hasAcceptance),
                Success = check?.ExecutionCompleted == true
                    && check.HasToolError == false
                    && (!hasAcceptance || check.Success),
                TotalMilliseconds = check?.TotalMilliseconds ?? 0D,
                Message = (prefixFailed ? "Prefix execution failed. " : "") + (check?.Message ?? string.Empty),
                TargetSummary = targetSummary,
                MetricText = string.IsNullOrWhiteSpace(metricText) ? "-" : metricText,
                RunReportPath = check?.RunReportPath ?? string.Empty,
                SourceSnapshotPath = sourcePath,
                DrawingPath = drawingPath,
                SourceSha256 = report?.SourceImageSha256 ?? string.Empty,
                FailedStep = check?.FailedStepText ?? string.Empty,
                IsCompleted = true
            };
        }

        private static string ResolveStatus(VisionPipelineSampleCheckResult check, bool hasAcceptance)
        {
            if (check?.ExecutionCompleted != true || check.HasToolError)
            {
                return "ERROR";
            }

            if (!hasAcceptance)
            {
                return "RUN OK";
            }

            return check.Success ? "OK" : "NG";
        }

        private static string SaveBatchSummary(
            string recipeName,
            VisionPipeline pipeline,
            string toolName,
            string definitionSha256,
            IReadOnlyList<VisionToolNImageVerificationRow> rows,
            DateTime startedAt,
            DateTime finishedAt,
            bool wasCancelled,
            bool normalizeInputToGray,
            bool hasAcceptance, string evidenceDirectory)
        {
            List<VisionPipelineBatchSampleRunResult> results = rows
                .Select(row => new VisionPipelineBatchSampleRunResult
                {
                    SampleName = $"{row.Index:0000} {row.FileName}",
                    Status = row.Status,
                    Success = row.Success,
                    TotalMilliseconds = row.TotalMilliseconds,
                    FailedStep = row.FailedStep,
                    Message = row.Message,
                    ReportPath = row.RunReportPath,
                    SampleImagePath = row.ImagePath,
                    PairGroup = "ToolView N-image verification",
                    PairRole = "UNLABELED",
                    ExpectedText = hasAcceptance
                        ? "Acceptance gate result retained"
                        : "Execution-only; operator review required",
                    MetricText = row.MetricText,
                    MetricReviewText = hasAcceptance
                        ? "Acceptance gate evaluated; operator drawing review retained."
                        : "No acceptance gate; operator drawing review required.",
                    FinalLayer = pipeline.Steps.Last().OutputLayer,
                    OverlayCount = row.HasDrawing ? "retained" : "missing",
                    ActionSummary = wasCancelled ? "Partial run stopped by operator." : "Completed sequential run.",
                    RunReportPath = row.RunReportPath
                })
                .ToList();
            string notes =
                $"ToolView={toolName}; StepDefinitionSha256={definitionSha256}; Execution=Sequential; "
                + $"InputNormalization={(normalizeInputToGray ? "GrayLikeToolPreview" : "PreserveChannels")}; "
                + $"State={(wasCancelled ? "StoppedPartial" : "Completed")}; "
                + (hasAcceptance
                    ? "Configured acceptance gate was evaluated."
                    : "No OK/NG acceptance gate was inferred.");
            VisionPipelineExecutionProvenance executionProvenance = rows
                .Select(row => VisionPipelineRunReportStorage.Load(row.RunReportPath)?.ExecutionProvenance)
                .FirstOrDefault(provenance => provenance != null);
            return VisionPipelineBatchRunSummaryStorage.Save(
                recipeName,
                pipeline.Name,
                startedAt,
                finishedAt,
                results,
                suiteName: toolName + " N-image verification",
                suiteKind: wasCancelled ? "ToolViewNImagePartial" : "ToolViewNImage",
                notes: notes,
                pipelineSnapshot: pipeline,
                executionProvenance: executionProvenance, destinationRoot: evidenceDirectory == null ? null : Path.Combine(evidenceDirectory, "summary"));
        }

        private static void ApplyReviewReasons(
            string summaryPath,
            IReadOnlyList<VisionToolNImageVerificationRow> rows)
        {
            VisionPipelineBatchRunSummary summary =
                VisionPipelineBatchRunSummaryStorage.Load(summaryPath);
            if (summary?.ReviewQueue == null)
            {
                return;
            }

            foreach (VisionPipelineBatchReviewQueueEntry entry in summary.ReviewQueue)
            {
                if (entry.ResultIndex < 0 || entry.ResultIndex >= rows.Count)
                {
                    continue;
                }

                rows[entry.ResultIndex].ReviewReasonText = string.Join(", ", entry.Reasons ?? new List<string>());
            }
        }

        private static string BuildMetricText(VisionPipelineRunReport report)
        {
            return string.Join(
                "; ",
                (report?.Steps ?? new List<VisionPipelineStepRunReport>())
                    .SelectMany(step => step?.Metrics ?? new List<VisionPipelineMetricRunReport>())
                    .OrderBy(metric => metric.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(metric => metric.Name + "=" + metric.Value.ToString("0.###", CultureInfo.InvariantCulture)));
        }

        private static string ResolveExistingPath(string directory, string fileName)
        {
            if (string.IsNullOrWhiteSpace(directory) || string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            string path = Path.Combine(directory, fileName);
            return File.Exists(path) ? path : string.Empty;
        }

        internal static string SerializePipeline(VisionPipeline pipeline)
        {
            using StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            new XmlSerializer(typeof(VisionPipeline)).Serialize(writer, pipeline);
            return writer.ToString();
        }

        private static bool IsSupportedImage(string path)
        {
            string extension = Path.GetExtension(path);
            return string.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".tif", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".tiff", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeName(string primary, string secondary, string fallback)
        {
            if (!string.IsNullOrWhiteSpace(primary))
            {
                return primary.Trim();
            }

            return string.IsNullOrWhiteSpace(secondary) ? fallback : secondary.Trim();
        }

        private static string SanitizeName(string value)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string sanitized = new string((value ?? string.Empty)
                .Select(character => invalid.Contains(character) || char.IsWhiteSpace(character) ? '_' : character)
                .ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Tool" : sanitized;
        }
    }
}
