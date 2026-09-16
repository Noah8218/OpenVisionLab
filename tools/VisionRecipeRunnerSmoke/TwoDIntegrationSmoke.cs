using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OpenVisionLab;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;

internal static class TwoDIntegrationSmoke
{
    private const string ProducerCommit = "1111111111111111111111111111111111111111";

    public static async Task<int> RunAsync(
        string evidenceRoot,
        string goodImagePath,
        string badImagePath,
        string pipelineXmlPath,
        string runtimeBuildManifestPath)
    {
        string root = Path.GetFullPath(evidenceRoot);
        Directory.CreateDirectory(root);
        string runRoot = Path.Combine(
            root,
            $"two-d-integration-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runRoot);

        var runtimeBuildManifest = IntegrationContractJson.DeserializeRuntimeBuildManifest(
            File.ReadAllBytes(runtimeBuildManifestPath));
        var qualifiedTarget = runtimeBuildManifest.Identity with
        {
            SourceState = IntegrationSourceState.Clean
        };
        await RunWrongModalityFailClosedCaseAsync(
            runRoot,
            goodImagePath,
            pipelineXmlPath,
            qualifiedTarget,
            runtimeBuildManifestPath);
        if (runtimeBuildManifest.Identity.SourceState != IntegrationSourceState.Clean)
        {
            RunRuntimeManifestIntegrityNegativeCases(
                runRoot,
                goodImagePath,
                pipelineXmlPath,
                qualifiedTarget,
                runtimeBuildManifestPath);
            return await RunDirtyRuntimeFailClosedAsync(
                runRoot,
                goodImagePath,
                pipelineXmlPath,
                runtimeBuildManifest.Identity,
                runtimeBuildManifestPath);
        }

        var consumer = TwoDIntegrationBuildIdentity.LoadQualifiedIdentity(
            runtimeBuildManifestPath);
        RunRuntimeManifestIntegrityNegativeCases(
            runRoot,
            goodImagePath,
            pipelineXmlPath,
            consumer,
            runtimeBuildManifestPath);
        RunMetricUnitContractCase(runRoot);
        await RunConcurrentRunCaseAsync(
            runRoot,
            goodImagePath,
            pipelineXmlPath,
            consumer,
            runtimeBuildManifestPath);

        var good = await RunCaseAsync(
            runRoot,
            "good",
            goodImagePath,
            pipelineXmlPath,
            consumer,
            runtimeBuildManifestPath,
            expectedOutcome: IntegrationInspectionOutcome.Pass);
        var bad = await RunCaseAsync(
            runRoot,
            "bad",
            badImagePath,
            pipelineXmlPath,
            consumer,
            runtimeBuildManifestPath,
            expectedOutcome: IntegrationInspectionOutcome.Ng);
        await RunRejectedCaseAsync(
            runRoot,
            goodImagePath,
            pipelineXmlPath,
            consumer,
            runtimeBuildManifestPath);
        RunTamperCase(runRoot, goodImagePath, pipelineXmlPath, consumer);
        await RunQualifiedRuntimeIdentityNegativeCasesAsync(
            runRoot,
            goodImagePath,
            pipelineXmlPath,
            consumer,
            runtimeBuildManifestPath);

        Console.WriteLine(
            $"2D integration smoke passed. Good={good.Outcome}, Bad={bad.Outcome}, Evidence={runRoot}");
        return 0;
    }

    public static async Task<int> RunConcurrentProcessAsync(
        string evidenceRoot,
        string imagePath,
        string recipePath,
        string runtimeBuildManifestPath)
    {
        string root = Path.GetFullPath(evidenceRoot);
        Directory.CreateDirectory(root);
        string runRoot = Path.Combine(
            root,
            $"two-d-concurrent-process-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runRoot);

        _ = IntegrationContractJson.DeserializeRuntimeBuildManifest(
            File.ReadAllBytes(runtimeBuildManifestPath));
        var consumer = TwoDIntegrationBuildIdentity.LoadQualifiedIdentity(
            runtimeBuildManifestPath);

        var acknowledgementHandoff = CreateFixture(
            runRoot,
            "concurrent-ack",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        ConcurrentWorkerObservation[] acknowledgementWorkers =
            await RunWorkerPairAsync(
                "ack",
                runRoot,
                acknowledgementHandoff.TransactionId,
                runtimeBuildManifestPath);
        Require(
            acknowledgementWorkers.Count(worker => worker.Status == "Accepted") == 1
            && acknowledgementWorkers.Count(worker => worker.ErrorCode == IntegrationErrorCode.InvalidState.ToString()) == 1,
            "Concurrent ACK did not produce exactly one Accepted and one InvalidState result.");
        var persistedAcknowledgement = TwoDIntegrationExchange.ReadAcknowledgement(
            runRoot,
            acknowledgementHandoff.TransactionId);
        Require(
            persistedAcknowledgement.Status == IntegrationAcknowledgementStatus.Accepted,
            "Concurrent ACK did not leave one accepted Acknowledgement.");

        var runHandoff = CreateFixture(
            runRoot,
            "concurrent-run-process",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        var acknowledgement = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            runHandoff.TransactionId,
            runtimeBuildManifestPath);
        Require(
            acknowledgement.Status == IntegrationAcknowledgementStatus.Accepted,
            "The process-level concurrent Run fixture did not receive an accepted Acknowledgement.");
        ConcurrentWorkerObservation[] runWorkers = await RunWorkerPairAsync(
            "run",
            runRoot,
            runHandoff.TransactionId,
            runtimeBuildManifestPath);
        ConcurrentWorkerObservation[] successfulRuns = runWorkers
            .Where(worker => worker.Status == IntegrationResultStatus.Completed.ToString()
                && worker.Outcome == IntegrationInspectionOutcome.Pass.ToString())
            .ToArray();
        Require(
            successfulRuns.Length == 1
            && runWorkers.Count(worker => worker.ErrorCode == IntegrationErrorCode.InvalidState.ToString()) == 1,
            "Concurrent Run did not produce exactly one Completed/Pass and one InvalidState result.");
        var persistedResult = TwoDIntegrationExchange.ReadResult(
            runRoot,
            runHandoff.TransactionId);
        string resultPath = GetTransactionMessagePath(
            runRoot,
            runHandoff.TransactionId,
            IntegrationTransactionLayout.ResultFileName);
        Require(
            File.Exists(resultPath)
            && persistedResult.RunId == successfulRuns[0].RunId,
            "Concurrent Run did not leave one correlated Result.");

        string reportPath = Path.Combine(runRoot, "two-d-concurrent-process-smoke.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    acknowledgementTransactionId = acknowledgementHandoff.TransactionId,
                    acknowledgementWorkers,
                    persistedAcknowledgementStatus = persistedAcknowledgement.Status.ToString(),
                    runTransactionId = runHandoff.TransactionId,
                    runWorkers,
                    persistedResultStatus = persistedResult.Status.ToString(),
                    persistedResultOutcome = persistedResult.Outcome.ToString(),
                    persistedResultCount = File.Exists(resultPath) ? 1 : 0
                },
                new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(
            $"2D process concurrency: ackAccepted=1, ackRejected=1, runCompleted=1, runRejected=1, evidence={reportPath}");
        return 0;
    }

    public static int RunDiscoveryIsolationContract(
        string evidenceRoot,
        string imagePath,
        string recipePath)
    {
        string root = Path.GetFullPath(evidenceRoot);
        Directory.CreateDirectory(root);
        string runRoot = Path.Combine(
            root,
            $"two-d-discovery-isolation-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runRoot);

        var consumer = new IntegrationApplicationIdentity(
            IntegrationApplicationIds.TwoDStudio,
            "2.1.0",
            new string('2', 40),
            IntegrationSourceState.Clean);
        var first = CreateFixture(
            runRoot,
            "discovery-first",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        var second = CreateFixture(
            runRoot,
            "discovery-second",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        var corruptTransactionId = Guid.NewGuid();
        string corruptDirectory = Path.Combine(
            runRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            corruptTransactionId.ToString("D"));
        Directory.CreateDirectory(corruptDirectory);
        File.WriteAllText(
            Path.Combine(corruptDirectory, IntegrationTransactionLayout.HandoffFileName),
            "{\"schema\":");

        TwoDIntegrationDiscoveryResult detailed =
            TwoDIntegrationExchange.DiscoverHandoffsDetailed(runRoot);
        IReadOnlyList<TwoDIntegrationTransactionSummary> legacy =
            TwoDIntegrationExchange.DiscoverHandoffs(runRoot);
        TwoDIntegrationDiscoveryDiagnostic diagnostic =
            detailed.Diagnostics.Single(item => item.TransactionId == corruptTransactionId);

        Require(
            detailed.Transactions.Count == 2
                && detailed.Transactions.All(transaction =>
                    transaction.Handoff.TransactionId is var transactionId
                    && (transactionId == first.TransactionId || transactionId == second.TransactionId))
                && detailed.Diagnostics.Count == 1
                && diagnostic.ErrorCode == IntegrationErrorCode.MalformedMessage
                && !string.IsNullOrWhiteSpace(diagnostic.Message)
                && legacy.Count == 2
                && !File.Exists(Path.Combine(
                    corruptDirectory,
                    IntegrationTransactionLayout.AcknowledgementFileName))
                && !File.Exists(Path.Combine(
                    corruptDirectory,
                    IntegrationTransactionLayout.ResultFileName)),
            "A corrupt transaction blocked discovery, lost its typed diagnostic, or was acknowledged/run.");

        string reportPath = Path.Combine(runRoot, "two-d-discovery-isolation-contract.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    validTransactionCount = detailed.Transactions.Count,
                    legacyTransactionCount = legacy.Count,
                    diagnosticCount = detailed.Diagnostics.Count,
                    corruptTransactionId,
                    diagnosticErrorCode = diagnostic.ErrorCode.ToString(),
                    diagnosticMessage = diagnostic.Message,
                    acknowledgementPublished = File.Exists(Path.Combine(
                        corruptDirectory,
                        IntegrationTransactionLayout.AcknowledgementFileName)),
                    resultPublished = File.Exists(Path.Combine(
                        corruptDirectory,
                        IntegrationTransactionLayout.ResultFileName))
                },
                new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(
            $"2D discovery isolation: valid={detailed.Transactions.Count}, diagnostics={detailed.Diagnostics.Count}, error={diagnostic.ErrorCode}");
        Console.WriteLine($"2D discovery isolation evidence={reportPath}");
        return 0;
    }

    public static async Task<int> RunRunRecordRecoveryContractAsync(
        string evidenceRoot,
        string imagePath,
        string recipePath,
        string runtimeBuildManifestPath)
    {
        string root = Path.GetFullPath(evidenceRoot);
        Directory.CreateDirectory(root);
        string runRoot = Path.Combine(
            root,
            $"two-d-run-record-recovery-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runRoot);

        var runtimeManifest = IntegrationContractJson.DeserializeRuntimeBuildManifest(
            File.ReadAllBytes(runtimeBuildManifestPath));
        string qualifiedManifestPath = Path.Combine(
            root,
            "openvisionlab.runtime.clean.json");
        File.WriteAllBytes(
            qualifiedManifestPath,
            IntegrationContractJson.SerializeCanonical(
                runtimeManifest with
                {
                    Identity = runtimeManifest.Identity with
                    {
                        SourceState = IntegrationSourceState.Clean
                    }
                }));
        var consumer = TwoDIntegrationBuildIdentity.LoadQualifiedIdentity(
            qualifiedManifestPath);

        var normal = await RunCaseAsync(
            runRoot,
            "normal-recovery-baseline",
            imagePath,
            recipePath,
            consumer,
            qualifiedManifestPath,
            IntegrationInspectionOutcome.Pass);
        Require(
            normal.Status == IntegrationResultStatus.Completed
                && normal.Outcome == IntegrationInspectionOutcome.Pass,
            "The normal RunRecord/Result publication baseline did not complete.");

        var writeFailureHandoff = CreateFixture(
            runRoot,
            "result-write-failure",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        _ = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            writeFailureHandoff.TransactionId,
            qualifiedManifestPath);
        IntegrationResultV2 writeFailureResult;
        using (TwoDIntegrationExchange.BeginResultWriteFailureInjectionForTest())
        {
            writeFailureResult = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                writeFailureHandoff.TransactionId,
                qualifiedManifestPath);
        }

        string writeFailureDirectory = GetTransactionDirectory(
            runRoot,
            writeFailureHandoff.TransactionId);
        string writeFailureRecordPath = Path.Combine(
            writeFailureDirectory,
            IntegrationTransactionLayout.ArtifactsDirectoryName,
            "2d-run-record.json");
        string writeFailureResultPath = Path.Combine(
            writeFailureDirectory,
            IntegrationTransactionLayout.ResultFileName);
        Require(
            writeFailureResult.Status == IntegrationResultStatus.Failed
                && writeFailureResult.Outcome == IntegrationInspectionOutcome.ExecutionError
                && writeFailureResult.Error?.Code == IntegrationErrorCode.ExecutionFailed
                && !File.Exists(writeFailureRecordPath)
                && File.Exists(writeFailureResultPath),
            "A Result write failure did not clean the Run Record and publish one typed failure Result.");
        var persistedWriteFailure = TwoDIntegrationExchange.ReadResult(
            runRoot,
            writeFailureHandoff.TransactionId);
        Require(
            persistedWriteFailure.Status == IntegrationResultStatus.Failed
                && persistedWriteFailure.Error?.Code == IntegrationErrorCode.ExecutionFailed,
            "The Result write failure recovery Result was not readable or typed.");

        var crashHandoff = CreateFixture(
            runRoot,
            "crash-after-run-record",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        _ = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            crashHandoff.TransactionId,
            qualifiedManifestPath);
        string crashDirectory = GetTransactionDirectory(runRoot, crashHandoff.TransactionId);
        string markerPath = Path.Combine(runRoot, "crash-after-run-record.marker");
        string workerReportPath = Path.Combine(runRoot, "crash-after-run-record-worker.json");
        using Process worker = StartRunRecordPauseWorker(
            runRoot,
            crashHandoff.TransactionId,
            qualifiedManifestPath,
            markerPath,
            workerReportPath);
        bool markerObserved = await WaitForMarkerAsync(markerPath, worker);
        string crashRecordPath = Path.Combine(
            crashDirectory,
            IntegrationTransactionLayout.ArtifactsDirectoryName,
            "2d-run-record.json");
        string crashResultPath = Path.Combine(
            crashDirectory,
            IntegrationTransactionLayout.ResultFileName);
        Require(
            markerObserved
                && File.Exists(crashRecordPath)
                && !File.Exists(crashResultPath),
            "The process-boundary fixture did not stop after RunRecord publication and before Result publication.");

        if (!worker.HasExited)
        {
            worker.Kill(entireProcessTree: true);
        }

        await worker.WaitForExitAsync();
        int killedWorkerExitCode = worker.ExitCode;
        Require(
            !File.Exists(workerReportPath),
            "The killed worker unexpectedly published a completion report.");

        IntegrationResultV2 recoveryResult = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
            runRoot,
            crashHandoff.TransactionId,
            qualifiedManifestPath);
        Require(
            recoveryResult.Status == IntegrationResultStatus.Failed
                && recoveryResult.Outcome == IntegrationInspectionOutcome.ExecutionError
                && recoveryResult.Error?.Code == IntegrationErrorCode.ExecutionFailed
                && recoveryResult.Error.Message.Contains(
                    "Automatic rerun is blocked",
                    StringComparison.Ordinal),
            "An orphan Run Record was re-executed or did not publish the recovery-required Result.");
        var persistedRecovery = TwoDIntegrationExchange.ReadResult(
            runRoot,
            crashHandoff.TransactionId);
        Require(
            persistedRecovery.Status == IntegrationResultStatus.Failed
                && persistedRecovery.Outcome == IntegrationInspectionOutcome.ExecutionError
                && persistedRecovery.Error?.Code == IntegrationErrorCode.ExecutionFailed
                && File.Exists(crashRecordPath)
                && File.Exists(crashResultPath),
            "The recovery-required Result or original Run Record was not retained after restart.");

        string recoveryResultHash = ComputeSha256(crashResultPath);
        bool secondRestartRejected = false;
        try
        {
            _ = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                crashHandoff.TransactionId,
                qualifiedManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.InvalidState)
        {
            secondRestartRejected = true;
        }

        Require(
            secondRestartRejected
                && string.Equals(
                    recoveryResultHash,
                    ComputeSha256(crashResultPath),
                    StringComparison.OrdinalIgnoreCase),
            "A second restart did not preserve the single recovery Result.");

        string reportPath = Path.Combine(runRoot, "two-d-run-record-recovery-contract.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    normalStatus = normal.Status.ToString(),
                    normalOutcome = normal.Outcome.ToString(),
                    resultWriteFailureStatus = writeFailureResult.Status.ToString(),
                    resultWriteFailureErrorCode = writeFailureResult.Error?.Code.ToString(),
                    resultWriteFailureRunRecordRetained = File.Exists(writeFailureRecordPath),
                    resultWriteFailureResultCount = File.Exists(writeFailureResultPath) ? 1 : 0,
                    crashHandoffTransactionId = crashHandoff.TransactionId,
                    markerObserved,
                    killedWorkerExitCode,
                    orphanRunRecordRetained = File.Exists(crashRecordPath),
                    recoveryStatus = recoveryResult.Status.ToString(),
                    recoveryOutcome = recoveryResult.Outcome.ToString(),
                    recoveryErrorCode = recoveryResult.Error?.Code.ToString(),
                    recoveryResultCount = File.Exists(crashResultPath) ? 1 : 0,
                    secondRestartRejected
                },
                new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(
            $"2D RunRecord recovery: normal={normal.Outcome}, writeFailure={writeFailureResult.Error?.Code}, "
            + $"crashMarker={markerObserved}, recovery={recoveryResult.Error?.Code}, "
            + $"secondRestartRejected={secondRestartRejected}");
        Console.WriteLine($"2D RunRecord recovery evidence={reportPath}");
        return 0;
    }

    public static async Task<int> RunRunRecordRecoveryWorkerAsync(
        string runRoot,
        string transactionIdText,
        string runtimeBuildManifestPath,
        string markerPath,
        string reportPath)
    {
        try
        {
            Guid transactionId = Guid.Parse(transactionIdText);
            using var pause = TwoDIntegrationExchange.BeginRunRecordPauseForTest(markerPath);
            var result = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                transactionId,
                runtimeBuildManifestPath);
            File.WriteAllText(
                reportPath,
                JsonSerializer.Serialize(
                    new
                    {
                        status = result.Status.ToString(),
                        outcome = result.Outcome.ToString(),
                        errorCode = result.Error?.Code.ToString()
                    },
                    new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }
        catch (Exception exception)
        {
            File.WriteAllText(
                reportPath,
                JsonSerializer.Serialize(
                    new { error = exception.ToString() },
                    new JsonSerializerOptions { WriteIndented = true }));
            return 2;
        }
    }

    public static async Task<int> RunConcurrentProcessWorkerAsync(
        string operation,
        string runRoot,
        string transactionIdText,
        string runtimeBuildManifestPath,
        string reportPath)
    {
        try
        {
            Guid transactionId = Guid.Parse(transactionIdText);
            if (string.Equals(operation, "ack", StringComparison.OrdinalIgnoreCase))
            {
                var acknowledgement = TwoDIntegrationExchange.AcknowledgeHandoff(
                    runRoot,
                    transactionId,
                    runtimeBuildManifestPath);
                File.WriteAllText(
                    reportPath,
                    JsonSerializer.Serialize(
                        new
                        {
                            operation,
                            status = acknowledgement.Status.ToString(),
                            messageId = acknowledgement.MessageId.ToString("D")
                        },
                        new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            if (!string.Equals(operation, "run", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Unsupported concurrent worker operation: {operation}",
                    nameof(operation));
            }

            var result = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                transactionId,
                runtimeBuildManifestPath);
            File.WriteAllText(
                reportPath,
                JsonSerializer.Serialize(
                    new
                    {
                        operation,
                        status = result.Status.ToString(),
                        outcome = result.Outcome.ToString(),
                        runId = result.RunId
                    },
                    new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }
        catch (IntegrationContractException exception)
        {
            File.WriteAllText(
                reportPath,
                JsonSerializer.Serialize(
                    new
                    {
                        operation,
                        status = "Rejected",
                        errorCode = exception.ErrorCode.ToString(),
                        error = exception.Message
                    },
                    new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }
        catch (Exception exception)
        {
            File.WriteAllText(
                reportPath,
                JsonSerializer.Serialize(
                    new
                    {
                        operation,
                        status = "Error",
                        error = exception.ToString()
                    },
                    new JsonSerializerOptions { WriteIndented = true }));
            return 2;
        }
    }

    private static async Task<ConcurrentWorkerObservation[]> RunWorkerPairAsync(
        string operation,
        string runRoot,
        Guid transactionId,
        string runtimeBuildManifestPath)
    {
        string workerRoot = Path.Combine(runRoot, $"{operation}-workers");
        Directory.CreateDirectory(workerRoot);
        var workers = new List<Process>();
        var reportPaths = new List<string>();
        for (int index = 0; index < 2; index++)
        {
            string reportPath = Path.Combine(workerRoot, $"worker-{index + 1}.json");
            reportPaths.Add(reportPath);
            workers.Add(StartConcurrentWorker(
                operation,
                runRoot,
                transactionId,
                runtimeBuildManifestPath,
                reportPath));
        }

        int[] exitCodes = await Task.WhenAll(
            workers.Select(async worker =>
            {
                await worker.WaitForExitAsync();
                int exitCode = worker.ExitCode;
                worker.Dispose();
                return exitCode;
            }));
        Require(
            exitCodes.All(exitCode => exitCode == 0),
            $"A concurrent {operation} worker failed: {string.Join(",", exitCodes)}.");
        return reportPaths
            .Select(ReadConcurrentWorkerObservation)
            .ToArray();
    }

    private static Process StartConcurrentWorker(
        string operation,
        string runRoot,
        Guid transactionId,
        string runtimeBuildManifestPath,
        string reportPath)
    {
        string processPath = Environment.ProcessPath
            ?? throw new InvalidOperationException("The concurrent worker process path is unavailable.");
        var startInfo = new ProcessStartInfo
        {
            FileName = processPath,
            UseShellExecute = false,
            WorkingDirectory = AppContext.BaseDirectory
        };
        if (string.Equals(
                Path.GetFileNameWithoutExtension(processPath),
                "dotnet",
                StringComparison.OrdinalIgnoreCase))
        {
            startInfo.ArgumentList.Add(typeof(TwoDIntegrationSmoke).Assembly.Location);
        }
        startInfo.ArgumentList.Add("--integration-2d-concurrent-process-worker");
        startInfo.ArgumentList.Add(operation);
        startInfo.ArgumentList.Add(runRoot);
        startInfo.ArgumentList.Add(transactionId.ToString("D"));
        startInfo.ArgumentList.Add(runtimeBuildManifestPath);
        startInfo.ArgumentList.Add(reportPath);
        return Process.Start(startInfo)
            ?? throw new InvalidOperationException("The concurrent worker process could not start.");
    }

    private static Process StartRunRecordPauseWorker(
        string runRoot,
        Guid transactionId,
        string runtimeBuildManifestPath,
        string markerPath,
        string reportPath)
    {
        string processPath = Environment.ProcessPath
            ?? throw new InvalidOperationException("The RunRecord recovery worker process path is unavailable.");
        var startInfo = new ProcessStartInfo
        {
            FileName = processPath,
            UseShellExecute = false,
            WorkingDirectory = AppContext.BaseDirectory
        };
        if (string.Equals(
                Path.GetFileNameWithoutExtension(processPath),
                "dotnet",
                StringComparison.OrdinalIgnoreCase))
        {
            startInfo.ArgumentList.Add(typeof(TwoDIntegrationSmoke).Assembly.Location);
        }

        startInfo.ArgumentList.Add("--integration-2d-run-record-recovery-worker");
        startInfo.ArgumentList.Add(runRoot);
        startInfo.ArgumentList.Add(transactionId.ToString("D"));
        startInfo.ArgumentList.Add(runtimeBuildManifestPath);
        startInfo.ArgumentList.Add(markerPath);
        startInfo.ArgumentList.Add(reportPath);
        return Process.Start(startInfo)
            ?? throw new InvalidOperationException("The RunRecord recovery worker process could not start.");
    }

    private static async Task<bool> WaitForMarkerAsync(
        string markerPath,
        Process worker)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(15);
        while (!worker.HasExited && DateTime.UtcNow < deadline)
        {
            if (File.Exists(markerPath))
            {
                return true;
            }

            await Task.Delay(5);
        }

        return File.Exists(markerPath);
    }

    private static ConcurrentWorkerObservation ReadConcurrentWorkerObservation(
        string reportPath)
    {
        using JsonDocument document = JsonDocument.Parse(
            File.ReadAllText(reportPath));
        JsonElement root = document.RootElement;
        return new ConcurrentWorkerObservation(
            GetJsonString(root, "operation") ?? "unknown",
            GetJsonString(root, "status") ?? "unknown",
            GetJsonString(root, "outcome"),
            GetJsonString(root, "errorCode"),
            GetJsonString(root, "runId"));
    }

    private static string? GetJsonString(JsonElement root, string name) =>
        root.TryGetProperty(name, out JsonElement value)
        && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private sealed record ConcurrentWorkerObservation(
        string Operation,
        string Status,
        string? Outcome,
        string? ErrorCode,
        string? RunId);

    private static async Task RunWrongModalityFailClosedCaseAsync(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath)
    {
        var handoff = CreateFixture(
            runRoot,
            "wrong-modality",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false,
            modality: IntegrationInspectionModality.ThreeD,
            inputKind: IntegrationInspectionInputKind.HeightMap);
        var acknowledgement = new IntegrationAcknowledgementV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Acknowledgement,
            Guid.NewGuid(),
            handoff.TransactionId,
            handoff.MessageId,
            DateTimeOffset.UtcNow,
            consumer,
            IntegrationAcknowledgementStatus.Accepted,
            null);
        File.WriteAllBytes(
            GetTransactionMessagePath(
                runRoot,
                handoff.TransactionId,
                IntegrationTransactionLayout.AcknowledgementFileName),
            IntegrationContractJson.SerializeCanonical(acknowledgement));

        try
        {
            _ = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.RequestRejected)
        {
            Require(
                !File.Exists(GetTransactionMessagePath(
                    runRoot,
                    handoff.TransactionId,
                    IntegrationTransactionLayout.ResultFileName)),
                "A non-2D Handoff unexpectedly published a Result.");
            Console.WriteLine(
                "2D modality boundary rejected a crafted accepted ThreeD/HeightMap Handoff before Result publication.");
            return;
        }

        throw new InvalidOperationException(
            "A crafted accepted ThreeD/HeightMap Handoff crossed the 2D Run boundary.");
    }

    private static async Task<int> RunDirtyRuntimeFailClosedAsync(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity dirtyIdentity,
        string runtimeBuildManifestPath)
    {
        var handoff = CreateFixture(
            runRoot,
            "dirty-runtime",
            imagePath,
            recipePath,
            dirtyIdentity with
            {
                SourceState = IntegrationSourceState.Clean
            },
            tamperSourceAfterPublish: false);
        bool acknowledgementRejected = false;
        try
        {
            _ = TwoDIntegrationExchange.AcknowledgeHandoff(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.InvalidIdentity)
        {
            acknowledgementRejected = true;
            Require(
                !File.Exists(GetTransactionMessagePath(
                    runRoot,
                    handoff.TransactionId,
                    IntegrationTransactionLayout.AcknowledgementFileName)),
                "A dirty runtime unexpectedly published an Acknowledgement.");
        }

        Require(
            acknowledgementRejected
            && !File.Exists(GetTransactionMessagePath(
                runRoot,
                handoff.TransactionId,
                IntegrationTransactionLayout.AcknowledgementFileName)),
            "The dirty runtime Acknowledgement path did not fail closed.");

        try
        {
            _ = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.InvalidIdentity)
        {
            Require(
                !File.Exists(GetTransactionMessagePath(
                    runRoot,
                    handoff.TransactionId,
                    IntegrationTransactionLayout.ResultFileName)),
                "A dirty runtime unexpectedly published a Result.");
            string reportPath = Path.Combine(
                runRoot,
                "two-d-dirty-runtime-smoke.json");
            File.WriteAllText(
                reportPath,
                JsonSerializer.Serialize(
                    new
                    {
                        schemaVersion = "1.0",
                        runtimeSourceState = dirtyIdentity.SourceState.ToString(),
                        handoff.TransactionId,
                        acknowledgementErrorCode = IntegrationErrorCode.InvalidIdentity.ToString(),
                        runErrorCode = exception.ErrorCode.ToString(),
                        acknowledgementPublished = false,
                        resultPublished = false
                    },
                    new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(
                $"2D dirty runtime fail-closed smoke passed. Evidence={runRoot}");
            return 0;
        }

        throw new InvalidOperationException(
            "The dirty runtime Run path did not fail closed before Result publication.");
    }

    private static async Task<IntegrationResultV2> RunCaseAsync(
        string runRoot,
        string caseName,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath,
        IntegrationInspectionOutcome expectedOutcome)
    {
        var handoff = CreateFixture(
            runRoot,
            caseName,
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        var acknowledgement = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            handoff.TransactionId,
            runtimeBuildManifestPath);
        var result = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
            runRoot,
            handoff.TransactionId,
            runtimeBuildManifestPath);
        var persisted = TwoDIntegrationExchange.ReadResult(
            runRoot,
            handoff.TransactionId);

        Require(
            acknowledgement.Status == IntegrationAcknowledgementStatus.Accepted,
            $"2D {caseName} acknowledgement was not accepted.");
        Require(
            result.Outcome == expectedOutcome,
            $"2D {caseName} expected outcome {expectedOutcome}, got {result.Outcome}: {result.Error?.Message ?? "-"}.");
        Require(
            persisted.RunRecord is not null,
            $"2D {caseName} result did not reference a Run Record.");
        Require(
            !string.IsNullOrWhiteSpace(result.RunId),
            $"2D {caseName} result did not contain a Run ID.");
        Require(
            result.Correlation.InputKind == IntegrationInspectionInputKind.Image
                && result.Correlation.Modality == IntegrationInspectionModality.TwoD,
            $"2D {caseName} result correlation is not TwoD/Image.");

        ValidateRunRecordContract(
            runRoot,
            handoff.TransactionId,
            result,
            caseName);

        Console.WriteLine(
            $"2D {caseName}: outcome={result.Outcome}, runId={result.RunId}, metrics={result.Metrics.Count}");
        return result;
    }

    private static void RunMetricUnitContractCase(string runRoot)
    {
        var expectedUnits = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [VisionPipelineKnownMetrics.DistanceMmAvg] = TwoDIntegrationMetricUnits.Millimeter,
            [VisionPipelineKnownMetrics.AffineRotationDeg] = TwoDIntegrationMetricUnits.Degree,
            [VisionPipelineKnownMetrics.SourceImageWidth] = TwoDIntegrationMetricUnits.Pixel,
            [VisionPipelineKnownMetrics.MaskPixelRatio] = TwoDIntegrationMetricUnits.Fraction,
            [VisionPipelineKnownMetrics.AreaAvg] = TwoDIntegrationMetricUnits.PixelArea,
            [VisionPipelineKnownMetrics.UniqueMatchScoreMargin] = TwoDIntegrationMetricUnits.Fraction
        };
        foreach (var expected in expectedUnits)
        {
            Require(
                TwoDIntegrationMetricUnits.IsKnown(expected.Key)
                && string.Equals(
                    TwoDIntegrationMetricUnits.Resolve(expected.Key),
                    expected.Value,
                    StringComparison.Ordinal),
                $"Metric unit contract mismatch for {expected.Key}.");
        }

        const string unknownMetric = "FixtureMetricWithNoDeclaredUnit";
        Require(
            !TwoDIntegrationMetricUnits.IsKnown(unknownMetric)
            && string.Equals(
                TwoDIntegrationMetricUnits.Resolve(unknownMetric),
                TwoDIntegrationMetricUnits.Unknown,
                StringComparison.Ordinal),
            "An unknown metric was silently classified as a known unit.");

        using var syntheticRun = new VisionRecipeRunResult
        {
            TotalMilliseconds = 7D,
            Steps =
            [
                new VisionRecipeStepRunSummary
                {
                    Index = 1,
                    Metrics = new Dictionary<string, double>
                    {
                        [unknownMetric] = 42D
                    }
                }
            ]
        };
        var syntheticProjection = TwoDIntegrationExchange.CreateMetricProjection(syntheticRun);
        Require(
            syntheticProjection.Metrics.Any(metric =>
                string.Equals(metric.Name, $"step.1.{unknownMetric}", StringComparison.Ordinal)
                && metric.Value == 42D
                && string.Equals(metric.Unit, TwoDIntegrationMetricUnits.Unknown, StringComparison.Ordinal))
            && syntheticProjection.UnknownUnitDiagnostics.Count == 1,
            "Unknown metric projection did not retain the value and diagnostic.");

        var normalizedOverlayStep = new VisionRecipeStepRunSummary
        {
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "Normalized",
            Overlays = [new VisionRecipeOverlaySummary()]
        };
        var mergedOverlayStep = new VisionRecipeStepRunSummary
        {
            ToolType = "OverlayMerge",
            InputLayer = "Normalized",
            OutputLayer = "Merged",
            Overlays = [new VisionRecipeOverlaySummary()]
        };
        Require(
            string.Equals(
                TwoDIntegrationExchange.ResolveOverlayCoordinateLayer(normalizedOverlayStep),
                normalizedOverlayStep.OutputLayer,
                StringComparison.Ordinal)
            && string.Equals(
                TwoDIntegrationExchange.ResolveOverlayCoordinateLayer(mergedOverlayStep),
                "mixed",
                StringComparison.Ordinal),
            "Transform and merge overlay coordinate layers were not distinguished.");

        string reportPath = Path.Combine(runRoot, "two-d-metric-unit-contract.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    expectedUnits,
                    unknownMetric,
                    unknownUnit = TwoDIntegrationMetricUnits.Resolve(unknownMetric),
                    unknownDiagnosticRequired = true,
                    syntheticUnknownDiagnostics = syntheticProjection.UnknownUnitDiagnostics,
                    transformedOverlayCoordinateLayer = TwoDIntegrationExchange.ResolveOverlayCoordinateLayer(normalizedOverlayStep),
                    mergedOverlayCoordinateLayer = TwoDIntegrationExchange.ResolveOverlayCoordinateLayer(mergedOverlayStep)
                },
                new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"2D metric units: exact map and unknown diagnostic contract passed. Evidence={reportPath}");
    }

    private static void ValidateRunRecordContract(
        string runRoot,
        Guid transactionId,
        IntegrationResultV2 result,
        string caseName)
    {
        Require(
            result.RunRecord is not null,
            $"2D {caseName} result did not retain a Run Record reference.");
        string transactionDirectory = Path.Combine(
            runRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            transactionId.ToString("D"));
        string runRecordPath = Path.Combine(
            transactionDirectory,
            result.RunRecord!.RelativePath.Replace('/', Path.DirectorySeparatorChar));
        var runRecord = JsonSerializer.Deserialize<TwoDIntegrationRunRecord>(
            File.ReadAllText(runRecordPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Require(runRecord is not null, $"2D {caseName} Run Record could not round-trip through JSON.");

        var integrationMetrics = result.Metrics.ToDictionary(
            metric => metric.Name,
            StringComparer.OrdinalIgnoreCase);
        var unknownUnitResult = new IntegrationResultV2(
            result.SchemaVersion,
            result.MessageType,
            result.MessageId,
            result.TransactionId,
            result.HandoffMessageId,
            result.AcknowledgementMessageId,
            result.CreatedAtUtc,
            result.Producer,
            result.Status,
            result.Outcome,
            result.RunId,
            result.RunRecord,
            result.Correlation,
            result.Metrics
                .Concat(
                [new IntegrationMetric(
                    $"step.unknown.{caseName}",
                    42D,
                    TwoDIntegrationMetricUnits.Unknown)])
                .ToArray(),
            result.Evidence,
            result.Error);
        var unknownUnitValidation = IntegrationContractValidator.Validate(unknownUnitResult);
        Require(
            unknownUnitValidation.IsValid,
            $"2D {caseName} IntegrationMetric validator rejected the non-empty unknown unit.");
        int finiteMetricCount = 0;
        var steps = runRecord!.Steps ?? [];
        foreach (var step in steps)
        {
            Require(
                !string.IsNullOrWhiteSpace(step.InputLayer)
                && !string.IsNullOrWhiteSpace(step.OutputLayer),
                $"2D {caseName} step {step.Index} lost its input/output layer context.");
            if (step.Overlays.Count > 0)
            {
                Require(
                    !string.IsNullOrWhiteSpace(step.OverlayCoordinateLayer),
                    $"2D {caseName} step {step.Index} lost its overlay coordinate layer.");
            }

            foreach (var metric in step.Metrics)
            {
                if (!double.IsFinite(metric.Value))
                {
                    continue;
                }

                finiteMetricCount++;
                string name = $"step.{step.Index}.{metric.Key}";
                if (!integrationMetrics.TryGetValue(name, out var projected)
                    || projected is null)
                {
                    throw new InvalidOperationException(
                        $"2D {caseName} metric {name} was not projected into the Result.");
                }

                Require(
                    projected.Value == metric.Value,
                    $"2D {caseName} metric {name} changed value during integration projection.");
                Require(
                    string.Equals(
                        projected.Unit,
                        TwoDIntegrationMetricUnits.Resolve(metric.Key),
                        StringComparison.Ordinal),
                    $"2D {caseName} metric {name} has an unexpected unit '{projected.Unit}'.");
            }
        }

        if (!integrationMetrics.TryGetValue("totalMilliseconds", out var totalMilliseconds)
            || totalMilliseconds is null
            || totalMilliseconds.Value != runRecord.TotalMilliseconds
            || !string.Equals(
                totalMilliseconds.Unit,
                TwoDIntegrationMetricUnits.Millisecond,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"2D {caseName} totalMilliseconds did not preserve its value/unit contract.");
        }

        AssertMetricUnitIfPresent(result, ".DistanceMm", TwoDIntegrationMetricUnits.Millimeter, caseName);
        AssertMetricUnitIfPresent(result, ".Angle", TwoDIntegrationMetricUnits.Degree, caseName);
        AssertMetricUnitIfPresent(result, ".SourceImageWidth", TwoDIntegrationMetricUnits.Pixel, caseName);
        AssertMetricUnitIfPresent(result, ".Area", TwoDIntegrationMetricUnits.PixelArea, caseName);
        AssertMetricUnitIfPresent(result, ".BoundsWidthMin", TwoDIntegrationMetricUnits.Pixel, caseName);
        AssertMetricUnitIfPresent(result, ".BoundsWidthMm", TwoDIntegrationMetricUnits.Millimeter, caseName);

        string? directOverlayLayer = steps
            .Where(step => step.Overlays.Count > 0)
            .Where(step => !string.Equals(step.OverlayCoordinateLayer, "mixed", StringComparison.OrdinalIgnoreCase))
            .Select(step => step.OverlayCoordinateLayer)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(directOverlayLayer))
        {
            var directOverlayStep = steps.First(
                step => string.Equals(
                    step.OverlayCoordinateLayer,
                    directOverlayLayer,
                    StringComparison.OrdinalIgnoreCase));
            Require(
                string.Equals(
                    directOverlayStep.OverlayCoordinateLayer,
                    directOverlayStep.InputLayer,
                    StringComparison.OrdinalIgnoreCase),
                $"2D {caseName} direct overlay coordinates were not tied to the input layer.");
        }

        string evidencePath = Path.Combine(runRoot, $"two-d-{caseName}-metric-contract.json");
        File.WriteAllText(
            evidencePath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    caseName,
                    transactionId,
                    finiteMetricCount,
                    resultMetricCount = result.Metrics.Count,
                    unknownUnitValidatorAccepted = unknownUnitValidation.IsValid,
                    unknownUnitDiagnostics = runRecord.MetricUnitDiagnostics,
                    steps = steps.Select(step => new
                    {
                        step.Index,
                        step.ToolType,
                        step.InputLayer,
                        step.OutputLayer,
                        step.OverlayCoordinateLayer,
                        overlayCount = step.Overlays.Count
                    })
                },
                new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"2D {caseName} metric/coordinate round-trip: finiteMetrics={finiteMetricCount}, unknownDiagnostics={runRecord.MetricUnitDiagnostics.Count}, evidence={evidencePath}");
    }

    private static void AssertMetricUnitIfPresent(
        IntegrationResultV2 result,
        string nameSuffix,
        string expectedUnit,
        string caseName)
    {
        var matches = result.Metrics
            .Where(metric => metric.Name.EndsWith(nameSuffix, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        foreach (var metric in matches)
        {
            Require(
                string.Equals(metric.Unit, expectedUnit, StringComparison.Ordinal),
                $"2D {caseName} metric {metric.Name} expected unit '{expectedUnit}', got '{metric.Unit}'.");
        }
    }

    private static async Task RunConcurrentRunCaseAsync(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath)
    {
        var handoff = CreateFixture(
            runRoot,
            "concurrent-run",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        _ = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            handoff.TransactionId,
            runtimeBuildManifestPath);

        string transactionDirectory = Path.Combine(
            runRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            handoff.TransactionId.ToString("D"));
        string runLockPath = Path.Combine(transactionDirectory, ".2d-run.lock");
        Task<IntegrationResultV2> firstRun = Task.Run(() =>
            TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath));
        bool leaseObserved = await WaitForActiveRunLeaseAsync(runLockPath, firstRun);
        Task secondRun = Task.Run(() =>
            TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath));

        IntegrationResultV2 firstResult = await firstRun;
        IntegrationContractException? secondException = null;
        try
        {
            await secondRun;
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.InvalidState)
        {
            secondException = exception;
        }

        string resultPath = GetTransactionMessagePath(
            runRoot,
            handoff.TransactionId,
            IntegrationTransactionLayout.ResultFileName);
        var persisted = TwoDIntegrationExchange.ReadResult(
            runRoot,
            handoff.TransactionId);
        Require(
            leaseObserved,
            "The concurrent 2D Run smoke did not observe the transaction run lease while execution was active.");
        Require(
            firstResult.Status == IntegrationResultStatus.Completed
                && firstResult.Outcome == IntegrationInspectionOutcome.Pass,
            $"The first concurrent 2D Run did not complete as Pass: {firstResult.Status}/{firstResult.Outcome}.");
        Require(
            secondException is not null,
            "A concurrent 2D Run request was not rejected with InvalidState.");
        Require(
            File.Exists(resultPath)
                && persisted.RunId == firstResult.RunId
                && persisted.MessageId == firstResult.MessageId,
            "The concurrent 2D Run did not leave exactly one correlated Result.");

        bool completedRerequestRejected = false;
        try
        {
            _ = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.InvalidState)
        {
            completedRerequestRejected = true;
        }

        Require(
            completedRerequestRejected,
            "A completed 2D transaction was re-executed instead of being rejected.");
        string reportPath = Path.Combine(runRoot, "two-d-concurrent-run-smoke.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    handoff.TransactionId,
                    leaseObserved,
                    firstRunStatus = firstResult.Status.ToString(),
                    firstRunOutcome = firstResult.Outcome.ToString(),
                    firstRunId = firstResult.RunId,
                    secondRunErrorCode = secondException!.ErrorCode.ToString(),
                    persistedResultCount = File.Exists(resultPath) ? 1 : 0,
                    completedRerequestRejected
                },
                new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(
            $"2D concurrent Run: leaseObserved={leaseObserved}, first={firstResult.Outcome}, second={secondException.ErrorCode}, rerequestRejected={completedRerequestRejected}");
        Console.WriteLine($"2D concurrent Run evidence={reportPath}");
    }

    private static async Task<bool> WaitForActiveRunLeaseAsync(
        string runLockPath,
        Task firstRun)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(10);
        while (!firstRun.IsCompleted && DateTime.UtcNow < deadline)
        {
            if (File.Exists(runLockPath))
            {
                try
                {
                    using var probe = new FileStream(
                        runLockPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.None,
                        bufferSize: 1,
                        options: FileOptions.SequentialScan);
                }
                catch (IOException)
                {
                    return true;
                }
            }

            await Task.Delay(1);
        }

        return false;
    }

    private static async Task RunRejectedCaseAsync(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath)
    {
        var handoff = CreateFixture(
            runRoot,
            "rejected",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        var acknowledgement = TwoDIntegrationExchange.RejectHandoff(
            runRoot,
            handoff.TransactionId,
            "The recipe is not supported by this 2D consumer.",
            runtimeBuildManifestPath);
        var persisted = TwoDIntegrationExchange.ReadAcknowledgement(
            runRoot,
            handoff.TransactionId);

        Require(
            acknowledgement.Status == IntegrationAcknowledgementStatus.Rejected
                && persisted.Status == IntegrationAcknowledgementStatus.Rejected,
            "2D rejected acknowledgement was not persisted as Rejected.");

        try
        {
            _ = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.InvalidState)
        {
            string resultPath = Path.Combine(
                runRoot,
                IntegrationTransactionLayout.TransactionsDirectoryName,
                handoff.TransactionId.ToString("D"),
                IntegrationTransactionLayout.ResultFileName);
            Require(
                !File.Exists(resultPath),
                "A rejected 2D Handoff unexpectedly published a Result.");
            Console.WriteLine(
                "2D rejected: execution blocked after explicit Rejected acknowledgement.");
            return;
        }

        throw new InvalidOperationException(
            "A rejected 2D Handoff was executed unexpectedly.");
    }

    private static void RunRuntimeManifestIntegrityNegativeCases(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath)
    {
        string missingManifestPath = Path.Combine(
            runRoot,
            "missing-openvisionlab.runtime.json");
        var missingManifestHandoff = CreateFixture(
            runRoot,
            "runtime-manifest-missing",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        RequireAcknowledgementFailure(
            runRoot,
            missingManifestHandoff,
            missingManifestPath,
            IntegrationErrorCode.ArtifactMissing,
            "missing runtime manifest");

        var validManifest = IntegrationContractJson.DeserializeRuntimeBuildManifest(
            File.ReadAllBytes(runtimeBuildManifestPath));
        string changedCommit = new(
            consumer.SourceCommit[0] == '0' ? '1' : '0',
            consumer.SourceCommit.Length);
        string identityTamperPath = Path.Combine(
            runRoot,
            "identity-tampered-openvisionlab.runtime.json");
        File.WriteAllBytes(
            identityTamperPath,
            IntegrationContractJson.SerializeCanonical(
                validManifest with
                {
                    Identity = validManifest.Identity with
                    {
                        SourceCommit = changedCommit
                    }
                }));
        var identityTamperHandoff = CreateFixture(
            runRoot,
            "runtime-identity-tamper",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        RequireAcknowledgementFailure(
            runRoot,
            identityTamperHandoff,
            identityTamperPath,
            IntegrationErrorCode.InvalidIdentity,
            "runtime manifest identity tamper");

        string hashTamperPath = Path.Combine(
            runRoot,
            "hash-tampered-openvisionlab.runtime.json");
        File.WriteAllBytes(
            hashTamperPath,
            IntegrationContractJson.SerializeCanonical(
                validManifest with
                {
                    EntryAssembly = validManifest.EntryAssembly with
                    {
                        Sha256 = new string('0', 64)
                    }
                }));
        var hashTamperHandoff = CreateFixture(
            runRoot,
            "runtime-hash-tamper",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        RequireAcknowledgementFailure(
            runRoot,
            hashTamperHandoff,
            hashTamperPath,
            IntegrationErrorCode.ArtifactHashMismatch,
            "runtime manifest assembly hash tamper");
    }

    private static async Task RunQualifiedRuntimeIdentityNegativeCasesAsync(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath)
    {
        string missingManifestPath = Path.Combine(
            runRoot,
            "missing-openvisionlab.runtime.json");
        var mismatchedConsumer = consumer with
        {
            ApplicationVersion = consumer.ApplicationVersion + "-mismatch"
        };
        var targetMismatchHandoff = CreateFixture(
            runRoot,
            "runtime-target-mismatch",
            imagePath,
            recipePath,
            mismatchedConsumer,
            tamperSourceAfterPublish: false);
        RequireAcknowledgementFailure(
            runRoot,
            targetMismatchHandoff,
            runtimeBuildManifestPath,
            IntegrationErrorCode.CorrelationMismatch,
            "Handoff target mismatch");

        var runRecheckHandoff = CreateFixture(
            runRoot,
            "runtime-run-recheck",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: false);
        _ = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            runRecheckHandoff.TransactionId,
            runtimeBuildManifestPath);
        try
        {
            _ = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                runRoot,
                runRecheckHandoff.TransactionId,
                missingManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.ArtifactMissing)
        {
            Require(
                !File.Exists(GetTransactionMessagePath(
                    runRoot,
                    runRecheckHandoff.TransactionId,
                    IntegrationTransactionLayout.ResultFileName)),
                "A Run with a missing runtime manifest unexpectedly published a Result.");
            Console.WriteLine(
                "2D runtime identity: Run revalidation rejected a missing manifest before Result publication.");
            return;
        }

        throw new InvalidOperationException(
            "A Run with a missing runtime manifest was not rejected.");
    }

    private static void RequireAcknowledgementFailure(
        string runRoot,
        IntegrationHandoffV2 handoff,
        string runtimeBuildManifestPath,
        IntegrationErrorCode expectedErrorCode,
        string caseName)
    {
        try
        {
            _ = TwoDIntegrationExchange.AcknowledgeHandoff(
                runRoot,
                handoff.TransactionId,
                runtimeBuildManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == expectedErrorCode)
        {
            Require(
                !File.Exists(GetTransactionMessagePath(
                    runRoot,
                    handoff.TransactionId,
                    IntegrationTransactionLayout.AcknowledgementFileName)),
                $"The {caseName} case unexpectedly published an Acknowledgement.");
            Console.WriteLine(
                $"2D runtime identity: {caseName} rejected with {expectedErrorCode} before Acknowledgement publication.");
            return;
        }

        throw new InvalidOperationException(
            $"The {caseName} case was not rejected with {expectedErrorCode}.");
    }

    private static string GetTransactionMessagePath(
        string runRoot,
        Guid transactionId,
        string fileName) =>
        Path.Combine(
            runRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            transactionId.ToString("D"),
            fileName);

    private static string GetTransactionDirectory(
        string runRoot,
        Guid transactionId) =>
        Path.Combine(
            runRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            transactionId.ToString("D"));

    private static string ComputeSha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static void RunTamperCase(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer)
    {
        var handoff = CreateFixture(
            runRoot,
            "tamper",
            imagePath,
            recipePath,
            consumer,
            tamperSourceAfterPublish: true);
        try
        {
            _ = TwoDIntegrationExchange.ReadHandoff(
                runRoot,
                handoff.TransactionId);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode is IntegrationErrorCode.ArtifactLengthMismatch
                or IntegrationErrorCode.ArtifactHashMismatch)
        {
            Console.WriteLine(
                $"2D tamper: {exception.ErrorCode} rejected before acknowledgement/run.");
            return;
        }

        throw new InvalidOperationException(
            "2D tamper case was not rejected by artifact identity validation.");
    }

    private static IntegrationHandoffV2 CreateFixture(
        string runRoot,
        string caseName,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        bool tamperSourceAfterPublish,
        IntegrationInspectionModality modality = IntegrationInspectionModality.TwoD,
        IntegrationInspectionInputKind inputKind = IntegrationInspectionInputKind.Image)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("2D smoke image was not found.", imagePath);
        }
        if (!File.Exists(recipePath))
        {
            throw new FileNotFoundException("2D smoke recipe was not found.", recipePath);
        }

        var transactionId = Guid.NewGuid();
        string transactionDirectory = Path.Combine(
            runRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            transactionId.ToString("D"));
        string artifactsDirectory = Path.Combine(
            transactionDirectory,
            IntegrationTransactionLayout.ArtifactsDirectoryName);
        Directory.CreateDirectory(artifactsDirectory);

        var machineProject = WriteArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.MachineProject,
            "machine-project",
            "{\"schema\":\"machine-project/1.0\",\"fixture\":true}",
            "artifacts/machine-project.json");
        var source = CopyArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.InspectionSource,
            $"source-{caseName}",
            imagePath,
            "artifacts/source.png");
        var recipe = CopyArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.InspectionRecipe,
            $"recipe-{caseName}",
            recipePath,
            "artifacts/recipe.xml");

        if (tamperSourceAfterPublish)
        {
            File.AppendAllText(
                Path.Combine(transactionDirectory, source.RelativePath.Replace('/', Path.DirectorySeparatorChar)),
                "tampered");
        }

        var context = new IntegrationInspectionContextV2(
            "machine-fixture",
            "machine-project/1.0",
            "sequence-001",
            "inspect-image",
            "camera-virtual",
            $"acquisition-{caseName}",
            $"frame-{caseName}",
            "px",
            modality,
            inputKind,
            source.Sha256,
            recipe.Sha256,
            consumer,
            [machineProject, source, recipe]);
        var handoff = new IntegrationHandoffV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Handoff,
            Guid.NewGuid(),
            transactionId,
            DateTimeOffset.UtcNow,
            new IntegrationApplicationIdentity(
                IntegrationApplicationIds.MachineStudio,
                "1.4.0",
                ProducerCommit,
                IntegrationSourceState.Clean),
            context);
        File.WriteAllBytes(
            Path.Combine(transactionDirectory, IntegrationTransactionLayout.HandoffFileName),
            IntegrationContractJson.SerializeCanonical(handoff));
        return handoff;
    }

    private static IntegrationArtifactReference CopyArtifact(
        string transactionDirectory,
        string role,
        string artifactId,
        string sourcePath,
        string relativePath)
    {
        string targetPath = Path.Combine(
            transactionDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.Copy(sourcePath, targetPath, overwrite: false);
        return CreateArtifactReference(role, artifactId, targetPath, relativePath);
    }

    private static IntegrationArtifactReference WriteArtifact(
        string transactionDirectory,
        string role,
        string artifactId,
        string content,
        string relativePath)
    {
        string targetPath = Path.Combine(
            transactionDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.WriteAllText(targetPath, content, Encoding.UTF8);
        return CreateArtifactReference(role, artifactId, targetPath, relativePath);
    }

    private static IntegrationArtifactReference CreateArtifactReference(
        string role,
        string artifactId,
        string fullPath,
        string relativePath)
    {
        using var stream = File.OpenRead(fullPath);
        return new(
            role,
            artifactId,
            relativePath,
            stream.Length,
            Convert.ToHexString(SHA256.HashData(stream)));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
