using System;
using System.Globalization;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal enum GeometryLearnRole
    {
        Default,
        Candidate,
        Pass,
        Warning
    }

    /// <summary>Owns Geometry Transform lesson state and text; the View owns WPF controls and timer lifetime.</summary>
    internal sealed class GeometryLearnPresenter
    {
        internal const int AnimationStepCount = 3;
        internal const double SourceWidth = 768D;
        internal const double SourceHeight = 576D;

        internal double Angle { get; private set; } = 15D;
        internal double Scale { get; private set; } = 100D;
        internal int AnimationStep { get; private set; } = AnimationStepCount;
        internal bool IsAnimationComplete => AnimationStep >= AnimationStepCount;
        internal bool IsRotationApplied => AnimationStep >= 1;
        internal bool IsScaleApplied => AnimationStep >= 2;
        internal int OutputWidth => (int)Math.Round(SourceWidth * Scale / 100D);
        internal int OutputHeight => (int)Math.Round(SourceHeight * Scale / 100D);
        internal GeometryLearnRole SourceRole => AnimationStep == 0 ? GeometryLearnRole.Candidate : GeometryLearnRole.Default;
        internal GeometryLearnRole TargetRole => AnimationStep switch
        {
            1 => GeometryLearnRole.Candidate,
            2 => GeometryLearnRole.Pass,
            3 => GeometryLearnRole.Warning,
            _ => GeometryLearnRole.Default
        };

        internal string AngleText => Angle.ToString("0", CultureInfo.InvariantCulture) + " deg";
        internal string ScaleText => Scale.ToString("0", CultureInfo.InvariantCulture) + "%";
        internal string FormulaText => "RotateScale: Angle="
            + Angle.ToString("0", CultureInfo.InvariantCulture)
            + " deg, Scale="
            + Scale.ToString("0", CultureInfo.InvariantCulture)
            + "%, OutputSize~"
            + OutputWidth.ToString(CultureInfo.InvariantCulture)
            + "x"
            + OutputHeight.ToString(CultureInfo.InvariantCulture);
        internal string MeaningText => "회전과 배율이 바뀌면 기존 ROI와 측정점의 좌표도 달라집니다. Preview 결과에서 새 위치를 확인하고 ROI를 다시 맞추세요.";
        internal string AnimationStatusText => AnimationStep switch
        {
            0 => "0 / 3 - Reset: 원본 ROI와 좌표를 확인합니다.",
            1 => "1 / 3 - Rotate: 중심 기준 " + Angle.ToString("0", CultureInfo.InvariantCulture) + " deg 회전으로 좌표가 이동합니다.",
            2 => "2 / 3 - Scale: "
                + Scale.ToString("0", CultureInfo.InvariantCulture)
                + "% 적용 후 OutputSize~"
                + OutputWidth.ToString(CultureInfo.InvariantCulture)
                + "x"
                + OutputHeight.ToString(CultureInfo.InvariantCulture),
            _ => "3 / 3 - ROI review: 변환 결과에서 Rect, Template, Edge 방향, Pixel/mm 기준의 새 위치를 확인한 뒤 Preview/Run을 명시적으로 실행합니다."
        };

        internal string ToolLocationTitle { get; private set; } = "Rotate/Scale: Angle, Scale X/Y | Affine: Source/Destination 3점, Output, Validation Gates";
        internal string ToolLocationDetail { get; private set; } = "Preview를 명시적으로 실행한 뒤 OutputSize, Affine 2x3 행렬, destination triangle/frame 드로잉, AffineValidPixelRatio를 확인합니다.";

        internal void UpdateSettings(double angle, double scale)
        {
            Angle = angle;
            Scale = scale;
            AnimationStep = AnimationStepCount;
        }

        internal void ResetAnimation() => AnimationStep = 0;

        internal void AdvanceAnimation() => AnimationStep = IsAnimationComplete ? 1 : AnimationStep + 1;

        internal void UpdateToolLocation(VISION_MENU menu)
        {
            ToolLocationTitle = menu switch
            {
                VISION_MENU.RotateAndScale => "열림: Rotate / Scale | 찾을 위치: Input/Output Layer, Angle, Scale X, Scale Y",
                VISION_MENU.AffineTransform => "열림: Affine Transform | 찾을 위치: Source/Destination Points, Output, Sampling, Validation Gates",
                _ => ToolLocationTitle
            };
            ToolLocationDetail = menu switch
            {
                VISION_MENU.RotateAndScale => "Rotate / Scale에서 Angle과 Scale X/Y를 설정하고 Preview 결과의 OutputSize와 영상 방향을 확인하세요.",
                VISION_MENU.AffineTransform => "대응하는 세 점을 같은 순서로 입력하고 Preview에서 destination triangle, transformed frame, Affine 2x3 행렬, AffineValidPixelRatio를 확인하세요.",
                _ => ToolLocationDetail
            };
        }
    }
}
