using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class PipelineFailureLifetimeContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();
        foreach (bool failOnNextStep in new[] { false, true })
        {
            List<Mat> observedImages = new();
            InvalidOperationException expected = new("Injected review callback failure");
            using Mat source = new(16, 16, MatType.CV_8UC1, Scalar.White);
            using VisionPipelineContext context = new();
            context.SetLayer("Main", source);
            try
            {
                await VisionPipelineExecutionService.RunAsync(CreatePipeline(), context, 1000, CancellationToken.None, update =>
                {
                    if (update.StepResult?.ToolResult?.ResultImage is Mat image)
                    {
                        observedImages.Add(image);
                        if (!failOnNextStep)
                            throw expected;
                    }
                    if (failOnNextStep && update.Status == "RUN" && update.Step.Name == "Second")
                        throw expected;
                });
                failed.Add("Callback failure did not propagate.");
            }
            catch (Exception exception)
            {
                Check(ReferenceEquals(exception, expected), "Original callback exception propagates", passed, failed);
                Check(observedImages.Count == 1 && observedImages.All(image => image.IsDisposed),
                    failOnNextStep ? "Later callback failure releases earlier results" : "Result callback failure releases its result", passed, failed);
                Check(!source.IsDisposed, "Caller-owned source survives callback failure", passed, failed);
            }
            finally
            {
                foreach (Mat image in observedImages)
                    image.Dispose();
            }
        }

        using (Mat source = new(16, 16, MatType.CV_8UC1, Scalar.White))
        using (VisionPipelineContext context = new())
        {
            context.SetLayer("Main", source);
            VisionPipelineRunResult result = await VisionPipelineExecutionService.RunAsync(CreatePipeline(), context, 1000, CancellationToken.None);
            try
            {
                Check(result.Success && result.StepResults.Count == 2
                    && result.StepResults.All(step => step.ToolResult.ResultImage is Mat image && !image.IsDisposed),
                    "Successful return transfers live results to the caller", passed, failed);
            }
            finally
            {
                foreach (VisionPipelineStepResult step in result.StepResults)
                    step.ToolResult?.ResultImage?.Dispose();
            }
        }
        string reportPath = Path.Combine(evidenceDirectory, "pipeline-failure-lifetime-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
            Console.WriteLine("FAIL|" + item);
        Console.WriteLine($"CONTRACT|pipeline-failure-lifetime|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipeline CreatePipeline()
    {
        VisionPipeline pipeline = new() { Name = "FailureLifetime" };
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "First", ToolType = "Threshold", Enabled = true,
            InputLayer = "Main", OutputLayer = "FirstOutput"
        });
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "Second", ToolType = "Threshold", Enabled = true,
            InputLayer = "FirstOutput", OutputLayer = "SecondOutput"
        });
        return pipeline;
    }

    private static void Check(bool condition, string message, List<string> passed, List<string> failed)
    {
        (condition ? passed : failed).Add(message);
    }
}
