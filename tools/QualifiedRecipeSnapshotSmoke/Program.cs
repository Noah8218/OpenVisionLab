using Lib.OpenCV.Pipeline;
using OpenCvSharp;
using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            string outputRoot = args.Length > 0
                ? Path.GetFullPath(args[0])
                : Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "artifacts",
                    "qualified_recipe_snapshot_core_smoke");
            if (Directory.Exists(outputRoot))
            {
                throw new InvalidOperationException(
                    "Smoke output already exists; use a fresh artifact directory: " + outputRoot);
            }

            Directory.CreateDirectory(outputRoot);
            string workingRecipe = Path.Combine(outputRoot, "RECIPE", "SnapshotSmokeRecipe");
            string evidenceRoot = Path.Combine(workingRecipe, "evidence");
            string qualifiedRoot = Path.Combine(outputRoot, "QUALIFIED_RECIPE");
            Directory.CreateDirectory(evidenceRoot);
            string runtimeProbePath = Path.Combine(
                outputRoot,
                "runtime",
                "runtime-probe.bin");
            Directory.CreateDirectory(Path.GetDirectoryName(runtimeProbePath));
            File.WriteAllBytes(
                runtimeProbePath,
                Encoding.UTF8.GetBytes("runtime-probe-v1"));

            string pipelinePath = CreatePipeline(workingRecipe);
            string dependencyPath = Path.Combine(workingRecipe, "template.bin");
            File.WriteAllBytes(dependencyPath, Encoding.UTF8.GetBytes("fixed-template-v1"));
            List<QualifiedRecipeValidationImageSource> images =
                new List<QualifiedRecipeValidationImageSource>();
            List<VisionPipelineBatchSampleRunResult> rows =
                new List<VisionPipelineBatchSampleRunResult>();
            CreateEvidenceRow(
                evidenceRoot,
                pipelinePath,
                index: 0,
                expected: "OK",
                actual: "OK",
                images,
                rows);
            CreateEvidenceRow(
                evidenceRoot,
                pipelinePath,
                index: 1,
                expected: "NG",
                actual: "NG",
                images,
                rows);
            string summaryPath = CreateSummary(evidenceRoot, rows);

            QualifiedRecipeValidationSetSnapshot validationSet =
                new QualifiedRecipeValidationSetSnapshot
                {
                    Name = "Frozen OK-NG",
                    PipelineName = "Snapshot Pipeline",
                    PipelineDefinitionSha256 =
                        QualifiedRecipeSnapshotPreflight.ComputeTextSha256(
                            File.ReadAllText(pipelinePath)),
                    ImageSetSha256 = ComputeImageSetSha256(images),
                    Notes = "Two-row deterministic qualification smoke.",
                    Images = images,
                    Dependencies = new List<QualifiedRecipeDependencySource>
                    {
                        new QualifiedRecipeDependencySource
                        {
                            LogicalPath = "TemplatePath",
                            SourcePath = dependencyPath,
                            Sha256 = QualifiedRecipeSnapshotPreflight.ComputeFileSha256(
                                dependencyPath)
                        }
                    }
                };
            QualifiedRecipeSnapshotCreateRequest request =
                CreateRequest(
                    pipelinePath,
                    summaryPath,
                    validationSet,
                    runtimeProbePath,
                    "initial qualification");
            QualifiedRecipeSnapshotStore store =
                new QualifiedRecipeSnapshotStore(qualifiedRoot);

            QualifiedRecipeSnapshotCreateResult created = store.Create(request);
            Require(created.Success, "Initial snapshot creation failed: " + created.Error);
            Require(!created.ReusedExisting, "Initial snapshot unexpectedly reported reuse.");
            QualifiedRecipeSnapshotVerificationResult verified =
                store.Verify(created.SnapshotId);
            Require(verified.Success, "Initial verification failed: " + Join(verified.Errors));
            Require(verified.PayloadIntegrityValid, "Payload integrity was not valid.");
            Require(verified.RuntimeFingerprintMatches, "Runtime fingerprint did not match.");
            Require(verified.Manifest.Counts.Total == 2
                && verified.Manifest.Counts.ExpectedOk == 1
                && verified.Manifest.Counts.ExpectedNg == 1
                && verified.Manifest.Counts.CorrectAccept == 1
                && verified.Manifest.Counts.CorrectReject == 1,
                "Qualification counts were not frozen correctly.");
            Require(verified.Manifest.EvidenceRows.Count == 2
                && verified.Manifest.EvidenceRows.Any(row =>
                    string.Equals(row.VariantId, "Product_Field_FilmStripe_SurfaceReview", StringComparison.Ordinal)
                    && string.Equals(row.ExpectedMetricName, "ResultCount", StringComparison.Ordinal)
                    && string.Equals(row.ExpectedMetricMinimum, "3", StringComparison.Ordinal)
                    && string.Equals(row.ExpectedMetricMaximum, "8", StringComparison.Ordinal))
                && verified.Manifest.EvidenceRows.Any(row =>
                    string.Equals(row.VariantId, "Product_Field_TexturedRoller_SurfaceReview", StringComparison.Ordinal)
                    && string.Equals(row.ExpectedMetricMinimum, "1", StringComparison.Ordinal)
                    && string.Equals(row.ExpectedMetricMaximum, "4", StringComparison.Ordinal)),
                "Validation Variant contracts were not frozen in evidence rows.");

            request.CreatedAtUtc = request.CreatedAtUtc.AddMinutes(1);
            QualifiedRecipeSnapshotCreateResult reused = store.Create(request);
            Require(reused.Success
                && reused.ReusedExisting
                && reused.SnapshotId == created.SnapshotId,
                "Same-identity creation was not idempotent.");

            QualifiedRecipeSnapshotCreateRequest revisionRequest =
                CreateRequest(
                    pipelinePath,
                    summaryPath,
                    validationSet,
                    runtimeProbePath,
                    "approved revision");
            revisionRequest.PredecessorSnapshotId = created.SnapshotId;
            revisionRequest.ChangeReason = "Lifecycle smoke successor.";
            QualifiedRecipeSnapshotCreateResult revision = store.Create(revisionRequest);
            Require(revision.Success
                && revision.SnapshotId != created.SnapshotId,
                "Revision snapshot creation failed: " + revision.Error);

            QualifiedRecipeSnapshotLifecycleStore lifecycle =
                new QualifiedRecipeSnapshotLifecycleStore(qualifiedRoot, store);
            Require(
                lifecycle.TryAppend(
                    created.SnapshotId,
                    QualifiedRecipeSnapshotLifecycleAction.Superseded,
                    "Replaced by approved revision.",
                    revision.SnapshotId,
                    DateTime.UtcNow,
                    out QualifiedRecipeSnapshotLifecycleEvent supersededEvent,
                    out string lifecycleError),
                "Supersede lifecycle failed: " + lifecycleError);
            Require(
                lifecycle.Load(created.SnapshotId).State == "Superseded",
                "Superseded state did not reload.");
            Require(
                lifecycle.TryAppend(
                    revision.SnapshotId,
                    QualifiedRecipeSnapshotLifecycleAction.Revoked,
                    "Operator withdrew the revision.",
                    string.Empty,
                    DateTime.UtcNow,
                    out QualifiedRecipeSnapshotLifecycleEvent revokedEvent,
                    out lifecycleError),
                "Revoke lifecycle failed: " + lifecycleError);
            Require(
                lifecycle.Load(revision.SnapshotId).State == "Revoked",
                "Revoked state did not reload.");
            Require(
                !lifecycle.TryAppend(
                    revision.SnapshotId,
                    QualifiedRecipeSnapshotLifecycleAction.Revoked,
                    "Second terminal event must fail.",
                    string.Empty,
                    DateTime.UtcNow,
                    out _,
                    out _),
                "A second terminal lifecycle event was accepted.");

            string supersededEventDirectory = Path.Combine(
                qualifiedRoot,
                "lifecycle",
                created.SnapshotId + ".events");
            string supersededEventPath = Directory.EnumerateFiles(
                supersededEventDirectory,
                "*.xml").Single();
            byte[] originalLifecycleEvent = File.ReadAllBytes(supersededEventPath);
            File.WriteAllText(
                supersededEventPath,
                File.ReadAllText(supersededEventPath).Replace(
                    "Replaced by approved revision.",
                    "tampered lifecycle reason",
                    StringComparison.Ordinal));
            Require(
                !lifecycle.Load(created.SnapshotId).Success,
                "Lifecycle event tampering was not detected.");
            File.WriteAllBytes(supersededEventPath, originalLifecycleEvent);
            Require(
                lifecycle.Load(created.SnapshotId).Success,
                "Lifecycle did not verify after exact-byte restoration.");

            string pipelineArchive = Path.Combine(
                created.SnapshotDirectory,
                "pipeline.xml");
            byte[] originalPipeline = File.ReadAllBytes(pipelineArchive);
            File.AppendAllText(pipelineArchive, "\n<!-- tampered -->");
            QualifiedRecipeSnapshotVerificationResult tampered =
                store.Verify(created.SnapshotId);
            Require(
                !tampered.Success
                && tampered.Errors.Any(error =>
                    error.Contains("Inventory payload mismatch", StringComparison.Ordinal)
                    || error.Contains("Pipeline SHA-256 mismatch", StringComparison.Ordinal)),
                "Payload tampering did not fail closed with an exact reason.");
            File.WriteAllBytes(pipelineArchive, originalPipeline);
            Require(
                store.Verify(created.SnapshotId).Success,
                "Snapshot did not verify after exact-byte restoration.");

            string manifestArchive = Path.Combine(
                created.SnapshotDirectory,
                "snapshot.xml");
            byte[] originalManifest = File.ReadAllBytes(manifestArchive);
            string manifestText = File.ReadAllText(manifestArchive);
            File.WriteAllText(
                manifestArchive,
                manifestText.Replace(
                    verified.Manifest.CreatedAtUtc,
                    DateTime.UtcNow.AddDays(1).ToString("o"),
                    StringComparison.Ordinal));
            Require(
                !store.Verify(created.SnapshotId).Success,
                "Manifest creation-time tampering was not detected.");
            File.WriteAllBytes(manifestArchive, originalManifest);
            Require(
                store.Verify(created.SnapshotId).Success,
                "Snapshot did not verify after manifest restoration.");

            byte[] originalRuntimeProbe = File.ReadAllBytes(runtimeProbePath);
            File.AppendAllText(runtimeProbePath, "-changed");
            QualifiedRecipeSnapshotVerificationResult runtimeMismatch =
                store.Verify(created.SnapshotId);
            Require(
                !runtimeMismatch.Success
                && runtimeMismatch.PayloadIntegrityValid
                && !runtimeMismatch.RuntimeFingerprintMatches
                && runtimeMismatch.Errors.Any(error =>
                    error.Contains(
                        "Runtime fingerprint mismatch: SmokeRuntimeProbe",
                        StringComparison.Ordinal)),
                "Current-runtime drift was not separated from payload integrity.");
            File.WriteAllBytes(runtimeProbePath, originalRuntimeProbe);
            Require(
                store.Verify(created.SnapshotId).Success,
                "Snapshot did not verify after runtime restoration.");

            string collisionDirectory = Path.Combine(outputRoot, "collision");
            Directory.CreateDirectory(collisionDirectory);
            string collidingDependency = Path.Combine(
                collisionDirectory,
                "template.bin");
            File.Copy(dependencyPath, collidingDependency);
            validationSet.Dependencies.Add(new QualifiedRecipeDependencySource
            {
                LogicalPath = "SecondTemplatePath",
                SourcePath = collidingDependency,
                Sha256 = QualifiedRecipeSnapshotPreflight.ComputeFileSha256(
                    collidingDependency)
            });
            QualifiedRecipeSnapshotCreateRequest interruptedRequest =
                CreateRequest(
                    pipelinePath,
                    summaryPath,
                    validationSet,
                    runtimeProbePath,
                    "forced archive collision");
            QualifiedRecipeSnapshotCreateResult interrupted =
                store.Create(interruptedRequest);
            Require(
                !interrupted.Success
                && store.ListSnapshotIds().Count == 2
                && !Directory.EnumerateDirectories(qualifiedRoot)
                    .Select(Path.GetFileName)
                    .Any(name => name.StartsWith(".creating-", StringComparison.Ordinal)),
                "Failed mid-creation archive was not rolled back completely.");
            validationSet.Dependencies.RemoveAt(
                validationSet.Dependencies.Count - 1);

            string staleCreation = Path.Combine(
                qualifiedRoot,
                ".creating-interrupted-smoke");
            Directory.CreateDirectory(staleCreation);
            File.WriteAllText(Path.Combine(staleCreation, "partial.tmp"), "partial");
            Require(
                store.ListSnapshotIds().Count == 2
                && !store.ListSnapshotIds().Contains(
                    Path.GetFileName(staleCreation),
                    StringComparer.Ordinal),
                "Interrupted temporary directory was exposed as qualified.");

            Directory.Delete(workingRecipe, recursive: true);
            Require(
                store.Verify(created.SnapshotId).Success
                && store.Verify(revision.SnapshotId).Success,
                "Snapshot no longer verified after the mutable source Recipe was deleted.");

            WriteResult(
                outputRoot,
                created,
                revision,
                supersededEvent,
                revokedEvent,
                verified);
            Console.WriteLine("qualified_recipe_snapshot_core=OK");
            Console.WriteLine("snapshot_id=" + created.SnapshotId);
            Console.WriteLine("revision_snapshot_id=" + revision.SnapshotId);
            Console.WriteLine("artifact_root=" + outputRoot);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.GetBaseException().Message);
            return 1;
        }
    }

    private static string CreatePipeline(string workingRecipe)
    {
        Directory.CreateDirectory(workingRecipe);
        VisionPipeline pipeline = new VisionPipeline { Name = "Snapshot Pipeline" };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Threshold",
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Threshold_Result"
        };
        step.Parameters["THRESHOLD_TYPE"] = "Binary";
        step.Parameters["THRESHOLD"] = "128";
        step.Parameters["MAX_VALUE"] = "255";
        pipeline.Steps.Add(step);
        string path = Path.Combine(workingRecipe, "pipeline.xml");
        SerializeHelper.SaveXmlFile(path, pipeline);
        return path;
    }

    private static void CreateEvidenceRow(
        string evidenceRoot,
        string pipelinePath,
        int index,
        string expected,
        string actual,
        List<QualifiedRecipeValidationImageSource> images,
        List<VisionPipelineBatchSampleRunResult> rows)
    {
        string sampleName = (index + 1).ToString("D3") + "_" + expected + ".bin";
        string sourcePath = Path.Combine(evidenceRoot, sampleName);
        File.WriteAllBytes(
            sourcePath,
            Encoding.UTF8.GetBytes("source-" + index + "-" + expected));
        string sourceSha =
            QualifiedRecipeSnapshotPreflight.ComputeFileSha256(sourcePath);
        string reportDirectory = Path.Combine(
            evidenceRoot,
            "runs",
            (index + 1).ToString("D4"));
        Directory.CreateDirectory(reportDirectory);
        string reportPipelinePath = Path.Combine(reportDirectory, "pipeline.xml");
        File.Copy(pipelinePath, reportPipelinePath);
        string reportSourcePath = Path.Combine(reportDirectory, "source.bin");
        File.Copy(sourcePath, reportSourcePath);
        string drawingPath = Path.Combine(reportDirectory, "001_threshold_overlay.bin");
        File.WriteAllBytes(drawingPath, Encoding.UTF8.GetBytes("drawing-" + index));
        VisionPipelineRunReport report = new VisionPipelineRunReport
        {
            RecipeName = "SnapshotSmokeRecipe",
            PipelineName = "Snapshot Pipeline",
            StartedAt = DateTime.UtcNow.ToString("o"),
            FinishedAt = DateTime.UtcNow.ToString("o"),
            Success = string.Equals(actual, "OK", StringComparison.Ordinal),
            PipelineSnapshotFile = "pipeline.xml",
            SourceImageFile = "source.bin",
            SourceImageSha256 = sourceSha,
            Steps = new List<VisionPipelineStepRunReport>
            {
                new VisionPipelineStepRunReport
                {
                    Index = 1,
                    Name = "Threshold",
                    ToolType = "Threshold",
                    Enabled = true,
                    InputLayer = "Main",
                    OutputLayer = "Threshold_Result",
                    Status = actual,
                    ToolSuccess = true,
                    AcceptancePassed = string.Equals(actual, "OK", StringComparison.Ordinal),
                    OverlayImageFile = "001_threshold_overlay.bin"
                }
            }
        };
        string reportPath = Path.Combine(reportDirectory, "report.xml");
        SerializeHelper.SaveXmlFile(reportPath, report);
        VisionPipelineBatchSampleRunResult row =
            new VisionPipelineBatchSampleRunResult
            {
                SampleName = sampleName,
                Status = actual,
                Success = true,
                SampleImagePath = sourcePath,
                ReportPath = sourcePath,
                PairGroup = "Frozen OK-NG",
                PairRole = expected,
                VariantId = string.Equals(expected, "OK", StringComparison.Ordinal)
                    ? "Product_Field_FilmStripe_SurfaceReview"
                    : "Product_Field_TexturedRoller_SurfaceReview",
                ExpectedMetricName = "ResultCount",
                ExpectedMetricMinimum = string.Equals(expected, "OK", StringComparison.Ordinal) ? "3" : "1",
                ExpectedMetricMaximum = string.Equals(expected, "OK", StringComparison.Ordinal) ? "8" : "4",
                ExpectedText = "ExpectedActual: Expected " + expected,
                RunReportPath = reportPath
            };
        VisionPipelineBatchOutcomeContract.Apply(
            row,
            executionCompleted: true,
            actualSuccess: string.Equals(actual, "OK", StringComparison.Ordinal),
            hasJudgment: true,
            expectedSuccess: string.Equals(expected, "OK", StringComparison.Ordinal),
            judgmentCorrect: string.Equals(expected, actual, StringComparison.Ordinal));
        images.Add(new QualifiedRecipeValidationImageSource
        {
            ExpectedOutcome = expected,
            SourcePath = sourcePath,
            Sha256 = sourceSha,
            VariantId = row.VariantId,
            ExpectedMetricName = row.ExpectedMetricName,
            ExpectedMetricMinimum = row.ExpectedMetricMinimum,
            ExpectedMetricMaximum = row.ExpectedMetricMaximum,
            Notes = "Qualification smoke row."
        });
        rows.Add(row);
    }

    private static string CreateSummary(
        string evidenceRoot,
        List<VisionPipelineBatchSampleRunResult> rows)
    {
        VisionPipelineBatchRunSummaryStorage.BatchReviewQueue queue =
            VisionPipelineBatchRunSummaryStorage.BuildReviewQueue(rows);
        VisionPipelineBatchRunSummary summary = new VisionPipelineBatchRunSummary
        {
            SchemaVersion = VisionPipelineBatchRunSummaryStorage.CurrentSchemaVersion,
            RecipeName = "SnapshotSmokeRecipe",
            PipelineName = "Snapshot Pipeline",
            SuiteName = "Frozen OK-NG",
            SuiteKind = "LocalValidationSet",
            StartedAt = DateTime.UtcNow.ToString("o"),
            FinishedAt = DateTime.UtcNow.ToString("o"),
            TotalCount = rows.Count,
            PassCount = rows.Count,
            JudgmentCount = rows.Count,
            JudgmentCorrectCount = rows.Count,
            Results = rows,
            ReviewQueuePolicy = queue.Policy,
            ReviewQueueSha256 = queue.Sha256,
            ReviewQueue = queue.Entries
        };
        string summaryPath = Path.Combine(evidenceRoot, "summary.xml");
        SerializeHelper.SaveXmlFile(summaryPath, summary);
        File.WriteAllText(
            Path.Combine(evidenceRoot, "summary.tsv"),
            "SampleName\tExpectedOutcome\tActualOutcome\tJudgmentCorrect\r\n"
            + string.Join(
                "\r\n",
                rows.Select(row =>
                    row.SampleName + "\t" + row.ExpectedOutcome + "\t"
                    + row.ActualOutcome + "\t" + row.JudgmentCorrect)));
        return summaryPath;
    }

    private static QualifiedRecipeSnapshotCreateRequest CreateRequest(
        string pipelinePath,
        string summaryPath,
        QualifiedRecipeValidationSetSnapshot validationSet,
        string runtimeProbePath,
        string note)
    {
        string openVisionLabAssembly = typeof(SerializeHelper).Assembly.Location;
        string libOpenCvAssembly = typeof(VisionPipeline).Assembly.Location;
        string openCvSharpAssembly = typeof(Mat).Assembly.Location;
        return new QualifiedRecipeSnapshotCreateRequest
        {
            Scope = QualifiedRecipeSnapshotScope.InspectionJudgment,
            DisplayName = "Snapshot smoke qualification",
            QualificationNote = note,
            SourceRecipeName = "SnapshotSmokeRecipe",
            PipelineName = "Snapshot Pipeline",
            PipelineFilePath = pipelinePath,
            BatchSummaryFilePath = summaryPath,
            ValidationSet = validationSet,
            CreatedAtUtc = DateTime.UtcNow,
            RuntimeFiles = new List<QualifiedRecipeRuntimeFileSource>
            {
                new QualifiedRecipeRuntimeFileSource
                {
                    Label = "OpenVisionLab",
                    SourcePath = openVisionLabAssembly
                },
                new QualifiedRecipeRuntimeFileSource
                {
                    Label = "Lib.OpenCV.dll",
                    SourcePath = libOpenCvAssembly
                },
                new QualifiedRecipeRuntimeFileSource
                {
                    Label = "OpenCvSharp.dll",
                    SourcePath = openCvSharpAssembly
                },
                new QualifiedRecipeRuntimeFileSource
                {
                    Label = "SmokeRuntimeProbe",
                    SourcePath = runtimeProbePath
                }
            }
        };
    }

    private static string ComputeImageSetSha256(
        IEnumerable<QualifiedRecipeValidationImageSource> images)
    {
        StringBuilder canonical = new StringBuilder();
        int index = 0;
        foreach (QualifiedRecipeValidationImageSource image in images)
        {
            canonical
                .Append(index++.ToString("D6"))
                .Append('|')
                .Append(Path.GetFullPath(image.SourcePath))
                .Append('|')
                .Append(image.Sha256.ToUpperInvariant())
                .Append('|')
                .Append(string.IsNullOrWhiteSpace(image.VariantId) ? "Default" : image.VariantId.Trim())
                .Append('|')
                .Append((image.ExpectedMetricName ?? string.Empty).Trim())
                .Append('|')
                .Append((image.ExpectedMetricMinimum ?? string.Empty).Trim())
                .Append('|')
                .Append((image.ExpectedMetricMaximum ?? string.Empty).Trim())
                .AppendLine();
        }

        return QualifiedRecipeSnapshotPreflight.ComputeTextSha256(canonical.ToString());
    }

    private static void WriteResult(
        string outputRoot,
        QualifiedRecipeSnapshotCreateResult created,
        QualifiedRecipeSnapshotCreateResult revision,
        QualifiedRecipeSnapshotLifecycleEvent superseded,
        QualifiedRecipeSnapshotLifecycleEvent revoked,
        QualifiedRecipeSnapshotVerificationResult verification)
    {
        File.WriteAllLines(
            Path.Combine(outputRoot, "SMOKE_RESULT.txt"),
            new[]
            {
                "Status=Complete",
                "SnapshotId=" + created.SnapshotId,
                "RevisionSnapshotId=" + revision.SnapshotId,
                "InitialSnapshotDirectory=" + created.SnapshotDirectory,
                "RevisionSnapshotDirectory=" + revision.SnapshotDirectory,
                "PayloadIntegrityValid=" + verification.PayloadIntegrityValid,
                "RuntimeFingerprintMatches=" + verification.RuntimeFingerprintMatches,
                "Counts=2 total, 1 expected OK, 1 expected NG, 1 correct accept, 1 correct reject",
                "IdempotentReuse=True",
                "TamperRejected=True",
                "ManifestTimestampTamperRejected=True",
                "LifecycleTamperRejected=True",
                "RuntimeMismatchSeparated=True",
                "InterruptedTemporaryExcluded=True",
                "FailedCreationRolledBack=True",
                "SourceRecipeDeletedAndVerified=True",
                "SupersededEventSha256=" + superseded.EventSha256,
                "RevokedEventSha256=" + revoked.EventSha256
            });
    }

    private static string Join(IEnumerable<string> values)
    {
        return string.Join(" | ", values ?? Array.Empty<string>());
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
