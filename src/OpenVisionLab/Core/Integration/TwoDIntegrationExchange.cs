#nullable enable annotations

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenVisionLab.Integration.Contracts;

namespace OpenVisionLab.Core.Integration;

public sealed record TwoDIntegrationTransactionSummary(
    IntegrationHandoffV2 Handoff,
    bool HasAcknowledgement,
    bool HasResult);

public sealed record TwoDIntegrationRunRecord(
    string SchemaVersion,
    string RunId,
    DateTimeOffset RecordedAtUtc,
    string SourceRelativePath,
    string SourceSha256,
    long SourceByteLength,
    string RecipeRelativePath,
    string RecipeSha256,
    string Outcome,
    string Message,
    double TotalMilliseconds,
    IReadOnlyList<TwoDIntegrationStepRecord> Steps)
{
    /// <summary>
    /// Source dimensions are retained so another consumer can interpret the
    /// persisted overlay coordinates without reopening the source image.
    /// </summary>
    public int SourceImageWidth { get; init; }
    public int SourceImageHeight { get; init; }

    /// <summary>
    /// Unknown metric names are retained with an explicit unit diagnostic
    /// instead of being silently projected to the dimensionless unit.
    /// </summary>
    public IReadOnlyList<string> MetricUnitDiagnostics { get; init; } = [];
}

public sealed record TwoDIntegrationStepRecord(
    int Index,
    string Name,
    string ToolType,
    string Status,
    bool ToolSuccess,
    bool AcceptancePassed,
    string Message,
    double ElapsedMilliseconds,
    IReadOnlyDictionary<string, double> Metrics)
{
    /// <summary>
    /// Runtime geometry is preserved separately from metrics so a paired 3D
    /// consumer can project the actual detected points.
    /// </summary>
    public IReadOnlyList<TwoDIntegrationOverlayRecord> Overlays { get; init; } = [];

    /// <summary>
    /// The layers used by the step are persisted with the geometry. A
    /// consumer must not assume that an overlay belongs to the source image.
    /// </summary>
    public string InputLayer { get; init; } = string.Empty;
    public string OutputLayer { get; init; } = string.Empty;
    public string OverlayCoordinateLayer { get; init; } = string.Empty;
}

public sealed record TwoDIntegrationOverlayRecord(
    string Kind,
    string Label,
    double BoundsX,
    double BoundsY,
    double BoundsWidth,
    double BoundsHeight,
    double CenterX,
    double CenterY,
    double StartX,
    double StartY,
    double EndX,
    double EndY,
    double Angle,
    int PointCount,
    IReadOnlyList<TwoDIntegrationOverlayPoint> Points);

public sealed record TwoDIntegrationOverlayPoint(double X, double Y);

internal readonly record struct TwoDIntegrationRunDisposition(
    IntegrationResultStatus Status,
    IntegrationInspectionOutcome Outcome,
    IntegrationError Error);

/// <summary>
/// Explicit 2D consumer adapter for the v2 file exchange. Reading and
/// acknowledgement validate only; an inspection starts only through the
/// explicit RunAcceptedHandoffAsync call.
/// </summary>
public static class TwoDIntegrationExchange
{
    private const string RunRecordFileName = "2d-run-record.json";
    private const string RunLockFileName = ".2d-run.lock";
    private const string MixedOverlayCoordinateLayer = "mixed";
    private const string UnknownOverlayCoordinateLayer = "unknown";
    private static readonly JsonSerializerOptions RunRecordJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static IReadOnlyList<TwoDIntegrationTransactionSummary> DiscoverHandoffs(
        string exchangeRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exchangeRoot);
        var transactionsRoot = Path.Combine(
            Path.GetFullPath(exchangeRoot),
            IntegrationTransactionLayout.TransactionsDirectoryName);
        if (!Directory.Exists(transactionsRoot))
        {
            return [];
        }

        var transactions = new List<TwoDIntegrationTransactionSummary>();
        foreach (var directory in Directory.EnumerateDirectories(transactionsRoot))
        {
            if (!Guid.TryParse(Path.GetFileName(directory), out var transactionId))
            {
                continue;
            }

            var handoffPath = Path.Combine(
                directory,
                IntegrationTransactionLayout.HandoffFileName);
            if (!File.Exists(handoffPath))
            {
                continue;
            }

            var handoff = ReadHandoffEnvelope(exchangeRoot, transactionId);
            transactions.Add(new(
                handoff,
                File.Exists(Path.Combine(
                    directory,
                    IntegrationTransactionLayout.AcknowledgementFileName)),
                File.Exists(Path.Combine(
                    directory,
                    IntegrationTransactionLayout.ResultFileName))));
        }

