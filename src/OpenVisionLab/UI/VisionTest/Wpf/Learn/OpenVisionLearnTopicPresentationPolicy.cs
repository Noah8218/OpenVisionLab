using System;

namespace OpenVisionLab
{
    [Flags]
    internal enum OpenVisionLearnTopicGuideUpdates
    {
        None = 0,
        Foundation = 1,
        MatChannel = 2,
        Brightness = 4,
        Filtering = 8,
        Morphology = 16,
        Blob = 32,
        Contour = 64,
        EdgeLine = 128,
        LineDistance = 256,
        LayerRecipe = 512,
        MetricsAcceptance = 1024,
        Arithmetic = 2048,
        Geometry = 4096,
        ColorHsv = 8192
    }

    internal readonly struct OpenVisionLearnTopicPresentation
    {
        public OpenVisionLearnTopicPresentation(
            OpenVisionLearnTopicMetadata topic,
            string subtitle,
            bool isPracticeWorkflowExpanded,
            OpenVisionLearnTopicGuideUpdates guideUpdates)
        {
            TopicIndex = topic.Index;
            Title = topic.Title;
            Subtitle = subtitle ?? string.Empty;
            PracticeText = topic.PracticeText;
            IsPracticeWorkflowExpanded = isPracticeWorkflowExpanded;
            GuideUpdates = guideUpdates;
        }

        public OpenVisionLearnTopicIndex TopicIndex { get; }

        public string Title { get; }

        public string Subtitle { get; }

        public string PracticeText { get; }

        public bool IsPracticeWorkflowExpanded { get; }

        public OpenVisionLearnTopicGuideUpdates GuideUpdates { get; }

        public bool ShowColorHsvTopic => TopicIndex == OpenVisionLearnTopicIndex.ColorHsv;

        public bool ShowLayerRecipeTopic => TopicIndex == OpenVisionLearnTopicIndex.LayerRecipe;

        public bool ShowMetricsAcceptanceTopic => TopicIndex == OpenVisionLearnTopicIndex.MetricsAcceptance;

        public bool ShowAnimationLegend => true;
    }

    /// <summary>
    /// Resolves Learn topic metadata into presentation state without owning WPF controls.
    /// The Window applies the state and keeps the existing topic View/Presenter owners.
    /// </summary>
    internal sealed class OpenVisionLearnTopicPresentationPolicy
    {
        public OpenVisionLearnTopicPresentation Resolve(int selectedTopicIndex)
        {
            OpenVisionLearnTopicMetadata topic = OpenVisionLearnTopicCatalog.Resolve(selectedTopicIndex);
            return new OpenVisionLearnTopicPresentation(
                topic,
                ResolveSubtitle(topic.Index),
                IsPracticeWorkflowExpanded(topic.Index),
                ResolveGuideUpdates(topic.Index));
        }

