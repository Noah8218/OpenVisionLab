using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class MatchingLearnPresenter
    {
        private const int AnimationStepCount = 3;

        public double Threshold { get; private set; } = 0.85;
        public bool IsEdgeBasedMatching { get; private set; }
        public int AnimationStep { get; private set; } = AnimationStepCount;
        public bool IsAnimationComplete => AnimationStep >= AnimationStepCount;
        public OpenVisionLearnMatchingSimulationModel.TemplateEvaluation Evaluation { get; private set; } =
            OpenVisionLearnMatchingSimulationModel.EvaluateTemplate(0.85);

        public string ThresholdText => Threshold.ToString("0.00", CultureInfo.InvariantCulture);
        public string TemplateMark => IsEdgeBasedMatching ? "E" : "T";
        public string SearchMark => IsEdgeBasedMatching ? "E" : "1";

        public string ConceptTitle => IsEdgeBasedMatching
            ? "EdgeBasedMatching은 밝기보다 edge 형상으로 판정합니다"
            : "Matching은 작은 Template을 찾아 점수로 판정합니다";

        public string ConceptDescription => IsEdgeBasedMatching
            ? "검색 이미지와 기준 Template에서 edge를 추출한 뒤, edge 점이 가장 잘 겹치는 위치를 찾습니다."
            : "Template Matching은 기준 모양을 검색 영역 위로 움직이며 가장 비슷한 위치와 Score를 찾습니다.";

        public string ThresholdDescription => IsEdgeBasedMatching
            ? "Canny 기준이 너무 낮으면 배경 edge가 늘고, 너무 높으면 기준 형상의 edge가 끊겨 Score와 ResultCount가 불안정해집니다."
            : "ScoreThreshold가 낮으면 오검출이 늘고, 너무 높으면 조명이나 작은 흔들림에도 NG가 날 수 있습니다.";

        public string SearchTitle => IsEdgeBasedMatching ? "검색 edge map과 edge Template" : "검색 이미지와 Template";
        public string SearchSubtitle => IsEdgeBasedMatching
            ? "E는 추출 edge, S는 검색 후보, B는 최고 edge score 위치입니다."
            : "T는 기준 Template, B는 최고 점수 위치입니다.";
        public string ScoreTitle => IsEdgeBasedMatching ? "Edge 후보 Score" : "후보 Score";
        public string PlayToolTip => IsEdgeBasedMatching
            ? "edge 추출, 후보 점수 계산, 판정 기준 적용 순서를 보여줍니다."
            : "Template 검색, 후보 점수 계산, 판정 기준 적용 순서를 보여줍니다.";

        public string FormulaText => (IsEdgeBasedMatching ? "EdgeScoreMax=" : "BestScore=")
            + Evaluation.BestScore.ToString("0.00", CultureInfo.InvariantCulture)
            + ", Threshold=" + ThresholdText + ", Result=" + (Evaluation.Pass ? "OK" : "NG");

        public string MeaningText => IsEdgeBasedMatching
            ? Evaluation.Pass
                ? "EdgeScoreMax가 기준 이상입니다. 실제 Run Review에서는 ResultCount와 overlay 위치가 대상 형상에 맞는지도 확인합니다."
                : "Edge score가 부족합니다. Canny 기준, edge Template, ROI, ScoreThreshold 순서로 확인합니다."
            : Evaluation.Pass
                ? "최고 점수가 기준 이상이면 Template 위치 후보로 볼 수 있습니다. 회전/스케일 변화가 크면 EdgeBasedMatching이나 FeatureMatching을 검토합니다."
                : "최고 점수가 기준보다 낮으면 NG입니다. Template, ROI, 조명, ScoreThreshold를 순서대로 확인합니다.";

        public string AnimationStatusText => AnimationStep switch
        {
            0 => IsEdgeBasedMatching
                ? "0 / 3 - 검색 이미지와 edge Template을 확인합니다."
                : "0 / 3 - 검색 이미지와 Template을 확인합니다.",
            1 => IsEdgeBasedMatching
                ? "1 / 3 - 각 후보 위치에서 edge 점의 일치도를 계산합니다."
                : "1 / 3 - 각 Template 후보 위치의 점수를 계산합니다.",
            2 => (IsEdgeBasedMatching ? "2 / 3 - EdgeScoreMax: " : "2 / 3 - BestScore: ")
                + Evaluation.BestScore.ToString("0.00", CultureInfo.InvariantCulture),
            _ => (IsEdgeBasedMatching ? "3 / 3 - Edge score 판정: " : "3 / 3 - Threshold 판정: ")
                + (Evaluation.Pass ? "OK" : "NG") + ", "
                + (IsEdgeBasedMatching ? "EdgeScoreMax " : "BestScore ")
                + Evaluation.BestScore.ToString("0.00", CultureInfo.InvariantCulture)
                + (Evaluation.Pass ? " >= " : " < ") + ThresholdText
        };

        public void Update(double threshold, bool isEdgeBasedMatching)
        {
            Threshold = threshold;
            IsEdgeBasedMatching = isEdgeBasedMatching;
            Evaluation = OpenVisionLearnMatchingSimulationModel.EvaluateTemplate(threshold);
        }

        public bool IsCandidateAccepted(int index) => Evaluation.Scores[index] >= Threshold;

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
