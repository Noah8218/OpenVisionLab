using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class FeatureMatchingLearnPresenter
    {
        private const int AnimationStepCount = 3;

        public int AnimationStep { get; private set; } = AnimationStepCount;
        public bool IsAnimationComplete => AnimationStep >= AnimationStepCount;
        public OpenVisionLearnMatchingSimulationModel.FeatureEvaluation Evaluation { get; private set; } =
            OpenVisionLearnMatchingSimulationModel.EvaluateFeatures(4);

        public string RequiredText => Evaluation.Required.ToString(CultureInfo.InvariantCulture);
        public string FormulaText => "GoodMatches="
            + Evaluation.GoodCount.ToString(CultureInfo.InvariantCulture)
            + ", Required=" + RequiredText + ", DescriptorScore>=0.65";
        public string MeaningText => Evaluation.Pass
            ? "Good match가 충분하면 대상 후보로 볼 수 있습니다. 실제 검사는 match 위치 일관성과 결과 overlay를 같이 확인합니다."
            : "Good match 수가 부족하면 NG입니다. 특징점이 적거나 조명/초점/ROI가 흔들린 상태인지 먼저 확인합니다.";

        public string AnimationStatusText => AnimationStep switch
        {
            0 => "0 / 3 - Reference와 Scene 영상을 확인합니다.",
            1 => "1 / 3 - Reference와 Scene에서 반복 검출 가능한 특징점을 찾습니다.",
            2 => "2 / 3 - Descriptor 기준: score >= 0.65인 Good Match " + Evaluation.GoodCount.ToString(CultureInfo.InvariantCulture) + "개",
            _ => "3 / 3 - GoodMatches 판정: " + (Evaluation.Pass ? "OK" : "NG")
                + ", 검출 " + Evaluation.GoodCount.ToString(CultureInfo.InvariantCulture) + "개"
                + (Evaluation.Pass ? " >= " : " < ") + RequiredText
                + "; RANSAC과 overlay 위치도 함께 확인합니다."
        };

        public void Update(double requiredValue)
        {
            Evaluation = OpenVisionLearnMatchingSimulationModel.EvaluateFeatures(requiredValue);
        }

        public void ResetAnimation() => AnimationStep = 0;

        public void ShowResult() => AnimationStep = AnimationStepCount;

        public void AdvanceAnimation()
        {
            if (IsAnimationComplete)
            {
                AnimationStep = 0;
            }

            AnimationStep++;
        }
    }
}
