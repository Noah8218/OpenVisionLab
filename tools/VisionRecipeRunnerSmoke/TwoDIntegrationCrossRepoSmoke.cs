using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;

internal static class TwoDIntegrationCrossRepoSmoke
{
    public static async Task<int> RunAsync(
        string exchangeRoot,
        string producerManifestPath,
        string evidenceRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exchangeRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(producerManifestPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(evidenceRoot);

        var manifest = ReadManifest(producerManifestPath);
        var handoff = TwoDIntegrationExchange.ReadHandoff(
            exchangeRoot,
            manifest.TransactionId);
        Require(
            handoff.Producer.ApplicationId == IntegrationApplicationIds.MachineStudio,
            "The published Handoff was not produced by Machine Studio.");
        Require(
            handoff.Context.Modality == IntegrationInspectionModality.TwoD
                && handoff.Context.InputKind == IntegrationInspectionInputKind.Image,
            "The published Handoff is not a TwoD/Image request.");
        Require(
            ApplicationIdentitiesMatch(handoff.Producer, manifest.Producer),
            "The producer identity in the manifest does not match the Handoff.");
        Require(
            ApplicationIdentitiesMatch(handoff.Context.ConsumerBuild, manifest.Consumer),
            "The consumer identity in the manifest does not match the Handoff.");
        Require(
            string.Equals(
                handoff.Context.InputSha256,
                manifest.SourceSha256,
                StringComparison.OrdinalIgnoreCase)
            && string.Equals(
                handoff.Context.RecipeSha256,
                manifest.RecipeSha256,
                StringComparison.OrdinalIgnoreCase),
            "The manifest artifact identities do not match the Handoff context.");

        var acknowledgement = TwoDIntegrationExchange.AcknowledgeHandoff(
            exchangeRoot,
            manifest.TransactionId,
            manifest.Consumer);
        var transactionDirectory = Path.Combine(
            Path.GetFullPath(exchangeRoot),
            IntegrationTransactionLayout.TransactionsDirectoryName,
            manifest.TransactionId.ToString("D"));
        Require(
            acknowledgement.Status == IntegrationAcknowledgementStatus.Accepted
                && !File.Exists(Path.Combine(
                    transactionDirectory,
                    IntegrationTransactionLayout.ResultFileName)),
            "Acknowledging the published Handoff unexpectedly executed the consumer.");

        var result = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                exchangeRoot,
                manifest.TransactionId,
                manifest.Consumer)
            .ConfigureAwait(false);
        var persisted = TwoDIntegrationExchange.ReadResult(
            exchangeRoot,
            manifest.TransactionId);
        Require(
            result.Status == IntegrationResultStatus.Completed
                && result.RunRecord is not null
                && persisted.RunRecord is not null
                && !string.IsNullOrWhiteSpace(result.RunId),
            "The 2D consumer did not publish a completed correlated Run Record.");

        var runRecordPath = Path.GetFullPath(Path.Combine(
            transactionDirectory,
            result.RunRecord!.RelativePath.Replace('/', Path.DirectorySeparatorChar)));
        var runRecord = JsonSerializer.Deserialize<TwoDIntegrationRunRecord>(
            File.ReadAllText(runRecordPath),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        Require(
            runRecord is not null
                && runRecord.SourceImageWidth > 1
                && runRecord.SourceImageHeight > 1
                && runRecord.Steps?.SelectMany(step => step.Overlays ?? []).Any() == true,
            "The 2D Run Record did not retain runtime overlay geometry and source dimensions.");
        var validRunRecord = runRecord
            ?? throw new InvalidOperationException("The 2D Run Record was unexpectedly null.");
        var overlayCount = validRunRecord.Steps?.SelectMany(step => step.Overlays ?? []).Count() ?? 0;

        var fullEvidenceRoot = Path.GetFullPath(evidenceRoot);
        Directory.CreateDirectory(fullEvidenceRoot);
        var reportPath = Path.Combine(
            fullEvidenceRoot,
            "2d-cross-repo-consumer-result.json");
        var report = new TwoDIntegrationCrossRepoReport(
            "1.0",
            manifest.TransactionId,
            manifest.MessageId,
            handoff.Producer.ApplicationId,
            handoff.Producer.SourceCommit,
            manifest.Consumer.ApplicationId,
            manifest.Consumer.SourceCommit,
            acknowledgement.Status.ToString(),
            result.Status.ToString(),
            result.Outcome.ToString(),
            result.RunId!,
            manifest.SourceSha256,
            manifest.RecipeSha256,
            result.Metrics.Count,
            validRunRecord.SourceImageWidth,
            validRunRecord.SourceImageHeight,
            overlayCount);
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                report,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }));

        Console.WriteLine(
            $"2D cross-repo smoke passed. Transaction={manifest.TransactionId}, Outcome={result.Outcome}, RunId={result.RunId}");
        Console.WriteLine($"Consumer evidence={reportPath}");
        return 0;
    }

    private static MachineTwoDProducerManifest ReadManifest(string path)
    {
        var manifest = JsonSerializer.Deserialize<MachineTwoDProducerManifest>(
            File.ReadAllText(Path.GetFullPath(path)),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (manifest is null || manifest.SchemaVersion != "1.0")
        {
            throw new InvalidOperationException(
                "The Machine Studio producer manifest is missing or unsupported.");
        }

        return manifest;
    }

    private static bool ApplicationIdentitiesMatch(
        IntegrationApplicationIdentity actual,
        IntegrationApplicationIdentity expected) =>
        string.Equals(actual.ApplicationId, expected.ApplicationId, StringComparison.Ordinal)
        && string.Equals(actual.ApplicationVersion, expected.ApplicationVersion, StringComparison.Ordinal)
        && string.Equals(actual.SourceCommit, expected.SourceCommit, StringComparison.OrdinalIgnoreCase)
        && actual.SourceState == expected.SourceState;

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

internal sealed record MachineTwoDProducerManifest(
    string SchemaVersion,
    Guid TransactionId,
    Guid MessageId,
    DateTimeOffset CreatedAtUtc,
    IntegrationApplicationIdentity Producer,
    IntegrationApplicationIdentity Consumer,
    string SourceSha256,
    string RecipeSha256);

internal sealed record TwoDIntegrationCrossRepoReport(
    string SchemaVersion,
    Guid TransactionId,
    Guid MessageId,
    string ProducerApplicationId,
    string ProducerCommit,
    string ConsumerApplicationId,
    string ConsumerCommit,
    string AcknowledgementStatus,
    string ResultStatus,
    string Outcome,
    string RunId,
    string SourceSha256,
    string RecipeSha256,
    int MetricCount,
    int SourceImageWidth,
    int SourceImageHeight,
    int OverlayCount);
