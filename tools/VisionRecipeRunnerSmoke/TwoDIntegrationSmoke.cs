using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;

internal static class TwoDIntegrationSmoke
{
    private const string ProducerCommit = "1111111111111111111111111111111111111111";
    private const string ConsumerCommit = "2222222222222222222222222222222222222222";

    public static async Task<int> RunAsync(
        string evidenceRoot,
        string goodImagePath,
        string badImagePath,
        string pipelineXmlPath)
    {
        string root = Path.GetFullPath(evidenceRoot);
        Directory.CreateDirectory(root);
        string runRoot = Path.Combine(
            root,
            $"two-d-integration-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runRoot);

        var consumer = new IntegrationApplicationIdentity(
            IntegrationApplicationIds.TwoDStudio,
            "2.1.0",
            ConsumerCommit,
            IntegrationSourceState.Clean);

        var good = await RunCaseAsync(
            runRoot,
            "good",
            goodImagePath,
            pipelineXmlPath,
            consumer,
            expectedOutcome: IntegrationInspectionOutcome.Pass);
        var bad = await RunCaseAsync(
            runRoot,
            "bad",
            badImagePath,
            pipelineXmlPath,
            consumer,
            expectedOutcome: IntegrationInspectionOutcome.Ng);
        await RunRejectedCaseAsync(
            runRoot,
            goodImagePath,
            pipelineXmlPath,
            consumer);
        RunTamperCase(runRoot, goodImagePath, pipelineXmlPath, consumer);

        Console.WriteLine(
            $"2D integration smoke passed. Good={good.Outcome}, Bad={bad.Outcome}, Evidence={runRoot}");
        return 0;
    }

    private static async Task<IntegrationResultV2> RunCaseAsync(
        string runRoot,
        string caseName,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
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
            consumer);
        var result = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
            runRoot,
            handoff.TransactionId,
            consumer);
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

        Console.WriteLine(
            $"2D {caseName}: outcome={result.Outcome}, runId={result.RunId}, metrics={result.Metrics.Count}");
        return result;
    }

    private static async Task RunRejectedCaseAsync(
        string runRoot,
        string imagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer)
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
            consumer,
            "The recipe is not supported by this 2D consumer.");
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
                consumer);
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
        bool tamperSourceAfterPublish)
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
            IntegrationInspectionModality.TwoD,
            IntegrationInspectionInputKind.Image,
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
