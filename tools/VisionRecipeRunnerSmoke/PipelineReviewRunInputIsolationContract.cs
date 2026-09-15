using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class PipelineReviewRunInputIsolationContract
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
            using Bitmap source = CreateSourceImage();
            displayManager.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(new Bitmap(source)), "Main");

            using OpenVisionPipelineReviewExecutionController controller =
                new OpenVisionPipelineReviewExecutionController(displayManager, action => action());

            VisionPipeline producerPipeline = CreateProducerPipeline();
            OpenVisionPipelineReviewExecutionResult producerRun = await controller.RunAsync(
                producerPipeline,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                inputRevision: 1,
                recipeRevision: 1).ConfigureAwait(false);
            if (producerRun.WasSuperseded || producerRun.StepResultCount != 1
                || !controller.TryGetSummary(producerPipeline.Steps[0], out VisionPipelineStepResultSummary producerSummary)
                || !producerSummary.Success)
            {
                failures.Add("The A Run producer did not create a successful output result.");
            }

            using Bitmap producedOutput = controller.AcquireCachedOutputSnapshot(0, "A_Output");
            if (producedOutput == null)
            {
                failures.Add("The A Run output snapshot was not available for the stale-layer fixture.");
            }
            else
            {
                displayManager.CreateLayerDisplay(
                    ImageSpaceFrame.TakeOwnership(new Bitmap(producedOutput)),
                    "A_Output");
                observations.Add("A-Run-output: stale A_Output was retained in the workspace fixture");
            }

            controller.Reset();
            VisionPipeline deletedProducerPipeline = CreateConsumerPipeline("B_Deleted_Output", allowBranchInput: false);
            OpenVisionPipelineReviewExecutionResult deletedRun = await controller.RunAsync(
                deletedProducerPipeline,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                inputRevision: 2,
                recipeRevision: 2).ConfigureAwait(false);
            AssertMissingInput(
                deletedRun,
                controller,
                deletedProducerPipeline.Steps[0],
                "deleted producer",
                observations,
                failures);

            controller.Reset();
            VisionPipeline disabledProducerPipeline = CreateDisabledProducerPipeline();
            OpenVisionPipelineReviewExecutionResult disabledRun = await controller.RunAsync(
                disabledProducerPipeline,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                inputRevision: 3,
                recipeRevision: 3).ConfigureAwait(false);
            AssertMissingInput(
                disabledRun,
                controller,
                disabledProducerPipeline.Steps[1],
                "disabled producer",
                observations,
                failures);

            controller.Reset();
            VisionPipeline reorderedPipeline = CreateReorderedPipeline();
            OpenVisionPipelineReviewExecutionResult reorderedRun = await controller.RunAsync(
                reorderedPipeline,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                inputRevision: 4,
                recipeRevision: 4).ConfigureAwait(false);
            AssertMissingInput(
                reorderedRun,
                controller,
                reorderedPipeline.Steps[0],
                "reordered producer",
                observations,
                failures);

            controller.Reset();
            VisionPipeline explicitBranchPipeline = CreateConsumerPipeline("Branch_Output", allowBranchInput: true);
            OpenVisionPipelineReviewExecutionResult branchRun = await controller.RunAsync(
                explicitBranchPipeline,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                inputRevision: 5,
                recipeRevision: 5).ConfigureAwait(false);
            if (branchRun.WasSuperseded
                || branchRun.StepResultCount != 1
                || !controller.TryGetSummary(explicitBranchPipeline.Steps[0], out VisionPipelineStepResultSummary branchSummary)
                || !branchSummary.Success)
            {
                failures.Add("An explicitly allowed external branch could not consume A_Output.");
            }
            else
            {
                observations.Add("ExplicitBranch: PASS (ALLOW_BRANCH_INPUT can opt a prior output layer back in)");
            }
        }
        catch (Exception exception)
        {
            failures.Add(exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(outputDirectory, "pipeline-review-run-input-isolation-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-012 Pipeline Review run input isolation",
                "InputPolicy: current pipeline outputs and prior Review-produced outputs are not seeded as external inputs; ALLOW_BRANCH_INPUT is explicit opt-in",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Pipeline Review run input isolation contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Pipeline Review run input isolation contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static void AssertMissingInput(
        OpenVisionPipelineReviewExecutionResult run,
        OpenVisionPipelineReviewExecutionController controller,
        VisionPipelineStep step,
        string label,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        if (run.WasSuperseded
            || run.StepResultCount == 0
            || !controller.TryGetSummary(step, out VisionPipelineStepResultSummary summary)
            || summary.Success
            || summary.Status != "ERROR"
            || summary.Message.IndexOf("Input layer 'A_Output' has no image.", StringComparison.OrdinalIgnoreCase) < 0)
        {
            failures.Add($"The {label} case consumed the retained A_Output layer instead of failing closed.");
            return;
        }

        observations.Add($"{label}: PASS (A_Output was not seeded into the new Run context)");
    }

    private static VisionPipeline CreateProducerPipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-012 A Producer" };
        pipeline.Steps.Add(CreateArithmeticStep("Producer", "Main", "A_Output"));
        return pipeline;
    }

    private static VisionPipeline CreateConsumerPipeline(string outputLayer, bool allowBranchInput)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-012 B Consumer" };
        VisionPipelineStep step = CreateArithmeticStep("Consumer", "A_Output", outputLayer);
        if (allowBranchInput)
        {
            step.Parameters["ALLOW_BRANCH_INPUT"] = "true";
        }

        pipeline.Steps.Add(step);
        return pipeline;
    }

    private static VisionPipeline CreateDisabledProducerPipeline()
    {
        VisionPipeline pipeline = CreateConsumerPipeline("B_Disabled_Output", allowBranchInput: false);
        VisionPipelineStep disabledProducer = CreateArithmeticStep("Disabled Producer", "Main", "A_Output");
        disabledProducer.Enabled = false;
        pipeline.Steps.Insert(0, disabledProducer);
        return pipeline;
    }

    private static VisionPipeline CreateReorderedPipeline()
    {
        VisionPipeline pipeline = CreateConsumerPipeline("B_Reordered_Output", allowBranchInput: false);
        pipeline.Steps.Add(CreateArithmeticStep("Reordered Producer", "Main", "A_Output"));
        return pipeline;
    }

    private static VisionPipelineStep CreateArithmeticStep(string name, string inputLayer, string outputLayer)
    {
        return VisionPipelineStepBuilder.FromArithmetic(
            name,
            "ADD",
            inputLayer,
            "Main",
            outputLayer,
            useConstantInput: true,
            useColorConstant: false,
            gray: 10,
            b: 10,
            g: 10,
            r: 10,
            offsetX: 0,
            offsetY: 0);
    }

    private static Bitmap CreateSourceImage()
    {
        Bitmap source = new Bitmap(8, 8);
        source.SetPixel(0, 0, Color.White);
        return source;
    }
}
