using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Property;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class ThresholdSuggestionSessionContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "threshold-suggestion-session-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);

        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        try
        {
            Require(
                string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
                "Threshold suggestion evidence must be on D:.");

            string repositoryRoot = ResolveRepositoryRoot();
            string viewSource = File.ReadAllText(Path.Combine(
                repositoryRoot,
                "src",
                "OpenVisionLab",
                "UI",
                "VisionTest",
                "Wpf",
                "ToolViews",
                "ThresholdToolWpfView.xaml.cs"));
            string controllerSource = File.ReadAllText(Path.Combine(
                repositoryRoot,
                "src",
                "OpenVisionLab",
                "UI",
                "VisionTest",
                "Wpf",
                "Tooling",
                "Threshold",
                "ThresholdToolSuggestionController.cs"));
            Require(
                viewSource.Contains("ThresholdToolSuggestionController", StringComparison.Ordinal)
                    && viewSource.Contains("thresholdSuggestionController.Analyze", StringComparison.Ordinal)
                    && viewSource.Contains("thresholdSuggestionController.Use", StringComparison.Ordinal)
                    && viewSource.Contains("thresholdSuggestionController.Undo", StringComparison.Ordinal),
                "Threshold View should route suggestion actions through one workflow controller.");
            Require(
                !viewSource.Contains("VisionToolThresholdSuggestionSession", StringComparison.Ordinal)
                    && !viewSource.Contains("thresholdSuggestionSession", StringComparison.Ordinal),
                "Threshold View should not retain the suggestion session or its mutable policy state.");
            Require(
                controllerSource.Contains("VisionToolThresholdSuggestionSession", StringComparison.Ordinal)
                    && controllerSource.Contains("session.Analyze", StringComparison.Ordinal)
                    && controllerSource.Contains("session.Use", StringComparison.Ordinal)
                    && controllerSource.Contains("session.Undo", StringComparison.Ordinal),
                "Threshold suggestion controller should delegate policy to the existing session owner.");
            Require(
                !controllerSource.Contains("System.Windows", StringComparison.Ordinal)
                    && !controllerSource.Contains("Window", StringComparison.Ordinal)
                    && !controllerSource.Contains("Button", StringComparison.Ordinal),
                "Threshold suggestion controller should not depend on WPF controls.");
            passed.Add("View routes suggestion actions through a WPF-free workflow controller and existing session owner");

            VisionToolSignalEvidence evidence = CreateEvidence("evidence-1", "source-1");
            ThresholdToolProperty initialProperty = CreateProperty(25);
            VisionToolThresholdSuggestionSession session = new VisionToolThresholdSuggestionSession();

            VisionToolThresholdSuggestion suggestion = session.Analyze(evidence, initialProperty);
            if (suggestion == null || !suggestion.Accepted)
            {
                throw new InvalidOperationException("A two-mode histogram should produce an accepted suggestion.");
            }

            Require(session.CurrentSuggestion == suggestion, "Analyze should retain the current suggestion in the session.");
            passed.Add("analysis state is owned by the Threshold suggestion session");

            VisionToolThresholdSuggestionUseResult useResult = session.Use(evidence, initialProperty);
            Require(useResult == VisionToolThresholdSuggestionUseResult.Applied, "Use should record a changed threshold as applied.");
            Require(session.HasUndo, "Use should retain an Undo snapshot.");
            Require(session.PreviousThreshold == 25, "Undo should retain the normalized previous threshold.");
            Require(session.AppliedThreshold == suggestion.Threshold, "Undo should retain the applied suggestion threshold.");
            passed.Add("Use records the previous value and applied value without applying UI changes");

            ThresholdToolProperty appliedProperty = CreateProperty(suggestion.Threshold);
            Require(session.CanUndo(evidence, appliedProperty), "Undo should remain valid for the same evidence and applied value.");
            session.ClearSuggestion();
            Require(session.CurrentSuggestion == null, "Clearing preview evidence should clear the suggestion only.");
            Require(session.CanUndo(evidence, appliedProperty), "Clearing the suggestion should preserve a valid Undo snapshot.");
            int previousThreshold = session.Undo(evidence, appliedProperty);
            Require(previousThreshold == 25 && !session.HasUndo, "Undo should return the previous value and consume the snapshot.");
            passed.Add("ClearSuggestion preserves Undo and Undo consumes it after validation");

            VisionToolThresholdSuggestionUseResult alreadyCurrent = session.Analyze(evidence, CreateProperty(suggestion.Threshold)) != null
                ? session.Use(evidence, CreateProperty(suggestion.Threshold))
                : VisionToolThresholdSuggestionUseResult.Applied;
            Require(
                alreadyCurrent == VisionToolThresholdSuggestionUseResult.AlreadyCurrent,
                "Use should report AlreadyCurrent without creating an Undo snapshot.");
            Require(!session.HasUndo, "AlreadyCurrent should not create an Undo snapshot.");
            passed.Add("AlreadyCurrent is explicit and does not schedule a parameter change");

            session.Analyze(evidence, initialProperty);
            bool staleUseRejected = false;
            try
            {
                session.Use(CreateEvidence("evidence-2", "source-2"), initialProperty);
            }
            catch (InvalidOperationException)
            {
                staleUseRejected = true;
            }

            Require(staleUseRejected, "Use should reject evidence that no longer matches the analyzed suggestion.");
            passed.Add("stale evidence is rejected before an apply result is returned");

            Require(
                session.Analyze(evidence, CreateNonBasicProperty()) == null
                    && session.CurrentSuggestion == null,
                "Non-Basic modes should not retain a Threshold suggestion.");
            passed.Add("non-Basic mode does not expose Threshold suggestion state");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }

        string outputPath = Path.Combine(evidenceDirectory, "threshold-suggestion-session-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: Threshold suggestion session ownership and stale/Undo policy",
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
            "CONTRACT|threshold-suggestion-session|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static string ResolveRepositoryRoot()
    {
        foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            DirectoryInfo? current = new DirectoryInfo(Path.GetFullPath(start));
            while (current != null)
            {
                string viewPath = Path.Combine(
                    current.FullName,
                    "src",
                    "OpenVisionLab",
                    "UI",
                    "VisionTest",
                    "Wpf",
                    "ToolViews",
                    "ThresholdToolWpfView.xaml.cs");
                if (File.Exists(viewPath))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root could not be located.");
    }

    private static VisionToolSignalEvidence CreateEvidence(string evidenceId, string sourceSha256)
    {
        double[] histogram = new double[256];
        for (int offset = -2; offset <= 2; offset++)
        {
            histogram[50 + offset] = 1000 - (Math.Abs(offset) * 100);
            histogram[200 + offset] = 800 - (Math.Abs(offset) * 80);
        }

        return new VisionToolSignalEvidence(
            evidenceId,
            sourceSha256,
            "result-" + evidenceId,
            "Threshold/" + ThresholdToolMode.Threshold,
            "Input",
            "Full image",
            "Basic threshold",
            "Gray",
            "Count",
            new[]
            {
                new VisionToolSignalSeries("Gray population", "#157C86", 0, 1, histogram)
            });
    }

    private static ThresholdToolProperty CreateProperty(double threshold)
    {
        return new ThresholdToolProperty
        {
            Mode = ThresholdToolMode.Threshold,
            Threshold = threshold,
            ThresholdType = ThresholdTypes.Binary
        };
    }

    private static ThresholdToolProperty CreateNonBasicProperty()
    {
        return new ThresholdToolProperty
        {
            Mode = ThresholdToolMode.Range,
            Threshold = 25,
            ThresholdType = ThresholdTypes.Binary
        };
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
