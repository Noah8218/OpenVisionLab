using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

internal static class PipelineStepEditContract
{
    public static int Run(string evidenceDirectory)
    {
        string root = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(root);
        List<string> observations = new List<string>();
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            foreach (string culture in new[] { "ko-KR", "en-US" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                VisionPipeline baseline = CreatePipeline();
                string original = ToXml(baseline);
                Assert(VisionPipelineStepEditService.Validate(baseline).Success, "baseline is valid");

                VisionPipelineStep inserted = Step("입력 복사", "Main", "Independent");
                VisionPipelineStepEditResult insert = VisionPipelineStepEditService.Insert(baseline, 1, inserted);
                Assert(insert.Validation.Success && insert.Pipeline.Steps[1].Name == inserted.Name, "insert at exact position");
                insert.Pipeline.Steps[1].Parameters["Threshold"] = "42";
                Assert(inserted.Parameters["Threshold"] == "127", "inserted step is a deep copy");
                RoundTrip(root, culture + "-insert", insert.Pipeline);

                VisionPipelineStep replacement = Step("수정한 검사", "Main", "C");
                replacement.UseAcceptance = true;
                replacement.AcceptanceMetricMinimum = 5;
                replacement.Parameters["PreservedExtension"] = "extension-value";
                VisionPipelineStepEditResult replaced = VisionPipelineStepEditService.Replace(baseline, 0, replacement);
                Assert(replaced.Pipeline.Steps[0].Name == replacement.Name && replaced.Pipeline.Steps[0].AcceptanceMetricMinimum == 5
                    && replaced.AffectedIndex == 0 && replaced.SelectedIndex == 0, "replace retains metadata and edit indexes");
                Assert(!replaced.Validation.Success, "replace reports a broken downstream input without repairing it");
                replaced.Pipeline.Steps[0].Parameters["PreservedExtension"] = "changed";
                Assert(replacement.Parameters["PreservedExtension"] == "extension-value", "replacement parameters are detached");
                RoundTrip(root, culture + "-replace", replaced.Pipeline);

                VisionPipelineStepEditResult moved = VisionPipelineStepEditService.Move(baseline, 1, 0);
                Assert(!moved.Validation.Success && moved.Pipeline.Steps[0].InputLayer == "A", "move does not repair broken input");
                Assert(moved.AffectedIndex == 0 && moved.SelectedIndex == 0, "move indexes identify affected and selected rows");
                RoundTrip(root, culture + "-move", moved.Pipeline);

                VisionPipelineStepEditResult deleted = VisionPipelineStepEditService.Delete(baseline, 0, new[] { "Main", "A", "B" });
                Assert(!deleted.Validation.Success && deleted.Validation.Errors.Any(error => error.Contains("input layer 'A'", StringComparison.Ordinal)), "delete rejects cached output as source");
                RoundTrip(root, culture + "-delete", deleted.Pipeline);

                VisionPipelineStepEditResult disabled = VisionPipelineStepEditService.SetEnabled(baseline, 0, false);
                Assert(!disabled.Validation.Success && !disabled.Pipeline.Steps[0].Enabled, "disabled producer breaks consumer");
                VisionPipelineStepEditResult enabled = VisionPipelineStepEditService.SetEnabled(disabled.Pipeline, 0, true);
                Assert(enabled.Validation.Success, "re-enable recovers validation");
                RoundTrip(root, culture + "-enabled", enabled.Pipeline);

                VisionPipelineStepEditResult duplicate = VisionPipelineStepEditService.Duplicate(baseline, 0);
                Assert(duplicate.Pipeline.Steps[1].Name == "검사_2", "duplicate uses shared suffix rule");
                Assert(!duplicate.Validation.Success && duplicate.Pipeline.Steps[1].OutputLayer == "A", "duplicate does not silently rewrite output");
                duplicate.Pipeline.Steps[1].Parameters["Threshold"] = "99";
                Assert(duplicate.Pipeline.Steps[0].Parameters["Threshold"] == "127", "duplicate parameters are independent");
                RoundTrip(root, culture + "-duplicate", duplicate.Pipeline);

                VisionPipelineStepEditResult renamed = VisionPipelineStepEditService.Rename(baseline, 1, "검사");
                Assert(renamed.Pipeline.Steps[1].Name == "검사_2" && renamed.Validation.Success, "rename resolves duplicate case-insensitively");
                RoundTrip(root, culture + "-rename", renamed.Pipeline);
                VisionPipeline named = VisionPipelineStepEditService.Copy(baseline);
                named.Steps[0].Name = "Threshold";
                named.Steps[1].Name = "threshold_2";
                VisionPipelineStepEditResult suffix = VisionPipelineStepEditService.Insert(named, 2, Step("THRESHOLD", "Main", "C"));
                Assert(suffix.Pipeline.Steps[2].Name == "THRESHOLD_3", "existing numbered and differently cased names retain suffix rule");
                Assert(VisionPipelineStepEditService.Rename(baseline, 1, "").Pipeline.Steps[1].Name == "Threshold", "blank name uses tool type as Append does");

                Stack<VisionPipeline> undo = new Stack<VisionPipeline>();
                undo.Push(VisionPipelineStepEditService.Copy(baseline));
                VisionPipeline restored = undo.Pop();
                Assert(ToXml(restored) == original && VisionPipelineStepEditService.Validate(restored).Success, "caller-owned undo restores XML and validation");
                Assert(ToXml(baseline) == original, "all edits leave input pipeline unchanged");
                restored.Steps[0].Parameters["Threshold"] = "1";
                Assert(ToXml(baseline) == original, "undo snapshot is independent");

                VisionPipeline mainOutput = VisionPipelineStepEditService.Copy(baseline);
                mainOutput.Steps[0].OutputLayer = "main";
                Assert(!VisionPipelineStepEditService.Validate(mainOutput).Success, "Main output is rejected");
                VisionPipeline emptyOutput = VisionPipelineStepEditService.Copy(baseline);
                emptyOutput.Steps[0].OutputLayer = "";
                Assert(!VisionPipelineStepEditService.Validate(emptyOutput).Success, "empty output is rejected");
                VisionPipeline singleStep = new VisionPipeline { Name = "Empty candidate" };
                singleStep.Steps.Add(Step("Only", "Main", "OnlyOut"));
                VisionPipelineStepEditResult lastDeleted = VisionPipelineStepEditService.Delete(singleStep, 0);
                Assert(lastDeleted.SelectedIndex == -1 && !lastDeleted.Validation.Success, "deleting last step yields non-runnable empty working copy");

                VisionPipeline arithmetic = CreatePipeline();
                arithmetic.Steps.Add(VisionPipelineStepBuilder.FromArithmetic("연산", "ADD", "B", "A", "Sum", false, false, 0, 0, 0, 0, 0, 0));
                Assert(VisionPipelineStepEditService.Validate(arithmetic).Success, "arithmetic baseline");
                VisionPipelineStepEditResult missingB = VisionPipelineStepEditService.Delete(arithmetic, 0);
                Assert(missingB.Validation.Errors.Any(error => error.Contains("input layer B 'A'", StringComparison.Ordinal)), "B producer deletion fails closed");

                VisionPipeline fixture = CreateFixturePipeline();
                Assert(VisionPipelineStepEditService.Validate(fixture).Success, "fixture baseline");
                VisionPipelineStepEditResult missingFrame = VisionPipelineStepEditService.Delete(fixture, 0);
                Assert(missingFrame.Validation.Errors.Any(error => error.Contains("fixture frame 'Frame'", StringComparison.Ordinal)), "fixture producer deletion fails closed");

                AssertThrows<ArgumentOutOfRangeException>(() => VisionPipelineStepEditService.Move(baseline, -1, 0));
                AssertThrows<ArgumentOutOfRangeException>(() => VisionPipelineStepEditService.Insert(baseline, 9, inserted));
                AssertThrows<ArgumentOutOfRangeException>(() => VisionPipelineStepEditService.Replace(baseline, -1, replacement));
                AssertThrows<ArgumentOutOfRangeException>(() => VisionPipelineStepEditService.Replace(baseline, baseline.Steps.Count, replacement));
                AssertThrows<ArgumentNullException>(() => VisionPipelineStepEditService.Replace(baseline, 0, null!));
                AssertThrows<ArgumentNullException>(() => VisionPipelineStepEditService.Copy(null!));
                Assert(ToXml(baseline) == original, "rejected operations leave source intact");
                observations.Add(culture + ": insert/replace/move/delete/duplicate/rename/enable, metadata, deep copy, XML, names, undo, Main/empty/B/fixture/arguments PASS");
            }

            string report = "Result: PASS" + Environment.NewLine + string.Join(Environment.NewLine, observations);
            File.WriteAllText(Path.Combine(root, "report.txt"), report);
            Console.WriteLine(report);
            return 0;
        }
        catch (Exception exception)
        {
            string report = "Result: FAIL" + Environment.NewLine + exception;
            File.WriteAllText(Path.Combine(root, "report.txt"), report);
            Console.Error.WriteLine(report);
            return 1;
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    private static VisionPipeline CreatePipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "Step edit contract" };
        pipeline.Steps.Add(Step("검사", "Main", "A"));
        VisionPipelineStep consumer = Step("후처리", "A", "B");
        consumer.UseAcceptance = true;
        consumer.MaxElapsedMilliseconds = 1000;
        pipeline.Steps.Add(consumer);
        return pipeline;
    }

