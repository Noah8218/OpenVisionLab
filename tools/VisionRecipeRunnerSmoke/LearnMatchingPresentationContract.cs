using OpenVisionLab;
using System.Globalization;
using System.IO;

internal static class LearnMatchingPresentationContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> observations = new();
        List<string> failures = new();
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            Check("Template score/acceptance and invariant text", () =>
            {
                MatchingLearnPresenter presenter = new();
                Require(presenter.AnimationStep == 3 && presenter.IsAnimationComplete, "Initial frame changed.");
                Require(presenter.Evaluation.Scores.SequenceEqual(new[] { 0D, 1D, 0D, 0.5D, 0.5D })
                    && presenter.Evaluation.BestIndex == 1, "Template candidate results changed.");
                Require(presenter.FormulaText == "BestScore=1.00, Threshold=0.85, Result=OK", "Formula/culture changed.");
                Require(Enumerable.Range(0, 5).Count(presenter.IsCandidateAccepted) == 1, "Default candidate gate changed.");
                presenter.Update(0.5, false);
                Require(Enumerable.Range(0, 5).Count(presenter.IsCandidateAccepted) == 3, "Inclusive threshold boundary changed.");
                presenter.Update(1.0, false);
                Require(presenter.Evaluation.Pass && presenter.ThresholdText == "1.00", "Maximum teaching threshold changed.");
            });

            Check("Template/edge topic switching retains stage and changes explanation", () =>
            {
                MatchingLearnPresenter presenter = new();
                presenter.ResetAnimation();
                Require(presenter.AnimationStatusText == "0 / 3 - 검색 이미지와 Template을 확인합니다.", "Reset status changed.");
                presenter.AdvanceAnimation();
                presenter.Update(0.82, true);
                Require(presenter.AnimationStep == 1 && presenter.TemplateMark == "E" && presenter.SearchMark == "E",
                    "Switching topic reset the stage or lost edge labels.");
                Require(presenter.ConceptTitle == "EdgeBasedMatching은 밝기보다 edge 형상으로 판정합니다"
                    && presenter.FormulaText == "EdgeScoreMax=1.00, Threshold=0.82, Result=OK", "Edge teaching text changed.");
                presenter.AdvanceAnimation();
                Require(presenter.AnimationStatusText == "2 / 3 - EdgeScoreMax: 1.00", "Best edge frame changed.");
                presenter.AdvanceAnimation();
                Require(presenter.AnimationStatusText == "3 / 3 - Edge score 판정: OK, EdgeScoreMax 1.00 >= 0.82", "Edge result changed.");
                presenter.AdvanceAnimation();
                Require(presenter.AnimationStep == 1, "Completed animation did not restart at step 1.");
                presenter.Update(0.85, false);
                presenter.ShowResult();
                Require(presenter.IsAnimationComplete && presenter.TemplateMark == "T" && presenter.SearchMark == "1"
                    && presenter.AnimationStatusText == "3 / 3 - Threshold 판정: OK, BestScore 1.00 >= 0.85", "Return to template changed.");
            });

            Check("Feature good-match count and OK/NG boundary", () =>
            {
                FeatureMatchingLearnPresenter presenter = new();
                Require(presenter.Evaluation.GoodMatches.SequenceEqual(new[] { true, true, true, true, true, false })
                    && presenter.FormulaText == "GoodMatches=5, Required=4, DescriptorScore>=0.65", "Descriptor gate changed.");
                presenter.Update(5);
                Require(presenter.Evaluation.Pass, "Equal match count was rejected.");
                presenter.Update(6);
                Require(!presenter.Evaluation.Pass && presenter.RequiredText == "6"
                    && presenter.MeaningText.StartsWith("Good match 수가 부족하면 NG입니다.", StringComparison.Ordinal), "Insufficient-match decision changed.");
                Require(presenter.AnimationStatusText == "3 / 3 - GoodMatches 판정: NG, 검출 5개 < 6; RANSAC과 overlay 위치도 함께 확인합니다.", "NG explanation changed.");
                presenter.Update(1);
                Require(presenter.Evaluation.Pass && presenter.RequiredText == "1", "Minimum teaching count changed.");
            });

            Check("Feature stages/reset/restart and independent instance state", () =>
            {
                FeatureMatchingLearnPresenter presenter = new();
                FeatureMatchingLearnPresenter other = new();
                presenter.ResetAnimation();
                for (int step = 0; step <= 3; step++)
                {
                    Require(presenter.AnimationStep == step && presenter.AnimationStatusText.StartsWith(step + " / 3 -", StringComparison.Ordinal),
                        "Feature frame and explanatory stage disagree.");
                    if (step < 3)
                        presenter.AdvanceAnimation();
                }
                presenter.AdvanceAnimation();
                presenter.Update(6);
                Require(presenter.AnimationStep == 1 && other.AnimationStep == 3 && other.Evaluation.Required == 4,
                    "Presenter instances shared state or input update advanced a stage.");
                presenter.ShowResult();
                Require(presenter.IsAnimationComplete, "ShowResult did not reach the final frame.");
            });
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }

        string path = Path.Combine(evidenceDirectory, "learn-matching-presentation-contract.txt");
        File.WriteAllLines(path, new[] { "Result: " + (failures.Count == 0 ? "PASS" : "FAIL") }
            .Concat(observations.Select(item => "PASS: " + item)).Concat(failures.Select(item => "FAIL: " + item)));
        Console.WriteLine($"Learn Matching presentation: {observations.Count} passed, {failures.Count} failed. {path}");
        return failures.Count == 0 ? 0 : 1;

        void Check(string name, Action action)
        {
            try
            {
                action();
                observations.Add(name);
            }
            catch (Exception error)
            {
                failures.Add(name + ": " + error.Message);
            }
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
