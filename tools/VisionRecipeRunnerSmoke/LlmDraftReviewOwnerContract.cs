using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class LlmDraftReviewOwnerContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "ovl45-llm-draft-review-owner-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        string recipeName = "Smoke_LlmDraftReview_" + Guid.NewGuid().ToString("N")[..10];
        const string activePipelineName = "ActivePipeline";
        const string draftPipelineName = "DraftPipeline";
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "LLM draft review owner evidence must be on D:.");

            string dataRoot = Path.Combine(evidenceDirectory, "data");
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                dataRoot);

            VisionPipeline activePipeline = CreatePipeline(activePipelineName, "Threshold");
            VisionPipelineStorage.Save(recipeName, activePipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, activePipelineName);

            VisionPipeline draftPipeline = CreatePipeline(draftPipelineName, "Blob");
            OpenVisionRecipeLlmDraftReviewOwner owner = new OpenVisionRecipeLlmDraftReviewOwner();
            OpenVisionRecipeLlmDraftReview review = owner.Build(recipeName, draftPipeline);
            Require(ContainsAny(review.ImportReviewText, "Current active: ActivePipeline", "현재 활성: ActivePipeline"),
                "The owner did not compare the draft against the persisted active Pipeline.");
            Require(ContainsAny(review.ImportReviewText, "Draft: DraftPipeline", "초안: DraftPipeline"),
                "The import review did not identify the draft Pipeline.");
            Require(ContainsAny(review.DiffReviewText, "Baseline: ActivePipeline", "비교 기준: ActivePipeline"),
                "The diff review did not identify the active baseline.");
            Require(ContainsAny(review.DiffReviewText, "Draft: DraftPipeline", "초안: DraftPipeline"),
                "The diff review did not identify the draft Pipeline.");
            passed.Add("active Pipeline lookup and read-only import/diff review projection");

            VisionPipeline persisted = VisionPipelineStorage.Load(recipeName, activePipelineName);
            Require(string.Equals(persisted?.Name, activePipelineName, StringComparison.Ordinal),
                "The review owner changed the persisted active Pipeline.");
            Require(string.Equals(
                    VisionPipelineStorage.LoadActivePipelineName(
                        recipeName,
                        VisionPipelineAppendService.DefaultPipelineName),
                    activePipelineName,
                    StringComparison.OrdinalIgnoreCase),
                "The review owner changed the active Pipeline selection.");
            passed.Add("review path preserved Recipe persistence and active selection");

            OpenVisionRecipeLlmDraftReview missingDraftReview = owner.Build(recipeName, null!);
            Require(ContainsAny(missingDraftReview.ImportReviewText, "Draft import review", "초안 가져오기 검토"),
                "The null draft path did not return the existing safe review text.");
            passed.Add("null draft returned a safe read-only review result");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                previousDataRoot);
        }

        string outputPath = Path.Combine(evidenceDirectory, "llm-draft-review-owner-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-45 LLM draft review owner",
                "EvidenceDirectory: " + evidenceDirectory,
                "RecipeName: " + recipeName
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            "CONTRACT|llm-draft-review-owner|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipeline CreatePipeline(string name, string toolType)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = name };
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = name + " step",
            ToolType = toolType,
            InputLayer = "Main",
            OutputLayer = name + "Output",
            Enabled = true
        });
        return pipeline;
    }

    private static bool ContainsAny(string value, params string[] expectedValues)
    {
        return expectedValues.Any(expected => value?.Contains(expected, StringComparison.Ordinal) == true);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
