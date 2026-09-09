using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static OpenVisionLab.DEFINE;

internal static class LearnFoundationPresentationContract
{
    internal static int Run(string evidenceDirectory)
    {
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();
        Check("Point, Size, Rect, ROI and RotatedRect stages retain their exact teaching geometry", () =>
        {
            FoundationLearnPresenter presenter = new();
            int[] roiIndices = { 10, 11, 12, 13, 18, 19, 20, 21, 26, 27, 28, 29 };
            string[] statuses =
            {
                "0 / 5 - Mat: 6행 x 8열의 픽셀 격자입니다.",
                "1 / 5 - Point(2,1): X=2 열, Y=1 행의 위치입니다.",
                "2 / 5 - Size(4,3): Width=4 열, Height=3 행이며 위치 정보는 없습니다.",
                "3 / 5 - Rect(2,1,4,3): Point와 Size를 결합하면 CvROI=2,1,4,3이 됩니다.",
                "4 / 5 - Mat ROI: CvROI(2,1,4,3)가 선택한 12개 픽셀을 별도 Mat 영역으로 보여줍니다.",
                "5 / 5 - RotatedRect: Center(4,2.5), Size(4,3), Angle(25°). 점선은 회전 영역 전체를 감싸는 BoundingRect입니다."
            };
            Require(FoundationLearnPresenter.FoundationCellCount == 48 && presenter.FoundationAnimationStep == 5
                && presenter.IsFoundationAnimationComplete, "Initial complete stage or 6x8 sample size changed.");
            presenter.ResetFoundationAnimation();
            for (int stage = 0; stage <= 5; stage++)
            {
                if (stage > 0) presenter.AdvanceFoundationAnimation();
                Require(presenter.FoundationAnimationStatusText == statuses[stage], "Foundation stage wording changed.");
                Require(presenter.IsFoundationPointVisible == (stage is >= 1 and <= 4)
                    && presenter.IsFoundationRectVisible == (stage is >= 2 and <= 4)
                    && presenter.IsFoundationRotatedRectVisible == (stage == 5), "Marker visibility changed across Point/Size/Rect/RotatedRect stages.");
                Require(presenter.FoundationSelectedCellCount == (stage == 3 || stage == 4 ? 12 : 0), "ROI-selected cell count changed.");
                Require(presenter.FoundationPointRole == (stage >= 4 ? FoundationLearnRole.Pass : stage == 1 ? FoundationLearnRole.Warning : FoundationLearnRole.Candidate)
                    && presenter.FoundationRectRole == (stage >= 4 ? FoundationLearnRole.Pass : FoundationLearnRole.Candidate), "Point/ROI semantic role changed.");
                for (int index = 0; index < FoundationLearnPresenter.FoundationCellCount; index++)
                {
                    FoundationLearnRole expected = stage == 1 && index == 10 ? FoundationLearnRole.Warning
                        : (stage == 3 || stage == 4) && roiIndices.Contains(index)
                            ? stage == 4 ? FoundationLearnRole.Pass : FoundationLearnRole.Candidate : FoundationLearnRole.Default;
                    Require(presenter.GetFoundationCellRole(index) == expected, $"Unexpected cell selection at stage {stage}, index {index}.");
                }
            }
            presenter.AdvanceFoundationAnimation();
            Require(presenter.FoundationAnimationStep == 1 && presenter.GetFoundationCellRole(10) == FoundationLearnRole.Warning,
                "Step after completed RotatedRect must restart at Point stage 1.");
        });
        Check("BGR, split channels, Gray and Mat type reveal stages retain exact meanings", () =>
        {
            FoundationLearnPresenter presenter = new();
            string[] statuses =
            {
                "0 / 4 - 한 픽셀은 Mat의 행, 열, 채널 위치에 저장됩니다.",
                "1 / 4 - BGR 원본: OpenCV 채널 순서는 B, G, R이며 값은 220, 110, 40입니다.",
                "2 / 4 - 채널 분리: B=220, G=110, R=40이며 BGR Mat 크기는 행 x 열 x 3입니다.",
                "3 / 4 - Gray 변환: 0.114B + 0.587G + 0.299R = 102이며 Gray Mat 크기는 행 x 열 x 1입니다.",
                "4 / 4 - Mat 형식: CV_8U는 0~255의 8비트 값이고 C1/C3은 채널 수입니다. Gray=CV_8UC1, BGR=CV_8UC3."
            };
            Require(presenter.MatChannelAnimationStep == 4 && presenter.IsMatChannelAnimationComplete, "Initial Mat type stage changed.");
            presenter.ResetMatChannelAnimation();
            for (int stage = 0; stage <= 4; stage++)
            {
                if (stage > 0) presenter.AdvanceMatChannelAnimation();
                Require(presenter.MatChannelAnimationStatusText == statuses[stage], "Mat-channel stage wording changed.");
                Require(presenter.AreMatSplitChannelsRevealed == (stage >= 2) && presenter.IsMatGrayChannelRevealed == (stage >= 3)
                    && presenter.IsMatTypeGuideRevealed == (stage >= 4), "Mat-channel reveal ordering changed.");
                Require(presenter.MatBgrBorderRole == (stage == 1 ? FoundationLearnRole.Warning : FoundationLearnRole.Default)
                    && presenter.MatSplitBorderRole == (stage == 2 ? FoundationLearnRole.Candidate : FoundationLearnRole.Default)
                    && presenter.MatGrayBorderRole == (stage >= 3 ? FoundationLearnRole.Pass : FoundationLearnRole.Default), "Mat-channel stage role changed.");
            }
            presenter.AdvanceMatChannelAnimation();
            Require(presenter.MatChannelAnimationStep == 1 && !presenter.AreMatSplitChannelsRevealed,
                "Step after completed Mat type guide must restart at BGR stage 1.");
        });
        Check("Refresh observations, reset, parallel lesson states and separate instances remain independent", () =>
        {
            FoundationLearnPresenter first = new();
            FoundationLearnPresenter second = new();
            first.ResetFoundationAnimation();
            first.ResetMatChannelAnimation();
            first.AdvanceFoundationAnimation();
            first.AdvanceFoundationAnimation();
            first.AdvanceFoundationAnimation();
            first.AdvanceMatChannelAnimation();
            string foundationStatus = first.FoundationAnimationStatusText;
            string matStatus = first.MatChannelAnimationStatusText;
            for (int i = 0; i < FoundationLearnPresenter.FoundationCellCount; i++)
                _ = first.GetFoundationCellRole(i);
            Require(first.FoundationAnimationStep == 3 && first.FoundationSelectedCellCount == 12 && first.MatChannelAnimationStep == 1
                && first.FoundationAnimationStatusText == foundationStatus && first.MatChannelAnimationStatusText == matStatus, "A frame refresh mutated lesson state.");
            first.UpdateToolLocation(VISION_MENU.Blob);
            first.ResetFoundationAnimation();
            Require(first.FoundationAnimationStep == 0 && first.MatChannelAnimationStep == 1
                && first.ToolLocationTitle == "열림: Blob | 찾을 위치: PropertyGrid > ROI > Use ROI / ROI (CvROI)", "Foundation reset changed another stage or the opened-tool hint.");
            first.ResetMatChannelAnimation();
            Require(first.FoundationAnimationStep == 0 && first.MatChannelAnimationStep == 0, "Mat reset changed Foundation state.");
            Require(second.FoundationAnimationStep == 5 && second.MatChannelAnimationStep == 4
                && second.ToolLocationTitle == "도구 위치: Blob > ROI | Filter > Kernel | Rotate/Scale > Angle/Scale", "Separate presenters shared mutable state.");
        });
        Check("Tool location mappings retain specific hints and the Threshold fallback", () =>
        {
            FoundationLearnPresenter presenter = new();
            const string commonDetail = "강조된 PropertyGrid 항목과 입력 레이어를 확인하고 Preview에서 값의 영향을 비교하세요.";
            (VISION_MENU Menu, string Title, string Detail)[] cases =
            {
                (VISION_MENU.Blob, "열림: Blob | 찾을 위치: PropertyGrid > ROI > Use ROI / ROI (CvROI)", commonDetail),
                (VISION_MENU.Filter, "열림: Filter | 찾을 위치: Filter options > Kernel Width / Kernel Height", commonDetail),
                (VISION_MENU.RotateAndScale, "열림: Rotate / Scale | 찾을 위치: Angle / Scale X / Scale Y", "Angle과 Scale X/Y를 설정하고 Preview 결과에서 OutputSize와 영상 방향을 확인하세요."),
                (VISION_MENU.AffineTransform, "열림: Affine Transform | 찾을 위치: Source/Destination 3 points / Validation Gates", "세 대응점을 입력하고 Preview에서 destination/frame 드로잉과 valid-pixel ratio를 확인하세요."),
                (VISION_MENU.Threshold, "열림: Threshold | 찾을 위치: Parameter panel", commonDetail)
            };
            Require(presenter.ToolLocationDetail == "도구가 열리면 위 경로에서 같은 값을 찾고, Preview를 눌러 결과 이미지와 비교하세요.", "Initial tool guidance changed.");
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                foreach (string culture in new[] { "ko-KR", "en-US", "fr-FR" })
                {
                    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                    foreach (var item in cases)
                    {
                        presenter.UpdateToolLocation(item.Menu);
                        Require(presenter.ToolLocationTitle == item.Title && presenter.ToolLocationDetail == item.Detail,
                            "Tool location content changed for " + item.Menu + " under " + culture + ".");
                        Require(presenter.FoundationAnimationStep == 5 && presenter.MatChannelAnimationStep == 4,
                            "Tool location update changed lesson stages.");
                    }
                }
            }
            finally { CultureInfo.CurrentCulture = previous; }
        });
        string path = Path.Combine(evidenceDirectory, "learn-foundation-presentation-contract.txt");
        File.WriteAllLines(path, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine($"Learn Foundation Presentation: {passed.Count} passed, {failed.Count} failed. {path}");
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
