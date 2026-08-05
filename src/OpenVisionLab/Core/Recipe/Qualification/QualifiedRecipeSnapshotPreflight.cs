using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal static class QualifiedRecipeSnapshotPreflight
    {
        internal const int CurrentSnapshotSchemaVersion = 2;
        internal const int CurrentValidationSetSchemaVersion = 1;

        internal static QualifiedRecipeSnapshotPreflightResult Evaluate(
            QualifiedRecipeSnapshotCreateRequest request)
        {
            QualifiedRecipeSnapshotPreflightResult result =
                new QualifiedRecipeSnapshotPreflightResult();
            if (request == null)
            {
                result.Errors.Add("Qualification request is null.");
                return result;
            }

            ValidateRequiredText(request.DisplayName, "Display name", result);
            ValidateRequiredText(request.QualificationNote, "Qualification note", result);
            ValidateRequiredText(request.SourceRecipeName, "Source Recipe name", result);
            ValidateRequiredText(request.PipelineName, "Pipeline name", result);
            ValidateRequiredFile(request.PipelineFilePath, "Pipeline XML", result);
            ValidateRequiredFile(request.BatchSummaryFilePath, "Batch summary", result);
            if (request.ValidationSet == null)
            {
                result.Errors.Add("Validation Set snapshot is missing.");
            }

            if (!result.Success)
            {
                return result;
            }

            ValidatePipeline(request, result);
            ValidateValidationSet(request, result);
            ValidateBatch(request, result);
            ValidateRuntimeFingerprint(request, result);
            if (result.Success)
            {
                ValidateScope(request.Scope, result);
            }

            return result;
        }

        internal static string ComputeFileSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            return Convert.ToHexString(SHA256.HashData(stream));
        }

        internal static string ComputeTextSha256(string value)
        {
            return Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)));
        }

        internal static bool IsSha256(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.Trim().Length == 64
                && value.Trim().All(Uri.IsHexDigit);
        }

        internal static string ResolveReportFile(string reportDirectory, string storedPath)
        {
            if (string.IsNullOrWhiteSpace(storedPath))
            {
                return string.Empty;
            }

            string candidate = Path.IsPathRooted(storedPath)
                ? storedPath
                : Path.Combine(reportDirectory ?? string.Empty, storedPath);
            return File.Exists(candidate) ? Path.GetFullPath(candidate) : string.Empty;
        }

        private static void ValidatePipeline(
            QualifiedRecipeSnapshotCreateRequest request,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            string xml = File.ReadAllText(request.PipelineFilePath);
            result.PipelineSha256 = ComputeFileSha256(request.PipelineFilePath);
            result.PipelineDefinitionSha256 = ComputeTextSha256(xml);
            if (!SerializeHelper.TryLoadFromXmlText(
                    xml,
                    out VisionPipeline pipeline,
                    out string loadError)
                || pipeline == null)
            {
                result.Errors.Add("Pipeline XML round-trip load failed: " + loadError);
                return;
            }

            if (!string.Equals(
                    pipeline.Name ?? string.Empty,
                    request.PipelineName,
                    StringComparison.Ordinal))
            {
                result.Errors.Add(
                    $"Pipeline identity mismatch. Expected '{request.PipelineName}', "
                    + $"actual '{pipeline.Name}'.");
            }

            if (pipeline.Steps == null || pipeline.Steps.Count == 0)
            {
                result.Errors.Add("Pipeline has no executable Steps.");
                return;
            }

            string roundTripPath = Path.Combine(
                Path.GetTempPath(),
                "OpenVisionLab-qualification-roundtrip-" + Guid.NewGuid().ToString("N") + ".xml");
            try
            {
                SerializeHelper.SaveXmlFile(roundTripPath, pipeline);
                if (!SerializeHelper.TryLoadFromXmlFile(roundTripPath, out VisionPipeline reloaded)
                    || reloaded == null
                    || !HaveEquivalentPipelineShape(pipeline, reloaded))
                {
                    result.Errors.Add("Pipeline XML round-trip changed the Pipeline/Step identity.");
                }
            }
            finally
            {
                if (File.Exists(roundTripPath))
                {
                    File.Delete(roundTripPath);
                }
            }
        }

        private static void ValidateValidationSet(
            QualifiedRecipeSnapshotCreateRequest request,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            QualifiedRecipeValidationSetSnapshot set = request.ValidationSet;
            if (set.SchemaVersion != CurrentValidationSetSchemaVersion)
            {
                result.Errors.Add(
                    "Unsupported qualification Validation Set schema: " + set.SchemaVersion);
            }

            ValidateRequiredText(set.Name, "Validation Set name", result);
            if (!string.Equals(set.PipelineName, request.PipelineName, StringComparison.Ordinal))
            {
                result.Errors.Add("Validation Set Pipeline name does not match the selected Pipeline.");
            }

            if (!string.Equals(
                    NormalizeSha(set.PipelineDefinitionSha256),
                    result.PipelineDefinitionSha256,
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Validation Set Pipeline definition SHA-256 mismatch.");
            }

            List<QualifiedRecipeValidationImageSource> images =
                set.Images?.Where(image => image != null).ToList()
                ?? new List<QualifiedRecipeValidationImageSource>();
            if (images.Count == 0)
            {
                result.Errors.Add("Validation Set has no images.");
            }

            HashSet<string> imagePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (QualifiedRecipeValidationImageSource image in images)
            {
                string expected = NormalizeOutcome(image.ExpectedOutcome);
                if (string.IsNullOrEmpty(expected))
                {
                    result.Errors.Add(
                        "Validation image expected outcome must be OK or NG: "
                        + (image.SourcePath ?? string.Empty));
                }

                ValidateHashedSourceFile(
                    image.SourcePath,
                    image.Sha256,
                    "Validation image",
                    result);
                if (File.Exists(image.SourcePath)
                    && !imagePaths.Add(Path.GetFullPath(image.SourcePath)))
                {
                    result.Errors.Add("Validation image path is duplicated: " + image.SourcePath);
                }

                if (!TryValidateVariantContract(image, out string contractError))
                {
                    result.Errors.Add("Validation image Variant contract is invalid: " + contractError);
                }
            }

            string imageSetSha = ComputeImageSetSha256(images);
            if (!string.Equals(
                    NormalizeSha(set.ImageSetSha256),
                    imageSetSha,
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Validation Set image-set SHA-256 mismatch.");
            }

            HashSet<string> dependencyPaths =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (QualifiedRecipeDependencySource dependency in
                set.Dependencies?.Where(item => item != null)
                ?? Enumerable.Empty<QualifiedRecipeDependencySource>())
            {
                ValidateRequiredText(dependency.LogicalPath, "Dependency logical path", result);
                ValidateHashedSourceFile(
                    dependency.SourcePath,
                    dependency.Sha256,
                    "Pipeline dependency",
                    result);
                if (File.Exists(dependency.SourcePath)
                    && !dependencyPaths.Add(Path.GetFullPath(dependency.SourcePath)))
                {
                    result.Errors.Add("Pipeline dependency is duplicated: " + dependency.SourcePath);
                }
            }
        }

        private static void ValidateBatch(
            QualifiedRecipeSnapshotCreateRequest request,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            VisionPipelineBatchRunSummary summary =
                VisionPipelineBatchRunSummaryStorage.Load(request.BatchSummaryFilePath);
            result.Summary = summary;
            if (summary == null)
            {
                result.Errors.Add("Batch summary XML could not be loaded.");
                return;
            }

            if (summary.SchemaVersion != VisionPipelineBatchRunSummaryStorage.CurrentSchemaVersion)
            {
                result.Errors.Add(
                    "Batch summary must use explicit outcome schema "
                    + VisionPipelineBatchRunSummaryStorage.CurrentSchemaVersion + ".");
            }

            if (!string.Equals(
                    summary.SuiteKind,
                    "LocalValidationSet",
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add("Qualification requires a LocalValidationSet batch.");
            }

            if (!string.Equals(
                    summary.RecipeName,
                    request.SourceRecipeName,
                    StringComparison.OrdinalIgnoreCase)
                || !string.Equals(
                    summary.PipelineName,
                    request.PipelineName,
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Batch Recipe/Pipeline identity mismatch.");
            }

            if (!string.Equals(
                    summary.SuiteName,
                    request.ValidationSet.Name,
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Batch Validation Set identity mismatch.");
            }

            List<VisionPipelineBatchSampleRunResult> rows =
                summary.Results?.Where(row => row != null).ToList()
                ?? new List<VisionPipelineBatchSampleRunResult>();
            List<QualifiedRecipeValidationImageSource> images =
                request.ValidationSet.Images?.Where(image => image != null).ToList()
                ?? new List<QualifiedRecipeValidationImageSource>();
            if (summary.TotalCount != rows.Count || rows.Count != images.Count)
            {
                result.Errors.Add(
                    $"Batch row count mismatch. Summary={summary.TotalCount}, "
                    + $"rows={rows.Count}, Validation Set={images.Count}.");
                return;
            }

            VisionPipelineBatchRunSummaryStorage.BatchReviewQueue queue =
                VisionPipelineBatchRunSummaryStorage.BuildReviewQueue(
                    rows,
                    summary.ReviewQueuePolicy);
            result.ReviewQueueSha256 = queue.Sha256;
            if (!string.Equals(
                    summary.ReviewQueuePolicy,
                    queue.Policy,
                    StringComparison.Ordinal)
                || !string.Equals(
                    NormalizeSha(summary.ReviewQueueSha256),
                    queue.Sha256,
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Stored review queue policy or SHA-256 does not rebuild exactly.");
            }

            if (!HaveEquivalentReviewQueue(summary.ReviewQueue, queue.Entries))
            {
                result.Errors.Add("Stored review queue entries do not rebuild exactly.");
            }

            if (queue.Entries.Any(entry =>
                entry?.Reasons?.Any(reason =>
                    string.Equals(reason, "evidence-gap", StringComparison.Ordinal)
                    || string.Equals(reason, "execution-error", StringComparison.Ordinal)
                    || string.Equals(reason, "runtime-failure", StringComparison.Ordinal)) == true))
            {
                result.Errors.Add("Review queue contains an execution or evidence gap.");
            }

            for (int index = 0; index < rows.Count; index++)
            {
                ValidateBatchRow(request, result, rows[index], images[index], index);
            }

            result.Counts.Total = rows.Count;
            result.Counts.ExpectedOk = rows.Count(row =>
                string.Equals(row.ExpectedOutcome, "OK", StringComparison.Ordinal));
            result.Counts.ExpectedNg = rows.Count(row =>
                string.Equals(row.ExpectedOutcome, "NG", StringComparison.Ordinal));
            result.Counts.CorrectAccept = rows.Count(row =>
                row.JudgmentCorrect
                && string.Equals(row.ExpectedOutcome, "OK", StringComparison.Ordinal)
                && string.Equals(row.ActualOutcome, "OK", StringComparison.Ordinal));
            result.Counts.CorrectReject = rows.Count(row =>
                row.JudgmentCorrect
                && string.Equals(row.ExpectedOutcome, "NG", StringComparison.Ordinal)
                && string.Equals(row.ActualOutcome, "NG", StringComparison.Ordinal));
            result.Counts.FalseAccept = summary.FalseAcceptCount;
            result.Counts.FalseReject = summary.FalseRejectCount;
            result.Counts.ExecutionError = summary.ExecutionErrorCount;
            result.Counts.EvidenceGap = queue.Entries.Count(entry =>
                entry?.Reasons?.Contains("evidence-gap", StringComparer.Ordinal) == true);

            if (summary.LegacyAmbiguousCount != 0
                || summary.JudgmentCount != rows.Count
                || summary.JudgmentCorrectCount != rows.Count
                || summary.FalseAcceptCount != 0
                || summary.FalseRejectCount != 0
                || summary.ExecutionErrorCount != 0)
            {
                result.Errors.Add(
                    "Batch contains legacy, incomplete, incorrect, or execution-error outcomes.");
            }
        }

        private static void ValidateBatchRow(
            QualifiedRecipeSnapshotCreateRequest request,
            QualifiedRecipeSnapshotPreflightResult preflight,
            VisionPipelineBatchSampleRunResult row,
            QualifiedRecipeValidationImageSource image,
            int index)
        {
            string prefix = "Batch row " + (index + 1).ToString(CultureInfo.InvariantCulture) + ": ";
            if (!VisionPipelineBatchOutcomeContract.HasExplicitOutcome(row)
                || !VisionPipelineBatchOutcomeContract.IsExecutionCompleted(row)
                || !row.HasJudgment
                || !row.JudgmentCorrect)
            {
                preflight.Errors.Add(prefix + "explicit completed correct judgment is required.");
            }

            string expected = NormalizeOutcome(image.ExpectedOutcome);
            if (!string.Equals(row.ExpectedOutcome, expected, StringComparison.Ordinal))
            {
                preflight.Errors.Add(prefix + "expected outcome/order does not match the Validation Set.");
            }

            if (!string.Equals(
                    NormalizeVariantId(row.VariantId),
                    NormalizeVariantId(image.VariantId),
                    StringComparison.Ordinal)
                || !string.Equals(row.ExpectedMetricName?.Trim(), image.ExpectedMetricName?.Trim(), StringComparison.Ordinal)
                || !string.Equals(row.ExpectedMetricMinimum?.Trim(), image.ExpectedMetricMinimum?.Trim(), StringComparison.Ordinal)
                || !string.Equals(row.ExpectedMetricMaximum?.Trim(), image.ExpectedMetricMaximum?.Trim(), StringComparison.Ordinal))
            {
                preflight.Errors.Add(prefix + "Variant or expected metric contract does not match the Validation Set.");
            }

            if (!SameFullPath(row.SampleImagePath, image.SourcePath)
                || !File.Exists(row.SampleImagePath)
                || !string.Equals(
                    ComputeFileSha256(row.SampleImagePath),
                    NormalizeSha(image.Sha256),
                    StringComparison.Ordinal))
            {
                preflight.Errors.Add(prefix + "source path or SHA-256 does not match the Validation Set.");
            }

            if (string.IsNullOrWhiteSpace(row.RunReportPath)
                || !File.Exists(row.RunReportPath))
            {
                preflight.Errors.Add(prefix + "run report is missing.");
                return;
            }

            VisionPipelineRunReport report =
                VisionPipelineRunReportStorage.Load(row.RunReportPath);
            if (report == null)
            {
                preflight.Errors.Add(prefix + "run report XML is invalid.");
                return;
            }

            if (!string.Equals(
                    report.RecipeName,
                    request.SourceRecipeName,
                    StringComparison.OrdinalIgnoreCase)
                || !string.Equals(
                    report.PipelineName,
                    request.PipelineName,
                    StringComparison.Ordinal))
            {
                preflight.Errors.Add(prefix + "run report Recipe/Pipeline identity mismatch.");
            }

            string reportDirectory = Path.GetDirectoryName(row.RunReportPath) ?? string.Empty;
            string reportPipeline =
                ResolveReportFile(reportDirectory, report.PipelineSnapshotFile);
            string reportSource =
                ResolveReportFile(reportDirectory, report.SourceImageFile);
            if (string.IsNullOrWhiteSpace(reportPipeline)
                || !string.Equals(
                    ComputeTextSha256(File.ReadAllText(reportPipeline)),
                    preflight.PipelineDefinitionSha256,
                    StringComparison.Ordinal))
            {
                preflight.Errors.Add(prefix + "stored Pipeline snapshot is missing or changed.");
            }

            if (string.IsNullOrWhiteSpace(reportSource)
                || !string.Equals(
                    ComputeFileSha256(reportSource),
                    NormalizeSha(image.Sha256),
                    StringComparison.Ordinal)
                || !string.Equals(
                    NormalizeSha(report.SourceImageSha256),
                    NormalizeSha(image.Sha256),
                    StringComparison.Ordinal))
            {
                preflight.Errors.Add(prefix + "stored source snapshot is missing or changed.");
            }

            bool hasDrawing = report.Steps?.Where(step => step != null).Any(step =>
            {
                string overlay = ResolveReportFile(
                    reportDirectory,
                    step.OverlayImageFile);
                string resultImage = ResolveReportFile(
                    reportDirectory,
                    step.ResultImageFile);
                return (!string.IsNullOrWhiteSpace(overlay)
                        && IsPathWithin(reportDirectory, overlay))
                    || (!string.IsNullOrWhiteSpace(resultImage)
                        && IsPathWithin(reportDirectory, resultImage));
            }) == true;
            if (!hasDrawing)
            {
                preflight.Errors.Add(prefix + "no retained drawing/result artifact exists.");
            }

            preflight.Rows.Add(new QualifiedRecipePreparedEvidenceRow
            {
                Index = index,
                Result = row,
                Report = report,
                ValidationImage = image,
                ReportDirectory = reportDirectory,
                PipelinePath = reportPipeline,
                SourcePath = reportSource
            });
        }

        private static void ValidateRuntimeFingerprint(
            QualifiedRecipeSnapshotCreateRequest request,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            List<QualifiedRecipeRuntimeFileSource> files =
                request.RuntimeFiles?.Where(file => file != null).ToList()
                ?? new List<QualifiedRecipeRuntimeFileSource>();
            HashSet<string> labels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (QualifiedRecipeRuntimeFileSource file in files)
            {
                ValidateRequiredText(file.Label, "Runtime fingerprint label", result);
                if (!labels.Add(file.Label ?? string.Empty))
                {
                    result.Errors.Add("Runtime fingerprint label is duplicated: " + file.Label);
                }

                if (!File.Exists(file.SourcePath))
                {
                    result.Errors.Add("Runtime fingerprint file is missing: " + file.SourcePath);
                    continue;
                }

                FileInfo info = new FileInfo(file.SourcePath);
                result.RuntimeFingerprint.Add(new QualifiedRecipeRuntimeFingerprint
                {
                    Label = file.Label.Trim(),
                    SourcePath = info.FullName,
                    FileVersion = FileVersionInfo.GetVersionInfo(info.FullName).FileVersion
                        ?? string.Empty,
                    Size = info.Length,
                    Sha256 = ComputeFileSha256(info.FullName)
                });
            }

            foreach (string required in new[]
            {
                "OpenVisionLab",
                "OpenVisionLab.Vision2D.dll",
                "OpenCvSharp.dll"
            })
            {
                if (!labels.Contains(required))
                {
                    result.Errors.Add("Required runtime fingerprint is missing: " + required);
                }
            }
        }

        private static void ValidateScope(
            QualifiedRecipeSnapshotScope scope,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            if (scope == QualifiedRecipeSnapshotScope.InspectionJudgment)
            {
                if (result.Counts.ExpectedOk < 1 || result.Counts.ExpectedNg < 1)
                {
                    result.Errors.Add(
                        "InspectionJudgment requires at least one expected OK and one expected NG row.");
                }

                return;
            }

            if (result.Counts.ExpectedOk < 1 || result.Counts.ExpectedNg != 0)
            {
                result.Errors.Add(
                    "LocatorStability requires one or more expected-OK rows and no expected-NG rows.");
            }

            if (result.Rows.Any(row =>
                !string.Equals(row.Result.ActualOutcome, "OK", StringComparison.Ordinal)))
            {
                result.Errors.Add("LocatorStability requires every locator row to complete as OK.");
            }
        }

        private static string ComputeImageSetSha256(
            IEnumerable<QualifiedRecipeValidationImageSource> images)
        {
            StringBuilder canonical = new StringBuilder();
            int index = 0;
            foreach (QualifiedRecipeValidationImageSource image in
                images ?? Enumerable.Empty<QualifiedRecipeValidationImageSource>())
            {
                canonical
                    .Append(index++.ToString("D6", CultureInfo.InvariantCulture))
                    .Append('|')
                    .Append(Path.GetFullPath(image?.SourcePath ?? string.Empty))
                    .Append('|')
                    .Append(NormalizeSha(image?.Sha256))
                    .Append('|')
                    .Append(NormalizeVariantId(image?.VariantId))
                    .Append('|')
                    .Append((image?.ExpectedMetricName ?? string.Empty).Trim())
                    .Append('|')
                    .Append((image?.ExpectedMetricMinimum ?? string.Empty).Trim())
                    .Append('|')
                    .Append((image?.ExpectedMetricMaximum ?? string.Empty).Trim())
                    .AppendLine();
            }

            return ComputeTextSha256(canonical.ToString());
        }

        private static bool HaveEquivalentReviewQueue(
            IReadOnlyList<VisionPipelineBatchReviewQueueEntry> expected,
            IReadOnlyList<VisionPipelineBatchReviewQueueEntry> actual)
        {
            List<VisionPipelineBatchReviewQueueEntry> left =
                expected?.Where(entry => entry != null).ToList()
                ?? new List<VisionPipelineBatchReviewQueueEntry>();
            List<VisionPipelineBatchReviewQueueEntry> right =
                actual?.Where(entry => entry != null).ToList()
                ?? new List<VisionPipelineBatchReviewQueueEntry>();
            if (left.Count != right.Count)
            {
                return false;
            }

            for (int index = 0; index < left.Count; index++)
            {
                VisionPipelineBatchReviewQueueEntry leftEntry = left[index];
                VisionPipelineBatchReviewQueueEntry rightEntry = right[index];
                if (leftEntry.ResultIndex != rightEntry.ResultIndex
                    || !string.Equals(
                        leftEntry.SampleName,
                        rightEntry.SampleName,
                        StringComparison.Ordinal)
                    || !SameFullPath(
                        leftEntry.SampleImagePath,
                        rightEntry.SampleImagePath)
                    || !SameFullPath(
                        leftEntry.RunReportPath,
                        rightEntry.RunReportPath)
                    || !string.Equals(
                        NormalizeSha(leftEntry.SourceSha256),
                        NormalizeSha(rightEntry.SourceSha256),
                        StringComparison.Ordinal)
                    || !string.Equals(
                        NormalizeVariantId(leftEntry.VariantId),
                        NormalizeVariantId(rightEntry.VariantId),
                        StringComparison.Ordinal)
                    || !(leftEntry.Reasons ?? new List<string>())
                        .SequenceEqual(
                            rightEntry.Reasons ?? new List<string>(),
                            StringComparer.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static string NormalizeVariantId(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Default" : value.Trim();
        }

        internal static bool TryValidateVariantContract(
            QualifiedRecipeValidationImageSource image,
            out string error)
        {
            string variantId = image?.VariantId?.Trim() ?? string.Empty;
            if (variantId.Length > 80 || variantId.Any(char.IsControl))
            {
                error = "Variant ID is invalid.";
                return false;
            }

            string metricNamesText = image?.ExpectedMetricName?.Trim() ?? string.Empty;
            string minimumsText = image?.ExpectedMetricMinimum?.Trim() ?? string.Empty;
            string maximumsText = image?.ExpectedMetricMaximum?.Trim() ?? string.Empty;
            if (metricNamesText.Length > 500 || metricNamesText.Any(char.IsControl))
            {
                error = "Expected metric names are invalid.";
                return false;
            }

            string[] metricNames = SplitMetricContractParts(metricNamesText);
            string[] minimums = SplitMetricContractParts(minimumsText);
            string[] maximums = SplitMetricContractParts(maximumsText);
            if (metricNames.Length == 0)
            {
                error = minimums.Length > 0 || maximums.Length > 0
                    ? "Expected metric name is required when a bound is entered."
                    : string.Empty;
                return string.IsNullOrEmpty(error);
            }

            if (!IsMetricContractPartCountValid(minimums, metricNames.Length)
                || !IsMetricContractPartCountValid(maximums, metricNames.Length))
            {
                error = "Expected metric bounds must contain either one value or one value per metric.";
                return false;
            }

            for (int index = 0; index < metricNames.Length; index++)
            {
                string metricName = metricNames[index];
                if (string.IsNullOrWhiteSpace(metricName)
                    || metricName.Length > 100
                    || metricName.Any(char.IsControl))
                {
                    error = "Expected metric name is invalid.";
                    return false;
                }

                string minimum = ResolveMetricContractPart(minimums, index);
                string maximum = ResolveMetricContractPart(maximums, index);
                bool hasMinimum = !string.IsNullOrWhiteSpace(minimum);
                bool hasMaximum = !string.IsNullOrWhiteSpace(maximum);
                if (!hasMinimum && !hasMaximum)
                {
                    error = "At least one expected metric bound is required.";
                    return false;
                }

                double minimumValue = double.NaN;
                double maximumValue = double.NaN;
                bool minimumValid = !hasMinimum || TryParseFinite(minimum, out minimumValue);
                bool maximumValid = !hasMaximum || TryParseFinite(maximum, out maximumValue);
                if (!minimumValid || !maximumValid)
                {
                    error = "Expected metric bounds must be finite numbers.";
                    return false;
                }

                if (hasMinimum && hasMaximum && minimumValue > maximumValue)
                {
                    error = "Expected metric minimum cannot exceed maximum.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        private static bool TryParseFinite(string value, out double parsed)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed)
                && !double.IsNaN(parsed)
                && !double.IsInfinity(parsed);
        }

        private static string[] SplitMetricContractParts(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? Array.Empty<string>()
                : value.Split(new[] { ';' }, StringSplitOptions.None)
                    .Select(part => part.Trim())
                    .ToArray();
        }

        private static bool IsMetricContractPartCountValid(string[] values, int metricCount)
        {
            return values.Length == 0 || values.Length == 1 || values.Length == metricCount;
        }

        private static string ResolveMetricContractPart(string[] values, int index)
        {
            if (values == null || values.Length == 0)
            {
                return string.Empty;
            }

            if (index >= 0 && index < values.Length)
            {
                return values[index]?.Trim() ?? string.Empty;
            }

            return values.Length == 1 ? values[0]?.Trim() ?? string.Empty : string.Empty;
        }

        private static bool HaveEquivalentPipelineShape(
            VisionPipeline expected,
            VisionPipeline actual)
        {
            if (!string.Equals(expected?.Name, actual?.Name, StringComparison.Ordinal)
                || expected?.Steps == null
                || actual?.Steps == null
                || expected.Steps.Count != actual.Steps.Count)
            {
                return false;
            }

            for (int index = 0; index < expected.Steps.Count; index++)
            {
                VisionPipelineStep left = expected.Steps[index];
                VisionPipelineStep right = actual.Steps[index];
                if (left == null
                    || right == null
                    || !string.Equals(left.Name, right.Name, StringComparison.Ordinal)
                    || !string.Equals(left.ToolType, right.ToolType, StringComparison.Ordinal)
                    || left.Enabled != right.Enabled
                    || !string.Equals(left.InputLayer, right.InputLayer, StringComparison.Ordinal)
                    || !string.Equals(left.OutputLayer, right.OutputLayer, StringComparison.Ordinal)
                    || (left.Parameters?.Count ?? 0) != (right.Parameters?.Count ?? 0))
                {
                    return false;
                }
            }

            return true;
        }

        private static void ValidateHashedSourceFile(
            string path,
            string expectedSha256,
            string label,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                result.Errors.Add(label + " is missing: " + (path ?? string.Empty));
                return;
            }

            if (!IsSha256(expectedSha256)
                || !string.Equals(
                    ComputeFileSha256(path),
                    NormalizeSha(expectedSha256),
                    StringComparison.Ordinal))
            {
                result.Errors.Add(label + " SHA-256 mismatch: " + path);
            }
        }

        private static void ValidateRequiredFile(
            string value,
            string label,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            if (string.IsNullOrWhiteSpace(value) || !File.Exists(value))
            {
                result.Errors.Add(label + " is missing: " + (value ?? string.Empty));
            }
        }

        private static void ValidateRequiredText(
            string value,
            string label,
            QualifiedRecipeSnapshotPreflightResult result)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result.Errors.Add(label + " is required.");
            }
        }

        private static bool SameFullPath(string left, string right)
        {
            try
            {
                return string.Equals(
                    Path.GetFullPath(left ?? string.Empty),
                    Path.GetFullPath(right ?? string.Empty),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsPathWithin(string root, string path)
        {
            try
            {
                string rootWithSeparator =
                    Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                return Path.GetFullPath(path).StartsWith(
                    rootWithSeparator,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeOutcome(string value)
        {
            if (string.Equals(value?.Trim(), "OK", StringComparison.OrdinalIgnoreCase))
            {
                return "OK";
            }

            if (string.Equals(value?.Trim(), "NG", StringComparison.OrdinalIgnoreCase))
            {
                return "NG";
            }

            return string.Empty;
        }

        internal static string NormalizeSha(string value)
        {
            return value?.Trim().ToUpperInvariant() ?? string.Empty;
        }
    }
}