        return transactions
            .OrderByDescending(transaction => transaction.Handoff.CreatedAtUtc)
            .ToArray();
    }

    public static IntegrationHandoffV2 ReadHandoff(
        string exchangeRoot,
        Guid transactionId)
    {
        var handoff = ReadHandoffEnvelope(exchangeRoot, transactionId);
        ValidateTwoDConsumer(handoff);
        var transactionDirectory = GetTransactionDirectory(exchangeRoot, transactionId);
        foreach (var artifact in handoff.Context.Artifacts)
        {
            EnsureNoReparsePoints(transactionDirectory, artifact.RelativePath);
            ThrowIfInvalid(IntegrationContractValidator.ValidateArtifactFile(
                artifact,
                transactionDirectory));
        }

        RequireContextArtifact(handoff, IntegrationArtifactRoles.InspectionSource);
        RequireContextArtifact(handoff, IntegrationArtifactRoles.InspectionRecipe);
        return handoff;
    }

    public static IntegrationHandoffV2 ReadHandoffEnvelope(
        string exchangeRoot,
        Guid transactionId)
    {
        var transactionDirectory = GetTransactionDirectory(exchangeRoot, transactionId);
        var handoff = IntegrationContractJson.DeserializeHandoffV2(
            ReadMessage(
                transactionDirectory,
                IntegrationTransactionLayout.HandoffFileName));
        if (handoff.TransactionId != transactionId)
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.CorrelationMismatch,
                "Handoff transaction identity does not match its directory.");
        }

        return handoff;
    }

    public static IntegrationAcknowledgementV2 AcknowledgeHandoff(
        string exchangeRoot,
        Guid transactionId) =>
        PublishAcknowledgement(
            exchangeRoot,
            transactionId,
            rejectionReason: null,
            runtimeBuildManifestPath: null);

    internal static IntegrationAcknowledgementV2 AcknowledgeHandoff(
        string exchangeRoot,
        Guid transactionId,
        string runtimeBuildManifestPath) =>
        PublishAcknowledgement(
            exchangeRoot,
            transactionId,
            rejectionReason: null,
            runtimeBuildManifestPath: runtimeBuildManifestPath);

    public static IntegrationAcknowledgementV2 RejectHandoff(
        string exchangeRoot,
        Guid transactionId,
        string rejectionReason) =>
        PublishAcknowledgement(
            exchangeRoot,
            transactionId,
            rejectionReason,
            runtimeBuildManifestPath: null);

    internal static IntegrationAcknowledgementV2 RejectHandoff(
        string exchangeRoot,
        Guid transactionId,
        string rejectionReason,
        string runtimeBuildManifestPath) =>
        PublishAcknowledgement(
            exchangeRoot,
            transactionId,
            rejectionReason,
            runtimeBuildManifestPath);

    private static IntegrationAcknowledgementV2 PublishAcknowledgement(
        string exchangeRoot,
        Guid transactionId,
        string? rejectionReason,
        string? runtimeBuildManifestPath)
    {
        if (rejectionReason is not null && string.IsNullOrWhiteSpace(rejectionReason))
        {
            throw new ArgumentException(
                "Rejection reason cannot be blank.",
                nameof(rejectionReason));
        }

        var handoff = rejectionReason is null
            ? ReadHandoff(exchangeRoot, transactionId)
            : ReadHandoffEnvelope(exchangeRoot, transactionId);
        ValidateTwoDConsumer(handoff);
        var consumerBuild = TwoDIntegrationBuildIdentity.LoadQualifiedTargetIdentity(
            handoff.Context.ConsumerBuild,
            runtimeBuildManifestPath);
        var transactionDirectory = GetTransactionDirectory(exchangeRoot, transactionId);
        var acknowledgementPath = Path.Combine(
            transactionDirectory,
            IntegrationTransactionLayout.AcknowledgementFileName);
        if (File.Exists(acknowledgementPath))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidState,
                "The Handoff already has an Acknowledgement.");
        }

        var acknowledgement = new IntegrationAcknowledgementV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Acknowledgement,
            Guid.NewGuid(),
            handoff.TransactionId,
            handoff.MessageId,
            NotBefore(handoff.CreatedAtUtc),
            consumerBuild,
            rejectionReason is null
                ? IntegrationAcknowledgementStatus.Accepted
                : IntegrationAcknowledgementStatus.Rejected,
            rejectionReason is null
                ? null
                : new IntegrationError(
                    IntegrationErrorCode.RequestRejected,
                    rejectionReason,
                    false));
        ThrowIfInvalid(IntegrationContractValidator.ValidateV2Sequence(
            handoff,
            acknowledgement));
        WriteNewMessage(
            transactionDirectory,
            IntegrationTransactionLayout.AcknowledgementFileName,
            IntegrationContractJson.SerializeCanonical(acknowledgement));
        return acknowledgement;
    }

    public static IntegrationAcknowledgementV2 ReadAcknowledgement(
        string exchangeRoot,
        Guid transactionId)
    {
        var handoff = ReadHandoff(exchangeRoot, transactionId);
        var acknowledgement = IntegrationContractJson.DeserializeAcknowledgementV2(
            ReadMessage(
                GetTransactionDirectory(exchangeRoot, transactionId),
                IntegrationTransactionLayout.AcknowledgementFileName));
        ThrowIfInvalid(IntegrationContractValidator.ValidateV2Sequence(
            handoff,
            acknowledgement));
        return acknowledgement;
    }

    public static IntegrationResultV2 ReadResult(
        string exchangeRoot,
        Guid transactionId)
    {
        var handoff = ReadHandoff(exchangeRoot, transactionId);
        var transactionDirectory = GetTransactionDirectory(exchangeRoot, transactionId);
        var acknowledgement = IntegrationContractJson.DeserializeAcknowledgementV2(
            ReadMessage(
                transactionDirectory,
                IntegrationTransactionLayout.AcknowledgementFileName));
        var result = IntegrationContractJson.DeserializeResultV2(
            ReadMessage(
                transactionDirectory,
                IntegrationTransactionLayout.ResultFileName));
        ThrowIfInvalid(IntegrationContractValidator.ValidateV2Sequence(
            handoff,
            acknowledgement,
            result));

        if (result.RunRecord is not null)
        {
            EnsureNoReparsePoints(transactionDirectory, result.RunRecord.RelativePath);
            ThrowIfInvalid(IntegrationContractValidator.ValidateArtifactFile(
                result.RunRecord,
                transactionDirectory));
        }
        foreach (var evidence in result.Evidence)
        {
            EnsureNoReparsePoints(transactionDirectory, evidence.RelativePath);
            ThrowIfInvalid(IntegrationContractValidator.ValidateArtifactFile(
                evidence,
                transactionDirectory));
        }

        return result;
    }

    public static Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        string exchangeRoot,
        Guid transactionId,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default) =>
        RunAcceptedHandoffCoreAsync(
            exchangeRoot,
            transactionId,
            runtimeBuildManifestPath: null,
            stepTimeoutMilliseconds: stepTimeoutMilliseconds,
            cancellationToken: cancellationToken);

    internal static Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        string exchangeRoot,
        Guid transactionId,
        string runtimeBuildManifestPath,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default) =>
        RunAcceptedHandoffCoreAsync(
            exchangeRoot,
            transactionId,
            runtimeBuildManifestPath,
            stepTimeoutMilliseconds,
            cancellationToken);

    private static async Task<IntegrationResultV2> RunAcceptedHandoffCoreAsync(
        string exchangeRoot,
        Guid transactionId,
        string? runtimeBuildManifestPath,
        int stepTimeoutMilliseconds,
        CancellationToken cancellationToken)
    {
        if (stepTimeoutMilliseconds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stepTimeoutMilliseconds),
                "Step timeout must be greater than zero.");
        }

        var handoff = ReadHandoff(exchangeRoot, transactionId);
        ValidateTwoDConsumer(handoff);
        var consumerBuild = TwoDIntegrationBuildIdentity.LoadQualifiedTargetIdentity(
            handoff.Context.ConsumerBuild,
            runtimeBuildManifestPath);
        var acknowledgement = ReadAcknowledgement(exchangeRoot, transactionId);
        if (acknowledgement.Status != IntegrationAcknowledgementStatus.Accepted)
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidState,
                "A completed 2D inspection requires an accepted Acknowledgement.");
        }

        var transactionDirectory = GetTransactionDirectory(exchangeRoot, transactionId);
        using var runLease = AcquireRunLease(transactionDirectory);
        var resultPath = Path.Combine(
            transactionDirectory,
            IntegrationTransactionLayout.ResultFileName);
        if (File.Exists(resultPath))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidState,
                "The Handoff already has a Result.");
        }

        var sourceArtifact = RequireContextArtifact(
            handoff,
            IntegrationArtifactRoles.InspectionSource);
        var recipeArtifact = RequireContextArtifact(
            handoff,
            IntegrationArtifactRoles.InspectionRecipe);
        var sourcePath = ResolveArtifactPath(transactionDirectory, sourceArtifact);
        var recipePath = ResolveArtifactPath(transactionDirectory, recipeArtifact);
        string locatorEvidenceDirectory = string.Empty;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var source = Cv2.ImRead(sourcePath, ImreadModes.Unchanged);
            if (source.Empty())
            {
                throw new InvalidOperationException(
                    $"The inspection source could not be decoded: {sourceArtifact.RelativePath}");
            }

            var locatorPlan = TwoDIntegrationLocatorRecipeContract.IsLocatorRecipe(recipePath)
                ? TwoDIntegrationLocatorRecipeContract.CreatePlan(
                    recipePath,
                    ResolveArtifactPath(
                        transactionDirectory,
                        RequireLocatorTemplateArtifact(handoff)))
                : null;
            var locatorPipeline = locatorPlan is null
                ? null
                : OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(locatorPlan);
            using var run = locatorPipeline is null
                ? await new VisionRecipeRunner().RunAsync(
                        recipePath,
                        source,
                        VisionRecipeRunner.DefaultInputLayer,
                        stepTimeoutMilliseconds,
                        cancellationToken)
                    .ConfigureAwait(false)
                : await new VisionRecipeRunner().RunAsync(
                        locatorPipeline,
                        source,
                        VisionRecipeRunner.DefaultInputLayer,
                        stepTimeoutMilliseconds,
                        cancellationToken)
                    .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            TwoDIntegrationRunDisposition disposition = ClassifyRunResult(run);
            if (disposition.Status != IntegrationResultStatus.Completed)
            {
                return PublishFailedResult(
                    transactionDirectory,
                    handoff,
                    acknowledgement,
                    consumerBuild,
                    disposition.Status,
                    disposition.Outcome,
                    disposition.Error);
            }

            var runId = CreateRunId(handoff);
            var metricProjection = CreateMetricProjection(run);
            var runRecord = CreateRunRecord(
                runId,
                handoff,
                sourceArtifact,
                recipeArtifact,
                run,
                source.Width,
                source.Height,
                metricProjection.UnknownUnitDiagnostics);
            IReadOnlyList<IntegrationArtifactReference> evidence = [];
            if (locatorPlan is not null)
            {
                locatorEvidenceDirectory = Path.Combine(
                    transactionDirectory,
                    IntegrationTransactionLayout.ArtifactsDirectoryName,
                    "locator");
                if (run.PipelineRunResult is null
                    || locatorPipeline is null)
                {
                    throw new InvalidOperationException(
                        "The locator runtime result was not retained for evidence export.");
                }

                if (!OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport(
                        locatorPlan,
                        locatorPipeline,
                        run.PipelineRunResult,
                        source,
                        sourcePath,
                        locatorEvidenceDirectory,
                        consumerBuild.ApplicationVersion,
                        out _,
                        out var packetPath,
                        out var overlayPath,
                        out var exportMessage))
                {
                    throw new InvalidOperationException(
                        $"The locator evidence packet was not published: {exportMessage}");
                }

                evidence =
                [
                    CreateArtifactReference(
                        TwoDIntegrationLocatorRecipeContract.EvidenceRole,
                        TwoDIntegrationLocatorRecipeContract.EvidenceArtifactId,
                        packetPath,
                        $"{IntegrationTransactionLayout.ArtifactsDirectoryName}/locator/evidence.packet.json"),
                    CreateArtifactReference(
                        TwoDIntegrationLocatorRecipeContract.OverlayRole,
                        TwoDIntegrationLocatorRecipeContract.OverlayArtifactId,
                        overlayPath,
                        $"{IntegrationTransactionLayout.ArtifactsDirectoryName}/locator/locator-runtime-overlay.png")
                ];
            }

            var runRecordPath = WriteRunRecord(
                transactionDirectory,
                runRecord);
            try
            {
                var runRecordReference = CreateArtifactReference(
                    IntegrationArtifactRoles.RunRecord,
                    runId,
                    runRecordPath,
                    $"{IntegrationTransactionLayout.ArtifactsDirectoryName}/{RunRecordFileName}");
                var result = new IntegrationResultV2(
                    IntegrationContractSchema.V2,
                    IntegrationMessageKind.Result,
                    Guid.NewGuid(),
                    handoff.TransactionId,
                    handoff.MessageId,
                    acknowledgement.MessageId,
                    NotBefore(acknowledgement.CreatedAtUtc),
                    consumerBuild,
                    disposition.Status,
                    disposition.Outcome,
                    runId,
                    runRecordReference,
                    IntegrationRunCorrelation.FromContext(handoff.Context),
                    metricProjection.Metrics,
                    evidence,
                    disposition.Error);
                ThrowIfInvalid(IntegrationContractValidator.ValidateV2Sequence(
                    handoff,
                    acknowledgement,
                    result));
                WriteNewMessage(
                    transactionDirectory,
                    IntegrationTransactionLayout.ResultFileName,
                    IntegrationContractJson.SerializeCanonical(result));
                return result;
            }
            catch
            {
                TryDeleteFile(runRecordPath);
                throw;
            }
        }
        catch (OperationCanceledException)
        {
            TryDeleteLocatorArtifacts(locatorEvidenceDirectory);
            return PublishFailedResult(
                transactionDirectory,
                handoff,
                acknowledgement,
                consumerBuild,
                IntegrationResultStatus.Cancelled,
                IntegrationInspectionOutcome.Indeterminate,
                new IntegrationError(
                    IntegrationErrorCode.Cancelled,
                    "The 2D inspection was cancelled before a completed Run Record was published.",
                    true));
        }
        catch (Exception exception)
        {
            TryDeleteLocatorArtifacts(locatorEvidenceDirectory);
            return PublishFailedResult(
                transactionDirectory,
                handoff,
                acknowledgement,
                consumerBuild,
                IntegrationResultStatus.Failed,
                IntegrationInspectionOutcome.ExecutionError,
                new IntegrationError(
                    IntegrationErrorCode.ExecutionFailed,
                    exception.Message,
                    false));
        }
    }

    internal static TwoDIntegrationRunDisposition ClassifyRunResult(
        VisionRecipeRunResult run)
    {
        if (run == null)
        {
            return ExecutionFailure("The 2D inspection returned no run result.");
        }

        VisionRecipeStepRunSummary failedStep = run.FirstFailedStep;
        if (failedStep == null)
        {
            bool hasExecutedStep = run.Steps?.Any(step => step != null && !step.Skipped) == true;
            bool allStepsPassed = hasExecutedStep
                && run.Steps.All(step => step != null
                    && (step.Skipped || string.Equals(step.Status, "OK", StringComparison.OrdinalIgnoreCase)));
            return run.Success && allStepsPassed
                ? new TwoDIntegrationRunDisposition(
                    IntegrationResultStatus.Completed,
                    IntegrationInspectionOutcome.Pass,
                    null)
                : ExecutionFailure("The 2D inspection did not produce a completed step result.");
        }

        string status = failedStep.Status?.Trim() ?? string.Empty;
        if (string.Equals(status, "NG", StringComparison.OrdinalIgnoreCase)
            && failedStep.ToolSuccess
            && !failedStep.AcceptancePassed)
        {
            return new TwoDIntegrationRunDisposition(
                IntegrationResultStatus.Completed,
                IntegrationInspectionOutcome.Ng,
                null);
        }

        if (string.Equals(status, "CANCEL", StringComparison.OrdinalIgnoreCase))
        {
            return new TwoDIntegrationRunDisposition(
                IntegrationResultStatus.Cancelled,
                IntegrationInspectionOutcome.Indeterminate,
                new IntegrationError(
                    IntegrationErrorCode.Cancelled,
                    BuildStepFailureMessage(failedStep),
                    true));
        }

        return ExecutionFailure(BuildStepFailureMessage(failedStep));
    }

    private static TwoDIntegrationRunDisposition ExecutionFailure(string message) =>
        new(
            IntegrationResultStatus.Failed,
            IntegrationInspectionOutcome.ExecutionError,
            new IntegrationError(
                IntegrationErrorCode.ExecutionFailed,
                message,
                false));

    private static string BuildStepFailureMessage(VisionRecipeStepRunSummary step)
    {
        if (step == null)
        {
            return "The 2D inspection failed before a step result was available.";
        }

        string stepName = string.IsNullOrWhiteSpace(step.Name) ? "Step" : step.Name;
        string status = string.IsNullOrWhiteSpace(step.Status) ? "UNKNOWN" : step.Status;
        string error = step.ErrorCode == 0 && string.IsNullOrWhiteSpace(step.ErrorName)
            ? string.Empty
            : $" Error={step.ErrorCode}:{step.ErrorName}.";
        string detail = string.IsNullOrWhiteSpace(step.Message) ? string.Empty : $" {step.Message}";
        return $"{stepName} returned {status}.{error}{detail}".Trim();
    }

    private static IntegrationResultV2 PublishFailedResult(
        string transactionDirectory,
        IntegrationHandoffV2 handoff,
        IntegrationAcknowledgementV2 acknowledgement,
        IntegrationApplicationIdentity consumerBuild,
        IntegrationResultStatus status,
        IntegrationInspectionOutcome outcome,
        IntegrationError error)
    {
        var result = new IntegrationResultV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Result,
            Guid.NewGuid(),
            handoff.TransactionId,
            handoff.MessageId,
            acknowledgement.MessageId,
            NotBefore(acknowledgement.CreatedAtUtc),
            consumerBuild,
            status,
            outcome,
            null,
            null,
            IntegrationRunCorrelation.FromContext(handoff.Context),
            [],
            [],
            error);
        ThrowIfInvalid(IntegrationContractValidator.ValidateV2Sequence(
            handoff,
            acknowledgement,
            result));
        WriteNewMessage(
            transactionDirectory,
            IntegrationTransactionLayout.ResultFileName,
            IntegrationContractJson.SerializeCanonical(result));
        return result;
    }

    internal sealed record IntegrationMetricProjection(
        IReadOnlyList<IntegrationMetric> Metrics,
        IReadOnlyList<string> UnknownUnitDiagnostics);

    private static TwoDIntegrationRunRecord CreateRunRecord(
        string runId,
        IntegrationHandoffV2 handoff,
        IntegrationArtifactReference sourceArtifact,
        IntegrationArtifactReference recipeArtifact,
        VisionRecipeRunResult run,
        int sourceImageWidth,
        int sourceImageHeight,
        IReadOnlyList<string> metricUnitDiagnostics) =>
        new(
            "1.1",
            runId,
            DateTimeOffset.UtcNow,
            sourceArtifact.RelativePath,
            sourceArtifact.Sha256,
            sourceArtifact.ByteLength,
            recipeArtifact.RelativePath,
            recipeArtifact.Sha256,
            run.Success ? "Pass" : "Fail",
            string.IsNullOrWhiteSpace(run.Message)
                ? run.SummaryText
                : run.Message,
            run.TotalMilliseconds,
            (run.Steps ?? [])
                .Select(step => new TwoDIntegrationStepRecord(
                    step.Index,
                    step.Name,
                    step.ToolType,
                    step.Status,
                    step.ToolSuccess,
                    step.AcceptancePassed,
                    step.Message,
                    step.ElapsedMilliseconds,
                    new Dictionary<string, double>(
                        step.Metrics ?? new Dictionary<string, double>(),
                        StringComparer.OrdinalIgnoreCase))
                    {
                        InputLayer = step.InputLayer ?? string.Empty,
                        OutputLayer = step.OutputLayer ?? string.Empty,
                        OverlayCoordinateLayer = ResolveOverlayCoordinateLayer(step),
                        Overlays = (step.Overlays ?? [])
                            .Select(overlay => new TwoDIntegrationOverlayRecord(
                                overlay.Kind,
                                overlay.Label,
                                overlay.BoundsX,
                                overlay.BoundsY,
                                overlay.BoundsWidth,
                                overlay.BoundsHeight,
                                overlay.CenterX,
                                overlay.CenterY,
                                overlay.StartX,
                                overlay.StartY,
                                overlay.EndX,
                                overlay.EndY,
                                overlay.Angle,
                                overlay.PointCount,
                                (overlay.Points ?? [])
                                    .Select(point => new TwoDIntegrationOverlayPoint(
                                        point.X,
                                        point.Y))
                                    .ToArray()))
                            .ToArray()
                    })
                .ToArray())
        {
            SourceImageWidth = sourceImageWidth,
            SourceImageHeight = sourceImageHeight,
            MetricUnitDiagnostics = metricUnitDiagnostics ?? []
        };

    internal static IntegrationMetricProjection CreateMetricProjection(
        VisionRecipeRunResult run)
    {
        var metrics = new List<IntegrationMetric>
        {
            new("totalMilliseconds", run.TotalMilliseconds, TwoDIntegrationMetricUnits.Millisecond)
        };
        var unknownUnitDiagnostics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var step in run.Steps ?? [])
        {
            foreach (var metric in step.Metrics ?? new Dictionary<string, double>())
            {
                if (double.IsFinite(metric.Value))
                {
                    string name = $"step.{step.Index}.{metric.Key}";
                    string unit = TwoDIntegrationMetricUnits.Resolve(metric.Key);
                    if (!TwoDIntegrationMetricUnits.IsKnown(metric.Key))
                    {
                        unknownUnitDiagnostics.Add(
                            $"{name}: metric unit is unknown; emitted as '{TwoDIntegrationMetricUnits.Unknown}'.");
                    }

                    metrics.Add(new(name, metric.Value, unit));
                }
            }
        }

        return new(
            metrics,
            unknownUnitDiagnostics
                .OrderBy(diagnostic => diagnostic, StringComparer.OrdinalIgnoreCase)
                .ToArray());
    }

    internal static string ResolveOverlayCoordinateLayer(
        VisionRecipeStepRunSummary step)
    {
        if (step?.Overlays is not { Count: > 0 })
        {
            return string.Empty;
        }

        string toolType = step.ToolType?.Trim() ?? string.Empty;
        if (IsOverlayMergeTool(toolType))
        {
            return MixedOverlayCoordinateLayer;
        }

        string layer = IsCoordinateTransformTool(toolType)
            ? step.OutputLayer
            : step.InputLayer;
        return string.IsNullOrWhiteSpace(layer)
            ? UnknownOverlayCoordinateLayer
            : layer.Trim();
    }

    private static bool IsCoordinateTransformTool(string toolType) =>
        string.Equals(toolType, "rotatescale", StringComparison.OrdinalIgnoreCase)
        || string.Equals(toolType, "rotateandscale", StringComparison.OrdinalIgnoreCase)
        || string.Equals(toolType, "affine", StringComparison.OrdinalIgnoreCase)
        || string.Equals(toolType, "affinematrix", StringComparison.OrdinalIgnoreCase)
        || string.Equals(toolType, "affinetransform", StringComparison.OrdinalIgnoreCase);

    private static bool IsOverlayMergeTool(string toolType) =>
        string.Equals(toolType, "overlaymerge", StringComparison.OrdinalIgnoreCase)
        || string.Equals(toolType, "resultmerge", StringComparison.OrdinalIgnoreCase)
        || string.Equals(toolType, "mergeresult", StringComparison.OrdinalIgnoreCase);

    private static string WriteRunRecord(
        string transactionDirectory,
        TwoDIntegrationRunRecord runRecord)
    {
        var artifactsDirectory = Path.Combine(
            transactionDirectory,
            IntegrationTransactionLayout.ArtifactsDirectoryName);
        Directory.CreateDirectory(artifactsDirectory);
        var target = Path.Combine(artifactsDirectory, RunRecordFileName);
        var temporary = Path.Combine(
            artifactsDirectory,
            $".{RunRecordFileName}.{Guid.NewGuid():N}.tmp");
        try
        {
            File.WriteAllText(
                temporary,
                JsonSerializer.Serialize(runRecord, RunRecordJsonOptions),
                Encoding.UTF8);
            File.Move(temporary, target);
            return target;
        }
        finally
        {
            TryDeleteFile(temporary);
        }
    }

    private static string CreateRunId(IntegrationHandoffV2 handoff)
    {
        var bytes = Encoding.UTF8.GetBytes(
            $"{handoff.TransactionId:D}|{handoff.Context.InputSha256}|{handoff.Context.RecipeSha256}");
        return $"2d-{Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()}";
    }

    private static IntegrationArtifactReference RequireContextArtifact(
        IntegrationHandoffV2 handoff,
        string role)
    {
        var matches = handoff.Context.Artifacts
            .Where(artifact => string.Equals(
                artifact.Role,
                role,
                StringComparison.Ordinal))
            .ToArray();
        if (matches.Length != 1)
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidArtifact,
                $"A 2D Handoff requires exactly one '{role}' artifact.");
        }

        var artifact = matches[0];
        var expectedHash = role == IntegrationArtifactRoles.InspectionSource
            ? handoff.Context.InputSha256
            : handoff.Context.RecipeSha256;
        if (!string.Equals(
                artifact.Sha256,
                expectedHash,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.CorrelationMismatch,
                $"The '{role}' artifact hash does not match the inspection context.");
        }

        return artifact;
    }

    private static IntegrationArtifactReference RequireLocatorTemplateArtifact(
        IntegrationHandoffV2 handoff)
    {
        var matches = handoff.Context.Artifacts
            .Where(artifact => string.Equals(
                artifact.Role,
                TwoDIntegrationLocatorRecipeContract.TemplateArtifactRole,
                StringComparison.Ordinal)
                && string.Equals(
                    artifact.ArtifactId,
                    TwoDIntegrationLocatorRecipeContract.TemplateArtifactId,
                    StringComparison.Ordinal))
            .ToArray();
        if (matches.Length != 1)
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidArtifact,
                "A locator integration Handoff requires exactly one hash-checked locator-template artifact.");
        }

        return matches[0];
    }

    private static void ValidateTwoDConsumer(IntegrationHandoffV2 handoff)
    {
        if (handoff.Context.Modality != IntegrationInspectionModality.TwoD
            || handoff.Context.InputKind != IntegrationInspectionInputKind.Image
            || !string.Equals(
                handoff.Context.ConsumerBuild.ApplicationId,
                IntegrationApplicationIds.TwoDStudio,
                StringComparison.Ordinal))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.RequestRejected,
                "The Handoff is not a 2D Image inspection request.");
        }
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

    private static DateTimeOffset NotBefore(DateTimeOffset predecessor)
    {
        var now = DateTimeOffset.UtcNow;
        return now < predecessor ? predecessor : now;
    }

    private static string GetTransactionDirectory(
        string exchangeRoot,
        Guid transactionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exchangeRoot);
        if (transactionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Transaction identity cannot be empty.",
                nameof(transactionId));
        }

        return Path.Combine(
            Path.GetFullPath(exchangeRoot),
            IntegrationTransactionLayout.TransactionsDirectoryName,
            transactionId.ToString("D"));
    }

    private static IDisposable AcquireRunLease(string transactionDirectory)
    {
        var lockPath = Path.Combine(transactionDirectory, RunLockFileName);
        try
        {
            return new RunLease(lockPath);
        }
        catch (IOException exception) when (IsRunLeaseContention(exception))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidState,
                "The Handoff is already being run.",
                exception);
        }
    }

    private static bool IsRunLeaseContention(IOException exception)
    {
        var win32Error = exception.HResult & 0xFFFF;
        return win32Error is 32 or 33;
    }

    private static string ResolveArtifactPath(
        string transactionDirectory,
        IntegrationArtifactReference artifact)
    {
        var root = Path.GetFullPath(transactionDirectory);
        var path = Path.GetFullPath(Path.Combine(
            root,
            artifact.RelativePath.Replace('/', Path.DirectorySeparatorChar)));
        var rootPrefix = root.TrimEnd(Path.DirectorySeparatorChar)
                         + Path.DirectorySeparatorChar;
        if (!path.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.UnsafeArtifactPath,
                "Artifact path escapes the transaction directory.");
        }

        return path;
    }

    private static void EnsureNoReparsePoints(
        string transactionDirectory,
        string relativePath)
    {
        var current = Path.GetFullPath(transactionDirectory);
        var root = new DirectoryInfo(current);
        if (root.Exists && root.Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.UnsafeArtifactPath,
                "The transaction directory cannot be a symbolic link or reparse point.");
        }

        var segments = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        for (var index = 0; index < segments.Length; index++)
        {
            var segment = segments[index];
            current = Path.Combine(current, segment);
            FileSystemInfo entry = index == segments.Length - 1
                ? new FileInfo(current)
                : new DirectoryInfo(current);
            if (entry.Exists && entry.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                throw new IntegrationContractException(
                    IntegrationErrorCode.UnsafeArtifactPath,
                    "Artifact paths cannot traverse symbolic links or reparse points.");
            }
        }
    }

    private static byte[] ReadMessage(
        string transactionDirectory,
        string fileName) =>
        File.ReadAllBytes(Path.Combine(transactionDirectory, fileName));

    private static void WriteNewMessage(
        string transactionDirectory,
        string fileName,
        byte[] bytes)
    {
        var target = Path.Combine(transactionDirectory, fileName);
        var temporary = Path.Combine(
            transactionDirectory,
            $".{fileName}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(
                       temporary,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.Read,
                       bufferSize: 4096,
                       options: FileOptions.SequentialScan))
            {
                stream.Write(bytes);
                stream.Flush(flushToDisk: true);
            }

            File.Move(temporary, target);
        }
        finally
        {
            TryDeleteFile(temporary);
        }
    }

    private sealed class RunLease : IDisposable
    {
        private readonly string _path;
        private readonly FileStream _stream;

        public RunLease(string path)
        {
            _path = path;
            _stream = new FileStream(
                path,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None,
                bufferSize: 1,
                options: FileOptions.SequentialScan);
        }

        public void Dispose()
        {
            _stream.Dispose();
            TryDeleteFile(_path);
        }
    }

    private static void ThrowIfInvalid(IntegrationValidationResult validation)
    {
        if (validation.IsValid)
        {
            return;
        }

        var issue = validation.Issues[0];
        throw new IntegrationContractException(
            issue.Code,
            $"{issue.Field}: {issue.Message}");
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Preserve the original contract or I/O failure.
        }
    }

    private static void TryDeleteLocatorArtifacts(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        TryDeleteFile(Path.Combine(directory, "evidence.packet.json"));
        TryDeleteFile(Path.Combine(directory, "locator-runtime-overlay.png"));
    }
}
