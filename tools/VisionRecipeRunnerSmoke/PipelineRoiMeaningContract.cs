using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class PipelineRoiMeaningContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        VisionRecipeRunner runner = new VisionRecipeRunner();

        using Mat source = new Mat(new Size(8, 8), MatType.CV_8UC3, new Scalar(0, 255, 0));

        using (VisionRecipeRunResult unset = await runner.RunAsync(CreateHsvPipeline(false, "malformed"), source).ConfigureAwait(false))
        {
            if (!unset.Success
                || !TryGetMetric(unset, VisionPipelineKnownMetrics.MaskPixelCount, out double maskPixelCount)
                || maskPixelCount != 64D
                || unset.ResultImageWidth != 8
                || unset.ResultImageHeight != 8)
            {
                failures.Add("USE_ROI=false did not preserve full-image execution when CvROI was malformed.");
            }
            else
            {
                observations.Add("UnsetMalformedCvROI: PASS (full image preserved)");
            }
        }

        using (VisionRecipeRunResult valid = await runner.RunAsync(CreateHsvPipeline(true, "2,2,4,4"), source).ConfigureAwait(false))
        {
            if (!valid.Success
                || !TryGetMetric(valid, VisionPipelineKnownMetrics.MaskPixelCount, out double maskPixelCount)
                || maskPixelCount != 16D
                || !TryGetMetric(valid, VisionPipelineKnownMetrics.MaskPixelRatio, out double maskPixelRatio)
                || Math.Abs(maskPixelRatio - 1D) > 1e-9D)
            {
                failures.Add("A valid in-image ROI did not constrain HSV execution to the reviewed rectangle.");
            }
            else
            {
                observations.Add("ValidCvROI: PASS (4x4 ROI)");
            }
        }

        await AssertDefinitionRejectedAsync(
            runner,
            CreateHsvPipeline(true, "malformed"),
            source,
            "MalformedCvROI",
            "four comma-separated integers",
            observations,
            failures).ConfigureAwait(false);
        await AssertDefinitionRejectedAsync(
            runner,
            CreateHsvPipeline(true, "1,1,0,3"),
            source,
            "ZeroWidthCvROI",
            "greater than zero",
            observations,
            failures).ConfigureAwait(false);
        await AssertDefinitionRejectedAsync(
            runner,
            CreateHsvPipeline(true, "1,-1,2,2"),
            source,
            "NegativeCoordinateCvROI",
            "non-negative",
            observations,
            failures).ConfigureAwait(false);

        await AssertRuntimeRejectedAsync(
            runner,
            CreateHsvPipeline(true, "6,6,4,4"),
            source,
            "PartiallyOutOfBoundsCvROI",
            "outside the input image",
            observations,
            failures).ConfigureAwait(false);
        await AssertRuntimeRejectedAsync(
            runner,
            CreateHsvPipeline(true, "2147483647,0,1,1"),
            source,
            "OverflowRangeCvROI",
            "outside the input image",
            observations,
            failures).ConfigureAwait(false);

        AssertDirectToolRejection(source, "EdgeDetection", observations, failures);
        AssertDirectToolRejection(source, "HsvMask", observations, failures);

        VisionPipeline multiRoi = CreateBlobMultiRoiPipeline();
        VisionPipelineValidationResult multiRoiValidation = VisionPipelineValidator.Validate(
            multiRoi,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!multiRoiValidation.Success)
        {
            failures.Add("A valid multi-ROI definition was rejected by the shared validator: " + string.Join(" | ", multiRoiValidation.Errors));
        }
        else
        {
            observations.Add("MultiRoiDefinition: PASS (two valid regions retained)");
        }

        string reportPath = Path.Combine(outputDirectory, "pipeline-roi-meaning-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-010 ROI meaning boundaries",
                "Policy: Reject malformed, non-positive, negative-coordinate, and out-of-bounds ROI; USE_ROI=false remains full-image unset.",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Pipeline ROI meaning contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Pipeline ROI meaning contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task AssertDefinitionRejectedAsync(
        VisionRecipeRunner runner,
        VisionPipeline pipeline,
        Mat source,
        string caseName,
        string expectedText,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        try
        {
            using VisionRecipeRunResult _ = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
            failures.Add(caseName + " executed instead of being rejected by pre-validation.");
        }
        catch (VisionPipelineValidationException exception)
        {
            if (!exception.Errors.Any(error => error.IndexOf(expectedText, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                failures.Add(caseName + " returned an unexpected pre-validation reason: " + string.Join(" | ", exception.Errors));
            }
            else
            {
                observations.Add(caseName + ": PASS (pre-validation)");
            }
        }
    }

    private static async Task AssertRuntimeRejectedAsync(
        VisionRecipeRunner runner,
        VisionPipeline pipeline,
        Mat source,
        string caseName,
        string expectedText,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        using VisionRecipeRunResult result = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
        if (result.Success
            || !string.Equals(result.FirstFailedErrorName, "InvalidRoi", StringComparison.Ordinal)
            || result.Message.IndexOf(expectedText, StringComparison.OrdinalIgnoreCase) < 0)
        {
            failures.Add(caseName + " did not fail at runtime as InvalidRoi: " + result.SummaryText);
        }
        else
        {
            observations.Add(caseName + ": PASS (runtime bounds rejection)");
        }
    }

    private static void AssertDirectToolRejection(
        Mat source,
        string toolType,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipelineStep step = CreateHsvPipeline(true, "malformed").Steps[0];
        step.ToolType = toolType;
        IVisionTool tool = VisionPipelineAppToolFactory.Create(step);
        IDisposable? toolLifetime = tool as IDisposable;
        using VisionToolResult result = tool.Execute(source);
        bool rejected = !result.Success && string.Equals(result.ErrorName, "InvalidRoi", StringComparison.Ordinal);
        toolLifetime?.Dispose();
        if (!rejected)
        {
            failures.Add(toolType + " direct execution did not reject malformed CvROI as InvalidRoi.");
        }
        else
        {
            observations.Add(toolType + " direct malformed CvROI: PASS");
        }
    }

    private static bool TryGetMetric(VisionRecipeRunResult result, string name, out double value)
    {
        value = 0D;
        return result?.FinalStepSummary?.Metrics != null
            && result.FinalStepSummary.Metrics.TryGetValue(name, out value)
            && double.IsFinite(value);
    }

    private static VisionPipeline CreateHsvPipeline(bool useRoi, string roi)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-010 ROI Meaning" };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "HSV",
            ToolType = "HsvMask",
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = "HSV_Output"
        };
        step.Parameters["USE_ROI"] = useRoi.ToString();
        step.Parameters["CvROI"] = roi;
        step.Parameters["HueMin"] = "0";
        step.Parameters["HueMax"] = "179";
        step.Parameters["SaturationMin"] = "0";
        step.Parameters["SaturationMax"] = "255";
        step.Parameters["ValueMin"] = "0";
        step.Parameters["ValueMax"] = "255";
        pipeline.Steps.Add(step);
        return pipeline;
    }

    private static VisionPipeline CreateBlobMultiRoiPipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-010 Multi ROI" };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Blob",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = "Blob_Output"
        };
        step.Parameters["USE_ROI"] = "true";
        step.Parameters["USE_MULTI_ROI"] = "true";
        step.Parameters["CvROIS"] = "0,0,4,4;4,4,4,4";
        step.Parameters["USE_THRESHOLD"] = "false";
        step.Parameters["MIN_AREA"] = "1";
        step.Parameters["MAX_AREA"] = "64";
        pipeline.Steps.Add(step);
        return pipeline;
    }
}
