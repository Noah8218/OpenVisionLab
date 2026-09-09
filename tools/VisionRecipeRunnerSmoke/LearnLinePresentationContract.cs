using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class LearnLinePresentationContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();
        Check("Edge gradient, threshold and connected-line presentation across all stages", () =>
        {
            LineLearnPresenter presenter = new();
            int[] strengths = { 6, 6, 132, 7, 0, 5, 7, 133, 8, 0, 6, 6, 133, 8, 0, 6, 7, 134, 7, 0, 7, 7, 133, 8, 0 };
            int[] edgeIndices = { 2, 7, 12, 17, 22 };
            string[] statuses =
            {
                "0 / 3 - GV 샘플에서 밝기 변화를 확인합니다.",
                "1 / 3 - Gradient: abs(오른쪽 GV - 왼쪽 GV)",
                "2 / 3 - Edge: strength >= 80",
                "3 / 3 - LineRun: best vertical chain = 5 px"
            };
            Require(presenter.EdgeThreshold == 80 && presenter.EdgeLineAnimationStep == 3, "Initial Edge stage/parameter changed.");
            Require(presenter.EdgeLineFormulaText == "Edge = abs(right GV - left GV) >= 80, LineRun = 5 px", "Edge formula changed.");
            presenter.ResetEdgeLineAnimation();
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0)
                    presenter.AdvanceEdgeLineAnimation();
                Require(presenter.EdgeLineAnimationStatusText == statuses[stage], "Edge stage status changed.");
                for (int i = 0; i < strengths.Length; i++)
                {
                    bool edge = edgeIndices.Contains(i);
                    LineLearnCell cell = presenter.GetEdgeOutputCell(i);
                    string expectedText = stage == 0 ? "-" : stage >= 2 && edge ? (stage == 3 ? "L" : "E") : strengths[i].ToString(CultureInfo.InvariantCulture);
                    LineLearnCellRole expectedRole = stage == 0 ? LineLearnCellRole.Neutral
                        : stage >= 2 && edge ? (stage == 3 ? LineLearnCellRole.Pass : LineLearnCellRole.Candidate) : LineLearnCellRole.Gray;
                    Require(cell.Text == expectedText && cell.Role == expectedRole, $"Edge output changed at stage {stage}, cell {i}.");
                    if (expectedRole == LineLearnCellRole.Gray)
                        Require(cell.Gray == Math.Min(220, strengths[i] + 35), "Gradient display shade changed.");
                    LineLearnCellRole? expectedHighlight = stage >= 2 && edge
                        ? (stage == 3 ? LineLearnCellRole.Pass : LineLearnCellRole.Candidate) : null;
                    Require(presenter.GetEdgeInputHighlight(i) == expectedHighlight, "Edge input evidence highlight changed.");
                }
            }
            presenter.UpdateEdgeThreshold(133);
            Require(presenter.EdgeLineFormulaText.EndsWith("LineRun = 4 px", StringComparison.Ordinal), "Threshold must retain the four-row chain.");
            presenter.UpdateEdgeThreshold(134);
            Require(presenter.GetEdgeOutputCell(17).Text == "E" && presenter.GetEdgeOutputCell(17).Role == LineLearnCellRole.Candidate,
                "One surviving Edge must not become a Line candidate.");
            presenter.UpdateEdgeThreshold(150);
            Require(Enumerable.Range(0, 25).All(i => presenter.GetEdgeInputHighlight(i) == null), "High threshold must remove Edge highlights.");
            presenter.UpdateEdgeThreshold(132.5);
            Require(presenter.EdgeThresholdText == "132 GV", "Model midpoint rounding changed.");
            presenter.UpdateEdgeThreshold(0);
            Require(presenter.EdgeThresholdText == "10 GV", "Model minimum threshold changed.");
        });
        Check("Distance samples, average, range equality and outlier presentation", () =>
        {
            LineLearnPresenter presenter = new();
            int[] distances = { 4, 4, 5, 4, 4 };
            string[] statuses =
            {
                "0 / 3 - 각 스캔선의 왼쪽/오른쪽 edge 쌍을 확인합니다.",
                "1 / 3 - 각 스캔선에서 Gap/Pitch를 측정합니다.",
                "2 / 3 - Average: DistancePxAvg = 4.2",
                "3 / 3 - Range 판정: NG, DistancePxRange = 1 > 0.50"
            };
            Require(presenter.LineDistanceRangeMaximum == 0.5 && presenter.LineDistanceAnimationStep == 3, "Initial Distance stage/parameter changed.");
            Require(presenter.LineDistanceFormulaText == "DistancePxAvg=4.2, DistancePxRange=1, DistanceMmAvg=0.025, DistanceMmRange=0.006, DistanceMmMax=0.030", "Distance formula changed.");
            for (int row = 0; row < 5; row++)
            {
                int right = row == 2 ? 7 : 6;
                for (int column = 0; column < 9; column++)
                {
                    LineLearnCell cell = presenter.GetDistanceInputCell(row, column);
                    string expected = column == 2 ? "L" : column == right ? "R" : column > 2 && column < right ? "-" : "0";
                    Require(cell.Text == expected, "Fixed scanline input geometry changed.");
                }
            }
            presenter.ResetLineDistanceAnimation();
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0)
                    presenter.AdvanceLineDistanceAnimation();
                Require(presenter.LineDistanceAnimationStatusText == statuses[stage], "Distance stage status changed.");
                for (int i = 0; i < 5; i++)
                {
                    LineLearnCell cell = presenter.GetDistanceOutputCell(i);
                    string text = stage == 0 ? "-" : stage == 2 ? "avg 4.2" : distances[i].ToString(CultureInfo.InvariantCulture) + " px";
                    LineLearnCellRole role = stage == 0 ? LineLearnCellRole.Neutral : stage < 3 ? LineLearnCellRole.Candidate
                        : i == 2 ? LineLearnCellRole.Outlier : LineLearnCellRole.Pass;
                    Require(cell.Text == text && cell.Role == role, "Distance cell meaning changed.");
                    LineLearnCellRole? highlight = stage == 0 ? null : stage == 3 && i == 2 ? LineLearnCellRole.Outlier : LineLearnCellRole.Candidate;
                    Require(presenter.GetDistanceInputHighlight(i) == highlight, "Distance input evidence highlight changed.");
                }
            }
            presenter.UpdateLineDistanceRangeMaximum(1);
            Require(presenter.LineDistanceAnimationStatusText == "3 / 3 - Range 판정: OK, DistancePxRange = 1 <= 1.00", "Range equality must pass.");
            Require(Enumerable.Range(0, 5).All(i => presenter.GetDistanceOutputCell(i).Role == LineLearnCellRole.Pass), "Passing range left an outlier highlight.");
        });
        Check("Refresh preserves stage, manual completion, restart and instance isolation", () =>
        {
            LineLearnPresenter first = new();
            LineLearnPresenter second = new();
            first.ResetEdgeLineAnimation();
            first.AdvanceEdgeLineAnimation();
            first.ResetLineDistanceAnimation();
            first.UpdateEdgeThreshold(100);
            first.UpdateLineDistanceRangeMaximum(1.5);
            Require(first.EdgeLineAnimationStep == 1 && first.LineDistanceAnimationStep == 0, "Topic refresh changed animation stage.");
            Require(second.EdgeLineAnimationStep == 3 && second.EdgeThreshold == 80
                && second.LineDistanceAnimationStep == 3 && second.LineDistanceRangeMaximum == 0.5, "Instances shared state.");
            first.CompleteEdgeLineAnimation();
            first.CompleteLineDistanceAnimation();
            Require(first.IsEdgeLineAnimationComplete && first.IsLineDistanceAnimationComplete, "Manual completion failed.");
            first.AdvanceEdgeLineAnimation();
            first.AdvanceLineDistanceAnimation();
            Require(first.EdgeLineAnimationStep == 1 && first.LineDistanceAnimationStep == 1, "Completed stage must restart at 1.");
            first.ResetEdgeLineAnimation();
            Require(first.LineDistanceAnimationStep == 1 && first.EdgeThreshold == 100, "Reset changed another topic or retained parameter.");
        });
        Check("Invariant numeric text and distinct detection/measurement tool hints", () =>
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                LineLearnPresenter presenter = new();
                string formula = presenter.LineDistanceFormulaText;
                foreach (string culture in new[] { "ko-KR", "en-US", "fr-FR" })
                {
                    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                    Require(presenter.LineDistanceFormulaText == formula && presenter.LineDistanceRangeMaximumText == "0.50 px"
                        && presenter.EdgeThresholdText == "80 GV", "Culture changed numeric presentation.");
                }
                Require(LineLearnPresenter.EdgeLineToolTitle.Contains("Polarity/Direction/Contrast/Thickness", StringComparison.Ordinal)
                    && LineLearnPresenter.LineDistanceToolTitle.Contains("Purpose > Measure", StringComparison.Ordinal)
                    && LineLearnPresenter.LineDistanceToolDetail.Contains("DistanceMmRange/Max", StringComparison.Ordinal), "The two Line tool hints were conflated.");
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        });
        string path = Path.Combine(evidenceDirectory, "learn-line-presentation-contract.txt");
        File.WriteAllLines(path, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine($"Learn Line Presentation: {passed.Count} passed, {failed.Count} failed. {path}");
        return failed.Count == 0 ? 0 : 1;

        void Check(string name, Action action)
        {
            try { action(); passed.Add(name); }
            catch (Exception error) { failed.Add(name + ": " + error.Message); }
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
