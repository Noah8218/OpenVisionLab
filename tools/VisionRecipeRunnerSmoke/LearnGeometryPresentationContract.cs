using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using static OpenVisionLab.DEFINE;

internal static class LearnGeometryPresentationContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();

        Check("Reset, Rotate, Scale and ROI review stages retain their exact lesson policy", () =>
        {
            GeometryLearnPresenter presenter = new();
            string[] statuses =
            {
                "0 / 3 - Reset: 원본 ROI와 좌표를 확인합니다.",
                "1 / 3 - Rotate: 중심 기준 25 deg 회전으로 좌표가 이동합니다.",
                "2 / 3 - Scale: 80% 적용 후 OutputSize~614x461",
                "3 / 3 - ROI review: 변환 결과에서 Rect, Template, Edge 방향, Pixel/mm 기준의 새 위치를 확인한 뒤 Preview/Run을 명시적으로 실행합니다."
            };
            presenter.UpdateSettings(25D, 80D);
            Require(presenter.AnimationStep == 3 && presenter.IsAnimationComplete, "Initial settings did not produce a complete review stage.");
            presenter.ResetAnimation();
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0) presenter.AdvanceAnimation();
                Require(presenter.AnimationStep == stage && presenter.AnimationStatusText == statuses[stage], "Geometry stage policy changed at stage " + stage + ".");
                Require(presenter.IsRotationApplied == (stage >= 1) && presenter.IsScaleApplied == (stage >= 2), "Geometry transform reveal ordering changed.");
                Require(presenter.SourceRole == (stage == 0 ? GeometryLearnRole.Candidate : GeometryLearnRole.Default)
                    && presenter.TargetRole == (stage == 1 ? GeometryLearnRole.Candidate : stage == 2 ? GeometryLearnRole.Pass : stage == 3 ? GeometryLearnRole.Warning : GeometryLearnRole.Default),
                    "Geometry semantic roles changed at stage " + stage + ".");
            }
            presenter.AdvanceAnimation();
            Require(presenter.AnimationStep == 1 && presenter.IsRotationApplied && !presenter.IsScaleApplied, "Completed Geometry lesson did not restart at Rotate.");
        });

        Check("Settings calculate invariant text and output dimensions without UI dependencies", () =>
        {
            GeometryLearnPresenter presenter = new();
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                foreach (string culture in new[] { "ko-KR", "en-US", "fr-FR" })
                {
                    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                    presenter.UpdateSettings(25D, 80D);
                    Require(presenter.AngleText == "25 deg" && presenter.ScaleText == "80%"
                        && presenter.FormulaText == "RotateScale: Angle=25 deg, Scale=80%, OutputSize~614x461"
                        && presenter.OutputWidth == 614 && presenter.OutputHeight == 461,
                        "Geometry settings became culture dependent under " + culture + ".");
                    presenter.ResetAnimation();
                    Require(presenter.AnimationStep == 0 && presenter.Angle == 25D && presenter.Scale == 80D, "Reset changed Geometry settings.");
                }
            }
            finally { CultureInfo.CurrentCulture = previous; }
        });

        Check("Presenter instances and read-only projections remain independent", () =>
        {
            GeometryLearnPresenter first = new();
            GeometryLearnPresenter second = new();
            first.UpdateSettings(-15D, 125D);
            first.ResetAnimation();
            first.AdvanceAnimation();
            string formula = first.FormulaText;
            string status = first.AnimationStatusText;
            int width = first.OutputWidth;
            int height = first.OutputHeight;
            Require(first.FormulaText == formula && first.AnimationStatusText == status && first.OutputWidth == width && first.OutputHeight == height,
                "Reading Geometry projections mutated presenter state.");
            Require(first.AnimationStep == 1 && first.Angle == -15D && first.Scale == 125D && second.AnimationStep == 3
                && second.Angle == 15D && second.Scale == 100D, "Geometry presenter instances shared mutable state.");
        });

        Check("Tool location mappings preserve explicit hints and ignore unrelated menus", () =>
        {
            GeometryLearnPresenter presenter = new();
            string initialTitle = presenter.ToolLocationTitle;
            string initialDetail = presenter.ToolLocationDetail;
            presenter.UpdateToolLocation(VISION_MENU.RotateAndScale);
            Require(presenter.ToolLocationTitle == "열림: Rotate / Scale | 찾을 위치: Input/Output Layer, Angle, Scale X, Scale Y"
                && presenter.ToolLocationDetail == "Rotate / Scale에서 Angle과 Scale X/Y를 설정하고 Preview 결과의 OutputSize와 영상 방향을 확인하세요.", "RotateScale hint changed.");
            presenter.UpdateToolLocation(VISION_MENU.AffineTransform);
            Require(presenter.ToolLocationTitle == "열림: Affine Transform | 찾을 위치: Source/Destination Points, Output, Sampling, Validation Gates"
                && presenter.ToolLocationDetail == "대응하는 세 점을 같은 순서로 입력하고 Preview에서 destination triangle, transformed frame, Affine 2x3 행렬, AffineValidPixelRatio를 확인하세요.", "Affine hint changed.");
            presenter.UpdateToolLocation(VISION_MENU.Threshold);
            Require(presenter.ToolLocationTitle == "열림: Affine Transform | 찾을 위치: Source/Destination Points, Output, Sampling, Validation Gates"
                && presenter.ToolLocationDetail.Contains("destination triangle", StringComparison.Ordinal), "Unrelated menu replaced the Geometry hint.");
            GeometryLearnPresenter fresh = new();
            fresh.UpdateToolLocation(VISION_MENU.Threshold);
            Require(fresh.ToolLocationTitle == initialTitle && fresh.ToolLocationDetail == initialDetail, "Initial Geometry hint changed for an unrelated menu.");
        });

        string path = Path.Combine(evidenceDirectory, "learn-geometry-presentation-contract.txt");
        File.WriteAllLines(path, passed.ConvertAll(item => "PASS: " + item).ToArray());
        if (failed.Count > 0)
            File.AppendAllLines(path, failed.ConvertAll(item => "FAIL: " + item));
        Console.WriteLine($"Learn Geometry Presentation: {passed.Count} passed, {failed.Count} failed. {path}");
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
