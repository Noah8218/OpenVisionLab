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

/// <summary>
/// Explicit 2D consumer adapter for the v2 file exchange. Reading and
/// acknowledgement validate only; an inspection starts only through the
/// explicit RunAcceptedHandoffAsync call.
/// </summary>
public static class TwoDIntegrationExchange
{
    private const string RunRecordFileName = "2d-run-record.json";
    private const string MetricUnit = "unitless";
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
        AcknowledgeHandoff(
            exchangeRoot,
            transactionId,
            ReadHandoff(exchangeRoot, transactionId).Context.ConsumerBuild);

    internal static IntegrationAcknowledgementV2 AcknowledgeHandoff(
        string exchangeRoot,
        Guid transactionId,
        string runtimeBuildManifestPath) =>
        AcknowledgeHandoff(
            exchangeRoot,
            transactionId,
            TwoDIntegrationBuildIdentity.LoadQualifiedTargetIdentity(
                ReadHandoff(exchangeRoot, transactionId).Context.ConsumerBuild,
                runtimeBuildManifestPath));

    public static IntegrationAcknowledgementV2 AcknowledgeHandoff(
        string exchangeRoot,
        Guid transactionId,
        IntegrationApplicationIdentity consumerBuild) =>
        PublishAcknowledgement(
            exchangeRoot,
            transactionId,
            consumerBuild,
            rejectionReason: null);

    public static IntegrationAcknowledgementV2 RejectHandoff(
        string exchangeRoot,
        Guid transactionId,
        string rejectionReason) =>
        RejectHandoff(
            exchangeRoot,
            transactionId,
            ReadHandoffEnvelope(exchangeRoot, transactionId).Context.ConsumerBuild,
            rejectionReason);

    internal static IntegrationAcknowledgementV2 RejectHandoff(
        string exchangeRoot,
        Guid transactionId,
        string rejectionReason,
        string runtimeBuildManifestPath) =>
        RejectHandoff(
            exchangeRoot,
            transactionId,
            TwoDIntegrationBuildIdentity.LoadQualifiedTargetIdentity(
                ReadHandoffEnvelope(exchangeRoot, transactionId).Context.ConsumerBuild,
                runtimeBuildManifestPath),
            rejectionReason);

    public static IntegrationAcknowledgementV2 RejectHandoff(
        string exchangeRoot,
        Guid transactionId,
        IntegrationApplicationIdentity consumerBuild,
        string rejectionReason) =>
        PublishAcknowledgement(
            exchangeRoot,
            transactionId,
            consumerBuild,
            rejectionReason);

    private static IntegrationAcknowledgementV2 PublishAcknowledgement(
        string exchangeRoot,
        Guid transactionId,
        IntegrationApplicationIdentity consumerBuild,
        string rejectionReason)
    {
        ArgumentNullException.ThrowIfNull(consumerBuild);
        if (rejectionReason is not null && string.IsNullOrWhiteSpace(rejectionReason))
        {
            throw new ArgumentException(
                "Rejection reason cannot be blank.",
                nameof(rejectionReason));
        }

        var handoff = rejectionReason is null
            ? ReadHandoff(exchangeRoot, transactionId)
            : ReadHandoffEnvelope(exchangeRoot, transactionId);
        EnsureConsumerIdentity(handoff, consumerBuild);
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

    public static async Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        string exchangeRoot,
        Guid transactionId,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default) =>
        await RunAcceptedHandoffAsync(
                exchangeRoot,
                transactionId,
                ReadHandoff(exchangeRoot, transactionId).Context.ConsumerBuild,
                stepTimeoutMilliseconds,
                cancellationToken)
            .ConfigureAwait(false);

