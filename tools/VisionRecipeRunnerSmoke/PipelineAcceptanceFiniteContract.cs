using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class PipelineAcceptanceFiniteContract
{
    public static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();

        RunCase("NaN metric fails closed", () =>
        {
            VisionToolResult result = CreateResult(double.NaN);
            VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(CreateMetricStep(), result);
            Require(result.Success && !acceptance.Passed && acceptance.Message.Contains("finite", StringComparison.OrdinalIgnoreCase),
                "NaN metric was not rejected with a finite-value diagnostic.");
        }, passed, failed);

        RunCase("positive infinity metric fails closed", () =>
        {
            VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(
                CreateMetricStep(),
                CreateResult(double.PositiveInfinity));
            Require(!acceptance.Passed, "Positive infinity metric was accepted.");
        }, passed, failed);

        RunCase("negative infinity metric fails closed", () =>
        {
            VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(
                CreateMetricStep(),
                CreateResult(double.NegativeInfinity));
            Require(!acceptance.Passed, "Negative infinity metric was accepted.");
        }, passed, failed);

        RunCase("nonfinite minimum fails closed", () =>
        {
            foreach (double value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            {
                VisionPipelineStep step = CreateMetricStep();
                step.AcceptanceMetricMinimum = value;
                VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(step, CreateResult(0.5));
                Require(!acceptance.Passed && acceptance.Message.Contains("minimum", StringComparison.OrdinalIgnoreCase),
                    $"Nonfinite metric minimum {value} was not rejected.");
            }
        }, passed, failed);

        RunCase("nonfinite maximum fails closed", () =>
        {
            foreach (double value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            {
                VisionPipelineStep step = CreateMetricStep();
                step.AcceptanceMetricMaximum = value;
                VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(step, CreateResult(0.5));
                Require(!acceptance.Passed && acceptance.Message.Contains("maximum", StringComparison.OrdinalIgnoreCase),
                    $"Nonfinite metric maximum {value} was not rejected.");
            }
        }, passed, failed);

        RunCase("finite boundary equality remains accepted", () =>
        {
            foreach (double value in new[] { 0D, 1D })
            {
                VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(
                    CreateMetricStep(),
                    CreateResult(value));
                Require(acceptance.Passed, $"Finite boundary value {value} was rejected.");
            }
        }, passed, failed);

        RunCase("missing metric remains rejected", () =>
        {
            VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(
                CreateMetricStep(),
                new VisionToolResult { Success = true });
            Require(!acceptance.Passed && acceptance.Message.Contains("not produced", StringComparison.OrdinalIgnoreCase),
                "Missing acceptance metric was accepted.");
        }, passed, failed);

        RunCase("disabled acceptance ignores inactive nonfinite values", () =>
        {
            VisionPipelineStep step = CreateMetricStep();
            step.UseAcceptance = false;
            step.AcceptanceMetricMinimum = double.NaN;
            step.AcceptanceMetricMaximum = double.PositiveInfinity;
            VisionToolResult result = CreateResult(double.NaN);
            VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(step, result);
            Require(result.Success && acceptance.Passed, "Disabled acceptance changed the tool-success contract.");
        }, passed, failed);

        RunCase("nonfinite elapsed limit fails closed", () =>
        {
            foreach (double value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            {
                VisionPipelineStep step = CreateMetricStep();
                step.MaxElapsedMilliseconds = value;
                VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(step, CreateResult(0.5));
                Require(!acceptance.Passed && acceptance.Message.Contains("MaxElapsedMilliseconds", StringComparison.Ordinal),
                    $"Nonfinite elapsed limit {value} was accepted.");
            }
        }, passed, failed);

        RunCase("quality NG remains distinct from tool success", () =>
        {
            VisionPipelineStep step = CreateMetricStep();
            step.ExpectedSuccess = false;
            VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(step, CreateResult(0.5));
            Require(!acceptance.Passed && acceptance.Message.Contains("ExpectedSuccess", StringComparison.Ordinal),
                "Expected-failure quality policy changed unexpectedly.");
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "pipeline-acceptance-finite-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|pipeline-acceptance-finite|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipelineStep CreateMetricStep()
    {
        return new VisionPipelineStep
        {
            Name = "Acceptance",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "Score",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 0D,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 1D
        };
    }

    private static VisionToolResult CreateResult(double metric)
    {
        VisionToolResult result = new VisionToolResult { Success = true };
        result.Metrics["Score"] = metric;
        return result;
    }

    private static void RunCase(string name, Action action, List<string> passed, List<string> failed)
    {
        try
        {
            action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add($"{name}: {exception.GetBaseException().Message}");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
