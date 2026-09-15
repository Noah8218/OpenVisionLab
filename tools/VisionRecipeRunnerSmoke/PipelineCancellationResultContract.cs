using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class PipelineCancellationResultContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();

        await RunCaseAsync("pre-canceled pipeline records a failed step", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            using VisionPipelineContext context = CreateContext(source);
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();

            List<VisionPipelineStepExecutionUpdate> updates = new();
            VisionPipelineRunResult result = await VisionPipelineExecutionService.RunAsync(
                CreatePipeline(),
                context,
                1000,
                cancellation.Token,
                updates.Add);
            try
            {
                VisionPipelineStepResult canceled = result.StepResults.Single();
                Require(!result.Success, "Pre-canceled run incorrectly reported success.");
                Require(!canceled.Success && canceled.ToolResult?.ErrorCode == VisionToolErrorCode.StepCanceled,
                    "Pre-canceled run did not retain a failed StepCanceled result.");
                Require(updates.Count == 1 && updates[0].Status == "CANCEL",
                    "Pre-canceled run did not publish a CANCEL result update.");
            }
            finally
            {
                result.Dispose();
            }
        }, passed, failed);

        await RunCaseAsync("public runner projects cancellation failure", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();

            using VisionRecipeRunResult result = await new VisionRecipeRunner().RunAsync(
                CreatePipeline(),
                source,
                cancellation.Token);

            Require(!result.Success
                && result.HasFailedStep
                && result.FirstFailedErrorCode == (int)VisionToolErrorCode.StepCanceled,
                "Public VisionRecipeRunner result did not project cancellation as a failed StepCanceled result.");
        }, passed, failed);

        await RunCaseAsync("cancellation between steps records the unexecuted step", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            using VisionPipelineContext context = CreateContext(source);
            using CancellationTokenSource cancellation = new();
            List<string> runSteps = new();

            VisionPipelineRunResult result = await VisionPipelineExecutionService.RunAsync(
                CreatePipeline(),
                context,
                1000,
                cancellation.Token,
                update =>
                {
                    if (update.Status == "RUN")
                    {
                        runSteps.Add(update.Step?.Name ?? string.Empty);
                    }

                    if (update.Step?.Name == "First" && update.StepResult != null)
                    {
                        cancellation.Cancel();
                    }
                });
            try
            {
                VisionPipelineStepResult canceled = result.StepResults.Last();
                Require(result.StepResults.Count == 2,
                    "Between-step cancellation did not retain both the completed and canceled Step results.");
                Require(result.StepResults[0].Success && canceled.ToolResult?.ErrorCode == VisionToolErrorCode.StepCanceled,
                    "Between-step cancellation did not preserve the first success and second StepCanceled result.");
                Require(!canceled.Success && !result.Success,
                    "Between-step cancellation incorrectly reported overall success.");
                Require(runSteps.SequenceEqual(new[] { "First" }),
                    "The canceled second step was executed instead of stopping before its RUN update.");
                using Mat secondOutput = context.GetLayer("SecondOutput");
                Require(secondOutput == null,
                    "The canceled second step wrote an output layer.");
            }
            finally
            {
                result.Dispose();
            }
        }, passed, failed);

        await RunCaseAsync("cancellation cannot satisfy an expected-failure acceptance", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            using VisionPipelineContext context = CreateContext(source);
            using CancellationTokenSource cancellation = new();

            VisionPipelineRunResult result = await VisionPipelineExecutionService.RunAsync(
                CreatePipeline(secondExpectedSuccess: false),
                context,
                1000,
                cancellation.Token,
                update =>
                {
                    if (update.Step?.Name == "First" && update.StepResult != null)
                    {
                        cancellation.Cancel();
                    }
                });
            try
            {
                VisionPipelineStepResult canceled = result.StepResults.Last();
                Require(canceled.ToolResult?.ErrorCode == VisionToolErrorCode.StepCanceled
                    && !canceled.AcceptancePassed
                    && !canceled.Success
                    && !result.Success,
                    "ExpectedSuccess=false converted cancellation into an accepted result.");
            }
            finally
            {
                result.Dispose();
            }
        }, passed, failed);

        await RunCaseAsync("uncanceled pipeline keeps its successful result", async () =>
        {
            using Mat source = new(32, 32, MatType.CV_8UC1, Scalar.White);
            using VisionPipelineContext context = CreateContext(source);
            using VisionPipelineRunResult result = await VisionPipelineExecutionService.RunAsync(
                CreatePipeline(),
                context,
                1000,
                CancellationToken.None);

            Require(result.Success && result.StepResults.Count == 2
                && result.StepResults.All(step => step.Success),
                "An uncanceled two-step pipeline no longer reports success.");
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "pipeline-cancellation-result-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|pipeline-cancellation-result|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static async Task RunCaseAsync(
        string name,
        Func<Task> action,
        List<string> passed,
        List<string> failed)
    {
        try
        {
            await action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add($"{name}: {exception.GetBaseException().Message}");
        }
    }

    private static VisionPipelineContext CreateContext(Mat source)
    {
        VisionPipelineContext context = new();
        context.SetLayer("Main", source);
        return context;
    }

    private static VisionPipeline CreatePipeline(bool secondExpectedSuccess = true)
    {
        VisionPipeline pipeline = new() { Name = "CancellationResult" };
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "First",
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "FirstOutput"
        });
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "Second",
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = "FirstOutput",
            OutputLayer = "SecondOutput",
            UseAcceptance = !secondExpectedSuccess,
            ExpectedSuccess = secondExpectedSuccess
        });
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
