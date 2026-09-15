using OpenVisionLab;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class TwoDIntegrationResultDispositionContract
{
    public static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();

        RunCase("quality pass", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: true,
                CreateStep("OK", toolSuccess: true, acceptancePassed: true));
            AssertDisposition(
                run,
                IntegrationResultStatus.Completed,
                IntegrationInspectionOutcome.Pass,
                expectedError: null);
        }, passed, failed);

        RunCase("acceptance NG remains Completed/Ng", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: false,
                CreateStep(
                    "NG",
                    toolSuccess: true,
                    acceptancePassed: false,
                    message: "Score is below the acceptance range."));
            AssertDisposition(
                run,
                IntegrationResultStatus.Completed,
                IntegrationInspectionOutcome.Ng,
                expectedError: null);
        }, passed, failed);

        RunCase("ToolFactoryFailed is execution error", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: false,
                CreateStep(
                    "ERROR",
                    toolSuccess: false,
                    acceptancePassed: false,
                    errorCode: 200,
                    errorName: "ToolFactoryFailed"));
            AssertExecutionFailure(run, "ToolFactoryFailed");
        }, passed, failed);

        RunCase("InvalidRoi is execution error", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: false,
                CreateStep(
                    "ERROR",
                    toolSuccess: false,
                    acceptancePassed: false,
                    errorCode: 110,
                    errorName: "InvalidRoi"));
            AssertExecutionFailure(run, "InvalidRoi");
        }, passed, failed);

        RunCase("StepTimeout is execution error", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: false,
                CreateStep(
                    "TIMEOUT",
                    toolSuccess: false,
                    acceptancePassed: false,
                    errorCode: 300,
                    errorName: "StepTimeout"));
            AssertExecutionFailure(run, "StepTimeout");
        }, passed, failed);

        RunCase("StepCanceled is Cancelled/Indeterminate", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: false,
                CreateStep(
                    "CANCEL",
                    toolSuccess: false,
                    acceptancePassed: false,
                    errorCode: 301,
                    errorName: "StepCanceled"));
            var disposition = TwoDIntegrationExchange.ClassifyRunResult(run);
            Require(
                disposition.Status == IntegrationResultStatus.Cancelled
                && disposition.Outcome == IntegrationInspectionOutcome.Indeterminate
                && disposition.Error?.Code == IntegrationErrorCode.Cancelled
                && disposition.Error.Retryable,
                "StepCanceled did not map to Cancelled/Indeterminate.");
        }, passed, failed);

        RunCase("all skipped steps fail closed", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: true,
                CreateStep("SKIP", toolSuccess: false, acceptancePassed: true, skipped: true));
            AssertExecutionFailure(run, "completed step result");
        }, passed, failed);

        RunCase("unknown step status fails closed", () =>
        {
            using VisionRecipeRunResult run = CreateRun(
                success: false,
                CreateStep("UNKNOWN", toolSuccess: false, acceptancePassed: false));
            AssertExecutionFailure(run, "returned UNKNOWN");
        }, passed, failed);

        RunCase("execution error disposition is valid v2 Result state", () =>
        {
            IntegrationApplicationIdentity identity = new(
                "OpenVisionLab.2DStudio",
                "0.2.0-alpha.3",
                new string('1', 40),
                IntegrationSourceState.Clean);
            IntegrationRunCorrelation correlation = new(
                "project",
                "project/1.0",
                "sequence",
                "step",
                "camera",
                "acquisition",
                "frame",
                "px",
                IntegrationInspectionModality.TwoD,
                IntegrationInspectionInputKind.Image,
                new string('2', 64),
                new string('3', 64),
                identity);
            IntegrationResultV2 result = new(
                IntegrationContractSchema.V2,
                IntegrationMessageKind.Result,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                identity,
                IntegrationResultStatus.Failed,
                IntegrationInspectionOutcome.ExecutionError,
                null,
                null,
                correlation,
                [],
                [],
                new IntegrationError(
                    IntegrationErrorCode.ExecutionFailed,
                    "Fixture step returned ERROR. Error=200:ToolFactoryFailed.",
                    false));
            var validation = IntegrationContractValidator.Validate(result);
            Require(
                validation.IsValid,
                "Failed/ExecutionError without run identity was rejected by the v2 validator: "
                + string.Join(
                    "; ",
                    validation.Issues.Select(issue => $"{issue.Field}={issue.Message}")));
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "two-d-integration-result-disposition-contract.txt");
        File.WriteAllLines(
            reportPath,
            passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            $"CONTRACT|2d-integration-result-disposition|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
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

    private static VisionRecipeRunResult CreateRun(
        bool success,
        params VisionRecipeStepRunSummary[] steps) =>
        new()
        {
            Success = success,
            Steps = steps.ToList()
        };

    private static VisionRecipeStepRunSummary CreateStep(
        string status,
        bool toolSuccess,
        bool acceptancePassed,
        int errorCode = 0,
        string errorName = "",
        string message = "",
        bool skipped = false) =>
        new()
        {
            Index = 1,
            Name = "Fixture step",
            ToolType = "Fixture",
            Status = status,
            ToolSuccess = toolSuccess,
            Success = status.Equals("OK", StringComparison.OrdinalIgnoreCase) || skipped,
            Skipped = skipped,
            AcceptancePassed = acceptancePassed,
            ErrorCode = errorCode,
            ErrorName = errorName,
            Message = message
        };

    private static void AssertDisposition(
        VisionRecipeRunResult run,
        IntegrationResultStatus expectedStatus,
        IntegrationInspectionOutcome expectedOutcome,
        IntegrationErrorCode? expectedError)
    {
        var disposition = TwoDIntegrationExchange.ClassifyRunResult(run);
        Require(
            disposition.Status == expectedStatus
            && disposition.Outcome == expectedOutcome
            && (expectedError is null
                ? disposition.Error is null
                : disposition.Error?.Code == expectedError),
            $"Expected {expectedStatus}/{expectedOutcome}/{expectedError?.ToString() ?? "no error"}, "
            + $"got {disposition.Status}/{disposition.Outcome}/{disposition.Error?.Code.ToString() ?? "no error"}.");
    }

    private static void AssertExecutionFailure(VisionRecipeRunResult run, string expectedMessage)
    {
        var disposition = TwoDIntegrationExchange.ClassifyRunResult(run);
        Require(
            disposition.Status == IntegrationResultStatus.Failed
            && disposition.Outcome == IntegrationInspectionOutcome.ExecutionError
            && disposition.Error?.Code == IntegrationErrorCode.ExecutionFailed
            && disposition.Error.Message.Contains(expectedMessage, StringComparison.Ordinal),
            $"Execution failure did not preserve the typed step detail: {disposition.Error?.Message ?? "-"}.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