    private static VisionPipelineStep Step(string name, string input, string output)
    {
        VisionPipelineStep step = new VisionPipelineStep { Name = name, ToolType = "Threshold", Enabled = true, InputLayer = input, OutputLayer = output };
        step.Parameters["Mode"] = "Threshold";
        step.Parameters["Threshold"] = "127";
        step.Parameters["MaxValue"] = "255";
        step.Parameters["ThresholdType"] = "Binary";
        return step;
    }

    private static VisionPipeline CreateFixturePipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "Fixture edit contract" };
        VisionPipelineStep producer = new VisionPipelineStep { Name = "Fixture", ToolType = "Matching", Enabled = true, InputLayer = "Main", OutputLayer = "Pose" };
        producer.Parameters["NUM_MATCH"] = "1";
        producer.Parameters[VisionPipelineFixtureFrameService.PublishParameter] = "true";
        producer.Parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = "Frame";
        producer.Parameters[VisionPipelineFixtureFrameService.ReferenceXParameter] = "0";
        producer.Parameters[VisionPipelineFixtureFrameService.ReferenceYParameter] = "0";
        producer.Parameters[VisionPipelineFixtureFrameService.ReferenceAngleParameter] = "0";
        producer.Parameters[VisionPipelineFixtureFrameService.ReferenceScaleParameter] = "1";
        VisionPipelineStep consumer = Step("Measure", "Main", "Measured");
        consumer.Parameters[VisionPipelineFixtureFrameService.ConsumeParameter] = "true";
        consumer.Parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = "Frame";
        consumer.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        consumer.Parameters["USE_ROI"] = "true";
        consumer.Parameters["CvROI"] = "1,1,10,10";
        pipeline.Steps.Add(producer);
        pipeline.Steps.Add(consumer);
        return pipeline;
    }

    private static string ToXml(VisionPipeline pipeline)
    {
        using StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
        new XmlSerializer(typeof(VisionPipeline)).Serialize(writer, pipeline);
        return writer.ToString();
    }

    private static void RoundTrip(string root, string name, VisionPipeline pipeline)
    {
        string path = Path.Combine(root, name + ".xml");
        Assert(SerializeHelper.SaveXmlFile(path, pipeline), "XML save");
        Assert(SerializeHelper.TryLoadFromXmlFile(path, out VisionPipeline loaded), "XML reload");
        Assert(ToXml(loaded) == ToXml(pipeline), "XML preserves all pipeline fields and parameter values");
    }

    private static void Assert(bool passed, string message)
    {
        if (!passed) { throw new InvalidOperationException(message); }
    }

    private static void AssertThrows<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException("Expected " + typeof(T).Name);
    }
}
