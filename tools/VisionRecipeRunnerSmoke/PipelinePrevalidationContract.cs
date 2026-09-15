using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Common;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class PipelinePrevalidationContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        VisionRecipeRunner runner = new VisionRecipeRunner();

        using Mat source = new Mat(new Size(64, 64), MatType.CV_8UC1, Scalar.White);
        using (VisionRecipeRunResult validResult = await runner.RunAsync(CreateThresholdPipeline(), source).ConfigureAwait(false))
        {
            if (!validResult.Success || validResult.Steps.Count != 1)
            {
                failures.Add("A valid object pipeline was rejected or did not execute one step.");
            }
            else
            {
                observations.Add("ValidObjectPipeline: PASS");
            }
        }

        VisionPipeline invalidTool = CreateThresholdPipeline();
        invalidTool.Steps[0].ToolType = "FutureUnsupportedTool";
        await AssertRejectedAsync(
            runner,
            invalidTool,
            source,
            "UnsupportedToolType",
            observations,
            failures).ConfigureAwait(false);

        VisionPipeline missingLayer = CreateThresholdPipeline();
        missingLayer.Steps[0].InputLayer = "MissingLayer";
        await AssertRejectedAsync(
            runner,
            missingLayer,
            source,
            "MissingInputLayer",
            observations,
            failures).ConfigureAwait(false);

        VisionPipeline invalidParameter = CreateThresholdPipeline();
        invalidParameter.Steps[0].Parameters["Threshold"] = "not-a-number";
        await AssertRejectedAsync(
            runner,
            invalidParameter,
            source,
            "InvalidParameter",
            observations,
            failures).ConfigureAwait(false);

        VisionPipeline invalidAcceptance = CreateThresholdPipeline();
        VisionPipelineStep acceptanceStep = invalidAcceptance.Steps[0];
        acceptanceStep.UseAcceptance = true;
        acceptanceStep.UseAcceptanceMetricMinimum = true;
        acceptanceStep.UseAcceptanceMetricMaximum = true;
        acceptanceStep.AcceptanceMetricMinimum = 10;
        acceptanceStep.AcceptanceMetricMaximum = 1;
        await AssertRejectedAsync(
            runner,
            invalidAcceptance,
            source,
            "InvalidAcceptance",
            observations,
            failures).ConfigureAwait(false);

        string invalidXmlPath = Path.Combine(outputDirectory, "unsupported-tool.xml");
        if (!SerializeHelper.SaveXmlFile(invalidXmlPath, invalidTool))
        {
            failures.Add("Could not save the invalid XML fixture.");
        }
        else
        {
            VisionPipelineValidationResult expected = VisionPipelineValidator.Validate(
                invalidTool,
                new[] { VisionRecipeRunner.DefaultInputLayer });
            try
            {
                using VisionRecipeRunResult _ = await runner.RunAsync(
                    invalidXmlPath,
                    source,
                    VisionRecipeRunner.DefaultInputLayer,
                    VisionRecipeRunner.DefaultStepTimeoutMilliseconds).ConfigureAwait(false);
                failures.Add("The invalid XML runner path executed instead of rejecting pre-validation.");
            }
            catch (VisionPipelineValidationException exception)
            {
                if (!exception.Errors.SequenceEqual(expected.Errors, StringComparer.Ordinal))
                {
                    failures.Add("The invalid XML runner path did not preserve the shared validator errors.");
                }
                else
                {
                    observations.Add("ValidatingXMLRunnerPath: PASS");
                }
            }
        }

        string reportPath = Path.Combine(outputDirectory, "pipeline-prevalidation-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-009 UI/shared Validator and public VisionRecipeRunner pre-validation",
                "TypedException: VisionPipelineValidationException",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Pipeline pre-validation contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Pipeline pre-validation contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task AssertRejectedAsync(
        VisionRecipeRunner runner,
        VisionPipeline pipeline,
        Mat source,
        string caseName,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipelineValidationResult expected = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (expected.Success || expected.Errors.Count == 0)
        {
            failures.Add(caseName + " fixture was not rejected by the shared UI validator.");
            return;
        }

        try
        {
            using VisionRecipeRunResult _ = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
            failures.Add(caseName + " was executed instead of rejected before native execution.");
        }
        catch (VisionPipelineValidationException exception)
        {
            if (!exception.Errors.SequenceEqual(expected.Errors, StringComparer.Ordinal))
            {
                failures.Add(caseName + " returned a different typed validation reason than the shared validator.");
            }
            else
            {
                observations.Add(caseName + ": PASS");
            }
        }
    }

    private static VisionPipeline CreateThresholdPipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-009 Prevalidation" };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Threshold",
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = "Threshold_Output"
        };
        step.Parameters["Mode"] = "Threshold";
        step.Parameters["Threshold"] = "127";
        step.Parameters["MaxValue"] = "255";
        step.Parameters["ThresholdType"] = ThresholdTypes.Binary.ToString();
        pipeline.Steps.Add(step);
        return pipeline;
    }
}
