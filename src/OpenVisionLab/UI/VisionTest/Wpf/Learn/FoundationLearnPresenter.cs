using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal enum FoundationLearnRole { Default, Candidate, Pass, Warning }

    /// <summary>Owns the fixed Point/ROI and Mat-channel lesson stages; the View owns drawings and timer lifetime.</summary>
    internal sealed class FoundationLearnPresenter
    {
        internal const int FoundationCellCount = 48;
        internal const int FoundationAnimationStepCount = 5;
        internal const int MatChannelAnimationStepCount = 4;

        internal int FoundationAnimationStep { get; private set; } = FoundationAnimationStepCount;
        internal int MatChannelAnimationStep { get; private set; } = MatChannelAnimationStepCount;
        internal bool IsFoundationAnimationComplete => FoundationAnimationStep >= FoundationAnimationStepCount;
        internal bool IsMatChannelAnimationComplete => MatChannelAnimationStep >= MatChannelAnimationStepCount;
        internal int FoundationSelectedCellCount => FoundationAnimationStep >= 3 && FoundationAnimationStep <= 4 ? 12 : 0;
        internal bool IsFoundationPointVisible => FoundationAnimationStep >= 1 && FoundationAnimationStep <= 4;
        internal bool IsFoundationRectVisible => FoundationAnimationStep >= 2 && FoundationAnimationStep <= 4;
        internal bool IsFoundationRotatedRectVisible => FoundationAnimationStep >= 5;
        internal FoundationLearnRole FoundationPointRole => FoundationAnimationStep >= 4 ? FoundationLearnRole.Pass
            : FoundationAnimationStep == 1 ? FoundationLearnRole.Warning : FoundationLearnRole.Candidate;
        internal FoundationLearnRole FoundationRectRole => FoundationAnimationStep >= 4 ? FoundationLearnRole.Pass : FoundationLearnRole.Candidate;
        internal bool AreMatSplitChannelsRevealed => MatChannelAnimationStep >= 2;
        internal bool IsMatGrayChannelRevealed => MatChannelAnimationStep >= 3;
        internal bool IsMatTypeGuideRevealed => MatChannelAnimationStep >= 4;
        internal FoundationLearnRole MatBgrBorderRole => MatChannelAnimationStep == 1 ? FoundationLearnRole.Warning : FoundationLearnRole.Default;
        internal FoundationLearnRole MatSplitBorderRole => MatChannelAnimationStep == 2 ? FoundationLearnRole.Candidate : FoundationLearnRole.Default;
        internal FoundationLearnRole MatGrayBorderRole => MatChannelAnimationStep >= 3 ? FoundationLearnRole.Pass : FoundationLearnRole.Default;

        internal string FoundationAnimationStatusText => FoundationAnimationStep switch
        {
            0 => "0 / 5 - Mat: 6행 x 8열의 픽셀 격자입니다.",
            1 => "1 / 5 - Point(2,1): X=2 열, Y=1 행의 위치입니다.",
            2 => "2 / 5 - Size(4,3): Width=4 열, Height=3 행이며 위치 정보는 없습니다.",
            3 => "3 / 5 - Rect(2,1,4,3): Point와 Size를 결합하면 CvROI=2,1,4,3이 됩니다.",
            4 => "4 / 5 - Mat ROI: CvROI(2,1,4,3)가 선택한 12개 픽셀을 별도 Mat 영역으로 보여줍니다.",
            _ => "5 / 5 - RotatedRect: Center(4,2.5), Size(4,3), Angle(25°). 점선은 회전 영역 전체를 감싸는 BoundingRect입니다."
        };

        internal string MatChannelAnimationStatusText => MatChannelAnimationStep switch
        {
            0 => "0 / 4 - 한 픽셀은 Mat의 행, 열, 채널 위치에 저장됩니다.",
            1 => "1 / 4 - BGR 원본: OpenCV 채널 순서는 B, G, R이며 값은 220, 110, 40입니다.",
            2 => "2 / 4 - 채널 분리: B=220, G=110, R=40이며 BGR Mat 크기는 행 x 열 x 3입니다.",
            3 => "3 / 4 - Gray 변환: 0.114B + 0.587G + 0.299R = 102이며 Gray Mat 크기는 행 x 열 x 1입니다.",
            _ => "4 / 4 - Mat 형식: CV_8U는 0~255의 8비트 값이고 C1/C3은 채널 수입니다. Gray=CV_8UC1, BGR=CV_8UC3."
        };

        internal string ToolLocationTitle { get; private set; } = "도구 위치: Blob > ROI | Filter > Kernel | Rotate/Scale > Angle/Scale";
        internal string ToolLocationDetail { get; private set; } = "도구가 열리면 위 경로에서 같은 값을 찾고, Preview를 눌러 결과 이미지와 비교하세요.";

        internal void ResetFoundationAnimation() => FoundationAnimationStep = 0;
        internal void ResetMatChannelAnimation() => MatChannelAnimationStep = 0;
        internal void AdvanceFoundationAnimation() => FoundationAnimationStep = IsFoundationAnimationComplete ? 1 : FoundationAnimationStep + 1;
        internal void AdvanceMatChannelAnimation() => MatChannelAnimationStep = IsMatChannelAnimationComplete ? 1 : MatChannelAnimationStep + 1;

        internal FoundationLearnRole GetFoundationCellRole(int index)
        {
            int x = index % 8;
            int y = index / 8;
            bool inRoi = x >= 2 && x < 6 && y >= 1 && y < 4;
            return FoundationSelectedCellCount > 0 && inRoi
                ? FoundationAnimationStep >= 4 ? FoundationLearnRole.Pass : FoundationLearnRole.Candidate
                : FoundationAnimationStep == 1 && x == 2 && y == 1 ? FoundationLearnRole.Warning : FoundationLearnRole.Default;
        }

        internal void UpdateToolLocation(VISION_MENU menu)
        {
            ToolLocationTitle = menu switch
            {
                VISION_MENU.Blob => "열림: Blob | 찾을 위치: PropertyGrid > ROI > Use ROI / ROI (CvROI)",
                VISION_MENU.Filter => "열림: Filter | 찾을 위치: Filter options > Kernel Width / Kernel Height",
                VISION_MENU.RotateAndScale => "열림: Rotate / Scale | 찾을 위치: Angle / Scale X / Scale Y",
                VISION_MENU.AffineTransform => "열림: Affine Transform | 찾을 위치: Source/Destination 3 points / Validation Gates",
                _ => "열림: " + menu + " | 찾을 위치: Parameter panel"
            };
            ToolLocationDetail = menu switch
            {
                VISION_MENU.RotateAndScale => "Angle과 Scale X/Y를 설정하고 Preview 결과에서 OutputSize와 영상 방향을 확인하세요.",
                VISION_MENU.AffineTransform => "세 대응점을 입력하고 Preview에서 destination/frame 드로잉과 valid-pixel ratio를 확인하세요.",
                _ => "강조된 PropertyGrid 항목과 입력 레이어를 확인하고 Preview에서 값의 영향을 비교하세요."
            };
        }
    }
}
