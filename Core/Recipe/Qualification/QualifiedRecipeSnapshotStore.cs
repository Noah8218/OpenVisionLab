using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal sealed class QualifiedRecipeSnapshotStore
    {
        internal const string SnapshotManifestFileName = "snapshot.xml";
        internal const string InventoryFileName = "inventory.sha256";
        private const string CreatingPrefix = ".creating-";
        private readonly string _rootDirectory;

        internal QualifiedRecipeSnapshotStore(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                throw new ArgumentException("Qualified Recipe root is required.", nameof(rootDirectory));
            }

            _rootDirectory = Path.GetFullPath(rootDirectory);
        }

        internal static QualifiedRecipeSnapshotStore CreateDefault()
        {
            return new QualifiedRecipeSnapshotStore(
                Path.Combine(AppPathService.StartupPath, "QUALIFIED_RECIPE"));
        }

        internal QualifiedRecipeSnapshotCreateResult Create(
            QualifiedRecipeSnapshotCreateRequest request)
        {
            QualifiedRecipeSnapshotPreflightResult preflight =
                QualifiedRecipeSnapshotPreflight.Evaluate(request);
            if (!preflight.Success)
            {
                return Failure(string.Join(Environment.NewLine, preflight.Errors));
            }

            Directory.CreateDirectory(_rootDirectory);
            string predecessorId =
                QualifiedRecipeSnapshotPreflight.NormalizeSha(
                    request.PredecessorSnapshotId);
            if (!string.IsNullOrWhiteSpace(predecessorId))
            {
                if (!QualifiedRecipeSnapshotPreflight.IsSha256(predecessorId)
                    || !Verify(predecessorId).Success)
                {
                    return Failure(
                        "Predecessor Snapshot must be an existing valid snapshot.");
                }

                if (string.IsNullOrWhiteSpace(request.ChangeReason))
                {
                    return Failure(
                        "A predecessor Snapshot requires a non-empty change reason.");
                }
            }

            string temporaryDirectory = Path.Combine(
                _rootDirectory,
                CreatingPrefix + Guid.NewGuid().ToString("N"));
            string createdFinalDirectory = string.Empty;
            try
            {
                Directory.CreateDirectory(temporaryDirectory);
                QualifiedRecipeSnapshotManifest manifest =
                    WritePayload(temporaryDirectory, request, preflight);
                WriteInventory(temporaryDirectory, manifest);
                manifest.IdempotencyKeySha256 = ComputeIdempotencyKey(manifest);
                manifest.SnapshotId = ComputeSnapshotId(manifest);
                SerializeHelper.SaveXmlFile(
                    Path.Combine(temporaryDirectory, SnapshotManifestFileName),
                    manifest);

                QualifiedRecipeSnapshotVerificationResult temporaryVerification =
                    VerifyDirectory(
                        temporaryDirectory,
                        expectedSnapshotId: manifest.SnapshotId,
                        requireDirectoryIdentity: false);
                if (!temporaryVerification.Success)
                {
                    throw new InvalidDataException(
                        "Temporary snapshot verification failed: "
                        + string.Join(" | ", temporaryVerification.Errors));
                }

                QualifiedRecipeSnapshotVerificationResult reusable =
                    FindReusableSnapshot(manifest.IdempotencyKeySha256);
                if (reusable != null)
                {
                    DeleteOwnedTemporaryDirectory(temporaryDirectory);
                    return new QualifiedRecipeSnapshotCreateResult
                    {
                        Success = true,
                        ReusedExisting = true,
                        SnapshotId = reusable.SnapshotId,
                        SnapshotDirectory = Path.Combine(
                            _rootDirectory,
                            reusable.SnapshotId)
                    };
                }

                string finalDirectory = Path.Combine(_rootDirectory, manifest.SnapshotId);
                if (Directory.Exists(finalDirectory))
                {
                    QualifiedRecipeSnapshotVerificationResult existing =
                        VerifyDirectory(
                            finalDirectory,
                            expectedSnapshotId: manifest.SnapshotId,
                            requireDirectoryIdentity: true);
                    if (!existing.Success)
                    {
                        throw new InvalidDataException(
                            "An existing snapshot with the same identity is invalid: "
                            + string.Join(" | ", existing.Errors));
                    }

                    DeleteOwnedTemporaryDirectory(temporaryDirectory);
                    return new QualifiedRecipeSnapshotCreateResult
                    {
                        Success = true,
                        ReusedExisting = true,
                        SnapshotId = manifest.SnapshotId,
                        SnapshotDirectory = finalDirectory
                    };
                }

                Directory.Move(temporaryDirectory, finalDirectory);
                createdFinalDirectory = finalDirectory;
                QualifiedRecipeSnapshotVerificationResult finalVerification =
                    VerifyDirectory(
                        finalDirectory,
                        expectedSnapshotId: manifest.SnapshotId,
                        requireDirectoryIdentity: true);
                if (!finalVerification.Success)
                {
                    throw new InvalidDataException(
                        "Final snapshot verification failed: "
                        + string.Join(" | ", finalVerification.Errors));
                }

                return new QualifiedRecipeSnapshotCreateResult
                {
                    Success = true,
                    SnapshotId = manifest.SnapshotId,
                    SnapshotDirectory = finalDirectory
                };
            }
            catch (Exception ex)
            {
                DeleteOwnedTemporaryDirectory(temporaryDirectory);
                DeleteOwnedFinalDirectory(createdFinalDirectory);
                return Failure(ex.GetBaseException().Message);
            }
        }

        internal QualifiedRecipeSnapshotVerificationResult Verify(string snapshotId)
        {
            if (!QualifiedRecipeSnapshotPreflight.IsSha256(snapshotId))
            {
                QualifiedRecipeSnapshotVerificationResult invalid =
                    new QualifiedRecipeSnapshotVerificationResult();
                invalid.Errors.Add("Snapshot ID must be a 64-character SHA-256 value.");
                return invalid;
            }

            return VerifyDirectory(
                Path.Combine(_rootDirectory, snapshotId.ToUpperInvariant()),
                snapshotId.ToUpperInvariant(),
                requireDirectoryIdentity: true);
        }

        internal IReadOnlyList<string> ListSnapshotIds()
        {
            if (!Directory.Exists(_rootDirectory))
            {
                return Array.Empty<string>();
            }

            return Directory.EnumerateDirectories(_rootDirectory)
                .Select(Path.GetFileName)
                .Where(QualifiedRecipeSnapshotPreflight.IsSha256)
                .Select(value => value.ToUpperInvariant())
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToList();
        }

        internal string GetSnapshotDirectory(string snapshotId)
        {
            string normalized =
                QualifiedRecipeSnapshotPreflight.NormalizeSha(snapshotId);
            return QualifiedRecipeSnapshotPreflight.IsSha256(normalized)
                ? Path.Combine(_rootDirectory, normalized)
                : string.Empty;
        }

        private static QualifiedRecipeSnapshotManifest WritePayload(
            string temporaryDirectory,
            QualifiedRecipeSnapshotCreateRequest request,
            QualifiedRecipeSnapshotPreflightResult preflight)
        {
            string pipelinePath = Path.Combine(temporaryDirectory, "pipeline.xml");
            File.Copy(request.PipelineFilePath, pipelinePath, overwrite: false);

            string validationPath = Path.Combine(temporaryDirectory, "validation-set.xml");
            SerializeHelper.SaveXmlFile(validationPath, request.ValidationSet);

            string evidenceDirectory = Path.Combine(temporaryDirectory, "evidence");
            Directory.CreateDirectory(evidenceDirectory);
            string summaryPath = Path.Combine(evidenceDirectory, "summary.xml");
            File.Copy(request.BatchSummaryFilePath, summaryPath, overwrite: false);
            string sourceSummaryTsv = Path.Combine(
                Path.GetDirectoryName(request.BatchSummaryFilePath) ?? string.Empty,
                "summary.tsv");
            if (!File.Exists(sourceSummaryTsv))
            {
                throw new FileNotFoundException(
                    "Batch summary TSV is required for a self-contained snapshot.",
                    sourceSummaryTsv);
            }

            File.Copy(
                sourceSummaryTsv,
                Path.Combine(evidenceDirectory, "summary.tsv"),
                overwrite: false);

            QualifiedRecipeSnapshotManifest manifest = new QualifiedRecipeSnapshotManifest
            {
                SchemaVersion = QualifiedRecipeSnapshotPreflight.CurrentSnapshotSchemaVersion,
                Scope = request.Scope.ToString(),
                CreatedAtUtc = request.CreatedAtUtc.ToUniversalTime().ToString("o"),
                DisplayName = request.DisplayName.Trim(),
                QualificationNote = request.QualificationNote.Trim(),
                SourceRecipeName = request.SourceRecipeName.Trim(),
                PipelineName = request.PipelineName.Trim(),
                PipelineSha256 = preflight.PipelineSha256,
                ValidationSetSha256 =
                    QualifiedRecipeSnapshotPreflight.ComputeFileSha256(validationPath),
                BatchSummarySha256 =
                    QualifiedRecipeSnapshotPreflight.ComputeFileSha256(summaryPath),
                ReviewQueuePolicy = preflight.Summary.ReviewQueuePolicy ?? string.Empty,
                ReviewQueueSha256 = preflight.ReviewQueueSha256,
                PredecessorSnapshotId =
                    QualifiedRecipeSnapshotPreflight.NormalizeSha(request.PredecessorSnapshotId),
                ChangeReason = request.ChangeReason?.Trim() ?? string.Empty,
                Counts = preflight.Counts,
                RuntimeFingerprint = preflight.RuntimeFingerprint
                    .OrderBy(item => item.Label, StringComparer.Ordinal)
                    .ToList()
            };

            WriteDependencies(temporaryDirectory, request.ValidationSet, manifest);
            WriteEvidenceRows(temporaryDirectory, preflight, manifest);
            return manifest;
        }

        private static void WriteDependencies(
            string temporaryDirectory,
            QualifiedRecipeValidationSetSnapshot validationSet,
            QualifiedRecipeSnapshotManifest manifest)
        {
            string dependencyDirectory = Path.Combine(temporaryDirectory, "dependencies");
            foreach (QualifiedRecipeDependencySource dependency in
                validationSet.Dependencies?.Where(item => item != null)
                ?? Enumerable.Empty<QualifiedRecipeDependencySource>())
            {
                Directory.CreateDirectory(dependencyDirectory);
                string sha = QualifiedRecipeSnapshotPreflight.NormalizeSha(dependency.Sha256);
                string archiveName = sha + "_" + SafeFileName(Path.GetFileName(dependency.SourcePath));
                string archivePath = Path.Combine(dependencyDirectory, archiveName);
                File.Copy(dependency.SourcePath, archivePath, overwrite: false);
                FileInfo info = new FileInfo(archivePath);
                manifest.Dependencies.Add(new QualifiedRecipeArchivedFile
                {
                    LogicalPath = dependency.LogicalPath ?? string.Empty,
                    ArchivePath = ToArchivePath(
                        Path.GetRelativePath(temporaryDirectory, archivePath)),
                    Size = info.Length,
                    Sha256 = QualifiedRecipeSnapshotPreflight.ComputeFileSha256(archivePath)
                });
            }
        }

        private static void WriteEvidenceRows(
            string temporaryDirectory,
            QualifiedRecipeSnapshotPreflightResult preflight,
            QualifiedRecipeSnapshotManifest manifest)
        {
            string runsDirectory = Path.Combine(temporaryDirectory, "evidence", "runs");
            Directory.CreateDirectory(runsDirectory);
            foreach (QualifiedRecipePreparedEvidenceRow prepared in
                preflight.Rows.OrderBy(row => row.Index))
            {
                string directoryName =
                    (prepared.Index + 1).ToString("D4", CultureInfo.InvariantCulture)
                    + "_" + SafeFileName(prepared.Result.SampleName);
                string destinationDirectory = Path.Combine(runsDirectory, directoryName);
                CopyEvidenceDirectory(prepared.ReportDirectory, destinationDirectory);

                string sourceReportPath = Path.GetFullPath(prepared.Result.RunReportPath);
                string reportRelativeToSource =
                    Path.GetRelativePath(prepared.ReportDirectory, sourceReportPath);
                string reportArchivePath = Path.Combine(
                    destinationDirectory,
                    reportRelativeToSource);
                string pipelineArchivePath = MapEvidencePath(
                    prepared.ReportDirectory,
                    destinationDirectory,
                    prepared.PipelinePath);
                string sourceArchivePath = MapEvidencePath(
                    prepared.ReportDirectory,
                    destinationDirectory,
                    prepared.SourcePath);
                if (!File.Exists(reportArchivePath)
                    || !File.Exists(pipelineArchivePath)
                    || !File.Exists(sourceArchivePath))
                {
                    throw new InvalidDataException(
                        "Copied evidence row is incomplete: " + prepared.Result.SampleName);
                }

                manifest.EvidenceRows.Add(new QualifiedRecipeEvidenceRow
                {
                    Index = prepared.Index,
                    SampleName = prepared.Result.SampleName ?? string.Empty,
                    ExpectedOutcome = prepared.Result.ExpectedOutcome ?? string.Empty,
                    ActualOutcome = prepared.Result.ActualOutcome ?? string.Empty,
                    JudgmentCorrect = prepared.Result.JudgmentCorrect,
                    SourceSha256 = QualifiedRecipeSnapshotPreflight.NormalizeSha(
                        prepared.ValidationImage.Sha256),
                    ReportFile = ToArchivePath(
                        Path.GetRelativePath(temporaryDirectory, reportArchivePath)),
                    ReportSha256 =
                        QualifiedRecipeSnapshotPreflight.ComputeFileSha256(reportArchivePath),
                    PipelineFile = ToArchivePath(
                        Path.GetRelativePath(temporaryDirectory, pipelineArchivePath)),
                    PipelineSha256 =
                        QualifiedRecipeSnapshotPreflight.ComputeFileSha256(pipelineArchivePath),
                    SourceFile = ToArchivePath(
                        Path.GetRelativePath(temporaryDirectory, sourceArchivePath))
                });
            }
        }

        private static void WriteInventory(
            string temporaryDirectory,
            QualifiedRecipeSnapshotManifest manifest)
        {
            List<InventoryEntry> entries = BuildPayloadInventory(temporaryDirectory);
            string inventoryText = string.Join(
                "\n",
                entries.Select(entry => entry.Sha256 + " *" + entry.ArchivePath)) + "\n";
            string inventoryPath = Path.Combine(temporaryDirectory, InventoryFileName);
            File.WriteAllText(inventoryPath, inventoryText, new UTF8Encoding(false));
            manifest.InventorySha256 =
                QualifiedRecipeSnapshotPreflight.ComputeFileSha256(inventoryPath);
        }

        private static QualifiedRecipeSnapshotVerificationResult VerifyDirectory(
            string directory,
            string expectedSnapshotId,
            bool requireDirectoryIdentity)
        {
            QualifiedRecipeSnapshotVerificationResult result =
                new QualifiedRecipeSnapshotVerificationResult();
            if (!Directory.Exists(directory))
            {
                result.Errors.Add("Snapshot directory is missing: " + directory);
                return result;
            }

            string manifestPath = Path.Combine(directory, SnapshotManifestFileName);
            string inventoryPath = Path.Combine(directory, InventoryFileName);
            if (!SerializeHelper.TryLoadFromXmlFile(
                    manifestPath,
                    out QualifiedRecipeSnapshotManifest manifest)
                || manifest == null)
            {
                result.Errors.Add("Snapshot manifest is missing or invalid.");
                return result;
            }

            result.Manifest = manifest;
            result.SnapshotId =
                QualifiedRecipeSnapshotPreflight.NormalizeSha(manifest.SnapshotId);
            if (manifest.SchemaVersion !=
                QualifiedRecipeSnapshotPreflight.CurrentSnapshotSchemaVersion)
            {
                result.Errors.Add("Unsupported snapshot schema: " + manifest.SchemaVersion);
            }

            string computedId = ComputeSnapshotId(manifest);
            string computedIdempotencyKey = ComputeIdempotencyKey(manifest);
            if (!string.Equals(
                    QualifiedRecipeSnapshotPreflight.NormalizeSha(
                        manifest.IdempotencyKeySha256),
                    computedIdempotencyKey,
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Snapshot idempotency identity SHA-256 mismatch.");
            }

            if (!string.Equals(result.SnapshotId, computedId, StringComparison.Ordinal))
            {
                result.Errors.Add("Snapshot manifest identity SHA-256 mismatch.");
            }

            if (!string.IsNullOrWhiteSpace(expectedSnapshotId)
                && !string.Equals(
                    result.SnapshotId,
                    QualifiedRecipeSnapshotPreflight.NormalizeSha(expectedSnapshotId),
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Snapshot ID does not match the requested identity.");
            }

            if (requireDirectoryIdentity
                && !string.Equals(
                    Path.GetFileName(directory),
                    result.SnapshotId,
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add("Snapshot directory name does not match the manifest identity.");
            }

            VerifyInventory(directory, inventoryPath, manifest, result);
            VerifySemanticPayload(directory, manifest, result);
            VerifyRuntimeFingerprint(manifest, result);
            result.PayloadIntegrityValid = !result.Errors.Any(error =>
                !error.StartsWith("Runtime fingerprint", StringComparison.Ordinal));
            return result;
        }

        private static void VerifyInventory(
            string directory,
            string inventoryPath,
            QualifiedRecipeSnapshotManifest manifest,
            QualifiedRecipeSnapshotVerificationResult result)
        {
            if (!File.Exists(inventoryPath))
            {
                result.Errors.Add("Inventory file is missing.");
                return;
            }

            string inventorySha =
                QualifiedRecipeSnapshotPreflight.ComputeFileSha256(inventoryPath);
            if (!string.Equals(
                    inventorySha,
                    QualifiedRecipeSnapshotPreflight.NormalizeSha(manifest.InventorySha256),
                    StringComparison.Ordinal))
            {
                result.Errors.Add("Inventory SHA-256 does not match the manifest.");
                return;
            }

            if (!TryParseInventory(inventoryPath, out List<InventoryEntry> stored, out string error))
            {
                result.Errors.Add(error);
                return;
            }

            List<InventoryEntry> actual = BuildPayloadInventory(directory);
            if (stored.Count != actual.Count)
            {
                result.Errors.Add(
                    $"Inventory file count mismatch. Stored={stored.Count}, actual={actual.Count}.");
                return;
            }

            for (int index = 0; index < stored.Count; index++)
            {
                if (!string.Equals(
                        stored[index].ArchivePath,
                        actual[index].ArchivePath,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        stored[index].Sha256,
                        actual[index].Sha256,
                        StringComparison.Ordinal))
                {
                    result.Errors.Add(
                        "Inventory payload mismatch: " + stored[index].ArchivePath);
                    return;
                }
            }
        }

        private static void VerifySemanticPayload(
            string directory,
            QualifiedRecipeSnapshotManifest manifest,
            QualifiedRecipeSnapshotVerificationResult result)
        {
            VerifyManifestFile(
                directory,
                manifest.PipelineFile,
                manifest.PipelineSha256,
                "Pipeline",
                result);
            VerifyManifestFile(
                directory,
                manifest.ValidationSetFile,
                manifest.ValidationSetSha256,
                "Validation Set",
                result);
            string summaryPath = VerifyManifestFile(
                directory,
                manifest.BatchSummaryFile,
                manifest.BatchSummarySha256,
                "Batch summary",
                result);
            if (!string.IsNullOrWhiteSpace(summaryPath))
            {
                VisionPipelineBatchRunSummary summary =
                    VisionPipelineBatchRunSummaryStorage.Load(summaryPath);
                if (summary == null
                    || summary.SchemaVersion
                        != VisionPipelineBatchRunSummaryStorage.CurrentSchemaVersion
                    || !string.Equals(
                        summary.ReviewQueueSha256,
                        manifest.ReviewQueueSha256,
                        StringComparison.OrdinalIgnoreCase)
                    || summary.Results?.Count != manifest.EvidenceRows?.Count)
                {
                    result.Errors.Add("Archived batch summary semantic identity mismatch.");
                }
            }

            foreach (QualifiedRecipeArchivedFile dependency in
                manifest.Dependencies ?? new List<QualifiedRecipeArchivedFile>())
            {
                string path = VerifyManifestFile(
                    directory,
                    dependency.ArchivePath,
                    dependency.Sha256,
                    "Dependency",
                    result);
                if (!string.IsNullOrWhiteSpace(path)
                    && new FileInfo(path).Length != dependency.Size)
                {
                    result.Errors.Add("Dependency size mismatch: " + dependency.ArchivePath);
                }
            }

            foreach (QualifiedRecipeEvidenceRow row in
                manifest.EvidenceRows ?? new List<QualifiedRecipeEvidenceRow>())
            {
                string reportPath = VerifyManifestFile(
                    directory,
                    row.ReportFile,
                    row.ReportSha256,
                    "Run report",
                    result);
                string pipelinePath = VerifyManifestFile(
                    directory,
                    row.PipelineFile,
                    row.PipelineSha256,
                    "Per-row Pipeline",
                    result);
                string sourcePath = VerifyManifestFile(
                    directory,
                    row.SourceFile,
                    row.SourceSha256,
                    "Per-row source",
                    result);
                if (string.IsNullOrWhiteSpace(reportPath)
                    || string.IsNullOrWhiteSpace(pipelinePath)
                    || string.IsNullOrWhiteSpace(sourcePath))
                {
                    continue;
                }

                VisionPipelineRunReport report =
                    VisionPipelineRunReportStorage.Load(reportPath);
                string archivedReportDirectory =
                    Path.GetDirectoryName(reportPath) ?? string.Empty;
                string reportPipelinePath = ResolveArchivedReportArtifact(
                    archivedReportDirectory,
                    report?.PipelineSnapshotFile);
                string reportSourcePath = ResolveArchivedReportArtifact(
                    archivedReportDirectory,
                    report?.SourceImageFile);
                bool hasDrawing = report?.Steps?.Where(step => step != null).Any(step =>
                    !string.IsNullOrWhiteSpace(
                        ResolveArchivedReportArtifact(
                            archivedReportDirectory,
                            step.OverlayImageFile))
                    || !string.IsNullOrWhiteSpace(
                        ResolveArchivedReportArtifact(
                            archivedReportDirectory,
                            step.ResultImageFile))) == true;
                if (report == null
                    || !string.Equals(
                        report.SourceImageSha256,
                        row.SourceSha256,
                        StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(
                        report.PipelineName,
                        manifest.PipelineName,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        reportPipelinePath,
                        pipelinePath,
                        StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(
                        reportSourcePath,
                        sourcePath,
                        StringComparison.OrdinalIgnoreCase)
                    || !hasDrawing)
                {
                    result.Errors.Add(
                        "Archived run report semantic identity mismatch: " + row.SampleName);
                }
            }
        }

        private static string VerifyManifestFile(
            string directory,
            string archivePath,
            string expectedSha256,
            string label,
            QualifiedRecipeSnapshotVerificationResult result)
        {
            if (!TryResolveArchivePath(directory, archivePath, out string path)
                || !File.Exists(path))
            {
                result.Errors.Add(label + " file is missing: " + (archivePath ?? string.Empty));
                return string.Empty;
            }

            string actualSha = QualifiedRecipeSnapshotPreflight.ComputeFileSha256(path);
            if (!string.Equals(
                    actualSha,
                    QualifiedRecipeSnapshotPreflight.NormalizeSha(expectedSha256),
                    StringComparison.Ordinal))
            {
                result.Errors.Add(label + " SHA-256 mismatch: " + archivePath);
                return string.Empty;
            }

            return path;
        }

        private static void VerifyRuntimeFingerprint(
            QualifiedRecipeSnapshotManifest manifest,
            QualifiedRecipeSnapshotVerificationResult result)
        {
            bool matches = true;
            foreach (QualifiedRecipeRuntimeFingerprint runtime in
                manifest.RuntimeFingerprint ?? new List<QualifiedRecipeRuntimeFingerprint>())
            {
                if (!File.Exists(runtime.SourcePath))
                {
                    matches = false;
                    result.Errors.Add(
                        "Runtime fingerprint file is missing: " + runtime.Label);
                    continue;
                }

                FileInfo info = new FileInfo(runtime.SourcePath);
                if (info.Length != runtime.Size
                    || !string.Equals(
                        QualifiedRecipeSnapshotPreflight.ComputeFileSha256(info.FullName),
                        runtime.Sha256,
                        StringComparison.OrdinalIgnoreCase))
                {
                    matches = false;
                    result.Errors.Add(
                        "Runtime fingerprint mismatch: " + runtime.Label);
                }
            }

            result.RuntimeFingerprintMatches = matches;
        }

        private QualifiedRecipeSnapshotVerificationResult FindReusableSnapshot(
            string idempotencyKey)
        {
            foreach (string snapshotId in ListSnapshotIds())
            {
                QualifiedRecipeSnapshotVerificationResult verification = Verify(snapshotId);
                if (verification.Success
                    && string.Equals(
                        verification.Manifest?.IdempotencyKeySha256,
                        idempotencyKey,
                        StringComparison.Ordinal))
                {
                    return verification;
                }
            }

            return null;
        }

        private static string ComputeIdempotencyKey(
            QualifiedRecipeSnapshotManifest manifest)
        {
            StringBuilder canonical = new StringBuilder();
            AppendIdentity(canonical, "schema", manifest.SchemaVersion.ToString(CultureInfo.InvariantCulture));
            AppendIdentity(canonical, "scope", manifest.Scope);
            AppendIdentity(canonical, "display", manifest.DisplayName);
            AppendIdentity(canonical, "note", manifest.QualificationNote);
            AppendIdentity(canonical, "recipe", manifest.SourceRecipeName);
            AppendIdentity(canonical, "pipeline", manifest.PipelineName);
            AppendIdentity(canonical, "pipelineSha", manifest.PipelineSha256);
            AppendIdentity(canonical, "validationSha", manifest.ValidationSetSha256);
            AppendIdentity(canonical, "summarySha", manifest.BatchSummarySha256);
            AppendIdentity(canonical, "queuePolicy", manifest.ReviewQueuePolicy);
            AppendIdentity(canonical, "queueSha", manifest.ReviewQueueSha256);
            AppendIdentity(canonical, "inventorySha", manifest.InventorySha256);
            AppendIdentity(canonical, "predecessor", manifest.PredecessorSnapshotId);
            AppendIdentity(canonical, "changeReason", manifest.ChangeReason);
            QualifiedRecipeQualificationCounts counts =
                manifest.Counts ?? new QualifiedRecipeQualificationCounts();
            AppendIdentity(
                canonical,
                "counts",
                string.Join(
                    ",",
                    counts.Total,
                    counts.ExpectedOk,
                    counts.ExpectedNg,
                    counts.CorrectAccept,
                    counts.CorrectReject,
                    counts.FalseAccept,
                    counts.FalseReject,
                    counts.ExecutionError,
                    counts.EvidenceGap));
            foreach (QualifiedRecipeArchivedFile dependency in
                (manifest.Dependencies ?? new List<QualifiedRecipeArchivedFile>())
                .OrderBy(item => item.ArchivePath, StringComparer.Ordinal))
            {
                AppendIdentity(
                    canonical,
                    "dependency",
                    dependency.LogicalPath + "|" + dependency.ArchivePath + "|"
                    + dependency.Size + "|" + dependency.Sha256);
            }

            foreach (QualifiedRecipeRuntimeFingerprint runtime in
                (manifest.RuntimeFingerprint ?? new List<QualifiedRecipeRuntimeFingerprint>())
                .OrderBy(item => item.Label, StringComparer.Ordinal))
            {
                AppendIdentity(
                    canonical,
                    "runtime",
                    runtime.Label + "|" + runtime.SourcePath + "|" + runtime.FileVersion + "|"
                    + runtime.Size + "|" + runtime.Sha256);
            }

            foreach (QualifiedRecipeEvidenceRow row in
                (manifest.EvidenceRows ?? new List<QualifiedRecipeEvidenceRow>())
                .OrderBy(item => item.Index))
            {
                AppendIdentity(
                    canonical,
                    "row",
                    row.Index + "|" + row.SampleName + "|" + row.ExpectedOutcome + "|"
                    + row.ActualOutcome + "|" + row.JudgmentCorrect + "|"
                    + row.SourceSha256 + "|" + row.ReportFile + "|"
                    + row.ReportSha256 + "|" + row.PipelineFile + "|"
                    + row.PipelineSha256 + "|" + row.SourceFile);
            }

            return QualifiedRecipeSnapshotPreflight.ComputeTextSha256(canonical.ToString());
        }

        private static string ComputeSnapshotId(QualifiedRecipeSnapshotManifest manifest)
        {
            StringBuilder canonical = new StringBuilder();
            AppendIdentity(
                canonical,
                "idempotencyKey",
                manifest.IdempotencyKeySha256);
            AppendIdentity(canonical, "createdAtUtc", manifest.CreatedAtUtc);
            return QualifiedRecipeSnapshotPreflight.ComputeTextSha256(canonical.ToString());
        }

        private static void AppendIdentity(
            StringBuilder builder,
            string key,
            string value)
        {
            string normalized = value ?? string.Empty;
            builder
                .Append(key)
                .Append(':')
                .Append(normalized.Length.ToString(CultureInfo.InvariantCulture))
                .Append(':')
                .Append(normalized)
                .Append('\n');
        }

        private static List<InventoryEntry> BuildPayloadInventory(string directory)
        {
            return Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                .Where(path =>
                {
                    string relative = ToArchivePath(Path.GetRelativePath(directory, path));
                    return !string.Equals(relative, SnapshotManifestFileName, StringComparison.Ordinal)
                        && !string.Equals(relative, InventoryFileName, StringComparison.Ordinal);
                })
                .Select(path => new InventoryEntry
                {
                    ArchivePath = ToArchivePath(Path.GetRelativePath(directory, path)),
                    Sha256 = QualifiedRecipeSnapshotPreflight.ComputeFileSha256(path)
                })
                .OrderBy(entry => entry.ArchivePath, StringComparer.Ordinal)
                .ToList();
        }

        private static bool TryParseInventory(
            string inventoryPath,
            out List<InventoryEntry> entries,
            out string error)
        {
            entries = new List<InventoryEntry>();
            error = string.Empty;
            HashSet<string> paths = new HashSet<string>(StringComparer.Ordinal);
            foreach (string rawLine in File.ReadAllLines(inventoryPath))
            {
                string line = rawLine.TrimEnd();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                int delimiter = line.IndexOf(" *", StringComparison.Ordinal);
                if (delimiter != 64)
                {
                    error = "Inventory line is invalid.";
                    return false;
                }

                string sha = line.Substring(0, delimiter).ToUpperInvariant();
                string path = line.Substring(delimiter + 2);
                if (!QualifiedRecipeSnapshotPreflight.IsSha256(sha)
                    || !IsSafeArchivePath(path)
                    || !paths.Add(path))
                {
                    error = "Inventory path/hash is invalid or duplicated: " + path;
                    return false;
                }

                entries.Add(new InventoryEntry
                {
                    ArchivePath = path,
                    Sha256 = sha
                });
            }

            entries = entries.OrderBy(entry => entry.ArchivePath, StringComparer.Ordinal).ToList();
            return true;
        }

        private static void CopyEvidenceDirectory(string source, string destination)
        {
            DirectoryInfo sourceInfo = new DirectoryInfo(source);
            if (!sourceInfo.Exists)
            {
                throw new DirectoryNotFoundException("Evidence directory is missing: " + source);
            }

            if ((sourceInfo.Attributes & FileAttributes.ReparsePoint) != 0)
            {
                throw new InvalidDataException("Evidence directory cannot be a reparse point.");
            }

            Directory.CreateDirectory(destination);
            foreach (string directory in Directory.EnumerateDirectories(
                source,
                "*",
                SearchOption.AllDirectories))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    throw new InvalidDataException(
                        "Evidence cannot contain a reparse directory: " + directory);
                }

                Directory.CreateDirectory(
                    Path.Combine(destination, Path.GetRelativePath(source, directory)));
            }

            foreach (string file in Directory.EnumerateFiles(
                source,
                "*",
                SearchOption.AllDirectories))
            {
                FileInfo info = new FileInfo(file);
                if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    throw new InvalidDataException(
                        "Evidence cannot contain a reparse file: " + file);
                }

                string target = Path.Combine(
                    destination,
                    Path.GetRelativePath(source, file));
                Directory.CreateDirectory(Path.GetDirectoryName(target) ?? destination);
                File.Copy(file, target, overwrite: false);
            }
        }

        private static string MapEvidencePath(
            string sourceDirectory,
            string destinationDirectory,
            string sourcePath)
        {
            string relative = Path.GetRelativePath(sourceDirectory, sourcePath);
            if (!IsSafeArchivePath(ToArchivePath(relative)))
            {
                throw new InvalidDataException(
                    "Evidence artifact is outside its report directory: " + sourcePath);
            }

            return Path.Combine(destinationDirectory, relative);
        }

        private static bool TryResolveArchivePath(
            string root,
            string archivePath,
            out string fullPath)
        {
            fullPath = string.Empty;
            if (!IsSafeArchivePath(archivePath))
            {
                return false;
            }

            try
            {
                string rootWithSeparator =
                    Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                string candidate = Path.GetFullPath(
                    Path.Combine(root, archivePath.Replace('/', Path.DirectorySeparatorChar)));
                if (!candidate.StartsWith(
                        rootWithSeparator,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                fullPath = candidate;
                return true;
            }
            catch
            {
                fullPath = string.Empty;
                return false;
            }
        }

        private static string ResolveArchivedReportArtifact(
            string reportDirectory,
            string storedPath)
        {
            if (string.IsNullOrWhiteSpace(storedPath)
                || Path.IsPathRooted(storedPath))
            {
                return string.Empty;
            }

            try
            {
                string rootWithSeparator =
                    Path.GetFullPath(reportDirectory).TrimEnd(Path.DirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                string candidate = Path.GetFullPath(
                    Path.Combine(reportDirectory, storedPath));
                return candidate.StartsWith(
                        rootWithSeparator,
                        StringComparison.OrdinalIgnoreCase)
                    && File.Exists(candidate)
                    ? candidate
                    : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool IsSafeArchivePath(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && !Path.IsPathRooted(value)
                && !value.Split('/').Any(part =>
                    string.IsNullOrWhiteSpace(part)
                    || string.Equals(part, "..", StringComparison.Ordinal)
                    || string.Equals(part, ".", StringComparison.Ordinal));
        }

        private static string ToArchivePath(string value)
        {
            return (value ?? string.Empty).Replace('\\', '/');
        }

        private static string SafeFileName(string value)
        {
            string name = string.IsNullOrWhiteSpace(value) ? "item" : value.Trim();
            char[] invalid = Path.GetInvalidFileNameChars();
            string safe = new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            if (safe.Length > 96)
            {
                safe = safe.Substring(0, 96);
            }

            return string.IsNullOrWhiteSpace(safe) ? "item" : safe;
        }

        private void DeleteOwnedTemporaryDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return;
            }

            string fullPath = Path.GetFullPath(path);
            string rootWithSeparator =
                _rootDirectory.TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase)
                || !Path.GetFileName(fullPath).StartsWith(
                    CreatingPrefix,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Refusing to delete a directory outside the owned creation scope.");
            }

            Directory.Delete(fullPath, recursive: true);
        }

        private void DeleteOwnedFinalDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return;
            }

            string fullPath = Path.GetFullPath(path);
            string rootWithSeparator =
                _rootDirectory.TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string name = Path.GetFileName(fullPath);
            if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase)
                || !QualifiedRecipeSnapshotPreflight.IsSha256(name))
            {
                throw new InvalidOperationException(
                    "Refusing to roll back a directory outside the owned Snapshot scope.");
            }

            Directory.Delete(fullPath, recursive: true);
        }

        private static QualifiedRecipeSnapshotCreateResult Failure(string error)
        {
            return new QualifiedRecipeSnapshotCreateResult
            {
                Success = false,
                Error = error ?? string.Empty
            };
        }

        private sealed class InventoryEntry
        {
            public string ArchivePath { get; set; } = string.Empty;
            public string Sha256 { get; set; } = string.Empty;
        }
    }
}
