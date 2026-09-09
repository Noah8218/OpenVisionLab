using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class RecipeRunHistoryOrchestrationContract
{
    internal static int Run(string requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable);
        string recipeName = "Smoke_RunHistory_" + Guid.NewGuid().ToString("N")[..12];
        const string pipelineName = "HistoryPipeline";
        List<string> passed = new();
        List<string> failed = new();

        try
        {
            Require(string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
                "Run History contract evidence must be on D:.");
            string dataRoot = Path.Combine(evidenceDirectory, "data", recipeName);
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);
            Require(string.Equals(
                    Path.GetFullPath(AppPathService.DataRootDirectory),
                    Path.GetFullPath(dataRoot),
                    StringComparison.OrdinalIgnoreCase),
                "AppPathService was initialized before the isolated Run History data-root override.");

            DateTime baselineStart = DateTime.Now.AddMinutes(-2);
            DateTime currentStart = DateTime.Now.AddMinutes(-1);
            string baselinePath = VisionPipelineBatchRunSummaryStorage.Save(
                recipeName,
                pipelineName,
                baselineStart,
                baselineStart.AddSeconds(1),
                new[] { CreateResult("sample-1", true, "baseline") },
                "History contract",
                "Catalog");
            string currentPath = VisionPipelineBatchRunSummaryStorage.Save(
                recipeName,
                pipelineName,
                currentStart,
                currentStart.AddSeconds(1),
                new[] { CreateResult("sample-1", false, "current") },
                "History contract",
                "Catalog");

            OpenVisionRecipeRunHistoryOrchestrationOwner owner =
                new OpenVisionRecipeRunHistoryOrchestrationOwner();
            OpenVisionRecipeRunHistorySelection recent = owner.BuildRecentRunSelection(
                recipeName,
                pipelineName,
                currentPath);
            Require(recent.Options.Count == 2, "Recent Run History did not load both saved summaries.");
            Require(string.Equals(recent.SelectedOption?.SummaryPath, currentPath, StringComparison.OrdinalIgnoreCase),
                "Recent Run History did not preserve the previously selected summary.");
            passed.Add("recent inventory and selected summary");

            OpenVisionRecipeRunHistorySelection baseline = owner.BuildBaselineRunSelection(
                recent.SelectedOption,
                recent.Options,
                string.Empty);
            Require(string.Equals(baseline.SelectedOption?.SummaryPath, baselinePath, StringComparison.OrdinalIgnoreCase),
                "Baseline selection did not resolve the adjacent prior run.");
            passed.Add("baseline fallback selection");

            OpenVisionRecipeRunHistoryComparison comparison = owner.BuildComparison(
                recent.SelectedOption,
                baseline.SelectedOption,
                recent.Options);
            Require(string.Equals(comparison.Baseline?.SummaryPath, baselinePath, StringComparison.OrdinalIgnoreCase),
                "Comparison did not retain the resolved baseline option.");
            Require(comparison.Rows.Count == 1
                && comparison.Rows[0] != null
                && comparison.Rows[0].SampleName.IndexOf("sample-1", StringComparison.OrdinalIgnoreCase) >= 0,
                "Comparison rows did not load the persisted sample identity: count="
                + comparison.Rows.Count
                + ", name="
                + (comparison.Rows.FirstOrDefault()?.SampleName ?? "<null>"));
            Require(comparison.SelectedRow != null, "Comparison did not select a review row.");
            passed.Add("summary load and comparison projection");

            OpenVisionRecipeRunHistorySelection empty = owner.BuildRecentRunSelection(
                recipeName + "_missing",
                pipelineName,
                string.Empty);
            Require(empty.Options.Count == 1 && string.IsNullOrWhiteSpace(empty.Options[0].SummaryPath),
                "Empty Run History did not retain the existing placeholder option.");
            passed.Add("empty inventory placeholder");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, previousDataRoot);
        }

        string outputPath = Path.Combine(evidenceDirectory, "recipe-run-history-orchestration-contract.txt");
        File.WriteAllLines(
            outputPath,
            passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine("CONTRACT|recipe-run-history-orchestration|passed=" + passed.Count + "|failed=" + failed.Count);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipelineBatchSampleRunResult CreateResult(
        string sampleName,
        bool success,
        string runLabel)
    {
        return new VisionPipelineBatchSampleRunResult
        {
            SampleName = sampleName,
            Status = success ? "OK" : "NG",
            OutcomeSchemaVersion = 1,
            ExecutionState = "Completed",
            HasJudgment = true,
            ExpectedOutcome = "OK",
            ActualOutcome = success ? "OK" : "NG",
            JudgmentCorrect = success,
            Success = success,
            Message = runLabel,
            SampleImagePath = sampleName + ".bmp",
            VariantId = "Default"
        };
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
