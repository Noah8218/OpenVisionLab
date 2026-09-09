using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    /// <summary>Owns the Line lessons' stages and presentation; the existing simulation model owns their calculations.</summary>
    internal sealed class LineLearnPresenter
    {
        internal const int AnimationStepCount = 3;
        private OpenVisionLearnLineSimulationModel.EdgeLineEvaluation edge = OpenVisionLearnLineSimulationModel.EvaluateEdgeLine(80);
        private OpenVisionLearnLineSimulationModel.LineDistanceEvaluation distance = OpenVisionLearnLineSimulationModel.EvaluateLineDistance(0.5);

        internal int EdgeLineAnimationStep { get; private set; } = AnimationStepCount;
        internal int LineDistanceAnimationStep { get; private set; } = AnimationStepCount;
        internal double EdgeThreshold { get; private set; } = 80;
        internal double LineDistanceRangeMaximum { get; private set; } = 0.5;
        internal IReadOnlyList<int> EdgeSampleValues => OpenVisionLearnLineSimulationModel.EdgeSampleValues;
        internal int DistanceSampleCount => OpenVisionLearnLineSimulationModel.DistanceLeftEdges.Count;
        internal bool IsEdgeLineAnimationComplete => EdgeLineAnimationStep >= AnimationStepCount;
        internal bool IsLineDistanceAnimationComplete => LineDistanceAnimationStep >= AnimationStepCount;
        internal string EdgeThresholdText => edge.Threshold.ToString(CultureInfo.InvariantCulture) + " GV";
        internal string EdgeLineFormulaText => "Edge = abs(right GV - left GV) >= "
            + edge.Threshold.ToString(CultureInfo.InvariantCulture) + ", LineRun = "
            + edge.BestRun.ToString(CultureInfo.InvariantCulture) + " px";
        internal string EdgeLineMeaningText => edge.BestRun >= 3
            ? "같은 X 위치에서 Edge 후보가 3 px 이상 이어져 Line 후보로 볼 수 있습니다. 실제 검사는 ROI, 방향, 길이 조건을 같이 둡니다."
            : "Edge 기준이 너무 높으면 후보가 끊겨 Line으로 보기 어렵습니다. 기준값, ROI, 전처리 상태를 순서대로 확인합니다.";
        internal string EdgeLineAnimationStatusText => EdgeLineAnimationStep switch
        {
            0 => "0 / 3 - GV 샘플에서 밝기 변화를 확인합니다.",
            1 => "1 / 3 - Gradient: abs(오른쪽 GV - 왼쪽 GV)",
            2 => "2 / 3 - Edge: strength >= " + edge.Threshold.ToString(CultureInfo.InvariantCulture),
            _ => "3 / 3 - LineRun: best vertical chain = " + edge.BestRun.ToString(CultureInfo.InvariantCulture) + " px"
        };
        internal string LineDistanceRangeMaximumText => LineDistanceRangeMaximum.ToString("0.00", CultureInfo.InvariantCulture) + " px";
        internal string LineDistanceFormulaText => "DistancePxAvg=" + distance.Average.ToString("0.0", CultureInfo.InvariantCulture)
            + ", DistancePxRange=" + distance.Range.ToString(CultureInfo.InvariantCulture)
            + ", DistanceMmAvg=" + distance.AverageMillimeters.ToString("0.000", CultureInfo.InvariantCulture)
            + ", DistanceMmRange=" + distance.RangeMillimeters.ToString("0.000", CultureInfo.InvariantCulture)
            + ", DistanceMmMax=" + distance.MaximumMillimeters.ToString("0.000", CultureInfo.InvariantCulture);
        internal string LineDistanceMeaningText => distance.RangePass
            ? "평균과 줄별 흔들림이 모두 기준 안입니다. 실제 레시피도 DistanceAvg와 DistanceRange를 함께 판정합니다."
            : "평균값만 보면 지나칠 수 있지만 줄별 거리 차이가 큽니다. Range/Max 게이트로 긴 측정선을 NG 처리해야 합니다.";
        internal string LineDistanceAnimationStatusText => LineDistanceAnimationStep switch
        {
            0 => "0 / 3 - 각 스캔선의 왼쪽/오른쪽 edge 쌍을 확인합니다.",
            1 => "1 / 3 - 각 스캔선에서 Gap/Pitch를 측정합니다.",
            2 => "2 / 3 - Average: DistancePxAvg = " + distance.Average.ToString("0.0", CultureInfo.InvariantCulture),
            _ => "3 / 3 - Range 판정: " + (distance.RangePass ? "OK" : "NG") + ", DistancePxRange = "
                + distance.Range.ToString(CultureInfo.InvariantCulture) + (distance.RangePass ? " <= " : " > ")
                + LineDistanceRangeMaximum.ToString("0.00", CultureInfo.InvariantCulture)
        };

        // Refreshing a topic retains its animation frame; only a manual parameter edit reveals the final frame.
        internal void UpdateEdgeThreshold(double value)
        {
            EdgeThreshold = value;
            edge = OpenVisionLearnLineSimulationModel.EvaluateEdgeLine(value);
        }

        internal void UpdateLineDistanceRangeMaximum(double value)
        {
            LineDistanceRangeMaximum = value;
            distance = OpenVisionLearnLineSimulationModel.EvaluateLineDistance(value);
        }

        internal void ResetEdgeLineAnimation() => EdgeLineAnimationStep = 0;
        internal void ResetLineDistanceAnimation() => LineDistanceAnimationStep = 0;
        internal void CompleteEdgeLineAnimation() => EdgeLineAnimationStep = AnimationStepCount;
        internal void CompleteLineDistanceAnimation() => LineDistanceAnimationStep = AnimationStepCount;
        internal void AdvanceEdgeLineAnimation() => EdgeLineAnimationStep = IsEdgeLineAnimationComplete ? 1 : EdgeLineAnimationStep + 1;
        internal void AdvanceLineDistanceAnimation() => LineDistanceAnimationStep = IsLineDistanceAnimationComplete ? 1 : LineDistanceAnimationStep + 1;

        internal LineLearnCell GetEdgeOutputCell(int index)
        {
            if (EdgeLineAnimationStep == 0)
                return new("-", LineLearnCellRole.Neutral);
            int shade = Math.Min(220, edge.Strengths[index] + 35);
            if (EdgeLineAnimationStep == 1 || !edge.Edges[index])
                return new(edge.Strengths[index].ToString(CultureInfo.InvariantCulture), LineLearnCellRole.Gray, shade);
            return IsLineCandidate(index)
                ? new("L", LineLearnCellRole.Pass)
                : new("E", LineLearnCellRole.Candidate);
        }

        internal LineLearnCellRole? GetEdgeInputHighlight(int index)
        {
            if (EdgeLineAnimationStep < 2 || !edge.Edges[index])
                return null;
            return IsLineCandidate(index) ? LineLearnCellRole.Pass : LineLearnCellRole.Candidate;
        }

        private bool IsLineCandidate(int index) => EdgeLineAnimationStep >= 3 && edge.Edges[index]
            && index % 5 == edge.BestColumn && edge.BestRun >= 3;

        internal LineLearnCell GetDistanceInputCell(int row, int column)
        {
            int left = OpenVisionLearnLineSimulationModel.DistanceLeftEdges[row];
            int right = OpenVisionLearnLineSimulationModel.DistanceRightEdges[row];
            if (column == left)
                return new("L", LineLearnCellRole.Candidate);
            if (column == right)
                return new("R", LineLearnCellRole.Pass);
            return column > left && column < right
                ? new("-", LineLearnCellRole.Gap)
                : new("0", LineLearnCellRole.Gray);
        }

        internal LineLearnCell GetDistanceOutputCell(int index) => LineDistanceAnimationStep switch
        {
            0 => new("-", LineLearnCellRole.Neutral),
            1 => new(distance.Distances[index].ToString(CultureInfo.InvariantCulture) + " px", LineLearnCellRole.Candidate),
            2 => new("avg " + distance.Average.ToString("0.0", CultureInfo.InvariantCulture), LineLearnCellRole.Candidate),
            _ => new(distance.Distances[index].ToString(CultureInfo.InvariantCulture) + " px",
                IsDistanceOutlier(index) ? LineLearnCellRole.Outlier : LineLearnCellRole.Pass)
        };

        internal LineLearnCellRole? GetDistanceInputHighlight(int index)
        {
            if (LineDistanceAnimationStep == 0)
                return null;
            return LineDistanceAnimationStep >= 3 && IsDistanceOutlier(index) ? LineLearnCellRole.Outlier : LineLearnCellRole.Candidate;
        }

        private bool IsDistanceOutlier(int index) => !distance.RangePass && distance.Distances[index] == distance.Maximum;

        internal static string EdgeDetectionToolTitle => "열림: Edge Detection | 찾을 위치: Edge Type, Canny Low/High, Canny Aperture, Use L2 Gradient";
        internal static string EdgeDetectionToolDetail => "Edge Detection에서 Canny/Sobel/Scharr/Laplacian을 선택하고 방향과 Kernel 값을 확인한 뒤 Preview에서 에지 영상을 비교하세요.";
        internal static string EdgeLineToolTitle => "열림: Line | 찾을 위치: Purpose, Line A/B, ROI, Edge > Polarity/Direction/Contrast/Thickness";
        internal static string EdgeLineToolDetail => "Line에서 Scan direction/interval과 표시 옵션을 설정하고 Preview에서 검출선의 위치와 방향을 확인하세요.";
        internal static string LineDistanceToolTitle => "열림: Line | 찾을 위치: Purpose > Measure, Line A/B, ROI";
        internal static string LineDistanceToolDetail => "Line의 Purpose를 Measure로 선택하고 Pixel/mm와 edge/scan 값을 설정한 뒤 Preview 또는 Run Review에서 DistanceMmAvg와 DistanceMmRange/Max를 함께 확인하세요.";
    }

    internal enum LineLearnCellRole { Gray, Neutral, Candidate, Pass, Gap, Outlier }

    internal readonly record struct LineLearnCell(string Text, LineLearnCellRole Role, int Gray = 0);
}
