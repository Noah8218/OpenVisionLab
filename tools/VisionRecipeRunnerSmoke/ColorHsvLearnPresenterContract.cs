using System;
using System.Collections.Generic;
using System.IO;
using OpenVisionLab;

internal static class ColorHsvLearnPresenterContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine("D:\\OpenVisionLab-TestData", "OpenVisionLab_Dev", "partial-structural-elimination-20260913", "m2-color-hsv")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        ColorHsvLearnPresenter presenter = new ColorHsvLearnPresenter();
        List<string> results = new();
        ColorHsvLearnGuide completeGuide = presenter.BuildGuide(45, 185);
        Check(
            "Initial animation state and guide values remain complete",
            presenter.IsAnimationComplete
                && completeGuide.VisibleStep == ColorHsvLearnPresenter.AnimationStepCount
                && completeGuide.MaskPass
                && completeGuide.HueMin == 35
                && completeGuide.HueMax == 55,
            results);
        Check(
            "Guide text retains the operator-facing HSV contract",
            completeGuide.FormulaText.Contains("HSV mask", StringComparison.Ordinal)
                && completeGuide.ScalarBoundsText.Contains("Scalar", StringComparison.Ordinal)
                && completeGuide.AnimationStatusText.Contains("Mask=255", StringComparison.Ordinal),
            results);

        presenter.ResetAnimation();
        Check("Reset moves the animation to the first stage", presenter.AnimationStep == 0, results);
        presenter.AdvanceAnimation();
        presenter.AdvanceAnimation();
        Check("Advance owns the staged animation state", presenter.AnimationStep == 2, results);
        presenter.CompleteAnimation();
        ColorHsvLearnGuide failedGuide = presenter.BuildGuide(45, 220);
        Check(
            "Value threshold changes the mask decision without UI state",
            presenter.IsAnimationComplete
                && !failedGuide.ValuePass
                && !failedGuide.MaskPass
                && failedGuide.AnimationStatusText.Contains("Mask=0", StringComparison.Ordinal),
            results);

        File.WriteAllLines(Path.Combine(evidenceDirectory, "color-hsv-learn-presenter-contract.txt"), results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine("COLOR_HSV_LEARN_PRESENTER_CONTRACT=" + (passed ? "PASS" : "FAIL") + "|checks=" + results.Count);
        return passed ? 0 : 1;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }

    private static string ResolveRepositoryRoot()
    {
        DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }
}
