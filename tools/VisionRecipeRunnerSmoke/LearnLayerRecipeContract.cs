using OpenVisionLab;
using System.Globalization;
using System.IO;

internal static class LearnLayerRecipeContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();
        Check("Exact routes and highlighted layers", () =>
        {
            LayerRecipeLearnPresenter presenter = new();
            string[] formulas =
            {
                "Step 1: Input=Main -> Tool=Threshold -> Output=Pin_Binary",
                "Step 2: Input=Pin_Binary -> Tool=LineDistance -> Output=Pin_Gap",
                "Step 3: Input=Main + Pin_Gap -> Tool=Overlay -> Output=Pin_Review",
                "Step 4: Input=Pin_Gap -> Tool=Accept -> Output=Inspection"
            };
            int[][] activeLayers = { new[] { 0, 1 }, new[] { 1, 2 }, new[] { 0, 2, 3 }, new[] { 2 } };
            for (int step = 1; step <= 4; step++)
            {
                presenter.SelectStep(step);
                Require(presenter.FormulaText == formulas[step - 1], "Route/formula changed.");
                Require(Enumerable.Range(0, 4).Where(presenter.IsRouteLayer).SequenceEqual(activeLayers[step - 1]),
                    "Input/output layer highlight changed.");
                Require(Enumerable.Range(0, 16).Where(presenter.IsSelectedFlowCell)
                    .SequenceEqual(Enumerable.Range((step - 1) * 4, 4)), "Flow-row highlight changed.");
            }
        });
        Check("Reset retains selection/explanation, then restarts at step 1", () =>
        {
            LayerRecipeLearnPresenter presenter = new();
            presenter.SelectStep(3);
            string formula = presenter.FormulaText;
            string meaning = presenter.MeaningText;
            presenter.ResetAnimation();
            Require(presenter.AnimationStep == 0 && presenter.SelectedStep == 3 && presenter.SelectedStepText == "0 / 4"
                && presenter.FormulaText == formula && presenter.MeaningText == meaning, "Reset lost retained state.");
            Require(!Enumerable.Range(0, 4).Any(presenter.IsRouteLayer) && !Enumerable.Range(0, 16).Any(presenter.IsSelectedFlowCell),
                "Reset retained highlighted cells.");
            presenter.AdvanceAnimation();
            Require(presenter.AnimationStep == 1 && presenter.SelectedStep == 1, "Reset restart changed.");
        });
        Check("Existing rounding/clamping and complete/restart", () =>
        {
            LayerRecipeLearnPresenter presenter = new();
            foreach (var entry in new (double Value, int Expected)[] { (-10, 1), (1.5, 2), (2.5, 2), (3.5, 4), (20, 4) })
            {
                presenter.SelectStep(entry.Value);
                Require(presenter.SelectedStep == entry.Expected && presenter.AnimationStep == entry.Expected,
                    "Selection rounding/clamping changed.");
            }
            Require(presenter.IsAnimationComplete && presenter.AnimationStatusText.Contains("Acceptance", StringComparison.Ordinal),
                "Final explanation changed.");
            presenter.AdvanceAnimation();
            Require(presenter.SelectedStep == 1 && !presenter.IsAnimationComplete, "Complete -> first step changed.");
        });
        Check("Independent instance state and invariant text", () =>
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                LayerRecipeLearnPresenter first = new();
                LayerRecipeLearnPresenter second = new();
                first.ResetAnimation();
                for (int step = 1; step <= 4; step++)
                {
                    first.AdvanceAnimation();
                    Require(first.SelectedStepText == step + " / 4"
                        && first.AnimationStatusText.StartsWith(step + " / 4 -", StringComparison.Ordinal), "Stage text changed.");
                }
                Require(second.SelectedStep == 2 && second.AnimationStep == 2, "Instances shared mutable step state.");
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        });
        string path = Path.Combine(evidenceDirectory, "learn-layer-recipe-contract.txt");
        File.WriteAllLines(path, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine($"Learn Layer Recipe: {passed.Count} passed, {failed.Count} failed. {path}");
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
