using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ExpectedFailureContract
{
    public static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        RunCase("strict quality NG requires acceptance failure", () =>
        {
            VisionPipelineExpectedFailureEvaluation evaluation = Evaluate(
                CreateSample("QualityNG", string.Empty, "Acceptance"),
                CreateRun(CreateQualityNgStep()));
            Require(evaluation.IsValid && !evaluation.IsLegacy && evaluation.IsQualityNg
                && evaluation.Classification == VisionPipelineExpectedFailureContract.QualityNgOutcome,
                "Strict quality NG was not accepted as a typed acceptance failure.");
        }, passed, failed);

        RunCase("strict controlled no-result requires supported error", () =>
        {
            VisionPipelineExpectedFailureEvaluation evaluation = Evaluate(
                CreateSample("ControlledNoResult", "MatchingNoResult", "Match"),
                CreateRun(CreateNoResultStep("MatchingNoResult", 606)));
            Require(evaluation.IsValid && !evaluation.IsLegacy && evaluation.IsControlledNoResult
                && evaluation.Classification == VisionPipelineExpectedFailureContract.ControlledNoResultOutcome,
                "Strict controlled no-result was not accepted.");
        }, passed, failed);

        RunCase("legacy quality NG is accepted but marked legacy", () =>
        {
            VisionPipelineExpectedFailureEvaluation evaluation = Evaluate(
                CreateSample(string.Empty, string.Empty, string.Empty),
                CreateRun(CreateQualityNgStep()));
            Require(evaluation.IsValid && evaluation.IsLegacy && evaluation.ValidationStrength == "Legacy",
                "Unspecified ExpectedFailure was not marked legacy.");
        }, passed, failed);

        RunCase("timeout is not a quality NG", () =>
        {
            VisionPipelineExpectedFailureEvaluation evaluation = Evaluate(
                CreateSample(string.Empty, string.Empty, string.Empty),
                CreateRun(CreateNoResultStep("StepTimeout", 408)));
            Require(!evaluation.IsValid && !evaluation.IsQualityNg && !evaluation.IsControlledNoResult,
                "Timeout was incorrectly accepted as an ExpectedFailure quality NG.");
        }, passed, failed);

        RunCase("invalid tool is not a controlled no-result", () =>
        {
            VisionPipelineExpectedFailureEvaluation evaluation = Evaluate(
                CreateSample("ControlledNoResult", "ToolFactoryFailed", "Invalid Tool"),
                CreateRun(CreateNoResultStep("ToolFactoryFailed", 500)));
            Require(!evaluation.IsValid && !evaluation.IsControlledNoResult,
                "ToolFactoryFailed was incorrectly accepted as a controlled no-result.");
        }, passed, failed);

        RunCase("wrong error and step fail closed", () =>
        {
            VisionPipelineExpectedFailureEvaluation evaluation = Evaluate(
                CreateSample("ControlledNoResult", "MatchingNoResult", "Other Step"),
                CreateRun(CreateNoResultStep("ContourNoResult", 607)));
            Require(!evaluation.IsValid && evaluation.Message.Contains("ExpectedError", StringComparison.OrdinalIgnoreCase),
                "Mismatched ExpectedError/ExpectedFailedStep did not fail closed.");
        }, passed, failed);

        RunCase("sample service keeps quality NG completed", () =>
        {
            VisionPipelineSampleCatalogItem sample = FindProductSample("Product_Battery_TabGap_Narrow_Bad");
            VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService
                .RunSampleCheckSafeAsync(sample)
                .GetAwaiter()
                .GetResult();
            Require(result.Success && result.ExecutionCompleted && !result.HasToolError
                && result.ExpectedFailureClassification == VisionPipelineExpectedFailureContract.QualityNgOutcome,
                "Sample service did not preserve a completed quality NG ExpectedFailure.");
        }, passed, failed);

        RunCase("sample service separates controlled no-result", () =>
        {
            VisionPipelineSampleCatalogItem sample = FindProductSample("Product_Battery_LaserMark_Missing_Bad");
            VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService
                .RunSampleCheckSafeAsync(sample)
                .GetAwaiter()
                .GetResult();
            Require(result.Success && result.ExecutionCompleted && result.HasToolError
                && result.ExpectedFailureClassification == VisionPipelineExpectedFailureContract.ControlledNoResultOutcome,
                "Sample service did not preserve the declared controlled no-result contract.");
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "expected-failure-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|expected-failure|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipelineExpectedFailureEvaluation Evaluate(
        VisionPipelineSampleCatalogItem sample,
        VisionRecipeRunResult run)
    {
        return VisionPipelineExpectedFailureContract.Evaluate(sample, run);
    }

    private static VisionPipelineSampleCatalogItem CreateSample(
        string outcome,
        string error,
        string failedStep)
    {
        return new VisionPipelineSampleCatalogItem
        {
            ValidationMode = "ExpectedFailure",
            ExpectedOutcome = outcome,
            ExpectedError = error,
            ExpectedFailedStep = failedStep
        };
    }

    private static VisionPipelineSampleCatalogItem FindProductSample(string sampleName)
    {
        VisionPipelineSampleCatalogItem sample = VisionPipelineSampleCatalogItem
            .LoadRunnable(VisionPipelineSampleCatalogSourceKind.Product)
            .FirstOrDefault(item => string.Equals(item.SampleName, sampleName, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Product sample '{sampleName}' was not found.");
        return sample;
    }

    private static VisionRecipeRunResult CreateRun(VisionRecipeStepRunSummary step)
    {
        return new VisionRecipeRunResult
        {
            Success = false,
            Steps = new List<VisionRecipeStepRunSummary> { step }
        };
    }

    private static VisionRecipeStepRunSummary CreateQualityNgStep()
    {
        return new VisionRecipeStepRunSummary
        {
            Index = 1,
            Name = "Acceptance",
            Status = "NG",
            ToolSuccess = true,
            Success = false,
            Executed = true,
            AcceptanceEvaluated = true,
            AcceptancePassed = false,
            ErrorCode = 0,
            ErrorName = "None"
        };
    }

    private static VisionRecipeStepRunSummary CreateNoResultStep(string errorName, int errorCode)
    {
        return new VisionRecipeStepRunSummary
        {
            Index = 1,
            Name = errorName == "MatchingNoResult" ? "Match" : "Invalid Tool",
            Status = "ERROR",
            ToolSuccess = false,
            Success = false,
            Executed = true,
            ErrorCode = errorCode,
            ErrorName = errorName
        };
    }

    private static void RunCase(string name, Action action, List<string> passed, List<string> failed)
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
}
