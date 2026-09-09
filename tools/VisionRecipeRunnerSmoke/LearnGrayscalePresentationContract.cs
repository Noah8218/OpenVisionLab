using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class LearnGrayscalePresentationContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();
        Check("Threshold equality, maximum, inversion, rounding and persistent bounce direction", () =>
        {
            GrayscaleLearnPresenter presenter = new();
            presenter.ConfigureMaximumValue(100);
            presenter.UpdateThreshold(127.5, false);
            Require(presenter.MaximumValueText == "MaxValue = 100" && presenter.ThresholdValueText == "Threshold = 128", "Threshold rounding or maximum text changed.");
            Require(presenter.ThresholdFormulaText == "Binary: GV >= threshold -> MaxValue, GV < threshold -> 0", "Binary formula changed.");
            for (int i = 0; i < 8; i++)
            {
                GrayscaleLearnCell cell = presenter.GetThresholdOutputCell(i);
                Require(cell.Gray == (i < 4 ? 0 : 100) && cell.BackgroundRole == GrayscaleLearnCellRole.Gray,
                    "Threshold equality or configured maximum changed.");
            }
            Require(presenter.GetThresholdOutputCell(4).Text == "128 -> 100", "Equal threshold must belong to the high result.");
            presenter.UpdateThreshold(128.5, true);
            Require(presenter.ThresholdValueText == "Threshold = 128" && presenter.GetThresholdOutputCell(4).Text == "128 -> 0", "Inversion or midpoint rounding changed.");
            Require(presenter.ThresholdFormulaText == "BinaryInv: GV >= threshold -> 0, GV < threshold -> MaxValue", "BinaryInv formula changed.");
            Require(Enumerable.Range(0, 8).All(i => presenter.GetThresholdOutputCell(i).Gray == (i < 4 ? 100 : 0)), "BinaryInv did not invert both classes.");
            presenter.ConfigureMaximumValue(42.5);
            Require(presenter.MaximumValue == 42 && GrayscaleLearnPresenter.NormalizeThresholdValue(-1) == 0
                && GrayscaleLearnPresenter.NormalizeThresholdValue(300) == 255
                && GrayscaleLearnPresenter.NormalizeThresholdValue(double.NaN) == 0
                && GrayscaleLearnPresenter.NormalizeThresholdValue(double.PositiveInfinity) == 0, "Existing constructor/test-hook byte normalization changed.");
            Require(presenter.NextThresholdAnimationValue(227) == 230, "Forward animation did not stop at 230.");
            presenter.UpdateThreshold(100, true);
            Require(presenter.NextThresholdAnimationValue(100) == 95, "Manual threshold/invert refresh reset animation direction.");
            Require(presenter.NextThresholdAnimationValue(25) == 25 && presenter.NextThresholdAnimationValue(100) == 105, "Lower bounce did not restore forward direction.");
            Require(presenter.NextThresholdAnimationValue(240) == 230 && presenter.NextThresholdAnimationValue(0) == 25, "Animation outside its bounce interval changed.");
        });
        Check("Brightness stages, histogram, signed offset and clamped pixel presentation", () =>
        {
            GrayscaleLearnPresenter presenter = new();
            int[] results = { 57, 83, 110, 138, 161, 187, 219, 253 };
            int[] bins = { 0, 1, 1, 1, 1, 2, 1, 1 };
            string[] statuses =
            {
                "0 / 3 - Reset: 원본 GV부터 확인합니다.",
                "1 / 3 - Input GV: 각 픽셀의 밝기 값을 읽습니다.",
                "2 / 3 - Brightness: GV +35 후 0~255로 제한합니다.",
                "3 / 3 - Histogram shift: 평균 GV 116 -> 151, 분포가 오른쪽으로 이동합니다."
            };
            Require(presenter.BrightnessFormulaText == "Result GV = clamp(Source GV +35, 0, 255)" && presenter.BrightnessOffsetText == "Offset = +35 GV", "Initial brightness formula changed.");
            Require(presenter.BrightnessHistogramBins.SequenceEqual(bins), "Default histogram changed.");
            presenter.ResetBrightnessAnimation();
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0) presenter.AdvanceBrightnessAnimation();
                Require(presenter.BrightnessAnimationStatusText == statuses[stage] && presenter.ShowBrightnessHistogram == (stage == 3), "Brightness stage/status changed.");
                Require(presenter.BrightnessInputBorderRole == (stage == 1 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default), "Brightness input highlight changed.");
                Require(presenter.GetBrightnessHistogramLabel(7) == "224-255" + Environment.NewLine + (stage == 3 ? "1" : "-"), "Histogram visibility/last-bin label changed.");
                for (int i = 0; i < 8; i++)
                {
                    GrayscaleLearnCell cell = presenter.GetBrightnessOutputCell(i);
                    Require(cell.Gray == results[i] && cell.Text == (stage >= 2 ? presenter.BrightnessSamples[i] + " -> " + results[i] : "-"), "Brightness pixel presentation changed.");
                    Require(cell.BackgroundRole == (stage >= 2 ? GrayscaleLearnCellRole.Gray : GrayscaleLearnCellRole.Neutral)
                        && cell.BorderRole == (stage == 3 ? GrayscaleLearnCellRole.Pass : stage == 2 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default), "Brightness stage role changed.");
                }
            }
            presenter.UpdateBrightness(80);
            Require(presenter.GetBrightnessOutputCell(6).Gray == 255 && presenter.GetBrightnessOutputCell(7).Gray == 255, "Positive offset stopped saturating.");
            presenter.UpdateBrightness(-80);
            Require(presenter.GetBrightnessOutputCell(0).Gray == 0 && presenter.GetBrightnessOutputCell(2).Gray == 0
                && presenter.BrightnessAnimationStatusText == "3 / 3 - Histogram shift: 평균 GV 116 -> 48, 분포가 왼쪽으로 이동합니다.", "Negative offset clamp/direction changed.");
            presenter.UpdateBrightness(0);
            Require(presenter.BrightnessAnimationStatusText.EndsWith("분포가 같은 위치으로 이동합니다.", StringComparison.Ordinal), "Unchanged-distribution wording changed.");
        });
        Check("Arithmetic five operations and staged input/output contract", () =>
        {
            GrayscaleLearnPresenter presenter = new();
            (string Mode, int[] Results, string Expression)[] cases =
            {
                ("AbsDiff", new[] { 10, 15, 50, 20, 40, 20, 180, 20 }, "|20 - 10| = 10"),
                ("Add", new[] { 30, 105, 130, 255, 255, 255, 240, 255 }, "clamp(20 + 10) = 30"),
                ("Subtract", new[] { 10, 0, 50, 0, 40, 0, 180, 20 }, "clamp(20 - 10) = 10"),
                ("Bitwise AND", new[] { 0, 44, 8, 8, 6, 128, 18, 208 }, "20 & 10 = 0"),
                ("Bitwise OR", new[] { 30, 61, 122, 252, 254, 252, 222, 252 }, "20 | 10 = 30")
            };
            foreach (var item in cases)
            {
                presenter.UpdateArithmetic(item.Mode);
                presenter.ResetArithmeticAnimation();
                for (int stage = 0; stage <= 3; stage++)
                {
                    if (stage > 0) presenter.AdvanceArithmeticAnimation();
                    Require(presenter.ArithmeticFormulaText == item.Mode + ": A/B -> Output", "Arithmetic formula changed.");
                    Require(presenter.ArithmeticInputBorderRole == (stage == 1 || stage == 2 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default), "Arithmetic input highlight changed.");
                    if (stage == 2) Require(presenter.ArithmeticAnimationStatusText == "2 / 3 - " + item.Mode + " 적용: " + item.Expression, "Arithmetic teaching expression changed.");
                    for (int i = 0; i < 8; i++)
                    {
                        GrayscaleLearnCell cell = presenter.GetArithmeticOutputCell(i);
                        Require(cell.Gray == item.Results[i] && cell.Text == (stage >= 2 ? item.Results[i].ToString(CultureInfo.InvariantCulture) : "-"), "Arithmetic result changed.");
                        Require(cell.BackgroundRole == (stage >= 2 ? GrayscaleLearnCellRole.Gray : GrayscaleLearnCellRole.Neutral)
                            && cell.BorderRole == (stage == 3 ? GrayscaleLearnCellRole.Pass : stage == 2 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default), "Arithmetic output stage role changed.");
                    }
                }
            }
        });
        Check("Filter three operations, center-only transformation and kernel/calculation roles", () =>
        {
            GrayscaleLearnPresenter presenter = new();
            (string Mode, int Result, string Calculation)[] cases =
            {
                ("Mean blur", 75, "Mean: 677 / 9"),
                ("Median", 59, "Sort: 42, 54, 57, 58, 59, 60, 62, 65, 220"),
                ("Sharpen", 255, "220 x 5 - 58 - 60 - 65 - 62")
            };
            foreach (var item in cases)
            {
                presenter.UpdateFilter(item.Mode);
                presenter.ResetFilterAnimation();
                for (int stage = 0; stage <= 3; stage++)
                {
                    if (stage > 0) presenter.AdvanceFilterAnimation();
                    if (stage == 2) Require(presenter.FilterAnimationStatusText == "2 / 3 - Calculate: " + item.Calculation, "Filter calculation text changed.");
                    for (int i = 0; i < 9; i++)
                    {
                        bool center = i == 4;
                        GrayscaleLearnCell cell = presenter.GetFilterOutputCell(i);
                        int value = center ? item.Result : presenter.FilterSamples[i];
                        Require(cell.Gray == value && cell.Text == (stage == 3 ? value.ToString(CultureInfo.InvariantCulture) : stage == 2 && center ? "..." : "-"), "Filter center transformation or cell text changed.");
                        Require(presenter.GetFilterInputBorderRole(i) == (stage == 2 && center ? GrayscaleLearnCellRole.Warning : stage == 1 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default), "Filter kernel/calculation input evidence changed.");
                        Require(cell.BackgroundRole == (stage == 3 ? GrayscaleLearnCellRole.Gray : stage == 2 && center ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Neutral)
                            && cell.BorderRole == (center && stage >= 2 ? stage == 3 ? GrayscaleLearnCellRole.Pass : GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default), "Filter output stage role changed.");
                    }
                }
            }
        });
        Check("Stage-preserving refresh, completion, restart and independent lesson instances", () =>
        {
            GrayscaleLearnPresenter first = new();
            GrayscaleLearnPresenter second = new();
            first.ResetBrightnessAnimation();
            first.ResetArithmeticAnimation();
            first.ResetFilterAnimation();
            first.AdvanceBrightnessAnimation();
            first.AdvanceArithmeticAnimation();
            first.AdvanceFilterAnimation();
            first.UpdateBrightness(-35);
            first.UpdateArithmetic("Add");
            first.UpdateFilter("Median");
            Require(first.BrightnessAnimationStep == 1 && first.ArithmeticAnimationStep == 1 && first.FilterAnimationStep == 1, "Topic refresh changed stage.");
            Require(second.BrightnessAnimationStep == 3 && second.ArithmeticAnimationStep == 3 && second.FilterAnimationStep == 3
                && second.BrightnessOffsetText == "Offset = +35 GV" && second.ArithmeticFormulaText == "AbsDiff: A/B -> Output", "Presenters shared mutable lesson state.");
            first.CompleteBrightnessAnimation();
            first.CompleteArithmeticAnimation();
            first.CompleteFilterAnimation();
            Require(first.IsBrightnessAnimationComplete && first.IsArithmeticAnimationComplete && first.IsFilterAnimationComplete, "Loaded parameter-change completion failed.");
            first.AdvanceBrightnessAnimation();
            first.AdvanceArithmeticAnimation();
            first.AdvanceFilterAnimation();
            Require(first.BrightnessAnimationStep == 1 && first.ArithmeticAnimationStep == 1 && first.FilterAnimationStep == 1, "Step after completion must restart at 1.");
            first.ResetBrightnessAnimation();
            Require(first.ArithmeticAnimationStep == 1 && first.FilterAnimationStep == 1 && first.BrightnessOffsetText == "Offset = -35 GV", "Reset changed another lesson or its retained setting.");
        });
        Check("Invariant numeric lesson text and existing tool location guidance", () =>
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                GrayscaleLearnPresenter presenter = new();
                foreach (string culture in new[] { "ko-KR", "en-US", "fr-FR" })
                {
                    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                    presenter.UpdateBrightness(35.5);
                    Require(presenter.BrightnessFormulaText == "Result GV = clamp(Source GV +36, 0, 255)"
                        && presenter.ThresholdValueText == "Threshold = 127" && presenter.GetThresholdOutputCell(4).Text == "128 -> 255", "Culture changed numeric lesson presentation.");
                }
                Require(GrayscaleLearnPresenter.MeanOpenedTitle.Contains("Mean Type, Min Mean, Max Mean", StringComparison.Ordinal)
                    && GrayscaleLearnPresenter.HistogramOpenedTitle.Contains("Clip Limit, Tile Grid", StringComparison.Ordinal)
                    && GrayscaleLearnPresenter.ArithmeticOpenedDetail.Contains("Constant/Offset", StringComparison.Ordinal)
                    && GrayscaleLearnPresenter.FilteringOpenedDetail.Contains("Bilateral", StringComparison.Ordinal), "Tool location guidance changed.");
            }
            finally { CultureInfo.CurrentCulture = previous; }
        });
        string path = Path.Combine(evidenceDirectory, "learn-grayscale-presentation-contract.txt");
        File.WriteAllLines(path, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine($"Learn Grayscale Presentation: {passed.Count} passed, {failed.Count} failed. {path}");
        return failed.Count == 0 ? 0 : 1;

        void Check(string name, Action action)
        {
            try { action(); passed.Add(name); }
            catch (Exception error) { failed.Add(name + ": " + error.Message); }
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