        private static string ResolveSubtitle(OpenVisionLearnTopicIndex topicIndex)
        {
            return topicIndex switch
            {
                OpenVisionLearnTopicIndex.Curriculum => "픽셀과 Mat, 좌표와 ROI를 익힌 뒤 전처리·검출·측정·판정 흐름으로 이어갑니다.",
                OpenVisionLearnTopicIndex.BrightnessAndHistogram => "GV 분포를 보면 조명 변화, 배경 분리, Threshold 기준값 후보를 더 안정적으로 판단할 수 있습니다.",
                OpenVisionLearnTopicIndex.Filtering => "주변 픽셀을 같이 계산해 노이즈를 줄이거나 경계를 강조하고, 다음 검출 도구가 안정적으로 동작하도록 준비합니다.",
                OpenVisionLearnTopicIndex.Morphology => "Threshold로 만든 흰 영역을 줄이거나 키워 작은 노이즈, 구멍, 끊어진 부분을 정리합니다.",
                OpenVisionLearnTopicIndex.Blob => "연결된 흰 영역을 후보로 세고, 면적과 개수 기준으로 OK/NG 판단의 기초 값을 만듭니다.",
                OpenVisionLearnTopicIndex.Contour => "통과한 Blob 후보의 경계를 따라 실제 모양, 위치, 폭/높이를 검토하는 단계입니다.",
                OpenVisionLearnTopicIndex.EdgeDetection => "밝기 차이가 큰 경계를 찾고, 같은 방향으로 이어진 후보를 라인으로 해석하는 단계입니다.",
                OpenVisionLearnTopicIndex.LineDistance => "두 Edge 사이 거리를 여러 줄에서 재고 평균과 흔들림을 함께 판정합니다.",
                OpenVisionLearnTopicIndex.Matching => "기준 Template과 검색 영역을 비교해 최고 Score 위치를 찾고 ScoreThreshold로 판정합니다.",
                OpenVisionLearnTopicIndex.FeatureMatching => "keypoint와 descriptor match를 이용해 회전/크기 변화가 있는 대상 후보를 찾습니다.",
                OpenVisionLearnTopicIndex.LayerRecipe => "각 Step의 InputLayer와 OutputLayer를 따라가며 이미지 처리 순서와 결과 위치를 읽습니다.",
                OpenVisionLearnTopicIndex.MetricsAcceptance => "Good/Bad 샘플의 실제 지표를 OK/NG 기준과 연결하고, 평균이 이상치를 가릴 때 Range/Max 기준을 함께 사용합니다.",
                OpenVisionLearnTopicIndex.Arithmetic => "Add, Subtract, AbsDiff, Bitwise AND/OR로 두 이미지의 차이와 공통 영역을 계산합니다.",
                OpenVisionLearnTopicIndex.GeometryTransform => "RotateScale의 회전각과 배율이 OutputSize, 여백, ROI 좌표에 미치는 영향을 확인합니다.",
                OpenVisionLearnTopicIndex.ColorHsv => "Hue, Saturation, Value를 나누어 원하는 색 영역을 마스크로 분리하고 MaskPixelRatio로 비교합니다.",
                OpenVisionLearnTopicIndex.EdgeBasedMatching => "밝기 무늬보다 Edge 형상과 Score를 기준으로 대상을 찾고 ResultCount와 ScoreMax를 비교합니다.",
                _ => "이미지의 각 픽셀 GV를 기준값과 비교해서 검정/흰색 결과 이미지를 만드는 전처리입니다."
            };
        }

        private static bool IsPracticeWorkflowExpanded(OpenVisionLearnTopicIndex topicIndex)
        {
            return topicIndex is not OpenVisionLearnTopicIndex.Blob
                and not OpenVisionLearnTopicIndex.Matching
                and not OpenVisionLearnTopicIndex.FeatureMatching
                and not OpenVisionLearnTopicIndex.EdgeBasedMatching
                and not OpenVisionLearnTopicIndex.MetricsAcceptance;
        }

        private static OpenVisionLearnTopicGuideUpdates ResolveGuideUpdates(OpenVisionLearnTopicIndex topicIndex)
        {
            return topicIndex switch
            {
                OpenVisionLearnTopicIndex.Curriculum => OpenVisionLearnTopicGuideUpdates.Foundation | OpenVisionLearnTopicGuideUpdates.MatChannel,
                OpenVisionLearnTopicIndex.BrightnessAndHistogram => OpenVisionLearnTopicGuideUpdates.Brightness,
                OpenVisionLearnTopicIndex.Filtering => OpenVisionLearnTopicGuideUpdates.Filtering,
                OpenVisionLearnTopicIndex.Morphology => OpenVisionLearnTopicGuideUpdates.Morphology,
                OpenVisionLearnTopicIndex.Blob => OpenVisionLearnTopicGuideUpdates.Blob,
                OpenVisionLearnTopicIndex.Contour => OpenVisionLearnTopicGuideUpdates.Contour,
                OpenVisionLearnTopicIndex.EdgeDetection => OpenVisionLearnTopicGuideUpdates.EdgeLine,
                OpenVisionLearnTopicIndex.LineDistance => OpenVisionLearnTopicGuideUpdates.LineDistance,
                OpenVisionLearnTopicIndex.LayerRecipe => OpenVisionLearnTopicGuideUpdates.LayerRecipe,
                OpenVisionLearnTopicIndex.MetricsAcceptance => OpenVisionLearnTopicGuideUpdates.MetricsAcceptance,
                OpenVisionLearnTopicIndex.Arithmetic => OpenVisionLearnTopicGuideUpdates.Arithmetic,
                OpenVisionLearnTopicIndex.GeometryTransform => OpenVisionLearnTopicGuideUpdates.Geometry,
                OpenVisionLearnTopicIndex.ColorHsv => OpenVisionLearnTopicGuideUpdates.ColorHsv,
                _ => OpenVisionLearnTopicGuideUpdates.None
            };
        }
    }
}
