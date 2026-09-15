using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class PipelineReviewDuplicateStepIdentityContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();

        try
        {
            using DisplayManagerService displayManager = new DisplayManagerService();
            Bitmap source = new Bitmap(8, 8);
            source.SetPixel(0, 0, Color.White);
            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.TakeOwnership(source),
                "Main");

            VisionPipeline pipeline = CreateDuplicateRoutePipeline(includeAcceptanceFailure: true);
            using OpenVisionPipelineReviewExecutionController controller =
                new OpenVisionPipelineReviewExecutionController(displayManager, action => action());

            OpenVisionPipelineReviewExecutionResult run = await controller.RunAsync(
                pipeline,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                inputRevision: 1,
                recipeRevision: 1).ConfigureAwait(false);
            Require(!run.WasSuperseded, "The duplicate-step Review run was unexpectedly superseded.");
            if (run.StepResultCount != 2)
            {
                failures.Add($"The duplicate-step Review run retained {run.StepResultCount} results instead of 2.");
            }

            bool hasFirstSummary = controller.TryGetSummary(pipeline.Steps[0], out VisionPipelineStepResultSummary firstSummary);
            bool hasSecondSummary = controller.TryGetSummary(pipeline.Steps[1], out VisionPipelineStepResultSummary secondSummary);
            if (!hasFirstSummary)
            {
                failures.Add("The first duplicate Step did not resolve to a retained summary.");
            }
            else if (firstSummary.Index != 1)
            {
                failures.Add($"First duplicate Step resolved to summary index {firstSummary.Index}.");
            }
            else
            {
                observations.Add($"FirstSummary: {firstSummary.Status} / {firstSummary.Message}");
            }

            if (!hasSecondSummary)
            {
                failures.Add("The second duplicate Step did not resolve to a retained summary.");
            }
            else if (secondSummary.Index != 2)
            {
                failures.Add($"Second duplicate Step resolved to summary index {secondSummary.Index}.");
            }
            else
            {
                observations.Add($"SecondSummary: {secondSummary.Status} / {secondSummary.Message}");
            }

            if (hasFirstSummary && hasSecondSummary)
            {
                Require(firstSummary.Metrics.ContainsKey("OutputWidth"),
                    "First duplicate Step lost its metric identity.");
                Require(secondSummary.Metrics.ContainsKey("OutputWidth"),
                    "Second duplicate Step lost its metric identity.");
                Require(secondSummary.IsAcceptanceNg && !string.IsNullOrWhiteSpace(secondSummary.AcceptanceMessage),
                    "Second duplicate Step did not retain its NG acceptance reason.");
                observations.Add("SummaryMapping: PASS (same name/tool/input/output resolved by run-scoped index)");
                observations.Add("MetricAndNgMapping: PASS (metric and acceptance reason stayed on Step 2)");
            }

            using Bitmap firstOutput = controller.AcquireCachedOutputSnapshot(0, "Shared_Output");
            using Bitmap secondOutput = controller.AcquireCachedOutputSnapshot(1, "Shared_Output");
            if (firstOutput == null || secondOutput == null)
            {
                failures.Add("Duplicate-step output snapshots were not retained separately.");
            }
            else if (firstOutput.GetPixel(7, 7).ToArgb() == secondOutput.GetPixel(7, 7).ToArgb())
            {
                failures.Add("Duplicate-step output snapshots collapsed to the same image.");
            }
            else
            {
                observations.Add($"OutputPixels: {firstOutput.GetPixel(7, 7).ToArgb()} -> {secondOutput.GetPixel(7, 7).ToArgb()}");
                observations.Add("OutputMapping: PASS (same output route retained both step images)");
            }

            using Bitmap latestOutput = controller.AcquireCachedOutputSnapshot("Shared_Output");
            if (latestOutput == null)
            {
                failures.Add("Legacy layer-name output snapshot lookup returned no image.");
            }
            else if (secondOutput == null || latestOutput.GetPixel(7, 7).ToArgb() != secondOutput.GetPixel(7, 7).ToArgb())
            {
                failures.Add("Legacy layer-name lookup did not select the latest run-scoped output.");
            }
            else
            {
                observations.Add("LegacyLookup: PASS (layer-only lookup remains latest-output compatible)");
            }

            await AssertReorderedDuplicatePipelineAsync(displayManager, observations, failures).ConfigureAwait(false);
            await AssertDisabledDuplicatePipelineAsync(displayManager, observations, failures).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add(exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(outputDirectory, "pipeline-review-duplicate-step-identity-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-011 duplicate Step identity and identical route mapping",
                "IdentityPolicy: run-scoped pipeline Step index; no name/tool/layer first-match fallback",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Pipeline Review duplicate Step identity contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Pipeline Review duplicate Step identity contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task AssertReorderedDuplicatePipelineAsync(
        DisplayManagerService displayManager,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipeline pipeline = CreateDuplicateRoutePipeline(includeAcceptanceFailure: false);
        VisionPipelineStep first = pipeline.Steps[0];
        pipeline.Steps[0] = pipeline.Steps[1];
        pipeline.Steps[1] = first;
        using OpenVisionPipelineReviewExecutionController controller =
            new OpenVisionPipelineReviewExecutionController(displayManager, action => action());

        OpenVisionPipelineReviewExecutionResult run = await controller.RunAsync(
            pipeline,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            inputRevision: 2,
            recipeRevision: 2).ConfigureAwait(false);
        if (run.StepResultCount != 2
            || !controller.TryGetSummary(pipeline.Steps[0], out VisionPipelineStepResultSummary firstSummary)
            || !controller.TryGetSummary(pipeline.Steps[1], out VisionPipelineStepResultSummary secondSummary)
            || firstSummary.Index != 1
            || secondSummary.Index != 2)
        {
            failures.Add("Reordered duplicate Steps did not retain index-scoped summaries.");
            return;
        }

        observations.Add("ReorderMapping: PASS (reordering preserved current run index identity)");
    }

    private static async Task AssertDisabledDuplicatePipelineAsync(
        DisplayManagerService displayManager,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipeline pipeline = CreateDuplicateRoutePipeline(includeAcceptanceFailure: false);
        pipeline.Steps[0].Enabled = false;
        using OpenVisionPipelineReviewExecutionController controller =
            new OpenVisionPipelineReviewExecutionController(displayManager, action => action());

        OpenVisionPipelineReviewExecutionResult run = await controller.RunAsync(
            pipeline,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            inputRevision: 3,
            recipeRevision: 3).ConfigureAwait(false);
        if (run.StepResultCount != 2
            || !controller.TryGetSummary(pipeline.Steps[0], out VisionPipelineStepResultSummary skippedSummary)
            || !controller.TryGetSummary(pipeline.Steps[1], out VisionPipelineStepResultSummary enabledSummary)
            || !skippedSummary.Skipped
            || enabledSummary.Index != 2)
        {
            failures.Add("Disabled duplicate Step did not preserve the disabled row and enabled Step identity.");
            return;
        }

        observations.Add("DisabledMapping: PASS (disabled row and later duplicate retained separate identities)");
    }

    private static VisionPipeline CreateDuplicateRoutePipeline(bool includeAcceptanceFailure)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-011 Duplicate Step Identity" };
        pipeline.Steps.Add(VisionPipelineStepBuilder.FromArithmetic(
            "Same Step",
            "ADD",
            "Main",
            "Main",
            "Shared_Output",
            useConstantInput: true,
            useColorConstant: false,
            gray: 10,
            b: 10,
            g: 10,
            r: 10,
            offsetX: 0,
            offsetY: 0));
        VisionPipelineStep secondStep = VisionPipelineStepBuilder.FromArithmetic(
            "Same Step",
            "ADD",
            "Main",
            "Main",
            "Shared_Output",
            useConstantInput: true,
            useColorConstant: false,
            gray: 100,
            b: 100,
            g: 100,
            r: 100,
            offsetX: 0,
            offsetY: 0);
        if (includeAcceptanceFailure)
        {
            secondStep.UseAcceptance = true;
            secondStep.AcceptanceMetricName = "OutputWidth";
            secondStep.UseAcceptanceMetricMinimum = true;
            secondStep.AcceptanceMetricMinimum = 9;
        }

        pipeline.Steps.Add(secondStep);
        return pipeline;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
