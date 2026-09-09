using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    /// <summary>Owns the fixed binary-region lessons and their decisions; existing simulation algorithms remain authoritative.</summary>
    internal sealed class BinaryLearnPresenter
    {
        internal enum CellKind { Background, Pending, Rejected, Candidate, Accepted, Region, Contour, Box }

        internal const int ContourAnimationStepCount = 3;
        private readonly int[] morphologySampleValues =
        {
            0, 0, 0, 0, 0,
            0, 255, 255, 255, 0,
            0, 255, 255, 255, 0,
            0, 255, 255, 255, 0,
            255, 0, 0, 0, 0
        };
        private readonly int[] blobSampleValues =
        {
            0, 0, 0, 0, 0, 0,
            0, 255, 255, 0, 255, 0,
            0, 255, 255, 0, 255, 255,
            0, 0, 0, 0, 255, 255,
            255, 0, 0, 0, 0, 0
        };
        private readonly int[] contourSampleValues =
        {
            0, 0, 0, 0, 0, 0, 0,
            0, 255, 255, 255, 255, 0, 0,
            0, 255, 255, 0, 255, 0, 0,
            0, 255, 255, 255, 255, 0, 0,
            0, 0, 0, 0, 0, 0, 255
        };
        private readonly int[] blobLabels;
        private readonly int[] blobAreas;
        private readonly int[] contourLabels;
        private readonly bool[] acceptedContourRegion;
        private readonly bool[] contourPixels;
        private readonly (int MinX, int MinY, int MaxX, int MaxY)? contourBounds;
        private bool[] morphologyResult;
        private string morphologyMode = "Erosion";
        private string contourMode = "Contour";
        private int blobMinimumArea = 3;

        internal BinaryLearnPresenter()
        {
            morphologyResult = OpenVisionLearnBinarySimulationModel.CalculateMorphology(morphologySampleValues, 5, 5, morphologyMode);
            (blobLabels, blobAreas) = OpenVisionLearnBinarySimulationModel.LabelConnectedBlobs(blobSampleValues, 6, 5);
            (int[] labels, int[] areas) = OpenVisionLearnBinarySimulationModel.LabelConnectedBlobs(contourSampleValues, 7, 5);
            contourLabels = labels;
            acceptedContourRegion = labels.Select(label => label > 0 && areas[label - 1] >= 4).ToArray();
            contourPixels = OpenVisionLearnBinarySimulationModel.FindContourPixels(acceptedContourRegion, 7, 5);
            contourBounds = OpenVisionLearnBinarySimulationModel.FindBounds(acceptedContourRegion, 7, 5);
            BlobAnimationStep = blobAreas.Length;
        }

        internal IReadOnlyList<int> MorphologySamples => morphologySampleValues;
        internal IReadOnlyList<int> BlobSamples => blobSampleValues;
        internal IReadOnlyList<int> ContourSamples => contourSampleValues;
        internal IReadOnlyList<bool> MorphologyResult => morphologyResult;
        internal IReadOnlyList<int> BlobLabels => blobLabels;
        internal IReadOnlyList<int> BlobAreas => blobAreas;
        internal int MorphologyAnimationStep { get; private set; } = 25;
        internal int BlobAnimationStep { get; private set; }
        internal int ContourAnimationStep { get; private set; } = ContourAnimationStepCount;
        internal bool IsMorphologyAnimationComplete => MorphologyAnimationStep >= morphologySampleValues.Length;
        internal bool IsBlobAnimationComplete => BlobAnimationStep >= blobAreas.Length;
        internal bool IsContourAnimationComplete => ContourAnimationStep >= ContourAnimationStepCount;
        internal int ActiveMorphologyCellIndex => MorphologyAnimationStep > 0 && !IsMorphologyAnimationComplete ? MorphologyAnimationStep - 1 : -1;
        internal int VisibleBlobCount => Math.Max(0, Math.Min(BlobAnimationStep, blobAreas.Length));
        internal int BlobCandidateCount => blobAreas.Length;
        internal int BlobMinimumArea => blobMinimumArea;
        internal int AcceptedBlobCount => blobAreas.Count(area => area >= blobMinimumArea);

        internal string MorphologyFormulaText => morphologyMode switch
        {
            "Dilation" => "Dilation: 3x3 이웃 중 하나라도 흰색이면 결과를 흰색으로 만듭니다.",
            "Opening" => "Opening: Erosion 후 Dilation을 적용합니다.",
            "Closing" => "Closing: Dilation 후 Erosion을 적용합니다.",
            _ => "Erosion: 3x3 이웃이 모두 흰색일 때만 결과를 흰색으로 만듭니다."
        };

        internal string MorphologyMeaningText => morphologyMode switch
        {
            "Dilation" => "팽창은 흰 영역을 키웁니다. 끊어진 부분이나 작은 구멍을 메우는 데 도움이 되지만 대상이 두꺼워질 수 있습니다.",
            "Opening" => "열기는 침식 후 팽창입니다. 작은 흰 점 노이즈를 지운 뒤 원래 크기에 가깝게 되돌릴 때 씁니다.",
            "Closing" => "닫기는 팽창 후 침식입니다. 작은 검은 구멍이나 끊어진 틈을 메우는 데 씁니다.",
            _ => "침식은 흰 영역을 줄입니다. 작은 흰 점 노이즈를 제거하지만 얇은 대상은 사라질 수 있습니다."
        };

        internal string MorphologyAnimationStatusText => ActiveMorphologyCellIndex >= 0
            ? $"커널 중심 ({ActiveMorphologyCellIndex % 5 + 1}, {ActiveMorphologyCellIndex / 5 + 1}) · {MorphologyAnimationStep} / {morphologyResult.Length}"
            : MorphologyAnimationStep == 0 ? "준비 · 외곽 밖은 검정(0)으로 계산" : "전체 결과 · Play 또는 Step으로 커널 이동 확인";

        internal string BlobMinimumAreaText => blobMinimumArea.ToString(CultureInfo.InvariantCulture) + " px";
        internal string BlobFormulaText => "ResultCount = " + AcceptedBlobCount.ToString(CultureInfo.InvariantCulture)
            + " / Areas: " + string.Join(", ", blobAreas.Select((area, index) => ((char)('A' + index)).ToString() + "=" + area.ToString(CultureInfo.InvariantCulture)));
        internal string BlobMeaningText => "MIN_AREA보다 작은 후보는 먼지나 노이즈로 보고 제외합니다. 지금 설정에서는 면적 "
            + blobMinimumArea.ToString(CultureInfo.InvariantCulture) + " px 이상인 Blob만 통과합니다.";

        internal string BlobAnimationStatusText
        {
            get
            {
                int visibleCount = VisibleBlobCount;
                if (blobAreas.Length == 0)
                    return "Blob 후보 없음";
                if (visibleCount == 0)
                    return "초기 상태 · 후보 " + blobAreas.Length.ToString(CultureInfo.InvariantCulture) + "개를 차례로 확인합니다.";
                if (visibleCount >= blobAreas.Length)
                    return "완료 · 통과 " + AcceptedBlobCount.ToString(CultureInfo.InvariantCulture) + " / 전체 " + blobAreas.Length.ToString(CultureInfo.InvariantCulture);
                int area = blobAreas[visibleCount - 1];
                return "후보 " + ((char)('A' + visibleCount - 1)).ToString() + " · 면적 " + area.ToString(CultureInfo.InvariantCulture)
                    + " px · " + (area >= blobMinimumArea ? "통과" : "제외") + " · " + visibleCount.ToString(CultureInfo.InvariantCulture)
                    + " / " + blobAreas.Length.ToString(CultureInfo.InvariantCulture);
            }
        }

        internal string ContourFormulaText => contourMode switch
        {
            "Bounding box" when contourBounds.HasValue => "BoundingBox = x" + contourBounds.Value.MinX.ToString(CultureInfo.InvariantCulture)
                + ", y" + contourBounds.Value.MinY.ToString(CultureInfo.InvariantCulture)
                + ", w" + (contourBounds.Value.MaxX - contourBounds.Value.MinX + 1).ToString(CultureInfo.InvariantCulture)
                + ", h" + (contourBounds.Value.MaxY - contourBounds.Value.MinY + 1).ToString(CultureInfo.InvariantCulture),
            "Contour + box" => "Contour pixels = " + contourPixels.Count(item => item).ToString(CultureInfo.InvariantCulture) + ", with BoundingBox",
            _ => "Contour pixels = " + contourPixels.Count(item => item).ToString(CultureInfo.InvariantCulture)
        };

        internal string ContourMeaningText => contourMode switch
        {
            "Bounding box" => "Bounding box는 후보의 대략 위치와 폭/높이를 빠르게 봅니다. 실제 모양 결함은 외곽선과 같이 확인해야 합니다.",
            "Contour + box" => "외곽선과 박스를 같이 보면 모양 결함과 크기/위치 차이를 한 화면에서 비교할 수 있습니다.",
            _ => "Contour는 통과한 후보의 경계 픽셀만 강조합니다. 끊김, 찌그러짐, 튀어나온 부분을 볼 때 유리합니다."
        };

        internal string ContourAnimationStatusText => ContourAnimationStep switch
        {
            0 => "0 / 3 - 이진 입력에서 연결 영역을 확인합니다.",
            1 => "1 / 3 - 면적 기준으로 작은 연결 영역을 제외합니다.",
            2 => "2 / 3 - 통과한 영역의 경계 픽셀을 표시합니다.",
            _ => "3 / 3 - 표시 방식: " + contourMode
        };

        // A Morphology topic/mode refresh shows the complete result; Blob and Contour refreshes retain their stage.
        internal void UpdateMorphology(string mode)
        {
            SynchronizeMorphologyMode(mode);
            MorphologyAnimationStep = morphologySampleValues.Length;
        }

        internal void SynchronizeMorphologyMode(string mode)
        {
            morphologyMode = mode;
            morphologyResult = OpenVisionLearnBinarySimulationModel.CalculateMorphology(morphologySampleValues, 5, 5, mode);
        }

        internal void UpdateBlob(double minimumArea) => blobMinimumArea = Math.Max(1, (int)Math.Round(minimumArea));
        internal void UpdateContour(string mode) => contourMode = mode;
        internal void CompleteContourAnimation() => ContourAnimationStep = ContourAnimationStepCount;
        internal void ResetMorphologyAnimation() => MorphologyAnimationStep = 0;
        internal void ResetBlobAnimation() => BlobAnimationStep = 0;
        internal void ResetContourAnimation() => ContourAnimationStep = 0;
        internal void AdvanceMorphologyAnimation() => MorphologyAnimationStep = IsMorphologyAnimationComplete ? 1 : MorphologyAnimationStep + 1;
        internal void AdvanceBlobAnimation() => BlobAnimationStep = blobAreas.Length == 0 ? 0 : IsBlobAnimationComplete ? 1 : BlobAnimationStep + 1;
        internal void AdvanceContourAnimation() => ContourAnimationStep = IsContourAnimationComplete ? 1 : ContourAnimationStep + 1;

        internal bool IsMorphologyCellProcessed(int index) => index < MorphologyAnimationStep;

        internal CellKind GetBlobCellKind(int index)
        {
            int label = blobLabels[index];
            if (label == 0) return CellKind.Background;
            if (label > VisibleBlobCount) return CellKind.Pending;
            if (blobAreas[label - 1] < blobMinimumArea) return CellKind.Rejected;
            return label == 1 ? CellKind.Candidate : CellKind.Accepted;
        }

        internal string GetBlobCellText(int index) => GetBlobCellKind(index) switch
        {
            CellKind.Background => "0",
            CellKind.Pending => ".",
            CellKind.Rejected => "x",
            _ => ((char)('A' + blobLabels[index] - 1)).ToString()
        };

        internal bool IsContourInputHighlighted(int index) => contourLabels[index] > 0 && ContourAnimationStep > 0;
        internal bool IsContourRegionAccepted(int index) => acceptedContourRegion[index];

        internal CellKind GetContourCellKind(int index)
        {
            if (ContourAnimationStep == 0) return CellKind.Background;
            if (contourLabels[index] > 0 && !acceptedContourRegion[index]) return CellKind.Rejected;
            if (!acceptedContourRegion[index]) return CellKind.Background;
            if (ContourAnimationStep == 1) return CellKind.Region;
            if (ContourAnimationStep == 2) return contourPixels[index] ? CellKind.Contour : CellKind.Region;

            bool drawContour = contourMode == "Contour" || contourMode == "Contour + box";
            bool drawBox = contourMode == "Bounding box" || contourMode == "Contour + box";
            bool isBox = contourBounds.HasValue && OpenVisionLearnBinarySimulationModel.IsOnBounds(index, 7, contourBounds.Value);
            if (contourMode == "Contour + box" && drawBox && isBox) return CellKind.Box;
            if (drawContour && contourPixels[index]) return CellKind.Contour;
            return drawBox && isBox ? CellKind.Box : CellKind.Region;
        }

        internal string GetContourCellText(int index) => GetContourCellKind(index) switch
        {
            CellKind.Background => "0",
            CellKind.Rejected => "x",
            CellKind.Contour => "C",
            CellKind.Box => "B",
            _ => "1"
        };

        internal static string MorphologyOpenedTitle => "열림: Morphology | 찾을 위치: Input/Output Layer, Operation, Kernel Width/Height, Shape";
        internal static string MorphologyOpenedDetail => "Morphology에서 Kernel 프리셋과 Shape를 확인하고 Preview를 눌러 연산 전후의 형상 변화를 비교하세요.";
        internal static string BlobOpenedTitle => "열림: Blob | PropertyGrid: Use ROI / ROI, Blob Parameter > Min area / Max area";
        internal static string BlobOpenedDetail => "Blob에서 ROI와 면적 범위를 설정하고 Preview 또는 Run Review에서 ResultCount, AreaMin/AreaMax, BoundsWidth/BoundsHeight를 확인하세요.";
        internal static string ContourOpenedTitle => "열림: Contour | PropertyGrid: Contour > 컨투어 표시, Retrieval mode, Min area / Max area";
        internal static string ContourOpenedDetail => "Contour에서 검색 방식과 면적 범위를 설정하고 Preview 또는 Run Review에서 ResultCount, AreaMax, BoundsWidthMax, BoundsHeightMax를 확인하세요.";
    }
}
