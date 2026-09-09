using OpenVisionLab;
using System.Globalization;
using System.IO;

internal static class LearnMetricsAcceptanceContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();
        Check("Fixed sample statistics and average/outlier decisions", () =>
        {
            MetricsAcceptanceLearnPresenter presenter = new();
            Require(presenter.Samples.SequenceEqual(new[] { 0.50, 0.51, 0.49, 0.82, 0.50 }), "Lesson samples changed.");
            Require(Math.Abs(presenter.Average - 0.564) < 1e-12 && Math.Abs(presenter.Range - 0.33) < 1e-12
                && presenter.Maximum == 0.82, "Sample statistics changed.");
            Require(Enumerable.Range(0, 5).Where(presenter.IsOutlier).SequenceEqual(new[] { 3 }), "Outlier identity changed.");
            Require(presenter.AnimationStep == 3 && presenter.ShowsSampleDecision && presenter.IsAnimationComplete,
                "Initial completed frame changed.");
            Require(presenter.FormulaText == "Range=0.33 / Max=0.82 -> NG", "Outlier gate must reject the sample set.");
            presenter.ResetAnimation();
            presenter.AdvanceAnimation();
            presenter.AdvanceAnimation();
            Require(presenter.FormulaText == "DistanceMmAvg=0.56 -> OK" && !presenter.ShowsSampleDecision,
                "Average-only frame must illustrate its passing result before revealing the outlier.");
        });
        Check("Reset, exact stage text, neutral samples and restart", () =>
        {
            MetricsAcceptanceLearnPresenter presenter = new();
            string[] formulas =
            {
                "Avg 0.45..0.60 | Range <= 0.10 | Max <= 0.65",
                "Samples=5 | 측정값 5개를 모두 확인합니다.",
                "DistanceMmAvg=0.56 -> OK",
                "Range=0.33 / Max=0.82 -> NG"
            };
            string[] statuses =
            {
                "0 / 3 - 평균, 범위, 최대값 판정 기준을 확인합니다.",
                "1 / 3 - Samples: 5개 측정값을 모두 확인합니다.",
                "2 / 3 - 평균 판정: OK; 평균만 보면 통과합니다.",
                "3 / 3 - 범위/최대값 판정: NG; 0.82 mm 이상치를 검출합니다."
            };
            presenter.ResetAnimation();
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0)
                    presenter.AdvanceAnimation();
                Require(presenter.AnimationStep == stage && presenter.FormulaText == formulas[stage]
                    && presenter.AnimationStatusText == statuses[stage], "Stage text changed.");
                Require(presenter.ShowsSampleDecision == (stage == 3), "Sample decision appeared at the wrong stage.");
                Require(presenter.GetSampleText(3) == (stage == 0 ? "-" : "0.82"), "Reset/sample visibility changed.");
            }
            presenter.AdvanceAnimation();
            Require(presenter.AnimationStep == 1 && !presenter.IsAnimationComplete, "Completed frame did not wrap to 1.");
        });
        Check("Instance state independence and invariant numeric text", () =>
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                MetricsAcceptanceLearnPresenter first = new();
                MetricsAcceptanceLearnPresenter second = new();
                first.ResetAnimation();
                Require(second.AnimationStep == 3, "Two lessons shared animation state.");
                foreach (string culture in new[] { "ko-KR", "en-US", "fr-FR" })
                {
                    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                    Require(second.GetSampleText(1) == "0.51" && second.FormulaText == "Range=0.33 / Max=0.82 -> NG",
                        "Culture changed the numeric lesson text.");
                }
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        });
        string path = Path.Combine(evidenceDirectory, "learn-metrics-acceptance-contract.txt");
        File.WriteAllLines(path, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine($"Learn Metrics Acceptance: {passed.Count} passed, {failed.Count} failed. {path}");
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
