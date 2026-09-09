using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    /// <summary>Owns the fixed acceptance lesson and its stage results, independently of controls and timers.</summary>
    internal sealed class MetricsAcceptanceLearnPresenter
    {
        internal const int AnimationStepCount = 3;
        private const double AverageMin = 0.45D;
        private const double AverageMax = 0.60D;
        private const double RangeMax = 0.10D;
        private const double ValueMax = 0.65D;

        internal IReadOnlyList<double> Samples { get; } = new[] { 0.50D, 0.51D, 0.49D, 0.82D, 0.50D };
        internal int AnimationStep { get; private set; } = AnimationStepCount;
        internal bool IsAnimationComplete => AnimationStep >= AnimationStepCount;
        internal bool ShowsSampleDecision => AnimationStep == AnimationStepCount;
        internal double Average => Samples.Average();
        internal double Range => Samples.Max() - Samples.Min();
        internal double Maximum => Samples.Max();
        private bool AverageOk => Average >= AverageMin && Average <= AverageMax;
        private bool OutlierGateOk => Range <= RangeMax && Maximum <= ValueMax;

        internal string FormulaText => AnimationStep switch
        {
            0 => "Avg 0.45..0.60 | Range <= 0.10 | Max <= 0.65",
            1 => "Samples=5 | 측정값 5개를 모두 확인합니다.",
            2 => "DistanceMmAvg=" + Average.ToString("0.00", CultureInfo.InvariantCulture) + " -> " + (AverageOk ? "OK" : "NG"),
            _ => "Range=" + Range.ToString("0.00", CultureInfo.InvariantCulture)
                + " / Max=" + Maximum.ToString("0.00", CultureInfo.InvariantCulture)
                + " -> " + (OutlierGateOk ? "OK" : "NG")
        };

        internal string AnimationStatusText => AnimationStep switch
        {
            0 => "0 / 3 - 평균, 범위, 최대값 판정 기준을 확인합니다.",
            1 => "1 / 3 - Samples: 5개 측정값을 모두 확인합니다.",
            2 => "2 / 3 - 평균 판정: " + (AverageOk ? "OK" : "NG") + "; 평균만 보면 통과합니다.",
            _ => "3 / 3 - 범위/최대값 판정: " + (OutlierGateOk ? "OK" : "NG")
                + "; 0.82 mm 이상치를 검출합니다."
        };

        internal string GetSampleText(int index)
        {
            return AnimationStep == 0 ? "-" : Samples[index].ToString("0.00", CultureInfo.InvariantCulture);
        }

        internal bool IsOutlier(int index)
        {
            return Samples[index] > ValueMax;
        }

        internal void ResetAnimation()
        {
            AnimationStep = 0;
        }

        internal void AdvanceAnimation()
        {
            AnimationStep = IsAnimationComplete ? 1 : AnimationStep + 1;
        }
    }
}