    internal static Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        string exchangeRoot,
        Guid transactionId,
        string runtimeBuildManifestPath,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default) =>
        RunAcceptedHandoffAsync(
            exchangeRoot,
            transactionId,
            TwoDIntegrationBuildIdentity.LoadQualifiedTargetIdentity(
                ReadHandoff(exchangeRoot, transactionId).Context.ConsumerBuild,
                runtimeBuildManifestPath),
            stepTimeoutMilliseconds,
            cancellationToken);

    public static async Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        string exchangeRoot,
        Guid transactionId,
        IntegrationApplicationIdentity consumerBuild,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(consumerBuild);
        if (stepTimeoutMilliseconds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stepTimeoutMilliseconds),
                "Step timeout must be greater than zero.");
        }

        var handoff = ReadHandoff(exchangeRoot, transactionId);
        EnsureConsumerIdentity(handoff, consumerBuild);
        var acknowledgement = ReadAcknowledgement(exchangeRoot, transactionId);
        if (acknowledgement.Status != IntegrationAcknowledgementStatus.Accepted)
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidState,
                "A completed 2D inspection requires an accepted Acknowledgement.");
        }

        var transactionDirectory = GetTransactionDirectory(exchangeRoot, transactionId);
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

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var source = Cv2.ImRead(sourcePath, ImreadModes.Unchanged);
            if (source.Empty())
            {
                throw new InvalidOperationException(
                    $"The inspection source could not be decoded: {sourceArtifact.RelativePath}");
            }

            using var run = await new VisionRecipeRunner().RunAsync(
                    recipePath,
                    source,
                    VisionRecipeRunner.DefaultInputLayer,
                    stepTimeoutMilliseconds,
                    cancellationToken)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var runId = CreateRunId(handoff);
            var runRecord = CreateRunRecord(
                runId,
                handoff,
                sourceArtifact,
                recipeArtifact,
                run,
                source.Width,
                source.Height);
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
                    IntegrationResultStatus.Completed,
                    run.Success
                        ? IntegrationInspectionOutcome.Pass
                        : IntegrationInspectionOutcome.Ng,
                    runId,
                    runRecordReference,
                    IntegrationRunCorrelation.FromContext(handoff.Context),
                    CreateMetrics(run),
                    [],
                    null);
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

    private static TwoDIntegrationRunRecord CreateRunRecord(
        string runId,
        IntegrationHandoffV2 handoff,
        IntegrationArtifactReference sourceArtifact,
        IntegrationArtifactReference recipeArtifact,
        VisionRecipeRunResult run,
        int sourceImageWidth,
        int sourceImageHeight) =>
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
            SourceImageHeight = sourceImageHeight
        };

    private static IReadOnlyList<IntegrationMetric> CreateMetrics(
        VisionRecipeRunResult run)
    {
        var metrics = new List<IntegrationMetric>
        {
            new("totalMilliseconds", run.TotalMilliseconds, "ms")
        };
        foreach (var step in run.Steps ?? [])
        {
            foreach (var metric in step.Metrics ?? new Dictionary<string, double>())
            {
                if (double.IsFinite(metric.Value))
                {
                    metrics.Add(new(
                        $"step.{step.Index}.{metric.Key}",
                        metric.Value,
                        MetricUnit));
                }
            }
        }

        return metrics;
    }

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

    private static void EnsureConsumerIdentity(
        IntegrationHandoffV2 handoff,
        IntegrationApplicationIdentity consumerBuild)
    {
        ValidateTwoDConsumer(handoff);
        if (!ApplicationIdentitiesMatch(
                handoff.Context.ConsumerBuild,
                consumerBuild))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.CorrelationMismatch,
                "The supplied 2D consumer build does not match the Handoff context.");
        }
    }

    private static bool ApplicationIdentitiesMatch(
        IntegrationApplicationIdentity actual,
        IntegrationApplicationIdentity expected) =>
        string.Equals(actual.ApplicationId, expected.ApplicationId, StringComparison.Ordinal)
        && string.Equals(actual.ApplicationVersion, expected.ApplicationVersion, StringComparison.Ordinal)
        && string.Equals(actual.SourceCommit, expected.SourceCommit, StringComparison.OrdinalIgnoreCase)
        && actual.SourceState == expected.SourceState;

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
}
