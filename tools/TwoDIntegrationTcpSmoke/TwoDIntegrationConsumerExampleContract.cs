using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using OpenVisionLab.Integration.Contracts;

internal static class TwoDIntegrationConsumerExampleContract
{
    public static int Run(string evidenceDirectory)
    {
        string root = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(root);
        List<string> passed = new();
        List<string> failed = new();
        ConsumerFixture fixture = CreateFixture();

        RunCase(
            "Completed/Pass dispatches quality pass",
            () => AssertOutcome(
                fixture,
                IntegrationResultStatus.Completed,
                IntegrationInspectionOutcome.Pass,
                TwoDIntegrationConsumerAction.QualityPass,
                null),
            passed,
            failed);
        RunCase(
            "Completed/Ng dispatches quality NG",
            () => AssertOutcome(
                fixture,
                IntegrationResultStatus.Completed,
                IntegrationInspectionOutcome.Ng,
                TwoDIntegrationConsumerAction.QualityNg,
                null),
            passed,
            failed);
        RunCase(
            "Failed/ExecutionError dispatches execution error",
            () => AssertOutcome(
                fixture,
                IntegrationResultStatus.Failed,
                IntegrationInspectionOutcome.ExecutionError,
                TwoDIntegrationConsumerAction.ExecutionError,
                new IntegrationError(
                    IntegrationErrorCode.ExecutionFailed,
                    "Fixture execution failed.",
                    false)),
            passed,
            failed);
        RunCase(
            "Cancelled/Indeterminate dispatches cancellation",
            () => AssertOutcome(
                fixture,
                IntegrationResultStatus.Cancelled,
                IntegrationInspectionOutcome.Indeterminate,
                TwoDIntegrationConsumerAction.Cancelled,
                new IntegrationError(
                    IntegrationErrorCode.Cancelled,
                    "Fixture was cancelled.",
                    true)),
            passed,
            failed);
        RunCase(
            "unclassified terminal pair fails closed",
            () =>
            {
                IntegrationResultV2 result = CreateResult(
                    fixture,
                    IntegrationResultStatus.Completed,
                    IntegrationInspectionOutcome.Indeterminate,
                    null);
                try
                {
                    _ = TwoDIntegrationConsumerExample.Dispatch(result);
                    throw new InvalidOperationException(
                        "The consumer accepted an unclassified status/outcome pair.");
                }
                catch (IntegrationContractException exception)
                {
                    Require(
                        exception.ErrorCode == IntegrationErrorCode.InvalidState,
                        "The unclassified pair did not return InvalidState.");
                }
            },
            passed,
            failed);
        RunCase(
            "correlation mismatch fails before dispatch",
            () =>
            {
                IntegrationResultV2 result = CreateResult(
                    fixture,
                    IntegrationResultStatus.Completed,
                    IntegrationInspectionOutcome.Pass,
                    null) with
                {
                    TransactionId = Guid.NewGuid()
                };
                try
                {
                    TwoDIntegrationConsumerExample.AssertCorrelation(
                        fixture.Handoff,
                        fixture.Acknowledgement,
                        result);
                    throw new InvalidOperationException(
                        "The consumer accepted a mismatched transaction identity.");
                }
                catch (InvalidOperationException exception)
                    when (exception.Message.Contains(
                        "identity does not match",
                        StringComparison.Ordinal))
                {
                    // Expected fail-closed consumer behavior.
                }
            },
            passed,
            failed);

        var report = new
        {
            schemaVersion = "1.0",
            actions = new[]
            {
                new { status = "Completed", outcome = "Pass", action = "QualityPass" },
                new { status = "Completed", outcome = "Ng", action = "QualityNg" },
                new { status = "Failed", outcome = "ExecutionError", action = "ExecutionError" },
                new { status = "Cancelled", outcome = "Indeterminate", action = "Cancelled" }
            },
            explicitAckRunRequired = true,
            receiveDoesNotRun = true,
            awaitThenDispose = true,
            passed,
            failed
        };
        string reportPath = Path.Combine(
            root,
            "two-d-integration-consumer-example-contract.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            $"CONTRACT|2d-integration-csharp-consumer-example|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void AssertOutcome(
        ConsumerFixture fixture,
        IntegrationResultStatus status,
        IntegrationInspectionOutcome outcome,
        TwoDIntegrationConsumerAction expectedAction,
        IntegrationError? error)
    {
        IntegrationResultV2 result = CreateResult(fixture, status, outcome, error);
        IntegrationValidationResult validation =
            IntegrationContractValidator.ValidateV2Sequence(
                fixture.Handoff,
                fixture.Acknowledgement,
                result);
        Require(
            validation.IsValid,
            "The fixture result failed the shared v2 validator: "
            + string.Join(
                "; ",
                validation.Issues.Select(issue => $"{issue.Field}={issue.Message}")));
        TwoDIntegrationConsumerExample.AssertCorrelation(
            fixture.Handoff,
            fixture.Acknowledgement,
            result);
        Require(
            TwoDIntegrationConsumerExample.Dispatch(result) == expectedAction,
            $"Expected {expectedAction}, got {result.Status}/{result.Outcome}.");
    }

    private static ConsumerFixture CreateFixture()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        IntegrationApplicationIdentity producer = new(
            IntegrationApplicationIds.MachineStudio,
            "2.2.0-dev.5",
            new string('1', 40),
            IntegrationSourceState.Clean);
        IntegrationApplicationIdentity consumer = new(
            IntegrationApplicationIds.TwoDStudio,
            "2.2.0-dev.5",
            new string('2', 40),
            IntegrationSourceState.Clean);
        IntegrationArtifactReference source = new(
            IntegrationArtifactRoles.InspectionSource,
            "source",
            "artifacts/source.png",
            12,
            new string('3', 64));
        IntegrationArtifactReference recipe = new(
            IntegrationArtifactRoles.InspectionRecipe,
            "recipe",
            "artifacts/recipe.xml",
            14,
            new string('4', 64));
        IntegrationInspectionContextV2 context = new(
            "project-001",
            "machine-project/1.0",
            "sequence-001",
            "inspect-image",
            "camera-virtual",
            "acquisition-example",
            "frame-001",
            "px",
            IntegrationInspectionModality.TwoD,
            IntegrationInspectionInputKind.Image,
            source.Sha256,
            recipe.Sha256,
            consumer,
            new[] { source, recipe });
        IntegrationHandoffV2 handoff = new(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Handoff,
            Guid.NewGuid(),
            Guid.NewGuid(),
            now.AddMinutes(-2),
            producer,
            context);
        IntegrationAcknowledgementV2 acknowledgement = new(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Acknowledgement,
            Guid.NewGuid(),
            handoff.TransactionId,
            handoff.MessageId,
            now.AddMinutes(-1),
            consumer,
            IntegrationAcknowledgementStatus.Accepted,
            null);
        return new(handoff, acknowledgement, context, consumer);
    }

    private static IntegrationResultV2 CreateResult(
        ConsumerFixture fixture,
        IntegrationResultStatus status,
        IntegrationInspectionOutcome outcome,
        IntegrationError? error)
    {
        bool completed = status == IntegrationResultStatus.Completed;
        IntegrationArtifactReference? runRecord = completed
            ? new IntegrationArtifactReference(
                IntegrationArtifactRoles.RunRecord,
                "run-example",
                "artifacts/2d-run-record.json",
                1,
                new string('5', 64))
            : null;
        return new(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Result,
            Guid.NewGuid(),
            fixture.Handoff.TransactionId,
            fixture.Handoff.MessageId,
            fixture.Acknowledgement.MessageId,
            DateTimeOffset.UtcNow,
            fixture.Consumer,
            status,
            outcome,
            completed ? "run-example" : null,
            runRecord,
            IntegrationRunCorrelation.FromContext(fixture.Context),
            Array.Empty<IntegrationMetric>(),
            Array.Empty<IntegrationArtifactReference>(),
            error);
    }

    private static void RunCase(
        string name,
        Action action,
        List<string> passed,
        List<string> failed)
    {
        try
        {
            action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add($"{name}: {exception.GetBaseException().Message}");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed record ConsumerFixture(
        IntegrationHandoffV2 Handoff,
        IntegrationAcknowledgementV2 Acknowledgement,
        IntegrationInspectionContextV2 Context,
        IntegrationApplicationIdentity Consumer);
}
