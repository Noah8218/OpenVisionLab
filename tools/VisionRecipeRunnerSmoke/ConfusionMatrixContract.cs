using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ConfusionMatrixContract
{
    public static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        RunCase("four-cell matrix separates execution error, not-run, and unknown label", () =>
        {
            List<VisionPipelineBatchSampleRunResult> results = new List<VisionPipelineBatchSampleRunResult>
            {
                CreateCompleted(expectedSuccess: true, actualSuccess: true),
                CreateCompleted(expectedSuccess: false, actualSuccess: false),
                CreateCompleted(expectedSuccess: false, actualSuccess: true),
                CreateCompleted(expectedSuccess: true, actualSuccess: false),
                CreateExecutionError(),
                CreateUnknownLabel()
            };
            VisionPipelineBatchConfusionMatrix matrix = VisionPipelineBatchOutcomeContract.BuildConfusionMatrix(results, 8);
            Require(matrix.InputSampleCount == 8, "Input sample denominator changed.");
            Require(matrix.TruePositiveCount == 1 && matrix.TrueNegativeCount == 1
                && matrix.FalsePositiveCount == 1 && matrix.FalseNegativeCount == 1,
                "The four confusion-matrix cells were not classified independently.");
            Require(matrix.ExecutionErrorCount == 1 && matrix.NotRunCount == 2 && matrix.UnknownLabelCount == 1,
                "Execution error, not-run, and unknown-label counts were merged.");
            Require(matrix.AccountedCount == 8 && matrix.IsBalanced,
                "Confusion-matrix counts do not reconcile to the input sample count.");
            Require(matrix.AccuracyText == "50.0%"
                && matrix.FalseAcceptRateText == "50.0%"
                && matrix.FalseRejectRateText == "50.0%",
                "Matrix rates did not use the expected denominators.");
        }, passed, failed);

        RunCase("zero denominators render N/A", () =>
        {
            VisionPipelineBatchConfusionMatrix matrix = VisionPipelineBatchOutcomeContract.BuildConfusionMatrix(
                Array.Empty<VisionPipelineBatchSampleRunResult>(),
                0);
            Require(matrix.AccountedCount == 0 && matrix.IsBalanced,
                "The empty matrix was not balanced.");
            Require(matrix.AccuracyText == "N/A"
                && matrix.FalseAcceptRateText == "N/A"
                && matrix.FalseRejectRateText == "N/A",
                "Zero-denominator rates were rendered as numeric percentages.");
        }, passed, failed);

        RunCase("legacy saved summary remains loadable and derives its denominator", () =>
        {
            string previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable) ?? string.Empty;
            string dataRoot = Path.Combine(evidenceDirectory, "runtime-data");
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);
            try
            {
                List<VisionPipelineBatchSampleRunResult> results = new List<VisionPipelineBatchSampleRunResult>
                {
                    CreateCompleted(expectedSuccess: true, actualSuccess: true),
                    CreateCompleted(expectedSuccess: false, actualSuccess: true)
                };
                string summaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    "ConfusionMatrixContractRecipe",
                    "Pipeline",
                    DateTime.Now,
                    DateTime.Now.AddMilliseconds(1),
                    results,
                    "Validation",
                    "LocalValidationSet",
                    "",
                    inputSampleCount: 3);
                VisionPipelineBatchRunSummary summary = VisionPipelineBatchRunSummaryStorage.Load(summaryPath);
                VisionPipelineBatchConfusionMatrix matrix = VisionPipelineBatchOutcomeContract.BuildConfusionMatrix(
                    summary?.Results,
                    summary?.InputSampleCount ?? 0);
                OpenVisionRecipeBatchRunOption option = OpenVisionRecipeBatchRunOption.Create(
                    new VisionPipelineBatchRunSummaryStorage.BatchRunSummaryInfo
                    {
                        SummaryPath = summaryPath,
                        StartedAt = DateTime.Now,
                        FinishedAt = DateTime.Now,
                        TotalCount = summary?.TotalCount ?? 0,
                        PassCount = summary?.PassCount ?? 0,
                        FailCount = summary?.FailCount ?? 0
                    });
                string projectionText = OpenVisionRecipeRunHistoryPresenter.BuildConfusionMatrixText(option);
                Require(summary != null && summary.InputSampleCount == 3
                    && matrix.FalsePositiveCount == 1
                    && matrix.NotRunCount == 1
                    && matrix.IsBalanced
                    && projectionText.Contains("TP 1", StringComparison.Ordinal)
                    && (projectionText.Contains("not run 1", StringComparison.OrdinalIgnoreCase)
                        || projectionText.Contains("미실행 1", StringComparison.Ordinal)),
                    "The persisted input denominator or run-history projection was not retained across save/reload.");
            }
            finally
            {
                Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, previousDataRoot);
            }
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "confusion-matrix-contract.txt");
        File.WriteAllLines(
            reportPath,
            passed.Select(item => "PASS: " + item)
                .Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|confusion-matrix|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipelineBatchSampleRunResult CreateCompleted(bool expectedSuccess, bool actualSuccess)
    {
        return new VisionPipelineBatchSampleRunResult
        {
            OutcomeSchemaVersion = VisionPipelineBatchOutcomeContract.CurrentVersion,
            ExecutionState = VisionPipelineBatchOutcomeContract.CompletedState,
            HasJudgment = true,
            ExpectedOutcome = VisionPipelineBatchOutcomeContract.ToOutcome(expectedSuccess),
            ActualOutcome = VisionPipelineBatchOutcomeContract.ToOutcome(actualSuccess),
            JudgmentCorrect = expectedSuccess == actualSuccess,
            Success = actualSuccess
        };
    }

    private static VisionPipelineBatchSampleRunResult CreateExecutionError()
    {
        return new VisionPipelineBatchSampleRunResult
        {
            OutcomeSchemaVersion = VisionPipelineBatchOutcomeContract.CurrentVersion,
            ExecutionState = VisionPipelineBatchOutcomeContract.ErrorState,
            HasJudgment = true,
            ExpectedOutcome = VisionPipelineBatchOutcomeContract.OkOutcome,
            Success = false
        };
    }

    private static VisionPipelineBatchSampleRunResult CreateUnknownLabel()
    {
        return new VisionPipelineBatchSampleRunResult
        {
            Success = true,
            SampleName = "unknown-label"
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
            failed.Add(name + ": " + exception.GetBaseException().Message);
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
