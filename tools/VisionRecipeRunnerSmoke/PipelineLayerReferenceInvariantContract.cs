using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class PipelineLayerReferenceInvariantContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        VisionPipeline baseline = CreateReferenceGraph();

        try
        {
            AssertValid(baseline, "BaselineGraph", observations, failures);

            string baselineXml = Path.Combine(outputDirectory, "baseline-reference-graph.xml");
            VisionPipeline roundTripped = ClonePipeline(baseline, baselineXml);
            AssertValid(roundTripped, "XmlRoundTrip", observations, failures);
            AssertReferenceParametersPreserved(baseline, roundTripped, observations, failures);

            ExpectInvalid(
                baseline,
                outputDirectory,
                "delete-serial-producer",
                pipeline => RemoveStep(pipeline, "Serial Producer"),
                "input layer 'Serial_A' does not exist",
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "reorder-serial-consumer-before-producer",
                pipeline => SwapSteps(pipeline, "Serial Producer", "Serial Consumer"),
                "input layer 'Serial_A' does not exist",
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "rename-layer-without-reference-update",
                pipeline => FindStep(pipeline, "Serial Producer").OutputLayer = "Serial_Renamed",
                "input layer 'Serial_A' does not exist",
                observations,
                failures);
            ExpectValid(
                baseline,
                outputDirectory,
                "rename-layer-with-explicit-reference-update",
                pipeline =>
                {
                    FindStep(pipeline, "Serial Producer").OutputLayer = "Serial_Renamed";
                    FindStep(pipeline, "Serial Consumer").InputLayer = "Serial_Renamed";
                },
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "branch-route-without-explicit-repair",
                pipeline => FindStep(pipeline, "Branch Threshold").InputLayer = "Missing_Branch",
                "input layer 'Missing_Branch' does not exist",
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "arithmetic-b-layer-removed",
                pipeline => FindStep(pipeline, "Arithmetic B").Parameters[VisionPipelineArithmeticStep.ParameterInputLayerB] = "Removed_B",
                "input layer B 'Removed_B' does not exist",
                observations,
                failures);
            ExpectValid(
                baseline,
                outputDirectory,
                "arithmetic-b-layer-explicitly-restored",
                pipeline => FindStep(pipeline, "Arithmetic B").Parameters[VisionPipelineArithmeticStep.ParameterInputLayerB] = "Main",
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "delete-fixture-producer",
                pipeline => RemoveStep(pipeline, "Line Fixture"),
                "fixture frame 'Frame' must be published",
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "rename-fixture-source-without-reference-update",
                pipeline => FindStep(pipeline, "Point A").Name = "Point A Renamed",
                "source Step 'Point A'",
                observations,
                failures);
            ExpectValid(
                baseline,
                outputDirectory,
                "rename-fixture-source-with-explicit-reference-update",
                pipeline =>
                {
                    FindStep(pipeline, "Point A").Name = "Point A Renamed";
                    FindStep(pipeline, "Line Fixture").Parameters[VisionPipelineLineFixtureService.SourceStepAParameter] = "Point A Renamed";
                    FindStep(pipeline, "Affine Detected Points").Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] = "Point A Renamed/Start";
                },
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "duplicate-fixture-source-step",
                pipeline =>
                {
                    int index = IndexOfStep(pipeline, "Point A");
                    pipeline.Steps.Insert(index + 1, CloneStep(FindStep(pipeline, "Point A")));
                },
                "ambiguous",
                observations,
                failures);
            ExpectInvalid(
                baseline,
                outputDirectory,
                "reorder-affine-source-after-consumer",
                pipeline =>
                {
                    VisionPipelineStep pointC = RemoveStep(pipeline, "Point C");
                    int affineIndex = IndexOfStep(pipeline, "Affine Detected Points");
                    pipeline.Steps.Insert(affineIndex + 1, pointC);
                },
                "source Step 'Point C' is not an earlier enabled Step",
                observations,
                failures);
            ExpectValid(
                baseline,
                outputDirectory,
                "reorder-affine-source-with-explicit-restore",
                pipeline =>
                {
                    VisionPipelineStep pointC = RemoveStep(pipeline, "Point C");
                    pipeline.Steps.Insert(IndexOfStep(pipeline, "Line Fixture"), pointC);
                },
                observations,
                failures);

            AssertNoAutomaticReferenceRepair(baseline, outputDirectory, observations, failures);
            AssertCloneCancelPreservesOriginal(baseline, outputDirectory, observations, failures);
            await AssertArithmeticLayerBExecutionAsync(observations, failures).ConfigureAwait(false);
            await AssertInPlaceLayerPreservesSourceAsync(observations, failures).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add(exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(outputDirectory, "pipeline-layer-reference-invariant-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-015 Layer/Step reference invariants after delete, duplicate, reorder, rename, XML round-trip, and cancel/undo discard",
                "Owner: VisionPipelineValidator + existing VisionPipelineLineFixtureService/VisionPipelineAffinePointBindingService + SerializeHelper",
                "Policy: broken references fail closed; reference repair is explicit; no guessed reconnect",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Pipeline layer/reference invariant contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Pipeline layer/reference invariant contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static VisionPipeline CreateReferenceGraph()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-015 Layer Reference Invariants" };
        pipeline.Steps.Add(CreateThresholdStep("Serial Producer", "Main", "Serial_A"));
        pipeline.Steps.Add(CreateThresholdStep("Serial Consumer", "Serial_A", "Serial_B"));

        VisionPipelineStep arithmetic = VisionPipelineStepBuilder.FromArithmetic(
            "Arithmetic B",
            "ADD",
            "Serial_B",
            "Main",
            "Arithmetic_Out",
            useConstantInput: false,
            useColorConstant: false,
            gray: 0,
            b: 0,
            g: 0,
            r: 0,
            offsetX: 0,
            offsetY: 0);
        arithmetic.Enabled = true;
        pipeline.Steps.Add(arithmetic);

        VisionPipelineStep branch = CreateThresholdStep("Branch Threshold", "Main", "Branch_Out");
        branch.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        pipeline.Steps.Add(branch);

        pipeline.Steps.Add(CreateLineStep("Point A", "Point_A"));
        pipeline.Steps.Add(CreateLineStep("Point B", "Point_B"));
        pipeline.Steps.Add(CreateLineStep("Point C", "Point_C"));

        VisionPipelineStep fixture = CreateLineFixtureStep();
        pipeline.Steps.Add(fixture);

        VisionPipelineStep fixtureConsumer = CreateThresholdStep("Fixture Consumer", "Main", "Fixture_Adjusted");
        fixtureConsumer.Parameters[VisionPipelineFixtureFrameService.ConsumeParameter] = "true";
        fixtureConsumer.Parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = "Frame";
        fixtureConsumer.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        fixtureConsumer.Parameters["USE_ROI"] = "true";
        fixtureConsumer.Parameters["CvROI"] = "1,1,10,10";
        pipeline.Steps.Add(fixtureConsumer);

        pipeline.Steps.Add(CreateAffineDetectedPointStep());
        return pipeline;
    }

    private static VisionPipelineStep CreateThresholdStep(string name, string inputLayer, string outputLayer)
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = name,
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = inputLayer,
            OutputLayer = outputLayer
        };
        step.Parameters["Mode"] = "Threshold";
        step.Parameters["Threshold"] = "127";
        step.Parameters["MaxValue"] = "255";
        step.Parameters["ThresholdType"] = "Binary";
        return step;
    }

    private static VisionPipelineStep CreateLineStep(string name, string outputLayer)
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = name,
            ToolType = "Line",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = outputLayer
        };
        step.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        return step;
    }

    private static VisionPipelineStep CreateLineFixtureStep()
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Line Fixture",
            ToolType = "LineFixture",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Fixture_Frame"
        };
        step.Parameters[VisionPipelineLineFixtureService.SourceStepAParameter] = "Point A";
        step.Parameters[VisionPipelineLineFixtureService.SourceFeatureAParameter] = "Segment";
        step.Parameters[VisionPipelineLineFixtureService.SourceStepBParameter] = "Point B";
        step.Parameters[VisionPipelineLineFixtureService.SourceFeatureBParameter] = "Segment";
        step.Parameters[VisionPipelineFixtureFrameService.PublishParameter] = "true";
        step.Parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = "Frame";
        step.Parameters[VisionPipelineFixtureFrameService.ReferenceXParameter] = "0";
        step.Parameters[VisionPipelineFixtureFrameService.ReferenceYParameter] = "0";
        step.Parameters[VisionPipelineFixtureFrameService.ReferenceAngleParameter] = "0";
        step.Parameters[VisionPipelineFixtureFrameService.ReferenceScaleParameter] = "1";
        step.Parameters[VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter] = "2";
        step.Parameters[VisionPipelineFixtureFrameService.MinimumScaleRatioParameter] = "0.5";
        step.Parameters[VisionPipelineFixtureFrameService.MaximumScaleRatioParameter] = "2";
        step.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        return step;
    }

    private static VisionPipelineStep CreateAffineDetectedPointStep()
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Affine Detected Points",
            ToolType = "AffineTransform",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Affine_Out"
        };
        step.Parameters[VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter] = "true";
        step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] = "Point A/Start";
        step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] = "Point B/Start";
        step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] = "Point C/Start";
        step.Parameters["SourcePoint1X"] = "0";
        step.Parameters["SourcePoint1Y"] = "0";
        step.Parameters["SourcePoint2X"] = "100";
        step.Parameters["SourcePoint2Y"] = "0";
        step.Parameters["SourcePoint3X"] = "0";
        step.Parameters["SourcePoint3Y"] = "100";
        step.Parameters["DestinationPoint1X"] = "0";
        step.Parameters["DestinationPoint1Y"] = "0";
        step.Parameters["DestinationPoint2X"] = "100";
        step.Parameters["DestinationPoint2Y"] = "0";
        step.Parameters["DestinationPoint3X"] = "0";
        step.Parameters["DestinationPoint3Y"] = "100";
        step.Parameters["OutputWidth"] = "128";
        step.Parameters["OutputHeight"] = "128";
        step.Parameters["MinimumSourceTriangleArea"] = "1";
        step.Parameters["MinimumDestinationTriangleArea"] = "1";
        step.Parameters["MinimumValidPixelRatio"] = "0.25";
        step.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        return step;
    }

    private static async Task AssertArithmeticLayerBExecutionAsync(
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-015 Arithmetic B execution" };
        VisionPipelineStep step = VisionPipelineStepBuilder.FromArithmetic(
            "Arithmetic B runtime",
            "ADD",
            "Main",
            "Main",
            "Arithmetic_Result",
            useConstantInput: false,
            useColorConstant: false,
            gray: 0,
            b: 0,
            g: 0,
            r: 0,
            offsetX: 0,
            offsetY: 0);
        step.Enabled = true;
        pipeline.Steps.Add(step);

        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            failures.Add("Arithmetic B runtime fixture was rejected: " + string.Join(" | ", validation.Errors));
            return;
        }

        using Mat source = new Mat(new OpenCvSharp.Size(16, 16), MatType.CV_8UC1, Scalar.All(10));
        VisionRecipeRunner runner = new VisionRecipeRunner();
        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
        if (!run.Success || run.Steps.Count != 1)
        {
            failures.Add("Arithmetic B runtime did not execute the valid Main/Main graph: " + run.Message);
        }
        else
        {
            observations.Add("ArithmeticBExecution: PASS (valid B layer admitted and executed)");
        }
    }

    private static async Task AssertInPlaceLayerPreservesSourceAsync(
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-037 In-place source preservation" };
        VisionPipelineStep step = VisionPipelineStepBuilder.FromArithmetic(
            "Arithmetic InPlace",
            "ADD",
            "Main",
            "Main",
            "Main",
            useConstantInput: false,
            useColorConstant: false,
            gray: 0,
            b: 0,
            g: 0,
            r: 0,
            offsetX: 0,
            offsetY: 0);
        step.Enabled = true;
        pipeline.Steps.Add(step);

        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            failures.Add("In-place source preservation fixture was rejected: " + string.Join(" | ", validation.Errors));
            return;
        }

        using Mat source = new Mat(new OpenCvSharp.Size(16, 16), MatType.CV_8UC1, Scalar.All(10));
        using Mat original = source.Clone();
        VisionRecipeRunner runner = new VisionRecipeRunner();
        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
        if (!run.Success || run.ResultImage == null || run.ResultImage.Empty())
        {
            failures.Add("In-place source preservation run did not produce a result: " + run.Message);
            return;
        }

        if (Cv2.Norm(source, original, NormTypes.INF) != 0)
        {
            failures.Add("In-place pipeline execution mutated the source Mat.");
            return;
        }

        run.ResultImage.SetTo(Scalar.All(250));
        if (Cv2.Norm(source, original, NormTypes.INF) != 0)
        {
            failures.Add("In-place pipeline result shares storage with the source Mat.");
            return;
        }

        observations.Add(
            "InPlaceSourcePreservation: PASS (same-layer Run preserved source bytes and returned independent result; validation warnings="
            + validation.Warnings.Count + ")");
    }

    private static void AssertNoAutomaticReferenceRepair(
        VisionPipeline baseline,
        string outputDirectory,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipeline candidate = ClonePipeline(
            baseline,
            Path.Combine(outputDirectory, "no-auto-reconnect.xml"));
        VisionPipelineStep consumer = FindStep(candidate, "Serial Consumer");
        consumer.InputLayer = "Broken_Link";
        IReadOnlyList<VisionPipelineNormalizationChange> changes = VisionPipelineNormalizer.NormalizeForRun(candidate);
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            candidate,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (changes.Any(change => change.Properties.Any(property => string.Equals(property.PropertyName, "InputLayer", StringComparison.OrdinalIgnoreCase)))
            || string.Equals(consumer.InputLayer, "Serial_A", StringComparison.OrdinalIgnoreCase)
            || validation.Success)
        {
            failures.Add("Broken serial reference was automatically reconnected instead of remaining fail-closed.");
        }
        else
        {
            observations.Add("NoAutomaticReferenceRepair: PASS (normalizer left broken reference untouched)");
        }
    }

    private static void AssertCloneCancelPreservesOriginal(
        VisionPipeline baseline,
        string outputDirectory,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        string originalName = FindStep(baseline, "Serial Producer").OutputLayer;
        int originalCount = baseline.Steps.Count;
        VisionPipeline editSession = ClonePipeline(
            baseline,
            Path.Combine(outputDirectory, "cancel-edit-session.xml"));
        RemoveStep(editSession, "Serial Producer");
        FindStep(editSession, "Point A").Name = "Point A Cancelled Edit";

        VisionPipelineValidationResult originalValidation = VisionPipelineValidator.Validate(
            baseline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!originalValidation.Success
            || baseline.Steps.Count != originalCount
            || !string.Equals(FindStep(baseline, "Serial Producer").OutputLayer, originalName, StringComparison.Ordinal))
        {
            failures.Add("Discarding a mutated edit graph changed the original pipeline graph.");
        }
        else
        {
            observations.Add("CancelUndoGraph: PASS (discarded clone did not mutate original graph)");
        }
    }

    private static void AssertReferenceParametersPreserved(
        VisionPipeline expected,
        VisionPipeline actual,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        string[] keys =
        {
            VisionPipelineArithmeticStep.ParameterInputLayerB,
            VisionPipelineLineFixtureService.SourceStepAParameter,
            VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter
        };
        foreach (string key in keys)
        {
            string expectedValue = FindParameter(expected, key);
            string actualValue = FindParameter(actual, key);
            if (!string.Equals(expectedValue, actualValue, StringComparison.Ordinal))
            {
                failures.Add($"XML round-trip changed {key} from '{expectedValue}' to '{actualValue}'.");
            }
        }

        if (failures.Count == 0)
        {
            observations.Add("XmlRoundTripReferences: PASS (serial, Arithmetic B, fixture, and affine references preserved)");
        }
    }

    private static void AssertValid(
        VisionPipeline pipeline,
        string caseName,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            failures.Add(caseName + " was rejected: " + string.Join(" | ", validation.Errors));
            return;
        }

        observations.Add(caseName + ": PASS (warnings=" + validation.Warnings.Count + ")");
    }

    private static void ExpectInvalid(
        VisionPipeline baseline,
        string outputDirectory,
        string caseName,
        Action<VisionPipeline> mutation,
        string expectedErrorFragment,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        try
        {
            VisionPipeline candidate = ClonePipeline(
                baseline,
                Path.Combine(outputDirectory, caseName + ".xml"));
            mutation(candidate);
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
                candidate,
                new[] { VisionRecipeRunner.DefaultInputLayer });
            if (validation.Success)
            {
                failures.Add(caseName + " unexpectedly passed validation.");
            }
            else if (!validation.Errors.Any(error => error.Contains(expectedErrorFragment, StringComparison.OrdinalIgnoreCase)))
            {
                failures.Add(caseName + " failed for an unexpected reason: " + string.Join(" | ", validation.Errors));
            }
            else
            {
                observations.Add(caseName + ": PASS (fail-closed: " + validation.Errors.First() + ")");
            }
        }
        catch (Exception exception)
        {
            failures.Add(caseName + " threw: " + exception.GetBaseException().Message);
        }
    }

    private static void ExpectValid(
        VisionPipeline baseline,
        string outputDirectory,
        string caseName,
        Action<VisionPipeline> mutation,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        try
        {
            VisionPipeline candidate = ClonePipeline(
                baseline,
                Path.Combine(outputDirectory, caseName + ".xml"));
            mutation(candidate);
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
                candidate,
                new[] { VisionRecipeRunner.DefaultInputLayer });
            if (!validation.Success)
            {
                failures.Add(caseName + " was rejected after explicit reference update: " + string.Join(" | ", validation.Errors));
            }
            else
            {
                observations.Add(caseName + ": PASS (explicit reference update restored validity)");
            }
        }
        catch (Exception exception)
        {
            failures.Add(caseName + " threw: " + exception.GetBaseException().Message);
        }
    }

    private static VisionPipeline ClonePipeline(VisionPipeline source, string xmlPath)
    {
        if (!SerializeHelper.SaveXmlFile(xmlPath, source))
        {
            throw new InvalidOperationException("Could not save pipeline XML: " + xmlPath);
        }

        if (!SerializeHelper.TryLoadFromXmlFile(xmlPath, out VisionPipeline clone) || clone == null)
        {
            throw new InvalidOperationException("Could not reload pipeline XML: " + xmlPath);
        }

        return clone;
    }

    private static VisionPipelineStep FindStep(VisionPipeline pipeline, string name)
    {
        VisionPipelineStep? step = pipeline.Steps.FirstOrDefault(item =>
            item != null && string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        return step ?? throw new InvalidOperationException("Step not found: " + name);
    }

    private static int IndexOfStep(VisionPipeline pipeline, string name)
    {
        int index = pipeline.Steps.FindIndex(item =>
            item != null && string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            throw new InvalidOperationException("Step not found: " + name);
        }

        return index;
    }

    private static VisionPipelineStep RemoveStep(VisionPipeline pipeline, string name)
    {
        int index = IndexOfStep(pipeline, name);
        VisionPipelineStep removed = pipeline.Steps[index];
        pipeline.Steps.RemoveAt(index);
        return removed;
    }

    private static void SwapSteps(VisionPipeline pipeline, string firstName, string secondName)
    {
        int firstIndex = IndexOfStep(pipeline, firstName);
        int secondIndex = IndexOfStep(pipeline, secondName);
        VisionPipelineStep first = pipeline.Steps[firstIndex];
        pipeline.Steps[firstIndex] = pipeline.Steps[secondIndex];
        pipeline.Steps[secondIndex] = first;
    }

    private static VisionPipelineStep CloneStep(VisionPipelineStep source)
    {
        VisionPipelineStep clone = new VisionPipelineStep
        {
            Name = source.Name,
            ToolType = source.ToolType,
            Enabled = source.Enabled,
            InputLayer = source.InputLayer,
            OutputLayer = source.OutputLayer,
            UseAcceptance = source.UseAcceptance,
            ExpectedSuccess = source.ExpectedSuccess,
            MaxElapsedMilliseconds = source.MaxElapsedMilliseconds,
            RequiredMessageText = source.RequiredMessageText,
            AcceptanceMetricName = source.AcceptanceMetricName,
            UseAcceptanceMetricMinimum = source.UseAcceptanceMetricMinimum,
            AcceptanceMetricMinimum = source.AcceptanceMetricMinimum,
            UseAcceptanceMetricMaximum = source.UseAcceptanceMetricMaximum,
            AcceptanceMetricMaximum = source.AcceptanceMetricMaximum
        };
        foreach (KeyValuePair<string, string> parameter in source.Parameters)
        {
            clone.Parameters[parameter.Key] = parameter.Value;
        }

        return clone;
    }

    private static string FindParameter(VisionPipeline pipeline, string key)
    {
        foreach (VisionPipelineStep step in pipeline.Steps)
        {
            if (step != null && step.Parameters.TryGetValue(key, out string? value))
            {
                return value ?? string.Empty;
            }
        }

        return string.Empty;
    }
}
