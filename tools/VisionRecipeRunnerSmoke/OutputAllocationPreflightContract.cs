using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class OutputAllocationPreflightContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();

        try
        {
            VerifyGuardCases(observations);
        }
        catch (Exception exception)
        {
            failures.Add("guard cases: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyNormalTransformsAsync(observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("normal transform regression: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyRunnerPreservesPriorResultAsync(observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("runner preservation: " + exception.GetBaseException().Message);
        }

        try
        {
            VerifyPreviewOwners(observations);
        }
        catch (Exception exception)
        {
            failures.Add("preview owner: " + exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(outputDirectory, "output-allocation-preflight-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-036 transform output allocation preflight",
                "Policy: checked output dimensions and width*height*element-bytes only; no inferred product memory cap or automatic downscale.",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations.Select(item => "PASS: " + item))
            .Concat(failures.Select(item => "FAIL: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Output allocation preflight contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Output allocation preflight contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static void VerifyGuardCases(ICollection<string> observations)
    {
        using Mat source = CreateSource();

        VisionPipelineOutputAllocationPreflightResult normalRotate =
            VisionPipelineOutputAllocationGuard.Validate(
                CreateRotateScaleStep(100D, 100D),
                source);
        Require(normalRotate.Success, "normal RotateScale was rejected: " + normalRotate.Message);
        Require(normalRotate.OutputWidth == source.Width && normalRotate.OutputHeight == source.Height,
            "normal RotateScale changed the output dimensions unexpectedly.");
        Require(normalRotate.EstimatedBytes == (long)source.Width * source.Height * source.ElemSize(),
            "normal RotateScale byte estimate was not checked from width*height*element-bytes.");
        observations.Add("normal RotateScale passed with the input dimensions and checked byte estimate.");

        VisionPipelineOutputAllocationPreflightResult nonFiniteRotate =
            VisionPipelineOutputAllocationGuard.Validate(
                CreateRotateScaleStep(double.NaN, 100D),
                source);
        Require(!nonFiniteRotate.Success && nonFiniteRotate.ErrorCode == VisionToolErrorCode.RotateScaleInvalidScale,
            "non-finite RotateScale scale was not rejected before native execution.");
        observations.Add("non-finite RotateScale scale was rejected with RotateScaleInvalidScale.");

        VisionPipelineOutputAllocationPreflightResult overflowRotate =
            VisionPipelineOutputAllocationGuard.Validate(
                CreateRotateScaleStep(double.MaxValue, 100D),
                source);
        Require(!overflowRotate.Success && overflowRotate.ErrorCode == VisionToolErrorCode.RotateScaleInvalidScale,
            "overflowing RotateScale output was not rejected before native execution.");
        observations.Add("RotateScale output beyond the native integer dimension range was rejected without allocation.");

        VisionPipelineOutputAllocationPreflightResult maximumAffine =
            VisionPipelineOutputAllocationGuard.Validate(
                CreateAffineStep(
                    VisionPipelineOutputAllocationGuard.MaximumAffineDimension,
                    VisionPipelineOutputAllocationGuard.MaximumAffineDimension),
                source);
        Require(maximumAffine.Success, "the existing supported Affine maximum was rejected: " + maximumAffine.Message);
        Require(maximumAffine.EstimatedBytes ==
                (long)VisionPipelineOutputAllocationGuard.MaximumAffineDimension
                * VisionPipelineOutputAllocationGuard.MaximumAffineDimension
                * source.ElemSize(),
            "maximum Affine byte estimate was not checked.");
        observations.Add("Affine 32768x32768 remains the supported maximum and is measured without allocating it.");

        VisionPipelineOutputAllocationPreflightResult oversizedAffine =
            VisionPipelineOutputAllocationGuard.Validate(
                CreateAffineStep(
                    VisionPipelineOutputAllocationGuard.MaximumAffineDimension + 1,
                    source.Height),
                source);
        Require(!oversizedAffine.Success && oversizedAffine.ErrorCode == VisionToolErrorCode.AffineInvalidOutputSize,
            "Affine output beyond the existing 32768 cap was not rejected before native execution.");
        observations.Add("Affine output beyond the existing 32768 dimension cap was rejected without allocation.");
    }

    private static async Task VerifyNormalTransformsAsync(ICollection<string> observations)
    {
        using Mat source = CreateSource();
        VisionRecipeRunner runner = new VisionRecipeRunner();

        VisionPipeline rotatePipeline = new VisionPipeline { Name = "2D-036 normal RotateScale" };
        rotatePipeline.Steps.Add(CreateRotateScaleStep(
            100D,
            100D,
            "NormalRotateScale",
            VisionRecipeRunner.DefaultInputLayer,
            "NormalRotateScaleOutput"));
        using VisionRecipeRunResult rotateRun = await runner.RunAsync(rotatePipeline, source).ConfigureAwait(false);
        Require(rotateRun.Success && rotateRun.ResultImage != null && !rotateRun.ResultImage.Empty(),
            "normal RotateScale runtime failed: " + rotateRun.Message);
        Require(rotateRun.ResultImage!.Width == source.Width && rotateRun.ResultImage.Height == source.Height,
            "normal RotateScale runtime changed the input dimensions unexpectedly.");

        VisionPipeline affinePipeline = new VisionPipeline { Name = "2D-036 normal Affine" };
        affinePipeline.Steps.Add(CreateAffineStep(
            source.Width,
            source.Height,
            "NormalAffine",
            VisionRecipeRunner.DefaultInputLayer,
            "NormalAffineOutput"));
        using VisionRecipeRunResult affineRun = await runner.RunAsync(affinePipeline, source).ConfigureAwait(false);
        Require(affineRun.Success && affineRun.ResultImage != null && !affineRun.ResultImage.Empty(),
            "normal Affine runtime failed: " + affineRun.Message);
        Require(affineRun.ResultImage!.Width == source.Width && affineRun.ResultImage.Height == source.Height,
            "normal Affine runtime changed the input dimensions unexpectedly.");

        observations.Add("normal RotateScale and Affine recipe executions completed with the source dimensions.");
    }

    private static async Task VerifyRunnerPreservesPriorResultAsync(ICollection<string> observations)
    {
        using Mat source = CreateSource();
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-036 Output Allocation Preflight" };
        pipeline.Steps.Add(
            VisionPipelineStepBuilder.FromThresholdProperty(
                new ThresholdToolProperty
                {
                    Mode = ThresholdToolMode.Threshold,
                    Threshold = 100,
                    MaxValue = 255,
                    ThresholdType = ThresholdTypes.Binary
                },
                "PriorResult",
                VisionRecipeRunner.DefaultInputLayer,
                "PriorResult"));
        pipeline.Steps.Add(CreateRotateScaleStep(double.MaxValue, 100D, "RejectedTransform", "PriorResult", "RejectedOutput"));

        using VisionRecipeRunResult run = await new VisionRecipeRunner().RunAsync(pipeline, source).ConfigureAwait(false);
        Require(!run.Success, "the impossible output pipeline unexpectedly succeeded.");
        Require(run.Steps.Count == 2, "the rejected transform did not retain both step summaries.");
        Require(run.Steps[0].Success && run.Steps[0].HasResultImage,
            "the prior successful output was not retained in the run summary.");
        Require(!run.Steps[1].Success
                && run.Steps[1].ErrorCode == (int)VisionToolErrorCode.RotateScaleInvalidScale
                && !run.Steps[1].HasResultImage,
            "the rejected transform did not fail before producing a result image.");
        Require(run.HasFinalResultImage
                && run.ResultImageWidth == source.Width
                && run.ResultImageHeight == source.Height,
            "the prior result image was not preserved after the rejected transform.");
        observations.Add("Recipe Run rejected the impossible transform before native execution and preserved the previous result layer/image.");
    }

    private static void VerifyPreviewOwners(ICollection<string> observations)
    {
        string root = FindRepositoryRoot();
        string affinePreview = File.ReadAllText(Path.Combine(
            root,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "NativeTools",
            "Preview",
            "OpenVisionNativeToolPreviewExecutor.cs"));
        string rotatePreview = File.ReadAllText(Path.Combine(
            root,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "NativeTools",
            "Preview",
            "OpenVisionNativeSimplePreprocessPreviewExecutor.cs"));
        Require(affinePreview.Contains("VisionPipelineOutputAllocationGuard.Validate(", StringComparison.Ordinal),
            "direct Affine Preview does not use the shared output preflight owner.");
        Require(rotatePreview.Contains("VisionPipelineOutputAllocationGuard.Validate(", StringComparison.Ordinal),
            "direct RotateScale Preview does not use the shared output preflight owner.");
        observations.Add("direct Affine and RotateScale Preview owners route through the same preflight guard; rendered WPF states remain a separate runtime boundary.");
    }

    private static VisionPipelineStep CreateRotateScaleStep(
        double scaleXPercent,
        double scaleYPercent,
        string name = "RotateScale",
        string inputLayer = "Main",
        string outputLayer = "RotateScaleOutput")
    {
        return VisionPipelineStepBuilder.FromRotateScaleProperty(
            new RotateScaleToolProperty
            {
                Angle = 0D,
                ScaleXPercent = scaleXPercent,
                ScaleYPercent = scaleYPercent,
                Interpolation = InterpolationFlags.Linear,
                BorderType = BorderTypes.Constant
            },
            name,
            inputLayer,
            outputLayer);
    }

    private static VisionPipelineStep CreateAffineStep(
        int outputWidth,
        int outputHeight,
        string name = "Affine",
        string inputLayer = "Main",
        string outputLayer = "AffineOutput")
    {
        return VisionPipelineStepBuilder.FromAffineTransformProperty(
            new AffineTransformProperty
            {
                SourcePoint1X = 0D,
                SourcePoint1Y = 0D,
                SourcePoint2X = 30D,
                SourcePoint2Y = 0D,
                SourcePoint3X = 0D,
                SourcePoint3Y = 30D,
                DestinationPoint1X = 0D,
                DestinationPoint1Y = 0D,
                DestinationPoint2X = 30D,
                DestinationPoint2Y = 0D,
                DestinationPoint3X = 0D,
                DestinationPoint3Y = 30D,
                OutputWidth = outputWidth,
                OutputHeight = outputHeight,
                MinimumSourceTriangleArea = 1D,
                MinimumDestinationTriangleArea = 1D
            },
            name,
            inputLayer,
            outputLayer);
    }

    private static Mat CreateSource()
    {
        Mat source = new Mat(48, 64, MatType.CV_8UC1, Scalar.All(128));
        Cv2.Rectangle(source, new Rect(8, 8, 24, 18), Scalar.White, -1);
        return source;
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "OpenVisionLab.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the OpenVisionLab repository root.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
