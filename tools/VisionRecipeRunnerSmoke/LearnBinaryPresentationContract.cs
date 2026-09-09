using OpenVisionLab;
using System.Globalization;
using System.IO;

internal static class LearnBinaryPresentationContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();
        Check("Morphology fixed kernels, reset, per-cell reveal and completed restart", () =>
        {
            BinaryLearnPresenter presenter = new();
            Require(presenter.MorphologyAnimationStep == 25 && presenter.ActiveMorphologyCellIndex == -1,
                "Initial Morphology frame changed.");
            int[] square = { 6, 7, 8, 11, 12, 13, 16, 17, 18 };
            foreach ((string mode, int[] expected) in new (string, int[])[]
            {
                ("Erosion", new[] { 12 }),
                ("Dilation", Enumerable.Range(0, 25).ToArray()),
                ("Opening", square),
                ("Closing", square)
            })
            {
                presenter.UpdateMorphology(mode);
                Require(Enumerable.Range(0, 25).Where(i => presenter.MorphologyResult[i]).SequenceEqual(expected),
                    mode + " changed the fixed kernel output or boundary rule.");
                Require(presenter.IsMorphologyAnimationComplete && presenter.MorphologyFormulaText.StartsWith(mode + ":", StringComparison.Ordinal),
                    "Mode refresh must show the completed result and corresponding formula.");
            }

            presenter.ResetMorphologyAnimation();
            Require(presenter.MorphologyAnimationStatusText == "준비 · 외곽 밖은 검정(0)으로 계산"
                && !Enumerable.Range(0, 25).Any(presenter.IsMorphologyCellProcessed), "Reset did not hide all results.");
            for (int step = 1; step <= 25; step++)
            {
                presenter.AdvanceMorphologyAnimation();
                Require(presenter.MorphologyAnimationStep == step
                    && Enumerable.Range(0, 25).Count(presenter.IsMorphologyCellProcessed) == step,
                    "Morphology did not reveal exactly one cell per step.");
                Require(presenter.ActiveMorphologyCellIndex == (step < 25 ? step - 1 : -1), "Active kernel center changed.");
                if (step == 8)
                    Require(presenter.MorphologyAnimationStatusText == "커널 중심 (3, 2) · 8 / 25", "Kernel position status changed.");
            }
            Require(presenter.MorphologyAnimationStatusText == "전체 결과 · Play 또는 Step으로 커널 이동 확인", "Completion status changed.");
            presenter.AdvanceMorphologyAnimation();
            Require(presenter.MorphologyAnimationStep == 1, "Complete frame did not restart at one.");
            presenter.SynchronizeMorphologyMode("Dilation");
            Require(presenter.MorphologyAnimationStep == 1 && presenter.MorphologyResult.All(value => value),
                "Detached control synchronization must update the calculation without resetting stage.");
            presenter.UpdateMorphology("Erosion");
            Require(presenter.MorphologyAnimationStep == 25, "Topic/mode refresh no longer completes Morphology.");
        });

        Check("Blob labeling, area gate, reveal, quantization and stage-preserving refresh", () =>
        {
            BinaryLearnPresenter presenter = new();
            Require(presenter.BlobAreas.SequenceEqual(new[] { 4, 5, 1 }) && presenter.BlobCandidateCount == 3,
                "Fixed connected candidates changed.");
            Require(presenter.BlobFormulaText == "ResultCount = 2 / Areas: A=4, B=5, C=1"
                && presenter.BlobAnimationStatusText == "완료 · 통과 2 / 전체 3", "Default area gate changed.");
            presenter.ResetBlobAnimation();
            Require(presenter.GetBlobCellText(0) == "0" && presenter.GetBlobCellText(7) == "."
                && presenter.GetBlobCellKind(7) == BinaryLearnPresenter.CellKind.Pending,
                "Reset must keep background and hide candidates.");
            presenter.AdvanceBlobAnimation();
            Require(presenter.BlobAnimationStatusText == "후보 A · 면적 4 px · 통과 · 1 / 3"
                && presenter.GetBlobCellText(7) == "A" && presenter.GetBlobCellText(10) == ".",
                "First candidate reveal changed.");
            Require(presenter.GetBlobCellKind(7) == BinaryLearnPresenter.CellKind.Candidate, "First accepted candidate highlight changed.");
            presenter.UpdateBlob(5);
            Require(presenter.BlobAnimationStep == 1 && presenter.GetBlobCellText(7) == "x"
                && presenter.GetBlobCellKind(7) == BinaryLearnPresenter.CellKind.Rejected,
                "Changing MIN_AREA must reevaluate the visible candidate without advancing/resetting.");
            presenter.AdvanceBlobAnimation();
            Require(presenter.GetBlobCellKind(10) == BinaryLearnPresenter.CellKind.Accepted
                && presenter.GetBlobCellText(10) == "B" && presenter.GetBlobCellText(24) == ".",
                "Second candidate or pending third candidate changed.");
            presenter.AdvanceBlobAnimation();
            Require(presenter.IsBlobAnimationComplete && presenter.GetBlobCellText(24) == "x"
                && presenter.BlobAnimationStatusText == "완료 · 통과 1 / 전체 3", "Final area decision changed.");
            presenter.AdvanceBlobAnimation();
            Require(presenter.BlobAnimationStep == 1, "Complete Blob frame did not restart at one.");
            foreach ((double minimum, int rounded, int accepted) in new (double, int, int)[]
            {
                (1, 1, 3), (2.5, 2, 2), (3.5, 4, 2), (5, 5, 1), (6, 6, 0), (0, 1, 3)
            })
            {
                presenter.UpdateBlob(minimum);
                Require(presenter.BlobMinimumArea == rounded && presenter.AcceptedBlobCount == accepted
                    && presenter.BlobAnimationStep == 1, "MIN_AREA rounding, count or stage retention changed.");
            }
        });

        Check("Contour accepted region, boundary/box precedence and independent stage transitions", () =>
        {
            BinaryLearnPresenter presenter = new();
            Require(presenter.ContourFormulaText == "Contour pixels = 11" && presenter.ContourAnimationStep == 3,
                "Initial contour evaluation changed.");
            Require(presenter.GetContourCellKind(34) == BinaryLearnPresenter.CellKind.Rejected
                && presenter.GetContourCellText(34) == "x", "Small disconnected pixel must be rejected.");
            presenter.ResetContourAnimation();
            Require(Enumerable.Range(0, 35).All(i => presenter.GetContourCellText(i) == "0")
                && !Enumerable.Range(0, 35).Any(presenter.IsContourInputHighlighted), "Reset should show neutral input stage.");
            presenter.AdvanceContourAnimation();
            Require(Enumerable.Range(0, 35).Count(i => presenter.GetContourCellKind(i) == BinaryLearnPresenter.CellKind.Region) == 11
                && presenter.IsContourInputHighlighted(34) && !presenter.IsContourRegionAccepted(34),
                "Area-filter stage or rejected input highlight changed.");
            presenter.UpdateContour("Bounding box");
            Require(presenter.ContourAnimationStep == 1 && presenter.ContourFormulaText == "BoundingBox = x1, y1, w4, h3",
                "Refresh must retain stage and fixed region bounds.");
            presenter.AdvanceContourAnimation();
            Require(Enumerable.Range(0, 35).Count(i => presenter.GetContourCellKind(i) == BinaryLearnPresenter.CellKind.Contour) == 11,
                "Second stage must show boundary regardless of selected final draw mode.");
            presenter.AdvanceContourAnimation();
            Require(Enumerable.Range(0, 35).Count(i => presenter.GetContourCellKind(i) == BinaryLearnPresenter.CellKind.Box) == 10
                && presenter.GetContourCellKind(16) == BinaryLearnPresenter.CellKind.Region,
                "Bounding-box-only final frame changed.");
            presenter.UpdateContour("Contour + box");
            Require(presenter.ContourFormulaText == "Contour pixels = 11, with BoundingBox"
                && presenter.GetContourCellKind(8) == BinaryLearnPresenter.CellKind.Box
                && presenter.GetContourCellKind(16) == BinaryLearnPresenter.CellKind.Contour,
                "Combined drawing must give box priority only on the box boundary.");
            presenter.AdvanceContourAnimation();
            Require(presenter.ContourAnimationStep == 1, "Complete contour frame did not restart at one.");
            presenter.CompleteContourAnimation();
            Require(presenter.ContourAnimationStatusText == "3 / 3 - 표시 방식: Contour + box", "Mode-change completion status changed.");
        });

        Check("Topic/instance state isolation and culture-stable numeric formulas", () =>
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                BinaryLearnPresenter first = new();
                BinaryLearnPresenter second = new();
                first.ResetMorphologyAnimation();
                first.UpdateBlob(6);
                first.ResetContourAnimation();
                Require(first.BlobAnimationStep == 3 && second.MorphologyAnimationStep == 25
                    && second.BlobMinimumArea == 3 && second.ContourAnimationStep == 3,
                    "Topics or independently hosted lessons shared state.");
                foreach (string culture in new[] { "ko-KR", "en-US", "fr-FR" })
                {
                    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                    Require(second.BlobFormulaText == "ResultCount = 2 / Areas: A=4, B=5, C=1"
                        && second.BlobMinimumAreaText == "3 px", "Culture changed fixed numeric formulas.");
                    second.UpdateContour("Bounding box");
                    Require(second.ContourFormulaText == "BoundingBox = x1, y1, w4, h3", "Culture changed bound text.");
                }
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        });

        string path = Path.Combine(evidenceDirectory, "learn-binary-presentation-contract.txt");
        File.WriteAllLines(path, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine($"Learn Binary Presentation: {passed.Count} passed, {failed.Count} failed. {path}");
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
