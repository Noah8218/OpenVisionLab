using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal enum GrayscaleLearnCellRole { Default, Neutral, Candidate, Pass, Warning, Gray }

    internal readonly record struct GrayscaleLearnCell(string Text, int Gray, GrayscaleLearnCellRole BackgroundRole, GrayscaleLearnCellRole BorderRole);

    /// <summary>Owns grayscale lesson state and presentation; the existing simulation model owns all pixel calculations.</summary>
    internal sealed class GrayscaleLearnPresenter
    {
        internal const int BrightnessAnimationStepCount = 3;
        internal const int ArithmeticAnimationStepCount = 3;
        internal const int FilterAnimationStepCount = 3;
        private OpenVisionLearnBasicGrayscaleSimulationModel.ThresholdEvaluation thresholdEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateThreshold(127, false, 255);
        private OpenVisionLearnBasicGrayscaleSimulationModel.BrightnessEvaluation brightnessEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateBrightness(35);
        private OpenVisionLearnBasicGrayscaleSimulationModel.ArithmeticEvaluation arithmeticEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateArithmetic("AbsDiff");
        private OpenVisionLearnBasicGrayscaleSimulationModel.FilterEvaluation filterEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateFilter("Mean blur");
        private bool thresholdInverted;
        private bool thresholdAnimationForward = true;
        private string arithmeticMode = "AbsDiff";
        private string filterMode = "Mean blur";

        internal IReadOnlyList<int> ThresholdSamples => OpenVisionLearnBasicGrayscaleSimulationModel.ThresholdSamples;
        internal IReadOnlyList<int> BrightnessSamples => OpenVisionLearnBasicGrayscaleSimulationModel.BrightnessSamples;
        internal IReadOnlyList<int> ArithmeticInputA => OpenVisionLearnBasicGrayscaleSimulationModel.ArithmeticInputA;
        internal IReadOnlyList<int> ArithmeticInputB => OpenVisionLearnBasicGrayscaleSimulationModel.ArithmeticInputB;
        internal IReadOnlyList<int> FilterSamples => OpenVisionLearnBasicGrayscaleSimulationModel.FilterSamples;
        internal IReadOnlyList<int> BrightnessHistogramBins => brightnessEvaluation.HistogramBins;
        internal int MaximumValue { get; private set; } = 255;
        internal int BrightnessAnimationStep { get; private set; } = BrightnessAnimationStepCount;
        internal int ArithmeticAnimationStep { get; private set; } = ArithmeticAnimationStepCount;
        internal int FilterAnimationStep { get; private set; } = FilterAnimationStepCount;
        internal bool IsBrightnessAnimationComplete => BrightnessAnimationStep >= BrightnessAnimationStepCount;
        internal bool IsArithmeticAnimationComplete => ArithmeticAnimationStep >= ArithmeticAnimationStepCount;
        internal bool IsFilterAnimationComplete => FilterAnimationStep >= FilterAnimationStepCount;
        internal bool ShowBrightnessHistogram => BrightnessAnimationStep >= 3;
        internal GrayscaleLearnCellRole BrightnessInputBorderRole => BrightnessAnimationStep == 1 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default;
        internal GrayscaleLearnCellRole ArithmeticInputBorderRole => ArithmeticAnimationStep == 1 || ArithmeticAnimationStep == 2 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default;

        internal string MaximumValueText => "MaxValue = " + MaximumValue.ToString(CultureInfo.InvariantCulture);
        internal string ThresholdValueText => "Threshold = " + thresholdEvaluation.Threshold.ToString(CultureInfo.InvariantCulture);
        internal string ThresholdFormulaText => thresholdInverted
            ? "BinaryInv: GV >= threshold -> 0, GV < threshold -> MaxValue"
            : "Binary: GV >= threshold -> MaxValue, GV < threshold -> 0";
        internal string BrightnessOffsetText => "Offset = " + FormatSigned(brightnessEvaluation.Offset) + " GV";
        internal string BrightnessFormulaText => "Result GV = clamp(Source GV " + FormatSigned(brightnessEvaluation.Offset) + ", 0, 255)";

        internal string BrightnessAnimationStatusText
        {
            get
            {
                string direction = brightnessEvaluation.ResultAverage > brightnessEvaluation.SourceAverage ? "오른쪽"
                    : brightnessEvaluation.ResultAverage < brightnessEvaluation.SourceAverage ? "왼쪽" : "같은 위치";
                return BrightnessAnimationStep switch
                {
                    0 => "0 / 3 - Reset: 원본 GV부터 확인합니다.",
                    1 => "1 / 3 - Input GV: 각 픽셀의 밝기 값을 읽습니다.",
                    2 => "2 / 3 - Brightness: GV " + FormatSigned(brightnessEvaluation.Offset) + " 후 0~255로 제한합니다.",
                    _ => "3 / 3 - Histogram shift: 평균 GV " + brightnessEvaluation.SourceAverage.ToString(CultureInfo.InvariantCulture)
                        + " -> " + brightnessEvaluation.ResultAverage.ToString(CultureInfo.InvariantCulture)
                        + ", 분포가 " + direction + "으로 이동합니다."
                };
            }
        }

        internal string ArithmeticFormulaText => arithmeticMode + ": A/B -> Output";
        internal string ArithmeticMeaningText => arithmeticMode switch
        {
            "Add" => "Add는 두 Layer 값을 더합니다. 결과가 255를 넘으면 255로 제한됩니다.",
            "Subtract" => "Subtract는 A에서 B를 빼며 0보다 작은 결과는 0으로 제한됩니다.",
            "Bitwise AND" => "Bitwise AND는 두 이진 마스크에서 모두 흰색인 픽셀만 남깁니다.",
            "Bitwise OR" => "Bitwise OR는 두 이진 마스크 중 하나라도 흰색인 픽셀을 남깁니다.",
            _ => "AbsDiff는 A와 B의 절대 차이를 계산해 달라진 픽셀을 강조합니다."
        };

        internal string ArithmeticAnimationStatusText
        {
            get
            {
                string firstResult = arithmeticEvaluation.Results[0].ToString(CultureInfo.InvariantCulture);
                string firstExpression = arithmeticMode switch
                {
                    "Add" => "clamp(20 + 10) = " + firstResult,
                    "Subtract" => "clamp(20 - 10) = " + firstResult,
                    "Bitwise AND" => "20 & 10 = " + firstResult,
                    "Bitwise OR" => "20 | 10 = " + firstResult,
                    _ => "|20 - 10| = " + firstResult
                };
                return ArithmeticAnimationStep switch
                {
                    0 => "0 / 3 - Reset: InputLayer A/B를 먼저 확인합니다.",
                    1 => "1 / 3 - Inputs: 8개 A/B 픽셀 쌍을 읽습니다.",
                    2 => "2 / 3 - " + arithmeticMode + " 적용: " + firstExpression,
                    _ => "3 / 3 - OutputLayer: 8개 연산 결과를 비교할 준비가 됐습니다."
                };
            }
        }

        internal string FilterFormulaText => filterMode switch
        {
            "Median" => "Center = median(3x3 values) = " + filterEvaluation.Result.ToString(CultureInfo.InvariantCulture),
            "Sharpen" => "Center = clamp(center x 5 - up - left - right - down) = " + filterEvaluation.Result.ToString(CultureInfo.InvariantCulture),
            _ => "Center = average(3x3 values) = " + filterEvaluation.Result.ToString(CultureInfo.InvariantCulture)
        };

        internal string FilterMeaningText => filterMode switch
        {
            "Median" => "미디언은 정렬한 값의 가운데 값을 사용합니다. 작은 먼지나 점 노이즈처럼 튀는 값을 줄일 때 유리합니다.",
            "Sharpen" => "샤프닝은 중심 픽셀과 주변 픽셀 차이를 키웁니다. 흐릿한 경계를 강조하지만 노이즈도 같이 커질 수 있습니다.",
            _ => "평균 블러는 주변 값을 평균내서 부드럽게 만듭니다. 랜덤 노이즈는 줄지만 경계도 함께 흐려질 수 있습니다."
        };

        internal string FilterAnimationStatusText
        {
            get
            {
                string calculation = filterMode switch
                {
                    "Median" => "Sort: " + string.Join(", ", filterEvaluation.SortedValues),
                    "Sharpen" => "220 x 5 - 58 - 60 - 65 - 62",
                    _ => "Mean: " + filterEvaluation.Sum.ToString(CultureInfo.InvariantCulture) + " / 9"
                };
                string outputMeaning = filterMode switch
                {
                    "Median" => "220 노이즈를 중앙값 " + filterEvaluation.Result.ToString(CultureInfo.InvariantCulture) + "로 바꿉니다.",
                    "Sharpen" => "중심 GV가 " + filterEvaluation.Result.ToString(CultureInfo.InvariantCulture) + "로 커져 경계와 노이즈가 함께 강조됩니다.",
                    _ => "중심 220을 평균 " + filterEvaluation.Result.ToString(CultureInfo.InvariantCulture) + "로 낮춰 밝은 노이즈를 완화합니다."
                };
                return FilterAnimationStep switch
                {
                    0 => "0 / 3 - Reset: 원본 3x3 GV부터 확인합니다.",
                    1 => "1 / 3 - Kernel: 중심 220 주변의 9개 GV를 수집합니다.",
                    2 => "2 / 3 - Calculate: " + calculation,
                    _ => "3 / 3 - Output: " + outputMeaning
                };
            }
        }

        internal void ConfigureMaximumValue(double maximumValue) => MaximumValue = NormalizeThresholdValue(maximumValue);

        // Refreshes keep the animation stage. The View completes a stage only for the original loaded-control change events.
        internal void UpdateThreshold(double value, bool invert)
        {
            thresholdInverted = invert;
            thresholdEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateThreshold(value, invert, MaximumValue);
        }

        internal void UpdateBrightness(double offset) => brightnessEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateBrightness(offset);

        internal void UpdateArithmetic(string mode)
        {
            arithmeticMode = mode;
            arithmeticEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateArithmetic(mode);
        }

        internal void UpdateFilter(string mode)
        {
            filterMode = mode;
            filterEvaluation = OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateFilter(mode);
        }

        internal void ResetBrightnessAnimation() => BrightnessAnimationStep = 0;
        internal void ResetArithmeticAnimation() => ArithmeticAnimationStep = 0;
        internal void ResetFilterAnimation() => FilterAnimationStep = 0;
        internal void CompleteBrightnessAnimation() => BrightnessAnimationStep = BrightnessAnimationStepCount;
        internal void CompleteArithmeticAnimation() => ArithmeticAnimationStep = ArithmeticAnimationStepCount;
        internal void CompleteFilterAnimation() => FilterAnimationStep = FilterAnimationStepCount;
        internal void AdvanceBrightnessAnimation() => BrightnessAnimationStep = IsBrightnessAnimationComplete ? 1 : BrightnessAnimationStep + 1;
        internal void AdvanceArithmeticAnimation() => ArithmeticAnimationStep = IsArithmeticAnimationComplete ? 1 : ArithmeticAnimationStep + 1;
        internal void AdvanceFilterAnimation() => FilterAnimationStep = IsFilterAnimationComplete ? 1 : FilterAnimationStep + 1;

        internal double NextThresholdAnimationValue(double current)
        {
            double next = current + (thresholdAnimationForward ? 5D : -5D);
            if (next >= 230D)
            {
                next = 230D;
                thresholdAnimationForward = false;
            }
            else if (next <= 25D)
            {
                next = 25D;
                thresholdAnimationForward = true;
            }

            return next;
        }

        internal GrayscaleLearnCell GetThresholdOutputCell(int index)
        {
            int result = thresholdEvaluation.Results[index];
            return new GrayscaleLearnCell(ThresholdSamples[index].ToString(CultureInfo.InvariantCulture) + " -> " + result.ToString(CultureInfo.InvariantCulture),
                result, GrayscaleLearnCellRole.Gray, GrayscaleLearnCellRole.Default);
        }

        internal GrayscaleLearnCell GetBrightnessOutputCell(int index)
        {
            int result = brightnessEvaluation.Results[index];
            bool visible = BrightnessAnimationStep >= 2;
            return new GrayscaleLearnCell(visible ? BrightnessSamples[index].ToString(CultureInfo.InvariantCulture) + " -> " + result.ToString(CultureInfo.InvariantCulture) : "-",
                result, visible ? GrayscaleLearnCellRole.Gray : GrayscaleLearnCellRole.Neutral,
                visible ? (BrightnessAnimationStep >= 3 ? GrayscaleLearnCellRole.Pass : GrayscaleLearnCellRole.Candidate) : GrayscaleLearnCellRole.Default);
        }

        internal string GetBrightnessHistogramLabel(int index)
        {
            int low = index * 32;
            int high = index == 7 ? 255 : low + 31;
            return low.ToString(CultureInfo.InvariantCulture) + "-" + high.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + (ShowBrightnessHistogram ? brightnessEvaluation.HistogramBins[index].ToString(CultureInfo.InvariantCulture) : "-");
        }

        internal GrayscaleLearnCell GetArithmeticOutputCell(int index)
        {
            int result = arithmeticEvaluation.Results[index];
            bool visible = ArithmeticAnimationStep >= 2;
            return new GrayscaleLearnCell(visible ? result.ToString(CultureInfo.InvariantCulture) : "-",
                result, visible ? GrayscaleLearnCellRole.Gray : GrayscaleLearnCellRole.Neutral,
                visible ? (ArithmeticAnimationStep >= 3 ? GrayscaleLearnCellRole.Pass : GrayscaleLearnCellRole.Candidate) : GrayscaleLearnCellRole.Default);
        }

        internal GrayscaleLearnCellRole GetFilterInputBorderRole(int index) => FilterAnimationStep == 2 && index == 4 ? GrayscaleLearnCellRole.Warning
            : FilterAnimationStep == 1 ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Default;

        internal GrayscaleLearnCell GetFilterOutputCell(int index)
        {
            bool isCenter = index == 4;
            bool calculating = FilterAnimationStep == 2;
            bool complete = FilterAnimationStep >= 3;
            int result = isCenter ? filterEvaluation.Result : FilterSamples[index];
            return new GrayscaleLearnCell(complete ? result.ToString(CultureInfo.InvariantCulture) : calculating && isCenter ? "..." : "-",
                result, complete ? GrayscaleLearnCellRole.Gray : calculating && isCenter ? GrayscaleLearnCellRole.Candidate : GrayscaleLearnCellRole.Neutral,
                isCenter && FilterAnimationStep >= 2 ? (complete ? GrayscaleLearnCellRole.Pass : GrayscaleLearnCellRole.Candidate) : GrayscaleLearnCellRole.Default);
        }

        internal static int NormalizeThresholdValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return 0;
            return Math.Max(0, Math.Min(255, (int)Math.Round(value)));
        }

        private static string FormatSigned(int value) => value >= 0 ? "+" + value.ToString(CultureInfo.InvariantCulture) : value.ToString(CultureInfo.InvariantCulture);

        internal static string MeanOpenedTitle => "열림: Mean | 찾을 위치: Mean Type, Min Mean, Max Mean, Input/Output Layer";
        internal static string HistogramOpenedTitle => "열림: Histogram | 찾을 위치: Type, Clip Limit, Tile Grid 또는 Normalize Alpha/Beta";
        internal static string BrightnessOpenedDetail => "입력과 출력 레이어를 확인하고 Preview 또는 Run Review에서 밝기 분포와 결과 지표를 비교하세요.";
        internal static string ArithmeticOpenedTitle => "열림: Arithmetic | 찾을 위치: Input A / Input B / Output Layer, Mode, Arithmetic Type, Input B Source";
        internal static string ArithmeticOpenedDetail => "Arithmetic에서 두 입력 레이어 또는 Constant/Offset 모드를 설정하고 Preview에서 픽셀 연산 결과를 확인하세요.";
        internal static string FilteringOpenedTitle => "열림: Filter | 찾을 위치: Input/Output Layer, Filter Type, Border Type, Kernel Width/Height";
        internal static string FilteringOpenedDetail => "Filter에서 Median Kernel 또는 Bilateral의 Diameter/Sigma를 확인하고 Preview를 눌러 입력과 출력 영상을 비교하세요.";
    }
}
