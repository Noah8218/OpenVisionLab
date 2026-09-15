using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Pipeline.Controls;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class PipelineNotRunTailStatusContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();

        await RunCaseAsync("failure tail projects all five steps without extra NG", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            VisionPipeline pipeline = CreatePipeline(includeAcceptanceFailure: true, includeDisabledStep: true);
            using VisionRecipeRunResult result = await new VisionRecipeRunner().RunAsync(
                pipeline,
                source,
                CancellationToken.None).ConfigureAwait(false);

            Require(result.Steps.Count == 5, "Failure run did not project all five planned steps.");
            Require(result.Steps[0].ExecutionState == VisionPipelineResultSummaryService.ExecutedState
                && result.Steps[0].Executed
                && result.Steps[0].Success,
                "The first executed step lost its executed/pass state.");
            Require(result.Steps[1].ExecutionState == VisionPipelineResultSummaryService.ExecutedState
                && result.Steps[1].Status == "NG"
                && result.Steps[1].ToolSuccess
                && !result.Steps[1].AcceptancePassed,
                "The acceptance NG step was not retained as the actual failure.");
            Require(result.Steps[2].ExecutionState == VisionPipelineResultSummaryService.DisabledState
                && result.Steps[2].Skipped
                && !result.Steps[2].Executed,
                "The disabled step was not kept distinct from the failed tail.");
            Require(result.Steps.Skip(3).All(step =>
                    step.ExecutionState == VisionPipelineResultSummaryService.NotRunAfterFailureState
                    && step.Status == "NOT RUN"
                    && !step.Executed
                    && !step.Success),
                "The failed tail was not projected as NotRunAfterFailure.");
            Require(result.FailedStepCount == 1
                && result.SkippedStepCount == 1
                && result.FirstFailedStepIndex == 2
                && result.FinalStepSummary?.Index == 2,
                "Synthetic tail rows changed failure counts or final executed-step ownership.");
            Require(result.Steps.Skip(3).All(step => !step.HasResultImage && step.ObjectResults.Count == 0),
                "Synthetic tail rows acquired image or object ownership.");
        }, passed, failed);

        await RunCaseAsync("cancellation tail remains separate from failure tail", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            using VisionPipelineContext context = new();
            context.SetLayer("Main", source);
            using CancellationTokenSource cancellation = new();
            VisionPipeline pipeline = CreatePipeline(includeAcceptanceFailure: false, includeDisabledStep: false);
            List<string> runSteps = new();
            using VisionPipelineRunResult result = await VisionPipelineExecutionService.RunAsync(
                pipeline,
                context,
                1000,
                cancellation.Token,
                update =>
                {
                    if (update?.Status == "RUN")
                    {
                        runSteps.Add(update.Step?.Name ?? string.Empty);
                    }

                    if (update?.StepResult != null && update.Step?.Name == "Step 1")
                    {
                        cancellation.Cancel();
                    }
                }).ConfigureAwait(false);

            List<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(
                pipeline,
                result);
            Require(result.StepResults.Count == 2 && runSteps.SequenceEqual(new[] { "Step 1" }),
                "Cancellation did not stop before the next step execution.");
            Require(summaries.Count == 5
                && summaries[1].ExecutionState == VisionPipelineResultSummaryService.CancelledState
                && summaries[1].Executed
                && summaries.Skip(2).All(summary =>
                    summary.ExecutionState == VisionPipelineResultSummaryService.CancelledState
                    && !summary.Executed
                    && summary.Status == "CANCEL"),
                "Cancelled current/tail steps were not distinguished from failure tail steps.");
            Require(summaries.Skip(2).All(summary => !summary.HasResultImage && summary.ObjectResultCount == 0),
                "Cancelled synthetic tail rows acquired result ownership.");
        }, passed, failed);

        await RunCaseAsync("review flow and progress exclude synthetic tail from NG", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            using VisionPipelineContext context = new();
            context.SetLayer("Main", source);
            VisionPipeline pipeline = CreatePipeline(includeAcceptanceFailure: true, includeDisabledStep: true);
            using VisionPipelineRunResult result = await VisionPipelineExecutionService.RunAsync(
                pipeline,
                context,
                1000,
                CancellationToken.None).ConfigureAwait(false);
            List<VisionPipelineStepResultSummary> summaries = VisionPipelineResultSummaryService.CreateStepSummaries(
                pipeline,
                result);
            Dictionary<VisionPipelineStep, VisionPipelineStepResultSummary> byStep = pipeline.Steps
                .Select((step, index) => new { step, summary = summaries[index] })
                .ToDictionary(item => item.step, item => item.summary);

            OpenVisionPipelineReviewResultStatusProjectionOwner progressOwner =
                new OpenVisionPipelineReviewResultStatusProjectionOwner();
            string progress = progressOwner.ProjectProgress(
                pipeline.Steps,
                step => byStep[step],
                isRunning: false,
                isStopping: false);
            OpenVisionPipelineReviewFlowProjection tailProjection = OpenVisionPipelineReviewFlowPresenter.CreateStepProjection(
                pipeline.Steps,
                3,
                hasInputImage: false,
                hasOutputImage: false,
                summaries[3]);

            Require(progress.Contains("NG 1", StringComparison.Ordinal)
                && (progress.Contains("WAIT 2", StringComparison.Ordinal)
                    || progress.Contains("대기 2", StringComparison.Ordinal))
                && progress.Contains("OFF 1", StringComparison.Ordinal),
                $"Review progress counted the unexecuted tail as NG: {progress}");
            Require(tailProjection.Status == PipelineFlowStepStatus.Waiting
                && tailProjection.StatusText == "NOT RUN",
                "Review flow rendered an unexecuted tail as a failed step.");
            Require(OpenVisionPipelineReviewResultPresenter.FormatResultSummary(
                    pipeline.Steps[3],
                    summaries[3]).Contains("Not run", StringComparison.OrdinalIgnoreCase),
                "Review result presenter did not explain the unexecuted tail.");
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "pipeline-not-run-tail-status-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|pipeline-not-run-tail-status|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipeline CreatePipeline(bool includeAcceptanceFailure, bool includeDisabledStep)
    {
        VisionPipeline pipeline = new() { Name = "NotRunTailStatus" };
        pipeline.Steps.Add(CreateStep("Step 1", "Main", "Layer 1"));
        VisionPipelineStep failureStep = CreateStep("Step 2", "Layer 1", "Layer 2");
        failureStep.UseAcceptance = includeAcceptanceFailure;
        failureStep.ExpectedSuccess = !includeAcceptanceFailure;
        pipeline.Steps.Add(failureStep);
        VisionPipelineStep disabledStep = CreateStep("Step 3", "Main", "Layer 3");
        disabledStep.Enabled = !includeDisabledStep;
        pipeline.Steps.Add(disabledStep);
        pipeline.Steps.Add(CreateStep("Step 4", "Main", "Layer 4"));
        pipeline.Steps.Add(CreateStep("Step 5", "Main", "Layer 5"));
        return pipeline;
    }

    private static VisionPipelineStep CreateStep(string name, string inputLayer, string outputLayer)
    {
        return new VisionPipelineStep
        {
            Name = name,
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = inputLayer,
            OutputLayer = outputLayer
        };
    }

    private static async Task RunCaseAsync(
        string name,
        Func<Task> action,
        List<string> passed,
        List<string> failed)
    {
        try
        {
            await action().ConfigureAwait(false);
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
