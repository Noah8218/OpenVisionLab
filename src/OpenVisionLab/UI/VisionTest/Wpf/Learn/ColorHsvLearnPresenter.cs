using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal readonly record struct ColorHsvLearnGuide(
        int Hue,
        int Value,
        int HueMin,
        int HueMax,
        int SaturationMinimum,
        int VisibleStep,
        bool HuePass,
        bool SaturationPass,
        bool ValuePass,
        bool MaskPass,
        string HueText,
        string ValueText,
        string FormulaText,
        string MeaningText,
        string Vec3bTypeText,
        string ScalarBoundsText,
        string AnimationStatusText)
    {
        internal const int SampleHue = 45;
        internal const int SampleSaturation = 221;
        internal const int SampleValue = 185;
    }

    /// <summary>
    /// Owns Color/HSV lesson state and text decisions. The Learn Window keeps
    /// only WPF timer and control projection responsibilities.
    /// </summary>
    internal sealed class ColorHsvLearnPresenter
    {
        internal const int AnimationStepCount = 4;
        private int animationStep = AnimationStepCount;

        internal int AnimationStep => animationStep;

        internal bool IsAnimationComplete => animationStep >= AnimationStepCount;

        internal void ResetAnimation() => animationStep = 0;

        internal void CompleteAnimation() => animationStep = AnimationStepCount;

        internal void AdvanceAnimation()
        {
            if (animationStep >= AnimationStepCount)
            {
                animationStep = 0;
            }

            animationStep++;
        }

        internal ColorHsvLearnGuide BuildGuide(double hueValue, double valueValue)
        {
            int hue = Clamp((int)Math.Round(hueValue), 0, 179);
            int value = Clamp((int)Math.Round(valueValue), 40, 255);
            int hueMin = Math.Max(0, hue - 10);
            int hueMax = Math.Min(179, hue + 10);
            const int saturationMinimum = 60;
            int visibleStep = Clamp(animationStep, 0, AnimationStepCount);
            bool huePass = ColorHsvLearnGuide.SampleHue >= hueMin && ColorHsvLearnGuide.SampleHue <= hueMax;
            bool saturationPass = ColorHsvLearnGuide.SampleSaturation >= saturationMinimum;
            bool valuePass = ColorHsvLearnGuide.SampleValue >= value;
            bool maskPass = huePass && saturationPass && valuePass;

            string hueText = hue.ToString(CultureInfo.InvariantCulture) + " / 179";
            string valueText = value.ToString(CultureInfo.InvariantCulture) + " / 255";
            string formulaText = "HSV mask: H="
                + hueMin.ToString(CultureInfo.InvariantCulture)
                + ".."
                + hueMax.ToString(CultureInfo.InvariantCulture)
                + ", S>="
                + saturationMinimum.ToString(CultureInfo.InvariantCulture)
                + ", V>="
                + value.ToString(CultureInfo.InvariantCulture)
                + " -> OutputLayer=HSV_Mask, metric=MaskPixelRatio 또는 후속 ResultCount/Area";
            string meaningText = value < 110
                ? "Value가 낮으면 색이 어둡습니다. Hue만으로 영역이 불안정하면 Mean/Histogram의 밝기 분포도 함께 확인하세요."
                : "Hue는 색상 계열, Saturation은 회색 배경과의 차이, Value는 어두운 픽셀을 구분하는 기준입니다.";
            string vec3bTypeText = "변환된 HSV Mat 픽셀 = Vec3b(H,S,V) = (45,221,185): 0~255의 8비트 채널 값 3개";
            string scalarBoundsText = "lower = Scalar("
                + hueMin.ToString(CultureInfo.InvariantCulture)
                + ",60,"
                + value.ToString(CultureInfo.InvariantCulture)
                + ") | upper = Scalar("
                + hueMax.ToString(CultureInfo.InvariantCulture)
                + ",255,255); Scalar는 값 4개를 담고 HSV에서는 앞의 3개를 사용합니다.";
            string animationStatusText = visibleStep switch
            {
                0 => "0 / 4 - BGR 입력: Vec3b(B,G,R)=(25,185,105) 픽셀부터 확인합니다.",
                1 => "1 / 4 - Cv2.Split: B=25, G=185, R=105인 CV_8UC1 채널 Mat 3개로 분리합니다.",
                2 => "2 / 4 - Cv2.Merge로 BGR을 복원하고 Cv2.CvtColor(BGR2HSV)로 H=45, S=221, V=185를 얻습니다.",
                3 => "3 / 4 - 범위 판정: H "
                    + (huePass ? "OK" : "NG")
                    + ", S "
                    + (saturationPass ? "OK" : "NG")
                    + ", V "
                    + (valuePass ? "OK" : "NG")
                    + " -> "
                    + (maskPass ? "IN RANGE" : "OUT OF RANGE"),
                _ => "4 / 4 - Mask="
                    + (maskPass ? "255" : "0")
                    + ": MaskPixelRatio와 후속 ResultCount/Area를 Preview/Run 후 검토합니다."
            };

            return new ColorHsvLearnGuide(
                hue,
                value,
                hueMin,
                hueMax,
                saturationMinimum,
                visibleStep,
                huePass,
                saturationPass,
                valuePass,
                maskPass,
                hueText,
                valueText,
                formulaText,
                meaningText,
                vec3bTypeText,
                scalarBoundsText,
                animationStatusText);
        }

        private static int Clamp(int value, int minimum, int maximum) => Math.Max(minimum, Math.Min(maximum, value));
    }
}
