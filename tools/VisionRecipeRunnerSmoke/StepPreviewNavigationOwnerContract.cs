using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OpenVisionLab.Vision2D.Pipeline;

internal static class StepPreviewNavigationOwnerContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "ovl18_step_preview_navigation_owner_contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);

        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        try
        {
            OpenVisionRecipePipelineStepPreview first = CreatePreview(1, "Threshold", "Threshold", "Threshold_Output");
            OpenVisionRecipePipelineStepPreview second = CreatePreview(2, "Blob", "Blob", "Blob_Output");
            OpenVisionRecipePipelineStepPreview third = CreatePreview(3, "Measure", "LineGauge", "Measure_Output");
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps = new[] { first, second, third };
            OpenVisionRecipeStepPreviewNavigationOwner owner = new OpenVisionRecipeStepPreviewNavigationOwner();

            Require(ReferenceEquals(owner.Find(steps, "Step 2"), second),
                "Numeric Step reference did not resolve the matching preview.");
            passed.Add("failure text/index matching resolved the selected preview");

            Require(ReferenceEquals(owner.Find(steps, " blob output "), second),
                "Whitespace/case-insensitive layer matching changed.");
            passed.Add("normalized name/tool/layer matching remained compatible");

            Require(ReferenceEquals(owner.GetByOffset(steps, second, -1), first)
                && ReferenceEquals(owner.GetByOffset(steps, second, 1), third)
                && owner.GetByOffset(steps, first, -1) == null
                && owner.GetByOffset(steps, third, 1) == null,
                "Adjacent preview navigation or boundaries changed.");
            passed.Add("previous/next navigation and list boundaries were preserved");

            OpenVisionRecipePipelineStepPreview equivalent = CreatePreview(2, "Blob", "Blob", "Blob_Output");
            OpenVisionRecipePipelineStepPreview changedOutput = CreatePreview(2, "Blob", "Blob", "Other_Output");
            Require(owner.AreSame(second, equivalent)
                && !owner.AreSame(second, changedOutput)
                && owner.AreSame(second, second),
                "Preview Step identity comparison changed.");
            passed.Add("same-step identity used index/name/tool/input/output semantics");

            Require(owner.Find(steps, string.Empty) == null
                && owner.GetByOffset(steps, null, 1) == null
                && !owner.AreSame(first, null),
                "Empty and missing navigation inputs were not safely rejected.");
            passed.Add("empty reference and missing selection boundaries passed");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }

        string outputPath = Path.Combine(evidenceDirectory, "step-preview-navigation-owner-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-18 Shell Step preview navigation owner",
                "EvidenceDirectory: " + evidenceDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            "CONTRACT|step-preview-navigation-owner|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static OpenVisionRecipePipelineStepPreview CreatePreview(
        int index,
        string name,
        string toolType,
        string outputLayer)
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = name,
            ToolType = toolType,
            InputLayer = "Main",
            OutputLayer = outputLayer,
            Enabled = true
        };
        return new OpenVisionRecipePipelineStepPreview(
            index,
            step,
            OpenVisionRecipeLayerCard.CreateMissing);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
