using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class ValidationEvidenceOwnerContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "ovl17_validation_evidence_owner_contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);

        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        string dataRoot = Path.Combine(evidenceDirectory, "data");
        string recipeName = "ValidationEvidenceOwner_" + Guid.NewGuid().ToString("N")[..10];
        const string pipelineName = "EvidencePipeline";
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "Validation evidence contract output must be on D:.");
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);

            VisionPipeline pipeline = CreatePipeline(includeScale: true);
            VisionPipelineStorage.Save(recipeName, pipeline);

            OpenVisionRecipeValidationEvidenceOwner owner =
                new OpenVisionRecipeValidationEvidenceOwner();
            OpenVisionRecipeValidationEvidence evidence = owner.Build(
                recipeName,
                pipelineName,
                hasSelectedPipeline: true);
            Require(evidence.Succeeded, "The owner did not load the persisted Pipeline XML.");
            Require(evidence.AcceptanceText.Contains("DistanceMm", StringComparison.Ordinal)
                && evidence.AcceptanceText.Contains("1.25..2.5", StringComparison.Ordinal),
                "The owner changed the active acceptance gate projection.");
            Require(evidence.CalibrationText.Contains("PIXELPERMM 4", StringComparison.Ordinal)
                && evidence.CalibrationText.Contains("mm/px", StringComparison.Ordinal),
                "The owner changed the positive mm calibration projection.");
            passed.Add("persisted Pipeline load and acceptance/calibration projection");

            VisionPipelineStorage.Save(recipeName, CreatePipeline(includeScale: false));
            OpenVisionRecipeValidationEvidence missingScale = owner.Build(
                recipeName,
                pipelineName,
                hasSelectedPipeline: true);
            Require(missingScale.Succeeded
                && (missingScale.CalibrationText.Contains("PIXELPERMM", StringComparison.Ordinal)
                    || missingScale.CalibrationText.Contains("PIXELPERMM", StringComparison.OrdinalIgnoreCase)),
                "The owner did not preserve the missing PIXELPERMM safety gate.");
            Require(missingScale.CalibrationText.Contains("Required", StringComparison.Ordinal)
                || missingScale.CalibrationText.Contains("필수", StringComparison.Ordinal),
                "The owner did not mark missing mm calibration as required.");
            passed.Add("missing PIXELPERMM remained a non-judgment safety warning");

            OpenVisionRecipeValidationEvidence noSelection = owner.Build(
                recipeName,
                pipelineName,
                hasSelectedPipeline: false);
            Require(!noSelection.Succeeded
                && (noSelection.ErrorText.Contains("Select a pipeline", StringComparison.Ordinal)
                    || noSelection.ErrorText.Contains("파이프라인을 선택", StringComparison.Ordinal)),
                "The owner changed the no-selected-pipeline message contract.");
            passed.Add("no selected pipeline returned the existing review guidance");

            OpenVisionRecipeValidationEvidence missingPipeline = owner.Build(
                recipeName,
                "MissingPipeline",
                hasSelectedPipeline: true);
            Require(!missingPipeline.Succeeded
                && (missingPipeline.ErrorText.Contains("Pipeline XML", StringComparison.Ordinal)
                    || missingPipeline.ErrorText.Contains("파이프라인 XML", StringComparison.Ordinal)),
                "The owner did not surface a missing Pipeline XML error.");
            passed.Add("missing Pipeline XML returned a safe validation error");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                previousDataRoot);
        }

        string outputPath = Path.Combine(evidenceDirectory, "validation-evidence-owner-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-17 Shell validation evidence owner",
                "EvidenceDirectory: " + evidenceDirectory,
                "DataRoot: " + dataRoot,
                "RecipeName: " + recipeName,
                "PipelineName: " + pipelineName
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
            "CONTRACT|validation-evidence-owner|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipeline CreatePipeline(bool includeScale)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "EvidencePipeline" };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Distance gate",
            ToolType = "Line",
            Enabled = true,
            UseAcceptance = true,
            AcceptanceMetricName = "DistanceMm",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1.25D,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 2.5D,
            ExpectedSuccess = true
        };
        if (includeScale)
        {
            step.Parameters["PIXELPERMM"] = "4";
        }

        pipeline.Steps.Add(step);
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "Disabled gate",
            ToolType = "Line",
            Enabled = false,
            UseAcceptance = true,
            AcceptanceMetricName = "IgnoredMm",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1D
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
